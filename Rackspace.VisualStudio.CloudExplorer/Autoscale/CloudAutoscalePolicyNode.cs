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

    public class CloudAutoscalePolicyNode : AsyncNode
    {
        private readonly CloudAutoScaleProvider _provider;
        private readonly ScalingGroup _scalingGroup;
        private readonly Policy _policy;

        public CloudAutoscalePolicyNode(CloudAutoScaleProvider provider, ScalingGroup scalingGroup, Policy policy)
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
                return _policy.Name;
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
            return false;
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new Node[] { new CloudAutoscaleWebhooksRootNode(_provider, _scalingGroup, _policy) });
        }

        public override bool CanDeleteNode()
        {
            return true;
        }

        protected override DialogResult ConfirmUserDeletingNodeImpl()
        {
            string message = string.Format("Are you sure you want to delete the scaling policy \"{0}\"?", _policy.Name);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        protected override async Task<bool> DeleteNodeAsync(CancellationToken cancellationToken, IProgress<int> progress)
        {
            await _provider.DeletePolicyAsync(_scalingGroup.Id, _policy.Id, cancellationToken).ConfigureAwait(false);
            return true;
        }

        public override object GetBrowseComponent()
        {
            return new PolicyProperties(_provider, _scalingGroup, _policy);
        }

        public class PolicyProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudAutoScaleProvider _provider;
            private readonly ScalingGroup _scalingGroup;
            private readonly Policy _policy;

            public PolicyProperties(CloudAutoScaleProvider provider, ScalingGroup scalingGroup, Policy policy)
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

            [DisplayName("ID")]
            [Category(PropertyCategories.Identity)]
            public PolicyId Id
            {
                get
                {
                    return _policy.Id;
                }
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _policy.Name;
                }
            }

            [DisplayName("Change")]
            public long? Change
            {
                get
                {
                    return _policy.Change;
                }
            }

            [DisplayName("Change Percent")]
            public double? ChangePercent
            {
                get
                {
                    return _policy.ChangePercent;
                }
            }

            [DisplayName("Cooldown")]
            public TimeSpan? Cooldown
            {
                get
                {
                    return _policy.Cooldown;
                }
            }

            [DisplayName("Desired Capacity")]
            public long? DesiredCapacity
            {
                get
                {
                    return _policy.DesiredCapacity;
                }
            }

            [DisplayName("Policy Type")]
            public PolicyType PolicyType
            {
                get
                {
                    return _policy.PolicyType;
                }
            }

            public override string GetClassName()
            {
                return "Policy Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Auto Scale";
            }
        }
    }
}
