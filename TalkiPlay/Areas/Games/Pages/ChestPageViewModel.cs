using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Acr.UserDialogs;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;


namespace TalkiPlay.Shared
{
    public enum RewardMode
    {
        NotCollected,
        Collected
    }

    public class ChestPageViewModel : BasePageViewModel, IActivatableViewModel, IModalViewModel, IPopModalViewModel
    {        
        readonly IAssetRepository _assetRepository;
        readonly IGameMediator _gameMediator;
        readonly RewardMode _rewardMode;
        readonly Action _callback;

        public ChestPageViewModel(INavigationService navigator,
            RewardMode rewardMode = RewardMode.NotCollected,
            IReward reward = null,
            IAsset rewardAsset = null,
            Action callback = null,
            IGameMediator gameMediator = null,
            IAssetRepository assetRepository = null)
        {
            _rewardMode = rewardMode;
            Reward = reward;
            RewardAsset = rewardAsset;

            _callback = callback;
            _assetRepository = assetRepository ?? Locator.Current.GetService<IAssetRepository>();
            _gameMediator = gameMediator ?? Locator.Current.GetService<IGameMediator>();
            Activator = new ViewModelActivator();
            Navigator = navigator ?? Locator.Current.GetService<INavigationService>(TabItemType.Games.ToString());

            CurrentGame = _gameMediator.CurrentGame;
            RewardEgg = Images.RewardEggUnHatchedImage;            
            SetRewardMode(_rewardMode);
            SetupCommands();
        }

        public override string Title => "";

        [Reactive]
        public string RewardEgg { get; set; }

        [Reactive]
        public string RewardInstruction { get; set; }

        public ViewModelActivator Activator { get; }

        public ReactiveCommand<Unit, Unit> CrackEggCommand { get; private set; }

        [Reactive]
        public IAsset RewardAsset { get; set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }

        private IGame CurrentGame { get; set; }

        private IReward Reward { get; set; }

        [Reactive]
        public bool ShowCloseButton { get; set; }

        [Reactive] public bool StartPlayingAnim { get; set; } = false;

        [Reactive] public RewardMode CurrentMode { get; set; }

        void SetRewardMode(RewardMode rewardMode)
        {
            if (rewardMode == RewardMode.Collected)
            {
                RewardInstruction = "";                               
            }
            else
            {
                RewardInstruction = "Tap to crack the egg and collect your reward";
                ShowCloseButton = false;
            }

            CurrentMode = rewardMode;
        }

        void LoadRewardImage()
        {
            StartPlayingAnim = true;
            this.RaisePropertyChanged(nameof(StartPlayingAnim));
            RewardEgg = RewardAsset?.ImageContentPath;//.ToResizedImage(300);
        }

        void SetupCommands()
        {
            CrackEggCommand = ReactiveCommand.Create(() =>
            {
                if (CurrentMode == RewardMode.NotCollected)
                {
                    SetRewardMode(RewardMode.Collected);
                    LoadRewardImage();
                }
            });
       
            CrackEggCommand.ThrownExceptions.SubscribeAndLogException();
            
            BackCommand = ReactiveCommand.CreateFromTask(async() =>
            {
                await SimpleNavigationService.PopPopupAsync();
                _callback?.Invoke();
            });

            
            BackCommand.ThrownExceptions.SubscribeAndLogException();

            LoadDataCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (RewardAsset != null)
                {

                }
                else
                {
                    if (CurrentGame?.Chest?.Rewards == null)
                    {
                        return;
                    }

                    Reward = CurrentGame.Chest.Rewards.Shuffle().FirstOrDefault();

                    if (Reward == null)
                    {
                        return;
                    }

                    var userDialogs = Locator.Current.GetService<IUserDialogs>();
                    userDialogs.ShowLoading("Loading ...");
                    RewardAsset = await _assetRepository.GetAssetById(Reward.OpenImageAssetId);
                    userDialogs.HideLoading();
                }
                
                if (_rewardMode == RewardMode.Collected)
                {
                    LoadRewardImage();
                }
            });
            
            LoadDataCommand.ThrownExceptions.SubscribeAndLogException();
        }
    }
}