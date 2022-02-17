using CodeHollow.FeedReader;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TalkiPlay.Services.Utility
{
    public class RssImageFetcher
    {   
        public class RssFeedInfo
        {
            public string Title { get; set; }
            public string Link { get; set; }
            public string Image { get; set; }
        }

        public static readonly string[] DeviceRssUrls = new string [] {
                "https://www.talkiplay.com/articles?tag=Educator&format=rss",
                "https://www.talkiplay.com/articles?tag=all&format=rss" };

        public static readonly string[] QrRssUrls = new string[] {
                "https://www.talkiplay.com/articles?tag=Parent&format=rss",
                "https://www.talkiplay.com/articles?tag=all&format=rss" };


        public static void FetchDeviceImages(Action<List<RssFeedInfo>> fetchedAction, Action doneAction)
        {
            FetchImagesFromRssFeed(DeviceRssUrls, fetchedAction, doneAction);
        }

        public static void FetchQRCodeImages(Action<List<RssFeedInfo>> fetchedAction, Action doneAction)
        {
            FetchImagesFromRssFeed(QrRssUrls, fetchedAction, doneAction);
        }

        public static void FetchImagesFromRssFeed(string[] feedUrls, Action<List<RssFeedInfo>> fetchedAction, Action doneAction)
        {
            foreach(var feedUrl in feedUrls)
            {
                Observable.FromAsync(async () => await FetchRssImage(feedUrl))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(m =>
                {
                    fetchedAction?.Invoke(m);
                    doneAction?.Invoke();
                })
                .Subscribe();
            }
        }

        private static async Task<List<RssFeedInfo>> FetchRssImage(string feedUrl)
        {
            var urls = new List<RssFeedInfo>();
            try
            {
                var feed = await FeedReader.ReadAsync(feedUrl);

                Console.WriteLine("Feed Title: " + feed.Title);
                Console.WriteLine("Feed Description: " + feed.Description);
                Console.WriteLine("Feed Image: " + feed.ImageUrl);
                // ...
                foreach (var item in feed.Items)
                {
                    Console.WriteLine(item.Title + " - " + item.Link);
                    var idxImg = item.Content.IndexOf("<img");
                    if (idxImg < 0) continue;
                    var startTag = "src=\"";
                    int idxStart = item.Content.IndexOf(startTag, idxImg);
                    if (idxStart < 0) continue;
                    String endTag = "\"";
                    var idxEnd = item.Content.IndexOf(endTag, idxStart + startTag.Length, StringComparison.CurrentCultureIgnoreCase);
                    if (idxEnd < 0) continue;
                    var url = item.Content.Substring(idxStart + startTag.Length, idxEnd - idxStart - startTag.Length - 1);
                    System.Console.WriteLine($"Image: {url}");
                    urls.Add(new RssFeedInfo()
                    {
                        Title = item.Title,
                        Link = item.Link,
                        Image = url
                    });
                }
            }
            catch { }
            return urls;
        }
    }
}
