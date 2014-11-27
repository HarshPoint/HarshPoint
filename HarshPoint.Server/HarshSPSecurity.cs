using Microsoft.SharePoint;
using System;

namespace HarshPoint.Server
{
    public sealed class HarshSPSecurity : IDisposable
    {
        private HarshSPSecurity()
        {
        }

        public void Dispose()
        {
            if (Web != null)
            {
                Web.Dispose();
            }

            if (Site != null)
            {
                Site.Dispose();
            }

            List = null;
            ListItem = null;
            File = null;
            Folder = null;
            Web = null;
            Site = null;
        }

        public SPSite Site
        {
            get;
            private set;
        }

        public SPWeb Web
        {
            get;
            private set;
        }

        public SPList List
        {
            get;
            private set;
        }

        public SPListItem ListItem
        {
            get;
            private set;
        }

        public SPFile File
        {
            get;
            private set;
        }

        public SPFolder Folder
        {
            get;
            private set;
        }

        public static void RunWithElevatedPrivileges(SPSite site, Action<HarshSPSecurity> action)
        {
            if (site == null)
            {
                throw Error.ArgumentNull("site");
            }

            RunWithElevatedPrivileges(() => Reopen(site), action);
        }

        public static void RunWithElevatedPrivileges(SPWeb web, Action<HarshSPSecurity> action)
        {
            if (web == null)
            {
                throw Error.ArgumentNull("web");
            }

            RunWithElevatedPrivileges(() => Reopen(web), action);
        }

        public static void RunWithElevatedPrivileges(SPList list, Action<HarshSPSecurity> action)
        {
            if (list == null)
            {
                throw Error.ArgumentNull("list");
            }

            RunWithElevatedPrivileges(() => Reopen(list), action);
        }

        public static void RunWithElevatedPrivileges(SPListItem listItem, Action<HarshSPSecurity> action)
        {
            if (listItem == null)
            {
                throw Error.ArgumentNull("listItem");
            }

            RunWithElevatedPrivileges(() => Reopen(listItem), action);
        }

        public static void RunWithElevatedPrivileges(SPFile file, Action<HarshSPSecurity> action)
        {
            if (file == null)
            {
                throw Error.ArgumentNull("file");
            }

            RunWithElevatedPrivileges(() => Reopen(file), action);
        }

        public static void RunWithElevatedPrivileges(SPFolder folder, Action<HarshSPSecurity> action)
        {
            if (folder == null)
            {
                throw Error.ArgumentNull("folder");
            }

            RunWithElevatedPrivileges(() => Reopen(folder), action);
        }

        private static void RunWithElevatedPrivileges(Func<HarshSPSecurity> contextBuilder, Action<HarshSPSecurity> action)
        {
            if (action == null)
            {
                throw Error.ArgumentNull("action");
            }

            SPSecurity.RunWithElevatedPrivileges(delegate
            {
                using (var context = contextBuilder())
                {
                    action(context);
                }
            });    
        }

        private static HarshSPSecurity Reopen(SPSite site)
        {
            return new HarshSPSecurity()
            {
                Site = new SPSite(site.ID)
                {
                    AllowUnsafeUpdates = true
                }
            };
        }

        private static HarshSPSecurity Reopen(SPWeb web)
        {
            var result = Reopen(web.Site);
            result.Web = result.Site.OpenWeb(web.ID);
            result.Web.AllowUnsafeUpdates = true;
            return result;
        }

        private static HarshSPSecurity Reopen(SPList list)
        {
            var result = Reopen(list.ParentWeb);
            result.List = result.Web.Lists[list.ID];
            return result;
        }

        private static HarshSPSecurity Reopen(SPListItem listItem)
        {
            var result = Reopen(listItem.ParentList);
            result.ListItem = result.List.GetItemById(listItem.ID);
            return result;
        }

        private static HarshSPSecurity Reopen(SPFile file)
        {
            var result = Reopen(file.Web);
            result.File = result.Web.GetFile(file.UniqueId);
            return result;
        }

        private static HarshSPSecurity Reopen(SPFolder folder)
        {
            var result = Reopen(folder.ParentWeb);
            result.Folder = result.Web.GetFolder(folder.UniqueId);
            return result;
        }
    }
}
