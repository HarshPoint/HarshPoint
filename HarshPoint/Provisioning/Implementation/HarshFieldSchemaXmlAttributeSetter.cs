using System;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class HarshFieldSchemaXmlAttributeSetter : HarshFieldSchemaXmlTransformer
    {
        public HarshFieldSchemaXmlAttributeSetter(Expression<Func<Object>> valueAccessorExpr)
        {
            if (valueAccessorExpr == null)
            {
                throw Error.ArgumentNull(nameof(valueAccessorExpr));
            }

            PropertyName = valueAccessorExpr.GetMemberName();
            ValueAccessor = valueAccessorExpr.Compile();
            Name = PropertyName;
        }

        public XName Name
        {
            get;
            set;
        }

        public String PropertyName
        {
            get;
            private set;
        }

        public Func<Object> ValueAccessor
        {
            get;
            set;
        }

        public Action<String, Object> ValueValidator
        {
            get;
            set;
        }

        public override XElement Transform(XElement element)
        {
            if (element == null)
            {
                throw Error.ArgumentNull("element");
            }

            var value = ValueAccessor();

            if (ValueValidator != null)
            {
                ValueValidator(PropertyName, value);
            }

            if (value != null)
            {
                element.SetAttributeValue(Name, value);
            }

            return element;
        }
    }
}
