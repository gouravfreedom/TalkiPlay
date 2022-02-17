using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    public static class Images
    {
        private const string AnimationFolder = "animations/";

        #region Animations
        public const string LoadingAnimation = AnimationFolder + "loader.json";
        public const string ConfettiAnimation = AnimationFolder + "confetti.json";
        public const string ConfettiWithStarAnimation = AnimationFolder + "confetti_with_star.json";
        public const string TrophyAnimation = AnimationFolder + "trophy.json";
        public const string WindowOpenAnimation = AnimationFolder + "window_open.json";
        public static string TpSearch = AnimationFolder + "search.json";
        public static string TpUpdating = AnimationFolder + "updating.json";
        public static string ObActiveLearn = AnimationFolder + "ob_active_learn.json";
        public static string ObPersonal = AnimationFolder + "ob_personal.json";
        public static string ObRealObjects = AnimationFolder + "ob_real_objects.json";
        public static string ObShakeMeTapMe = AnimationFolder + "ob_shake_me_tap_me.json";
        public static string ObSuccess = AnimationFolder + "ob_success.json";
        public static string ObTalkiGuyWelcome = AnimationFolder + "ob_talkiguy_welcome.json";
        public static string ObTagMe = AnimationFolder + "ob_tap_me.json";
        public static string ObPluggedIn = AnimationFolder + "tp_charging.json";
        public static string EggWiggle = AnimationFolder + "egg_wiggle.json";
        public static string WelcomeWithWiggle = AnimationFolder + "welcome_with_wiggle.json";

        #endregion

        #region Images

#if __IOS__
        public const string ImagesFolder = "images/";
#endif

#if __ANDROID__
        public const string ImagesFolder = "";
#endif
        
        public const string GameTabIcon = ImagesFolder + "games_off";
        public const string GameTabSelectedIcon = ImagesFolder + "games_on";
        public const string ItemsTabIcon = ImagesFolder + "items_off";
        public const string ItemsTabSelectedIcon = ImagesFolder + "items_on";
        public const string SettingsTabIcon = ImagesFolder + "settings_off";
        public const string SettingsTabSelectedIcon = ImagesFolder + "settings_on";
        public const string KidsTabIcon = ImagesFolder + "profile_off";
        public const string KidsTabSelectedIcon = ImagesFolder + "profile_on";
        public const string RewardsTabIcon = ImagesFolder + "rewards_off";
        public const string RewardsTabSelectedIcon = ImagesFolder + "rewards_on";
        
        public const string RewardEggUnHatchedImage = ImagesFolder + "reward_egg";        
        public const string RewardUnknownIcon = ImagesFolder + "reward_egg_unhatched";        
        public const string ArrowBackIcon = ImagesFolder + "arrow_back_white";
        public const string ArrowDownIcon = ImagesFolder + "arrow_down_white";
        public const string BleConnectIcon = ImagesFolder + "ble_connect";
        public const string BleDisConnectIcon = ImagesFolder + "ble_disconnect";        
        public const string EditIcon = ImagesFolder + "edit_icon";
        public const string AvatarPlaceHolder = ImagesFolder + "avatar_place_holder";
        public const string PlaceHolder = ImagesFolder + "placeholder";
        public const string LoadingIndicator = ImagesFolder + "loading_indicator";
        public const string LogoButtonIcon = ImagesFolder + "logo";
        public static string AddIcon = ImagesFolder + "add_icon";
        public const string HuntBg = ImagesFolder + "hunt_bg.png";

        //public static readonly string ArrowBackDarkTealIcon = ImagesFolder + "arrow_back_darkteal";
        // public static readonly string OnboardingSelectGame = ImagesFolder + "onboarding_selectgame";
        // public static readonly string OnboardingReady = ImagesFolder + "onboarding_ready";
        //public static string ExploreBg = ImagesFolder + "explore_bg";        
        //public static readonly string RewardsBg = ImagesFolder + "rewards_bg";
        //public static string TalkiExplorerBg = ImagesFolder + "explore_game_bg";
        //public static string TalkiHuntBg = ImagesFolder + "hunt_game_bg";
        //public static string AccountIcon = ImagesFolder + "account_icon";        


        #endregion

        #region Embedded Images

        public const string SvgName = "TalkiPlay.Resources.Svg";
        public const string PngName = "TalkiPlay.Resources.Png";
        public const string JpgName = "TalkiPlay.Resources.Jpg";
        public const string SvgResources = "resource://" + SvgName;
        public const string PngResources = "resource://" + PngName;
        public const string JpgResources = "resource://" + JpgName;

        public const string LogoPng = PngResources + ".logo.png";

        public const string UnSelectedIcon = SvgResources + ".checkbox_default.svg";
        public const string SelectedIcon = SvgResources + ".checkbox_selected.svg";
                
        public const string ArrowIcon = SvgResources + ".arrow_forward_icon.svg";
        public const string ArrowNextWhite = SvgResources + ".arrow_next_white.svg";
        public const string RetryIcon = SvgResources + ".retry_icon.svg";

        public const string ThisIsYourTalkiPlayer = SvgResources + ".ob_01_thisisyourtalkiplayer.svg";
        public const string LogoWhite = SvgResources + ".talkiplay_logo_white.svg";
        public const string AddBlackIcon = SvgResources + ".add_black_icon.svg";
        public const string AddWhiteIcon = SvgResources + ".add_white_icon.svg";
        public const string PlusWhiteIcon = SvgResources + ".plus_white.svg";
        public const string ErrorIcon  = SvgResources + ".error_outline.svg";
        public const string RemoveIcon  = SvgResources + ".remove_circle_outline.svg";
        public const string PlayIcon  = SvgResources + ".play.svg";
        public const string StopIcon  = SvgResources + ".stop.svg";
        public const string DownloadIcon  = SvgResources + ".download_icon.svg";
        public const string TrashIcon  = SvgResources + ".remove_icon.svg";
        public const string ShakeTalkiPlayerImage  = SvgResources + ".shake.svg";
        public const string TapTagTalkiPlayerImage  = SvgResources + ".tapandhold.svg";
        public const string TapToStartGameImage  = SvgResources + ".taptostart.svg";
        public const string TapToEndgameImage  = SvgResources + ".taptoend.svg";
        public const string FireworksImage  = SvgResources + ".fireworks.svg";
        public const string TriangleImage  = SvgResources + ".triangle.svg";
        public const string CollectionImage  = SvgResources + ".collections.svg";
        public const string TreasureChestImage  = SvgResources + ".treasure_chest.svg";
        public const string LockImage  = SvgResources + ".lock_grey.svg";
        
        public const string StartGuideButtonImage  = PngResources + ".button_start_guide.png";
        public const string LetsPlayButtonImage  = PngResources + ".button_lets_play.png";        

        public const string GameBackgroundImage  = JpgResources + ".game_background.jpg";

        public const string TpCharging = SvgResources + ".tp_charging.svg";
        public const string TpCelebrating = SvgResources + ".tp_celebrating.svg";
        public const string TpSad = SvgResources + ".tp_sad.svg";
        public const string TpLightUp = SvgResources + ".tp_light_up.svg";
        public const string PlusIconwhite = SvgResources + ".plus_white.svg";
        public const string SpeechBubblePairMe = SvgResources + ".speech_bubble_pair_me.svg";
        public const string SpeechBubbleExplore = SvgResources + ".speech_bubble_explore.svg";

        public const string StarOn = SvgResources + ".star_on.svg";
        public const string StarOff = SvgResources + ".star_off.svg";

        //public const string PlayButton = SvgResources + ".play_btn.svg";
        //public const string AccountSettingsIcon = SvgResources + ".account_icon.svg";
        public const string Logo = SvgResources + ".talkiplay_logo.svg";
        //public const string HomeButtonIcon = SvgResources + ".home_btn.svg";
        //public const string HelpButtonIcon = SvgResources + ".helper_btn.svg";
        //public const string PowerIcon = SvgResources + ".power.svg";
        //public const string TalkiIcon = SvgResources + ".talki_icon.svg";
        //public const string NextHuntBtn = SvgResources + ".next_hunt_btn.svg";
        //public const string PlayAgainBtn = SvgResources + ".play_again_btn.svg";
        //public const string UploadIcon = SvgResources + ".upload_icon.svg";
        //public const string CloseIcon = SvgResources + ".close_icon.svg";
        //public const string BackIcon = SvgResources + ".arrow_back.svg";
        //public static string DoneIcon  = SvgResources + ".done_outline.svg";

        

        public static string GetSvgImage(string imageName)
        {
            return $"{SvgResources}.{imageName}.svg";
        }

        public static string GetPngImage(string imageName)
        {
            return $"{PngResources}.{imageName}.png";
        }

        public static ImageSource GetEmbeddedPngImage(string imageName)
        {
            return ImageSource.FromResource(
                imageName,
                Assembly.GetExecutingAssembly()
            );
        }
        
        
        #endregion
    }
}