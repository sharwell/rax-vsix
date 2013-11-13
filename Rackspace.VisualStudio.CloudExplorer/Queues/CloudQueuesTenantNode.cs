namespace Rackspace.VisualStudio.CloudExplorer.Queues
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Domain.Queues;
    using net.openstack.Core.Providers;
    using net.openstack.Providers.Rackspace;

    public class CloudQueuesTenantNode : AsyncNode
    {
        private readonly CloudIdentity _identity;
        private string _tenantId;

        public CloudQueuesTenantNode(CloudIdentity identity)
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
            Task<Tuple<CloudQueuesProvider, CloudQueue[]>>[] tasks = Array.ConvertAll(regions, region => ListQueuesInRegionAsync(region, CancellationToken.None));

            await Task.WhenAll(tasks);

            List<Tuple<CloudQueuesProvider, CloudQueue>> allQueues = new List<Tuple<CloudQueuesProvider, CloudQueue>>();
            foreach (Task<Tuple<CloudQueuesProvider, CloudQueue[]>> queueTask in tasks)
                allQueues.AddRange(queueTask.Result.Item2.Select(i => Tuple.Create(queueTask.Result.Item1, i)));

            return Array.ConvertAll(allQueues.ToArray(), tuple => CreateQueueNode(tuple.Item1, tuple.Item2));
        }

        private CloudQueueNode CreateQueueNode(CloudQueuesProvider provider, CloudQueue queue)
        {
            return new CloudQueueNode(provider, queue);
        }

        private async Task<Tuple<CloudQueuesProvider, CloudQueue[]>> ListQueuesInRegionAsync(string region, CancellationToken cancellationToken)
        {
            CloudQueuesProvider provider = new CloudQueuesProvider(_identity, region, Guid.NewGuid(), false, null);
            List<CloudQueue> queues = new List<CloudQueue>();
            queues.AddRange(await provider.ListQueuesAsync(null, null, true, cancellationToken));
            return Tuple.Create(provider, queues.ToArray());
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
