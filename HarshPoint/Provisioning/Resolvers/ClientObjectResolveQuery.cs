using Microsoft.SharePoint.Client;
using System;

namespace HarshPoint.Provisioning.Resolvers
{
    internal static class ClientObjectResolveQuery
    {
        public static readonly ClientObjectResolveQuery<ContentType, ContentTypeCollection, HarshContentTypeId> ContentTypeById =
            new ClientObjectResolveQuery<ContentType, ContentTypeCollection, HarshContentTypeId>(
                ct => HarshContentTypeId.Parse(ct.StringId),
                contentTypeCollection => contentTypeCollection.Include(
                    ct => ct.StringId,
                    ct => ct.Name // shouldn't be here and users should Include()
                                  // it themselves, once that functionaly is available
                )
            );

        public static readonly ClientObjectResolveQuery<List, Web, String> ListByUrl =
            new ClientObjectResolveQuery<List, Web, String>(
                list => HarshUrl.GetRelativeTo(list.RootFolder.ServerRelativeUrl, list.ParentWebUrl),
                web => web.Lists.Include(
                    list => list.ParentWebUrl,
                    list => list.RootFolder.ServerRelativeUrl
                ),
                StringComparer.OrdinalIgnoreCase
            );

        public static readonly ClientObjectResolveQuery<Field, FieldCollection, Guid> FieldById =
            new ClientObjectResolveQuery<Field, FieldCollection, Guid>(
                field => field.Id,
                fieldCollection => fieldCollection.Include(
                    field => field.Id,
                    field => field.InternalName // shouldn't be here and users should Include()
                )                               // it themselves, once that functionaly is available
            );

        public static readonly ClientObjectResolveQuery<Field, FieldCollection, String> FieldByInternalName =
            new ClientObjectResolveQuery<Field, FieldCollection, String>(
                field => field.InternalName,
                fieldCollection => fieldCollection.Include(
                    field => field.Id, // shouldn't be here and users should Include()
                                       // it themselves, once that functionaly is available
                    field => field.InternalName 
                ),

                // case sensitive because there are fields that differ only in case
                StringComparer.Ordinal
            );
    }
}
