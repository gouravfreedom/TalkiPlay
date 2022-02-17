using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ChilliSource.Mobile.UI.ReactiveUI;
using FormsControls.Base;
using ReactiveUI;
using Splat;

namespace TalkiPlay.Shared
{
    public class ItemsTagItemSetupPageViewModel : TagItemSetupPageViewModel
    {
        private readonly bool _isTagSetupOnly;

        public ItemsTagItemSetupPageViewModel(INavigationService navigator, TagItemsSelector tagItemsSelector, 
            ItemDto currenItem, bool isTagSetupOnly = false) : base(navigator, tagItemsSelector, currenItem)
        {
            _isTagSetupOnly = isTagSetupOnly;
            this.SetupCommand();
        }

        private void SetupCommand()
        {            
            GoCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                Message = "";

                if (Status == TagItemStatus.SetupTag || Status == TagItemStatus.ReadTag && HasError)
                {
                    var tags = await _assetRepository.GetTags();
                    var count = tags.Count(t => t.ItemIds.Contains(CurrentItem.Id));

                    if (count >= 10)
                    {
                        _userDialogs.Alert("A maximum of 10 tags can be assigned to any item.", "Tag limit reached");

                    }
                    else
                    {
                        Status = TagItemStatus.ReadTag;
                        HasError = false;
                        _talkiPlayerManager.Current?.CancelUpload();
                        _talkiPlayerManager.Current?.Upload(new DataUploadData(CurrentItem.Name,
                            new { CurrentItem.Id, CurrentItem.Name },
                            $"{CurrentItem.Id}", UploadDataType.TagData), TimeSpan.FromMinutes(10));
                    }
                    return Unit.Default;

                    //return _gameService.GetTags()
                    //    .Select(tags => tags.Count(t => t.ItemIds.Contains(CurrentItem.Id)))
                    //    .ObserveOn(RxApp.MainThreadScheduler)
                    //    .SelectMany(count =>
                    //    {
                    //        if (count >= 10)
                    //        {
                    //            _userDialogs.Alert("A maximum of 10 tags can be assigned to any item.", "Tag limit reached");
                                                                
                    //        }
                    //        else
                    //        {
                    //            Status = TagItemStatus.ReadTag;
                    //            HasError = false;
                    //            _talkiPlayerManager.Current?.CancelUpload();
                    //            _talkiPlayerManager.Current?.Upload(new DataUploadData(CurrentItem.Name,
                    //                new { CurrentItem.Id, CurrentItem.Name },
                    //                $"{CurrentItem.Id}", UploadDataType.TagData), TimeSpan.FromMinutes(10));

                                
                    //        }
                    //        return Observable.Return(Unit.Default);
                    //    });                                            
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
                    var roomTagIds = room.TagItems.Select(a => a.Id).ToList();
                    roomTagIds.AddRange(mappedTagIds);

                    var roomDto = new RoomDto(room.Id, roomTagIds, roomPackIds);
                    await _assetRepository.UpdateRoomItemTags(roomDto);
                    //var navigator = Locator.Current.GetService<INavigationService>(TabItemType.Items.ToString());
                    //await navigator.PushPage(new ItemListPageViewModel(navigator), resetStack: true);//.SubscribeSafe();
                    await SimpleNavigationService.PopModalAsync();


                    //return _gameService.UpdateRoomItemTags(roomDto)
                    //    .ObserveOn(RxApp.MainThreadScheduler)
                    //    .SelectMany(m =>
                    //    {
                    //        var navigator = Locator.Current.GetService<INavigationService>(TabItemType.Items.ToString());
                    //        navigator.PushPage(new ItemListPageViewModel(navigator), resetStack: true).SubscribeSafe();
                    //        return Navigator.PopModal();

                    //    });
                }
                else if (Status == TagItemStatus.TagMappingCompleted && _isTagSetupOnly)
                {
                    var roomId = _tagItemsSelector.Room.Id;

                    var room = await _assetRepository.GetRoom(roomId);
                    var pack = _tagItemsSelector.Pack;
                    var roomPackIds = new List<int>(room.Packs);
                    if (pack != null && pack.Id > 0)
                    {
                        roomPackIds.Add(pack.Id);
                    }
                    var mappedTagIds = _tagItemsSelector.MappedTags;
                    var roomTags = room.TagItems.Select(a => a.Id).ToList();
                    roomTags.AddRange(mappedTagIds);

                    var distinctRoomTags = roomTags.Distinct().ToList();
                    var roomDto = new RoomDto(roomId, roomTags, roomPackIds);

                    await _assetRepository.UpdateRoomItemTags(roomDto);
                    await SimpleNavigationService.PushAsync(new ItemListPageViewModel(Navigator));

                    //return _gameService.GetRoom(roomId)
                    //    .Select(room =>
                    //    {
                    //        var pack = _tagItemsSelector.Pack;
                    //        var roomPackIds = new List<int>(room.Packs);
                    //        if (pack != null && pack.Id > 0)
                    //        {
                    //            roomPackIds.Add(pack.Id);
                    //        }
                    //        var mappedTagIds = _tagItemsSelector.MappedTags;
                    //        var roomTags = room.TagItems.Select(a => a.Id).ToList();
                    //        roomTags.AddRange(mappedTagIds);
                    //        return (RoomTags: roomTags.Distinct().ToList(), Packs: roomPackIds);
                    //    })
                    //    .SelectMany(data =>
                    //    {
                    //        var roomTags = data.RoomTags;
                    //        var packIds = data.Packs;

                    //        var roomDto = new RoomDto(roomId, roomTags, packIds);

                    //        return _gameService.UpdateRoomItemTags(roomDto);
                    //    })
                    //    .ObserveOn(RxApp.MainThreadScheduler)
                    //    .SelectMany(m => Navigator.PushPage(new ItemListPageViewModel(Navigator), resetStack: true));
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
                        await SimpleNavigationService.PushAsync(new ItemsTagItemSetupPageViewModel(Navigator, _tagItemsSelector, next));
                    }
                }
                
                return Unit.Default;
            });

            GoCommand.ThrownExceptions.SubscribeAndLogException();
            
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
                        return SimpleNavigationService.PushAsync(new ItemsTagItemSetupPageViewModel(Navigator, _tagItemsSelector, next));
                    }
                }
                else
                {
                    return SimpleNavigationService.PushAsync(new ItemsTagItemSetupPageViewModel(Navigator, _tagItemsSelector, CurrentItem));
                }


                return Task.CompletedTask;
            });

            SkipCommand.ThrownExceptions.SubscribeAndLogException();

            BackCommand = ReactiveCommand.CreateFromTask(() =>
            {
                _talkiPlayerManager.Current?.CancelUpload();

                if (_isTagSetupOnly)
                {
                    return SimpleNavigationService.PushAsync(new ItemListPageViewModel(Navigator));
                }

                if (CurrentItem.Type == ItemType.Home)
                {
                    //var navigator = Locator.Current.GetService<INavigationService>(TabItemType.Items.ToString());
                    //navigator.PopToRootPage().SubscribeSafe();
                    SimpleNavigationService.PopToRootAsync().Wait();
                    return SimpleNavigationService.PopModalAsync();
                }

                var previous = _tagItemsSelector.GetPreviousItem();
                
                if (previous != null)
                {
                    return SimpleNavigationService.PushAsync(new ItemsTagItemSetupPageViewModel(Navigator, _tagItemsSelector, previous));
                }


                return Task.CompletedTask;
                
            });
        }

        protected override void UpdateUI(TagItemStatus status, IItem item, bool hasError)
        {
            switch (status)
            {
                 case TagItemStatus.SetupTag:
                     base.UpdateUI(status, item, hasError);
                     if (_isTagSetupOnly)
                     {
                         ShowSkipButton = false;
                     }
                     break;
                case TagItemStatus.ReadTag:
                    base.UpdateUI(status, item, hasError);
                    break;
                case TagItemStatus.TagMappingCompleted:
                    this.Message = "Item has been successfully tagged";
                    ShowGoButton = true;
                    ShowSkipButton = true;
                    GoButtonText = _isTagSetupOnly ?  "Done" : "Next";
                    SkipButtonText = "Add another one";
                   
                    break;
                case TagItemStatus.AllItemsMapped:
                    NavigationTitle = "Setup completed";
                    Heading = $"All Set!";
                    SubHeading = $"You can now see your items";
                    this.Message = "";
                    ShowGoButton = true;
                    ShowSkipButton = false;
                    GoButtonText = "Show me";
                    InstructionImage = Images.FireworksImage;
                    break;
            }
           
        }
    }
}