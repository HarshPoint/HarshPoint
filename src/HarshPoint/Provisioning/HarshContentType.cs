using HarshPoint.ObjectModel;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static System.FormattableString;

namespace HarshPoint.Provisioning
{
    public class HarshContentType : HarshProvisioner
    {
        private LazyObjectMapping<HarshContentType, ContentType> _map;

        public HarshContentType()
        {
            Map(ct => ct.Name);
            Map(ct => ct.Description);
            Map(ct => ct.Group);

            ModifyChildrenContextState(
                () => ContentType
            );

            ExistingContentType = DeferredResolveBuilder.Create(
                () => Resolve.ContentType().ById(Id)
            );

            WriteRecord = CreateRecordWriter<ContentType>(() =>
            {
                if (ParentContentType != null)
                {
                    return Invariant(
                        $"{Name} ({ParentContentType.Value.Name})"
                    );
                }

                if (!String.IsNullOrWhiteSpace(Name))
                {
                    return Invariant($"{Name} ({Id})");
                }

                return Id.ToString();
            });
        }

        [Parameter]
        public String Description { get; set; }

        [Parameter]
        [DefaultFromContext(typeof(DefaultContentTypeGroup))]
        public String Group { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Id")]
        public HarshContentTypeId Id { get; set; }

        [MandatoryWhenCreating]
        [Parameter(ParameterSetName = "Id")]
        [Parameter(ParameterSetName = "ParentContentType", Mandatory = true)]
        public String Name { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "ParentContentType")]
        public IResolveSingle<ContentType> ParentContentType { get; set; }

        [Parameter]
        public Boolean NoUpdateChildren { get; set; }

        protected ObjectMapper<HarshContentType, ContentType>.IEntryBuilder Map(
            Expression<Func<ContentType, Object>> targetProperty
        )
            => _map.Map(targetProperty);

        protected override void InitializeResolveContext(
            ClientObjectResolveContext context
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            context.Include<ContentType>(
                ct => ct.StringId
            );

            context.Include(
                _map.GetTargetExpressions()
            );

            base.InitializeResolveContext(context);
        }

        protected override async Task OnProvisioningAsync()
        {
            if (ExistingContentType.Value == null)
            {
                ValidateMandatoryWhenCreatingParameters();

                ContentType = Web.ContentTypes.Add(new ContentTypeCreationInformation()
                {
                    Description = Description,
                    Group = Group,
                    Id = Id?.ToString(),
                    ParentContentType = ParentContentType?.Value,
                    Name = Name,
                });

                await ClientContext.ExecuteQueryAsync();

                WriteRecord.Added(ContentType);
            }
            else
            {
                ContentType = ExistingContentType.Value;
                if (_map.Apply(WriteRecord, this, ContentType))
                {
                    ContentType.Update(!NoUpdateChildren);
                }
                else
                {
                    WriteRecord.AlreadyExists(ContentType);
                }
            }
        }

        protected override async Task OnUnprovisioningAsync()
        {
            if (ExistingContentType.HasValue)
            {
                ExistingContentType.Value.DeleteObject();

                await ClientContext.ExecuteQueryAsync();

                WriteRecord.Removed();
            }
            else
            {
                WriteRecord.DidNotExist();
            }
        }

        private ContentType ContentType { get; set; }

        private RecordWriter<HarshProvisionerContext, ContentType> WriteRecord
        {
            get;
        }

        internal IResolveSingleOrDefault<ContentType> ExistingContentType
        {
            get;
            set;
        }
    }
}
