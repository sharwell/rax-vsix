namespace Rackspace.VisualStudio.CloudExplorer.Queues
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Microsoft.VisualStudio;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain.Queues;
    using net.openstack.Providers.Rackspace;
    using Newtonsoft.Json;
    using CancellationToken = System.Threading.CancellationToken;
    using CancellationTokenSource = System.Threading.CancellationTokenSource;
    using LocalizableProperties = Microsoft.VisualStudio.Shell.LocalizableProperties;

    public class CloudQueueNode : AsyncNode
    {
        private readonly CloudQueuesProvider _provider;
        private readonly CloudQueue _queue;

        private bool _deleting;

        public CloudQueueNode(CloudQueuesProvider provider, CloudQueue queue)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (queue == null)
                throw new ArgumentNullException("queue");

            _provider = provider;
            _queue = queue;
        }

        public override int CompareUnique(Node node)
        {
            return Label.CompareTo(node.Label);
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(RackspaceProductsNode.EmptyChildren);
        }

        public override Image Icon
        {
            get
            {
                return ServerExplorerIcons.CloudDatabases;
            }
        }

        protected override string DisplayText
        {
            get
            {
                if (_deleting)
                    return string.Format("{0} (Deleting...)", _queue.Name);

                return _queue.Name.Value;
            }
        }

        public override bool CanDeleteNode()
        {
            return !_deleting;
        }

        public override bool ConfirmDeletingNode()
        {
            string message = string.Format("Are you sure you want to delete the queue \"{0}\"?", _queue.Name);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return false;

            try
            {
                _deleting = true;
                nodeSite.UpdateLabel();
                using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60)))
                {
                    _provider.DeleteQueueAsync(_queue.Name, cancellationTokenSource.Token).Wait();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;

                nodeSite.ShowMessageBox(string.Format("Could not delete queue: {0}", ex.Message), MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
            finally
            {
                _deleting = false;
            }
        }

        public override object GetBrowseComponent()
        {
            return new QueueProperties(_provider, _queue);
        }

        public class QueueProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudQueuesProvider _provider;
            private readonly CloudQueue _queue;

            public QueueProperties(CloudQueuesProvider provider, CloudQueue queue)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (queue == null)
                    throw new ArgumentNullException("queue");

                _provider = provider;
                _queue = queue;
            }

            [DisplayName("URI")]
            [Category(PropertyCategories.Identity)]
            public string Uri
            {
                get
                {
                    return _queue.Href.ToString();
                }
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _queue.Name.Value;
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

            [DisplayName("Metadata")]
            public string Metadata
            {
                get
                {
                    if (_queue.Metadata == null)
                        return string.Empty;

                    return _queue.Metadata.ToString(Formatting.None);
                }
            }

            public override string GetClassName()
            {
                return "Queue Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Queues";
            }
        }
    }
}
