namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using net.openstack.Core.Domain;

    [Export]
    internal sealed class AccountStore
    {
        private static readonly string AccountsCollectionPath = @"CloudExplorer\AccountManager\Accounts";
        private readonly SVsServiceProvider _serviceProvider;

        [ImportingConstructor]
        public AccountStore(SVsServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<Account> Credentials
        {
            get
            {
                IVsSettingsStore settingsStore = GetSettingsStore();

                int exists;
                ErrorHandler.ThrowOnFailure(settingsStore.CollectionExists(AccountsCollectionPath, out exists));
                if (exists == 0)
                    yield break;

                uint accountCount;
                ErrorHandler.ThrowOnFailure(settingsStore.GetSubCollectionCount(AccountsCollectionPath, out accountCount));
                for (uint i = 0; i < accountCount; i++)
                {
                    string accountName;
                    ErrorHandler.ThrowOnFailure(settingsStore.GetSubCollectionName(AccountsCollectionPath, i, out accountName));

                    string collectionPath = string.Format(@"{0}\{1}", AccountsCollectionPath, accountName);

                    string username;
                    string apiKey;
                    ErrorHandler.ThrowOnFailure(settingsStore.GetStringOrDefault(collectionPath, "username", string.Empty, out username));
                    ErrorHandler.ThrowOnFailure(settingsStore.GetStringOrDefault(collectionPath, "apiKey", string.Empty, out apiKey));

                    yield return new Account(this, accountName, username, apiKey);
                }
            }
        }

        public void AddAccount(CloudIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException("identity");
            if (string.IsNullOrEmpty(identity.Username))
                throw new ArgumentException("identity.Username cannot be empty");
            if (identity.Username.IndexOf('\\') > 0)
                throw new NotSupportedException("Username cannot contain a '\\' character.");

            IVsWritableSettingsStore settingsStore = GetWritableSettingsStore();

            string collectionPath = string.Format(@"{0}\{1}", AccountsCollectionPath, identity.Username);
            ErrorHandler.ThrowOnFailure(settingsStore.CreateCollection(collectionPath));
            ErrorHandler.ThrowOnFailure(settingsStore.SetString(collectionPath, "username", identity.Username));
            ErrorHandler.ThrowOnFailure(settingsStore.SetString(collectionPath, "apiKey", identity.APIKey ?? string.Empty));
        }

        public void RemoveAccount(Account account)
        {
            if (account == null)
                throw new ArgumentNullException("account");

            IVsWritableSettingsStore settingsStore = GetWritableSettingsStore();

            string collectionPath = string.Format(@"{0}\{1}", AccountsCollectionPath, account.Id);

            int exists;
            ErrorHandler.ThrowOnFailure(settingsStore.CollectionExists(collectionPath, out exists));
            if (exists == 0)
                return;

            ErrorHandler.ThrowOnFailure(settingsStore.DeleteCollection(collectionPath));
        }

        private IVsSettingsStore GetSettingsStore()
        {
            IVsSettingsManager settingsManager = (IVsSettingsManager)_serviceProvider.GetService(typeof(SVsSettingsManager));
            IVsSettingsStore readOnlyStore;
            ErrorHandler.ThrowOnFailure(settingsManager.GetReadOnlySettingsStore((int)__VsSettingsScope.SettingsScope_UserSettings, out readOnlyStore));
            return readOnlyStore;
        }

        private IVsWritableSettingsStore GetWritableSettingsStore()
        {
            IVsSettingsManager settingsManager = (IVsSettingsManager)_serviceProvider.GetService(typeof(SVsSettingsManager));
            IVsWritableSettingsStore writableStore;
            ErrorHandler.ThrowOnFailure(settingsManager.GetWritableSettingsStore((int)__VsSettingsScope.SettingsScope_UserSettings, out writableStore));
            return writableStore;
        }
    }
}
