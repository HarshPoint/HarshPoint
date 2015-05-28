using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning.Implementation
{
    public abstract class ResolvableChain
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

            var result = Clone();
            result.Chain.Last().Next = other.Clone();
            return result;
        }

        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        protected async Task<IEnumerable<T>> ResolveChain<T>(HarshProvisionerContextBase context)
        {
            var resultSets = await Chain
                .Cast<IResolvableChainElement<T>>()
                .SelectSequentially(e => e.ResolveChainElement(context));
                
            return resultSets.SelectMany(r => r);
        }

        protected async Task<T> ResolveChainSingle<T>(HarshProvisionerContextBase context)
        {
            var results = (await ResolveChain<T>(context)).ToArray();

            switch (results.Length)
            {
                case 1: return results[0];
                case 0: throw Error.InvalidOperation(SR.ResolvableChain_NoResult, this);
                default: throw Error.InvalidOperation(SR.ResolvableChain_ManyResults, this);
            }
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

        protected static TContext ValidateContext<TContext>(HarshProvisionerContextBase context)
            where TContext : HarshProvisionerContextBase
        {
            if (context == null)
            {
                throw Error.ArgumentNull(nameof(context));
            }

            var typedContext = (context as TContext);

            if (typedContext == null)
            {
                throw Error.ArgumentOutOfRange_ObjectNotAssignableTo(
                    nameof(context),
                    typeof(TContext).GetTypeInfo(),
                    context
                );
            }

            return typedContext;
        }
    }
}
