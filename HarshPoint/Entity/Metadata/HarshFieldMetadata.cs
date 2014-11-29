using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Entity.Metadata
{
    public sealed class HarshFieldMetadata
    {
        internal HarshFieldMetadata(PropertyInfo definitionProperty, FieldAttribute fieldAttribute)
        {
            if (definitionProperty == null)
            {
                throw Error.ArgumentNull("definitionProperty");
            }

            if (fieldAttribute == null)
            {
                throw Error.ArgumentNull("fieldAttribute");
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

        private void InitializeFromDefinition(PropertyInfo definitionProperty, FieldAttribute fieldAttribute)
        {
            FieldId = fieldAttribute.FieldId;
            InternalName = definitionProperty.Name;
            StaticName = InternalName;
        }
    }
}
