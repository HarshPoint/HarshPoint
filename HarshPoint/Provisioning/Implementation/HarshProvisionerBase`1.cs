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
        private Boolean _mayDeleteUserData;
        private HarshProvisionerMetadata _metadata;

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

        public Boolean MayDeleteUserData
        {
            get { return _mayDeleteUserData || (Context?.MayDeleteUserData ?? false); }
            set { _mayDeleteUserData = value; }
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
            await RunWithContext(OnProvisioningAsync, context);
        }

        public async Task UnprovisionAsync(TContext context)
        {
            if (MayDeleteUserData || !Metadata.UnprovisionDeletesUserData)
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
            return RunChildren(ProvisionChild);
        }

        [NeverDeletesUserData]
        protected override Task OnUnprovisioningAsync()
        {
            return RunChildren(UnprovisionChild, reverse: true);
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected Task<IEnumerable<T>> ResolveAsync<T>(IResolve<T> resolver)
        {
            if (resolver == null)
            {
                throw Error.ArgumentNull(nameof(resolver));
            }

            return Context.ResolveAsync(resolver);
        }

        protected Task<T> ResolveAsync<T>(IResolveSingle<T> resolver)
        {
            if (resolver == null)
            {
                throw Error.ArgumentNull(nameof(resolver));
            }

            return Context.ResolveSingleAsync(resolver);
        }

        internal virtual ICollection<HarshProvisionerBase> CreateChildrenCollection()
        {
            return new Collection<HarshProvisionerBase>();
        }

        internal abstract Task ProvisionChild(HarshProvisionerBase provisioner);

        internal abstract Task UnprovisionChild(HarshProvisionerBase provisioner);

        private async Task RunChildren(Func<HarshProvisionerBase, Task> action, Boolean reverse = false)
        {
            if (!HasChildren)
            {
                return;
            }

            var children = reverse ? _children.Reverse() : _children;

            foreach (var child in children)
            {
                await action(child);
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
