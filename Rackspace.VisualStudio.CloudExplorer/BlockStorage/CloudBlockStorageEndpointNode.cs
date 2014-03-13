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

    public class CloudBlockStorageEndpointNode : EndpointNode
    {
        public CloudBlockStorageEndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            : base(identity, serviceCatalog, endpoint)
        {
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            Tuple<CloudBlockStorageProvider, Volume[]> volumes = await ListVolumesAsync(cancellationToken);

            List<CloudBlockStorageVolumeNode> volumeNodes = new List<CloudBlockStorageVolumeNode>();
            foreach (Volume volume in volumes.Item2)
                volumeNodes.Add(CreateVolumeNode(volumes.Item1, volume));

            CloudBlockStorageOrphanedSnapshotsNode orphanedNode = CreateOrphanedSnapshotsNode(volumes.Item1);
            return volumeNodes.Cast<Node>().Concat(new[] { orphanedNode }).ToArray();
        }

        private CloudBlockStorageVolumeNode CreateVolumeNode(CloudBlockStorageProvider provider, Volume volume)
        {
            return new CloudBlockStorageVolumeNode(provider, volume);
        }

        private CloudBlockStorageOrphanedSnapshotsNode CreateOrphanedSnapshotsNode(CloudBlockStorageProvider provider)
        {
            return new CloudBlockStorageOrphanedSnapshotsNode(provider);
        }

        private async Task<Tuple<CloudBlockStorageProvider, Volume[]>> ListVolumesAsync(CancellationToken cancellationToken)
        {
            CloudBlockStorageProvider provider = CreateProvider();
            List<Volume> volumes = new List<Volume>();
            volumes.AddRange(await Task.Run(() => provider.ListVolumes()));
            return Tuple.Create(provider, volumes.ToArray());
        }

        private CloudBlockStorageProvider CreateProvider()
        {
            return new CloudBlockStorageProvider(Identity, Endpoint.Region, null, null);
        }
    }
}
