using Microsoft.SharePoint.Client;
using System;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Base class for provisioners that modify SharePoint fields using the 
    /// client-side object model.
    /// </summary>
    public abstract class HarshFieldProvisioner : HarshProvisioner
    {
        /// <summary>
        /// Gets or sets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier. Must not be an empty <see cref="Guid"/>.
        /// </value>
        public Guid Id
        {
            get;
            set;
        }

        protected override async Task InitializeAsync()
        {
            if (Id == Guid.Empty)
            {
                throw Error.InvalidOperation(SR.HarshFieldProvisionerBase_FieldIdEmpty);
            }

            TargetFieldCollection = Web.Fields;
            Field = await ResolveSingleOrDefaultAsync(Resolve.FieldById(Id));
        }

        public Field Field
        {
            get;
            protected set;
        }

        protected FieldCollection TargetFieldCollection
        {
            get;
            private set;
        }
    }
}