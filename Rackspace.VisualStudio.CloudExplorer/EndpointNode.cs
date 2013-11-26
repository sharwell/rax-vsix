namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using Image = System.Drawing.Image;

    public abstract class EndpointNode : AsyncNode
    {
        private readonly CloudIdentity _identity;
        private readonly Endpoint _endpoint;

        public EndpointNode(CloudIdentity identity, Endpoint endpoint)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            _identity = identity;
            _endpoint = endpoint;
        }

        public CloudIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public Endpoint Endpoint
        {
            get
            {
                return _endpoint;
            }
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

        protected override string DisplayText
        {
            get
            {
                if (!string.IsNullOrEmpty(_endpoint.Region))
                    return _endpoint.Region;

                return "Global";
            }
        }

        public override bool CanDeleteNode()
        {
            return false;
        }
    }
}
