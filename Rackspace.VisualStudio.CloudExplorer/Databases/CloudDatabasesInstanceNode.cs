namespace Rackspace.VisualStudio.CloudExplorer.Databases
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.Databases;
    using AsyncCompletionOption = net.openstack.Core.AsyncCompletionOption;
    using DialogResult = System.Windows.Forms.DialogResult;
    using Image = System.Drawing.Image;
    using LocalizableProperties = Microsoft.VisualStudio.Shell.LocalizableProperties;
    using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
    using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

    public class CloudDatabasesInstanceNode : AsyncNode
    {
        private readonly CloudDatabasesProvider _provider;
        private readonly DatabaseInstance _instance;

        public CloudDatabasesInstanceNode(CloudDatabasesProvider provider, DatabaseInstance instance)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (instance == null)
                throw new ArgumentNullException("instance");

            _provider = provider;
            _instance = instance;
        }

        protected override string DisplayText
        {
            get
            {
                return _instance.Name;
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
            string message = string.Format("Are you sure you want to delete the database instance \"{0}\"?", _instance.Name);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        protected override async Task<bool> DeleteNodeAsync(CancellationToken cancellationToken, IProgress<int> progress)
        {
            await _provider.RemoveDatabaseInstanceAsync(_instance.Id, AsyncCompletionOption.RequestCompleted, cancellationToken, null).ConfigureAwait(false);
            return true;
        }

        public override object GetBrowseComponent()
        {
            return new DatabaseInstanceProperties(_provider, _instance);
        }

        public class DatabaseInstanceProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudDatabasesProvider _provider;
            private readonly DatabaseInstance _instance;

            public DatabaseInstanceProperties(CloudDatabasesProvider provider, DatabaseInstance instance)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (instance == null)
                    throw new ArgumentNullException("instance");

                _provider = provider;
                _instance = instance;
            }

            [DisplayName("ID")]
            [Category(PropertyCategories.Identity)]
            public DatabaseInstanceId Id
            {
                get
                {
                    return _instance.Id;
                }
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _instance.Name;
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

            [DisplayName("Created")]
            public DateTimeOffset? Created
            {
                get
                {
                    return _instance.Created;
                }
            }

            [DisplayName("Last Modified")]
            public DateTimeOffset? LastModified
            {
                get
                {
                    return _instance.Updated;
                }
            }

            [DisplayName("Host Name")]
            public string HostName
            {
                get
                {
                    return _instance.HostName;
                }
            }

            [DisplayName("Status")]
            public DatabaseInstanceStatus Status
            {
                get
                {
                    return _instance.Status;
                }
            }

            [DisplayName("Size")]
            [Category(PropertyCategories.DatabaseVolumeConfiguration)]
            public int? Size
            {
                get
                {
                    if (_instance.VolumeConfiguration == null)
                        return null;

                    return _instance.VolumeConfiguration.Size;
                }
            }

            [DisplayName("Used")]
            [Category(PropertyCategories.DatabaseVolumeConfiguration)]
            public double? Used
            {
                get
                {
                    if (_instance.VolumeConfiguration == null)
                        return null;

                    return _instance.VolumeConfiguration.Used;
                }
            }
        }
    }
}
