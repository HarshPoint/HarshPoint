using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using Moq;
using System;
using System.Linq;

namespace HarshPoint.Tests.Provisioning
{
    public class MockResolve
    {
        public static IResolveBuilder<TResult> Build<TResult>(
            params TResult[] results
        )
            => Mock(results).Object;

        public static Mock<IResolveBuilder<TResult>> Mock<TResult>(
            params TResult[] results
        )
        {
            var mock = new Mock<IResolveBuilder>();

            mock.Setup(x => x.ToEnumerable(It.IsAny<IResolveContext>(), It.IsAny<Object>()))
                .Returns(() => results.Cast<Object>());

            return mock.As<IResolveBuilder<TResult>>();
        }
    }
}
