using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HarshPoint.Tests.Provisioning
{
    public class ResolvableTests : IClassFixture<SharePointClientFixture>
    {
        public ResolvableTests(SharePointClientFixture fixture)
        {
            ClientOM = fixture;
        }

        public SharePointClientFixture ClientOM { get; private set; }

        [Fact]
        public async Task TestResolvables_can_be_combined()
        {
            var foo = new TestResolvable<String>("foo");
            var bar = new TestResolvable<String>("bar");

            var combined = (IResolve<String>)foo.And(bar);

            Assert.NotNull(combined);
            Assert.NotSame(combined, foo);
            Assert.NotSame(combined, bar);

            var results = await combined.ResolveAsync(ClientOM.ResolveContext);

            Assert.Equal(2, results.Count());
            Assert.Contains("foo", results);
            Assert.Contains("bar", results);
        }

        [Fact]
        public async Task NestedResolvable_returns_grouping()
        {
            var root = new TestResolvable<String>("42");
            IResolve<IGrouping<String, Int32>> nested = new NestedTestResolvable<String, Int32>(root, 1);

            var results = await nested.ResolveAsync(ClientOM.ResolveContext);
            Assert.NotNull(results);

            var group = Assert.Single(results);
            Assert.Equal("42", group.Key);

            var child = Assert.Single(group);
            Assert.Equal(42, child);
        }

        [Fact]
        public async Task NestedResolvable_returns_child()
        {
            var root = new TestResolvable<String>("42");
            IResolve<Int32> nested = new NestedTestResolvable<String, Int32>(root, 1);

            var results = await nested.ResolveAsync(ClientOM.ResolveContext);
            Assert.NotNull(results);

            var child = Assert.Single(results);
            Assert.Equal(42, child);

        }

        [Fact]
        public async Task NestedResolvable_returns_more_groupings()
        {
            var root = new TestResolvable<String>("42", "4242");
            var nested = new NestedTestResolvable<String, Int32>(root, 2);

            var results = await ((IResolve<IGrouping<String, Int32>>)(nested)).ResolveAsync(ClientOM.ResolveContext);
            Assert.NotNull(results);

            Assert.Equal(2, results.Count());
            Assert.Contains("42", results.Select(r => r.Key));
            Assert.Contains("4242", results.Select(r => r.Key));

            var first = results.SingleOrDefault(x => x.Key == "42");
            Assert.Equal(2, first.Count());
            Assert.All(first, x => Assert.Equal(42, x));

            var second = results.SingleOrDefault(x => x.Key == "4242");
            Assert.Equal(2, second.Count());
            Assert.All(second, x => Assert.Equal(4242, x));
        }

        [Fact]
        public async Task NestedResolvable_returns_more_children()
        {
            var root = new TestResolvable<String>("42", "4242");
            IResolve<Int32> nested = new NestedTestResolvable<String, Int32>(root, 2);

            var results = await nested.ResolveAsync(ClientOM.ResolveContext);
            Assert.NotNull(results);

            Assert.Equal(4, results.Count());
            Assert.Equal(2, results.Count(x => x == 42));
            Assert.Equal(2, results.Count(x => x == 4242));

        }

        [Fact]
        public async Task Normal_and_nested_can_be_combined()
        {
            var normal = new TestResolvable<Int32>(42);
            var nested = new NestedTestResolvable<Int32, Int32>(normal, 2)
            {
                Converter = x => x * 2
            };

            var combined = normal.And(nested);
            var results = await ((IResolve<Int32>)(combined)).ResolveAsync(ClientOM.ResolveContext);

            Assert.Equal(3, results.Count());
            Assert.Equal(2, results.Count(x => x == 84));
            Assert.Contains(42, results);
        }

        [Fact]
        public async Task Nested_and_normal_can_be_combined()
        {
            var normal = new TestResolvable<Int32>(42);
            var nested = new NestedTestResolvable<Int32, Int32>(normal, 2)
            {
                Converter = x => x * 2
            };

            var combined = nested.And(normal);
            var results = await ((IResolve<Int32>)(combined)).ResolveAsync(ClientOM.ResolveContext);

            Assert.Equal(3, results.Count());
            Assert.Equal(2, results.Count(x => x == 84));
            Assert.Contains(42, results);
        }

        [Fact]
        public async Task Nested_combined_with_normal_crashes_when_trying_to_resolve_grouping()
        {
            var normal = new TestResolvable<Int32>(42);
            var nested = new NestedTestResolvable<Int32, Int32>(normal, 2);

            var combined = nested.And(normal);

            await Assert.ThrowsAsync<InvalidCastException>(
                () => ((IResolve<IGrouping<Int32, Int32>>)(combined)).ResolveAsync(ClientOM.ResolveContext)
            );
        }

        public class TestResolvable<T> : Resolvable<T, HarshProvisionerContext, TestResolvable<T>>
        {
            public TestResolvable(params T[] results)
            {
                Results = results;
            }

            public T[] Results { get; private set; }

            protected override Task<IEnumerable<T>> ResolveChainElement(HarshProvisionerContext context)
            {
                return Task.FromResult<IEnumerable<T>>(Results);
            }
        }

        public class NestedTestResolvable<T1, T2> : NestedResolvable<T1, T2, HarshProvisionerContext, NestedTestResolvable<T1, T2>>
        {
            public NestedTestResolvable(IResolve<T1> parent, Int32 count)
                : base(parent)
            {
                Count = count;
                Converter = x => (T2)Convert.ChangeType(x, typeof(T2));
            }

            public Int32 Count
            {
                get;
                set;
            }

            public Func<T1, T2> Converter
            {
                get;
                set;
            }

            protected override Task<IEnumerable<T2>> ResolveChainElement(HarshProvisionerContext context, T1 parent)
            {
                return Task.FromResult(
                    Enumerable.Repeat(
                        Converter(parent),
                        Count
                    )
                );
            }
        }
    }
}