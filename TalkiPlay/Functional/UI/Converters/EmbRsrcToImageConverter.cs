using System;
using System.Globalization;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace ChilliSource.Mobile.UI
{
    public class EmbRsrcToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return Images.GetEmbeddedPngImage((string)value);
            }
            catch (Exception) { }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("EmbRsrcToImageConverter.ConvertBack is not implemented yet");
        }
    }
}
