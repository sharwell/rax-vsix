namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    public class AccountViewModel : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs NamePropertyChangedEventArgs = new PropertyChangedEventArgs("Name");
        private static readonly PropertyChangedEventArgs ExistsPropertyChangedEventArgs = new PropertyChangedEventArgs("Exists");

        private readonly ICommand _removeCommand;

        private readonly Account _account;
        private bool _deleted;

        public AccountViewModel(Account account)
        {
            _removeCommand = new RelayCommand(HandleRemove, HandleCanRemove);

            if (account != null)
            {
                _account = account;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get
            {
                return _account.Name;
            }
        }

        public bool Exists
        {
            get
            {
                return !_deleted;
            }

            private set
            {
                if (value == !_deleted)
                    return;

                _deleted = !value;
                OnPropertyChanged(ExistsPropertyChangedEventArgs);
            }
        }

        public ICommand RemoveCommand
        {
            get
            {
                return _removeCommand;
            }
        }

        protected virtual void HandleRemove()
        {
            if (_account == null)
                throw new InvalidOperationException("No Account model is associated with this object.");

            var confirm = MessageBox.Show(string.Format("Are you sure you want to remove the account '{0}'?", Name), "Confirm Account Removal", MessageBoxButton.YesNoCancel, MessageBoxImage.Exclamation, MessageBoxResult.No);
            if (confirm != MessageBoxResult.Yes)
                return;

            _account.AccountStore.RemoveAccount(_account);
            Exists = false;
        }

        protected virtual bool HandleCanRemove()
        {
            return Exists && _account != null;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var t = PropertyChanged;
            if (t != null)
                t(this, e);
        }
    }
}
