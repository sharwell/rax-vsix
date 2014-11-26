namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Rackspace.VisualStudio.CloudExplorer.AccountManager;
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

        public override Image Icon
        {
            get
            {
                return ServerExplorerIcons.Cloud;
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

        public override ContextMenuItem[] GetContextMenuItems()
        {
            ContextMenuItem[] contextMenuItems = base.GetContextMenuItems();
            List<ContextMenuItem> items = contextMenuItems != null ? contextMenuItems.ToList() : new List<ContextMenuItem>();
            items.Insert(0, new ContextMenuItem("&Manage subscriptions...", HandleManageSubscriptions));
            return items.ToArray();
        }

        private void HandleManageSubscriptions(object sender, EventArgs e)
        {
            ManageSubscriptionsWindow window = new ManageSubscriptionsWindow();
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
    }
}
