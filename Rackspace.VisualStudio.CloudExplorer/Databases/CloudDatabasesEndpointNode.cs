namespace Rackspace.VisualStudio.CloudExplorer.Databases
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.Databases;

    public class CloudDatabasesEndpointNode : EndpointNode
    {
        public CloudDatabasesEndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            : base(identity, serviceCatalog, endpoint)
        {
        }

        protected override async Task<Node[]> CreateChildrenAsync(CancellationToken cancellationToken)
        {
            Tuple<CloudDatabasesProvider, DatabaseInstance[]> databaseInstances = await ListDatabaseInstancesAsync(cancellationToken);
            List<Node> results = new List<Node>();
            foreach (DatabaseInstance instance in databaseInstances.Item2)
                results.Add(await CreateDatabaseInstanceNodeAsync(databaseInstances.Item1, instance, cancellationToken).ConfigureAwait(false));

            return results.ToArray();
        }

        private Task<Node> CreateDatabaseInstanceNodeAsync(CloudDatabasesProvider provider, DatabaseInstance instance, CancellationToken cancellationToken)
        {
            return Task.FromResult<Node>(new CloudDatabasesInstanceNode(provider, instance));
        }

        private async Task<Tuple<CloudDatabasesProvider, DatabaseInstance[]>> ListDatabaseInstancesAsync(CancellationToken cancellationToken)
        {
            CloudDatabasesProvider provider = CreateProvider();
            List<DatabaseInstance> instances = new List<DatabaseInstance>();
            instances.AddRange(await provider.ListDatabaseInstancesAsync(null, null, cancellationToken).ConfigureAwait(false));

            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i].HostName == null)
                    instances[i] = await provider.GetDatabaseInstanceAsync(instances[i].Id, cancellationToken);
            }

            return Tuple.Create(provider, instances.ToArray());
        }

        private CloudDatabasesProvider CreateProvider()
        {
            return new CloudDatabasesProvider(Identity, Endpoint.Region, null);
        }
    }
}
