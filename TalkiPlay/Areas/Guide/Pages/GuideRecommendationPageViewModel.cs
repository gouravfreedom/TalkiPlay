using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.UI.ReactiveUI;
using Splat;

namespace TalkiPlay.Shared
{
    public class GuideRecommendationPageViewModel : WizardBasePageViewModel
    {
        public GuideRecommendationPageViewModel(GuideStep step, GuideState state) : base(step, state)
        {
            InfoText = GuideHelper.GetBodyTextForStep(step, state);
        

            
            LoadData().Forget();
        }
        
        public string InfoText { get; }
        
        public List<GuideGameViewModel> Items { get; set; }

        async Task LoadData()
        {
            Dialogs.ShowLoading();

            try
            {
                var games = await State.GetGameRecommendations();

                Items = new List<GuideGameViewModel>();
                var hasSubscription = await SubscriptionService.GetUserHasSubscription();
                foreach (var game in games)
                {
                    Items.Add(new GuideGameViewModel(game, hasSubscription));
                }
                
                RaisePropertyChanged(nameof(Items));
                
                State.SaveRecommendedGames(games);
            }
            catch (Exception e)
            {
                e.ShowExceptionDialog();
            }
            
            Dialogs.HideLoading();
        }
    }
}