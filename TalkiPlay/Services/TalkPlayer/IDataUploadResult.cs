using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public interface IDataUploadResult
    {
        UploadDataType Type { get;  }
        string Tag { get; }
        bool IsSuccess { get; }
        IDataResult Data { get; }

        T GetData<T>() where T : IDataResult;
    }

    public class DataUploadResult : IDataUploadResult
    {
        public DataUploadResult(UploadDataType type, string tag, bool isSuccess, IDataResult data)
        {
            Type = type;
            Tag = tag;
            IsSuccess = isSuccess;
            Data = data;
        }
        
        public UploadDataType Type { get; private set; }
        public string Tag { get; private set; }
        public bool IsSuccess { get; private set;}
        public IDataResult Data { get; private set;}
        
        public T GetData<T>() where T : IDataResult
        {
            return Data is T ? (T) Data :  default(T);
        }
        public static IDataUploadResult Failed(string tag,UploadDataType type = UploadDataType.Unknown, Exception ex = null) => new DataUploadResult(type, tag, false, new ErrorDataResult(ex));
    }

    public interface IDataResult
    {
        
    }

    public class EmptyResult : IDataResult
    {
        
    }
    
    public class ErrorDataResult : IDataResult
    {
        public ErrorDataResult(Exception ex)
        {
            Exception = ex;
        }
        
        public Exception Exception { get; }
    }
    
    public class HardwareErrorDataResult : IDataResult
    {
        [JsonProperty("errorCode")]
        public string ErrorCode { get; set; }

        [JsonProperty("errorMessage")] 
        public string Message { get; set; }

    }
    
    public class GameResult : IDataResult
    {
        public GameResult()
        {
            ItemResults = new List<ItemResultData>();
        }
        
        [JsonProperty("sessionId")]
        public string GameSessionId { get; set; }

        [JsonProperty("itemResults")] 
        public IList<ItemResultData> ItemResults { get; set; }
        
        [JsonProperty("lapsedTime")]
        public long ElapsedTime { get; set; }
    }

    public class TagData : IDataResult
    {
        [JsonProperty("serial")]
        public string SerialNumber { get; set; }
    }
    
    public class ItemResultData
    {
        
        [JsonProperty("itemId")]
        public int ItemId { get; set; }
        
        [JsonProperty("lapsedTime")]
        public long ElapsedTime { get; set; }
        
        [JsonProperty("isSuccess")]
        public bool IsSuccess { get; set; }
        
        [JsonProperty("noOfFailedAttempts")]
        public int NumberOfFailedAttempts { get; set; }

    }

    public class AvailableAudioFiles : IDataResult
    {
        public AvailableAudioFiles()
        {
            AudioFiles = new List<string>();
        }
        
        [JsonProperty("audioFiles")]
        public IList<string> AudioFiles { get; set; }
      
    }
    
    public class TalkiPlayerDeviceInfo : IDataResult
    {

        [JsonProperty("firmwareVersion")]
        public string FirmwareVersion { get; set; }

        [JsonProperty("hardwareVersion")]
        public string HardwareVersion { get; set; }

    }

    public class VolumeInfo : IDataResult
    {
        [JsonProperty("volume")]
        public int Volume { get; set; }
    }
}