using System;
using System.Globalization;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace ChilliSource.Mobile.UI
{
    public class ChildToAvatarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string filePath = null;
            try
            {
                if (value is ChildDto child)
                {
                    filePath = child.UIAssetPath;

                    if (string.IsNullOrWhiteSpace(filePath))
                    {
                        filePath = child.PhotoPath;

                        if (string.IsNullOrEmpty(filePath) && child.AssetId != null)
                        {
                            var assetRepo = Locator.Current.GetService<IAssetRepository>();

                            Task.Run(async () =>
                            {
                                var asset = await assetRepo?.GetAssetById(child.AssetId.Value);
                                if (asset != null && !string.IsNullOrEmpty(asset.ImageContentPath))
                                {
                                    filePath = asset.ImageContentPath;
                                }

                            }).Wait();
                        }

                        child.UIAssetPath = filePath;
                    }
                }
            }
            catch (Exception) { }
            return filePath ?? Images.AvatarPlaceHolder;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ChildToAvatarConverter ConvertBack Not Implemented");
        }
    }
}


