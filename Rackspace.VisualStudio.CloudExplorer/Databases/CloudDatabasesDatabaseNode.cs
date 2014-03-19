namespace Rackspace.VisualStudio.CloudExplorer.Databases
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.Databases;
    using DialogResult = System.Windows.Forms.DialogResult;
    using Image = System.Drawing.Image;
    using LocalizableProperties = Microsoft.VisualStudio.Shell.LocalizableProperties;
    using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
    using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

    public class CloudDatabasesDatabaseNode : AsyncNode
    {
        private readonly CloudDatabasesProvider _provider;
        private readonly DatabaseInstance _instance;
        private readonly Database _database;

        public CloudDatabasesDatabaseNode(CloudDatabasesProvider provider, DatabaseInstance instance, Database database)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (database == null)
                throw new ArgumentNullException("database");

            this._provider = provider;
            this._instance = instance;
            this._database = database;
        }

        protected override string DisplayText
        {
            get
            {
                return _database.Name.Value;
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
            string message = string.Format("Are you sure you want to delete the database \"{0}\"?", _database.Name);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        protected override async Task<bool> DeleteNodeAsync(CancellationToken cancellationToken, IProgress<int> progress)
        {
            await _provider.RemoveDatabaseAsync(_instance.Id, _database.Name, cancellationToken).ConfigureAwait(false);
            return true;
        }

        public override object GetBrowseComponent()
        {
            return new DatabaseProperties(_provider, _instance, _database);
        }

        public class DatabaseProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudDatabasesProvider _provider;
            private readonly DatabaseInstance _instance;
            private readonly Database _database;

            public DatabaseProperties(CloudDatabasesProvider provider, DatabaseInstance instance, Database database)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (instance == null)
                    throw new ArgumentNullException("instance");
                if (database == null)
                    throw new ArgumentNullException("database");

                this._provider = provider;
                this._instance = instance;
                this._database = database;
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public DatabaseName Name
            {
                get
                {
                    return _database.Name;
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

            public override string GetClassName()
            {
                return "Database Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Databases";
            }
        }
    }
}
