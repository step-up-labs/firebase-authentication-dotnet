using Firebase.Auth.UI.Converters;
using System.Globalization;

namespace Firebase.Auth.UI.MAUI.Converters
{
    public class TitleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return ProviderToTitleConverter.Convert((FirebaseProviderType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
