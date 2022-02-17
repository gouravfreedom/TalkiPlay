using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public enum GameSessionStatus
    {
        [Description("Waiting ...")]
        Pending,
        [Description("Playing ...")]
        WaitingForDevice,
        [Description("Playing ...")]
        WaitingForResult,
        [Description("Rewards Ceremony...")]
        RewardsCeremony,
        [Description("Completed")]
        Completed,
        [Description("Failed")]
        Failed
    }

    public interface IPlayerWithDevice
    {
        IList<IChild> Children { get; }
        Guid DeviceId { get; }
        
        string DeviceName { get; }
        
        DateTime GameTime { get;  }
    }

    public class DeviceWithPlayers : IPlayerWithDevice
    {
        public IList<IChild> Children { get; set; }
        public Guid DeviceId { get; set; }
        public string DeviceName { get; set; }
        public DateTime GameTime { get; set; }
    }

    public class GameSessionRecord
    {
        public IList<int> ChildId { get; set; }
        public string DeviceName { get; set; }
        public Guid DeviceId { get; set; }
        
        public DateTime GameStartOn { get; set; }

        public DateTime? GameEndOn { get; set; }
        public GameSessionStatus Status { get; set; } = GameSessionStatus.Pending;
    }

   
    public class GameSession
    { 
        public Guid GameSessionId { get; set; }
        public int GameId { get; set;}
        public int RoomId { get; set; }
        public DateTime TimeStamp { get; set; }

        public IList<GameSessionRecord> GameSessionRecords { get; set; }
        
        public static GameSession Create(Guid gameSessionId, IGame game, IRoom room, IList<IPlayerWithDevice> players)
        {
            return new GameSession()
            {
                GameSessionId = gameSessionId,
                GameId = game.Id,
                RoomId = room.Id,
                TimeStamp = DateTime.Now,
                GameSessionRecords = players.Select(m => new GameSessionRecord()
                {
                    ChildId = m.Children.Select(a => a.Id).ToList(),
                    DeviceId = m.DeviceId,
                    DeviceName = m.DeviceName,
                    GameStartOn = m.GameTime
                }).ToList()
            };
        }
    }

    public class QRGameSession
    {
        public Guid GameSessionId { get; set; }
        public DateTime TimeStamp { get; set; }

        public int GameId { get; set; }
        //public int RoomId { get; set; }
        public int PackId { get; set; }
        public IList<int> ChildrenIds { get; set; }

        public IList<GameSessionItemResult> Results { get; set; }
        public GameType GameType { get; set; }
        public DateTime GameStartTime { get; set; }

        public bool IsHuntGameCompleted { get; set; }
        public int HuntInstructionIndex { get; set; }
        public DateTime HuntInstructionStartTime { get; set; }
        public int HuntInstructionFailureCount { get; set; }
        public List<int> HuntInstructionsOrder { get; set; }
    }

    public class GameData
    {
        public GameData()
        {
            
        }
        
        public GameData(string gameSessionId, IGame game, IList<ITag> tags, IList<AssetDto> assets, IList<ItemDto> items, IList<ITag> roomTags)
        {
            GameTime = DateTime.Now;
            GameSessionId = gameSessionId;
            Name = game.Name;
            GameType = game.Type;
            var roomTagItemIds = roomTags.SelectMany(a => a.ItemIds).Distinct().ToList();
            var itemIds = tags.SelectMany(a => a.ItemIds).Distinct().ToList();

            ErrorAudioFile = game.Sounds?.FirstOrDefault(m => m.Type == SoundType.Error)?.Asset?.Filename;
            StarGameAudioFile = game.Sounds?.FirstOrDefault(m => m.Type == SoundType.StartGame)?.Asset?.Filename;
            CorrectScanAudioFile = game.Sounds?.FirstOrDefault(m => m.Type == SoundType.CorrectScan)?.Asset?.Filename;
            WrongScanAudioFile = game.Sounds?.FirstOrDefault(m => m.Type == SoundType.WrongScan)?.Asset?.Filename;
            HuntFinishedAudioFile = game.Sounds?.FirstOrDefault(m => m.Type == SoundType.HuntFinish)?.Asset?.Filename;
            VictoryAudioFile = game.Sounds?.FirstOrDefault(m => m.Type == SoundType.Victory)?.Asset?.Filename;

            if (game.Instructions != null && game.Instructions.Count > 0)
            {
                var instructions = game.Instructions
                    .Where(instruction => itemIds.Contains(instruction.Item.Id))
                    .Where(instruction => roomTagItemIds.Contains(instruction.Item.Id))
                    .Select(a => new InstructionData(a, tags, assets)).ToList();

                Instructions = game.InstructionsAreRandom ? instructions.Shuffle().ToList() : instructions;
            }
            else
            {
                Instructions = items.Where(a => a.Type == ItemType.Default)
                                    .Where(a => itemIds.Contains(a.Id))
                                    .Where(a => roomTagItemIds.Contains(a.Id))
                                    .Select(a => new InstructionData(a, tags, assets)).ToList();
            }

            var item = items.FirstOrDefault(a => a.Type == ItemType.Home);

            if (item != null)
            {
                HomeTags = tags.Where(a => a.ItemIds.Contains(item.Id)).Select(a => a.SerialNumber).ToList();
            }
        }
        
        
       [JsonProperty("sessionId")]
       public string GameSessionId { get; set; }
        
       [JsonProperty("name")]
       public string Name { get; set; }

       [JsonProperty("gameType")]
       public GameType GameType { get; set; }
       
       [JsonProperty("items")]
       public IList<InstructionData> Instructions { get; set; }
      
       
       [JsonProperty("homeTags")]
       public IList<string> HomeTags { get; set; }
       
       [JsonProperty("time")]
       public DateTime GameTime { get; set; }
       
       [JsonProperty("errorAudioFile")]
       public string ErrorAudioFile { get; set; }
       
       [JsonProperty("startGameAudioFile")]
       public string StarGameAudioFile { get; set; }

       [JsonProperty("correctScanAudioFile")]
       public string CorrectScanAudioFile { get; set; }

       [JsonProperty("wrongScanAudioFile")]
       public string WrongScanAudioFile { get; set; }

       [JsonProperty("huntFinishedAudioFile")]
       public string HuntFinishedAudioFile { get; set; }
       
       [JsonProperty("victoryAudioFile")]
       public string VictoryAudioFile { get; set; }

    }

    public class InstructionData
    {
        public InstructionData()
        {
            
        }

        public InstructionData(IInstruction instruction, IList<ITag> tags, IList<AssetDto> assets)
        {
            var mode = instruction.Modes.FirstOrDefault(a => a.Type == InstructionModeType.Receptive);

            if (mode != null && mode.AudioAssetId != null)
            {
                Instruction = new InstructionModeData(mode, assets);
            }
            else
            {
                mode = instruction.Modes.FirstOrDefault(a => a.Type == InstructionModeType.Expressive);
                if(mode != null && mode.AudioAssetId != null)
                {
                    Instruction = new InstructionModeData(mode, assets);
                }
            }


            var reward = instruction.Modes.FirstOrDefault(a => a.Type == InstructionModeType.Reward);

            if (reward?.AudioAssetId != null)
            {
                var rewardAssets = assets.Where(a => a.Id == reward.AudioAssetId.Value);
                Item = new ItemData(instruction.Item, tags, new List<IAsset>())
                {
                    AudioAssets = rewardAssets.Select(a => a.Filename).ToList(),
                    AudioAssetIds = rewardAssets.Select(a => a.Id).ToList()

                };
            }
            
        }
        
        public InstructionData(IItem item, IList<ITag> tags, IList<AssetDto> assets)
        {
            Item = new ItemData(item, tags, assets);
        }
        
        [JsonProperty("item")]
        public ItemData Item { get; set; }
        
        [JsonProperty("instruction")]
        public InstructionModeData Instruction { get; set; }
    }

    public class InstructionModeData
    {
        public InstructionModeData()
        {
            
        }
        
        public InstructionModeData(IMode mode, IEnumerable<IAsset> assets)
        {
            Type = mode.Type;
            var asset = assets.FirstOrDefault(a => a.Id == mode.AudioAssetId);

            if (asset != null)
            {
                AudioAsset = asset.Filename;
                AudioAssetId = asset.Id;
            }
        }
        
       [JsonIgnore]
        public InstructionModeType Type { get; set; }

        [JsonProperty("audioFile")]
        public string AudioAsset { get; set; }

        [JsonIgnore]
        public int AudioAssetId { get; set; }
        
    }
    
    public class ItemData
    {
        public ItemData()
        {
            
        }

        public ItemData(IItem item, IEnumerable<ITag> tags, IEnumerable<IAsset> assets)
        {
            Id = item.Id;
            QRCode = item.QRCode;
            Tags = tags.Where(a => a.ItemIds.Contains(item.Id)).Select(a => a.SerialNumber).ToList();

            var itemAssets = assets.Where(a => item.AudioAssetIds.Contains(a.Id));

            AudioAssets = itemAssets.Select(a => a.Filename).ToList();
            AudioAssetIds = itemAssets.Select(a => a.Id).ToList();
        }
        
     
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonIgnore]
        public string QRCode { get; set; }

        [JsonProperty("tags")]
        public IList<string> Tags { get; set; }
        
        [JsonProperty("audioFiles")]
        public IList<string> AudioAssets { get; set; }

        [JsonIgnore]
        public IList<int> AudioAssetIds { get; set; }
    }

}