using System;
using Windows.UI.Xaml.Data;

namespace Firebase.Auth.UI.Converters
{
    public class AssetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return $"ms-appx:///Firebase.Auth.UI.UWP{ProviderToAssetConverter.Convert((FirebaseProviderType)value)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
