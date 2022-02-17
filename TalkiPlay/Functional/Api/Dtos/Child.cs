using System;
using System.Collections.Generic;
using System.IO;
using ChilliSource.Mobile.Core;
using Newtonsoft.Json;
using Refit;

namespace TalkiPlay.Shared
{
    public class ChildDto : IChild
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("imageAssetId")]
        public int? AssetId { get; set;  }

        [JsonProperty("birthDay")]
        public DateTime? DateOfBirth { get; set; }

        public int Age => DateOfBirth != null ? (int) (DateTime.Today.Subtract(DateOfBirth.Value).TotalDays / 365) : -1;

        [JsonProperty("photoPath")]
        public string PhotoPath { get; set; }
        
        [JsonProperty("communicationLevel")]
        public ChildCommunicationLevel? CommunicationLevel { get; set; }
        
        [JsonProperty("responseLevel")]
        public ChildResponseLevel? ResponseLevel { get; set; }
        
        [JsonProperty("languageLevel")]
        public ChildLanguageLevel? LanguageLevel { get; set; }
        
        [JsonProperty("games")]
        public List<ChildPlayedGame> PlayedGames { get; set; }
        
        [JsonProperty("favouritePackId")]
        public int? FavouritePackId { get; set; }


        [JsonIgnore]
        public string UIAssetPath { get; set; }
    }

    public class ChildPlayedGame
    {
        [JsonProperty("id")]
        public int GameId { get; set; }
        
        [JsonProperty("items")]
        public List<ChildCollectedGameItem> Items { get; set; }
    }

    public class ChildCollectedGameItem
    {
        [JsonProperty("id")]
        public int ItemId { get; set; } 
        
        [JsonProperty("isCollected")]
        public bool IsCollected { get; set; } 
    }
    
    public class AddUpdateChildRequest 
    {
        public AddUpdateChildRequest()
        {
            
        }

        public AddUpdateChildRequest(IChild child)
        {
            Birthday = child.DateOfBirth;
            Name = child.Name;
            AssetId = child.AssetId ?? 0;
            CommunicationLevel = child.CommunicationLevel;
            LanguageLevel = child.LanguageLevel;
            ResponseLevel = child.ResponseLevel;
            FavouritePackId = child.FavouritePackId;
        }
        
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("imageAssetId")]
        public int AssetId { get; set; }

        [JsonProperty("birthDay")]
        public DateTime? Birthday { get; set; }

        [JsonProperty("isActive")]
        public bool IsActive { get; set; } = true;
        
        [JsonProperty("communicationLevel")]
        public ChildCommunicationLevel? CommunicationLevel { get; set; }
        
        [JsonProperty("responseLevel")]
        public ChildResponseLevel? ResponseLevel { get; set; }
        
        [JsonProperty("languageLevel")]
        public ChildLanguageLevel? LanguageLevel { get; set; }
        
        [JsonProperty("favouritePackId")]
        public int? FavouritePackId { get; set; }
    }
    
    public enum ChildCommunicationLevel
    {
        [Description("No words")] 
        NoWords = 0,
        [Description("A few words")] 
        AFewWords,
        [Description("Descriptive words (red ball)")]
        DescriptiveWords,
        [Description("Descriptive plus context (My red ball)")]
        DescriptivePlusContext
    }

    public enum ChildResponseLevel
    {
        [Description("My child would never do that if asked")]
        Never = 0,
        [Description("Maybe half the time")]
        HalfTheTime,
        [Description("It would take me asking 5 times")]
        AskingMultipleTimes,
        [Description("My child would sit at the table")]
        AllTheTime
    }
    
    public enum ChildLanguageLevel
    {
        [Description("Yes - we only speak English")]
        EnglishOnly = 0,
        [Description("Yes - we speak English and another language(s)")]
        EnglishAndOther,
        [Description("No - English is our second language")]
        EnglishAsSecondLanguage,
        [Description("No - We are hoping to start learning English")]
        EnglishStarting
    }
    
}