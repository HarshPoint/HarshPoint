using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint
{
    internal class HarshEqualityComparer<TSource, TResult> : IEqualityComparer<TSource>
    {
        private readonly Func<TSource, TResult> _selector;
        private readonly IEqualityComparer<TResult> _resultComparer;

        public HarshEqualityComparer(Func<TSource, TResult>  selector)
            : this(selector, null)
        {
        }

        public HarshEqualityComparer(Func<TSource, TResult> selector, IEqualityComparer<TResult> resultComparer)
        {
            if (selector == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(selector));
            }

            _selector = selector;
            _resultComparer = resultComparer ?? EqualityComparer<TResult>.Default;
        }

        public Boolean Equals(TSource x, TSource y)
        {
            return _resultComparer.Equals(_selector.Invoke(x), _selector.Invoke(y));
        }

        public Int32 GetHashCode(TSource obj)
        {
            return _resultComparer.GetHashCode(_selector.Invoke(obj));
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(HarshEqualityComparer<,>));
    }
}
