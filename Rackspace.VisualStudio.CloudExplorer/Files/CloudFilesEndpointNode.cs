namespace Rackspace.VisualStudio.CloudExplorer.Files
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using OpenStack.Collections;
    using OpenStack.Security.Authentication;
    using OpenStack.Services.Identity.V2;
    using OpenStack.Services.ObjectStorage.V1;

    public class CloudFilesEndpointNode : EndpointNodeV2
    {
        public CloudFilesEndpointNode(IAuthenticationService authenticationService, ServiceCatalogEntry serviceCatalogEntry, Endpoint endpoint)
            : base(authenticationService, serviceCatalogEntry, endpoint)
        {
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            Tuple<IObjectStorageService, Container[]> containers = await ListContainersAsync(cancellationToken);
            return Array.ConvertAll(containers.Item2, i => CreateContainerNode(containers.Item1, i));
        }

        private CloudFilesContainerNode CreateContainerNode(IObjectStorageService provider, Container container)
        {
            return new CloudFilesContainerNode(provider, container);
        }

        private async Task<Tuple<IObjectStorageService, Container[]>> ListContainersAsync(CancellationToken cancellationToken)
        {
            IObjectStorageService provider = CreateServiceClient();
            List<Container> containers = new List<Container>();
            Tuple<AccountMetadata, ReadOnlyCollectionPage<Container>> containersResult = await provider.ListContainersAsync(cancellationToken).ConfigureAwait(false);
            containers.AddRange(containersResult.Item2);
            return Tuple.Create(provider, containers.ToArray());
        }

        private IObjectStorageService CreateServiceClient()
        {
            ObjectStorageClient client = new ObjectStorageClient(AuthenticationService, Endpoint.Region, false);
            client.ConnectionLimit = 50;
            return client;
        }
    }
}
