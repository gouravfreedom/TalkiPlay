using System;

namespace TalkiPlay.Shared
{
    public static class Constants
    {
        public const int NumberOfConcurrentDownload = 2;
        public const int NumberOfRetry = 3;
        public const string MainNavigation = "MainNavigation";
        public const string ImageResizerParameter = "format=jpg&scale=both&quality=90&autorotate=true";
        public const float SmallMusicVolume = 0.1f;
        public const float SmallMusicSilentVolume = 0.01f;
        public static AudioPlayerSetting HuntMusic = new AudioPlayerSetting(Audio.HuntMusic, numberOfLoops: -1, volume:SmallMusicVolume);
        public static AudioPlayerSetting ExploreMusic = new AudioPlayerSetting(Audio.ExploreMusic, numberOfLoops: -1, volume: SmallMusicVolume);
        public static int NavigationBarHeight = 48;
        public static string DeviceName = "Talkiplayer";
        public static string DeviceId = "TalkiPlay";
        public static TimeSpan TimeoutUntilShowTalkiPlayActiviationInstruciton = TimeSpan.FromMinutes(1);

        public const string SubscriptionLegalText  = "Your payment will be charged to your iTunes Account at confirmation of purchase." +
                                                     "\n\nThe subscription automatically renews unless auto-renew is turned off at least " +
                                                     "24-hours before the end of the current period.\n\nYour account will be charged for " +
                                                     "renewal within 24-hours prior to the end of the current period, and identify the cost " +
                                                     "of the renewal.\n\nYou can manage or turn off auto-renew by going to your Apple ID " +
                                                     "Account Settings any time after your purchase.";
        
    }
}