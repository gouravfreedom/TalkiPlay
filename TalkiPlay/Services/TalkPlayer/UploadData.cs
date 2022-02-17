using System;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public interface IUploadData
    {
        string Name { get; }
        string Tag { get; }
        UploadDataType Type { get; }
    }

    public interface IFileUploadData : IUploadData
    {
        string Path { get; }
        long Size { get; }
        string Checksum { get; }

        bool IsLast { get; }

    }

    public interface IDataUploadData : IUploadData
    {
        object Data { get; }
        string DataJson { get; }
    }

    public class DataUploadData : IDataUploadData
    {
        public DataUploadData(
            string name,
            object data,
            string tag,
            UploadDataType type
        )
        {
            Name = name;
            Data = data;
            Tag = tag ?? Guid.NewGuid().ToString();
            Type = type;
        }

        public DataUploadData(
            string name,
            string json,
            string tag,
            UploadDataType type
        )
        {
            Name = name;
            DataJson = json;
            Tag = tag ?? Guid.NewGuid().ToString();
            Type = type;
        }
        
        public string Name { get; set; }
        public string Tag { get; set; }
        
        public UploadDataType Type { get; set; }
        
        public object Data { get; }
        public string DataJson { get; }
    }
    
    public class FileUploadData : IFileUploadData
    {
        public FileUploadData(
            string name,
            long size, 
            string checksum,
            string path,
            string tag,
            UploadDataType type
        )
        {
            Name = name;
            Size = size;
            Checksum = checksum;
            Path = path;
            Tag = tag ?? Guid.NewGuid().ToString();
            Type = type;
        }
        
        public string Name { get; set; }
        public long Size { get; set;}
        public string Checksum { get; set;}

        public bool IsLast { get; set; }

        [JsonIgnore]
        public string Path { get; set;}
        [JsonIgnore]
        public string Tag { get; set;}
        public UploadDataType Type { get; set; }
    }
}