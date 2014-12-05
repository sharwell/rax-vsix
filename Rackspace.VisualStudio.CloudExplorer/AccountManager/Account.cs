namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using net.openstack.Core.Domain;
    using OpenStack.Services.Identity.V2;
    using Rackspace.Security.Authentication;

    public class Account
    {
        private readonly AccountStore _accountStore;
        private readonly string _id;
        private readonly string _username;
        private readonly string _apiKey;

        internal Account(AccountStore accountStore, string id, string username, string apiKey)
        {
            _accountStore = accountStore;
            _id = id;
            _username = username;
            _apiKey = apiKey;
        }

        internal AccountStore AccountStore
        {
            get
            {
                return _accountStore;
            }
        }

        internal string Id
        {
            get
            {
                return _id;
            }
        }

        public string Name
        {
            get
            {
                return _username;
            }
        }

        public CloudIdentity ToCloudIdentity()
        {
            return new CloudIdentity
            {
                Username = _username,
                APIKey = _apiKey
            };
        }

        public AuthenticationRequest ToAuthenticationRequest()
        {
            return RackspaceAuthentication.ApiKey(_username, _apiKey);
        }
    }
}
