#define DEBUG

using System.Diagnostics;
using System.Threading.Tasks;

namespace HarshPoint
{
    internal sealed class HarshDebugTraceListener : HarshTraceListener
    {
        public override Task Write(HarshTraceEvent e)
        {
            if (e == null)
            {
                throw Error.ArgumentNull(nameof(e));
            }

            Debug.WriteLine(e.ToString());
            return HarshTask.Completed;
        }
    }
}
