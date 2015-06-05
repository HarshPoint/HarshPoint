using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    /// <summary>
    /// Controls the field schema XML generation.
    /// </summary>
    internal sealed class HarshFieldSchemaXmlBuilder
    {
        public HarshFieldSchemaXmlBuilder()
        {
            Transformers = new Collection<HarshFieldSchemaXmlTransformer>();
        }

        /// <summary>
        /// Gets the collection of transformers to be run on
        /// the field schema XML.
        /// </summary>
        public Collection<HarshFieldSchemaXmlTransformer> Transformers
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the existing field schema XML, if any, or an empty
        /// Field element.
        /// </summary>
        /// <param name="field">The field, may be <c>null</c>.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public async Task<XElement> GetExistingSchemaXml(Field field)
        {
            if (field.IsNull())
            {
                return new XElement("Field");
            }

            if (!field.IsPropertyAvailable(f => f.SchemaXmlWithResourceTokens))
            {
                field.Context.Load(field, f => f.SchemaXmlWithResourceTokens);
                await field.Context.ExecuteQueryAsync();
            }

            return XElement.Parse(field.SchemaXmlWithResourceTokens);
        }

        /// <summary>
        /// Updates the specified field schema using the specified <see cref="Transformers"/>.
        /// </summary>
        /// <param name="field">The field, may be <c>null</c> if creating a new field.</param>
        /// <param name="schemaXml">The schema XML, if <c>null</c>, existing field schema XML will be modifed.</param>
        /// <returns></returns>
        public async Task<XElement> Update(Field field, XElement schemaXml)
        {
            if (schemaXml == null)
            {
                schemaXml = await GetExistingSchemaXml(field);
            }

            if (field.IsNull())
            {
                return RunSchemaXmlTransformers(schemaXml, Transformers);
            }

            return RunSchemaXmlTransformers(
                schemaXml,
                Transformers.Where(t => !t.SkipWhenModifying)
            );
        }

        private static XElement RunSchemaXmlTransformers(XElement schemaXml, IEnumerable<HarshFieldSchemaXmlTransformer> transformers)
        {
            foreach (var trans in transformers)
            {
                schemaXml = trans.Transform(schemaXml);
            }

            return schemaXml;
        }
    }
}