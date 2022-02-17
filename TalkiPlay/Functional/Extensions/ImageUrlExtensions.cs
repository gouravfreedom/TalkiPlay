using System;

namespace TalkiPlay.Shared
{
    public static class ImageUrlExtensions
    {
        public static string ToResizedImage(this string url, double? width = null, double? height = null)
        {
            if (!String.IsNullOrWhiteSpace(url) && (url.Contains("https") || url.Contains("http")))
            {
                var w = width != null ? $"&w={width}" : "";
                var h = height != null ? $"&h={height}" : "";
                return String.IsNullOrWhiteSpace(url) ? url : $"{url}?{Constants.ImageResizerParameter}{w}{h}";
            }

            return url;
        }
    }
}