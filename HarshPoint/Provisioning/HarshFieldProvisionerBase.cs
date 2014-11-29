using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Provisioning
{
    public abstract class HarshFieldProvisionerBase : HarshPotentiallyDestructiveProvisioner
    {
        public Guid FieldId
        {
            get;
            set;
        }

        public List List
        {
            get;
            set;
        }

        protected override void Initialize()
        {
            TargetFieldCollection = Web.Fields;

            if (List != null)
            {
                TargetFieldCollection = List.Fields;
            }

            Field = TargetFieldCollection.GetById(FieldId);
            Context.Load(Field);
            Context.ExecuteQuery();

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