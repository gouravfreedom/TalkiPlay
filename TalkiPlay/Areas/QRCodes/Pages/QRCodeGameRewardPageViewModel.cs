using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Splat;
using System.Reactive.Linq;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class QRCodeGameRewardPageViewModel : SimpleBasePageModel
    {
        readonly Action _callback;
        readonly IChest _chest;
        //readonly int _gameId;
        readonly IReward _reward;

        public QRCodeGameRewardPageViewModel(IChest chest, IReward reward, Action callback = null)
        {
            _reward = reward;
            _chest = chest;
            _callback = callback;
            
            SetRewardMode(RewardMode.NotCollected);
            ImageSource = Images.RewardEggUnHatchedImage;
            
            
            TapCommand = new Command(() => ShowReward().Forget());

            CloseCommand = new Command(async () =>
            {
                await SimpleNavigationService.PopPopupAsync();
                _callback?.Invoke();
            });
        }
        
        public ICommand CloseCommand { get; set; }

        public ICommand TapCommand { get; set; }

        public string RewardInstruction { get; set; }
        
        public string ImageSource { get; set; }
                
        public bool ShowCloseButton { get; set; }


        async Task ShowReward()
        {
            Dialogs.ShowLoading();

            try
            {
                var assetRepository = Locator.Current.GetService<IAssetRepository>();
                IAsset asset = null;
                if (_reward != null)
                {
                    asset = await assetRepository.GetAssetById(_reward.OpenImageAssetId);
                }
            
                if (asset != null)
                {
                    ImageSource = asset.ImageContentPath.ToResizedImage(300);
                }

                RaisePropertyChanged(nameof(ImageSource));
                SetRewardMode(RewardMode.Collected);
            
                await Task.Delay(500);

                Dialogs.HideLoading();
            }
            catch (Exception e)
            {
                Dialogs.HideLoading();
                e.ShowExceptionDialog();
            }
        }

        void SetRewardMode(RewardMode rewardMode)
        {
            if (rewardMode == RewardMode.Collected)
            {
                RewardInstruction = "You've collected your reward";
                ShowCloseButton = true;
            }
            else
            {
                RewardInstruction = "Tap to crack an egg and collect your reward";
                ShowCloseButton = false;
            }

            RaisePropertyChanged(nameof(RewardInstruction));
            RaisePropertyChanged(nameof(ShowCloseButton));
        }

    }
}
