﻿namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System.Windows;
    using net.openstack.Core.Domain;

    public partial class AddAccountWindow : Window
    {
        private readonly AccountStore _accountStore;

        private AddAccountWindow()
        {
            InitializeComponent();
        }

        internal AddAccountWindow(AccountStore accountStore)
            : this()
        {
            _accountStore = accountStore;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HandleOkClick(object sender, RoutedEventArgs e)
        {
            if (_accountStore != null)
            {
                bool hasApiKey = chkHaveApiKey.IsChecked ?? false;
                CloudIdentity identity = new CloudIdentity
                {
                    Username = txtUsername.Text,
                    Password = hasApiKey ? null : txtPassword.Password,
                    APIKey = hasApiKey ? txtApiKey.Text : null
                };

                _accountStore.AddAccount(identity);
            }

            Close();
        }
    }
}
