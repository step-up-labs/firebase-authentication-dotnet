using System;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Firebase.Auth.UI.Pages
{
    internal class Styles
    {
        private static Styles defaults;

        public Styles(Style titleStyle, Style headerStyle, Style errorStyle, Style bodyStyle, Style confirmButtonStyle, Style cancelButtonStyle)
        {
            this.TitleStyle = titleStyle;
            this.HeaderStyle = headerStyle;
            this.ErrorStyle = errorStyle;
            this.BodyStyle = bodyStyle;
            this.ConfirmButtonStyle = confirmButtonStyle;
            this.CancelButtonStyle = cancelButtonStyle;
        }

        public Style TitleStyle { get; }

        public Style HeaderStyle { get; }

        public Style ErrorStyle { get; }

        public Style BodyStyle { get; }

        public Style ConfirmButtonStyle { get; }

        public Style CancelButtonStyle { get; }

        public static Styles Default => defaults ?? (defaults = new Styles(
            LoadStyle("TitleTextBlockStyle"),
            LoadStyle("BaseTextBlockStyle"),
            LoadStyle("CaptionTextBlockStyle", s => s.Setters.Add(new Setter(TextBlock.ForegroundProperty, new SolidColorBrush(Colors.Red)))),
            LoadStyle("BodyTextBlockStyle"),
            LoadStyle("AccentButtonStyle"),
            null
            ));

        public static Style LoadStyle(string name, Action<Style> modifier = null)
        {
            var style = (Style)Application.Current.Resources[name];
            
            if (modifier != null)
            {
                // clone the original style into new one and apply the modifier
                var original = style;
                style = new Style(style.TargetType);
                original.Setters.ToList().ForEach(style.Setters.Add);
                modifier(style);
            }

            return style;
        }
    }
}
