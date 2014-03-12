namespace Rackspace.VisualStudio.CloudExplorer.BlockStorage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
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

    public class CloudBlockStorageVolumeNode : AsyncNode
    {
        private readonly CloudBlockStorageProvider _provider;
        private readonly Volume _volume;

        public CloudBlockStorageVolumeNode(CloudBlockStorageProvider provider, Volume volume)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (volume == null)
                throw new ArgumentNullException("volume");

            this._provider = provider;
            this._volume = volume;
        }

        protected override string DisplayText
        {
            get
            {
                return _volume.DisplayName;
            }
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            IEnumerable<Snapshot> snapshots = await Task.Run(() => _provider.ListSnapshots()).ConfigureAwait(false);
            return snapshots.Where(i => string.Equals(i.VolumeId, _volume.Id, StringComparison.OrdinalIgnoreCase))
                .Select(CreateSnapshotNode)
                .ToArray();
        }

        private CloudBlockStorageSnapshotNode CreateSnapshotNode(Snapshot snapshot)
        {
            return new CloudBlockStorageSnapshotNode(_provider, snapshot);
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

        protected override DialogResult ConfirmUserDeletingNodeImpl()
        {
            string message = string.Format("Are you sure you want to delete the volume \"{0}\"?", _volume.DisplayName);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        protected override async Task<bool> DeleteNodeAsync(CancellationToken cancellationToken, IProgress<int> progress)
        {
            if (!await Task.Run(() => _provider.DeleteVolume(_volume.Id)).ConfigureAwait(false))
                return false;

            return await Task.Run(() => _provider.WaitForVolumeDeleted(_volume.Id, 1000, TimeSpan.FromSeconds(5))).ConfigureAwait(false);
        }

        public override object GetBrowseComponent()
        {
            return new VolumeProperties(_provider, _volume);
        }

        public class VolumeProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudBlockStorageProvider _provider;
            private readonly Volume _volume;

            public VolumeProperties(CloudBlockStorageProvider provider, Volume volume)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (volume == null)
                    throw new ArgumentNullException("volume");

                _provider = provider;
                _volume = volume;
            }

            [DisplayName("ID")]
            [Category(PropertyCategories.Identity)]
            public string Id
            {
                get
                {
                    return _volume.Id;
                }
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _volume.DisplayName;
                }
            }

            [DisplayName("Description")]
            [Category(PropertyCategories.Identity)]
            public string Description
            {
                get
                {
                    return _volume.DisplayDescription;
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
            public long Size
            {
                get
                {
                    return _volume.Size;
                }
            }

            [DisplayName("Volume Type")]
            public string VolumeType
            {
                get
                {
                    return _volume.VolumeType;
                }
            }

            [DisplayName("Created")]
            public DateTimeOffset Created
            {
                get
                {
                    return _volume.CreatedAt;
                }
            }

            [DisplayName("Availability Zone")]
            public string AvailabilityZone
            {
                get
                {
                    return _volume.AvailabilityZone;
                }
            }

            public override string GetClassName()
            {
                return "Volume Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Block Storage";
            }
        }
    }
}
