namespace Rackspace.VisualStudio.CloudExplorer.Autoscale
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.AutoScale;

    public class CloudAutoscaleEndpointNode : EndpointNode
    {
        public CloudAutoscaleEndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            : base(identity, serviceCatalog, endpoint)
        {
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            Tuple<CloudAutoScaleProvider, ScalingGroup[]> scalingGroups = await ListScalingGroupsAsync(cancellationToken).ConfigureAwait(false);
            List<Node> results = new List<Node>();
            foreach (ScalingGroup scalingGroup in scalingGroups.Item2)
                results.Add(await CreateScalingGroupNodeAsync(scalingGroups.Item1, scalingGroup, cancellationToken).ConfigureAwait(false));

            return results.ToArray();
        }

        private async Task<Node> CreateScalingGroupNodeAsync(CloudAutoScaleProvider provider, ScalingGroup scalingGroup, CancellationToken cancellationToken)
        {
            if (scalingGroup.GroupConfiguration == null || scalingGroup.LaunchConfiguration == null)
                scalingGroup = await provider.GetGroupAsync(scalingGroup.Id, cancellationToken);

            return new CloudAutoscaleScalingGroupNode(provider, scalingGroup);
        }

        private async Task<Tuple<CloudAutoScaleProvider, ScalingGroup[]>> ListScalingGroupsAsync(CancellationToken cancellationToken)
        {
            CloudAutoScaleProvider provider = CreateProvider();
            List<ScalingGroup> scalingGroups = new List<ScalingGroup>();
            scalingGroups.AddRange(await provider.ListScalingGroupsAsync(null, null, cancellationToken).ConfigureAwait(false));
            return Tuple.Create(provider, scalingGroups.ToArray());
        }

        private CloudAutoScaleProvider CreateProvider()
        {
            return new CloudAutoScaleProvider(Identity, Endpoint.Region, null);
        }
    }
}
