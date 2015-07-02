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

        public Guid SolutionId
        {
            get;
            set;
        }

        public String SolutionName
        {
            get;
            set;
        }

        public Stream ContentStream
        {
            get;
            set;
        }

        protected override Task InitializeAsync()
        {
            _createFile.ContentStream = ContentStream;
            _createFile.FileName = SolutionName;
            _createFile.Folder = HarshDesignPackage.SolutionCatalogFolder;
            _createFile.OverwriteExisting = true;

            _designPackage.DesignPackageId = SolutionId;
            _designPackage.DesignPackageName = SolutionName;

            return base.InitializeAsync();
        }
    }
}
