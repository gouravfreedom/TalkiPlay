using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ChilliSource.Mobile.UI.ReactiveUI;
using DynamicData;

namespace TalkiPlay.Shared
{

    public class EmptyChild : IChild
    {
        public int Id => 0;
        public string Name => "Add Child";
        public string FirstName => "";
        public string LastName => "";
        public string PhotoPath => "";
        public int? AssetId => 0;
        public DateTime? DateOfBirth => null;
        public int Age => 0;
        
        public ChildCommunicationLevel? CommunicationLevel { get; set; }

        public ChildResponseLevel? ResponseLevel { get; set; }
       
        public ChildLanguageLevel? LanguageLevel { get; set; }
        
        public int? FavouritePackId { get; set; }
    }

    public class EmptyTalkiPlayerData : ITalkiPlayerData
    {
        public string Name { get; set; }
        public Guid DeviceId { get; set; }
        public DateTime Time { get; set; }

        public ITalkiPlayer Device { get; set; }
    }

    public interface ITalkiPlayerData
    {
        string Name { get; }
        Guid DeviceId { get; }
        
        DateTime Time { get;  }
        
        ITalkiPlayer Device { get; }
    }

    public class TalkiPlayerData : ITalkiPlayerData
    {
        public string Name { get; set; }
        public Guid DeviceId { get; set; }
        public DateTime Time { get; set; }
        
        public ITalkiPlayer Device { get; set; }
    }
    
    public interface IGameMediator : IDisposable
    {
        IGame CurrentGame { get; set; }
        IRoom CurrentRoom { get; set; }
        IList<IItem> HomeTags { get; set; }
        IPack CurrentPack { get; set; }
        ISourceList<IChild> Children { get;  }
        IList<ITag> Tags { get; set; }
        ISourceList<ITalkiPlayerData> Devices { get; }
        IList<ITalkiPlayerData> TalkiPlayers { get; }
        IList<IPlayerWithDevice> DeviceWithPlayers { get; }

        void SetGameSessionId(Guid sessionId);
        
        Guid GameSessionId { get; }

        void Reset();
        

    }

    public class GameMediator : IGameMediator
    {
        readonly SourceList<IChild> _children = new SourceList<IChild>();
        readonly SourceList<ITalkiPlayerData> _talkiPlayers = new SourceList<ITalkiPlayerData>();
        readonly CompositeDisposable _disposable = new CompositeDisposable();

        private Guid _gameSessionId;

        public GameMediator()
        {
            _children.Add(new EmptyChild());
            _talkiPlayers.Add(new EmptyTalkiPlayerData());

            _talkiPlayers.Connect()
                .Filter(m => !(m is EmptyTalkiPlayerData))
                .Transform(device =>
                {
                    var data = new DeviceWithPlayers()
                    {
                        Children = _children.Items.NotEmpty().ToList(),
                        DeviceId = device.DeviceId,
                        DeviceName = device.Name,
                        GameTime = device.Time
                    } as IPlayerWithDevice;

                    return data;
                })
                .Bind(out var gameSessions)
                .SubscribeSafe()
                .DisposeWith(_disposable);

            DeviceWithPlayers = gameSessions;

        }

        public IGame CurrentGame { get; set; }

        public IRoom CurrentRoom { get; set; }

        public IPack CurrentPack { get; set; }

        public IList<ITag> Tags { get; set; }
        public ISourceList<IChild> Children => _children;

        public ISourceList<ITalkiPlayerData> Devices => _talkiPlayers;

        public IList<ITalkiPlayerData> TalkiPlayers => _talkiPlayers.Items.NotEmpty().ToList();

        public IList<IPlayerWithDevice> DeviceWithPlayers { get; }
        public void SetGameSessionId(Guid sessionId)
        {
            _gameSessionId = sessionId;
        }

        public Guid GameSessionId => _gameSessionId;

        public IList<IItem> HomeTags { get; set; } = new List<IItem>();

        public void Reset()
        {
            CurrentGame = null;
            CurrentRoom = null;
            CurrentPack = null;
            _children.Clear();
            _talkiPlayers.Clear();
            Devices.Clear();
            SetGameSessionId(Guid.Empty);
            _children.Add(new EmptyChild());
            _talkiPlayers.Add(new EmptyTalkiPlayerData());
        }

        public void Dispose()
        {
            _disposable.Clear();
        }
    }

    public static class GameMediatorExtension
    {
        public static IEnumerable<ITalkiPlayerData> NotEmpty(this IEnumerable<ITalkiPlayerData> data)
        {
            return data.Where(a => !(a is EmptyTalkiPlayerData));
        } 
        
        public static IEnumerable<IChild> NotEmpty(this IEnumerable<IChild> data)
        {
            return data.Where(a => !(a is EmptyChild));
        } 
    }
}