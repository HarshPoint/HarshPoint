using HarshPoint.ObjectModel;
using Serilog.Core.Enrichers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    /// <summary>
    /// Provides common initialization and completion logic for
    /// classes provisioning SharePoint artifacts.
    /// </summary>
    public abstract class HarshProvisionerBase<TContext> : HarshProvisionerBase, ITrackValueSource
        where TContext : HarshProvisionerContextBase<TContext>
    {
        private readonly HarshScopedValue<TContext> _context;
        private readonly HarshScopedValue<HarshLogger> _logger;
        private readonly HarshScopedValue<ParameterSet> _parameterSet;

        private ManualResolver _manualResolver;
        private HarshProvisionerMetadata _metadata;
        private PropertyValueSourceTracker _valueSourceTracker;

        protected HarshProvisionerBase()
        {
            _context = new HarshScopedValue<TContext>();
            _parameterSet = new HarshScopedValue<ParameterSet>();

            _logger = new HarshScopedValue<HarshLogger>(
                HarshLog.ForContext(GetType())
            );
        }

        public TContext Context => _context.Value;

        public HarshLogger Logger => _logger.Value;

        public Boolean MayDeleteUserData { get; set; }

        protected ManualResolver ManualResolver
            => HarshLazy.Initialize(
                ref _manualResolver,
                () => CreateManualResolver(CreateResolveContext)
            );

        protected String ParameterSetName => ParameterSet?.Name;

        internal HarshProvisionerBase<TContext> ForwardTarget { get; private set; }

        internal ParameterSet ParameterSet
            => _parameterSet.Value;

        internal HarshProvisionerMetadata Metadata
            => HarshLazy.Initialize(
                ref _metadata,
                () => HarshProvisionerMetadataRepository.Get(GetType())
            );

        internal PropertyValueSourceTracker ValueSourceTracker
            => HarshLazy.Initialize(ref _valueSourceTracker);

        public Task ProvisionAsync(TContext context, CancellationToken token)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return ProvisionAsync(context.WithToken(token));
        }

        public Task ProvisionAsync(
            TContext context,
            IProgress<HarshProvisionerRecord> progress
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return ProvisionAsync(
                context.WithProgress(progress)
            );
        }

        public Task ProvisionAsync(
            TContext context,
            CancellationToken token,
            IProgress<HarshProvisionerRecord> progress
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return ProvisionAsync(
                context.WithToken(token).WithProgress(progress)
            );
        }

        public Task ProvisionAsync(TContext context)
            => Run(context, HarshProvisionerAction.Provision);

        public Task UnprovisionAsync(TContext context, CancellationToken token)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return UnprovisionAsync(
                context.WithToken(token)
            );
        }

        public Task UnprovisionAsync(
            TContext context,
            IProgress<HarshProvisionerRecord> progress
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return UnprovisionAsync(
                context.WithProgress(progress)
            );
        }

        public Task UnprovisionAsync(
            TContext context,
            CancellationToken token,
            IProgress<HarshProvisionerRecord> progress
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            return UnprovisionAsync(
                context.WithToken(token).WithProgress(progress)
            );
        }

        public Task UnprovisionAsync(TContext context)
            => Run(context,HarshProvisionerAction.Unprovision);

        protected virtual Task InitializeAsync() => HarshTask.Completed;

        protected virtual void Complete() { }

        protected virtual Task OnProvisioningAsync() => HarshTask.Completed;

        protected virtual void OnValidating() { }

        [NeverDeletesUserData]
        protected virtual Task OnUnprovisioningAsync() => HarshTask.Completed;

        protected RecordWriter<TContext, T> CreateRecordWriter<T>()
            => CreateRecordWriter<T>(null);

        protected RecordWriter<TContext, T> CreateRecordWriter<T>(
            Func<String> identifierSelector
        )
            => new RecordWriter<TContext, T>(this, identifierSelector);

        protected virtual ResolveContext<TContext> CreateResolveContext()
            => new ResolveContext<TContext>(Context);

        protected void ForwardsTo(HarshProvisionerBase<TContext> target)
        {
            ForwardTarget = target;
        }

        protected internal Boolean IsValueDefaultFromContext(Expression<Func<Object>> expression)
            => _valueSourceTracker?.GetValueSource(expression)
                == DefaultFromContextPropertyValueSource.Instance;

        protected void ValidateMandatoryWhenCreatingParameters()
        {
            var mandatory = ParameterSet
                .Parameters
                .Where(Metadata.IsMandatoryWhenCreating);

            foreach (var param in mandatory)
            {
                ValidateHasNonDefaultValue(param);
            }
        }

        protected abstract Task ProvisionChild(
            HarshProvisionerBase provisioner,
            TContext context
        );

        protected abstract Task UnprovisionChild(
            HarshProvisionerBase provisioner,
            TContext context
        );

        internal virtual ManualResolver CreateManualResolver(
            Func<IResolveContext> resolveContextFactory
        )
            => new ManualResolver(resolveContextFactory);

        internal virtual Task OnResolvedPropertiesBound() => HarshTask.Completed;

        private ParameterSet ResolveParameterSet()
        {
            var resolver = new ParameterSetResolver(this, Metadata.ParameterSets);
            return resolver.Resolve();
        }

        private void ValidateParameters()
        {
            foreach (var parameter in ParameterSet.Parameters)
            {
                if (parameter.IsMandatory)
                {
                    ValidateHasNonDefaultValue(parameter);
                }

                foreach (var attr in parameter.ValidationAttributes)
                {
                    attr.Validate(parameter, parameter.GetValue(this));
                }
            }
        }

        private void ValidateHasNonDefaultValue(Parameter parameter)
        {
            if (parameter.HasDefaultValue(this))
            {
                throw Logger.Error.ParameterValidationFormat(
                    parameter,
                    SR.HarshProvisionerBase_ParameterMandatory,
                    parameter
                );
            }
        }

        private TContext PrepareChildrenContext()
        {
            if (ChildrenContextStateModifiers == null)
            {
                return Context;
            }

            return ChildrenContextStateModifiers
                .Select(fn => fn())
                .Where(state => state != null)
                .Aggregate(
                    Context, (ctx, state) => (TContext)ctx.PushState(state)
                );
        }

        private async Task Run(
            TContext context,
            HarshProvisionerAction action
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            var isSessionRoot = (context.Session == null);
            if (isSessionRoot)
            {
                context = context.WithSession(new ProvisioningSession(this, action));
                context.Session.OnSessionStarting(context);
            }

            await RunWithContext(context, () =>
                     RunSelfAndChildren(
                        context,
                        action
                    )
                );

            if (isSessionRoot)
            {
                context.Session.OnSessionEnded(context);
            }
        }

        private async Task RunChildren(
            HarshProvisionerAction action
        )
        {
            if (!HasChildren)
            {
                return;
            }

            var context = PrepareChildrenContext();

            foreach (var child in GetChildrenSorted(action))
            {
                context.Token.ThrowIfCancellationRequested();

                if (action == HarshProvisionerAction.Provision)
                {
                    await ProvisionChild(child, context);
                }
                else
                {
                    await UnprovisionChild(child, context);
                }
            }
        }

        private async Task RunWithContext(
            TContext context,
            Func<Task> action
        )
        {
            var contextLogger = Logger.ForContext(
                new PropertyEnricher("ProvisionerContext", context),
                new PropertyEnricher("MayDeleteUserData", MayDeleteUserData)
            );

            using (_context.Enter(context))
            using (_logger.Enter(contextLogger))
            {
                await action();
            }
        }

        private async Task RunSelfAndChildren(
            TContext context,
            HarshProvisionerAction action
        )
        {
            var mayDelete = MayDeleteUserData ||
                context.MayDeleteUserData ||
                !Metadata.UnprovisionDeletesUserData;

            if (action == HarshProvisionerAction.Provision || mayDelete)
            {
                await RunSelf(context, action);
            }
            else
            {
                context.Session.OnProvisioningSkipped(context, this);
            }

            await RunChildren(action);
        }

        private async Task RunSelf(TContext context, HarshProvisionerAction action)
        {
            context.Token.ThrowIfCancellationRequested();

            Metadata.DefaultFromContextPropertyBinder.Bind(
                this,
                Context
            );

            Metadata.ResolvedPropertyBinder.Bind(
                this,
                CreateResolveContext
            );

            context.Token.ThrowIfCancellationRequested();

            await OnResolvedPropertiesBound();

            // parameter set resolving depends on values
            // from context being already set

            using (_parameterSet.Enter(ResolveParameterSet()))
            {
                try
                {
                    ValidateParameters();
                    OnValidating();

                    context.Session.OnProvisioningStarting(context, this);

                    await InitializeAsync();

                    context.Token.ThrowIfCancellationRequested();

                    if (action == HarshProvisionerAction.Provision)
                    {
                        await OnProvisioningAsync();
                    }
                    else
                    {
                        await OnUnprovisioningAsync();
                    }

                    context.Token.ThrowIfCancellationRequested();
                }
                finally
                {
                    Complete();
                }
            }

            context.Session.OnProvisioningEnded(context, this);
        }

        PropertyValueSource ITrackValueSource.GetValueSource(PropertyInfo property)
            => _valueSourceTracker?.GetValueSource(property);

        void ITrackValueSource.SetValueSource(PropertyInfo property, PropertyValueSource source)
            => ValueSourceTracker.SetValueSource(property, source);
    }
}
