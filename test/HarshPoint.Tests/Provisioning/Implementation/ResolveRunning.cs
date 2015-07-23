using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class ResolveRunning : SharePointClientTest
    {
        public ResolveRunning(SharePointClientFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
        }

        [Fact]
        public void Resolve_existing_value()
        {
            var collection = new ResolveRunnerDefinitionCollection();
            collection.Add(() => SimpleResolve);

            SimpleResolve = MockResolve();

            Run(collection);

            Assert.NotNull(SimpleResolve);
            Assert.Equal(ExpectedArray, SimpleResolve);
        }

        [Fact]
        public void Resolve_factory()
        {
            var collection = new ResolveRunnerDefinitionCollection();

            collection.Add(
                () => SimpleResolve,
                () => MockResolve()
            );

            Run(collection);

            Assert.NotNull(SimpleResolve);
            Assert.Equal(ExpectedArray, SimpleResolve);
        }

        [Fact]
        public void ResolveSingle_factory()
        {
            var collection = new ResolveRunnerDefinitionCollection();

            collection.Add(
                () => SingleResolve,
                () => MockSingleResolve()
            );

            Run(collection);

            Assert.NotNull(SingleResolve);
            Assert.Equal("42", SingleResolve.Value);
        }

        [Fact]
        public void ResolveSingleOrDefault_factory()
        {
            var collection = new ResolveRunnerDefinitionCollection();

            collection.Add(
                () => SingleOrDefaultResolve,
                () => MockSingleOrDefaultResolve()
            );

            Run(collection);

            Assert.NotNull(SingleOrDefaultResolve);
            Assert.Equal(42, SingleOrDefaultResolve.Value);
        }

        private void Run(ResolveRunnerDefinitionCollection collection)
        {
            var runners = collection.Select(
                rrd => new ResolvedPropertyBinder(rrd, () => Mock.Of<IResolveContext>())
            );

            foreach (var runner in runners)
            {
                runner.Resolve(this);
            }
        }

        private IResolveSingleOrDefault<Int32> MockSingleOrDefaultResolve()
            => MockResolveBuilder(new[] { 42 }).As<IResolveSingleOrDefault<Int32>>().Object;


        private IResolveSingle<String> MockSingleResolve()
            => MockResolveBuilder(new[] { "42" }).As<IResolveSingle<String>>().Object;

        private IResolve<String> MockResolve(IEnumerable<String> result = null)
        => MockResolveBuilder(result).As<IResolve<String>>().Object;

        private static Mock<IResolveBuilder> MockResolveBuilder<T>(IEnumerable<T> result)
        {
            var mock = new Mock<IResolveBuilder>();

            mock.Setup(x => x.ToEnumerable(It.IsAny<Object>(), It.IsAny<IResolveContext>()))
                .Returns(((IEnumerable)result ?? ExpectedArray));

            return mock;
        }

        private IResolve<String> SimpleResolve { get; set; }

        private IResolveSingle<String> SingleResolve { get; set; }

        private IResolveSingleOrDefault<Int32> SingleOrDefaultResolve { get; set; }

        private static readonly IEnumerable<String> ExpectedArray = ImmutableArray.Create("42", "4242");
    }
}
