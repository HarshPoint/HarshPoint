using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning
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

        public void Provision(TContext context)
        {
            RunWithContext(OnProvisioning, context);
        }

        public void Unprovision(TContext context)
        {
            if (context.MayDeleteUserData || !Metadata.UnprovisionDeletesUserData)
            {
                RunWithContext(OnUnprovisioning, context);
            }
        }

        protected override void OnProvisioning()
        {
            base.OnProvisioning();
            RunChildren(ProvisionChild);
        }

        [NeverDeletesUserData]
        protected override void OnUnprovisioning()
        {
            RunChildren(UnprovisionChild, reverse: true);
            base.OnUnprovisioning();
        }

        internal abstract void ProvisionChild(HarshProvisionerBase p);

        internal abstract void UnprovisionChild(HarshProvisionerBase p);

        internal virtual ICollection<HarshProvisionerBase> CreateChildrenCollection()
        {
            return new Collection<HarshProvisionerBase>();
        }

        private void RunChildren(Action<HarshProvisionerBase> action, Boolean reverse = false)
        {
            if (_children == null || _children == NoChildren)
            {
                return;
            }

            var children = reverse ? _children.Reverse() : _children;

            foreach (var child in children)
            {
                action(child);
            }
        }

        private void RunWithContext(Action action, TContext context)
        {
            if (action == null)
            {
                throw Error.ArgumentNull("action");
            }

            if (context == null)
            {
                throw Error.ArgumentNull("context");
            }

            Context = context;

            try
            {
                try
                {
                    Initialize();
                    action();
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
            new ReadOnlyCollection<HarshProvisionerBase>(new HarshProvisionerBase[0]);
    }
}
