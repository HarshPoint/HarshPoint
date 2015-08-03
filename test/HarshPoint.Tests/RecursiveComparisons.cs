using System;
using Xunit;

namespace HarshPoint.Tests
{
    public class RecursiveComparisons
    {
        [Fact]
        public void Computes_hashcode_simple_objects()
        {
            var x = new C { a = 42, b = "123" };
            var y = new C { a = 32, b = "123" };

            var comparer = new HarshRecursiveEqualityComparer();
            comparer.AddProperty<C>(c => c.b);

            Assert.Equal(
                comparer.GetHashCode(x),
                comparer.GetHashCode(y)
            );

            comparer.AddProperty<C>(c => c.a);

            Assert.NotEqual(
                comparer.GetHashCode(x),
                comparer.GetHashCode(y)
            );
        }

        [Fact]
        public void Compares_simple_objects()
        {
            var x = new C { a = 42, b = "123" };
            var y = new C { a = 32, b = "123" };

            var comparer = new HarshRecursiveEqualityComparer();
            comparer.AddProperty<C>(c => c.b);

            Assert.True(comparer.Equals(x, y));

            comparer.AddProperty<C>(c => c.a);
            Assert.False(comparer.Equals(x, y));
        }

        [Fact]
        public void Compares_collections()
        {
            var x = new[]
            {
                new C { a = 42, b="123" },
                new C { a = 142, b= "123" },
            };

            var y = new[]
            {
                new C { a = 42, b="12344" },
                new C { a = 142, b= "12344" },
            };

            var comparer = new HarshRecursiveEqualityComparer();
            comparer.AddProperty<C>(c => c.a);

            Assert.True(comparer.Equals(x, y));

            comparer.AddProperty<C>(c => c.b);
            Assert.False(comparer.Equals(x, y));
        }

        private sealed class C
        {
            public Int32 a;
            public String b;
        }
    }
}
