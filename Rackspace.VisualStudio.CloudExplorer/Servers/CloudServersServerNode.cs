namespace Rackspace.VisualStudio.CloudExplorer.Servers
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

    public class CloudServersServerNode : AsyncNode
    {
        private readonly CloudServersProvider _provider;
        private readonly Server _server;

        public CloudServersServerNode(CloudServersProvider provider, Server server)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (server == null)
                throw new ArgumentNullException("server");

            this._provider = provider;
            this._server = server;
        }

        protected override string DisplayText
        {
            get
            {
                return _server.Name;
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
            string message = string.Format("Are you sure you want to delete the server \"{0}\"?", _server.Name);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        protected override async Task<bool> DeleteNodeAsync(CancellationToken cancellationToken, IProgress<int> progress)
        {
            if (!await Task.Run(() => _provider.DeleteServer(_server.Id)).ConfigureAwait(false))
                return false;

            progress = progress ?? new Progress<int>();
            await Task.Run(() => _provider.WaitForServerDeleted(_server.Id, 1000, TimeSpan.FromSeconds(5), progress.Report));
            return true;
        }

        public override object GetBrowseComponent()
        {
            return new ServerProperties(_provider, _server);
        }

        public class ServerProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudServersProvider _provider;
            private readonly Server _server;

            public ServerProperties(CloudServersProvider provider, Server server)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (server == null)
                    throw new ArgumentNullException("server");

                _provider = provider;
                _server = server;
            }

            [DisplayName("ID")]
            [Category(PropertyCategories.Identity)]
            public string Id
            {
                get
                {
                    return _server.Id;
                }
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _server.Name;
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

            [DisplayName("Tenant ID")]
            [Category(PropertyCategories.Identity)]
            public string TenantId
            {
                get
                {
                    return _server.TenantId;
                }
            }

            [DisplayName("User ID")]
            [Category(PropertyCategories.Identity)]
            public string UserId
            {
                get
                {
                    return _server.UserId;
                }
            }

            [DisplayName("Access IP v4")]
            public string AccessIPv4
            {
                get
                {
                    return _server.AccessIPv4;
                }
            }

            [DisplayName("Access IP v6")]
            public string AccessIPv6
            {
                get
                {
                    return _server.AccessIPv6;
                }
            }

            [DisplayName("Created")]
            public DateTimeOffset Created
            {
                get
                {
                    return _server.Created;
                }
            }

            [DisplayName("Last Modified")]
            public DateTimeOffset LastModified
            {
                get
                {
                    return _server.Updated;
                }
            }

            [DisplayName("Disk Configuration")]
            public DiskConfiguration DiskConfig
            {
                get
                {
                    return _server.DiskConfig;
                }
            }

            [DisplayName("Flavor Name")]
            public string Flavor
            {
                get
                {
                    return _server.Flavor != null ? _server.Flavor.Name ?? _server.Flavor.Id : null;
                }
            }

            [DisplayName("Host ID")]
            public string HostId
            {
                get
                {
                    return _server.HostId;
                }
            }

            [DisplayName("Image Name")]
            public string Image
            {
                get
                {
                    return _server.Image != null ? _server.Image.Name ?? _server.Image.Id : null;
                }
            }

            [DisplayName("Power State")]
            public PowerState PowerState
            {
                get
                {
                    return _server.PowerState;
                }
            }

            [DisplayName("Progress")]
            public int Progress
            {
                get
                {
                    return _server.Progress;
                }
            }

            [DisplayName("Status")]
            public ServerState Status
            {
                get
                {
                    return _server.Status;
                }
            }

            [DisplayName("Task State")]
            public TaskState TaskState
            {
                get
                {
                    return _server.TaskState;
                }
            }

            [DisplayName("VM State")]
            public VirtualMachineState VMState
            {
                get
                {
                    return _server.VMState;
                }
            }

            public override string GetClassName()
            {
                return "Server Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Servers";
            }
        }
    }
}
