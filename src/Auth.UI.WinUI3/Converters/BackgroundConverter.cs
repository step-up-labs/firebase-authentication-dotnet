using Microsoft.UI.Xaml.Data;
using System;

namespace Firebase.Auth.UI.Converters
{
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ProviderToBackgroundConverter.Convert((FirebaseProviderType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
