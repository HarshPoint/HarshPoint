using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultSingle<T> : ResolveResultBase, IResolveSingle<T>
    {
        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveResultSingle<>));

        public T Value
        {
            get
            {
                var array = EnumerateResults<T>();

                switch (array.Count)
                {
                    case 1: return array[0];
                    case 0: throw Logger.Fatal.InvalidOperationFormat(SR.Resolvable_NoResult, ResolveBuilder);
                    default: throw Logger.Fatal.InvalidOperationFormat(SR.Resolvable_ManyResults, ResolveBuilder);
                }
            }
        }
    }
}
