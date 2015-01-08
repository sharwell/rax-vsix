namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.Drawing;

    internal static class ServerExplorerIcons
    {
        private static readonly Lazy<Image> _cloud = new Lazy<Image>(() => Resources.CloudIcon);
        private static readonly Lazy<Image> _cloudAutoscale = new Lazy<Image>(() => Resources.CloudAutoscaleIcon);
        private static readonly Lazy<Image> _cloudBackup = new Lazy<Image>(() => Resources.CloudBackupIcon);
        private static readonly Lazy<Image> _cloudBlockStorage = new Lazy<Image>(() => Resources.CloudBlockStorageIcon);
        private static readonly Lazy<Image> _cloudBlockStorageVolume = new Lazy<Image>(() => Resources.CloudBlockStorageVolumeIcon);
        private static readonly Lazy<Image> _cloudDatabases = new Lazy<Image>(() => Resources.CloudDatabaseIcon);
        private static readonly Lazy<Image> _cloudDns = new Lazy<Image>(() => Resources.CloudDnsIcon);
        private static readonly Lazy<Image> _cloudFiles = new Lazy<Image>(() => Resources.CloudFilesIcon);
        private static readonly Lazy<Image> _cloudFilesContainer = new Lazy<Image>(() => Resources.CloudFilesContainerIcon);
        private static readonly Lazy<Image> _cloudLoadBalancers = new Lazy<Image>(() => Resources.CloudLoadBalancersIcon);
        private static readonly Lazy<Image> _cloudMonitoring = new Lazy<Image>(() => Resources.CloudMonitoringIcon);
        private static readonly Lazy<Image> _cloudNetworks = new Lazy<Image>(() => Resources.CloudNetworksIcon);
        private static readonly Lazy<Image> _cloudQueues = new Lazy<Image>(() => Resources.CloudQueuesIcon);
        private static readonly Lazy<Image> _cloudQueuesQueue = new Lazy<Image>(() => Resources.CloudQueuesQueueIcon);
        private static readonly Lazy<Image> _cloudServer = new Lazy<Image>(() => Resources.CloudServerIcon);
        private static readonly Lazy<Image> _cloudServers = new Lazy<Image>(() => Resources.CloudServersIcon);
        private static readonly Lazy<Image> _privateCloud = new Lazy<Image>(() => Resources.PrivateCloudIcon);
        private static readonly Lazy<Image> _regionIcon = new Lazy<Image>(() => Resources.RegionIcon);

        public static Image Cloud
        {
            get
            {
                return _cloud.Value;
            }
        }

        public static Image CloudAutoscale
        {
            get
            {
                return _cloudAutoscale.Value;
            }
        }

        public static Image CloudBackup
        {
            get
            {
                return _cloudBackup.Value;
            }
        }

        public static Image CloudBlockStorage
        {
            get
            {
                return _cloudBlockStorage.Value;
            }
        }

        public static Image CloudBlockStorageVolume
        {
            get
            {
                return _cloudBlockStorageVolume.Value;
            }
        }

        public static Image CloudDatabases
        {
            get
            {
                return _cloudDatabases.Value;
            }
        }

        public static Image CloudDns
        {
            get
            {
                return _cloudDns.Value;
            }
        }

        public static Image CloudFiles
        {
            get
            {
                return _cloudFiles.Value;
            }
        }

        public static Image CloudFilesContainer
        {
            get
            {
                return _cloudFilesContainer.Value;
            }
        }

        public static Image CloudLoadBalancers
        {
            get
            {
                return _cloudLoadBalancers.Value;
            }
        }

        public static Image CloudMonitoring
        {
            get
            {
                return _cloudMonitoring.Value;
            }
        }

        public static Image CloudNetworks
        {
            get
            {
                return _cloudNetworks.Value;
            }
        }

        public static Image CloudQueues
        {
            get
            {
                return _cloudQueues.Value;
            }
        }

        public static Image CloudQueuesQueue
        {
            get
            {
                return _cloudQueuesQueue.Value;
            }
        }

        public static Image CloudServer
        {
            get
            {
                return _cloudServer.Value;
            }
        }

        public static Image CloudServers
        {
            get
            {
                return _cloudServers.Value;
            }
        }

        public static Image PrivateCloud
        {
            get
            {
                return _privateCloud.Value;
            }
        }

        public static Image Region
        {
            get
            {
                return _regionIcon.Value;
            }
        }
    }
}
