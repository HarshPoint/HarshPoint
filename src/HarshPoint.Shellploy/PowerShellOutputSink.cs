using HarshPoint.Provisioning;

namespace HarshPoint.Shellploy
{
    internal sealed class PowerShellOutputSink : HarshProvisionerOutputSink
    {
        internal PowerShellOutputSink(InvokeProvisionerCommand owner)
        {
            Owner = owner;
        }

        public InvokeProvisionerCommand Owner { get; }

        protected override void WriteOutputCore(HarshProvisionerOutput output)
        {
            Owner.WriteObject(output);
        }
    }
}
