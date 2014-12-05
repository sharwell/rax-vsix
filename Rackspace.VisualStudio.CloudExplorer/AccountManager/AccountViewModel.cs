namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System.ComponentModel;

    public class AccountViewModel : INotifyPropertyChanged
    {
        private static readonly PropertyChangedEventArgs NamePropertyChangedEventArgs = new PropertyChangedEventArgs("Name");

        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                if (_name == value)
                    return;

                _name = value;
                OnPropertyChanged(NamePropertyChangedEventArgs);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var t = PropertyChanged;
            if (t != null)
                t(this, e);
        }
    }
}
