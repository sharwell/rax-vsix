namespace Rackspace.VisualStudio.CloudExplorer.Files
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;

    public class CloudFilesRootNode : CloudProductRootNode
    {
        private readonly CloudIdentity _identity;

        private Node[] _children;

        public CloudFilesRootNode(ServiceCatalog serviceCatalog, CloudIdentity identity)
            : base(serviceCatalog)
        {
            _identity = identity;
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            if (_children == null)
            {
                List<Node> nodes = new List<Node>();
                foreach (Endpoint endpoint in ServiceCatalog.Endpoints)
                    nodes.Add(new CloudFilesEndpointNode(_identity, ServiceCatalog, endpoint));

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
