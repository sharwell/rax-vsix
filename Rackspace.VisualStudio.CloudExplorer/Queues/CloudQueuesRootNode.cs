namespace Rackspace.VisualStudio.CloudExplorer.Queues
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;

    public class CloudQueuesRootNode : CloudProductRootNode
    {
        private CloudIdentity _identity;
        private ServiceCatalog _serviceCatalog;

        private Node[] _children;

        public CloudQueuesRootNode(CloudIdentity identity, ServiceCatalog serviceCatalog)
        {
            _identity = identity;
            _serviceCatalog = serviceCatalog;
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            if (_children == null)
            {
                List<Node> nodes = new List<Node>();
                foreach (Endpoint endpoint in _serviceCatalog.Endpoints)
                    nodes.Add(new CloudQueuesEndpointNode(_identity, endpoint));

                _children = nodes.ToArray();
            }

            return Task.FromResult(_children);
        }

        public override Image Icon
        {
            get
            {
                return ServerExplorerIcons.CloudDatabases;
            }
        }

        protected override string DisplayText
        {
            get
            {
                return "Queues";
            }
        }
    }
}
