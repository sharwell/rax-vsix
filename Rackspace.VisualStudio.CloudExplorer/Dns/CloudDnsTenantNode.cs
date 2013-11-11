namespace Rackspace.VisualStudio.CloudExplorer.Dns
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Core.Providers;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.Dns;

    public class CloudDnsTenantNode : AsyncNode
    {
        private readonly CloudIdentity _identity;
        private string _tenantId;

        public CloudDnsTenantNode(CloudIdentity identity)
        {
            _identity = identity;

            IIdentityService identityService = new CloudIdentityProvider();
            identityService.GetTokenAsync(identity, CancellationToken.None).ContinueWith(
                task =>
                {
                    _tenantId = task.Result.Tenant.Id;
                    INodeSite nodeSite = GetNodeSite();
                    if (nodeSite != null)
                        nodeSite.UpdateLabel();
                });
        }

        public override int CompareUnique(Node node)
        {
            return Label.CompareTo(node.Label);
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            Tuple<CloudDnsProvider, DnsDomain[]> domains = await ListDomainsAsync(cancellationToken);
            return Array.ConvertAll(domains.Item2, i => CreateDomainNode(domains.Item1, i));
        }

        private CloudDomainNode CreateDomainNode(CloudDnsProvider provider, DnsDomain domain)
        {
            return new CloudDomainNode(provider, domain);
        }

        private async Task<Tuple<CloudDnsProvider, DnsDomain[]>> ListDomainsAsync(CancellationToken cancellationToken)
        {
            CloudDnsProvider provider = new CloudDnsProvider(_identity, null, false, null);
            List<DnsDomain> domains = new List<DnsDomain>();
            domains.AddRange((await provider.ListDomainsAsync(null, null, null, cancellationToken)).Item1);
            return Tuple.Create(provider, domains.ToArray());
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
                if (!string.IsNullOrEmpty(_identity.Username) && !string.IsNullOrEmpty(_tenantId))
                    return string.Format("{0} ({1})", _identity.Username, _tenantId);
                else if (!string.IsNullOrEmpty(_identity.Username))
                    return _identity.Username;

                return _tenantId ?? string.Empty;
            }
        }

        public override bool CanDeleteNode()
        {
            return false;
        }
    }
}
