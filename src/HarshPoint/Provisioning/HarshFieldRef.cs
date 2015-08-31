using HarshPoint.Provisioning.Implementation;
using HarshPoint.Provisioning.Resolvers;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public class HarshFieldRef : HarshProvisioner
    {
        public HarshFieldRef()
        {
            ExistingFieldLinks = DeferredResolveBuilder.Create(
                () => new ResolveContentTypeFieldLink(ContentType.ValidateIsClientObjectResolveBuilder())
            );
        }

        [DefaultFromContext]
        [Parameter]
        public IResolveSingle<ContentType> ContentType
        {
            get;
            set;
        }

        [Parameter(Mandatory = true)]
        public IResolve<Field> Fields
        {
            get;
            set;
        }

        [Parameter]
        public Boolean Hidden
        {
            get;
            set;
        }

        [Parameter]
        public Boolean Required
        {
            get;
            set;
        }

        protected override void InitializeResolveContext(ClientObjectResolveContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            base.InitializeResolveContext(context);

            context.Include<Field>(
                f => f.Id
            );

            context.Include<FieldLink>(
                fl => fl.Id,
                fl => fl.Name
            );
        }

        protected override async Task OnProvisioningAsync()
        {
            var links = from field in Fields
                        select
                            ExistingFieldLinks.FirstOrDefault(fl => fl.Id == field.Id)
                            ??
                            ContentType.Value.FieldLinks.Add(
                                new FieldLinkCreationInformation() { Field = field }
                            );

            foreach (var link in links)
            {
                link.Hidden = Hidden;
                link.Required = Required;
            }

            ContentType.Value.Update(updateChildren: true);
            await ClientContext.ExecuteQueryAsync();
        }

        internal IResolve<FieldLink> ExistingFieldLinks { get; set; }
    }
}
