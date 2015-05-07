using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HarshPoint.Provisioning
{
    /// <summary>
    /// Provides common initialization and completion logic for 
    /// classes provisioning SharePoint artifacts.
    /// </summary>
    public abstract class HarshProvisionerBase<TContext> : HarshProvisionerBase
        where TContext : class
    {
        private ICollection<HarshProvisionerBase> _children;
        
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

        public void Provision(TContext context)
        {
            RunWithContext(OnProvisioning, context);
        }

        public void Unprovision(TContext context)
        {
            RunWithContext(OnUnprovisioning, context);
        }

        protected override void OnProvisioning()
        {
            base.OnProvisioning();

            if (_children != null)
            {
                foreach (var child in _children)
                {
                    ProvisionChild(child);
                }
            }
        }

        protected override void OnUnprovisioning()
        {
            if (_children != null)
            {
                foreach (var child in _children.Reverse())
                {
                    UnprovisionChild(child);
                }
            }

            base.OnUnprovisioning();
        }

        internal abstract void ProvisionChild(HarshProvisionerBase p);

        internal abstract void UnprovisionChild(HarshProvisionerBase p);

        internal virtual ICollection<HarshProvisionerBase> CreateChildrenCollection()
        {
            return new Collection<HarshProvisionerBase>();
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
