using System;
using System.Net.Http;

namespace TalkiPlay.Shared
{
    public interface IMessageHandlerFactory
    {
        HttpMessageHandler CreateHandler();
    }
}
