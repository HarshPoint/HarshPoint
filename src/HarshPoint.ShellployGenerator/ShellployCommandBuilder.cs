using HarshPoint.Provisioning.Implementation;
using System;
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

            var positionalParametersIndices = _positionalParameters?
                .Select(
                    (p, index) => Tuple.Create(p.ExtractSinglePropertyAccess(), index)
                )
                .ToImmutableDictionary(
                    tuple => tuple.Item1.Name,
                    tuple => (Int32?)tuple.Item2
                );

            var properties = new HarshProvisionerMetadata(type).Parameters
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
                .ToImmutableArray();

            return new ShellployCommand
            {
                ProvisionerType = type,
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