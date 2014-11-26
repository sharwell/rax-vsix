namespace Rackspace.VisualStudio.CloudExplorer.Files
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Microsoft.VSDesigner.ServerExplorer;
    using OpenStack.Collections;
    using OpenStack.Net;
    using OpenStack.Services.ObjectStorage.V1;
    using Container = OpenStack.Services.ObjectStorage.V1.Container;
    using Image = System.Drawing.Image;
    using LocalizableProperties = Microsoft.VisualStudio.Shell.LocalizableProperties;

    public class CloudFilesContainerNode : AsyncNode
    {
        private readonly IObjectStorageService _provider;
        private readonly Container _container;

        public CloudFilesContainerNode(IObjectStorageService provider, Container container)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (container == null)
                throw new ArgumentNullException("container");

            _provider = provider;
            _container = container;
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            try
            {
                int limit = 100;
                ContainerObject[] objects = await ListObjectsAsync(cancellationToken, limit);
                Node[] nodes = Array.ConvertAll(objects, i => CreateObjectNode(i));
                if (nodes.Length == limit)
                {
                    Array.Resize(ref nodes, nodes.Length + 1);
                    nodes[nodes.Length - 1] = new NotImplementedPlaceholderNode();
                }

                return nodes;
            }
            catch (HttpWebException ex)
            {
                if (ex.ResponseMessage == null || ex.ResponseMessage.StatusCode != HttpStatusCode.NotFound)
                    throw;

                return new[] { new DeletedContainerPlaceholderNode() };
            }
        }

        private CloudFilesObjectNode CreateObjectNode(ContainerObject containerObject)
        {
            return new CloudFilesObjectNode(_provider, _container, containerObject);
        }

        private async Task<ContainerObject[]> ListObjectsAsync(CancellationToken cancellationToken, int limit)
        {
            List<ContainerObject> objects = new List<ContainerObject>();
            var objectsResult = await _provider.ListObjectsAsync(_container.Name, cancellationToken).ConfigureAwait(false);
            objects.AddRange(objectsResult.Item2);
            return objects.ToArray();
        }

        public override Image Icon
        {
            get
            {
                return ServerExplorerIcons.CloudFilesContainer;
            }
        }

        protected override string DisplayText
        {
            get
            {
                return _container.Name.Value;
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

        protected override async Task<bool> DeleteNodeAsync(CancellationToken cancellationToken, IProgress<int> progress)
        {
            // use a default instance of Progress<int> so we don't have to use null checks throughout the method
            if (progress == null)
                progress = new Progress<int>();

            // get the current container object count as a second estimate of the number of objects we're deleting.
            ContainerMetadata containerMetadata = await _provider.GetContainerMetadataAsync(_container.Name, cancellationToken).ConfigureAwait(false);
            string containerObjectCountString = null;
            int containerObjectCount = 0;
            if (containerMetadata.Headers.TryGetValue(net.openstack.Providers.Rackspace.CloudFilesProvider.ContainerObjectCount, out containerObjectCountString))
            {
                if (!int.TryParse(containerObjectCountString, out containerObjectCount))
                    containerObjectCount = _container.ObjectCount ?? 1;
            }

            int deletedEstimate = 0;
            int progressEstimate = 0;
            progress.Report(0);

            while (true)
            {
                // operating on 100 items at a time improves performance when deleting objects from multiple containers
                ListObjectsApiCall apiCall = await _provider.PrepareListObjectsAsync(_container.Name, cancellationToken).WithPageSize(100).ConfigureAwait(false);
                Tuple<HttpResponseMessage, Tuple<ContainerMetadata, ReadOnlyCollectionPage<ContainerObject>>> objectsResult = await apiCall.SendAsync(cancellationToken).ConfigureAwait(false);
                ReadOnlyCollectionPage<ContainerObject> objects = objectsResult.Item2.Item2;
                if (objects.Count == 0)
                    break;

                int totalCount = objects.Count;
                if (container != null)
                    totalCount = Math.Max(totalCount, containerObjectCount);

                int count = 0;
                Action continuation =
                    () =>
                    {
                        Interlocked.Increment(ref deletedEstimate);
                        count++;
                        int updatedProgress = (int)Math.Round((100.0 * deletedEstimate) / totalCount, 0, MidpointRounding.AwayFromZero);
                        updatedProgress = Math.Max(0, Math.Min(100, updatedProgress));
                        if (updatedProgress != progressEstimate)
                        {
                            progressEstimate = updatedProgress;
                            progress.Report(updatedProgress);
                        }
                    };

                Task[] removeObjectTasks = Array.ConvertAll(objects.ToArray(),
                    obj => RemoveObjectAsync(_container.Name, obj.Name, continuation, cancellationToken));
                await Task.WhenAll(removeObjectTasks).ConfigureAwait(false);
            }

            await _provider.RemoveContainerAsync(_container.Name, cancellationToken).ConfigureAwait(false);
            return true;
        }

        private async Task RemoveObjectAsync(ContainerName containerName, ObjectName objectName, Action updateProgressAction, CancellationToken cancellationToken)
        {
            try
            {
                await _provider.RemoveObjectAsync(containerName, objectName, cancellationToken).ConfigureAwait(false);
                updateProgressAction();
            }
            catch (HttpWebException ex)
            {
                if (ex.ResponseMessage == null || ex.ResponseMessage.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }
        }

        public override object GetBrowseComponent()
        {
            return new ContainerProperties(_provider, _container);
        }

        public class ContainerProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly IObjectStorageService _provider;
            private readonly Container _container;

            public ContainerProperties(IObjectStorageService provider, Container container)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (container == null)
                    throw new ArgumentNullException("container");

                _provider = provider;
                _container = container;
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _container.Name.Value;
                }
            }

#if false
            [DisplayName("Region")]
            [Category(PropertyCategories.Identity)]
            public string Region
            {
                get
                {
                    return _provider.DefaultRegion;
                }
            }
#endif

            [DisplayName("Size")]
            public long? Size
            {
                get
                {
                    return _container.Size;
                }
            }

            [DisplayName("Object Count")]
            public long? Count
            {
                get
                {
                    return _container.ObjectCount;
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
