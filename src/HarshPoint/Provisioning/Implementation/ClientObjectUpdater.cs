using HarshPoint.ObjectModel;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    public partial class ClientObjectUpdater
    {
        private ClientObjectUpdater(
            Type clientObjectType,
            ImmutableDictionary<PropertyAccessor, PropertyAccessor> mappings
        )
        {
            if (mappings == null)
            {
                mappings = ImmutableDictionary
                    .Create<PropertyAccessor, PropertyAccessor>();
            }

            ClientObjectType = clientObjectType;
            Mappings = mappings;
        }

        private Type ClientObjectType { get; }

        private ImmutableDictionary<PropertyAccessor, PropertyAccessor> Mappings
        {
            get;
        }

        public Expression<Func<TClientObject, Object>>[] GetRetrievals<TClientObject>()
            where TClientObject : ClientObject
            => Mappings.Values
                .Select(p => p.Expression)
                .Cast<Expression<Func<TClientObject, Object>>>()
                .ToArray();

        public Boolean Update(
            ClientObject clientObject,
            HarshProvisioner provisioner
        )
        {
            if (provisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisioner));
            }

            if (provisioner.ParameterSet == null)
            {
                throw Logger.Fatal.Argument(
                    nameof(provisioner),
                    SR.ClientObjectUpdater_NoParameterSet
                );
            }

            return Update(
                clientObject,
                provisioner,
                provisioner.ParameterSet.Parameters
            );
        }

        public Boolean Update(
            ClientObject clientObject,
            HarshProvisioner provisioner,
            IEnumerable<Parameter> parameters
        )
        {
            if (clientObject == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(clientObject));
            }

            if (provisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisioner));
            }

            if (parameters == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parameters));
            }

            var maps = from p in parameters
                       where !HasDefaultValue(p, provisioner)

                       let clientObjectAccessor = TryGetClientObjectAccessor(p)
                       where clientObjectAccessor != null

                       let existingValue = clientObjectAccessor.GetValue(clientObject)
                       select new
                       {
                           Parameter = p,
                           ParameterValue = p.GetValue(provisioner),
                           ClientObjectAccessor = clientObjectAccessor,
                           ClientObjectValue = existingValue
                       };

            var changed = false;

            foreach (var m in maps)
            {
                if (Equals(m.ParameterValue, m.ClientObjectValue))
                {
                    Logger.Information(
                        "Parameter {Parameter} value {$ParameterValue} " +
                        "equals to client object {ClientObjectProperty} " +
                        "value {$ClientObjectValue}.",
                        m.Parameter,
                        m.ParameterValue,
                        m.ClientObjectAccessor,
                        m.ClientObjectValue
                    );
                }
                else
                {
                    changed = true;

                    Logger.Information(
                        "Updating client object {ClientObjectProperty}, " +
                        "current value {$ClientObjectValue} does not equal " +
                        "{Parameter} value {$ParameterValue}.",
                        m.ClientObjectAccessor,
                        m.ClientObjectValue,
                        m.Parameter,
                        m.ParameterValue
                    );

                    m.ClientObjectAccessor.SetValue(
                        clientObject,
                        m.ParameterValue
                    );
                }
            }

            return changed;
        }

        private PropertyAccessor TryGetClientObjectAccessor(Parameter parameter)
        {
            var result = Mappings.GetValueOrDefault(parameter.PropertyAccessor);

            if (result == null)
            {
                Logger.Information(
                    "Parameter {Parameter} is not mapped to any {ClientObjectType} property.",
                    parameter,
                    ClientObjectType
                );
            }

            return result;
        }

        public static Builder<TProvisioner, TClientObject> Build<TProvisioner, TClientObject>()
            where TProvisioner : HarshProvisioner
            where TClientObject : ClientObject
            => new Builder<TProvisioner, TClientObject>();

        private static Boolean HasDefaultValue(
            Parameter parameter, HarshProvisioner provisioner
        )
        {
            if (parameter.HasDefaultValue(provisioner))
            {
                Logger.Information(
                    "Parameter {Parameter} has default value, skipping."
                );

                return true;
            }

            return false;
        }

        internal static ClientObjectUpdater Empty { get; }
            = new ClientObjectUpdater(null, null);

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ClientObjectUpdater));
    }
}
