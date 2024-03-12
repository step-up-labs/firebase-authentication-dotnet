using Firebase.Auth.UI.Converters;
using System.Globalization;

namespace Firebase.Auth.UI.MAUI.Converters
{
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "#000";
            float aux = GetParameter(parameter);
            //aux
            var color = ProviderToBackgroundConverter.Convert((FirebaseProviderType)value);
            return Color.FromArgb(color).WithAlpha(aux).ToArgbHex();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        float GetParameter(object parameter)
        {
            if (parameter is float)
                return (float)parameter;
            else if (parameter is int)
                return (float)parameter;
            else if (parameter is string)
                return float.Parse((string)parameter, NumberStyles.Float, CultureInfo.InvariantCulture);

            return 1;
        }
    }
}
