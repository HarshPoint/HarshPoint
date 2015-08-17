using HarshPoint.ObjectModel;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ClientObjectUpdater<TProvisioner, TClientObject>
        where TProvisioner : HarshProvisioner
        where TClientObject : ClientObject
    {
        private ImmutableDictionary<PropertyAccessor, PropertyAccessor> _setters
            = ImmutableDictionary<PropertyAccessor, PropertyAccessor>.Empty;

        public ClientObjectUpdater(HarshObjectMetadata metadata)
        {
            if (metadata == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(metadata));
            }

            Metadata = metadata;
        }

        private HarshObjectMetadata Metadata { get; }

        public void Map<T>(
            Expression<Func<TClientObject, T>> clientObjectProperty,
            Expression<Func<TProvisioner, T>> provisionerProperty
        )
        {
            if (provisionerProperty == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisionerProperty));
            }

            if (clientObjectProperty == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(clientObjectProperty));
            }

            var provisionerAccessor = Metadata.GetPropertyAccessor(
                provisionerProperty.ExtractLastPropertyAccess()
            );

            var clientObjectAccessor = new PropertyAccessor(
                clientObjectProperty.ExtractLastPropertyAccess()
            );

            _setters = _setters.Add(provisionerAccessor, clientObjectAccessor);
        }

        public Boolean Update(TClientObject clientObject, TProvisioner provisioner)
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
            TClientObject clientObject,
            TProvisioner provisioner,
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
                       where !p.HasDefaultValue(provisioner)
                       let clientObjectAccessor = _setters[p.PropertyAccessor]
                       let value = clientObjectAccessor.GetValue(clientObject)
                       select new
                       {
                           ParameterValue = p.GetValue(provisioner),
                           ClientObjectAccessor = clientObjectAccessor,
                           ClientObjectValue = value
                       };

            var changed = false;

            foreach (var m in maps)
            {
                if (!Equals(m.ParameterValue, m.ClientObjectValue))
                {
                    changed = true;

                    m.ClientObjectAccessor.SetValue(
                        clientObject,
                        m.ParameterValue
                    );
                }
            }

            return changed;
        }

        public Expression<Func<TClientObject, Object>>[] GetRetrievals()
            => _setters.Values
                .Select(p => p.Expression)
                .Cast<Expression<Func<TClientObject, Object>>>()
                .ToArray();

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ClientObjectUpdater<,>));
    }
}
