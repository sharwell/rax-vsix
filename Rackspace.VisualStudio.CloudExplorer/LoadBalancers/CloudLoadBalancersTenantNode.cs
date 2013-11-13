namespace Rackspace.VisualStudio.CloudExplorer.LoadBalancers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Providers;
    using net.openstack.Providers.Rackspace;
    using LoadBalancer = net.openstack.Providers.Rackspace.Objects.LoadBalancers.LoadBalancer;

    public class CloudLoadBalancersTenantNode : AsyncNode
    {
        private readonly CloudIdentity _identity;
        private string _tenantId;

        public CloudLoadBalancersTenantNode(CloudIdentity identity)
        {
            _identity = identity;

            IIdentityService identityService = new CloudIdentityProvider();
            identityService.GetTokenAsync(identity, CancellationToken.None).ContinueWith(
                task =>
                {
                    _tenantId = task.Result.Tenant.Id;
                    INodeSite nodeSite = GetNodeSite();
                    if (nodeSite != null)
                        nodeSite.UpdateLabel();
                });
        }

        public override int CompareUnique(Node node)
        {
            return Label.CompareTo(node.Label);
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            string[] regions = { "DFW", "ORD", "IAD", "HKG", "SYD" };
            Task<Tuple<CloudLoadBalancerProvider, LoadBalancer[]>>[] tasks = Array.ConvertAll(regions, region => ListLoadBalancersInRegionAsync(region, cancellationToken));
            await Task.WhenAll(tasks);

            List<Tuple<CloudLoadBalancerProvider, LoadBalancer>> allLoadBalancers = new List<Tuple<CloudLoadBalancerProvider, LoadBalancer>>();
            foreach (Task<Tuple<CloudLoadBalancerProvider, LoadBalancer[]>> completedTask in tasks)
                allLoadBalancers.AddRange(completedTask.Result.Item2.Select(i => Tuple.Create(completedTask.Result.Item1, i)));

            return Array.ConvertAll(allLoadBalancers.ToArray(), tuple => CreateLoadBalancerNode(tuple.Item1, tuple.Item2));
        }

        private CloudLoadBalancerNode CreateLoadBalancerNode(CloudLoadBalancerProvider provider, LoadBalancer loadBalancer)
        {
            return new CloudLoadBalancerNode(provider, loadBalancer);
        }

        private async Task<Tuple<CloudLoadBalancerProvider, LoadBalancer[]>> ListLoadBalancersInRegionAsync(string region, CancellationToken cancellationToken)
        {
            CloudLoadBalancerProvider provider = new CloudLoadBalancerProvider(_identity, region, null);
            List<LoadBalancer> loadBalancers = new List<LoadBalancer>();
            loadBalancers.AddRange(await provider.ListLoadBalancersAsync(null, null, cancellationToken));
            return Tuple.Create(provider, loadBalancers.ToArray());
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
                if (!string.IsNullOrEmpty(_identity.Username) && !string.IsNullOrEmpty(_tenantId))
                    return string.Format("{0} ({1})", _identity.Username, _tenantId);
                else if (!string.IsNullOrEmpty(_identity.Username))
                    return _identity.Username;

                return _tenantId ?? string.Empty;
            }
        }

        public override bool CanDeleteNode()
        {
            return false;
        }
    }
}
