namespace Rackspace.VisualStudio.CloudExplorer.Databases
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;

    public class CloudDatabasesEndpointNode : EndpointNode
    {
        public CloudDatabasesEndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            : base(identity, serviceCatalog, endpoint)
        {
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new Node[] { new NotImplementedPlaceholderNode() });
        }
    }
}
