using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    internal sealed class ResolveResultSingleOrDefault<T> : ResolveResultBase, IResolveSingleOrDefault<T>
    {
        public T Value
        {
            get
            {
                var array = Results.Cast<T>().ToArray();

                switch (array.Length)
                {
                    case 1: return array[0];
                    case 0: return default(T);
                    default: throw Logger.Fatal.InvalidOperationFormat(SR.Resolvable_ManyResults, ResolveBuilder);
                }
            }
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext(typeof(ResolveResultSingle<>));
    }
}
