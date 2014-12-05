namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System.Windows;

    public partial class ManageSubscriptionsWindow : Window
    {
        private ManageSubscriptionsWindow()
        {
            InitializeComponent();
        }

        internal ManageSubscriptionsWindow(ManageSubscriptionsViewModel dataContext)
            : this()
        {
            DataContext = dataContext;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            ManageSubscriptionsViewModel dataContext = DataContext as ManageSubscriptionsViewModel;
            if (dataContext != null)
                dataContext.AddAccount(this);
        }
    }
}
