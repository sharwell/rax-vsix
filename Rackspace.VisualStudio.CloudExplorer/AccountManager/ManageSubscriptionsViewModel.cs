namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;

    public class ManageSubscriptionsViewModel : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs AccountsPropertyChangedEventArgs = new PropertyChangedEventArgs("Accounts");
        private static readonly PropertyChangedEventArgs RegionsPropertyChangedEventArgs = new PropertyChangedEventArgs("Regions");

        private readonly AccountStore _accountStore;
        private readonly List<RegionViewModel> _regions;

        private ManageSubscriptionsViewModel()
        {
            _regions = new List<RegionViewModel>();
            _regions.Add(new RegionViewModel { Name = "Dallas (DFW)" });
            _regions.Add(new RegionViewModel { Name = "Hong Kong (HKG)" });
            _regions.Add(new RegionViewModel { Name = "Northern Virginia (IAD)" });
            _regions.Add(new RegionViewModel { Name = "Chicago (ORD)" });
            _regions.Add(new RegionViewModel { Name = "Sydney (SYD)" });
            _regions.Add(new RegionViewModel { Name = "Global" });
        }

        internal ManageSubscriptionsViewModel(AccountStore accountStore)
            : this()
        {
            _accountStore = accountStore;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ReadOnlyCollection<AccountViewModel> Accounts
        {
            get
            {
                List<AccountViewModel> accounts = new List<AccountViewModel>();
                foreach (var credentials in _accountStore.Credentials)
                {
                    accounts.Add(new AccountViewModel { Name = credentials.Username });
                }

                return accounts.AsReadOnly();
            }
        }

        public ReadOnlyCollection<RegionViewModel> Regions
        {
            get
            {
                return _regions.AsReadOnly();
            }
        }

        internal void AddAccount(ManageSubscriptionsWindow window)
        {
            AddAccountWindow addAccountWindow = new AddAccountWindow(_accountStore);
            addAccountWindow.Owner = window;
            addAccountWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            if (addAccountWindow.ShowDialog() ?? false)
                OnPropertyChanged(AccountsPropertyChangedEventArgs);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var t = PropertyChanged;
            if (t != null)
                t(this, e);
        }
    }
}
