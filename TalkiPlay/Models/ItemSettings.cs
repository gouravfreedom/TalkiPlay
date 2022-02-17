using System;
namespace TalkiPlay.Shared
{
    public class ItemSettings
    {
        public ItemSettings()
        {
        }

        public ItemSettings(int itemId, bool isActive)
        {
            ItemId = itemId;
            IsActive = isActive;
        }

        public int ItemId { get; set; }
        public bool IsActive { get; set; }
    }
}
