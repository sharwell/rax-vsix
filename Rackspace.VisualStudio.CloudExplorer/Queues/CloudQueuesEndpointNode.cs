namespace Rackspace.VisualStudio.CloudExplorer.Queues
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Domain.Queues;
    using net.openstack.Providers.Rackspace;

    public class CloudQueuesEndpointNode : EndpointNode
    {
        public CloudQueuesEndpointNode(CloudIdentity identity, Endpoint endpoint)
            : base(identity, endpoint)
        {
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            Tuple<CloudQueuesProvider, CloudQueue[]> queues = await ListQueuesAsync(cancellationToken);
            return Array.ConvertAll(queues.Item2, i => CreateQueueNode(queues.Item1, i));
        }

        private CloudQueueNode CreateQueueNode(CloudQueuesProvider provider, CloudQueue queue)
        {
            return new CloudQueueNode(provider, queue);
        }

        private async Task<Tuple<CloudQueuesProvider, CloudQueue[]>> ListQueuesAsync(CancellationToken cancellationToken)
        {
            CloudQueuesProvider provider = CreateProvider();
            List<CloudQueue> queues = new List<CloudQueue>();
            queues.AddRange(await provider.ListQueuesAsync(null, null, true, cancellationToken));
            return Tuple.Create(provider, queues.ToArray());
        }

        private CloudQueuesProvider CreateProvider()
        {
            return new CloudQueuesProvider(Identity, Endpoint.Region, Guid.NewGuid(), false, null);
        }
    }
}
