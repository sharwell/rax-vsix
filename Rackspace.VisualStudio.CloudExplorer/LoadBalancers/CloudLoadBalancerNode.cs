namespace Rackspace.VisualStudio.CloudExplorer.LoadBalancers
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.LoadBalancers;
    using LocalizableProperties = Microsoft.VisualStudio.Shell.LocalizableProperties;
    using Node = Microsoft.VSDesigner.ServerExplorer.Node;

    public class CloudLoadBalancerNode : AsyncNode
    {
        private readonly CloudLoadBalancerProvider _provider;
        private readonly LoadBalancer _loadBalancer;

        public CloudLoadBalancerNode(CloudLoadBalancerProvider provider, LoadBalancer loadBalancer)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (loadBalancer == null)
                throw new ArgumentNullException("loadBalancer");

            _provider = provider;
            _loadBalancer = loadBalancer;
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
                return _loadBalancer.Name;
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

        public override object GetBrowseComponent()
        {
            return new LoadBalancerProperties(_provider, _loadBalancer);
        }

        public class LoadBalancerProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudLoadBalancerProvider _provider;
            private readonly LoadBalancer _loadBalancer;

            public LoadBalancerProperties(CloudLoadBalancerProvider provider, LoadBalancer loadBalancer)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (loadBalancer == null)
                    throw new ArgumentNullException("loadBalancer");

                _provider = provider;
                _loadBalancer = loadBalancer;
            }

            [DisplayName("ID")]
            public LoadBalancerId Id
            {
                get
                {
                    return _loadBalancer.Id;
                }
            }

            [DisplayName("Name")]
            public string Name
            {
                get
                {
                    return _loadBalancer.Name;
                }
            }

            [DisplayName("Region")]
            public string Region
            {
                get
                {
                    return _provider.DefaultRegion;
                }
            }

            [DisplayName("Port")]
            public int? Port
            {
                get
                {
                    return _loadBalancer.Port;
                }
            }

            [DisplayName("Protocol Name")]
            public string ProtocolName
            {
                get
                {
                    return _loadBalancer.ProtocolName;
                }
            }

            [DisplayName("Algorithm")]
            public LoadBalancingAlgorithm Algorithm
            {
                get
                {
                    return _loadBalancer.Algorithm;
                }
            }

            [DisplayName("Cluster")]
            public string Cluster
            {
                get
                {
                    if (_loadBalancer.Cluster == null)
                        return null;

                    return _loadBalancer.Cluster.Name;
                }
            }

            [DisplayName("Connection Logging")]
            public bool? ConnectionLogging
            {
                get
                {
                    return _loadBalancer.ConnectionLogging;
                }
            }

            [DisplayName("Content Caching")]
            public bool? ContentCaching
            {
                get
                {
                    return _loadBalancer.ContentCaching;
                }
            }

            [DisplayName("Half-Closed")]
            public bool? HalfClosed
            {
                get
                {
                    return _loadBalancer.HalfClosed;
                }
            }

            [DisplayName("Session Persistence")]
            public string SessionPersistence
            {
                get
                {
                    if (_loadBalancer.SessionPersistence == null)
                        return null;

                    if (_loadBalancer.SessionPersistence.PersistenceType == null)
                        return null;

                    return _loadBalancer.SessionPersistence.PersistenceType.Name;
                }
            }

            [DisplayName("Created")]
            public DateTime? Created
            {
                get
                {
                    if (_loadBalancer.Created == null)
                        return null;

                    return _loadBalancer.Created.Value.ToLocalTime().DateTime;
                }
            }

            [DisplayName("Last Modified")]
            public DateTime? Updated
            {
                get
                {
                    if (_loadBalancer.Updated == null)
                        return null;

                    return _loadBalancer.Updated.Value.ToLocalTime().DateTime;
                }
            }

            [DisplayName("Status")]
            public string Status
            {
                get
                {
                    if (_loadBalancer.Status == null)
                        return null;

                    return _loadBalancer.Status.Name;
                }
            }

            [DisplayName("Timeout")]
            public TimeSpan? Timeout
            {
                get
                {
                    return _loadBalancer.Timeout;
                }
            }

            public override string GetClassName()
            {
                return "Load Balancer Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud Load Balancers";
            }
        }
    }
}
