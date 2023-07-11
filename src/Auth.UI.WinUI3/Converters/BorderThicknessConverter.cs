using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace Firebase.Auth.UI.Converters
{
    public class BorderThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new Thickness((FirebaseProviderType)value == FirebaseProviderType.Google ? 1 : 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
