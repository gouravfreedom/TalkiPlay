using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using Splat;

namespace TalkiPlay.Shared
{
    public class RewardListPageViewModel : SimpleBasePageModel
    {
        private readonly IChild _child;

        public RewardListPageViewModel(IChild child)
        {
            _child = child;
            
            SetupCommands();

            LoadDataCommand.Execute();
        }
        
        public List<RewardItemsViewModel> Rewards { get; private set; }
        
        public int CurrentIndex { get; set; }

        public ReactiveCommand<Unit, Unit> LoadDataCommand { get; private set; }
        
        public ReactiveCommand<Unit, Unit> BackCommand { get; private set; }

        public double CarouselHeight { get; set; }
        
        public double CarouselWidth { get; set; }

        void SetupCommands()
        {
            BackCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                await SimpleNavigationService.PopAsync();
                return Unit.Default;
            });
            BackCommand.ThrownExceptions.SubscribeAndLogException();
            
            LoadDataCommand = ReactiveCommand.CreateFromTask( async (Unit _) =>
            {
                await LoadData();
                
                return Unit.Default;
            });
            
            LoadDataCommand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .ShowExceptionDialog()
                .SubscribeAndLogException();
        }

        async Task LoadData()
        {
            Dialogs.ShowLoading();

            var assetRepository = Locator.Current.GetService<IAssetRepository>();
            
            var repository = Locator.Current.GetService<IRewardsRepository>();
            var allRewards = await repository.GetAllRewards();
            var totalRewards = allRewards.Count;

            var childRewards = await repository.GetChildRewards((_child.Id));
            var items = new List<RewardItemViewModel>();
            foreach (var childReward in childRewards)
            {
                childReward.Asset = await assetRepository.GetAssetById(childReward.CollectImageAssetId);
                items.Add(new RewardItemViewModel(childReward));
            }

            for (var i = 0; i < totalRewards - childRewards.Count; ++i)
            {
                items.Add(new RewardItemViewModel(null));
            }
            
            var numberOfColumns = (int)((CarouselWidth-8.0) / 84.0);
            var numberOfRows = (int)((CarouselHeight-8.0) / 84.0);
            var rewardsPerPage = (int)numberOfRows * (int)numberOfColumns;
            
            var numberOfPages = totalRewards / rewardsPerPage;
            if (totalRewards % rewardsPerPage > 0)
            {
                numberOfPages++;
            }
            
            var list = new List<RewardItemsViewModel>();
            
            for (var i = 0; i < numberOfPages; ++i)
            {
                var count = Math.Min(items.Count - i * rewardsPerPage, rewardsPerPage);
                var range = items.GetRange(i * rewardsPerPage, count);
                
                list.Add(new RewardItemsViewModel(range, numberOfColumns));
            }
        
            Rewards = list;
            RaisePropertyChanged(nameof(Rewards));
            
            Dialogs.HideLoading();
            
        }
    }
}