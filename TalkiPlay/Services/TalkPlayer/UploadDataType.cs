using ChilliSource.Mobile.Core;

namespace TalkiPlay.Shared
{
    public enum UploadDataType
    {
       
        [Description("Unknown")]
        Unknown,
        [Description("Meta data")]
        MetaData,
        [Description("Firmware")]
        Firmware,
        [Description("Audio")]
        Audio,
        [Description("Game data")]
        GameData,
        [Description("Game result")]
        GameResult,
        [Description("Tag data")]
        TagData,
        [Description("Volume")]
        Volume,
        [Description("Get Available Audio Files")]
        AvailableAudioFiles,
        [Description("Delete Audio Files")]
        AudioDelete,
        [Description("Factory reset")]
        FactoryReset,
        [Description("Erase flash memory")]
        EraseFlash,
        [Description("Get Device information")]
        DeviceInfo,
        [Description("Get Current Volume")]
        VolumeInfo,
        [Description("Goto Deep Sleep")]
        DeepSleep
    }
}