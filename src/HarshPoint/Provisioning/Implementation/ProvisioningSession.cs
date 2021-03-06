﻿using System;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace HarshPoint.Provisioning.Implementation
{
    public sealed class ProvisioningSession : IProvisioningSession
    {
        private readonly HarshProvisionerBase _rootProvisioner;

        private IImmutableList<HarshProvisionerBase> _provisioners;

        public IImmutableList<HarshProvisionerBase> Provisioners
            => HarshLazy.Initialize(ref _provisioners, CreateProvisionersCollection);

        public HarshProvisionerAction Action { get; }

        private IImmutableList<HarshProvisionerBase> CreateProvisionersCollection()
            => GetFlattenedTree(_rootProvisioner);

        internal ProvisioningSession(
            HarshProvisionerBase rootProvisioner,
            HarshProvisionerAction action)
        {
            if (rootProvisioner == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(rootProvisioner));
            }

            _rootProvisioner = rootProvisioner;
            Action = action;
        }

        internal IImmutableList<HarshProvisionerBase> GetFlattenedTree(
            HarshProvisionerBase provisioner
        )
        {
            var children = from child in provisioner.GetChildrenSorted(Action)
                           from childRec in GetFlattenedTree(child)
                           select childRec;

            if (Action == HarshProvisionerAction.Provision)
            {
                return ImmutableList
                    .Create(provisioner)
                    .AddRange(children);
            }
            else
            {
                return ImmutableList
                    .CreateRange(children)
                    .Add(provisioner);
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void OnSessionStarting(IHarshProvisionerContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            NotifyInspectors(context, si => si.OnSessionStarting(context));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void OnSessionEnded(IHarshProvisionerContext context)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            NotifyInspectors(context, si => si.OnSessionEnded(context));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void OnProvisioningStarting(
            IHarshProvisionerContext context,
            HarshProvisionerBase provisioner
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            NotifyInspectors(context, si => si.OnProvisioningStarting(context, provisioner));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void OnProvisioningEnded(
            IHarshProvisionerContext context,
            HarshProvisionerBase provisioner
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            NotifyInspectors(context, si => si.OnProvisioningEnded(context, provisioner));
        }

        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void OnProvisioningSkipped(
            IHarshProvisionerContext context,
            HarshProvisionerBase provisioner
        )
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            NotifyInspectors(context, si => si.OnProvisioningSkipped(context, provisioner));
        }

        private static void NotifyInspectors(IHarshProvisionerContext context, Action<IProvisioningSessionInspector> action)
        {
            foreach (var sessionInspector in context.SessionInspectors)
            {
                action(sessionInspector);
            }
        }

        private static readonly HarshLogger Logger
            = HarshLog.ForContext(typeof(ProvisioningSession));
    }

}
