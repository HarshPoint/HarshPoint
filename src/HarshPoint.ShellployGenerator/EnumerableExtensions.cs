using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.ShellployGenerator
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<TValue> AsCollection<TValue>(this TValue value)
            => new[] { value };
    }
}
