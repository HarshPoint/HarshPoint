using HarshPoint.Provisioning.Implementation;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class ShellployCommandBuilder<TProvisioner> : IShellployCommandBuilder
        where TProvisioner : HarshProvisionerBase
    {
        private List<String> _positionalParameters = new List<String>();
        private Dictionary<String, Tuple<Type, ImmutableArray<ShellployCommandPropertyParameterAttribute>>> _customParameters =
            new Dictionary<String, Tuple<Type, ImmutableArray<ShellployCommandPropertyParameterAttribute>>>();
        private Dictionary<String, Object> _fixedParameters = new Dictionary<String, Object>();
        private Dictionary<String, Object> _defaultValues = new Dictionary<String, Object>();
        private HashSet<String> _ignoredParameters = new HashSet<String>();
        private Dictionary<String, String> _renamedParameters = new Dictionary<String, String>();
        private List<String> _usings = new List<String>();
        private IShellployCommandBuilderParent _parentProvisioner;
        private Boolean _hasChildren;
        private String _namespace;

        public ShellployCommandBuilderParent<TProvisioner, TParentProvisioner> AsChildOf<TParentProvisioner>()
            where TParentProvisioner : HarshProvisionerBase
        {
            var result = new ShellployCommandBuilderParent<TProvisioner, TParentProvisioner>(this);
            _parentProvisioner = result;
            return result;
        }

        public ShellployCommandBuilder<TProvisioner> HasChildren()
        {
            _hasChildren = true;
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> AddPositionalParameter(
            Expression<Func<TProvisioner, Object>> parameter
        )
        {
            _positionalParameters.Add(parameter.ExtractLastPropertyAccess().Name);
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> AddPositionalParameter<TParameter>(
            String name,
            params ShellployCommandPropertyParameterAttribute[] parameterAttributes
        )
        {
            AddCustomParameter<TParameter>(name, parameterAttributes);
            _positionalParameters.Add(name);
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> AddNamedParameter<TParameter>(
            String name,
            params ShellployCommandPropertyParameterAttribute[] parameterAttributes
        )
        {
            AddCustomParameter<TParameter>(name, parameterAttributes);
            return this;
        }

        private void AddCustomParameter<TParameter>(
            String name,
            params ShellployCommandPropertyParameterAttribute[] parameterAttributes
        )
        {
            if (!parameterAttributes.Any())
            {
                parameterAttributes = new[]
                {
                    new ShellployCommandPropertyParameterAttribute()
                };
            }

            _customParameters.Add(
                name,
                Tuple.Create(
                    typeof(TParameter),
                    parameterAttributes.ToImmutableArray()
                )
            );
        }

        public ShellployCommandBuilder<TProvisioner> RenameParameter(
            Expression<Func<TProvisioner, Object>> parameter,
            String newName
        )
        {
            _renamedParameters[parameter.ExtractLastPropertyAccess().Name] = newName;
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> SetParameterValue<TValue>(
            Expression<Func<TProvisioner, TValue>> parameter,
            TValue value
        )
        {
            _fixedParameters[parameter.ExtractLastPropertyAccess().Name] = value;
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> SetParameterValue(
            Expression<Func<TProvisioner, Object>> parameter,
            CodeExpression value
        )
        {
            _fixedParameters[parameter.ExtractLastPropertyAccess().Name] = value;
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> SetDefaultParameterValue<TValue>(
            Expression<Func<TProvisioner, TValue>> parameter,
            TValue value
        )
        {
            _defaultValues[parameter.ExtractLastPropertyAccess().Name] = value;
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> SetDefaultValue(
            Expression<Func<TProvisioner, Object>> parameter,
            CodeExpression value
        )
        {
            _defaultValues[parameter.ExtractLastPropertyAccess().Name] = value;
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> SetDefaultValue(
            String parameterName,
            Object value
        )
        {
            _defaultValues[parameterName] = value;
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> IgnoreParameter(
            Expression<Func<TProvisioner, Object>> parameter
        )
        {
            _ignoredParameters.Add(parameter.ExtractLastPropertyAccess().Name);
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> InNamespace(String ns)
        {
            _namespace = ns;
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> AddUsing(String ns)
        {
            if (!_usings.Contains(ns))
            {
                _usings.Add(ns);
            }
            return this;
        }

        public Type ProvisionerType
        {
            get
            {
                return typeof(TProvisioner);
            }
        }

        public ShellployCommand ToCommand()
        {
            return ToCommand(new Dictionary<Type, IShellployCommandBuilder>());
        }

        private HarshProvisionerMetadata _provisionerMetadata
            = new HarshProvisionerMetadata(typeof(TProvisioner));

        private IShellployCommandBuilder GetParentBuilder(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders
        )
        {
            if (_parentProvisioner != null)
            {
                return builders[_parentProvisioner.Type];
            }

            return null;
        }

        public IEnumerable<String> GetPositionalParameters(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders,
            Boolean hasChildren
        )
        {
            var parentPositionalParameters = new String[0];
            var parentBuilder = GetParentBuilder(builders);
            if (parentBuilder != null)
            {
                parentPositionalParameters = parentBuilder.GetPositionalParameters(builders, hasChildren = false).ToArray();
            }

            return parentPositionalParameters
                .Concat(_positionalParameters)
                .Concat(hasChildren ? ShellployCommand.ChildrenPropertyName.AsCollection() : new String[0]);
        }

        public IEnumerable<ShellployCommandProperty> GetProperties(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders,
            IImmutableDictionary<String, Int32?> positionalParametersIndices,
            Boolean hasChildren
        )
        {
            IEnumerable<ShellployCommandProperty> properties = _provisionerMetadata.Parameters
                .GroupBy(
                    param => param.Name,
                    (key, group) => new ShellployCommandProperty()
                    {
                        Name = key,
                        PropertyName = GetPropertyName(key, _renamedParameters),
                        Type = group.First().PropertyType,
                        AssignmentOnType = ProvisionerType,
                        ParameterAttributes = group
                            .DistinctBy(prop => prop.ParameterSetName)
                            .Select(prop => new ShellployCommandPropertyParameterAttribute()
                            {
                                Mandatory = prop.IsMandatory,
                                ParameterSet = prop.ParameterSetName,
                                Position = positionalParametersIndices.GetValueOrDefault(prop.Name, null),
                            })
                            .ToImmutableArray(),
                    }
                )
                .Where(prop => !_ignoredParameters.Contains(prop.Name))
                .ToArray();
            SetFixedParameters(properties, _fixedParameters);

            if (hasChildren
                && _provisionerMetadata.Parameters.Any(
                    p => p.Name == ShellployCommand.ChildrenPropertyName
                )
            )
            {
                throw Logger.Fatal.InvalidOperationFormat(
                    SR.ShellployCommandBuilder_PropertyChildrenAlreadyDefined,
                    ShellployCommand.ChildrenPropertyName
                );
            }

            properties = properties
                .Concat(
                    GetChildrenProperty(positionalParametersIndices, hasChildren)?.AsCollection()
                    ?? new ShellployCommandProperty[0]
                );

            var parentProperties = GetParentProperties(builders, positionalParametersIndices, hasChildren);

            if (
                new HashSet<ShellployCommandProperty>(
                    parentProperties,
                    new HarshEqualityComparer<ShellployCommandProperty, String>(p => p.Name)
                )
                .Overlaps(properties)
            )
            {
                throw Logger.Fatal.InvalidOperation(SR.ShellployCommandBuilder_Overlaps);
            }

            properties = parentProperties
                .Concat(properties);

            var customProperties = _customParameters
                .Select(kvp => new ShellployCommandProperty()
                {
                    Name = kvp.Key,
                    Type = kvp.Value.Item1,
                    AssignmentOnType = ProvisionerType,
                    ParameterAttributes = kvp.Value.Item2
                        .Select(attr =>
                        {
                            attr.Position = positionalParametersIndices.GetValueOrDefault(kvp.Key, null);
                            return attr;
                        })
                        .ToImmutableArray(),
                    Custom = true,
                });

            if (
                new HashSet<ShellployCommandProperty>(
                    customProperties,
                    new HarshEqualityComparer<ShellployCommandProperty, String>(p => p.Name)
                )
                .Overlaps(properties)
            )
            {
                throw Logger.Fatal.InvalidOperation(SR.ShellployCommandBuilder_Overlaps);
            }

            properties = properties
                .Concat(customProperties)
                .ToArray();

            SetDefaultValues(properties, _defaultValues);
            return properties;
        }

        private IEnumerable<ShellployCommandProperty> GetParentProperties(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders,
            IImmutableDictionary<String, Int32?> positionalParametersIndices,
            Boolean hasChildren
        )
        {
            var parentBuilder = GetParentBuilder(builders);
            if (parentBuilder != null)
            {
                var parentProperties = parentBuilder.GetProperties(
                    builders,
                    positionalParametersIndices,
                    hasChildren = false
                )
                .Where(prop => !_parentProvisioner.IgnoredParameters.Contains(prop.Name))
                .ToArray();

                SetFixedParameters(parentProperties, _parentProvisioner.FixedParameters);
                return parentProperties;
            }

            return new ShellployCommandProperty[0];
        }

        private static ShellployCommandProperty GetChildrenProperty(
            IImmutableDictionary<String, Int32?> positionalParametersIndices,
            Boolean hasChildren
        )
        {
            if (hasChildren)
            {
                return new ShellployCommandProperty
                {
                    Name = ShellployCommand.ChildrenPropertyName,
                    Type = typeof(Object),
                    AssignmentOnType = null,
                    ParameterAttributes = new List<ShellployCommandPropertyParameterAttribute>()
                    {
                        new ShellployCommandPropertyParameterAttribute()
                        {
                            Position = positionalParametersIndices.GetValueOrDefault(ShellployCommand.ChildrenPropertyName, null),
                            ValueFromPipeline = true,
                        },
                    }.ToImmutableList(),
                };
            }

            return null;
        }

        private static void SetFixedParameters(
            IEnumerable<ShellployCommandProperty> properties,
            IDictionary<String, Object> fixedParameters
        )
        {
            foreach (var prop in properties)
            {
                Object fixedValue;
                if (fixedParameters.TryGetValue(prop.Name, out fixedValue))
                {
                    prop.UseFixedValue = true;
                    prop.FixedValue = fixedValue;
                }
            }
        }

        private static void SetDefaultValues(
            IEnumerable<ShellployCommandProperty> properties,
            IDictionary<String, Object> defaultParameters
        )
        {
            foreach (var prop in properties)
            {
                Object defaultValue;
                if (defaultParameters.TryGetValue(prop.Name, out defaultValue))
                {
                    if (prop.UseFixedValue)
                    {
                        throw Logger.Fatal.InvalidOperationFormat(SR.ShellployCommandBuilder_Fixed_and_default, prop.Name);
                    }

                    prop.DefaultValue = defaultValue;
                }
            }
        }

        private static String GetPropertyName(String name, Dictionary<String, String> renamedParameters)
        {
            String newName;
            if (renamedParameters.TryGetValue(name, out newName))
            {
                return newName;
            }

            return name;
        }

        public IEnumerable<Type> GetParentProvisionerTypes(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders
        )
        {
            var parentBuilder = GetParentBuilder(builders);
            if (parentBuilder != null)
            {
                return parentBuilder.GetParentProvisionerTypes(builders)
                    .Concat(new[] { parentBuilder.ProvisionerType });
            }

            return new Type[0];
        }

        public ShellployCommand ToCommand(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders
        )
        {
            var positionalParametersIndices
                = GetPositionalParameters(builders, _hasChildren)
                    .Select(
                        (name, index) => Tuple.Create(name, index)
                    )
                    .ToImmutableDictionary(
                        tuple => tuple.Item1,
                        tuple => (Int32?)tuple.Item2
                    );

            var properties = GetProperties(builders, positionalParametersIndices, _hasChildren);

            var verb = VerbsCommon.New;
            var noun = ProvisionerType.Name;
            return new ShellployCommand
            {
                ProvisionerType = ProvisionerType,
                ContextType = _provisionerMetadata.ContextType,
                ParentProvisionerTypes = GetParentProvisionerTypes(builders),
                Namespace = _namespace,
                Usings = _usings.ToImmutableArray(),
                Properties = properties.ToImmutableArray(),
                HasChildren = _hasChildren,
                Verb = Tuple.Create(typeof(VerbsCommon), nameof(VerbsCommon.New)),
                Noun = noun,
                ClassName = $"{verb}{noun}Command",
            };
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ShellployCommandBuilder<>));
    }
}