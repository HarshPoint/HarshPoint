using HarshPoint.Provisioning;
using HarshPoint.Provisioning.Implementation;
using HarshPoint.Tests;
using Moq;
using System.Collections.Generic;
using System.Collections.Immutable;
using Xunit;
using Xunit.Abstractions;

namespace ProgressInspector
{
    public class Inspector : SeriloggedTest
    {
        public Inspector(ITestOutputHelper output)
        : base(output)
        {
        }

        private Mock<IProvisioningSession> CreateSessionMock(
            HarshProvisionerAction action,
            IEnumerable<HarshProvisionerBase> provisioners = null
        )
        {
            var mock = new Mock<IProvisioningSession>(MockBehavior.Strict);
            mock.SetupGet<HarshProvisionerAction>(s => s.Action)
                .Returns(action);
            mock.SetupGet<IImmutableList<HarshProvisionerBase>>(s => s.Provisioners)
                .Returns(provisioners?.ToImmutableArray() ?? ImmutableArray.Create<HarshProvisionerBase>());
            return mock;
        }

        private Mock<IHarshProvisionerContext> CreateContextMock(
            IProvisioningSession session
        )
        {
            var mock = new Mock<IHarshProvisionerContext>(MockBehavior.Strict);
            mock.SetupGet<IProvisioningSession>(ctx => ctx.Session).Returns(session);

            return mock;
        }

        [Theory]
        [InlineData(HarshProvisionerAction.Provision)]
        [InlineData(HarshProvisionerAction.Unprovision)]
        public void Action_is_set(HarshProvisionerAction action)
        {
            var sessionMock = CreateSessionMock(action);
            var contextMock = CreateContextMock(sessionMock.Object);

            var inspector = new SessionProgressInspector();
            IProvisioningSessionInspector inspectorAsInterface = inspector;

            inspectorAsInterface.OnSessionStarting(contextMock.Object);

            Assert.Equal(action, inspector.Current.Action);
        }

        [Fact]
        public void SessionProgress_is_null_before_session_starting()
        {
            var sessionMock = CreateSessionMock(
                HarshProvisionerAction.Provision
            );
            var contextMock = CreateContextMock(sessionMock.Object);

            var inspector = new SessionProgressInspector();
            IProvisioningSessionInspector inspectorAsInterface = inspector;

            Assert.Null(inspector.Current);
        }

        [Theory]
        [InlineData(HarshProvisionerAction.Provision)]
        [InlineData(HarshProvisionerAction.Unprovision)]
        public void CompletedProvisionersCount_is_correct(HarshProvisionerAction action)
        {
            var provisioner1 = new TestProvisioner();
            var provisionerContainer = new HarshProvisioner();
            var provisioner2 = new TestProvisioner();

            var sessionMock = CreateSessionMock(
                action,
                new[] { provisioner1, provisionerContainer, provisioner2 }
            );
            var contextMock = CreateContextMock(sessionMock.Object);

            var inspector = new SessionProgressInspector();
            IProvisioningSessionInspector inspectorAsInterface = inspector;

            inspectorAsInterface.OnSessionStarting(contextMock.Object);
            Assert.Equal(0, inspector.Current.CompletedProvisionersCount);

            inspectorAsInterface.OnProvisioningStarting(contextMock.Object, provisioner1);
            Assert.Equal(0, inspector.Current.CompletedProvisionersCount);

            inspectorAsInterface.OnProvisioningEnded(contextMock.Object, provisioner1);
            Assert.Equal(1, inspector.Current.CompletedProvisionersCount);

            inspectorAsInterface.OnProvisioningStarting(contextMock.Object, provisionerContainer);
            Assert.Equal(1, inspector.Current.CompletedProvisionersCount);

            inspectorAsInterface.OnProvisioningEnded(contextMock.Object, provisionerContainer);
            Assert.Equal(1, inspector.Current.CompletedProvisionersCount);

            inspectorAsInterface.OnProvisioningStarting(contextMock.Object, provisioner2);
            Assert.Equal(1, inspector.Current.CompletedProvisionersCount);

            inspectorAsInterface.OnProvisioningEnded(contextMock.Object, provisioner2);
            Assert.Equal(2, inspector.Current.CompletedProvisionersCount);

            inspectorAsInterface.OnSessionEnded(contextMock.Object);
            Assert.Equal(2, inspector.Current.CompletedProvisionersCount);
        }

        [Fact]
        public void CompletedProvisionersCount_is_correct_when_skipped()
        {
            var provisioner1 = new TestProvisioner();
            var provisionerContainer = new HarshProvisioner();
            var provisioner2 = new TestProvisioner();

            var sessionMock = CreateSessionMock(
                HarshProvisionerAction.Unprovision,
                new[] { provisioner1, provisionerContainer, provisioner2 }
            );
            var contextMock = CreateContextMock(sessionMock.Object);

            var inspector = new SessionProgressInspector();
            IProvisioningSessionInspector inspectorAsInterface = inspector;

            inspectorAsInterface.OnSessionStarting(contextMock.Object);
            Assert.Equal(0, inspector.Current.CompletedProvisionersCount);

            inspectorAsInterface.OnProvisioningSkipped(contextMock.Object, provisioner1);
            Assert.Equal(1, inspector.Current.CompletedProvisionersCount);

            inspectorAsInterface.OnProvisioningSkipped(contextMock.Object, provisionerContainer);
            Assert.Equal(1, inspector.Current.CompletedProvisionersCount);

            inspectorAsInterface.OnProvisioningSkipped(contextMock.Object, provisioner2);
            Assert.Equal(2, inspector.Current.CompletedProvisionersCount);

            inspectorAsInterface.OnSessionEnded(contextMock.Object);
            Assert.Equal(2, inspector.Current.CompletedProvisionersCount);
        }

        [Fact]
        public void ProvisionersCount_is_correct()
        {
            var provisioner1 = new TestProvisioner();
            var provisionerContainer = new HarshProvisioner();
            var provisioner2 = new TestProvisioner();

            var sessionMock = CreateSessionMock(
                HarshProvisionerAction.Provision,
                new[] { provisioner1, provisionerContainer, provisioner2 }
            );
            var contextMock = CreateContextMock(sessionMock.Object);

            var inspector = new SessionProgressInspector();
            IProvisioningSessionInspector inspectorAsInterface = inspector;

            inspectorAsInterface.OnSessionStarting(contextMock.Object);
            Assert.Equal(2, inspector.Current.ProvisionersCount);
        }

        [Theory]
        [InlineData(HarshProvisionerAction.Provision)]
        [InlineData(HarshProvisionerAction.Unprovision)]
        public void CurrentProvisioner_is_correct(HarshProvisionerAction action)
        {
            var provisioner1 = new TestProvisioner();
            var provisionerContainer = new HarshProvisioner();
            var provisioner2 = new TestProvisioner();

            var sessionMock = CreateSessionMock(
                action,
                new[] { provisioner1, provisionerContainer, provisioner2 }
            );
            var contextMock = CreateContextMock(sessionMock.Object);

            var inspector = new SessionProgressInspector();
            IProvisioningSessionInspector inspectorAsInterface = inspector;

            inspectorAsInterface.OnSessionStarting(contextMock.Object);
            Assert.Null(inspector.Current.CurrentProvisioner);

            inspectorAsInterface.OnProvisioningStarting(contextMock.Object, provisioner1);
            Assert.Same(provisioner1, inspector.Current.CurrentProvisioner);
            Assert.False(inspector.Current.CurrentProvisionerIsSkipped);

            inspectorAsInterface.OnProvisioningEnded(contextMock.Object, provisioner1);
            Assert.Same(provisioner1, inspector.Current.CurrentProvisioner);
            Assert.False(inspector.Current.CurrentProvisionerIsSkipped);

            inspectorAsInterface.OnProvisioningStarting(contextMock.Object, provisionerContainer);
            Assert.Same(provisioner1, inspector.Current.CurrentProvisioner);
            Assert.False(inspector.Current.CurrentProvisionerIsSkipped);

            inspectorAsInterface.OnProvisioningEnded(contextMock.Object, provisionerContainer);
            Assert.Same(provisioner1, inspector.Current.CurrentProvisioner);
            Assert.False(inspector.Current.CurrentProvisionerIsSkipped);

            inspectorAsInterface.OnProvisioningStarting(contextMock.Object, provisioner2);
            Assert.Same(provisioner2, inspector.Current.CurrentProvisioner);
            Assert.False(inspector.Current.CurrentProvisionerIsSkipped);

            inspectorAsInterface.OnProvisioningEnded(contextMock.Object, provisioner2);
            Assert.Same(provisioner2, inspector.Current.CurrentProvisioner);
            Assert.False(inspector.Current.CurrentProvisionerIsSkipped);

            inspectorAsInterface.OnSessionEnded(contextMock.Object);
            Assert.Same(provisioner2, inspector.Current.CurrentProvisioner);
            Assert.False(inspector.Current.CurrentProvisionerIsSkipped);
        }

        [Fact]
        public void CurrentProvisioner_is_correct_when_skipped()
        {
            var provisioner1 = new TestProvisioner();
            var provisionerContainer = new HarshProvisioner();
            var provisioner2 = new TestProvisioner();

            var sessionMock = CreateSessionMock(
                HarshProvisionerAction.Unprovision,
                new[] { provisioner1, provisionerContainer, provisioner2 }
            );
            var contextMock = CreateContextMock(sessionMock.Object);

            var inspector = new SessionProgressInspector();
            IProvisioningSessionInspector inspectorAsInterface = inspector;

            inspectorAsInterface.OnSessionStarting(contextMock.Object);
            Assert.Null(inspector.Current.CurrentProvisioner);

            inspectorAsInterface.OnProvisioningSkipped(contextMock.Object, provisioner1);
            Assert.Same(provisioner1, inspector.Current.CurrentProvisioner);
            Assert.True(inspector.Current.CurrentProvisionerIsSkipped);

            inspectorAsInterface.OnProvisioningSkipped(contextMock.Object, provisionerContainer);
            Assert.Same(provisioner1, inspector.Current.CurrentProvisioner);
            Assert.True(inspector.Current.CurrentProvisionerIsSkipped);

            inspectorAsInterface.OnProvisioningStarting(contextMock.Object, provisioner2);
            Assert.Same(provisioner2, inspector.Current.CurrentProvisioner);
            Assert.False(inspector.Current.CurrentProvisionerIsSkipped);

            inspectorAsInterface.OnProvisioningEnded(contextMock.Object, provisioner2);
            Assert.Same(provisioner2, inspector.Current.CurrentProvisioner);
            Assert.False(inspector.Current.CurrentProvisionerIsSkipped);

            inspectorAsInterface.OnSessionEnded(contextMock.Object);
            Assert.Same(provisioner2, inspector.Current.CurrentProvisioner);
            Assert.False(inspector.Current.CurrentProvisionerIsSkipped);
        }

        public void PercentComplete_is_correct()
        {
            var provisioner1 = new TestProvisioner();
            var provisionerContainer = new HarshProvisioner();
            var provisioner2 = new TestProvisioner();

            var sessionMock = CreateSessionMock(
                HarshProvisionerAction.Provision,
                new[] { provisioner1, provisionerContainer, provisioner2 }
            );
            var contextMock = CreateContextMock(sessionMock.Object);

            var inspector = new SessionProgressInspector();
            IProvisioningSessionInspector inspectorAsInterface = inspector;

            inspectorAsInterface.OnSessionStarting(contextMock.Object);
            Assert.Equal(0, inspector.Current.PercentComplete);

            inspectorAsInterface.OnProvisioningStarting(contextMock.Object, provisioner1);
            Assert.Equal(0, inspector.Current.PercentComplete);

            inspectorAsInterface.OnProvisioningEnded(contextMock.Object, provisioner1);
            Assert.Equal(50, inspector.Current.PercentComplete);

            inspectorAsInterface.OnProvisioningStarting(contextMock.Object, provisionerContainer);
            Assert.Equal(50, inspector.Current.PercentComplete);

            inspectorAsInterface.OnProvisioningEnded(contextMock.Object, provisionerContainer);
            Assert.Equal(50, inspector.Current.PercentComplete);

            inspectorAsInterface.OnProvisioningStarting(contextMock.Object, provisioner2);
            Assert.Equal(50, inspector.Current.PercentComplete);

            inspectorAsInterface.OnProvisioningEnded(contextMock.Object, provisioner2);
            Assert.Equal(100, inspector.Current.PercentComplete);

            inspectorAsInterface.OnSessionEnded(contextMock.Object);
            Assert.Equal(100, inspector.Current.PercentComplete);
        }


        private class TestProvisioner : HarshProvisioner
        {
        }
    }
}