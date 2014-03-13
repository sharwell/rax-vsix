namespace Rackspace.VisualStudio.CloudExplorer.BlockStorage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;
    using Image = System.Drawing.Image;

    public class CloudBlockStorageOrphanedSnapshotsNode : AsyncNode
    {
        private readonly CloudBlockStorageProvider _provider;

        public CloudBlockStorageOrphanedSnapshotsNode(CloudBlockStorageProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            _provider = provider;
        }

        protected override string DisplayText
        {
            get
            {
                return "Orphaned Snapshots";
            }
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            Task<IEnumerable<Volume>> volumes = Task.Run(() => _provider.ListVolumes());
            Task<IEnumerable<Snapshot>> snapshots = Task.Run(() => _provider.ListSnapshots());
            await Task.WhenAll(volumes, snapshots);

            HashSet<string> volumeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            volumeIds.UnionWith(volumes.Result.Select(i => i.Id));

            return snapshots.Result.Where(i => !volumeIds.Contains(i.VolumeId))
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
            return false;
        }
    }
}
