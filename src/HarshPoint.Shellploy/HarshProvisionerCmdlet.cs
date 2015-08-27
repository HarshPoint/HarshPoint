using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;

namespace HarshPoint.Shellploy
{
    public abstract class HarshProvisionerCmdlet : PSCmdlet
    {
        internal static void AddChildren<TContext>(
            HarshProvisionerBase<TContext> parent,
            Object children
        )
            where TContext : HarshProvisionerContextBase<TContext>
        {
            if (children == null)
            {
                return;
            }

            if ((children is HarshProvisionerBase) ||
                (children is IDefaultFromContextTag))
            {
                AddChild(parent, children);
            }

            var scriptBlock = children as ScriptBlock;
            if (scriptBlock != null)
            {
                children = scriptBlock
                    .Invoke()
                    .Select(c => c.BaseObject);
            }

            var enumerable = children as IEnumerable;
            if (enumerable != null)
            {
                AddChildren(parent, enumerable);
            }
            else
            {
                AddChild(parent, children);
            }
        }

        internal static void AddChildren<TContext>(
            HarshProvisionerBase<TContext> parent,
            IEnumerable children
        )
            where TContext : HarshProvisionerContextBase<TContext>
        {
            foreach (var child in children)
            {
                AddChild(parent, child);
            }
        }

        internal static void AddChild<TContext>(
            HarshProvisionerBase<TContext> parent,
            Object child
        )
            where TContext : HarshProvisionerContextBase<TContext>
        {
            if (parent == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(parent));
            }

            if (child == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(child));
            }

            var psobject = (child as PSObject);

            if (psobject != null)
            {
                child = psobject.BaseObject;
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

        protected void WriteExclusiveSwitchValidationError(
            String positive, 
            String negative
        )
        {
            if (positive == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(positive));
            }

            if (negative == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(negative));
            }

            var exc = Logger.Error.Write(
                new ValidationMetadataException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        SR.HarshProvisionerCmdlet_ExclusiveSwitchError,
                        positive,
                        negative
                    )
                )
            );

            var er = new ErrorRecord(
                exc,
                "ExclusiveSwitchValidationError",
                ErrorCategory.InvalidArgument,
                null
            );

            WriteError(er);
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(HarshProvisionerCmdlet));
    }
}
