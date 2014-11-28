using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarshPoint.Server.Tests
{
    public class SharePointClientFixture
    {
        public SharePointClientFixture()
        {
            ClientContext = new ClientContext("http://" + Environment.MachineName);
        }

        public ClientContext ClientContext
        {
            get;
            set;
        }
    }
}
