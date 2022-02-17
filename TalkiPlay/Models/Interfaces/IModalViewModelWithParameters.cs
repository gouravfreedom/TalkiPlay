using ChilliSource.Mobile.UI.ReactiveUI;

namespace TalkiPlay.Shared
{
    public interface IModalViewModelWithParameters : IModalViewModel
    {
        object Parameters { get; set; }
    }
}