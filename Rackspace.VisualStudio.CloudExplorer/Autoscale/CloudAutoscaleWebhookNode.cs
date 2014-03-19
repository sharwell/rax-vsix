namespace Rackspace.VisualStudio.CloudExplorer.Autoscale
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.AutoScale;
    using DialogResult = System.Windows.Forms.DialogResult;
    using Image = System.Drawing.Image;
    using LocalizableProperties = Microsoft.VisualStudio.Shell.LocalizableProperties;
    using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
    using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

    public class CloudAutoscaleWebhookNode : AsyncNode
    {
        private readonly CloudAutoScaleProvider _provider;
        private readonly ScalingGroup _scalingGroup;
        private readonly Policy _policy;
        private readonly Webhook _webhook;

        public CloudAutoscaleWebhookNode(CloudAutoScaleProvider provider, ScalingGroup scalingGroup, Policy policy, Webhook webhook)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (scalingGroup == null)
                throw new ArgumentNullException("scalingGroup");
            if (policy == null)
                throw new ArgumentNullException("policy");
            if (webhook == null)
                throw new ArgumentNullException("webhook");

            _provider = provider;
            _scalingGroup = scalingGroup;
            _policy = policy;
            _webhook = webhook;
        }

        protected override string DisplayText
        {
            get
            {
                return _webhook.Name;
            }
        }

        public override Image Icon
        {
            get
            {
                return RackspaceProductsNode.EmptyIcon;
            }
        }

        public override bool IsAlwaysLeaf()
        {
            return true;
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(RackspaceProductsNode.EmptyChildren);
        }

        public override bool CanDeleteNode()
        {
            return true;
        }

        protected override DialogResult ConfirmUserDeletingNodeImpl()
        {
            string message = string.Format("Are you sure you want to delete the webhook \"{0}\"?", _webhook.Name);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        protected override async Task<bool> DeleteNodeAsync(CancellationToken cancellationToken, IProgress<int> progress)
        {
            await _provider.DeleteWebhookAsync(_scalingGroup.Id, _policy.Id, _webhook.Id, cancellationToken).ConfigureAwait(false);
            return true;
        }

        public override object GetBrowseComponent()
        {
            return new WebhookProperties(_provider, _scalingGroup, _policy, _webhook);
        }

        public class WebhookProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudAutoScaleProvider _provider;
            private readonly ScalingGroup _scalingGroup;
            private readonly Policy _policy;
            private readonly Webhook _webhook;

            public WebhookProperties(CloudAutoScaleProvider provider, ScalingGroup scalingGroup, Policy policy, Webhook webhook)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (scalingGroup == null)
                    throw new ArgumentNullException("scalingGroup");
                if (policy == null)
                    throw new ArgumentNullException("policy");
                if (webhook == null)
                    throw new ArgumentNullException("webhook");

                _provider = provider;
                _scalingGroup = scalingGroup;
                _policy = policy;
                _webhook = webhook;
            }

            [DisplayName("ID")]
            [Category(PropertyCategories.Identity)]
            public WebhookId Id
            {
                get
                {
                    return _webhook.Id;
                }
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _webhook.Name;
                }
            }

            public override string GetClassName()
            {
                return "Webhook Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Auto Scale";
            }
        }
    }
}
