namespace Rackspace.VisualStudio.CloudExplorer.Queues
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain.Queues;
    using net.openstack.Providers.Rackspace;
    using Newtonsoft.Json;
    using CancellationToken = System.Threading.CancellationToken;
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
                return RackspaceProductsNode.EmptyIcon;
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
            return true;
        }

        public override bool ConfirmDeletingNode()
        {
            throw new NotImplementedException();
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
