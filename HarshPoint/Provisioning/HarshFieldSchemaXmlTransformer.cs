using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HarshPoint.Provisioning
{
    public abstract class HarshFieldSchemaXmlTransformer
    {
        public Boolean OnFieldAddOnly
        {
            get;
            set;
        }

        public abstract XElement Transform(XElement element);
    }
}
