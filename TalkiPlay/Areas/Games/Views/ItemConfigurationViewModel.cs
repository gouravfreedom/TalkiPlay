using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ChilliSource.Core.Extensions;
using ChilliSource.Mobile.UI.ReactiveUI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace TalkiPlay.Shared
{
    public class ItemConfigurationViewModel : ReactiveObject
    {        
        readonly IGameMediator _gameMediator;
        readonly IUserSettings _userSettings;
        readonly IAssetRepository _assetRepository;

        readonly bool _hasDevice;
        readonly List<ItemSettings> _itemsSettings;
        readonly ItemSettings _currentSettings;
        ReactiveCommand<bool, Unit> _toggleCommand;
        private bool _isEnabled;

        public ItemConfigurationViewModel(IItem item,
            bool hasDevice,
            List<ItemSettings> itemsSettings = null)
        {
            _hasDevice = hasDevice;
            _itemsSettings = itemsSettings;           
            Item = item;
            Name = item?.Name ?? "";
            ImagePath = item.ImagePath;

            _gameMediator = Locator.Current.GetService<IGameMediator>();
            _assetRepository = Locator.Current.GetService<IAssetRepository>();
            _userSettings = Locator.Current.GetService<IUserSettings>();

            if (_itemsSettings != null)
            {
                _currentSettings = itemsSettings.FirstOrDefault(i => i.ItemId == item.Id);
                _isEnabled = _currentSettings == null || _currentSettings.IsActive;

                if (_currentSettings == null)
                {
                    _currentSettings = new ItemSettings(Item.Id, true);
                    itemsSettings.Add(_currentSettings);
                }                
            }

            SetCommands();
        }


        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    _toggleCommand?.Execute(_isEnabled);
                }
            }
        }

        public void InitDefaultState(bool on)
        {
            _isEnabled = on;
        }


        [Reactive]
        public string Name { get; set; }

        public string ImagePath { get; private set; }

        public IItem Item { get; }


        void SetCommands()
        {
            _toggleCommand = ReactiveCommand.CreateFromTask<bool, Unit>(async enabled =>
            {
                if (_hasDevice)
                {
                    Dialogs.ShowLoading("Saving changes...");
                    var room = _gameMediator.CurrentRoom;
                    var tags = _gameMediator.Tags.Where(m => m.ItemIds.Contains(Item.Id)).Distinct().ToList();
                    List<int> tagItems;
                    if (enabled)
                    {
                        var itemTags = room.TagItems.ToList();
                        itemTags.AddRange(tags);
                        tagItems = itemTags.Select(a => a.Id).Distinct().ToList();
                    }
                    else
                    {
                        var tagIds = tags.Select(a => a.Id).ToList();
                        var itemTags = room.TagItems.Where(a => !tagIds.Contains(a.Id)).ToList();
                        tagItems = itemTags.Select(a => a.Id).Distinct().ToList();
                    }
                    var roomDto = new RoomDto(room.Id, tagItems, room.Packs)
                    {
                        Name = room.Name,
                        ImagePath = room.ImagePath
                    };
                    var newRoom = await _assetRepository.UpdateRoomItemTags(roomDto);
                    _gameMediator.CurrentRoom = newRoom;
                    Dialogs.HideLoading();
                }
                else
                {
                    _currentSettings.IsActive = enabled;
                    _userSettings.ItemsSettings = _itemsSettings;
                }
                return Unit.Default;
            });

            _toggleCommand.ThrownExceptions
                .HideLoading()
                .ShowExceptionDialog()
                .SubscribeAndLogException();
        }
    }
}