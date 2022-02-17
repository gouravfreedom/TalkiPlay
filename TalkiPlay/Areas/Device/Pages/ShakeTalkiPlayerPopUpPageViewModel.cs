using ChilliSource.Mobile.Core;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace TalkiPlay.Shared
{
    public class ShakeTalkiPlayerPopUpPageViewModel : ReactiveObject, IPopModalViewModel
    {
        //private readonly ILogger _logger;
        //private readonly INavigationService _navigator;
        private ObservableRangeCollection<TalkiPlayerInstructionItemViewModel> _instructions
            = new ObservableRangeCollection<TalkiPlayerInstructionItemViewModel>();

        public ShakeTalkiPlayerPopUpPageViewModel(
            //INavigationService navigator,
            //ILogger logger = null
            )
        {
            //_logger = logger ?? Locator.Current.GetService<ILogger>();
            //_navigator = navigator;
            SetupInstructions();
        }

        void SetupInstructions()
        {
            _instructions.Add(new TalkiPlayerInstructionItemViewModel()
            {
                Header = $"Shake {Constants.DeviceName} until you hear the sound",
                Image = Images.ShakeTalkiPlayerImage
            });
            
            _instructions.Add(new TalkiPlayerInstructionItemViewModel()
            {
                Header = $"Tap any tag to activate",
                Image = Images.TapTagTalkiPlayerImage
            });
        }

        public string Title => "";
        public ObservableRangeCollection<TalkiPlayerInstructionItemViewModel> Instructions => _instructions;
    }

    public class TalkiPlayerInstructionItemViewModel : ReactiveObject
    {
        [Reactive]
        public string Header { get; set; }
        
        [Reactive]
        public string Image { get; set; }                
    }
}