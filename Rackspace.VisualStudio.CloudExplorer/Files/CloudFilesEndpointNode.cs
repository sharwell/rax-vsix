namespace Rackspace.VisualStudio.CloudExplorer.Files
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;

    public class CloudFilesEndpointNode : EndpointNode
    {
        public CloudFilesEndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            : base(identity, serviceCatalog, endpoint)
        {
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            Tuple<CloudFilesProvider, Container[]> containers = await ListContainersAsync(cancellationToken);
            return Array.ConvertAll(containers.Item2, i => CreateContainerNode(containers.Item1, i, null));
        }

        private CloudFilesContainerNode CreateContainerNode(CloudFilesProvider provider, Container container, ContainerCDN containerCdn)
        {
            return new CloudFilesContainerNode(provider, container, containerCdn);
        }

        private async Task<Tuple<CloudFilesProvider, Container[]>> ListContainersAsync(CancellationToken cancellationToken)
        {
            CloudFilesProvider provider = CreateProvider();
            List<Container> containers = new List<Container>();
            containers.AddRange(await Task.Run(() => provider.ListContainers()));
            return Tuple.Create(provider, containers.ToArray());
        }

        private CloudFilesProvider CreateProvider()
        {
            CloudFilesProvider provider = new CloudFilesProvider(Identity, Endpoint.Region, null, null);
            provider.ConnectionLimit = 50;
            return provider;
        }
    }
}
