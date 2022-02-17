
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public interface ITag
    {
        int Id { get; }
        string SerialNumber { get; }
        IList<int> ItemIds { get; }
        
    }
}