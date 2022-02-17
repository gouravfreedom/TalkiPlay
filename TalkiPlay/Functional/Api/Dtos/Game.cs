using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class GameDto : IGame
    {
      
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("imagePath")]
        public string ImagePath { get; set; }

        [JsonProperty("type")]
        public GameType Type { get; set; }

        [JsonProperty("level")]
        public GameLevel Level { get; set; }

        [JsonProperty("access")]
        public GameAccessLevel AccessLevel { get; set; }
        
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("packId")]
        public int PackId { get; set; }

        [JsonProperty("instructions")]
        public IList<InstructionDto> InstructionList { get; set; }

        [JsonIgnore]
        public IList<IInstruction> Instructions => InstructionList?.ToList<IInstruction>();

        // [JsonProperty("items")]
        // public IList<ItemDto> ItemList { get; set; }
        //
        // [JsonIgnore]
        // public IList<IItem> Items => ItemList?.ToList<IItem>();

        [JsonProperty("sounds")]
        public IList<SoundDto> SoundList { get; set; }

        [JsonProperty("categoryIds")]
        public IList<int> Categories { get; set; }

        [JsonIgnore]
        public IList<ISound> Sounds => SoundList?.ToList<ISound>();

        [JsonProperty("chest")]
        public ChestDto ChestActual { get; set; }
 
        [JsonIgnore]
        public IChest Chest => ChestActual;

        [JsonProperty("instructionsAreRandom")]
        public bool InstructionsAreRandom { get; set; }
        
        [JsonIgnore]
        public bool IsRecommended { get; set; }

        
    }
    
    public enum GameLevel
    {
        Early,
        Learning,
        Conceptual
    }

    public enum GameAccessLevel
    {
        Free,
        Subscription
    }
    
    public enum GameType
    {
        [Description("TalkiExplore")]
        Explore = 1,
        [Description("TalkiHunt")]
        Hunt
    }

    public enum ItemResultStatus
    {
        None = 0,
        Success = 1,
        Failure = 2
    }
    
    public class GameSessionItemResult
    {

        [JsonProperty("itemId")]
        public int ItemId { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        [JsonProperty("status")]
        public ItemResultStatus Status { get; set; }

        [JsonProperty("failureCount")]
        public int FailureCount { get; set; }
        
        [JsonProperty("isCollected")]
        public bool IsCollected { get; set; }
      
    }
    

    public class RecordGameSessionResultRequest
    {
        public RecordGameSessionResultRequest()
        {
            Results = new List<GameSessionItemResult>();
        }

        [JsonProperty("gameId")]
        public int GameId { get; set; }

        [JsonProperty("roomId")]
        public int? RoomId { get; set; }

        [JsonProperty("childIds")]
        public IEnumerable<int> Children { get; set; }

        [JsonProperty("startTime")]
        public DateTime StartTime { get; set; }

        [JsonProperty("endTime")]
        public DateTime EndTime { get; set; }

        [JsonProperty("results")]
        public IEnumerable<GameSessionItemResult> Results { get; set; }
        
        [JsonProperty("deviceName")]
        public string DeviceName { get; set; }
    }

    public class RecommendedGame
    {
        public RecommendedGame()
        {
            
        }

        public RecommendedGame(int gameId, int childId)
        {
            GameId = gameId;
            ChildId = childId;
        }
        public int GameId { get; set; }
        public int ChildId { get; set; }
    }

}