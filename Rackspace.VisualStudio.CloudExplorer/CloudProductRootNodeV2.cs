namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.ComponentModel;
    using Microsoft.VisualStudio.Shell;
    using OpenStack.Security.Authentication;
    using OpenStack.Services.Identity.V2;

    public abstract class CloudProductRootNodeV2 : AsyncNode
    {
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// This is currently required because the authentication service does not support listing regions.
        /// </summary>
        private readonly ServiceCatalogEntry _serviceCatalogEntry;

        protected CloudProductRootNodeV2(IAuthenticationService authenticationService, ServiceCatalogEntry serviceCatalogEntry)
        {
            _authenticationService = authenticationService;
            _serviceCatalogEntry = serviceCatalogEntry;
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

        public override bool CanDeleteNode()
        {
            return false;
        }

        public override object GetBrowseComponent()
        {
            if (_serviceCatalogEntry != null)
                return new CloudProductProperties(_serviceCatalogEntry);

            return base.GetBrowseComponent();
        }

        protected class CloudProductProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly ServiceCatalogEntry _serviceCatalogEntry;

            public CloudProductProperties(ServiceCatalogEntry serviceCatalogEntry)
            {
                if (serviceCatalogEntry == null)
                    throw new ArgumentNullException("serviceCatalogEntry");

                _serviceCatalogEntry = serviceCatalogEntry;
            }

            [Browsable(false)]
            public ServiceCatalogEntry ServiceCatalogEntry
            {
                get
                {
                    return _serviceCatalogEntry;
                }
            }

            [DisplayName("Service Name")]
            [Category(PropertyCategories.Identity)]
            public string ServiceName
            {
                get
                {
                    return _serviceCatalogEntry.Name;
                }
            }

            [DisplayName("Service Type")]
            [Category(PropertyCategories.Identity)]
            public string ServiceType
            {
                get
                {
                    return _serviceCatalogEntry.Type;
                }
            }

            public override string GetClassName()
            {
                return "Service Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return string.Format("{0} ({1})", _serviceCatalogEntry.Name, _serviceCatalogEntry.Type);
            }
        }
    }
}
