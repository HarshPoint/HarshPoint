using System;
using System.Xml.Linq;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Represents an modification of a field schema XML, either
    /// ony when creating a new field, or when updating an existing one.
    /// </summary>
    public abstract class HarshFieldSchemaXmlTransformer
    {
        /// <summary>
        /// Gets or sets a value indicating whether to run this transformation
        /// when creating a new field only.
        /// </summary>
        /// <value>
        /// <c>true</c> if this transformation runs only when creating new fields; otherwise, <c>false</c>.
        /// </value>
        public Boolean SkipWhenModifying
        {
            get;
            set;
        }

        /// <summary>
        /// Transforms the specified Field element.
        /// </summary>
        /// <param name="element">The Field element.</param>
        /// <returns>The transformed Field element.</returns>
        public abstract XElement Transform(XElement element);
    }
}
