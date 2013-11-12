namespace Rackspace.VisualStudio.CloudExplorer.Dns
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Microsoft.VisualStudio;
    using Microsoft.VSDesigner.ServerExplorer;
    using net.openstack.Core;
    using net.openstack.Providers.Rackspace;
    using net.openstack.Providers.Rackspace.Objects.Dns;
    using LocalizableProperties = Microsoft.VisualStudio.Shell.LocalizableProperties;

    public class CloudDomainNode : AsyncNode
    {
        private readonly CloudDnsProvider _provider;
        private readonly DnsDomain _domain;

        private bool _deleting;

        public CloudDomainNode(CloudDnsProvider provider, DnsDomain domain)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            if (domain == null)
                throw new ArgumentNullException("domain");

            _provider = provider;
            _domain = domain;
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
                if (_deleting)
                    return string.Format("{0} (Deleting...)", _domain.Name);

                return _domain.Name;
            }
        }

        public override bool CanDeleteNode()
        {
            return !_deleting;
        }

        public override bool ConfirmDeletingNode()
        {
            string message = string.Format("Are you sure you want to delete the domain \"{0}\"?", _domain.Name);
            INodeSite nodeSite = GetNodeSite();
            if (nodeSite.ShowMessageBox(message, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return false;

            try
            {
                _deleting = true;
                nodeSite.UpdateLabel();
                using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(60)))
                {
                    _provider.RemoveDomainsAsync(new[] { _domain.Id }, false, AsyncCompletionOption.RequestCompleted, cancellationTokenSource.Token, null).Wait();
                }

                return true;
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;

                nodeSite.ShowMessageBox(string.Format("Could not delete domain: {0}", ex.Message), MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
            finally
            {
                _deleting = false;
            }
        }

        public override object GetBrowseComponent()
        {
            return new DomainProperties(_provider, _domain);
        }

        private class DomainProperties : LocalizableProperties, ICustomTypeDescriptor
        {
            private readonly CloudDnsProvider _provider;
            private readonly DnsDomain _domain;

            public DomainProperties(CloudDnsProvider provider, DnsDomain domain)
            {
                if (provider == null)
                    throw new ArgumentNullException("provider");
                if (domain == null)
                    throw new ArgumentNullException("domain");

                _provider = provider;
                _domain = domain;
            }

            [DisplayName("ID")]
            [Category(PropertyCategories.Identity)]
            public DomainId Id
            {
                get
                {
                    return _domain.Id;
                }
            }

            [DisplayName("Name")]
            [Category(PropertyCategories.Identity)]
            public string Name
            {
                get
                {
                    return _domain.Name;
                }
            }

            [DisplayName("Account ID")]
            public string AccountId
            {
                get
                {
                    return _domain.AccountId;
                }
            }

            [DisplayName("Email Address")]
            public string EmailAddress
            {
                get
                {
                    return _domain.EmailAddress;
                }
            }

            [DisplayName("Comment")]
            public string Comment
            {
                get
                {
                    return _domain.Comment;
                }
            }

            [DisplayName("Time-To-Live")]
            public TimeSpan? TimeToLive
            {
                get
                {
                    return _domain.TimeToLive;
                }
            }

            [DisplayName("Created")]
            public DateTime? Created
            {
                get
                {
                    if (_domain.Created == null)
                        return null;

                    return _domain.Created.Value.ToLocalTime().DateTime;
                }
            }

            [DisplayName("Last Modified")]
            public DateTime? Updated
            {
                get
                {
                    if (_domain.Updated == null)
                        return null;

                    return _domain.Updated.Value.ToLocalTime().DateTime;
                }
            }

            public override string GetClassName()
            {
                return "Domain Properties";
            }

            string ICustomTypeDescriptor.GetComponentName()
            {
                return "Cloud DNS";
            }
        }
    }
}
