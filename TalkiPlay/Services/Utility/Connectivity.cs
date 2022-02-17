using System;
using ChilliSource.Mobile.Api;

namespace TalkiPlay
{
    public class Connectivity : IConnectivity
    {
        public Connectivity()
        {
        }

        public bool IsConnected => Xamarin.Essentials.Connectivity.NetworkAccess == Xamarin.Essentials.NetworkAccess.Internet;
    }
}
