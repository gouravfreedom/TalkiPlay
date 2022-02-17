using System;
using System.Globalization;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class IsValidToColorConverter : IValueConverter
    {
        public IsValidToColorConverter()
        {

        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isValid)
            {
                return !isValid ? Colors.Red : Color.Transparent;
            }
            return Color.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
