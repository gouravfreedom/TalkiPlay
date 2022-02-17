using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Plugin.Settings;
using TalkiPlay.Shared;

namespace TalkiPlay
{
    public interface IUserSettings
    {
        string UniqueId { get; set; }
        
        List<ItemSettings> ItemsSettings { get; set; }
      
        
        void ClearSession();
        GameSession CurrentGameSession { get; set; }
        QRGameSession CurrentQRGameSession { get; set; }

        bool ReloadGameSession { get; set; }

        void ClearGameSession();
        
        bool IsOnboarded { get;}
        bool IsDeviceOnboarded { get; set; }
        bool IsQrOnboarded { get; set; }

        bool IsGuideCompleted { get; set; }

        bool HasTalkiPlayerDevice { get; set; }

        bool PlayInGameMusic { get; set; }
        
        List<RecommendedGame> RecommendedGames { get; set; }
        
        string ReaderDeviceId { get; set; }

        IChild CurrentChild { get; set; }
    }
    
    public class UserSettings : IUserSettings
    {
        
        private const string UniqueIdKey = "UniqueId";
        public string UniqueId
        {
            get => CrossSettings.Current.GetValueOrDefault(UniqueIdKey, null);
            set => CrossSettings.Current.AddOrUpdateValue(UniqueIdKey, value);
        }
        
        private const string ItemsSettingsKey = "ItemsSettingsKey";

        public List<ItemSettings> ItemsSettings
        {
            get
            {
                var r = CrossSettings.Current.GetValueOrDefault(ItemsSettingsKey, null);
                return !String.IsNullOrWhiteSpace(r) ? JsonConvert.DeserializeObject<List<ItemSettings>>(r) : new List<ItemSettings>();
            }
            set => CrossSettings.Current.AddOrUpdateValue(ItemsSettingsKey, JsonConvert.SerializeObject(value));
        }
        
        public void ClearSession()
        {
            CrossSettings.Current.Remove(GameSessionKey);
            CrossSettings.Current.Remove(QRGameSessionKey);
            CrossSettings.Current.Remove(RecommendedGamesKey);            
        }
        
        private const string GameSessionKey = "GameSession";

        public GameSession CurrentGameSession
        {
            get
            {
                var d = CrossSettings.Current.GetValueOrDefault(GameSessionKey, null);
                return !String.IsNullOrWhiteSpace(d) ? JsonConvert.DeserializeObject<GameSession>(d) : null;
            }
            set
            {
                if (value != null)
                {
                    CrossSettings.Current.AddOrUpdateValue(GameSessionKey, JsonConvert.SerializeObject(value));
                }
            }
        }

        private const string QRGameSessionKey = "QRGameSession";

        public QRGameSession CurrentQRGameSession
        {
            get
            {
                var d = CrossSettings.Current.GetValueOrDefault(QRGameSessionKey, null);
                return !String.IsNullOrWhiteSpace(d) ? JsonConvert.DeserializeObject<QRGameSession>(d) : null;
            }
            set
            {
                if (value != null)
                {
                    CrossSettings.Current.AddOrUpdateValue(QRGameSessionKey, JsonConvert.SerializeObject(value));
                }
            }
        }
        
        private const string ReloadGameSessionKey = "ReloadGameSession";

        public bool ReloadGameSession
        {
            get => CrossSettings.Current.GetValueOrDefault(ReloadGameSessionKey, true);
            set => CrossSettings.Current.AddOrUpdateValue(ReloadGameSessionKey, value);
        }

        public void ClearGameSession()
        {
            CrossSettings.Current.Remove(GameSessionKey);
            CrossSettings.Current.Remove(QRGameSessionKey);
        }

        public bool IsOnboarded
        {
            get => (HasTalkiPlayerDevice && IsDeviceOnboarded) || (!HasTalkiPlayerDevice && IsQrOnboarded);
        }

        private const string IsDeviceOnboardedKey = "IsDeviceOnboarded";
        public bool IsDeviceOnboarded 
        {
            get => CrossSettings.Current.GetValueOrDefault(IsDeviceOnboardedKey, false);
            set => CrossSettings.Current.AddOrUpdateValue(IsDeviceOnboardedKey, value);
        }

        private const string IsQrOnboardedKey = "IsQrOnboarded";
        public bool IsQrOnboarded
        {
            get => CrossSettings.Current.GetValueOrDefault(IsQrOnboardedKey, false);
            set => CrossSettings.Current.AddOrUpdateValue(IsQrOnboardedKey, value);
        }

        private const string IsGuideCompletedKey = "IsGuideCompleted";
        public bool IsGuideCompleted
        {
            get => CrossSettings.Current.GetValueOrDefault(IsGuideCompletedKey, false);
            set => CrossSettings.Current.AddOrUpdateValue(IsGuideCompletedKey, value);
        }
        
        private const string HasTalkiPlayerDeviceKey = "HasTalkiPlayerDevice";
        public bool HasTalkiPlayerDevice
        {
            get => CrossSettings.Current.GetValueOrDefault(HasTalkiPlayerDeviceKey, false);
            set => CrossSettings.Current.AddOrUpdateValue(HasTalkiPlayerDeviceKey, value);
        }

        private const string PlayInGameMusicKey = "PlayInGameMusic";
        public bool PlayInGameMusic
        {
            get => CrossSettings.Current.GetValueOrDefault(PlayInGameMusicKey, false);
            set => CrossSettings.Current.AddOrUpdateValue(PlayInGameMusicKey, value);
        }
        
        private const string RecommendedGamesKey = "RecommendedGames";
        public List<RecommendedGame> RecommendedGames
        {
            get
            {
                var r = CrossSettings.Current.GetValueOrDefault(RecommendedGamesKey, null);
                return !String.IsNullOrWhiteSpace(r) ? JsonConvert.DeserializeObject<List<RecommendedGame>>(r) : new List<RecommendedGame>();
            }
            set => CrossSettings.Current.AddOrUpdateValue(RecommendedGamesKey, JsonConvert.SerializeObject(value));
        }
        
        
        private const string ReaderDeviceIdKey = "ReaderDeviceId";
        public string ReaderDeviceId
        {
            get => CrossSettings.Current.GetValueOrDefault(ReaderDeviceIdKey, null);
            set => CrossSettings.Current.AddOrUpdateValue(ReaderDeviceIdKey, value);
        }

        private const string CurrentChildKey = "CurrentChild";
        public IChild CurrentChild
        {
            get
            {
                var r = CrossSettings.Current.GetValueOrDefault(CurrentChildKey, null);
                return !String.IsNullOrWhiteSpace(r) ? JsonConvert.DeserializeObject<ChildDto>(r) : null;
            }
            set => CrossSettings.Current.AddOrUpdateValue(CurrentChildKey, JsonConvert.SerializeObject(value));
        }
    }
}