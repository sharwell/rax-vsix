namespace Rackspace.VisualStudio.CloudExplorer.Autoscale
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;

    public class CloudAutoscaleEndpointNode : EndpointNode
    {
        public CloudAutoscaleEndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            : base(identity, serviceCatalog, endpoint)
        {
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new Node[] { new NotImplementedPlaceholderNode() });
        }
    }
}
