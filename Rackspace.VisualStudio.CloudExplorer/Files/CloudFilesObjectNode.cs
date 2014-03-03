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

    public class CloudFilesObjectNode : AsyncNode
    {
        private readonly CloudFilesProvider _provider;
        private readonly Container _container;
        private readonly ContainerObject _containerObject;

        public CloudFilesObjectNode(CloudFilesProvider provider, Container container, ContainerObject containerObject)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (container == null)
                throw new ArgumentNullException("container");
            if (containerObject == null)
                throw new ArgumentNullException("containerObject");

            _provider = provider;
            _container = container;
            _containerObject = containerObject;
        }

        public override bool IsAlwaysLeaf()
        {
            return true;
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
                return _containerObject.Name;
            }
        }

        public override bool CanDeleteNode()
        {
            return true;
        }

        protected override DialogResult ConfirmUserDeletingNodeImpl()
        {
            string message = string.Format("Are you sure you want to delete the object \"{0}\"?", _containerObject.Name);
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
                    _provider.DeleteObject(_container.Name, _containerObject.Name);
                });
            return true;
        }

        public override object GetBrowseComponent()
        {
            return new ObjectProperties(_provider, _container, _containerObject);
        }

        public class ObjectProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudFilesProvider _provider;
            private readonly Container _container;
            private readonly ContainerObject _containerObject;

            public ObjectProperties(CloudFilesProvider provider, Container container, ContainerObject containerObject)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (container == null)
                    throw new ArgumentNullException("container");
                if (containerObject == null)
                    throw new ArgumentNullException("containerObject");

                _provider = provider;
                _container = container;
                _containerObject = containerObject;
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _containerObject.Name;
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

            [DisplayName("ETag")]
            [Category(PropertyCategories.Identity)]
            public string ETag
            {
                get
                {
                    return _containerObject.Hash;
                }
            }

            [DisplayName("Size")]
            public long Size
            {
                get
                {
                    return _containerObject.Bytes;
                }
            }

            [DisplayName("Content Type")]
            public string ContentType
            {
                get
                {
                    return _containerObject.ContentType;
                }
            }

            [DisplayName("Last Modified")]
            public DateTimeOffset LastModified
            {
                get
                {
                    return _containerObject.LastModified;
                }
            }

            public override string GetClassName()
            {
                return "Object Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Files";
            }
        }
    }
}
