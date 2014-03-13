namespace Rackspace.VisualStudio.CloudExplorer.Servers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Exceptions.Response;
    using net.openstack.Providers.Rackspace;

    public class CloudServersEndpointNode : EndpointNode
    {
        public CloudServersEndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            : base(identity, serviceCatalog, endpoint)
        {
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            Task<Tuple<CloudServersProvider, Server[]>> serversTask = ListServersAsync(cancellationToken);
            Task<Tuple<CloudServersProvider, FlavorDetails[]>> flavorsTask = ListFlavorsAsync(cancellationToken);
            Task<Tuple<CloudServersProvider, ServerImage[]>> imagesTask = ListImagesAsync(cancellationToken);
            await Task.WhenAll(serversTask, flavorsTask, imagesTask);

            var servers = serversTask.Result;
            var flavors = flavorsTask.Result;
            var images = imagesTask.Result;
            List<Node> results = new List<Node>();
            foreach (Server server in servers.Item2)
                results.Add(await CreateServerNodeAsync(servers.Item1, server, flavors.Item2, images.Item2));

            return results.ToArray();
        }

        private async Task<CloudServersServerNode> CreateServerNodeAsync(CloudServersProvider provider, Server server, FlavorDetails[] flavors, ServerImage[] images)
        {
            FlavorDetails flavor = flavors.FirstOrDefault(i => server.Flavor != null && string.Equals(server.Flavor.Id, i.Id, StringComparison.OrdinalIgnoreCase));
            try
            {
                if (flavor == null && server.Flavor != null)
                    flavor = await Task.Run(() => provider.GetFlavor(server.Flavor.Id));
            }
            catch (ResponseException)
            {
                // ignore
                flavor = null;
            }

            ServerImage image = images.FirstOrDefault(i => server.Image != null && string.Equals(server.Image.Id, i.Id, StringComparison.OrdinalIgnoreCase));
            try
            {
                if (image == null && server.Image != null)
                    image = await Task.Run(() => provider.GetImage(server.Image.Id));
            }
            catch (ResponseException)
            {
                // ignore
                image = null;
            }

            return new CloudServersServerNode(provider, server, flavor, image);
        }

        private async Task<Tuple<CloudServersProvider, Server[]>> ListServersAsync(CancellationToken cancellationToken)
        {
            CloudServersProvider provider = CreateProvider();
            List<Server> servers = new List<Server>();
            servers.AddRange(await Task.Run(() => provider.ListServersWithDetails()));
            return Tuple.Create(provider, servers.ToArray());
        }

        private async Task<Tuple<CloudServersProvider, FlavorDetails[]>> ListFlavorsAsync(CancellationToken cancellationToken)
        {
            CloudServersProvider provider = CreateProvider();
            List<FlavorDetails> flavors = new List<FlavorDetails>();
            flavors.AddRange(await Task.Run(() => provider.ListFlavorsWithDetails()));
            return Tuple.Create(provider, flavors.ToArray());
        }

        private async Task<Tuple<CloudServersProvider, ServerImage[]>> ListImagesAsync(CancellationToken cancellationToken)
        {
            CloudServersProvider provider = CreateProvider();
            List<ServerImage> images = new List<ServerImage>();
            images.AddRange(await Task.Run(() => provider.ListImagesWithDetails()));
            return Tuple.Create(provider, images.ToArray());
        }

        private CloudServersProvider CreateProvider()
        {
            return new CloudServersProvider(Identity, Endpoint.Region, null, null);
        }
    }
}
