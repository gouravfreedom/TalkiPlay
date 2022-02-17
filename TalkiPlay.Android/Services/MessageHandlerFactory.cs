using System;
using System.Net.Http;
using TalkiPlay.Shared;
using Xamarin.Android.Net;

namespace TalkiPlay
{
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        public HttpMessageHandler CreateHandler()
        {
            return new AndroidClientHandler();
        }
    }
}
