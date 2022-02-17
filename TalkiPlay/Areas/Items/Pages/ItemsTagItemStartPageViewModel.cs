using System.Reactive;
using System.Reactive.Linq;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using Splat;

namespace TalkiPlay.Shared
{
    public class ItemsTagItemStartPageViewModel : TagItemStartPageViewModel
    {
        public ItemsTagItemStartPageViewModel(INavigationService navigator) : base(navigator)
        {
            SetupCommand();
        }

        private void SetupCommand()
        {
            BeginCommand = ReactiveCommand.CreateFromTask<Unit, Unit>(async m =>
            {
                var mediator = new TagItemsSelector(_room, PackItems, CurrentPack);
                var current = mediator.GetNextItem();
                if (current != null)
                {
                    await SimpleNavigationService.PushAsync(new ItemsTagItemSetupPageViewModel(Navigator, mediator, current));
                }
                else
                {
                    _userDialogs.Toast("There are no items to be tagged.");
                }

                return Unit.Default;
            });

            BeginCommand.ThrownExceptions.SubscribeAndLogException();


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