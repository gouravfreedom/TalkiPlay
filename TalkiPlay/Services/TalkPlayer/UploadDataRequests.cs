using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xamarin.Forms;

namespace TalkiPlay.Shared
{
    
    public enum TalkiplayerCommands {
        get_game_results,
        get_audio_file_list,
        factory_reset,
        erase_flash,
        device_info,
        update_volume,
        delete_audio_files,
        volumne_info,
        deep_sleep
    }
    
    public class DataRequest
    {
        [JsonProperty("command")]
        public string Command { get; set; }
        public static DataRequest VolumeRequest(int volume = 50) => new VolumeRequest(volume);
        public static DataRequest DeleteAudioFileRequest(List<string> audioFiles, bool deleteAll = false) => new DeleteAudioFileRequest(audioFiles, deleteAll);

        public static DataRequest GetAudioFileListRequest() => new DataRequest()
        {
            Command = TalkiplayerCommands.get_audio_file_list.ToString()
        };
        public  static DataRequest FactoryResetRequest() => new DataRequest()
        {
            Command = TalkiplayerCommands.factory_reset.ToString()
        };
        public static DataRequest DeviceInfoRequest() => new DataRequest()
        {
            Command = TalkiplayerCommands.device_info.ToString()
        };
        public static DataRequest EraseFlashRequest() => new DataRequest()
        {
            Command = TalkiplayerCommands.erase_flash.ToString()
        };
        
        public static DataRequest VolumeInfoRequest() => new DataRequest()
        {
            Command = TalkiplayerCommands.volumne_info.ToString()
        };
        
        public static DataRequest GameResultRequest() => new DataRequest()
        {
            Command = TalkiplayerCommands.get_game_results.ToString()
        };

        public static DataRequest DeepSleepRequest() => new DataRequest()
        {
            Command = TalkiplayerCommands.deep_sleep.ToString()
        };
    }
    
    public class VolumeRequest : DataRequest
    {
        public VolumeRequest(int volume = 50)
        {
            Command = TalkiplayerCommands.update_volume.ToString();
            Volume = volume;
        }
        
        [JsonProperty("volume")]
        public int Volume { get; set; }
        
        
    }
    public class DeleteAudioFileRequest : DataRequest
    {
        public DeleteAudioFileRequest()
        {
            AudioFiles = new List<string>();
            Command  = TalkiplayerCommands.delete_audio_files.ToString();
        }

        public DeleteAudioFileRequest(List<string> audioFiles, bool deleteAll)
        {
            AudioFiles = audioFiles;
            Command = TalkiplayerCommands.delete_audio_files.ToString();
            DeleteAll = deleteAll;
        }

        [JsonProperty("deleteAll")]
        public bool DeleteAll { get; set; }
        
        [JsonProperty("audioFiles")]
        public IList<string> AudioFiles { get; set; }

    }
    
}