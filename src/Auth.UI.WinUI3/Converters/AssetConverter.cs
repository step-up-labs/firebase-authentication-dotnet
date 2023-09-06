using Microsoft.UI.Xaml.Data;
using System;

namespace Firebase.Auth.UI.Converters
{
    public class AssetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return $"ms-appx:///Firebase.Auth.UI.WinUI3{ProviderToAssetConverter.Convert((FirebaseProviderType)value)}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
