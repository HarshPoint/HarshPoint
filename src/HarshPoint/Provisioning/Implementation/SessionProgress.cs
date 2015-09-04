using System;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class SessionProgress
    {
        public SessionProgress(
            HarshProvisionerAction action,
            HarshProvisionerBase currentProvisioner,
            Boolean currentProvisionerIsSkipped,
            Int32 completedProvisionersCount,
            Int32 provisionersCount,
            Int32 percentComplete,
            TimeSpan? remainingTime
        )
        {
            Action = action;
            CurrentProvisioner = currentProvisioner;
            CurrentProvisionerIsSkipped = currentProvisionerIsSkipped;
            CompletedProvisionersCount = completedProvisionersCount;
            ProvisionersCount = provisionersCount;
            PercentComplete = percentComplete;
            RemainingTime = remainingTime;
        }

        public HarshProvisionerAction Action { get; }
        public HarshProvisionerBase CurrentProvisioner { get; }
        public Boolean CurrentProvisionerIsSkipped { get; }
        public Int32 CompletedProvisionersCount { get; }
        public Int32 ProvisionersCount { get; }
        public Int32 PercentComplete { get; }
        public TimeSpan? RemainingTime { get; }
    }
}
