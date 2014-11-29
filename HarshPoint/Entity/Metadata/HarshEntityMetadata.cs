using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Entity.Metadata
{
    public abstract class HarshEntityMetadata
    {
        internal HarshEntityMetadata(Type entityType)
        {
            if (entityType == null)
            {
                throw Error.ArgumentNull("entityType");
            }

            EntityType = entityType;
        }

        public Type EntityType
        {
            get;
            private set;
        }
    }
}
