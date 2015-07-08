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
                Param = MockResolver("42")
            };

            binder.Bind(target, Mock.Of<IResolveContext>());

            Assert.NotNull(target.Param);
            Assert.Equal("42", target.Param.Result);
        }

        private IResolve2<String> MockResolverResult(params String[] results)
        {
            var mock = new Mock<IResolve2<String>>();

            mock.Setup(x => x.Result)
                .Returns(() => results.Single());

            mock.Setup(x => x.GetEnumerator())
                .Returns(() => results.AsEnumerable().GetEnumerator());

            return mock.Object;
        }

        private IResolve2<String> MockResolver(params String[] results)
        {
            var mock = new Mock<IResolve2<String>>();

            mock.As<IResolver>()
                .Setup(x => x.Resolve(It.IsAny<IResolveContext>()))
                .Returns(MockResolverResult(results));

            return mock.Object;
        }
        
        private sealed class SimpleTarget
        {
            [Parameter]
            public IResolve2<String> Param { get; set; }
        }
    }
}
