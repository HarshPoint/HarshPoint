using HarshPoint.Provisioning;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace HarshPoint.Entity.Metadata
{
    public sealed class HarshFieldMetadata 
    {
        internal HarshFieldMetadata(PropertyInfo definitionProperty, FieldAttribute fieldAttribute)
        {
            if (definitionProperty == null)
            {
                throw Logger.Fatal.ArgumentNull("definitionProperty");
            }

            if (fieldAttribute == null)
            {
                throw Logger.Fatal.ArgumentNull("fieldAttribute");
            }

            InitializeFromDefinition(definitionProperty, fieldAttribute);
        }

        public Guid FieldId
        {
            get;
            private set;
        }

        public String InternalName
        {
            get;
            private set;
        }

        public String StaticName
        {
            get;
            private set;
        }

        public IEnumerable<HarshProvisioner> CreateProvisioners()
        {
            yield return new HarshField()
            {
                Id = FieldId,
                InternalName = InternalName,
                StaticName = StaticName
            };
        }

        private void InitializeFromDefinition(PropertyInfo definitionProperty, FieldAttribute fieldAttribute)
        {
            FieldId = fieldAttribute.FieldId;
            InternalName = definitionProperty.Name;
            StaticName = InternalName;
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshFieldMetadata>();
    }
}
