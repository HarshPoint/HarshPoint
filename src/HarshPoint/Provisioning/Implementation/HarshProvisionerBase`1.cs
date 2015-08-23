using HarshPoint.ObjectModel;
using HarshPoint.Provisioning.Records;
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
        private readonly HarshScopedValue<RecordWriter<TContext>> _recordWriter;

        private ManualResolver _manualResolver;
        private HarshProvisionerMetadata _metadata;
        private PropertyValueSourceTracker _valueSourceTracker;

        protected HarshProvisionerBase()
        {
            _context = new HarshScopedValue<TContext>();
            _parameterSet = new HarshScopedValue<ParameterSet>();
            _recordWriter = new HarshScopedValue<RecordWriter<TContext>>();

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

        protected RecordWriter<TContext> WriteRecord
        {
            get
            {
                if (_recordWriter == null)
                {
                    throw Logger.Fatal.InvalidOperation(
                        SR.HarshProvisionerBase_NoContext
                    );
                }

                return _recordWriter.Value;
            }
        }

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
            => ProvisionAsync(context.WithToken(token));

        public Task ProvisionAsync(
            TContext context,
            IProgress<ProgressReport> progress
        )
            => ProvisionAsync(context.WithProgress(progress));

        public Task ProvisionAsync(
            TContext context,
            CancellationToken token,
            IProgress<ProgressReport> progress
        )
            => ProvisionAsync(context
                .WithToken(token)
                .WithProgress(progress)
            );

        public Task ProvisionAsync(TContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            if (context.Session == null)
            {
                context = context.WithSession(new ProvisioningSession(this, HarshProvisionerAction.Provision));
            }

            return RunSelfAndChildren(
                context,
                HarshProvisionerAction.Provision
            );
        }

        public Task UnprovisionAsync(TContext context, CancellationToken token)
            => UnprovisionAsync(context.WithToken(token));

        public Task UnprovisionAsync(
            TContext context,
            IProgress<ProgressReport> progress
        )
            => UnprovisionAsync(context.WithProgress(progress));

        public Task UnprovisionAsync(
            TContext context,
            CancellationToken token,
            IProgress<ProgressReport> progress
        )
            => UnprovisionAsync(context
                .WithToken(token)
                .WithProgress(progress)
            );

        public async Task UnprovisionAsync(TContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            if (context.Session == null)
            {
                context = context.WithSession(new ProvisioningSession(this, HarshProvisionerAction.Unprovision));
            }

            if (MayDeleteUserData || context.MayDeleteUserData || !Metadata.UnprovisionDeletesUserData)
            {
                await RunSelfAndChildren(
                    context,
                    HarshProvisionerAction.Unprovision
                );
            }
        }

        protected internal Boolean IsValueDefaultFromContext(Expression<Func<Object>> expression)
            => _valueSourceTracker?.GetValueSource(expression)
                == DefaultFromContextPropertyValueSource.Instance;

        protected void ForwardsTo(HarshProvisionerBase<TContext> target)
        {
            ForwardTarget = target;
        }

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

        protected virtual Task InitializeAsync() => HarshTask.Completed;

        protected virtual void Complete() { }

        protected virtual Task OnProvisioningAsync() => HarshTask.Completed;

        protected virtual void OnValidating() { }

        [NeverDeletesUserData]
        protected virtual Task OnUnprovisioningAsync() => HarshTask.Completed;

        protected virtual ResolveContext<TContext> CreateResolveContext()
            => new ResolveContext<TContext>(Context);

        internal virtual ManualResolver CreateManualResolver(Func<IResolveContext> resolveContextFactory)
            => new ManualResolver(resolveContextFactory);

        internal virtual Task OnResolvedPropertiesBound() => HarshTask.Completed;

        protected abstract Task ProvisionChild(
            HarshProvisionerBase provisioner,
            TContext context
        );

        protected abstract Task UnprovisionChild(
            HarshProvisionerBase provisioner,
            TContext context
        );

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

        private async Task RunChildren(
            HarshProvisionerAction action)
        {
            if (!HasChildren)
            {
                return;
            }

            var children = (action == HarshProvisionerAction.Provision) ? Children : Children.Reverse();
            var context = PrepareChildrenContext();

            foreach (var child in children)
            {
                context.Token.ThrowIfCancellationRequested();
                if (action == HarshProvisionerAction.Provision)
                {
                    await ProvisionChild(child, context);
                }
                else
                {
                    await ProvisionChild(child, context);
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

            var recordWriter = new RecordWriter<TContext>(context);

            using (_context.Enter(context))
            using (_logger.Enter(contextLogger))
            using (_recordWriter.Enter(recordWriter))
            {
                await action();
            }
        }

        private Task RunSelfAndChildren(
            TContext context,
            HarshProvisionerAction action
        )
            => RunWithContext(context, async delegate
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

                await OnResolvedPropertiesBound();
                context.Token.ThrowIfCancellationRequested();

                // parameter set resolving depends on values
                // from context being already set

                using (_parameterSet.Enter(ResolveParameterSet()))
                {
                    try
                    {
                        ValidateParameters();
                        OnValidating();

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

                await RunChildren(action);
            });

        PropertyValueSource ITrackValueSource.GetValueSource(PropertyInfo property)
            => _valueSourceTracker?.GetValueSource(property);

        void ITrackValueSource.SetValueSource(PropertyInfo property, PropertyValueSource source)
            => ValueSourceTracker.SetValueSource(property, source);
    }
}
