namespace Rackspace.VisualStudio.CloudExplorer.Queues
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Domain.Queues;
    using net.openstack.Providers.Rackspace;

    public class CloudQueuesEndpointNode : AsyncNode
    {
        private readonly CloudIdentity _identity;
        private readonly Endpoint _endpoint;

        public CloudQueuesEndpointNode(CloudIdentity identity, Endpoint endpoint)
        {
            _identity = identity;
            _endpoint = endpoint;
        }

        public override int CompareUnique(Node node)
        {
            return Label.CompareTo(node.Label);
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
            return new CloudQueuesProvider(_identity, _endpoint.Region, Guid.NewGuid(), false, null);
        }

        public override Image Icon
        {
            get
            {
                return ServerExplorerIcons.PrivateCloud;
            }
        }

        protected override string DisplayText
        {
            get
            {
                if (!string.IsNullOrEmpty(_endpoint.Region))
                    return _endpoint.Region;

                return "Global";
            }
        }

        public override bool CanDeleteNode()
        {
            return false;
        }
    }
}
