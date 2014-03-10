namespace Rackspace.VisualStudio.CloudExplorer.Backup
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;

    public class CloudBackupEndpointNode : EndpointNode
    {
        public CloudBackupEndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            : base(identity, serviceCatalog, endpoint)
        {
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new Node[] { new NotImplementedPlaceholderNode() });
        }
    }
}
