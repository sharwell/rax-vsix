namespace Rackspace.VisualStudio.CloudExplorer.Dns
{
    using System;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.Dns;

    public class CloudDomainNode : AsyncNode
    {
        private readonly CloudDnsProvider _provider;
        private readonly DnsDomain _domain;

        public CloudDomainNode(CloudDnsProvider provider, DnsDomain domain)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (domain == null)
                throw new ArgumentNullException("domain");

            _provider = provider;
            _domain = domain;
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
                return _domain.Name;
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
    }
}
