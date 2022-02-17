using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Acr.UserDialogs;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using Unit = System.Reactive.Unit;


namespace TalkiPlay.Shared
{

    public enum TagItemStatus
    {
        SetupTag,
        ReadTag,
        TagMappingCompleted,
        AllItemsMapped
    }

    public interface ITagItemSetupPageViewModel : IBasePageViewModel, IActivatableViewModel
    {
        string NavigationTitle { get; }

        string GoButtonText { get; set; }
        string SkipButtonText { get; set; }
        string Heading { get; }

        string InstructionImage { get; }

        string SubHeading { get; }

        ReactiveCommand<Unit, Unit> GoCommand { get; }
        ReactiveCommand<Unit, Unit> SkipCommand { get; }

        ReactiveCommand<Unit, Unit> LoadCommand { get; }

        bool ShowGoButton { get; }
        bool ShowSkipButton { get; }
        string Message { get; }
        bool ShowTabsHelp { get; set; }

        ReactiveCommand<Unit, Unit> CloseCommand { get; }
    }

    public class TagItemSetupPageViewModel : BasePageViewModel, ITagItemSetupPageViewModel
    {
        protected readonly TagItemsSelector _tagItemsSelector;
        protected readonly IGameService _gameService;
        protected readonly ITalkiPlayerManager _talkiPlayerManager;
        protected readonly IUserDialogs _userDialogs;
        protected readonly IAssetRepository _assetRepository;

        public TagItemSetupPageViewModel(
            INavigationService navigator,
            TagItemsSelector tagItemsSelector,
            ItemDto currenItem,
            IUserDialogs userDialogs = null,
            TagItemStatus status = TagItemStatus.SetupTag,
            ITalkiPlayerManager talkiPlayerManager = null,
            IGameService gameService = null)
        {
            _tagItemsSelector = tagItemsSelector;
            _gameService = gameService ?? Locator.Current.GetService<IGameService>();
            _talkiPlayerManager = talkiPlayerManager ?? Locator.Current.GetService<ITalkiPlayerManager>();
            _userDialogs = userDialogs ?? Locator.Current.GetService<IUserDialogs>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            Activator = new ViewModelActivator();
            CurrentItem = currenItem;
            InstructionImage = currenItem.ImagePath ?? Images.PlaceHolder;
            _tagItemsSelector.SetCurrent(currenItem);
            Navigator = navigator;
            NavigationTitle = Title;
            Status = status;
            SetupRx();
            SetupCommand();
        }

        public override string Title => "Tag setup";

        [Reactive] public string NavigationTitle { get; set; }
        public ViewModelActivator Activator { get; }

        [Reactive] public string GoButtonText { get; set; } = "I'm Ready";

        [Reactive] public string SkipButtonText { get; set; } = "Skip";

        [Reactive] public string Heading { get; set; }

        [Reactive] public string InstructionImage { get; set; }

        [Reactive] public string SubHeading { get; set; }

        [Reactive] public ItemDto CurrentItem { get; set; }

        [Reactive] public TagItemStatus Status { get; set; }

        public ReactiveCommand<Unit, Unit> GoCommand { get; protected set; }
        public ReactiveCommand<Unit, Unit> SkipCommand { get; protected set; }

        public ReactiveCommand<Unit, Unit> LoadCommand { get; protected set; }

        [Reactive] public bool ShowGoButton { get; set; } = true;
        [Reactive] public bool ShowSkipButton { get; set; } = true;

        [Reactive] public string Message { get; set; } = " ";
        [Reactive] public bool ShowTabsHelp { get; set; }

        public ReactiveCommand<Unit, Unit> CloseCommand { get; set; }

        [Reactive] public bool HasError { get; set; }

        ReactiveCommand<IDataUploadResult, Unit> DataHandleCommmand { get; set; }

        void SetupRx()
        {
            this.WhenActivated(d =>
            {
                _talkiPlayerManager?.Current?.OnDataResult()
                    .InvokeCommand(this, m => m.DataHandleCommmand)
                    .DisposeWith(d);

                this.WhenAnyValue(m => m.Status, m => m.CurrentItem, m => m.HasError, (status, item, hasError) => (status, item, hasError))
                    .ObserveOn(RxApp.MainThreadScheduler)
                    .SubscribeSafe(m =>
                    {
                        var (status, item, hasError) = m;
                        UpdateUI(status, item, hasError);
                    })
                    .DisposeWith(d);

                //_talkiPlayerManager?.Current?.WhenDisconnectedUponInactivity()
                //    .SubscribeSafe()
                //    .DisposeWith(d);

            });
        }

        void SetupCommand()
        {
            GoCommand = ReactiveCommand.CreateFromTask(PerformGoCommand);

            GoCommand.ThrownExceptions.HideLoading().SubscribeAndLogException();

            CloseCommand = ReactiveCommand.CreateFromTask(() =>
            {
                this.ShowTabsHelp = false;
                return SimpleNavigationService.PopModalAsync();
            });

            CloseCommand.ThrownExceptions.HideLoading().SubscribeAndLogException();

            SkipCommand = ReactiveCommand.CreateFromTask(() =>
            {
                HasError = false;
                _talkiPlayerManager.Current?.CancelUpload();
                if (Status != TagItemStatus.TagMappingCompleted)
                {
                    var next = _tagItemsSelector.GetNextItem();
                    if (next == null)
                    {
                        Status = TagItemStatus.AllItemsMapped;
                    }
                    else
                    {
                        return SimpleNavigationService.PushAsync(new TagItemSetupPageViewModel(Navigator, _tagItemsSelector, next));
                    }
                }
                else
                {
                    return SimpleNavigationService.PushAsync(new TagItemSetupPageViewModel(Navigator, _tagItemsSelector, CurrentItem));
                }


                return Task.CompletedTask;
            });

            SkipCommand.ThrownExceptions.SubscribeAndLogException();

            LoadCommand = ReactiveCommand.Create(() => { });

            LoadCommand.ThrownExceptions.SubscribeAndLogException();

            BackCommand = ReactiveCommand.CreateFromTask(() =>
            {
                _talkiPlayerManager.Current?.CancelUpload();

                if (CurrentItem.Type == ItemType.Home)
                {
                    //var navigator = Locator.Current.GetService<INavigationService>(TabItemType.Games.ToString());
                    //navigator.PopToRootPage().SubscribeSafe();
                    //return Navigator.PopModal();
                    SimpleNavigationService.PopToRootAsync().Wait();
                    return SimpleNavigationService.PopModalAsync();
                }

                var previous = _tagItemsSelector.GetPreviousItem();

                if (previous != null)
                {
                    return SimpleNavigationService.PopAsync();
                }

                return Task.CompletedTask;

            });

            BackCommand.ThrownExceptions.SubscribeAndLogException();


            DataHandleCommmand = ReactiveCommand.CreateFromTask<IDataUploadResult, Unit>(async data =>
            {
                if (!data.IsSuccess)
                {
                    var error = data.GetData<ErrorDataResult>();
                    throw error.Exception;
                }

                if (data.Type != UploadDataType.TagData ||
                    data.Data == null ||
                    !(data.Data is TagData tagData) ||
                    String.IsNullOrWhiteSpace(data.Tag))
                {
                    throw new ApplicationException("Data is invalid");
                }

                var itemId = int.Parse(data.Tag);
                var item = _tagItemsSelector.GetItem(itemId);
                if (item != null)
                {
                    _userDialogs.ShowLoading("Creating tag ...");
                    var tag = await _assetRepository.AddTag(item.Id, tagData.SerialNumber);
                    _userDialogs.HideLoading();
                    _tagItemsSelector.MappedTags.Add(tag.Id);
                    Status = TagItemStatus.TagMappingCompleted;

                   //return ObservableOperatorExtensions.StartShowLoading("Creating tag ...")
                   //    .ObserveOn(RxApp.TaskpoolScheduler)
                   //    .SelectMany(_ => _gameService.AddTag(item.Id, tagData.SerialNumber))                       
                   //    .ObserveOn(RxApp.MainThreadScheduler)
                   //    .HideLoading()
                   //    .Do(tag =>
                   //    {
                   //        _tagItemsSelector.MappedTags.Add(tag.Id);
                   //        Status = TagItemStatus.TagMappingCompleted;
                   //    })
                   //    .Select(m => Unit.Default);
               }

                return Unit.Default;

               //return Observable.Return(Unit.Default);
           });

            DataHandleCommmand.ThrownExceptions
                .ObserveOn(RxApp.MainThreadScheduler)
                .HideLoading()
                .Do(m => { HasError = true; })
                .SubscribeAndLogException();
        }

        async Task<Unit> PerformGoCommand()
        {
            Message = "";

            if (Status == TagItemStatus.SetupTag || (Status == TagItemStatus.ReadTag && HasError))
            {
                Status = TagItemStatus.ReadTag;
                HasError = false;
                _talkiPlayerManager.Current?.UploadTagData(CurrentItem);                
            }
            else if (Status == TagItemStatus.AllItemsMapped)
            {
                var room = _tagItemsSelector.Room;
                var pack = _tagItemsSelector.Pack;
                var roomPackIds = new List<int>(room.Packs);

                if (pack != null && pack.Id > 0)
                {
                    roomPackIds.Add(pack.Id);
                }

                var mappedTagIds = _tagItemsSelector.MappedTags;
                var roomTags = room.TagItems.Select(a => a.Id).ToList();
                roomTags.AddRange(mappedTagIds);

                _userDialogs.ShowLoading("Saving ...");
                await _assetRepository.UpdateRoomItemTags(new RoomDto(room.Id, roomTags, roomPackIds));
                //var items = await _gameService.GetItems(ItemType.Home);
                _userDialogs.HideLoading();
                await SimpleNavigationService.PopModalAsync();

                this.ShowTabsHelp = true;                

                //return ObservableOperatorExtensions
                //    .StartShowLoading("Saving ...")
                //    .ObserveOn(RxApp.TaskpoolScheduler)
                //    .SelectMany(m => _gameService.UpdateRoomItemTags(new RoomDto(room.Id, roomTags, roomPackIds)))
                //    .SelectMany(room => _gameService.GetItems(ItemType.Home).Select(items => (Items: items, Room: room)))
                //    .ObserveOn(RxApp.MainThreadScheduler)
                //    .HideLoading()
                //    .SelectMany(result =>
                //    {
                //        //var room = result.Room;
                //        //var homeTagItems = result.Items.Select(a => a.Id);
                //        //var navigator = Locator.Current.GetService<INavigationService>(TabItemType.Games.ToString());

                //        //if (room.TagItems.Count == 1 && room.TagItems.Any(a => homeTagItems.Contains(a.ItemId)))
                //        //{
                //        //    navigator.PopToRootPage().SubscribeSafe();
                //        //}
                //        //else
                //        //{
                //        //    navigator.PopToRootPage().SelectMany(_ => navigator.PushPage(new GameListPageViewModel(navigator))).SubscribeSafe();
                //        //}

                //        Navigator.PopModal();

                //        this.ShowTabsHelp = true;
                //        return Observable.Return(Unit.Default);

                //    });
            }
            else
            {
                var next = _tagItemsSelector.GetNextItem();
                if (next == null)
                {
                    Status = TagItemStatus.AllItemsMapped;
                }
                else
                {
                    await SimpleNavigationService.PushAsync(new TagItemSetupPageViewModel(Navigator, _tagItemsSelector, next));
                }
            }

            return Unit.Default;
        }



        protected virtual void UpdateUI(TagItemStatus status, IItem item, bool hasError)
        {
            Message = " ";
            switch (status)
            {
                case TagItemStatus.SetupTag:
                    ShowGoButton = true;
                    ShowSkipButton = true;

                    if (item.Type == ItemType.Home)
                    {
                        NavigationTitle = $"Setup {CurrentItem.Name}";
                        Heading = $"Find a {CurrentItem.Name} from pack!";
                        ShowSkipButton = _tagItemsSelector.Room?.TagItems?.SelectMany(a => a.ItemIds).Contains(item.Id) ??
                                         false;
                    }
                    else
                    {
                        NavigationTitle = "Stick me on a ...";
                        Heading = CurrentItem.Name;
                    }

                    GoButtonText = "I'm Ready";
                    SkipButtonText = "Skip";

                    break;
                case TagItemStatus.ReadTag:
                    if (hasError)
                    {
                        this.Message = "Oops! it didn't work. Please try again!'";
                        ShowGoButton = true;
                        ShowSkipButton = true;
                        this.GoButtonText = "Try again";
                        SkipButtonText = "Skip";
                        HasError = true;
                    }
                    else
                    {
                        this.Message = "";
                        ShowGoButton = false;
                        ShowSkipButton = false;
                        NavigationTitle = $"{CurrentItem.Name}";
                        Heading = $"Activate your {CurrentItem.Name}";
                        SubHeading = $"Tap and hold your disc to your {Constants.DeviceName}";
                    }

                    break;
                case TagItemStatus.TagMappingCompleted:
                    this.Message = "Item has been successfully tagged";
                    ShowGoButton = true;
                    ShowSkipButton = true;
                    GoButtonText = "Next";
                    SkipButtonText = "Add another one";
                    ShowSkipButton = item.Type != ItemType.Home;
                    break;
                case TagItemStatus.AllItemsMapped:
                    NavigationTitle = "Setup completed";
                    Heading = $"Congratulations!";
                    SubHeading = $"Your {_tagItemsSelector.Pack.Name} game pack is ready to start the magic!";
                    this.Message = " ";
                    ShowGoButton = true;
                    ShowSkipButton = false;
                    GoButtonText = "Let's play";
                    InstructionImage = Images.FireworksImage;
                    break;
            }
        }
    }
}