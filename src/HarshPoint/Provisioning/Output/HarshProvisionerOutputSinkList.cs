using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace HarshPoint.Provisioning.Output
{
    public sealed class HarshProvisionerOutputSinkList : HarshProvisionerOutputSink
    {
        private readonly List<HarshProvisionerOutput> _output
            = new List<HarshProvisionerOutput>();

        private readonly IReadOnlyCollection<HarshProvisionerOutput> _outputRo;

        public HarshProvisionerOutputSinkList()
        {
            _outputRo = new ReadOnlyCollection<HarshProvisionerOutput>(_output);
        }

        public IReadOnlyCollection<HarshProvisionerOutput> Output => _outputRo;

        protected internal override void WriteOutputCore(HarshProvisionerOutput output)
        {
            if (output == null)
            {
                throw Logger.Fatal.ArgumentNull(nameof(output));
            }

            _output.Add(output);
        }

        private static readonly HarshLogger Logger = HarshLog.ForContext<HarshProvisionerOutputSinkList>();
    }
}
