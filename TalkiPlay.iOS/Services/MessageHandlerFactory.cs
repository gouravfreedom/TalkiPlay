using System.Net.Http;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay
{
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        public HttpMessageHandler CreateHandler()
        {
            return new NSUrlSessionHandler();
        }
    }
}
