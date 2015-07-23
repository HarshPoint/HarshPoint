using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolvedPropertyBinder
    {
        public ResolvedPropertyBinder(ResolvedProperty definition, Func<IResolveContext> resolveContextFactory)
        {
            if (definition == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(definition));
            }

            if (resolveContextFactory == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(resolveContextFactory));
            }

            Definition = definition;
            ResolveContextFactory = resolveContextFactory;
        }

        public void Resolve(Object target)
        {
            if (target == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(target));
            }

            Target = target;

            if (!EnsureValue()) return;
            if (!EnsureResolveBuilder()) return;

            CreateResolveContext();
            InitializeResolveContext();

            if (CreateResultSource())
            {
                AssignResult();
            }
        }

        private ResolvedProperty Definition { get; set; }

        private Func<IResolveContext> ResolveContextFactory { get; set; }

        private String Name => Definition.Name;

        private Boolean EnsureValue()
        {
            Value = Definition.Getter(Target);

            if ((Value == null) && (Definition.ResolveBuilderFactory != null))
            {
                Logger.Debug(
                    "Resolve {ResolveName} is null, invoking factory method.",
                    Name
                );

                Value = Definition.ResolveBuilderFactory();
            }

            if (Value == null)
            {
                Logger.Debug(
                    "Resolve {ResolveName} is null, skipping.",
                    Name
                );

                return false;
            }

            return true;
        }

        private Boolean EnsureResolveBuilder()
        {
            ResolveBuilder = Value as IResolveBuilder;

            if (ResolveBuilder == null)
            {
                Logger.Debug(
                    "Resolve {ResolveName} value {$Value} is not an IResolveBuilder, skipping.",
                    Name,
                    Value
                );

                return false;
            }

            return true;
        }

        private void CreateResolveContext()
        {
            ResolveContext = ResolveContextFactory();

            if (ResolveContext == null)
            {
                throw Logger.Fatal.InvalidOperation(
                    SR.ResolveRegistrar_ContextFactoryReturnedNull
                );
            }
        }

        private void InitializeResolveContext()
        {
            Logger.Debug(
                "Resolve {ResolveName} resolver {$Resolver} initializing context.",
                Name,
                ResolveBuilder
            );

            ResolveBuilder.InitializeContext(ResolveContext);
        }

        private Boolean CreateResultSource()
        {
            Logger.Debug(
                "Resolve {ResolveName} resolver {$Resolver} initializing.",
                Name,
                ResolveBuilder
            );

            ResultSource = ResolveBuilder.ToEnumerable(
                ResolveBuilder.Initialize(ResolveContext),
                ResolveContext
            );

            if (ResultSource == null)
            {
                Logger.Warning(
                    "Resolve {ResolveName} resolver {$Resolver} resolved into null value.",
                    Name,
                    ResolveBuilder
                );

                Definition.Setter(Target, null);
                return false;
            }

            Logger.Debug(
                "Resolve {ResolveName} resolver {$Resolver} resolved into value {$Value}.",
                Name,
                ResolveBuilder,
                ResultSource
            );

            return true;
        }

        private void AssignResult()
        {
            var result = ResolveResultFactory.CreateResult(
                Definition.PropertyTypeInfo,
                ResultSource,
                ResolveBuilder
            );

            Logger.Debug(
                "Resolve {ResolveName} resolver {$Resolver} result adapted into {$Value}, assigning.",
                Name,
                ResolveBuilder,
                result
            );

            Definition.Setter(Target, result);
        }

        private Object Value { get; set; }

        private Object Target { get; set; }

        private IResolveBuilder ResolveBuilder { get; set; }

        private IResolveContext ResolveContext { get; set; }

        private IEnumerable ResultSource { get; set; }

        private static readonly HarshLogger Logger = HarshLog.ForContext<ResolvedPropertyBinder>();
    }
}
