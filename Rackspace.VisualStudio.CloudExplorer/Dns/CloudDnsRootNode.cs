namespace Rackspace.VisualStudio.CloudExplorer.Dns
{
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class CloudDnsRootNode : CloudProductRootNode
    {
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
                    return Task.FromResult(new Node[] { new CloudDnsTenantNode(identity) });
            }
            catch (JsonSerializationException)
            {
            }

            return Task.FromResult(RackspaceProductsNode.EmptyChildren);
        }

        public override Image Icon
        {
            get
            {
                return ServerExplorerIcons.CloudDns;
            }
        }

        protected override string DisplayText
        {
            get
            {
                return "DNS";
            }
        }
    }
}
