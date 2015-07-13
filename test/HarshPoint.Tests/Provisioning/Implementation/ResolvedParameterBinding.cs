using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Moq;
using System;
using System.Collections;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace HarshPoint.Tests.Provisioning.Implementation
{
    public class ResolvedParameterBinding : SharePointClientTest
    {
        public ResolvedParameterBinding(SharePointClientFixture fixture, ITestOutputHelper output) : base(fixture, output)
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
                    new[] { "42" }
                )
            };

            binder.Bind(target, Fixture.Context);

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
                    new[] { 42 }
                )
            };

            Assert.Throws<ArgumentOutOfRangeException>(
                () => binder.Bind(target, Fixture.Context)
            );
        }
        

        private IResolve<T> MockResolver<T>(IEnumerable result)
        {
            var mock = new Mock<IResolve<T>>();

            mock.As<IResolveBuilder<HarshProvisionerContext>>()
                .Setup(x => x.ToEnumerable(It.IsAny<Object>(), It.IsAny<ResolveContext<HarshProvisionerContext>>()))
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
