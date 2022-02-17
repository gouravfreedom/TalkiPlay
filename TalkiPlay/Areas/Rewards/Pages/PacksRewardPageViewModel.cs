using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.UI;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public class ItemRewardViewModel : ReactiveObject
    {
        public ItemRewardViewModel(ItemDto item)
        {
            Item = item;
        }

        public ItemDto Item { get; }

        [Reactive] public double Progress { get; private set; } = 0d;

        public void UpdateProgress(double progress)
        {
            Progress = progress;
            this.RaisePropertyChanged(nameof(Progress));
        }
    }


    public class PackRewardViewModel : ReactiveObject
    {
        public PackRewardViewModel(IPack pack)
        {
            UpdatePack(pack);
        }        

        public IPack Pack { get; private set; }
        [Reactive] public double Progress { get; private set; } = 0d;
        [Reactive] public int TotalMinutesPlayed { get; private set; } = 0;

        public IEnumerable<ItemRewardViewModel> Items { get; private set; }

        [Reactive] public string TimeUnit { get; private set; } = "min";

        public void UpdatePack(IPack pack)
        {
            Pack = pack;

            var items = new List<ItemRewardViewModel>();
            var packItems = pack.Items.Where(i => i.Type == ItemType.Default).OrderBy(a => a.Type.GetData<int>("SortOrder")).ToList();

            foreach (var item in packItems)
            {
                items.Add(new ItemRewardViewModel(item));
            }

            Items = items;
            this.RaisePropertyChanged(nameof(Items));
        }

        public void UpdateProgress(IChildPackProgress rewards)
        {
            Progress = rewards.Progress;
            TotalMinutesPlayed = (int)Math.Round(rewards.TotalMinutesPlayed);
            TimeUnit = TotalMinutesPlayed > 1 ? "mins" : "min";

            this.RaisePropertyChanged(nameof(TotalMinutesPlayed));
            this.RaisePropertyChanged(nameof(Progress));
            this.RaisePropertyChanged(nameof(TimeUnit));
        }

        public void UpdateItemsProgress(IList<ChildItemProgressDto> progresses)
        {
            foreach (var item in Items)
            {
                var itemReward = progresses?.FirstOrDefault(i => i.Id == item.Item.Id);
                item.UpdateProgress(itemReward == null ? 0d : itemReward.Star);
            }
        }
    }


    public class PacksRewardPageViewModel : BasePageViewModelEx
    {
        private readonly IGameService _gameService;
        private readonly IAssetRepository _assetRepository;
        private readonly IChildrenRepository _childRepoistory;
        private IList<PackDto> _allPacks { get; set; }
        private IList<GameDto> _allGames { get; set; }

        public PacksRewardPageViewModel()
        {
            _gameService = Locator.Current.GetService<IGameService>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            _childRepoistory = Locator.Current.GetService<IChildrenRepository>();
            var appService = Locator.Current.GetService<IApplicationService>();
            var padding = appService.ScreenSize.Width * 6 / 100;
            var overlap = appService.ScreenSize.Width * 2 / 100;

            var gameWidth = (appService.ScreenSize.Width - padding * 2) / 2;
            var gameHeight = WidthToHeightByImageRatio.Convert(gameWidth);
            Game1Bound = new Rectangle(padding + overlap/2, 0, gameWidth, gameHeight);
            Game2Bound = new Rectangle(appService.ScreenSize.Width / 2 - overlap / 2, 0, gameWidth, gameHeight);

            SetupCommands();
        }

        public ObservableCollection<PackRewardViewModel> Packs { get; } = new ObservableCollection<PackRewardViewModel>();
        private IChild _oldChild;
        [Reactive] public IChild ActiveChild { get; private set; }

        public ICommand SelectChildCommand { get; private set; }

        public ICommand GotoStartGuide { get; private set; }

        public ICommand GotoRewardList { get; private set; }

        public ReactiveCommand<Unit, Unit> LoadAllPacksCommand { get; private set; }
        public ReactiveCommand<PackRewardViewModel, Unit> LoadOnePackCommand { get; private set; }


        [Reactive] public GameDto FavoriteGame1 { get; private set; }
        [Reactive] public GameDto FavoriteGame2 { get; private set; }
       
        public Rectangle Game1Bound { get; }
        public Rectangle Game2Bound { get; }

        public override string Title => "Rewards";

        [Reactive] public PackRewardViewModel SelectedPack { get; set; }

        private async Task RefreshAllPacks()
        {
            try
            {
                var packsProgress = await _gameService.GetChildPacksProgress(ActiveChild != null ? ActiveChild.Id : 0);

                foreach (var pack in Packs)
                {
                    pack.UpdateProgress(packsProgress.FirstOrDefault(p => p.Id == pack.Pack.Id) ?? new ChildPackProgressDto() { Id = pack.Pack.Id });
                }
            }
            catch { }
        }

        private void SetupCommands()
        {
            SelectChildCommand = ReactiveCommand.CreateFromTask(() =>
            {
                SimpleNavigationService.PushModalAsync(new ChildListPageViewModel(true)).Forget();
                return Task.CompletedTask;
            });

            GotoStartGuide = ReactiveCommand.CreateFromTask(() =>
            {
                var state = new GuideState()
                {
                    IsModal = false,
                    SelectedChild = ActiveChild,
                    SelectedPack = Packs.FirstOrDefault(p => p.Pack.Id == ActiveChild?.FavouritePackId)?.Pack
                };
                SimpleNavigationService.PushAsync(new GuideInfoPageViewModel(state.StartStep, state)).Forget();
                return Task.CompletedTask;
            });

            GotoRewardList = ReactiveCommand.CreateFromTask(() =>
            {
                SimpleNavigationService.PushAsync(new RewardListPageViewModel(ActiveChild)).Forget();
                return Task.CompletedTask;
            });

            LoadAllPacksCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                Dialogs.ShowLoading();
                ActiveChild = _childRepoistory.ActiveChild;
                _oldChild = ActiveChild;
                _allGames = await _gameService.GetGames();
                _allPacks = await _assetRepository.GetPacks();

                var allGamePacks = _allPacks.Where(p => _allGames.Any(a => a.PackId == p.Id));
                var childFavoritePack = ActiveChild?.FavouritePackId ?? allGamePacks.FirstOrDefault(g => g.Name?.Contains("Color") == true)?.Id;
                var childDefaultGames = _allGames.Where(b => b.PackId == childFavoritePack);

                FavoriteGame1 = childDefaultGames.FirstOrDefault(g => g.Type != GameType.Hunt);
                FavoriteGame2 = childDefaultGames.FirstOrDefault(g => g.Type == GameType.Hunt);
                this.RaisePropertyChanged(nameof(FavoriteGame1));
                this.RaisePropertyChanged(nameof(FavoriteGame2));

                Packs.Clear();
                var packVms = _allPacks
                .Where(p => _allGames.Any(g => g.PackId == p.Id))
                .Select(p => {                    
                    var vm = new PackRewardViewModel(p);
                    return vm;
                });
                Packs.AddRange(packVms);
                await RefreshAllPacks();
                Dialogs.HideLoading();
            });

            LoadAllPacksCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();

            LoadOnePackCommand = ReactiveCommand.CreateFromTask<PackRewardViewModel, Unit>(async (packVm) =>
           {
               if (packVm == null) return Unit.Default;
               SelectedPack = packVm;
               Dialogs.ShowLoading();
               ActiveChild = _childRepoistory.ActiveChild;
               if (_oldChild?.Id != ActiveChild?.Id)
               {
                   await RefreshAllPacks();
               }
               _oldChild = ActiveChild;
               if (packVm.Items.Count() == 0)
               {
                   var pack = await _assetRepository.GetPack(packVm.Pack.Id);
                   packVm.UpdatePack(pack);
               }

               IList<ChildItemProgressDto> itemsProgress = null;

               try
               {
                   itemsProgress = await _gameService.GetChildItemsProgress(ActiveChild != null ? ActiveChild.Id : 0, packVm.Pack.Id);
               }
               catch (Exception) { }

               packVm.UpdateItemsProgress(itemsProgress ?? new List<ChildItemProgressDto>());

               Dialogs.HideLoading();
               return Unit.Default;
           });

            LoadOnePackCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();
        }

        public void LoadDataOnAppear(bool loadAll)
        {
            if (loadAll)
            {
                LoadAllPacksCommand.Execute();
            }
            else
            {
                if (SelectedPack == null)
                {
                    SelectedPack = Packs.FirstOrDefault();
                }

                LoadOnePackCommand.Execute(SelectedPack);
            }
        }
    }
}
