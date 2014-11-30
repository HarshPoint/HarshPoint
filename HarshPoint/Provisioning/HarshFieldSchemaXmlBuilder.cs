using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace HarshPoint.Provisioning
{
    internal sealed class HarshFieldSchemaXmlBuilder
    {
        public HarshFieldSchemaXmlBuilder()
        {
            Transformers = new Collection<HarshFieldSchemaXmlTransformer>();
        }

        public Collection<HarshFieldSchemaXmlTransformer> Transformers
        {
            get;
            private set;
        }

        public XElement GetExistingSchemaXml(Field field)
        {
            if (field.IsNull())
            {
                return new XElement("Field");
            }

            if (!field.IsPropertyAvailable(f => f.SchemaXmlWithResourceTokens))
            {
                field.Context.Load(field, f => f.SchemaXmlWithResourceTokens);
                field.Context.ExecuteQuery();
            }

            return XElement.Parse(field.SchemaXmlWithResourceTokens);
        }

        public XElement Update(Field field, XElement schemaXml)
        {
            if (schemaXml == null)
            {
                schemaXml = GetExistingSchemaXml(field);
            }

            if (field.IsNull())
            {
                return RunSchemaXmlTransformers(schemaXml, Transformers);
            }

            return RunSchemaXmlTransformers(
                schemaXml,
                Transformers.Where(t => !t.OnFieldAddOnly)
            );
        }

        private XElement RunSchemaXmlTransformers(XElement schemaXml, IEnumerable<HarshFieldSchemaXmlTransformer> transformers)
        {
            foreach (var trans in transformers)
            {
                schemaXml = trans.Transform(schemaXml);
            }

            return schemaXml;
        }
    }
}