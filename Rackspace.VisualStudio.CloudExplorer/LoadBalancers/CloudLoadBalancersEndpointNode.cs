namespace Rackspace.VisualStudio.CloudExplorer.LoadBalancers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;
    using LoadBalancer = net.openstack.Providers.Rackspace.Objects.LoadBalancers.LoadBalancer;

    public class CloudLoadBalancersEndpointNode : AsyncNode
    {
        private readonly CloudIdentity _identity;
        private readonly Endpoint _endpoint;

        public CloudLoadBalancersEndpointNode(CloudIdentity identity, Endpoint endpoint)
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
            Tuple<CloudLoadBalancerProvider, LoadBalancer[]> loadBalancers = await ListLoadBalancersAsync(cancellationToken);
            return Array.ConvertAll(loadBalancers.Item2, i => CreateLoadBalancerNode(loadBalancers.Item1, i));
        }

        private CloudLoadBalancerNode CreateLoadBalancerNode(CloudLoadBalancerProvider provider, LoadBalancer loadBalancer)
        {
            return new CloudLoadBalancerNode(provider, loadBalancer);
        }

        private async Task<Tuple<CloudLoadBalancerProvider, LoadBalancer[]>> ListLoadBalancersAsync(CancellationToken cancellationToken)
        {
            CloudLoadBalancerProvider provider = CreateProvider();
            List<LoadBalancer> loadBalancers = new List<LoadBalancer>();
            loadBalancers.AddRange(await provider.ListLoadBalancersAsync(null, null, cancellationToken));
            return Tuple.Create(provider, loadBalancers.ToArray());
        }

        private CloudLoadBalancerProvider CreateProvider()
        {
            return new CloudLoadBalancerProvider(_identity, _endpoint.Region, null);
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
