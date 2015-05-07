using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Base class for provisioners that modify SharePoint fields using the 
    /// client-side object model.
    /// </summary>
    public abstract class HarshFieldProvisioner : HarshPotentiallyDestructiveProvisioner
    {
        /// <summary>
        /// Gets or sets the field identifier.
        /// </summary>
        /// <value>
        /// The field identifier. Must not be an empty <see cref="Guid"/>.
        /// </value>
        public Guid FieldId
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
        public List List
        {
            get;
            set;
        }

        protected override void Initialize()
        {
            if (FieldId == Guid.Empty)
            {
                throw Error.InvalidOperation(SR.HarshFieldProvisionerBase_FieldIdEmpty);
            }

            TargetFieldCollection = Web.Fields;

            if (List != null)
            {
                TargetFieldCollection = List.Fields;
            }

            Field = TargetFieldCollection.GetById(FieldId);
            ClientContext.Load(Field);
            ClientContext.ExecuteQuery();

            base.Initialize();
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