using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal static class ClientObjectResolveQuery
    {
        public static readonly ClientObjectResolveQuery<ContentType, ContentType, ContentTypeCollection, HarshContentTypeId> ContentTypeById =
            new ClientObjectResolveQuery<ContentType, ContentTypeCollection, HarshContentTypeId>(
                ct => HarshContentTypeId.Parse(ct.StringId),
                contentTypes => contentTypes.Include(ct => ct.StringId)
            );

        public static readonly ClientObjectResolveQuery<List, List, Web, String> ListByUrl =
            new ClientObjectResolveQuery<List, Web, String>(
                list => HarshUrl.GetRelativeTo(list.RootFolder.ServerRelativeUrl, list.ParentWebUrl),
                web => web.Lists.Include(
                    list => list.ParentWebUrl,
                    list => list.RootFolder.ServerRelativeUrl
                )
            )
            .WithIdentifierComparer(StringComparer.OrdinalIgnoreCase);

        public static readonly ClientObjectResolveQuery<View, View, List, String> ListViewByUrl =
            new ClientObjectResolveQuery<View, List, String>(
                (list, view) => HarshUrl.GetRelativeTo(view.ServerRelativeUrl, list.RootFolder.ServerRelativeUrl),
                list => list.Views.Include(
                    view => view.ServerRelativeUrl
                )
            )
            .WithIdentifierComparer(StringComparer.OrdinalIgnoreCase)
            .WithIncludesOnParent(
                list => list.RootFolder.ServerRelativeUrl
            );

        public static readonly ClientObjectResolveQuery<Field, Field, FieldCollection, Guid> FieldById =
            new ClientObjectResolveQuery<Field, FieldCollection, Guid>(
                field => field.Id,
                fields => fields.Include(f => f.Id)
            );

        public static readonly ClientObjectResolveQuery<Field, Field, FieldCollection, String> FieldByInternalName =
            new ClientObjectResolveQuery<Field, FieldCollection, String>(
                field => field.InternalName,
                fields => fields.Include(f => f.InternalName)
            )
            // case sensitive because there are fields that differ only in case
            .WithIdentifierComparer(StringComparer.Ordinal);

        public static readonly ClientObjectResolveQuery<TermSet, TermGroup, TermStore, Guid> TermStoreTermSetById =
            new ClientObjectResolveQuery<TermSet, TermGroup, TermStore, Guid>(
                termSet => termSet.Id,
                termStore => termStore.Groups.Include(
                    group => group.TermSets.Include(termSet => termSet.Id)
                ),
                groups => groups.SelectMany(group => group.TermSets)
            );
    }
}
