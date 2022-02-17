namespace TalkiPlay.Shared
{
    public class StarItem
    {
        public StarItem(int itemId, bool isCollected)
        {
            ItemId = itemId;
            IsCollected = isCollected;
        }
        public int ItemId { get; set; }
        public bool IsCollected { get; set; }
    }
}