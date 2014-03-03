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

        public CloudQueueNode(CloudQueuesProvider provider, CloudQueue queue)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (queue == null)
                throw new ArgumentNullException("queue");

            _provider = provider;
            _queue = queue;
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
                return _queue.Name.Value;
            }
        }

        public override bool CanDeleteNode()
        {
            return !IsDeleting;
        }

        protected override DialogResult ConfirmUserDeletingNodeImpl()
        {
            string message = string.Format("Are you sure you want to delete the queue \"{0}\"?", _queue.Name);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        protected override async Task<bool> DeleteNodeAsync(CancellationToken cancellationToken)
        {
            await _provider.DeleteQueueAsync(_queue.Name, cancellationToken);
            return true;
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
