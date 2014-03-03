namespace Rackspace.VisualStudio.CloudExplorer
{
    using System;
    using System.ComponentModel;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core.Domain;

    public abstract class CloudProductRootNode : AsyncNode
    {
        private readonly ServiceCatalog _serviceCatalog;

        protected CloudProductRootNode(ServiceCatalog serviceCatalog)
        {
            _serviceCatalog = serviceCatalog;
        }

        public ServiceCatalog ServiceCatalog
        {
            get
            {
                return _serviceCatalog;
            }
        }

        public override bool CanDeleteNode()
        {
            return false;
        }

        public override object GetBrowseComponent()
        {
            if (_serviceCatalog != null)
                return new CloudProductProperties(_serviceCatalog);

            return base.GetBrowseComponent();
        }

        protected class CloudProductProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly ServiceCatalog _serviceCatalog;

            public CloudProductProperties(ServiceCatalog serviceCatalog)
            {
                if (serviceCatalog == null)
                    throw new ArgumentNullException("serviceCatalog");

                _serviceCatalog = serviceCatalog;
            }

            [Browsable(false)]
            public ServiceCatalog ServiceCatalog
            {
                get
                {
                    return _serviceCatalog;
                }
            }

            [DisplayName("Service Name")]
            [Category(PropertyCategories.Identity)]
            public string ServiceName
            {
                get
                {
                    return _serviceCatalog.Name;
                }
            }

            [DisplayName("Service Type")]
            [Category(PropertyCategories.Identity)]
            public string ServiceType
            {
                get
                {
                    return _serviceCatalog.Type;
                }
            }

            public override string GetClassName()
            {
                return "Service Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return string.Format("{0} ({1})", _serviceCatalog.Name, _serviceCatalog.Type);
            }
        }
    }
}
