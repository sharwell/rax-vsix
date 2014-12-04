namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System.Windows;

    public partial class ManageSubscriptionsWindow : Window
    {
        private readonly AccountStore _accountStore;

        private ManageSubscriptionsWindow()
        {
            InitializeComponent();
        }

        internal ManageSubscriptionsWindow(AccountStore accountStore)
            : this()
        {
            _accountStore = accountStore;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            AddAccountWindow addAccountWindow = new AddAccountWindow(_accountStore);
            addAccountWindow.Owner = this;
            addAccountWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addAccountWindow.ShowDialog();
        }
    }
}
