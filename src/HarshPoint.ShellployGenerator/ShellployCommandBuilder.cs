using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

namespace HarshPoint.ShellployGenerator
{
    internal sealed class ShellployCommandBuilder<TProvisioner> : IShellployCommandBuilder
        where TProvisioner : HarshProvisionerBase
    {
        private Expression<Func<TProvisioner, object>>[] _positionalParameters;
        private Type _parentProvisionerType;
        private Boolean _hasChildren;
        private String _namespace;

        public ShellployCommandBuilder<TProvisioner> AsChildOf<TParentProvisioner>()
            where TParentProvisioner : HarshProvisionerBase
        {
            _parentProvisionerType = typeof(TParentProvisioner);
            return this;
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
            _positionalParameters = parameters;
            return this;
        }

        public ShellployCommandBuilder<TProvisioner> InNamespace(string commandNamespace)
        {
            _namespace = commandNamespace;
            return this;
        }

        public ShellployCommand ToCommand()
        {
            var type = typeof(TProvisioner);
            var verb = VerbsCommon.New;
            var noun = type.Name;

            _positionalParameters = _positionalParameters ?? new Expression<Func<TProvisioner, object>>[] { };

            var provisionerMetadata = new HarshProvisionerMetadata(type);

            var childrenParameterNameArray = new String[] { };
            if (_hasChildren)
            {
                if (provisionerMetadata.Parameters.Any(
                    p => p.Name == ShellployCommand.ChildrenPropertyName
                ))
                {
                    throw Logger.Fatal.InvalidOperationFormat(
                        SR.ShellployCommandBuilder_PropertyChildrenAlreadyDefined,
                        ShellployCommand.ChildrenPropertyName
                    );
                }

                childrenParameterNameArray = new String[] { ShellployCommand.ChildrenPropertyName };
            }

            var positionalParametersIndices = _positionalParameters
                .Select(
                    p => p.ExtractSinglePropertyAccess().Name
                )
                .Concat(childrenParameterNameArray)
                .Select(
                    (name, index) => Tuple.Create(name, index)
                )
                .ToImmutableDictionary(
                    tuple => tuple.Item1,
                    tuple => (Int32?)tuple.Item2
                );

            var childrenPropertyArray = new ShellployCommandProperty[] { };
            if (_hasChildren)
            {
                childrenPropertyArray = new ShellployCommandProperty[]
                {
                    new ShellployCommandProperty{
                        Name = ShellployCommand.ChildrenPropertyName,
                        Type = typeof(ScriptBlock),
                        SkipAssignment = true,
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
            var properties = provisionerMetadata.Parameters
                .GroupBy(
                    param => param.Name,
                    (key, group) => new ShellployCommandProperty()
                    {
                        Name = key,
                        Type = group.First().PropertyType,
                        ParameterAttributes = group
                            .DistinctBy(prop => prop.ParameterSetName)
                            .Select(prop => new ShellployCommandPropertyParameterAttribute()
                            {
                                Mandatory = prop.IsMandatory,
                                ParameterSet = prop.ParameterSetName,
                                Position = positionalParametersIndices?.GetValueOrDefault(prop.Name, null),
                            })
                            .ToImmutableArray(),
                    }
                )
                .Concat(childrenPropertyArray)
                .ToImmutableArray();

            return new ShellployCommand
            {
                ProvisionerType = type,
                ContextType = provisionerMetadata.ContextType,
                ParentProvisionerType = _parentProvisionerType,
                Namespace = _namespace,
                Properties = properties,
                HasChildren = _hasChildren,
                Verb = verb,
                Noun = noun,
                ClassName = $"{verb}{noun}Command",
            };
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ShellployCommandBuilder<>));
    }
}