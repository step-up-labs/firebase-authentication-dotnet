using Firebase.Auth.UI.Converters;
using System.Globalization;

namespace Firebase.Auth.UI.MAUI.Converters
{
    public class ForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "#fff";
            return ProviderToForegroundConverter.Convert((FirebaseProviderType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
