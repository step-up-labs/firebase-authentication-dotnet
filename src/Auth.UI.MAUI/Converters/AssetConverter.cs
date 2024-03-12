using System.Globalization;

namespace Firebase.Auth.UI.MAUI.Converters
{
    public class AssetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            switch ((FirebaseProviderType)value)
            {
                case FirebaseProviderType.EmailAndPassword:
                    return "mail.png";
                default:
                    return $"{value.ToString().ToLower()}.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
