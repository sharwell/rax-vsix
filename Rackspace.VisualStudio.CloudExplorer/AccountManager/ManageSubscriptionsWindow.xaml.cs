namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System;
    using System.Windows;

    public partial class ManageSubscriptionsWindow : Window
    {
        public ManageSubscriptionsWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            AddAccountWindow addAccountWindow = new AddAccountWindow();
            addAccountWindow.Owner = this;
            addAccountWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            addAccountWindow.ShowDialog();
        }
    }
}
