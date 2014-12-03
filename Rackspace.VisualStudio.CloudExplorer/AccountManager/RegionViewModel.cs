namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System.ComponentModel;

    public class RegionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public string Name
        {
            get;
            set;
        }
    }
}
