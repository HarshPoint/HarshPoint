using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace HarshPoint.Shellploy
{
    /// <summary>
    /// Used by ShellPloy to dynamically add child provisioners and IDefaultFromContextTags.
    /// </summary>
    internal static class HarshProvisionerTreeBuilder
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(HarshProvisionerTreeBuilder));

        public static void AddChildren<TContext>(HarshProvisionerBase<TContext> parent, ScriptBlock children)
            where TContext : HarshProvisionerContextBase
        {
            if (children != null)
            {
                AddChildren(
                    parent,
                    children.Invoke()
                        .Select(c => c.BaseObject)
                );
            }
        }

        public static void AddChildren<TContext>(HarshProvisionerBase<TContext> parent, IEnumerable<Object> children)
            where TContext : HarshProvisionerContextBase
        {
            foreach (var child in children)
            {
                AddChild(parent, child);
            }
        }

        public static void AddChild<TContext>(HarshProvisionerBase<TContext> parent, Object child)
            where TContext : HarshProvisionerContextBase
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            if (child == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(child));
            }

            var provisioner = (child as HarshProvisionerBase);
            var defaultFromContextTag = (child as IDefaultFromContextTag);

            if (provisioner != null)
            {
                parent.Children.Add(provisioner);
            }
            else if (defaultFromContextTag != null)
            {
                parent.ModifyChildrenContextState(defaultFromContextTag);
            }
            else
            {
                throw Logger.Fatal.ArgumentNotAssignableTo(
                    nameof(child),
                    child,
                    typeof(HarshProvisionerBase),
                    typeof(IDefaultFromContextTag)
                );
            }
        }
    }
}
