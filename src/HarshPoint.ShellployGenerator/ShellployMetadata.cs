using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace HarshPoint.ShellployGenerator
{
    internal class ShellployMetadata
    {
        private const String CommandNamespace = "HarshPoint.Shellploy";
        private const String ProvisioningNamespace = "HarshPoint.Provisioning";

        private Dictionary<Type, IShellployCommandBuilder> builders
            = new Dictionary<Type, IShellployCommandBuilder>();

        public ShellployMetadata()
        {
            Map<HarshContentType>()
                .AddPositionalParameter(x => x.Id)
                .AddPositionalParameter(x => x.Name)
                .HasChildren();

            Map<HarshContentTypeRef>()
                .AddPositionalParameter<HarshContentTypeId[]>(
                    "ContentTypeId",
                    new ShellployCommandPropertyParameterAttribute()
                    {
                        Mandatory = true,
                    }
                )
                .SetParameterValue(x => x.ContentTypes,
                    new CodeTypeReferenceExpression(typeof(Resolve))
                        .Call(nameof(Resolve.ContentType))
                        .Call(nameof(Resolve.ById), new CodeVariableReferenceExpression("ContentTypeId"))
                        .Call(nameof(ResolveBuilderExtensions.As), typeof(ContentType))
                );

            Map<HarshRemoveContentTypeRef>()
                .AddPositionalParameter<HarshContentTypeId[]>(
                    "ContentTypeId",
                    new ShellployCommandPropertyParameterAttribute()
                    {
                        Mandatory = true,
                    }
                )
                .SetParameterValue(x => x.ContentTypes,
                    new CodeTypeReferenceExpression(typeof(Resolve))
                        .Call(nameof(Resolve.ContentType))
                        .Call(nameof(Resolve.ById), new CodeVariableReferenceExpression("ContentTypeId"))
                        .Call(nameof(ResolveBuilderExtensions.As), typeof(ContentType))
                );

            Map<HarshField>();

            Map<HarshModifyFieldDateTime>()
                .AsChildOf<HarshField>()
                    .SetValue(x => x.Type, FieldType.DateTime);

            Map<HarshModifyFieldMultilineText>()
                .AsChildOf<HarshField>()
                    .SetValue(x => x.Type, FieldType.Note);

            Map<HarshModifyFieldTaxonomy>()
                .AddNamedParameter<Guid>("TermSetId")
                .SetParameterValue(x => x.TermSet,
                    new CodeTypeReferenceExpression(typeof(Resolve))
                        .Call(nameof(Resolve.TermStoreSiteCollectionDefault))
                        .Call(nameof(Resolve.TermSet))
                        .Call(nameof(Resolve.ById), new CodeVariableReferenceExpression("TermSetId"))
                        .Call(nameof(ResolveBuilderExtensions.As), typeof(TermSet))
                )
                .AsChildOf<HarshField>()
                    .SetValue(x => x.TypeName, "TaxonomyFieldType")
                    .IgnoreParameter(x => x.Type);

            Map<HarshModifyFieldLookup>()
                .AddNamedParameter<String>("TargetListUrl")
                .AddNamedParameter<String>("TargetField")
                .SetDefaultValue("TargetField", "Title")
                .SetParameterValue(x => x.LookupTarget,
                    new CodeTypeReferenceExpression(typeof(Resolve))
                        .Call(nameof(Resolve.List))
                        .Call(nameof(Resolve.ByUrl), new CodeVariableReferenceExpression("TargetListUrl"))
                        .Call(nameof(Resolve.Field))
                        .Call(nameof(Resolve.ByInternalName), new CodeVariableReferenceExpression("TargetField"))
                        .Call(nameof(ResolveBuilderExtensions.As), typeof(Tuple<List, Field>))
                )
                .AsChildOf<HarshField>()
                    .SetValue(x => x.Type, FieldType.Lookup)
                    .IgnoreParameter(x => x.TypeName);

            Map<HarshFieldRef>()
                .AddNamedParameter<String>("InternalName")
                .SetParameterValue(x => x.Fields,
                    new CodeTypeReferenceExpression(typeof(Resolve))
                        .Call(nameof(Resolve.Field))
                        .Call(nameof(Resolve.ByInternalName), new CodeVariableReferenceExpression("InternalName"))
                        .Call(nameof(ResolveBuilderExtensions.As), typeof(Field))
                );

            Map<HarshList>()
                .AddPositionalParameter(x => x.Title)
                .AddPositionalParameter(x => x.Url)
                .SetDefaultParameterValue(x => x.TemplateType, ListTemplateType.GenericList)
                .HasChildren();

            Map<HarshEnableWebFeature>()
                .AddPositionalParameter(x => x.FeatureId);
        }

        private ShellployCommandBuilder<TProvisioner> Map<TProvisioner>()
            where TProvisioner : HarshProvisionerBase
        {
            var builder = new ShellployCommandBuilder<TProvisioner>();
            builders.Add(typeof(TProvisioner), builder);
            return builder
                .InNamespace(CommandNamespace)
                .AddUsing(ProvisioningNamespace);
        }

        public IEnumerable<ShellployCommand> GetCommands()
        {
            return builders.Values.Select(builder => builder.ToCommand(builders));
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ShellployMetadata>();
    }
}
