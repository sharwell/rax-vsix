namespace Rackspace.VisualStudio.CloudExplorer.Files
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using OpenStack.Security.Authentication;
    using OpenStack.Services.Identity.V2;

    public class CloudFilesRootNode : CloudProductRootNodeV2
    {
        private Node[] _children;

        public CloudFilesRootNode(IAuthenticationService authenticationService, ServiceCatalogEntry serviceCatalogEntry)
            : base(authenticationService, serviceCatalogEntry)
        {
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            if (_children == null)
            {
                List<Node> nodes = new List<Node>();
                foreach (Endpoint endpoint in ServiceCatalogEntry.Endpoints)
                    nodes.Add(new CloudFilesEndpointNode(AuthenticationService, ServiceCatalogEntry, endpoint));

                _children = nodes.ToArray();
            }

            return Task.FromResult(_children);
        }

        public override Image Icon
        {
            get
            {
                return ServerExplorerIcons.CloudFiles;
            }
        }

        protected override string DisplayText
        {
            get
            {
                return "Files";
            }
        }
    }
}
