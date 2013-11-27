namespace Rackspace.VisualStudio.CloudExplorer.Files
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;
    using Container = net.openstack.Core.Domain.Container;
    using Image = System.Drawing.Image;
    using LocalizableProperties = Microsoft.VisualStudio.Shell.LocalizableProperties;

    public class CloudFilesContainerNode : AsyncNode
    {
        private readonly CloudFilesProvider _provider;
        private readonly Container _container;
        private readonly ContainerCDN _containerCdn;

        public CloudFilesContainerNode(CloudFilesProvider provider, Container container, ContainerCDN containerCdn)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (container == null)
                throw new ArgumentNullException("container");

            _provider = provider;
            _container = container;
            _containerCdn = containerCdn;
        }

        public override int CompareUnique(Node node)
        {
            return Label.CompareTo(node.Label);
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new Node[] { new NotImplementedPlaceholderNode() });
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
                return _container.Name;
            }
        }

        public override bool CanDeleteNode()
        {
            return true;
        }

        protected override DialogResult ConfirmUserDeletingNodeImpl()
        {
            string message = string.Format("Are you sure you want to delete the container \"{0}\" (and all its contents)?", _container.Name);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        protected override async Task<bool> DeleteNodeAsync(CancellationToken cancellationToken)
        {
            await Task.Run(
                () =>
                {
                    _provider.DeleteContainer(_container.Name, deleteObjects: true);
                });
            return true;
        }

        public override object GetBrowseComponent()
        {
            return new ContainerProperties(_provider, _container, _containerCdn);
        }

        public class ContainerProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudFilesProvider _provider;
            private readonly Container _container;
            private readonly ContainerCDN _containerCdn;

            public ContainerProperties(CloudFilesProvider provider, Container container, ContainerCDN containerCdn)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (container == null)
                    throw new ArgumentNullException("container");

                _provider = provider;
                _container = container;
                _containerCdn = containerCdn;
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _container.Name;
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

            [DisplayName("Size")]
            public long Size
            {
                get
                {
                    return _container.Bytes;
                }
            }

            [DisplayName("Object Count")]
            public long Count
            {
                get
                {
                    return _container.Count;
                }
            }

            public override string GetClassName()
            {
                return "Container Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Files";
            }
        }
    }
}
