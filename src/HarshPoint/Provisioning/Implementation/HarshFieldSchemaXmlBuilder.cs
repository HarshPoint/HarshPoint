using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    /// <summary>
    /// Controls the field schema XML generation.
    /// </summary>
    internal sealed class HarshFieldSchemaXmlBuilder
    {
        public HarshFieldSchemaXmlBuilder(params HarshFieldSchemaXmlTransformer[] transformers)
        {
            if (transformers == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(transformers));
            }

            Transformers = transformers.ToImmutableArray();
        }

        /// <summary>
        /// Gets the collection of transformers to be run on
        /// the field schema XML.
        /// </summary>
        public IReadOnlyCollection<HarshFieldSchemaXmlTransformer> Transformers
        {
            get; private set;
        }

        public XElement Create()
            => RunSchemaXmlTransformers(
                new XElement("Field"),
                Transformers
            );

        public XElement Update(XElement existingSchemaXml)
        {
            if (existingSchemaXml == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(existingSchemaXml));
            }

            // do not modify the original

            return RunSchemaXmlTransformers(
                new XElement(existingSchemaXml),
                Transformers.Where(t => !t.OnlyOnCreate)
            );
        }

        private static XElement RunSchemaXmlTransformers(
            XElement schemaXml,
            IEnumerable<HarshFieldSchemaXmlTransformer> transformers
        )
            => transformers.Aggregate(
                schemaXml,
                (xml, trans) => trans.Transform(xml)
            );

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshFieldSchemaXmlBuilder>();
    }
}