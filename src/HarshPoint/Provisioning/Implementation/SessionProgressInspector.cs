using System;
using System.Linq;
using System.Reflection;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class SessionProgressInspector : IProvisioningSessionInspector
    {
        private readonly Object _syncRoot = new Object();

        private IProvisioningSession _session;
        private HarshProvisionerBase _currentProvisioner;
        private Boolean _currentProvisionerIsSkipped;
        private Int32 _completedProvisionersCount;

        public SessionProgress Current
        {
            get
            {
                lock (_syncRoot)
                {
                    if (_session == null)
                    {
                        return null;
                    }

                    var provisionersCount = _session.Provisioners
                        .Where(p => !IsContainerOnly(p))
                        .Count();

                    return new SessionProgress(
                        _session.Action,
                        _currentProvisioner,
                        _currentProvisionerIsSkipped,
                        _completedProvisionersCount,
                        provisionersCount,
                        GetPercentComplete(_completedProvisionersCount, provisionersCount),
                        GetRemainingTime()
                    );
                }
            }
        }

        private static Boolean IsContainerOnly(HarshProvisionerBase provisioner)
        {
            var baseType = provisioner.GetType().GetTypeInfo().BaseType;
            return
                baseType.IsConstructedGenericType &&
                baseType.GetGenericTypeDefinition() == typeof(HarshProvisionerBase<>);
        }

        private static Int32 GetPercentComplete(
            Int32 completed,
            Int32 total
        )
        {
            if (total == 0)
            {
                return 0;
            }

            return Math.Min(100, 100 * completed / total);
        }

        private TimeSpan? GetRemainingTime() => null; // TODO: implement

        void IProvisioningSessionInspector.OnSessionStarting(
            IHarshProvisionerContext context
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            lock (_syncRoot)
            {
                _session = context.Session;
            }
        }

        void IProvisioningSessionInspector.OnSessionEnded(
            IHarshProvisionerContext context
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }
        }

        void IProvisioningSessionInspector.OnProvisioningStarting(
            IHarshProvisionerContext context,
            HarshProvisionerBase provisioner
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            if (provisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisioner));
            }

            if (!IsContainerOnly(provisioner))
            {
                lock (_syncRoot)
                {
                    _currentProvisioner = provisioner;
                    _currentProvisionerIsSkipped = false;
                }
            }
        }

        void IProvisioningSessionInspector.OnProvisioningEnded(
            IHarshProvisionerContext context,
            HarshProvisionerBase provisioner
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            if (provisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisioner));
            }

            if (!IsContainerOnly(provisioner))
            {
                lock (_syncRoot)
                {
                    _completedProvisionersCount++;
                }
            }
        }

        void IProvisioningSessionInspector.OnProvisioningSkipped(
            IHarshProvisionerContext context,
            HarshProvisionerBase provisioner
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            if (provisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(provisioner));
            }

            if (!IsContainerOnly(provisioner))
            {
                lock (_syncRoot)
                {
                    _currentProvisioner = provisioner;
                    _currentProvisionerIsSkipped = true;
                    _completedProvisionersCount++;
                }
            }
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(SessionProgressInspector));
    }
}