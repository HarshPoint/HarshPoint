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
        private readonly LazyObjectMapping<HarshFieldRef, FieldLink> _map;
        private readonly RecordWriter<HarshProvisionerContext, FieldLink> _recordWriter;

        public HarshFieldRef()
        {
            ExistingFieldLinks = DeferredResolveBuilder.Create(
                () => new ResolveContentTypeFieldLink(ContentType.ValidateIsClientObjectResolveBuilder())
            );

            _map = new LazyObjectMapping<HarshFieldRef, FieldLink>(Metadata);

            _map.Map(fl => fl.Hidden);
            _map.Map(fl => fl.Required);

            _recordWriter = CreateRecordWriter<FieldLink>();
        }

        [Parameter]
        [DefaultFromContext]
        public IResolveSingle<ContentType> ContentType { get; set; }

        [Parameter(Mandatory = true)]
        public IResolve<Field> Fields { get; set; }

        [Parameter]
        public Boolean? Hidden { get; set; }

        [Parameter]
        public Boolean? Required { get; set; }

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

            context.Include(
                _map.GetTargetExpressions()
            );
        }

        protected override async Task OnProvisioningAsync()
        {
            var links =
                from field in Fields

                let existing = ExistingFieldLinks.FirstOrDefault(
                    fl => fl.Id == field.Id
                )

                let fieldLink =
                    existing ?? ContentType.Value.FieldLinks.Add(
                        new FieldLinkCreationInformation()
                        {
                            Field = field
                        }
                    )

                select new
                {
                    Created = (existing == null),
                    FieldLink = fieldLink
                };

            var changed = false;

            foreach (var link in links)
            {
                changed |= link.Created;
                changed |= _map.Apply(
                    _recordWriter, 
                    null,
                    this, 
                    link.FieldLink,
                    force: link.Created
                );
            }

            if (changed)
            {
                ContentType.Value.Update(updateChildren: true);
                await ClientContext.ExecuteQueryAsync();
            }
        }

        internal IResolve<FieldLink> ExistingFieldLinks { get; set; }
    }
}
