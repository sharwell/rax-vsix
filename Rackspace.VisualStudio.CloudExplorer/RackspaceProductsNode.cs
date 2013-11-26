namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.Drawing;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using File = System.IO.File;

    [Serializable]
    public class RackspaceProductsNode : AsyncNode, ISerializable, IChildrenNotSerializable
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

        private static Lazy<JObject> _developerSettings = new Lazy<JObject>(
            () =>
            {
                if (File.Exists(@"C:\.openstack_net"))
                    return JObject.Parse(File.ReadAllText(@"C:\.openstack_net"));

                return null;
            }, true);

        private readonly Lazy<Image> _icon = new Lazy<Image>(() => Resources.CloudDatabaseIcon);

        public RackspaceProductsNode()
        {
            if (EmptyIcon == null)
                EmptyIcon = new Bitmap(16, 16);
        }

        protected RackspaceProductsNode(SerializationInfo info, StreamingContext context)
        {
            if (EmptyIcon == null)
                EmptyIcon = new Bitmap(16, 16);
        }

        public static JObject DeveloperSettings
        {
            get
            {
                return _developerSettings.Value;
            }
        }

        public override int CompareUnique(Node node)
        {
            return 0;
        }

        public override Image Icon
        {
            get
            {
                return _icon.Value;
            }
        }

        protected override string DisplayText
        {
            get
            {
                return "Rackspace Public Cloud";
            }
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            JObject developerSettings = DeveloperSettings;
            if (developerSettings == null)
                return Task.FromResult(RackspaceProductsNode.EmptyChildren);

            JObject testIdentity = developerSettings["TestIdentity"] as JObject;
            if (testIdentity == null)
                return Task.FromResult(RackspaceProductsNode.EmptyChildren);

            try
            {
                CloudIdentity identity = testIdentity.ToObject<CloudIdentity>();
                if (!string.IsNullOrEmpty(identity.Username))
                    return Task.FromResult(new Node[] { new CloudProjectNode(identity) });
            }
            catch (JsonSerializationException)
            {
            }

            return Task.FromResult(RackspaceProductsNode.EmptyChildren);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }
    }
}
