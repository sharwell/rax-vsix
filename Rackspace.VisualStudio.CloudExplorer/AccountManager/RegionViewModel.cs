namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System.ComponentModel;

    public class RegionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            var t = PropertyChanged;
            if (t != null)
                t(this, e);
        }

        public string Name
        {
            get;
            set;
        }
    }
}
