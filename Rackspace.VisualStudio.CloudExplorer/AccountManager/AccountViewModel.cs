namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System.ComponentModel;

    public class AccountViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var t = PropertyChanged;
            if (t != null)
                t(this, e);
        }
    }
}
