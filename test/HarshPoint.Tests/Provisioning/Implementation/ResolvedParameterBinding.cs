using HarshPoint.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using System.Collections;
using HarshPoint.Provisioning.Implementation;
using Moq;

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
            Assert.Equal("42", target.Param.Result);
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

        private IResolve2<T> MockResolverResult<T>(params T[] results)
        {
            var mock = new Mock<IResolve2<T>>();

            mock.Setup(x => x.Result)
                .Returns(() => results.Single());

            mock.Setup(x => x.GetEnumerator())
                .Returns(() => results.AsEnumerable().GetEnumerator());

            return mock.Object;
        }

        private IResolve2<T> MockResolver<T>(Object result)
        {
            var mock = new Mock<IResolve2<T>>();

            mock.As<IResolver>()
                .Setup(x => x.Resolve(It.IsAny<IResolveContext>()))
                .Returns(result);

            return mock.Object;
        }

        private sealed class SimpleTarget
        {
            [Parameter]
            public IResolve2<String> Param { get; set; }
        }
    }
}
