namespace Rackspace.VisualStudio.CloudExplorer.BlockStorage
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;
    using DialogResult = System.Windows.Forms.DialogResult;
    using Image = System.Drawing.Image;
    using LocalizableProperties = Microsoft.VisualStudio.Shell.LocalizableProperties;
    using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
    using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

    public class CloudBlockStorageSnapshotNode : AsyncNode
    {
        private readonly CloudBlockStorageProvider _provider;
        private readonly Snapshot _snapshot;

        public CloudBlockStorageSnapshotNode(CloudBlockStorageProvider provider, Snapshot snapshot)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (snapshot == null)
                throw new ArgumentNullException("snapshot");

            this._provider = provider;
            this._snapshot = snapshot;
        }

        protected override string DisplayText
        {
            get
            {
                return _snapshot.DisplayName;
            }
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(RackspaceProductsNode.EmptyChildren);
        }

        public override Image Icon
        {
            get
            {
                return RackspaceProductsNode.EmptyIcon;
            }
        }

        public override bool CanDeleteNode()
        {
            return true;
        }

        public override bool IsAlwaysLeaf()
        {
            return true;
        }

        protected override DialogResult ConfirmUserDeletingNodeImpl()
        {
            string message = string.Format("Are you sure you want to delete the snapshot \"{0}\"?", _snapshot.DisplayName);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        protected override async Task<bool> DeleteNodeAsync(CancellationToken cancellationToken, IProgress<int> progress)
        {
            if (!await Task.Run(() => _provider.DeleteSnapshot(_snapshot.Id)).ConfigureAwait(false))
                return false;

            return await Task.Run(() => _provider.WaitForSnapshotDeleted(_snapshot.Id, 1000, TimeSpan.FromSeconds(5))).ConfigureAwait(false);
        }

        public override object GetBrowseComponent()
        {
            return new SnapshotProperties(_provider, _snapshot);
        }

        public class SnapshotProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudBlockStorageProvider _provider;
            private readonly Snapshot _snapshot;

            public SnapshotProperties(CloudBlockStorageProvider provider, Snapshot snapshot)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (snapshot == null)
                    throw new ArgumentNullException("snapshot");

                _provider = provider;
                _snapshot = snapshot;
            }

            [DisplayName("ID")]
            [Category(PropertyCategories.Identity)]
            public string Id
            {
                get
                {
                    return _snapshot.Id;
                }
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _snapshot.DisplayName;
                }
            }

            [DisplayName("Description")]
            [Category(PropertyCategories.Identity)]
            public string Description
            {
                get
                {
                    return _snapshot.DisplayDescription;
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

            [DisplayName("Size (GB)")]
            public string Size
            {
                get
                {
                    return _snapshot.Size;
                }
            }

            [DisplayName("Created")]
            public DateTimeOffset Created
            {
                get
                {
                    return _snapshot.CreatedAt;
                }
            }

            [DisplayName("Status")]
            public string Status
            {
                get
                {
                    return _snapshot.Status != null ? _snapshot.Status.Name : "";
                }
            }

            [DisplayName("Volume ID")]
            public string VolumeId
            {
                get
                {
                    return _snapshot.VolumeId;
                }
            }

            public override string GetClassName()
            {
                return "Snapshot Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Block Storage";
            }
        }
    }
}
