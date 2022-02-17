using System.Collections.Generic;
using System.Linq;
using Akavache.Sqlite3.Internal;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class InstructionDto : IInstruction
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("item")]
        public ItemDto ItemActual { get; set; }

        [JsonIgnore]
        public IItem Item => ItemActual;
        
        [JsonProperty("modes")]
        public IList<ModeDto> ModeList { get; set; }

        [JsonIgnore]
        public IList<IMode> Modes => ModeList?.ToList<IMode>();
    }
}