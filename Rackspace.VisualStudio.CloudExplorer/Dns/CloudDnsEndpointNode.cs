namespace Rackspace.VisualStudio.CloudExplorer.Dns
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.Dns;

    public class CloudDnsEndpointNode : EndpointNode
    {
        public CloudDnsEndpointNode(CloudIdentity identity, Endpoint endpoint)
            : base(identity, endpoint)
        {
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
            CloudDnsProvider provider = CreateProvider();
            List<DnsDomain> domains = new List<DnsDomain>();
            domains.AddRange((await provider.ListDomainsAsync(null, null, null, cancellationToken)).Item1);
            return Tuple.Create(provider, domains.ToArray());
        }

        private CloudDnsProvider CreateProvider()
        {
            return new CloudDnsProvider(Identity, Endpoint.Region, false, null);
        }
    }
}
