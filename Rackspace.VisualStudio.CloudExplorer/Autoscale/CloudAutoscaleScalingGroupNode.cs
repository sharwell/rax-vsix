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

    public class CloudAutoscaleScalingGroupNode : AsyncNode
    {
        private readonly CloudAutoScaleProvider _provider;
        private readonly ScalingGroup _scalingGroup;

        public CloudAutoscaleScalingGroupNode(CloudAutoScaleProvider provider, ScalingGroup scalingGroup)
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
                return _scalingGroup.State.Name;
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
            return Task.FromResult(new Node[] { new CloudAutoscalePoliciesRootNode(_provider, _scalingGroup) });
        }

        public override bool CanDeleteNode()
        {
            return true;
        }

        protected override DialogResult ConfirmUserDeletingNodeImpl()
        {
            string message = string.Format("Are you sure you want to delete the scaling group \"{0}\"?", _scalingGroup.State.Name);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        protected override async Task<bool> DeleteNodeAsync(CancellationToken cancellationToken, IProgress<int> progress)
        {
            await _provider.DeleteGroupAsync(_scalingGroup.Id, false, cancellationToken).ConfigureAwait(false);
            return true;
        }

        public override object GetBrowseComponent()
        {
            return new ScalingGroupProperties(_provider, _scalingGroup);
        }

        public class ScalingGroupProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudAutoScaleProvider _provider;
            private readonly ScalingGroup _scalingGroup;

            public ScalingGroupProperties(CloudAutoScaleProvider provider, ScalingGroup scalingGroup)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (scalingGroup == null)
                    throw new ArgumentNullException("scalingGroup");

                _provider = provider;
                _scalingGroup = scalingGroup;
            }

            [DisplayName("ID")]
            [Category(PropertyCategories.Identity)]
            public string Id
            {
                get
                {
                    if (_scalingGroup.Id == null)
                        return null;

                    return _scalingGroup.Id.Value;
                }
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _scalingGroup.State.Name;
                }
            }

            [DisplayName("Region")]
            [Category(PropertyCategories.Identity)]
            public string Region
            {
                get
                {
                    return _provider.DefaultRegion;
                }
            }

            [DisplayName("Active Capacity")]
            [Category(PropertyCategories.AutoScaleState)]
            public long? ActiveCapacity
            {
                get
                {
                    if (_scalingGroup.State == null)
                        return null;

                    return _scalingGroup.State.ActiveCapacity;
                }
            }

            [DisplayName("Desired Capacity")]
            [Description(
                "Gets the desired number of resources in the scaling group. This property " +
                "is the sum of the Active Capacity and Pending Capacity.")]
            [Category(PropertyCategories.AutoScaleState)]
            public long? DesiredCapacity
            {
                get
                {
                    if (_scalingGroup.State == null)
                        return null;

                    return _scalingGroup.State.DesiredCapacity;
                }
            }

            [DisplayName("Pending Capacity")]
            [Description("Gets the number of servers currently in the BUILD state.")]
            [Category(PropertyCategories.AutoScaleState)]
            public long? PendingCapacity
            {
                get
                {
                    if (_scalingGroup.State == null)
                        return null;

                    return _scalingGroup.State.PendingCapacity;
                }
            }

            [DisplayName("Paused")]
            [Category(PropertyCategories.AutoScaleState)]
            public bool? Paused
            {
                get
                {
                    if (_scalingGroup.State == null)
                        return null;

                    return _scalingGroup.State.Paused;
                }
            }

            [DisplayName("Cooldown")]
            [Category(PropertyCategories.AutoScaleGroupConfiguration)]
            public TimeSpan? Cooldown
            {
                get
                {
                    if (_scalingGroup.GroupConfiguration == null)
                        return null;

                    return _scalingGroup.GroupConfiguration.Cooldown;
                }
            }

            [DisplayName("Max Entities")]
            [Category(PropertyCategories.AutoScaleGroupConfiguration)]
            public long? MaxEntities
            {
                get
                {
                    if (_scalingGroup.GroupConfiguration == null)
                        return null;

                    return _scalingGroup.GroupConfiguration.MaxEntities;
                }
            }

            [DisplayName("Min Entities")]
            [Category(PropertyCategories.AutoScaleGroupConfiguration)]
            public long? MinEntities
            {
                get
                {
                    if (_scalingGroup.GroupConfiguration == null)
                        return null;

                    return _scalingGroup.GroupConfiguration.MinEntities;
                }
            }

            [DisplayName("Launch Type")]
            [Category(PropertyCategories.AutoScaleLaunchConfiguration)]
            public LaunchType LaunchType
            {
                get
                {
                    if (_scalingGroup.LaunchConfiguration == null)
                        return null;

                    return _scalingGroup.LaunchConfiguration.LaunchType;
                }
            }

            public override string GetClassName()
            {
                return "Scaling Group Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Auto Scale";
            }
        }
    }
}
