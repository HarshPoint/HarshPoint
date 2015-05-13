using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint
{
    public static class HarshTask
    {
        public static Task Completed
        {
            get { return Task.FromResult(false); }
        }
    }
}
