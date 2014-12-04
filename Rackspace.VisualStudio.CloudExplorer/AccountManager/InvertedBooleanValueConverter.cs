namespace Rackspace.VisualStudio.CloudExplorer.AccountManager
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class InvertedBooleanValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false.Equals(value);
        }
    }
}
