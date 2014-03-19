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

    public class CloudAutoscaleWebhooksRootNode : AsyncNode
    {
        private readonly CloudAutoScaleProvider _provider;
        private readonly ScalingGroup _scalingGroup;
        private readonly Policy _policy;

        public CloudAutoscaleWebhooksRootNode(CloudAutoScaleProvider provider, ScalingGroup scalingGroup, Policy policy)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (scalingGroup == null)
                throw new ArgumentNullException("scalingGroup");
            if (policy == null)
                throw new ArgumentNullException("policy");

            _provider = provider;
            _scalingGroup = scalingGroup;
            _policy = policy;
        }

        protected override string DisplayText
        {
            get
            {
                return "Webhooks";
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
            Webhook[] webhooks = await ListWebhooksAsync(cancellationToken).ConfigureAwait(false);
            List<Node> results = new List<Node>();
            foreach (Webhook webhook in webhooks)
                results.Add(await CreateWebhookNodeAsync(webhook, cancellationToken).ConfigureAwait(false));

            return results.ToArray();
        }

        private Task<Node> CreateWebhookNodeAsync(Webhook webhook, CancellationToken cancellationToken)
        {
            return Task.FromResult<Node>(new CloudAutoscaleWebhookNode(_provider, _scalingGroup, _policy, webhook));
        }

        private async Task<Webhook[]> ListWebhooksAsync(CancellationToken cancellationToken)
        {
            List<Webhook> webhooks = new List<Webhook>();
            webhooks.AddRange(await _provider.ListWebhooksAsync(_scalingGroup.Id, _policy.Id, null, null, cancellationToken).ConfigureAwait(false));
            return webhooks.ToArray();
        }

        public override bool CanDeleteNode()
        {
            return false;
        }

        public override object GetBrowseComponent()
        {
            return new WebhooksProperties();
        }

        protected class WebhooksProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            public override string GetClassName()
            {
                return "Webhooks";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Auto Scale";
            }
        }
    }
}
