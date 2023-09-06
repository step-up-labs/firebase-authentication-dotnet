using Microsoft.UI.Xaml.Data;
using System;

namespace Firebase.Auth.UI.Converters
{
    public class ForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ProviderToForegroundConverter.Convert((FirebaseProviderType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
