using System;
using System.Globalization;
using System.Windows.Data;

namespace Firebase.Auth.UI.Converters
{
    public class AssetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"/Firebase.Auth.UI.WPF;Component{ProviderToAssetConverter.Convert((FirebaseProviderType)value)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
