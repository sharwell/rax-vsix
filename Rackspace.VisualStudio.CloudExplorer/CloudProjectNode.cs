namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;
    using Rackspace.VisualStudio.CloudExplorer.Dns;
    using Rackspace.VisualStudio.CloudExplorer.LoadBalancers;
    using Rackspace.VisualStudio.CloudExplorer.Queues;
    using DialogResult = System.Windows.Forms.DialogResult;
    using Image = System.Drawing.Image;
    using MessageBoxButtons = System.Windows.Forms.MessageBoxButtons;
    using MessageBoxIcon = System.Windows.Forms.MessageBoxIcon;

    public class CloudProjectNode : AsyncNode
    {
        private readonly CloudIdentity _identity;
        private readonly CloudIdentityProvider _provider;
        private string _tenantId;

        private Node[] _children;

        public CloudProjectNode(CloudIdentity identity)
        {
            _identity = identity;

            _provider = new CloudIdentityProvider();
            _provider.GetTokenAsync(identity, CancellationToken.None).ContinueWith(
                task =>
                {
                    _tenantId = task.Result.Tenant.Id;
                    INodeSite nodeSite = GetNodeSite();
                    if (nodeSite != null)
                        nodeSite.UpdateLabel();
                });
        }

        protected override string DisplayText
        {
            get
            {
                if (!string.IsNullOrEmpty(_identity.Username) && !string.IsNullOrEmpty(_tenantId))
                    return string.Format("{0} ({1})", _identity.Username, _tenantId);
                else if (!string.IsNullOrEmpty(_identity.Username))
                    return _identity.Username;

                return _tenantId ?? string.Empty;
            }
        }

        public override bool CanDeleteNode()
        {
            return true;
        }

        protected override DialogResult ConfirmUserDeletingNodeImpl()
        {
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite == null)
                return DialogResult.Cancel;

            string message = "Are you sure you want to remove this identity? This operation will not delete the corresponding account at the provider; it simply removes it from the IDE's user interface.";
            return nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        protected override Task<bool> DeleteNodeAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            if (_children == null)
            {
                UserAccess userAccess = await _provider.GetUserAccessAsync(_identity, false, cancellationToken);
                if (userAccess == null || userAccess.ServiceCatalog == null || userAccess.ServiceCatalog.Length == 0)
                    return RackspaceProductsNode.EmptyChildren;

                List<Node> nodes = new List<Node>();
                foreach (ServiceCatalog serviceCatalog in userAccess.ServiceCatalog)
                {
                    switch (serviceCatalog.Type)
                    {
                    case "rax:dns":
                        nodes.Add(new CloudDnsRootNode(serviceCatalog, _identity));
                        break;

                    case "rax:queues":
                        nodes.Add(new CloudQueuesRootNode(serviceCatalog, _identity));
                        break;

                    case "rax:load-balancer":
                        nodes.Add(new CloudLoadBalancersRootNode(serviceCatalog, _identity));
                        break;

                    default:
                        break;
                    }
                }

                _children = nodes.ToArray();
            }

            return _children;
        }

        public override int CompareUnique(Node node)
        {
            return Label.CompareTo(node.Label);
        }

        public override Image Icon
        {
            get
            {
                return ServerExplorerIcons.PrivateCloud;
            }
        }
    }
}
