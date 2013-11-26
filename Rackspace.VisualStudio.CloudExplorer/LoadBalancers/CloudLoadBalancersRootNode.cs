namespace Rackspace.VisualStudio.CloudExplorer.LoadBalancers
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;

    public class CloudLoadBalancersRootNode : CloudProductRootNode
    {
        private CloudIdentity _identity;
        private ServiceCatalog _serviceCatalog;

        public CloudLoadBalancersRootNode(CloudIdentity identity, ServiceCatalog serviceCatalog)
        {
            _identity = identity;
            _serviceCatalog = serviceCatalog;
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            List<Node> nodes = new List<Node>();
            foreach (Endpoint endpoint in _serviceCatalog.Endpoints)
                nodes.Add(new CloudLoadBalancersEndpointNode(_identity, endpoint));

            return Task.FromResult(nodes.ToArray());
        }

        public override Image Icon
        {
            get
            {
                return ServerExplorerIcons.CloudLoadBalancers;
            }
        }

        protected override string DisplayText
        {
            get
            {
                return "Load Balancers";
            }
        }
    }
}
