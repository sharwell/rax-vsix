namespace Rackspace.VisualStudio.CloudExplorer.Dns
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;

    public class CloudDnsRootNode : CloudProductRootNode
    {
        private CloudIdentity _identity;
        private ServiceCatalog _serviceCatalog;

        public CloudDnsRootNode(CloudIdentity identity, ServiceCatalog serviceCatalog)
        {
            _identity = identity;
            _serviceCatalog = serviceCatalog;
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            List<Node> nodes = new List<Node>();
            foreach (Endpoint endpoint in _serviceCatalog.Endpoints)
                nodes.Add(new CloudDnsEndpointNode(_identity, endpoint));

            return Task.FromResult(nodes.ToArray());
        }

        public override Image Icon
        {
            get
            {
                return ServerExplorerIcons.CloudDns;
            }
        }

        protected override string DisplayText
        {
            get
            {
                return "DNS";
            }
        }
    }
}
