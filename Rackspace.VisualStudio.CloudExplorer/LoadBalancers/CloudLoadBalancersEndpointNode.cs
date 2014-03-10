namespace Rackspace.VisualStudio.CloudExplorer.LoadBalancers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;
    using LoadBalancer = net.openstack.Providers.Rackspace.Objects.LoadBalancers.LoadBalancer;

    public class CloudLoadBalancersEndpointNode : EndpointNode
    {
        public CloudLoadBalancersEndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            : base(identity, serviceCatalog, endpoint)
        {
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
            return new CloudLoadBalancerProvider(Identity, Endpoint.Region, null);
        }
    }
}
