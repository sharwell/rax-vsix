namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.Drawing;
    using System.Runtime.Serialization;
    using Microsoft.VSDesigner.ServerExplorer;
    using Rackspace.VisualStudio.CloudExplorer.Dns;
    using Rackspace.VisualStudio.CloudExplorer.LoadBalancers;
    using Rackspace.VisualStudio.CloudExplorer.Queues;

    [Serializable]
    public class RackspaceProductsNode : Node, ISerializable, IChildrenNotSerializable
    {
        /// <summary>
        /// The Server Explorer implementation looks for this field before creating a root node.
        /// </summary>
        /// <remarks>
        /// This field definition cannot be moved or altered.
        /// </remarks>
        public static readonly Type parentType = null;

        internal static Image EmptyIcon;
        internal static Node[] EmptyChildren = new Node[0];

        private readonly Bitmap _icon = new Bitmap(16, 16);
        private Node[] _children;

        public RackspaceProductsNode()
        {
            if (EmptyIcon == null)
                EmptyIcon = _icon;
        }

        protected RackspaceProductsNode(SerializationInfo info, StreamingContext context)
        {
            if (EmptyIcon == null)
                EmptyIcon = _icon;
        }

        public override int CompareUnique(Node node)
        {
            return 0;
        }

        public override Node[] CreateChildren()
        {
            if (_children == null)
            {
                _children = new Node[]
                {
                    new CloudLoadBalancersRootNode(),
                    new CloudQueuesRootNode(),
                    new CloudDnsRootNode()
                };
            }

            return _children;
        }

        public override Image Icon
        {
            get
            {
                return _icon;
            }
        }

        public override string Label
        {
            get
            {
                return "Rackspace Public Cloud";
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
