using System.Reactive;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using Splat;

namespace TalkiPlay.Shared
{
    public class ItemsPackListPageViewModel : PackListPageViewModel
    {
        public ItemsPackListPageViewModel(INavigationService navigator) 
            : base(navigator)
        {
            this.SetupCommands();
        }

        private void SetupCommands()
        {
             
            SelectCommand = ReactiveCommand.CreateFromTask<ItemSelectionViewModel, Unit>( async m =>
                {
                    _gameMediator.CurrentPack = m.Source as IPack;
                    var room = _gameMediator.CurrentRoom;
                    var itemGroup = _gameMediator.CurrentPack;
                    //var navigator = Locator.Current.GetService<INavigationService>(TabItemType.Items.ToString());

                    if (room.Packs.Contains(itemGroup.Id))
                    {
                        await SimpleNavigationService.PushAsync(new ItemListPageViewModel(null), animated: false);
                        await SimpleNavigationService.PopModalAsync();
                    }
                    else
                    {
                        await SimpleNavigationService.PushAsync(new ItemsTagItemStartPageViewModel(Navigator));
                    }
                    return Unit.Default;
                }
            );

            SelectCommand.ThrownExceptions.SubscribeAndLogException();
            
            BackCommand = ReactiveCommand.CreateFromTask(() =>
            {
                //var navigator = Locator.Current.GetService<INavigationService>(TabItemType.Items.ToString());
                //navigator.PopToRootPage().SubscribeSafe();
                //return Navigator.PopModal();
                SimpleNavigationService.PopToRootAsync().Wait();
                return SimpleNavigationService.PopModalAsync();

            });
            
            BackCommand.ThrownExceptions.SubscribeAndLogException();
        }
    }
}