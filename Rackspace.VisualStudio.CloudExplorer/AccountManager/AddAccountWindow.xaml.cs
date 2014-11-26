namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System;
    using System.Windows;

    public partial class AddAccountWindow : Window
    {
        public AddAccountWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
