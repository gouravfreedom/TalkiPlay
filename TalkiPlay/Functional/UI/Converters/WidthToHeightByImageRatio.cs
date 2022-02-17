using System;
using System.Globalization;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace ChilliSource.Mobile.UI
{
    public class WidthToHeightByImageRatio : IValueConverter
    {
        public static double Convert(double value)
        {
            try
            {
                if (value < 0) return value;

                return value / 1.7d;
            }
            catch (Exception) { }
            return 0d;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((double)value);
        }


        public static double ConvertBack(double value)
        {
            try
            {
                if (value < 0) return value;

                return value * 1.7d;
            }
            catch (Exception) { }
            return 0d;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((double)value);
        }
    }
}

