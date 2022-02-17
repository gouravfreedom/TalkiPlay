using System.Collections.Generic;
using System.Linq;

namespace TalkiPlay.Shared
{
    public class TagItemsSelector
    {        
        int _currentIndex;

        public TagItemsSelector(IRoom room, IList<ItemDto> items, IPack pack)
        {
            Room = room;
            Items = items;
            Pack = pack;

            MappedTags = new List<int>();     
        }

        public IRoom Room { get; }
        public IList<ItemDto> Items { get; }
        public IPack Pack { get; }
        public IList<int> MappedTags { get; }
       

        public ItemDto GetNextItem()
        {
            if (_currentIndex < Items.Count -1)
            {
                _currentIndex++;
                return Items[_currentIndex];
            }

            return null;            
        }

        public ItemDto GetPreviousItem()
        {
            if (_currentIndex > 0)
            {
                return Items[_currentIndex-1];
            }
            return null;            
        }

        public void SetCurrent(ItemDto item)
        {            
            var index = Items.IndexOf(item);
            if (index > 0)
            {
                _currentIndex = index;
            }         
        }

        public IItem GetItem(int itemId)
        {
            return Items.FirstOrDefault(m => m.Id == itemId);
        }
    }
}