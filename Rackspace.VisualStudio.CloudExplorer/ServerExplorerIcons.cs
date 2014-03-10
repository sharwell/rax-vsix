namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.Drawing;

    internal class ServerExplorerIcons
    {
        private static readonly Lazy<Image> _cloudBackup = new Lazy<Image>(() => Resources.CloudBackupIcon);
        private static readonly Lazy<Image> _cloudDatabases = new Lazy<Image>(() => Resources.CloudDatabaseIcon);
        private static readonly Lazy<Image> _cloudDns = new Lazy<Image>(() => Resources.CloudDnsIcon);
        private static readonly Lazy<Image> _cloudFiles = new Lazy<Image>(() => Resources.CloudFilesIcon);
        private static readonly Lazy<Image> _cloudLoadBalancers = new Lazy<Image>(() => Resources.CloudLoadBalancersIcon);
        private static readonly Lazy<Image> _cloudMonitoring = new Lazy<Image>(() => Resources.CloudMonitoringIcon);
        private static readonly Lazy<Image> _cloudServers = new Lazy<Image>(() => Resources.CloudServersIcon);
        private static readonly Lazy<Image> _privateCloud = new Lazy<Image>(() => Resources.PrivateCloudIcon);

        public static Image CloudBackup
        {
            get
            {
                return _cloudBackup.Value;
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
    }
}
