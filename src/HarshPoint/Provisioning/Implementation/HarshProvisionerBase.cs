using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class HarshProvisionerBase
    {
        private ICollection<HarshProvisionerBase> _children;
        private ICollection<Func<Object>> _childrenContextStateModifiers;

        public ICollection<HarshProvisionerBase> Children
            => HarshLazy.Initialize(ref _children, CreateChildrenCollection);

        public ICollection<Func<Object>> ChildrenContextStateModifiers
            => _childrenContextStateModifiers;

        internal Boolean HasChildren
            => (_children != null) && _children.Any();

        public void ModifyChildrenContextState(Func<Object> modifier)
        {
            if (modifier == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(modifier));
            }

            if (_childrenContextStateModifiers == null)
            {
                _childrenContextStateModifiers = new Collection<Func<Object>>();
            }

            _childrenContextStateModifiers.Add(modifier);
        }

        public void ModifyChildrenContextState(Object state)
        {
            if (state == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(state));
            }

            ModifyChildrenContextState(() => state);
        }

        internal IImmutableList<HarshProvisionerBase> GetChildrenSorted(
            HarshProvisionerAction action
        )
        {
            if (!HasChildren)
            {
                return ImmutableList<HarshProvisionerBase>.Empty;
            }

            if (action == HarshProvisionerAction.Provision)
            {
                return Children.ToImmutableList();
            }
            else
            {
                return Children.Reverse().ToImmutableList();
            }
        }

        protected virtual ICollection<HarshProvisionerBase> CreateChildrenCollection()
            => new Collection<HarshProvisionerBase>();

        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        protected static readonly ICollection<HarshProvisionerBase> NoChildren =
            ImmutableArray<HarshProvisionerBase>.Empty;

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(HarshProvisionerBase));
    }
}
