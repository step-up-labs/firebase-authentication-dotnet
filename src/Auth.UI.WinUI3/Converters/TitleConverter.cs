using Microsoft.UI.Xaml.Data;
using System;

namespace Firebase.Auth.UI.Converters
{
    public class TitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ProviderToTitleConverter.Convert((FirebaseProviderType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
