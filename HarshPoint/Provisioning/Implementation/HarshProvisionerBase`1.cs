using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    /// <summary>
    /// Provides common initialization and completion logic for 
    /// classes provisioning SharePoint artifacts.
    /// </summary>
    public abstract class HarshProvisionerBase<TContext> : HarshProvisionerBase
        where TContext : HarshProvisionerContextBase
    {
        private ICollection<HarshProvisionerBase> _children;
        private HarshProvisionerMetadata _metadata;
        
        protected HarshProvisionerBase()
        {
            Logger = Log.ForContext(GetType());
        }

        public TContext Context
        {
            get;
            private set;
        }

        public ICollection<HarshProvisionerBase> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = CreateChildrenCollection();
                }

                return _children;
            }
        }

        public ILogger Logger
        {
            get;
            private set;
        }

        public Boolean MayDeleteUserData
        {
            get;
            set;
        }

        internal HarshProvisionerMetadata Metadata
        {
            get
            {
                if (_metadata == null)
                {
                    _metadata = new HarshProvisionerMetadata(GetType());
                }

                return _metadata;
            }
        }

        internal Boolean HasChildren
        {
            get
            {
                if (_children == null || _children.IsReadOnly)
                {
                    return false;
                }

                return _children.Any();
            }
        }

        public async Task ProvisionAsync(TContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            await RunWithContext(OnProvisioningAsync, context);
        }

        public async Task UnprovisionAsync(TContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (MayDeleteUserData || context.MayDeleteUserData || !Metadata.UnprovisionDeletesUserData)
            {
                await RunWithContext(OnUnprovisioningAsync, context);
            }
        }

        protected override Task InitializeAsync()
        {
            return Task.FromResult(false);
        }

        protected override Task OnProvisioningAsync()
        {
            return ProvisionChildrenAsync();
        }

        [NeverDeletesUserData]
        protected override Task OnUnprovisioningAsync()
        {
            return UnprovisionChildrenAsync();
        }

        protected Task ProvisionChildrenAsync()
        {
            return ProvisionChildrenAsync(null);
        }

        protected Task ProvisionChildrenAsync(TContext context)
        {
            return RunChildren(ProvisionChild, context);
        }

        protected Task UnprovisionChildrenAsync()
        {
            return UnprovisionChildrenAsync(null);
        }

        protected Task UnprovisionChildrenAsync(TContext context)
        {
            return RunChildren(UnprovisionChild, context, reverse: true);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected Task<IEnumerable<T>> ResolveAsync<T>(IResolve<T> resolver)
        {
            if (resolver == null)
            {
                throw Error.ArgumentNull(nameof(resolver));
            }

            return resolver.ResolveAsync(Context);
        }

        protected Task<T> ResolveAsync<T>(IResolveSingle<T> resolver)
        {
            if (resolver == null)
            {
                throw Error.ArgumentNull(nameof(resolver));
            }

            return resolver.ResolveSingleAsync(Context);
        }

        internal virtual ICollection<HarshProvisionerBase> CreateChildrenCollection()
        {
            return new Collection<HarshProvisionerBase>();
        }

        internal abstract Task ProvisionChild(HarshProvisionerBase provisioner, TContext context);

        internal abstract Task UnprovisionChild(HarshProvisionerBase provisioner, TContext context);

        private void InitializeDefaultFromContextProperties()
        {
            var properties = from p in Metadata.DefaultFromContextProperties
                             where p.GetValue(this) == null
                             select p;

            foreach (var p in properties)
            {
                var resolver = Activator.CreateInstance(
                    typeof(ContextStateResolver<>).MakeGenericType(p.ResolvedType)
                );

                p.SetValue(this, resolver);
            }
        }

        private async Task RunChildren(Func<HarshProvisionerBase, TContext, Task> action, TContext context, Boolean reverse = false)
        {
            if (!HasChildren)
            {
                return;
            }

            var children = reverse ? _children.Reverse() : _children;

            context = context ?? Context;

            foreach (var child in children)
            {
                await action(child, context);
            }
        }

        private async Task RunWithContext(Func<Task> action, TContext context)
        {
            if (action == null)
            {
                throw Error.ArgumentNull(nameof(action));
            }

            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            Context = context;

            try
            {
                try
                {
                    InitializeDefaultFromContextProperties();

                    await InitializeAsync();
                    await action();
                }
                finally
                {
                    Complete();
                }
            }
            finally
            {
                Context = null;
            }
        }

        internal static readonly ICollection<HarshProvisionerBase> NoChildren =
            ImmutableList<HarshProvisionerBase>.Empty;
    }
}
