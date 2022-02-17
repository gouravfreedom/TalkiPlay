using ChilliSource.Mobile.Core;
using ReactiveUI;

namespace TalkiPlay.Shared
{
    public interface IConnectivityNotifier
    {
        NetworkConnectionNotifier Notifier { get; }
    }
    
    public class NetworkConnectionNotifier : Interaction<ServiceResult, bool>
    {
         
         
    }
}