using System;

namespace HarshPoint.Provisioning
{
    public class HarshProvisionerObjectResultFactory<T, TIdentifier>
    {
        public HarshProvisionerObjectResultFactory()
        {
        }

        public HarshProvisionerObjectResultFactory(Func<T, TIdentifier> identifierSelector)
        {
            IdentifierSelector = identifierSelector;
        }

        public Func<T, TIdentifier> IdentifierSelector
        {
            get;
            set;
        }

        public HarshProvisionerObjectResult<T, TIdentifier> Added(T value)
        {
            if (value == null)
            {
                throw Error.ArgumentNull(nameof(value));
            }

            return Added(value, GetIdentifier(value));
        }

        public HarshProvisionerObjectResult<T, TIdentifier> Added(T value, TIdentifier identifier)
        {
            if (value == null)
            {
                throw Error.ArgumentNull(nameof(value));
            }

            return new HarshProvisionerObjectResult<T, TIdentifier>()
            {
                Identifier = identifier,
                Object = value,
                ObjectAdded = true,
            };
        }

        public HarshProvisionerObjectResult<T, TIdentifier> Removed(TIdentifier identifier)
        {
            if (identifier == null)
            {
                throw Error.ArgumentNull(nameof(identifier));
            }

            return new HarshProvisionerObjectResult<T, TIdentifier>()
            {
                Identifier = identifier,
                ObjectRemoved = true,
            };
        }

        public HarshProvisionerObjectResult<T, TIdentifier> Unchanged(T value)
        {
            if (value == null)
            {
                throw Error.ArgumentNull(nameof(value));
            }

            return Unchanged(value, GetIdentifier(value));
        }

        public HarshProvisionerObjectResult<T, TIdentifier> Unchanged(T value, TIdentifier identifier)
        {
            if (value == null)
            {
                throw Error.ArgumentNull(nameof(value));
            }

            return new HarshProvisionerObjectResult<T, TIdentifier>()
            {
                Identifier = identifier,
                Object = value,
            };
        }

        public HarshProvisionerObjectResult<T, TIdentifier> Updated(T value)
        {
            if (value == null)
            {
                throw Error.ArgumentNull(nameof(value));
            }

            return Updated(value, GetIdentifier(value));
        }

        public HarshProvisionerObjectResult<T, TIdentifier> Updated(T value, TIdentifier identifier)
        {
            if (value == null)
            {
                throw Error.ArgumentNull(nameof(value));
            }

            return new HarshProvisionerObjectResult<T, TIdentifier>()
            {
                Identifier = identifier,
                Object = value,
                ObjectUpdated = true,
            };
        }

        private TIdentifier GetIdentifier(T value)
        {
            if (IdentifierSelector != null)
            {
                return IdentifierSelector(value);
            }

            return default(TIdentifier);
        }
    }
}
