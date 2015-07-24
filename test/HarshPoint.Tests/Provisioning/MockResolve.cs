using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using System;
using Moq;

namespace HarshPoint.Tests.Provisioning
{
    public class MockResolve
    {
        public static IResolveBuilder<TResult, IResolveContext> Build<TResult>(
            params TResult[] results
        )
            => Mock<TResult>(results).Object;

        public static Mock<IResolveBuilder<TResult, IResolveContext>> Mock<TResult>(
            params TResult[] results
        )
            => Mock<TResult, IResolveContext>(results);

        public static Mock<IResolveBuilder<TResult, TContext>> Mock<TResult, TContext>(
            params TResult[] results
        )
            where TContext : IResolveContext
        {
            var mock = new Mock<IResolveBuilder<TResult, TContext>>();

            mock.Setup(x => x.ToEnumerable(It.IsAny<Object>(), It.IsAny<IResolveContext>()))
                .Returns(results);

            return mock;
        }
    }
}
