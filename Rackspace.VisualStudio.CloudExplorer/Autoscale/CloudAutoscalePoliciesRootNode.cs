namespace Rackspace.VisualStudio.CloudExplorer.Autoscale
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.AutoScale;
    using Image = System.Drawing.Image;
    using LocalizableProperties = Microsoft.VisualStudio.Shell.LocalizableProperties;

    public class CloudAutoscalePoliciesRootNode : AsyncNode
    {
        private readonly CloudAutoScaleProvider _provider;
        private readonly ScalingGroup _scalingGroup;

        public CloudAutoscalePoliciesRootNode(CloudAutoScaleProvider provider, ScalingGroup scalingGroup)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (scalingGroup == null)
                throw new ArgumentNullException("scalingGroup");

            _provider = provider;
            _scalingGroup = scalingGroup;
        }

        protected override string DisplayText
        {
            get
            {
                return "Policies";
            }
        }

        public override Image Icon
        {
            get
            {
                return RackspaceProductsNode.EmptyIcon;
            }
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            Policy[] policies = await ListPoliciesAsync(cancellationToken).ConfigureAwait(false);
            List<Node> results = new List<Node>();
            foreach (Policy policy in policies)
                results.Add(await CreatePolicyNodeAsync(policy, cancellationToken).ConfigureAwait(false));

            return results.ToArray();
        }

        private Task<Node> CreatePolicyNodeAsync(Policy policy, CancellationToken cancellationToken)
        {
            return Task.FromResult<Node>(new CloudAutoscalePolicyNode(_provider, _scalingGroup, policy));
        }

        private async Task<Policy[]> ListPoliciesAsync(CancellationToken cancellationToken)
        {
            List<Policy> policies = new List<Policy>();
            policies.AddRange(await _provider.ListPoliciesAsync(_scalingGroup.Id, null, null, cancellationToken).ConfigureAwait(false));
            return policies.ToArray();
        }

        public override bool CanDeleteNode()
        {
            return false;
        }

        public override object GetBrowseComponent()
        {
            return new ScalingPoliciesProperties();
        }

        protected class ScalingPoliciesProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            public override string GetClassName()
            {
                return "Scaling Policies";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Auto Scale";
            }
        }
    }
}
