using System;
using System.IO;
using System.Threading.Tasks;

namespace HarshPoint.Provisioning
{
    public sealed class HarshDeployUserSolution : HarshProvisioner
    {
        private readonly HarshCreateFile _createFile;
        private readonly HarshDesignPackage _designPackage;

        public HarshDeployUserSolution()
        {
            Children.Add(_createFile = new HarshCreateFile());
            Children.Add(_designPackage = new HarshDesignPackage());
        }

        public String SolutionName
        {
            get { return _createFile.FileName; }
            set { _createFile.FileName = value; }
        }

        public Stream ContentStream
        {
            get { return _createFile.ContentStream; }
            set { _createFile.ContentStream = value; }
        }

        protected override Task InitializeAsync()
        {
            return base.InitializeAsync();
        }

        protected override async Task OnProvisioningAsync()
        {
            await base.OnProvisioningAsync();

        }
    }
}
