namespace Rackspace.VisualStudio.CloudExplorer.Queues
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain.Queues;
    using net.openstack.Providers.Rackspace;
    using CancellationToken = System.Threading.CancellationToken;

    public class CloudQueueNode : AsyncNode
    {
        private readonly CloudQueuesProvider _provider;
        private readonly CloudQueue _queue;

        public CloudQueueNode(CloudQueuesProvider provider, CloudQueue queue)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (queue == null)
                throw new ArgumentNullException("queue");

            _provider = provider;
            _queue = queue;
        }

        public override int CompareUnique(Node node)
        {
            return Label.CompareTo(node.Label);
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(RackspaceProductsNode.EmptyChildren);
        }

        public override Image Icon
        {
            get
            {
                return RackspaceProductsNode.EmptyIcon;
            }
        }

        protected override string DisplayText
        {
            get
            {
                return _queue.Name.Value;
            }
        }

        public override bool CanDeleteNode()
        {
            return true;
        }

        public override bool ConfirmDeletingNode()
        {
            throw new NotImplementedException();
        }
    }
}
