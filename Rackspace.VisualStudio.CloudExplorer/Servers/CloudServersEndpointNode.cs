namespace Rackspace.VisualStudio.CloudExplorer.Servers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;

    public class CloudServersEndpointNode : EndpointNode
    {
        public CloudServersEndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            : base(identity, serviceCatalog, endpoint)
        {
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            Tuple<CloudServersProvider, Server[]> servers = await ListServersAsync(cancellationToken);
            return Array.ConvertAll(servers.Item2, i => CreateServerNode(servers.Item1, i));
        }

        private CloudServersServerNode CreateServerNode(CloudServersProvider provider, Server server)
        {
            return new CloudServersServerNode(provider, server);
        }

        private async Task<Tuple<CloudServersProvider, Server[]>> ListServersAsync(CancellationToken cancellationToken)
        {
            CloudServersProvider provider = CreateProvider();
            List<Server> servers = new List<Server>();
            servers.AddRange(await Task.Run(() => provider.ListServersWithDetails()));
            return Tuple.Create(provider, servers.ToArray());
        }

        private CloudServersProvider CreateProvider()
        {
            return new CloudServersProvider(Identity, Endpoint.Region, null, null);
        }
    }
}
