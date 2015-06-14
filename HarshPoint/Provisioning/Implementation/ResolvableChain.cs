using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ResolvableChain : IHarshCloneable
    {
        public virtual ResolvableChain Clone()
        {
            var result = (ResolvableChain)MemberwiseClone();

            if (Next != null)
            {
                result.Next = Next.Clone();
            }

            return result;
        }

        Object IHarshCloneable.Clone() => Clone();

        //public override String ToString()
        //{
        //    return DapValueLogFormatter.Format(
        //        Chain.Select(x => x.ToLogObject())
        //    );
        //}

        protected ResolvableChain And(ResolvableChain other)
        {
            if (other == null)
            {
                throw Error.ArgumentNull(nameof(other));
            }

            return this.With(c => c.Chain.Last().Next = other.Clone());
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected async Task<IEnumerable<T>> ResolveChain<T>(IResolveContext context)
        {
            var resultSets = await Chain
                .Cast<IResolvableChainElement<T>>()
                .SelectSequentially(e => e.ResolveChainElement(context));
                
            return resultSets.SelectMany(r => r);
        }
    
        protected virtual Object ToLogObject()
        {
            return this;
        }

        private IEnumerable<ResolvableChain> Chain
        {
            get
            {
                var current = this;

                while (current != null)
                {
                    yield return current;
                    current = current.Next;
                }
            }
        }

        private ResolvableChain Next
        {
            get;
            set;
        }

        protected static ResolveContext<TContext> ValidateContext<TContext>(IResolveContext context)
            where TContext : HarshProvisionerContextBase
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            var typedContext = (context as ResolveContext<TContext>);

            if (typedContext == null)
            {
                throw Error.ArgumentOutOfRange_ObjectNotAssignableTo(
                    nameof(context),
                    typeof(ResolveContext<TContext>).GetTypeInfo(),
                    context
                );
            }

            return typedContext;
        }
    }
}
