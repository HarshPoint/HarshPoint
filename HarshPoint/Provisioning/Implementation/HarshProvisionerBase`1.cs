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
        private ICollection<Func<Object>> _childrenContextStateModifiers;
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
            => HarshLazy.Initialize(ref _children, CreateChildrenCollection);

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
            => HarshLazy.Initialize(ref _metadata, () => new HarshProvisionerMetadata(GetType()));

        public dynamic Result
        {
            get;
            private set;
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

        public void ModifyChildrenContextState(Func<Object> modifier)
        {
            if (modifier == null)
            {
                throw Error.ArgumentNull(nameof(modifier));
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
                throw Error.ArgumentNull(nameof(state));
            }

            ModifyChildrenContextState(() => state);
        }

        public Task<HarshProvisionerResult> ProvisionAsync(TContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            return RunSelfAndChildren(
                context,
                OnProvisioningAsync,
                ProvisionChildrenAsync);
        }

        public Task<HarshProvisionerResult> UnprovisionAsync(TContext context)
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            if (MayDeleteUserData || context.MayDeleteUserData || !Metadata.UnprovisionDeletesUserData)
            {
                return RunSelfAndChildren(
                    context,
                    OnUnprovisioningAsync,
                    UnprovisionChildrenAsync);
            }

            return Task.FromResult<HarshProvisionerResult>(
                new HarshProvisionerResultNotRun() { Provisioner = this }
            );
        }

        protected virtual Task InitializeAsync()
        {
            return HarshTask.Completed;
        }

        protected virtual void Complete()
        {
        }

        protected virtual Task<HarshProvisionerResult> OnProvisioningAsync()
        {
            return Task.FromResult(new HarshProvisionerResult());
        }

        [NeverDeletesUserData]
        protected virtual Task<HarshProvisionerResult> OnUnprovisioningAsync()
        {
            return Task.FromResult(new HarshProvisionerResult());
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected Task<IEnumerable<T>> TryResolveAsync<T>(IResolve<T> resolver)
        {
            if (resolver == null)
            {
                throw Error.ArgumentNull(nameof(resolver));
            }

            return resolver.TryResolveAsync(
                PrepareResolveContext()
            );
        }

        protected Task<T> TryResolveSingleAsync<T>(IResolve<T> resolver)
        {
            if (resolver == null)
            {
                throw Error.ArgumentNull(nameof(resolver));
            }

            return resolver.TryResolveSingleAsync(
                PrepareResolveContext()
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected Task<IEnumerable<T>> ResolveAsync<T>(IResolve<T> resolver)
        {
            if (resolver == null)
            {
                throw Error.ArgumentNull(nameof(resolver));
            }

            return resolver.ResolveAsync(
                PrepareResolveContext()
            );
        }

        protected Task<T> ResolveSingleAsync<T>(IResolve<T> resolver)
        {
            if (resolver == null)
            {
                throw Error.ArgumentNull(nameof(resolver));
            }

            return resolver.ResolveSingleAsync(
                PrepareResolveContext()
            );
        }

        protected virtual ICollection<HarshProvisionerBase> CreateChildrenCollection()
        {
            return new Collection<HarshProvisionerBase>();
        }

        internal virtual ResolveContext<TContext> CreateResolveContext()
        {
            return new ResolveContext<TContext>();
        }

        internal abstract Task<HarshProvisionerResult> ProvisionChild(HarshProvisionerBase provisioner, TContext context);

        internal abstract Task<HarshProvisionerResult> UnprovisionChild(HarshProvisionerBase provisioner, TContext context);

        private void InitializeDefaultFromContextProperties()
        {
            var properties = Metadata
                .DefaultFromContextProperties
                .Where(p => p.HasDefaultValue(this));

            foreach (var p in properties)
            {
                Object value = null;

                if (p.TagType != null)
                {
                    var tag = Context
                        .GetState(p.TagType)
                        .FirstOrDefault();

                    value = (tag as IDefaultFromContextTag)?.Value;
                }
                else if (p.ResolvedType != null)
                {
                    value = ContextStateResolver.Create(p.ResolvedType);
                }
                else
                {
                    value = Context.GetState(p.PropertyType).FirstOrDefault();
                }

                if (value != null)
                {
                    p.Setter(this, value);
                }
            }
        }

        private TContext PrepareChildrenContext()
        {
            if (_childrenContextStateModifiers == null)
            {
                return Context;
            }

            return _childrenContextStateModifiers
                .Select(fn => fn())
                .Where(state => state != null)
                .Aggregate(
                    Context, (ctx, state) => (TContext)ctx.PushState(state)
                );
        }

        private ResolveContext<TContext> PrepareResolveContext()
        {
            var resolveContext = CreateResolveContext();
            resolveContext.ProvisionerContext = Context;
            return resolveContext;
        }

        private Task<IEnumerable<HarshProvisionerResult>> ProvisionChildrenAsync()
        {
            return RunChildren(ProvisionChild);
        }

        private Task<IEnumerable<HarshProvisionerResult>> UnprovisionChildrenAsync()
        {
            return RunChildren(UnprovisionChild, reverse: true);
        }

        private async Task<IEnumerable<HarshProvisionerResult>> RunChildren(
            Func<HarshProvisionerBase, TContext, Task<HarshProvisionerResult>> action,
            Boolean reverse = false)
        {
            if (!HasChildren)
            {
                return NoResults;
            }

            var children = reverse ? _children.Reverse() : _children;
            var context = PrepareChildrenContext();

            return await children.SelectSequentially(
                child => action(child, context)
            );
        }

        private async Task<HarshProvisionerResult> RunWithContext(TContext context, Func<Task<HarshProvisionerResult>> action)
        {
            Context = context;

            try
            {
                return await action();
            }
            finally
            {
                Context = null;
            }
        }

        private Task<HarshProvisionerResult> RunSelfAndChildren(
            TContext context,
            Func<Task<HarshProvisionerResult>> action,
            Func<Task<IEnumerable<HarshProvisionerResult>>> childAction)
        {
            return RunWithContext(context, async () =>
            {
                HarshProvisionerResult result;

                try
                {
                    InitializeDefaultFromContextProperties();

                    await InitializeAsync();
                    result = await action();
                }
                finally
                {
                    Complete();
                }

                if (result == null)
                {
                    result = new HarshProvisionerResult();
                }

                var childResults = await childAction();
                return ProcessResult(result, childResults);
            });
        }

        private HarshProvisionerResult ProcessResult(HarshProvisionerResult result, IEnumerable<HarshProvisionerResult> childResults)
        {
            result.ChildResults = childResults?.ToImmutableArray() ?? NoResults;
            result.Provisioner = this;

            Result = result;
            return result;
        }

        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        protected static readonly ICollection<HarshProvisionerBase> NoChildren =
            ImmutableArray<HarshProvisionerBase>.Empty;

        private static readonly ImmutableArray<HarshProvisionerResult> NoResults =
            ImmutableArray<HarshProvisionerResult>.Empty;
    }
}
