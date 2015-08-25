using HarshPoint.Provisioning.Implementation;
using System;

namespace HarshPoint.Provisioning.Records
{
    public sealed class RecordWriter<TContext>
        where TContext : HarshProvisionerContextBase<TContext>
    {
        private readonly TContext _context;

        internal RecordWriter(TContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            _context = context;
        }

        public void Added<T>(String identifier, T @object)
            => Added(null, identifier, @object);

        public void Added<T>(String context, String identifier, T @object)
            => Write(context, new ObjectAdded<T>(identifier, @object));

        public void AlreadyExists<T>(String identifier, T @object)
            => AlreadyExists(null, identifier, @object);

        public void AlreadyExists<T>(String context, String identifier, T @object)
            => Write(context, new ObjectAlreadyExists<T>(identifier, @object));

        public void DidNotExist(String identifier)
            => DidNotExist(null, identifier);

        public void DidNotExist(String context, String identifier)
            => Write(context, new ObjectDidNotExist(identifier));

        public void Removed(String identifier)
            => Removed(null, identifier);

        public void Removed(String context, String identifier)
            => Write(context, new ObjectRemoved(identifier));

        private void Write(String context, HarshProvisionerRecord record)
        {
            if (context == null)
            {
                context = _context.ToString();
            }

            record.Context = context;
            record.Timestamp = DateTimeOffset.Now;

            _context.Report(record);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(RecordWriter<>));
    }
}
