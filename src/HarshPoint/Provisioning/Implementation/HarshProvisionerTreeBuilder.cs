using System;

namespace HarshPoint.Provisioning.Implementation
{
    /// <summary>
    /// Used by ShellPloy to dynamically add child provisioners and IDefaultFromContextTags.
    /// </summary>
    public static class HarshProvisionerTreeBuilder
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(HarshProvisionerTreeBuilder));

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
