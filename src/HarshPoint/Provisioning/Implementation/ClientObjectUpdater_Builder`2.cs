using HarshPoint.ObjectModel;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    partial class ClientObjectUpdater
    {
        public struct Builder<TProvisioner, TClientObject>
            where TProvisioner : HarshProvisioner
            where TClientObject : ClientObject
        {
            private Builder(
                HarshObjectMetadata metadata,
                ImmutableDictionary<PropertyAccessor, PropertyAccessor> mappings
            )
            {
                Metadata = metadata;
                Mappings = mappings;
            }

            public Builder<TProvisioner, TClientObject> Map<T>(
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

                var metadata = Metadata ??
                    HarshProvisionerMetadataRepository.Get(
                        typeof(TProvisioner)
                    );

                var provisionerPropertyInfo =
                    provisionerProperty.ExtractLastPropertyAccess();

                if (!IsParameter(provisionerPropertyInfo))
                {
                    throw Logger.Fatal.ArgumentFormat(
                        nameof(provisionerProperty),
                        SR.ClientObjectUpdater_NotAParameter,
                        provisionerPropertyInfo
                    );
                }

                var provisionerAccessor = metadata.GetPropertyAccessor(
                    provisionerPropertyInfo
                );

                var clientObjectAccessor = new PropertyAccessor(
                    clientObjectProperty.ExtractLastPropertyAccess()
                );

                var mappings = Mappings ??
                    ImmutableDictionary<PropertyAccessor, PropertyAccessor>
                    .Empty;

                return new Builder<TProvisioner, TClientObject>(
                    metadata,
                    mappings.Add(
                        provisionerAccessor,
                        clientObjectAccessor
                    )
                );
            }

            public ClientObjectUpdater ToClientObjectUpdater()
                => new ClientObjectUpdater(typeof(TClientObject), Mappings);

            private ImmutableDictionary<PropertyAccessor, PropertyAccessor> Mappings
            {
                get;
            }

            private HarshObjectMetadata Metadata
            {
                get;
            }

            private static Boolean IsParameter(PropertyInfo property)
                => property.IsDefined(typeof(ParameterAttribute), inherit: true);

            private static readonly HarshLogger Logger
                = HarshLog.ForContext(typeof(Builder<,>));
        }
    }
}