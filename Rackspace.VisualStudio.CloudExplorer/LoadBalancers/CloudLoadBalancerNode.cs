namespace Rackspace.VisualStudio.CloudExplorer.LoadBalancers
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.LoadBalancers;
    using Node = Microsoft.VSDesigner.ServerExplorer.Node;

    public class CloudLoadBalancerNode : AsyncNode
    {
        private readonly CloudLoadBalancerProvider _provider;
        private readonly LoadBalancer _loadBalancer;

        public CloudLoadBalancerNode(CloudLoadBalancerProvider provider, LoadBalancer loadBalancer)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (loadBalancer == null)
                throw new ArgumentNullException("loadBalancer");

            _provider = provider;
            _loadBalancer = loadBalancer;
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
                return _loadBalancer.Name;
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
