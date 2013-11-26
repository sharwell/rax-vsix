namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.ComponentModel;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;
    using Image = System.Drawing.Image;

    public abstract class EndpointNode : AsyncNode
    {
        private readonly CloudIdentity _identity;
        private readonly ServiceCatalog _serviceCatalog;
        private readonly Endpoint _endpoint;

        public EndpointNode(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");
            if (serviceCatalog == null)
                throw new ArgumentNullException("serviceCatalog");
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            _identity = identity;
            _serviceCatalog = serviceCatalog;
            _endpoint = endpoint;
        }

        public CloudIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public ServiceCatalog ServiceCatalog
        {
            get
            {
                return _serviceCatalog;
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

        public override object GetBrowseComponent()
        {
            return new EndpointProperties(_identity, _serviceCatalog, _endpoint);
        }

        protected class EndpointProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudIdentity _identity;
            private readonly ServiceCatalog _serviceCatalog;
            private readonly Endpoint _endpoint;

            public EndpointProperties(CloudIdentity identity, ServiceCatalog serviceCatalog, Endpoint endpoint)
            {
                if (identity == null)
                    throw new ArgumentNullException("identity");
                if (serviceCatalog == null)
                    throw new ArgumentNullException("serviceCatalog");
                if (endpoint == null)
                    throw new ArgumentNullException("endpoint");

                _identity = identity;
                _serviceCatalog = serviceCatalog;
                _endpoint = endpoint;
            }

            public CloudIdentity Identity
            {
                get
                {
                    return _identity;
                }
            }

            public ServiceCatalog ServiceCatalog
            {
                get
                {
                    return _serviceCatalog;
                }
            }

            public Endpoint Endpoint
            {
                get
                {
                    return _endpoint;
                }
            }

            [DisplayName("Public URL")]
            [Category(PropertyCategories.Identity)]
            public string PublicURL
            {
                get
                {
                    return _endpoint.PublicURL;
                }
            }

            [DisplayName("Internal URL")]
            [Category(PropertyCategories.Identity)]
            public string InternalURL
            {
                get
                {
                    return _endpoint.InternalURL;
                }
            }

            [DisplayName("Region")]
            [Category(PropertyCategories.Identity)]
            public string Region
            {
                get
                {
                    return _endpoint.Region;
                }
            }

            [DisplayName("Tenant")]
            [Category(PropertyCategories.Identity)]
            public string TenantId
            {
                get
                {
                    return _endpoint.TenantId;
                }
            }

            [DisplayName("Version ID")]
            [Category(PropertyCategories.Advanced)]
            public string VersionId
            {
                get
                {
                    return _endpoint.VersionId;
                }
            }

            [DisplayName("Version Info")]
            [Category(PropertyCategories.Advanced)]
            public string VersionInfo
            {
                get
                {
                    return _endpoint.VersionInfo;
                }
            }

            [DisplayName("Version List")]
            [Category(PropertyCategories.Advanced)]
            public string VersionList
            {
                get
                {
                    return _endpoint.VersionList;
                }
            }

            public override string GetClassName()
            {
                return "Endpoint Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return string.Format("{0} ({1})", _serviceCatalog.Name, _serviceCatalog.Type);
            }
        }
    }
}
