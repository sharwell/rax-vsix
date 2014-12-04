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
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using Rackspace.VisualStudio.CloudExplorer.AccountManager;

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

        internal AccountStore GetAccountStore()
        {
            IComponentModel componentModel = (IComponentModel)GetNodeSite().GetService(typeof(SComponentModel));
            return componentModel.DefaultExportProvider.GetExportedValue<AccountStore>();
        }

        protected override Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            List<Node> rootNodes = new List<Node>();
            foreach (CloudIdentity identity in GetAccountStore().Credentials)
            {
                rootNodes.Add(new CloudProjectNode(identity));
            }

            return Task.FromResult(rootNodes.ToArray());
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
            ManageSubscriptionsWindow window = new ManageSubscriptionsWindow(GetAccountStore());
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
        }
    }
}
