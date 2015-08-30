using HarshPoint.Provisioning.Records;
using System;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class RecordWriter<TContext, T>
        where TContext : HarshProvisionerContextBase<TContext>
    {
        private readonly HarshProvisionerBase<TContext> _owner;
        private readonly Func<String> _identifierSelector;

        private readonly HarshScopedValue<String> _context
            = new HarshScopedValue<String>();

        internal RecordWriter(
            HarshProvisionerBase<TContext> owner,
            Func<String> identifierSelector
        )
        {
            if (owner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(owner));
            }

            _owner = owner;
            _identifierSelector = identifierSelector;
        }

        public IDisposable EnterContext(String context) 
            => _context.Enter(context);

        public void Added(T @object)
            => Added(null, null, @object);

        public void Added(String context, T @object)
            => Added(context, null, @object);

        public void Added(String context, String identifier, T @object)
            => Write(context, identifier, new ObjectAdded<T>(@object));

        public void AlreadyExists(T @object)
            => AlreadyExists(null, null, @object);

        public void AlreadyExists(String context, T @object)
            => AlreadyExists(context, null, @object);

        public void AlreadyExists(String context, String identifier, T @object)
            => Write(context, identifier, new ObjectAlreadyExists<T>(@object));

        public void DidNotExist()
            => DidNotExist(null, null);

        public void DidNotExist(String context)
            => DidNotExist(context, null);

        public void DidNotExist(String context, String identifier)
            => Write(context, identifier, new ObjectDidNotExist<T>());

        public void PropertyChanged(
            String propertyName,
            T @object,
            Object oldValue,
            Object newValue
        )
            => PropertyChanged(null, propertyName, @object, oldValue, newValue);

        public void PropertyChanged(
            String context,
            String propertyName,
            T @object,
            Object oldValue,
            Object newValue
        )
            => Write(
                context, 
                propertyName, 
                new PropertyChanged<T>(@object, oldValue, newValue)
            );

        public void PropertyUnchanged(
            String propertyName,
            T @object,
            Object oldValue
        )
            => PropertyUnchanged(null, propertyName, @object, oldValue);

        public void PropertyUnchanged(
            String context,
            String propertyName,
            T @object,
            Object oldValue
        )
            => Write(
                context,
                propertyName,
                new PropertyUnchanged<T>(@object, oldValue)
            );

        public void Removed()
            => Removed(null, null);

        public void Removed(String context)
            => Removed(context, null);

        public void Removed(String context, String identifier)
            => Write(context, identifier, new ObjectRemoved<T>());

        private void Write(
            String context,
            String identifier,
            HarshProvisionerRecord record
        )
        {
            if (context == null)
            {
                context = _owner.Context.ToString();
            }

            if (identifier == null && _identifierSelector != null)
            {
                identifier = _identifierSelector();
            }

            record.Context = context;
            record.Identifier = identifier;
            record.Timestamp = DateTimeOffset.Now;

            _owner.Context.Report(record);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(RecordWriter<,>));
    }
}
