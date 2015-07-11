using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Moq;
using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class ResolvedParameterBinding : SeriloggedTest
    {
        public ResolvedParameterBinding(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Compatible_IResolver_result_gets_assigned_to_IResolve()
        {
            var parameters = new ParameterSetBuilder(typeof(SimpleTarget))
                .Build()
                .SelectMany(set => set.Parameters);

            var binder = new ResolvedParameterBinder(parameters);
            var target = new SimpleTarget()
            {
                Param = MockResolver<String>(
                    MockResolverResult("42")
                )
            };

            binder.Bind(target, Mock.Of<IResolveContext>());

            Assert.NotNull(target.Param);
            Assert.Equal("42", target.Param.First());
        }

        [Fact]
        public void Incompatible_results_throws_invalid_operation_exception()
        {
            var parameters = new ParameterSetBuilder(typeof(SimpleTarget))
                .Build()
                .SelectMany(set => set.Parameters);

            var binder = new ResolvedParameterBinder(parameters);
            var target = new SimpleTarget()
            {
                Param = MockResolver<String>(
                    MockResolverResult<Int32>(42)
                )
            };

            Assert.Throws<InvalidOperationException>(
                () => binder.Bind(target, Mock.Of<IResolveContext>())
            );
        }

        private IResolve<T> MockResolverResult<T>(params T[] results)
        {
            var mock = new Mock<IResolve<T>>();

            mock.Setup(x => x.GetEnumerator())
                .Returns(() => results.AsEnumerable().GetEnumerator());

            return mock.Object;
        }

        private IResolve<T> MockResolver<T>(Object result)
        {
            var mock = new Mock<IResolve<T>>();

            mock.As<IIndirectResolver>()
                .Setup(x => x.Initialize(It.IsAny<IResolveContext>()))
                .Returns(result);

            return mock.Object;
        }

        private sealed class SimpleTarget
        {
            [Parameter]
            public IResolve<String> Param { get; set; }
        }
    }
}
