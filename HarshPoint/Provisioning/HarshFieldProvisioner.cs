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

        /// <summary>
        /// Gets or sets the list containing the field.
        /// </summary>
        /// <value>
        /// The list. When null, a site column is created or updated.
        /// </value>
        public IResolveSingle<List> List
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

            if (List != null)
            {
                var resolved = await ResolveSingleAsync(List);
                TargetFieldCollection = resolved.Fields;
            }

            Field = TargetFieldCollection.GetById(Id);
            ClientContext.Load(Field);

            await ClientContext.ExecuteQueryAsync();
        }

        protected Field Field
        {
            get;
            set;
        }

        protected FieldCollection TargetFieldCollection
        {
            get;
            private set;
        }
    }
}