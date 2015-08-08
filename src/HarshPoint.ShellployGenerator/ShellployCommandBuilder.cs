using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;
using Microsoft.SharePoint.Client;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class ShellployCommandBuilder<TProvisioner> : IShellployCommandBuilder
        where TProvisioner : HarshProvisionerBase
    {
        private IEnumerable<String> _positionalParameters;
        private IShellployCommandBuilderParent _parentProvisioner;
        private Boolean _hasChildren;
        private String _namespace;

        public ShellployCommandBuilderParent<TProvisioner, TParentProvisioner> AsChildOf<TParentProvisioner>()
            where TParentProvisioner : HarshProvisionerBase
        {
            var result = new ShellployCommandBuilderParent<TProvisioner, TParentProvisioner>();
            _parentProvisioner = result;
            return result;
        }

        public ShellployCommandBuilder<TProvisioner> HasChildren()
        {
            _hasChildren = true;
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> PositionalParameters(
            params Expression<Func<TProvisioner, object>>[] parameters
        )
        {
            _positionalParameters = parameters.Select(x => x.ExtractSinglePropertyAccess().Name);
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> InNamespace(string commandNamespace)
        {
            _namespace = commandNamespace;
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
            var childrenParameterNameArray = new String[] { };
            if (hasChildren)
            {
                childrenParameterNameArray = new String[] { ShellployCommand.ChildrenPropertyName };
            }

            var parentPositionalParameters = new String[] { };
            var parentBuilder = GetParentBuilder(builders);
            if (parentBuilder != null)
            {
                parentPositionalParameters = parentBuilder.GetPositionalParameters(builders, hasChildren = false).ToArray();
            }

            return parentPositionalParameters
                .Concat(_positionalParameters ?? new String[] { })
                .Concat(childrenParameterNameArray);
        }

        public IEnumerable<ShellployCommandProperty> GetProperties(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders,
            IImmutableDictionary<String, Int32?> positionalParametersIndices,
            Boolean hasChildren
        )
        {
            var childrenPropertyArray = new ShellployCommandProperty[] { };
            if (_hasChildren)
            {
                childrenPropertyArray = new ShellployCommandProperty[]
                {
                    new ShellployCommandProperty{
                        Name = ShellployCommand.ChildrenPropertyName,
                        Type = typeof(ScriptBlock),
                        AssignmentOnType = null,
                        ParameterAttributes = new List<ShellployCommandPropertyParameterAttribute>()
                        {
                            new ShellployCommandPropertyParameterAttribute()
                            {
                                Position = positionalParametersIndices.GetValueOrDefault(ShellployCommand.ChildrenPropertyName, null),
                            },
                        }.ToImmutableList(),
                    },
                };
            }

            var properties = _provisionerMetadata.Parameters
                .GroupBy(
                    param => param.Name,
                    (key, group) => new ShellployCommandProperty()
                    {
                        Name = key,
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
                );

            var parentProperties = new ShellployCommandProperty[] { };
            var parentBuilder = GetParentBuilder(builders);
            if (parentBuilder != null)
            {
                parentProperties = parentBuilder.GetProperties(
                    builders,
                    positionalParametersIndices,
                    hasChildren = false
                )
                .ToArray();
                foreach (var prop in parentProperties)
                {
                    object fixedValue;
                    if (_parentProvisioner.FixedParameters.TryGetValue(prop.Name, out fixedValue))
                    {
                        prop.UseFixedValue = true;
                        prop.FixedValue = fixedValue;
                    }
                }
            }

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
                .Concat(properties)
                .Concat(childrenPropertyArray);
            return properties;
        }

        public IEnumerable<Type> GetParentProvisionerTypes(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders
        )
        {
            var parentBuilder = GetParentBuilder(builders);
            if (parentBuilder != null)
            {
                return parentBuilder.GetParentProvisionerTypes(builders)
                    .Concat(new Type[] { parentBuilder.ProvisionerType });
            }

            return new Type[] { };
        }

        public ShellployCommand ToCommand(
            IReadOnlyDictionary<Type, IShellployCommandBuilder> builders
        )
        {
            if (_hasChildren)
            {
                if (_provisionerMetadata.Parameters.Any(
                    p => p.Name == ShellployCommand.ChildrenPropertyName
                ))
                {
                    throw Logger.Fatal.InvalidOperationFormat(
                        SR.ShellployCommandBuilder_PropertyChildrenAlreadyDefined,
                        ShellployCommand.ChildrenPropertyName
                    );
                }
            }

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