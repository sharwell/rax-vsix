namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.ComponentModel;
    using Microsoft.VisualStudio.Shell;
    using OpenStack.Security.Authentication;
    using OpenStack.Services.Identity.V2;
    using Image = System.Drawing.Image;

    public abstract class EndpointNodeV2 : AsyncNode
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ServiceCatalogEntry _serviceCatalogEntry;
        private readonly Endpoint _endpoint;

        public EndpointNodeV2(IAuthenticationService authenticationService, ServiceCatalogEntry serviceCatalogEntry, Endpoint endpoint)
        {
            if (authenticationService == null)
                throw new ArgumentNullException("authenticationService");
            if (serviceCatalogEntry == null)
                throw new ArgumentNullException("serviceCatalogEntry");
            if (endpoint == null)
                throw new ArgumentNullException("endpoint");

            _authenticationService = authenticationService;
            _serviceCatalogEntry = serviceCatalogEntry;
            _endpoint = endpoint;
        }

        public IAuthenticationService AuthenticationService
        {
            get
            {
                return _authenticationService;
            }
        }

        protected ServiceCatalogEntry ServiceCatalogEntry
        {
            get
            {
                return _serviceCatalogEntry;
            }
        }

        protected Endpoint Endpoint
        {
            get
            {
                return _endpoint;
            }
        }

        public override Image Icon
        {
            get
            {
                return ServerExplorerIcons.Region;
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
            return new EndpointProperties(_authenticationService, _serviceCatalogEntry, _endpoint);
        }

        protected class EndpointProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly IAuthenticationService _authenticationService;
            private readonly ServiceCatalogEntry _serviceCatalogEntry;
            private readonly Endpoint _endpoint;

            public EndpointProperties(IAuthenticationService authenticationService, ServiceCatalogEntry serviceCatalogEntry, Endpoint endpoint)
            {
                if (authenticationService == null)
                    throw new ArgumentNullException("authenticationService");
                if (serviceCatalogEntry == null)
                    throw new ArgumentNullException("serviceCatalogEntry");
                if (endpoint == null)
                    throw new ArgumentNullException("endpoint");

                _authenticationService = authenticationService;
                _serviceCatalogEntry = serviceCatalogEntry;
                _endpoint = endpoint;
            }

            [Browsable(false)]
            protected IAuthenticationService AuthenticationService
            {
                get
                {
                    return _authenticationService;
                }
            }

            [Browsable(false)]
            protected ServiceCatalogEntry ServiceCatalogEntry
            {
                get
                {
                    return _serviceCatalogEntry;
                }
            }

            [Browsable(false)]
            protected Endpoint Endpoint
            {
                get
                {
                    return _endpoint;
                }
            }

            [DisplayName("Public URL")]
            [Category(PropertyCategories.Identity)]
            public Uri PublicURL
            {
                get
                {
                    return _endpoint.PublicUrl;
                }
            }

            [DisplayName("Internal URL")]
            [Category(PropertyCategories.Identity)]
            public Uri InternalURL
            {
                get
                {
                    return _endpoint.InternalUrl;
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

#if false
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
#endif

            public override string GetClassName()
            {
                return "Endpoint Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return string.Format("{0} ({1})", _serviceCatalogEntry.Name, _serviceCatalogEntry.Type);
            }
        }
    }
}
