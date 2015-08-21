using HarshPoint.Provisioning.Implementation;
using HarshPoint.Provisioning.Output;
using System;

namespace HarshPoint.Provisioning
{
    public abstract class HarshProvisionerOutputSink
    {
        public void WriteOutput(IHarshProvisionerContext context, HarshProvisionerOutput output)
        {
            if (context == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(context));
            }

            if (output == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(output));
            }

            output.Context = context.ToString();
            output.Timestamp = DateTimeOffset.Now;

            WriteOutputCore(output);
        }

        protected internal abstract void WriteOutputCore(HarshProvisionerOutput output);

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshProvisionerOutputSink>();

        public static HarshProvisionerOutputSink Null { get; } = new HarshProvisionerOutputSinkNull();
    }
}
