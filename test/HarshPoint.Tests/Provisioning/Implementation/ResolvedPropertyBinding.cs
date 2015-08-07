using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class ResolvedPropertyBinding : SharePointClientTest
    {
        public ResolvedPropertyBinding(SharePointClientFixture fixture, ITestOutputHelper output) : base(fixture, output)
        {
            Binder = new ResolvedPropertyBinder(GetType());
        }

        [Fact]
        public void Resolve_existing_value()
        {
            SimpleResolve = MockResolve();
            BindResolves();

            Assert.NotNull(SimpleResolve);
            Assert.Equal(ExpectedArray, SimpleResolve);
        }

        [Fact]
        public void ResolveSingle()
        {
            SingleResolve = MockSingleResolve();
            BindResolves();

            Assert.NotNull(SingleResolve);
            Assert.Equal("42", SingleResolve.Value);
        }

        [Fact]
        public void ResolveSingleOrDefault()
        {
            SingleOrDefaultResolve = MockSingleOrDefaultResolve();
            BindResolves();

            Assert.NotNull(SingleOrDefaultResolve);
            Assert.Equal(42, SingleOrDefaultResolve.Value);
        }

        private void BindResolves()
        {
            Binder.Bind(this, () => Mock.Of<IResolveContext>());
        }

        private ResolvedPropertyBinder Binder { get; set; }

        private IResolveSingleOrDefault<Int32> MockSingleOrDefaultResolve()
            => MockResolveBuilder(new Object[] { 42 }).As<IResolveSingleOrDefault<Int32>>().Object;

        private IResolveSingle<String> MockSingleResolve()
            => MockResolveBuilder(new Object[] { "42" }).As<IResolveSingle<String>>().Object;

        private IResolve<String> MockResolve(IEnumerable<String> result = null)
            => MockResolveBuilder(result).As<IResolve<String>>().Object;

        private static Mock<IResolveBuilder> MockResolveBuilder(IEnumerable<Object> result)
        {
            var mock = new Mock<IResolveBuilder>();

            mock.Setup(x => x.ToEnumerable(It.IsAny<IResolveContext>(), It.IsAny<Object>()))
                .Returns((result ?? ExpectedArray));

            return mock;
        }

        private IResolve<String> SimpleResolve { get; set; }

        private IResolveSingle<String> SingleResolve { get; set; }

        private IResolveSingleOrDefault<Int32> SingleOrDefaultResolve { get; set; }

        private static readonly IEnumerable<String> ExpectedArray = ImmutableArray.Create("42", "4242");
    }
}
