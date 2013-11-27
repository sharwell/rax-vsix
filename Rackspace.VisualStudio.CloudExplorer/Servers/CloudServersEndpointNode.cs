namespace Rackspace.VisualStudio.CloudExplorer.Servers
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;

    public class CloudServersEndpointNode : EndpointNode
    {
        public CloudServersEndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            : base(identity, serviceCatalog, endpoint)
        {
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new Node[] { new NotImplementedPlaceholderNode() });
        }
    }
}
