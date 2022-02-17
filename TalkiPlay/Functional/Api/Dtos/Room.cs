using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TalkiPlay.Shared
{
    public class RoomDto : IRoom
    {
        public RoomDto()
        {
            TagList = new List<TagDto>();
            PacksOnboardedList = new List<PackOnboardedDto>();
        }

        public RoomDto(int roomId, IList<int> roomTagIds, IList<int> packIds)
        {
            Id = roomId;

            TagList = roomTagIds.Select(id => new TagDto()
            {
                Id = id
            }).ToList();

            PacksOnboardedList = packIds.Select(groupId => new PackOnboardedDto()
            {
                Id = groupId
            }).ToList();
        }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imagePath")]
        public string ImagePath { get; set; }

        [JsonProperty("imageAssetId")]
        public int? AssetId { get; set; }

        [JsonProperty("itemGroups")]
        public IList<PackOnboardedDto> PacksOnboardedList { get; set; }

        [JsonIgnore]
        public IList<int> Packs => PacksOnboardedList.Select(m => m.Id).ToList();

        [JsonProperty("tags")]
        public IList<TagDto> TagList { get; set; }

        [JsonIgnore]
        public IList<ITag> TagItems => TagList?.ToList<ITag>();

        [JsonProperty("tagsCount")]
        public int TagsCount { get; set; }
    }

    public class AddUpdateRoomRequest
    {       
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("imageAssetId")]
        public int ImageAssetId { get; set; }
        
    }

    public class RoomItemTagsUploadRequest
    {
        public RoomItemTagsUploadRequest()
        {
            ItemGroupIds = new List<int>();
            TagIds = new List<int>();
        }
        
        [JsonProperty("id")]
        public int Id { get; set; }
        
        [JsonProperty("itemGroupIds")]
        public IList<int> ItemGroupIds { get; set; } 
        
        [JsonProperty("tagIds")]
        public IList<int> TagIds { get; set; } 

    }

}