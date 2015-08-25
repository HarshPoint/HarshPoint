using System;

namespace HarshPoint.Provisioning.Records
{
    public abstract class IdentifiedRecord : HarshProvisionerRecord
    {
        protected IdentifiedRecord(String identifier)
        {
            if (String.IsNullOrWhiteSpace(identifier))
            {
                throw Logger.Fatal.ArgumentNullOrWhiteSpace(nameof(identifier));
            }

            Identifier = identifier;
        }

        public String Identifier { get; }

        public Boolean ObjectAdded
            => RecordType == HarshProvisionerRecordType.Added;

        public Boolean ObjectRemoved
            => RecordType == HarshProvisionerRecordType.Removed;

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(IdentifiedRecord));
    }
}
