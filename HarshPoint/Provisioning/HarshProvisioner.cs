using HarshPoint.Provisioning.Implementation;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Provides context for classes provisioning SharePoint
    /// artifacts using the client-side object model.
    /// </summary>
    public class HarshProvisioner : HarshProvisionerBase<HarshProvisionerContext>
    {
        public ClientContext ClientContext => Context?.ClientContext;

        public Site Site => Context?.Site;

        public Web Web => Context?.Web;

        public void ModifyChildrenContextState(Func<ClientObject> modifier)
        {
            ModifyChildrenContextState(() =>
            {
                var result = modifier();

                if (result.IsNull())
                {
                    return null;
                }

                return (Object)(result);
            });
        }

        [SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification =
            "C# overload resolution doesn't consider base class methods once it finds " +
            "the TryResolveAsync(resolver, retrievals) overload, which only  accepts ClientObjects."
        )]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected new Task<IEnumerable<T>> TryResolveAsync<T>(IResolve<T> resolver)
        {
            return base.TryResolveAsync(resolver);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected Task<IEnumerable<T>> TryResolveAsync<T>(IResolve<T> resolver, params Expression<Func<T, Object>>[] retrievals)
            where T : ClientObject
        {
            return TryResolveAsync(
                resolver,
                CreateResolveContext(retrievals)
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification =
            "C# overload resolution doesn't consider base class methods once it finds " +
            "the TryResolveSingleAsync(resolver, retrievals) overload, which only  accepts ClientObjects."
        )]
        protected new Task<T> TryResolveSingleAsync<T>(IResolve<T> resolver)
        {
            return base.TryResolveSingleAsync(resolver);
        }

        protected Task<T> TryResolveSingleAsync<T>(IResolve<T> resolver, params Expression<Func<T, Object>>[] retrievals)
            where T : ClientObject
        {
            return TryResolveSingleAsync(
                resolver,
                CreateResolveContext(retrievals)
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification =
            "C# overload resolution doesn't consider base class methods once it finds " +
            "the ResolveAsync(resolver, retrievals) overload, which only  accepts ClientObjects."
        )]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected new Task<IEnumerable<T>> ResolveAsync<T>(IResolve<T> resolver)
        {
            return base.ResolveAsync(resolver);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected Task<IEnumerable<T>> ResolveAsync<T>(IResolve<T> resolver, params Expression<Func<T, Object>>[] retrievals)
            where T : ClientObject
        {
            return ResolveAsync(
                resolver,
                CreateResolveContext(retrievals)
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1061:DoNotHideBaseClassMethods", Justification =
            "C# overload resolution doesn't consider base class methods once it finds " +
            "the ResolveSingleAsync(resolver, retrievals) overload, which only  accepts ClientObjects."
        )]
        protected new Task<T> ResolveSingleAsync<T>(IResolve<T> resolver)
        {
            return base.ResolveSingleAsync(resolver);
        }

        protected Task<T> ResolveSingleAsync<T>(IResolve<T> resolver, params Expression<Func<T, Object>>[] retrievals)
            where T : ClientObject
        {
            return ResolveSingleAsync(
                resolver,
                CreateResolveContext(retrievals)
            );
        }

        internal sealed override Task<HarshProvisionerResult> ProvisionChild(HarshProvisionerBase provisioner, HarshProvisionerContext context)
        {
            if (provisioner == null)
            {
                throw Error.ArgumentNull(nameof(provisioner));
            }

            return ((HarshProvisioner)(provisioner)).ProvisionAsync(context);
        }

        internal sealed override Task<HarshProvisionerResult> UnprovisionChild(HarshProvisionerBase provisioner, HarshProvisionerContext context)
        {
            if (provisioner == null)
            {
                throw Error.ArgumentNull(nameof(provisioner));
            }

            return ((HarshProvisioner)(provisioner)).UnprovisionAsync(context);
        }

        private static ResolveContext<HarshProvisionerContext> CreateResolveContext<T>(Expression<Func<T, Object>>[] retrievals) 
            where T : ClientObject
        {
            var context = new ClientObjectResolveContext();
            context.Include(retrievals);
            return context;
        }
    }
}
