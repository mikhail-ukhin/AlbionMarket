using AlbionMarket.Core.Models;

namespace AlbionMarket.Services
{
    public class CheckedItemsService
    {
        private readonly List<CheckedItem> ItemsDb = new();

        public List<CheckedItem> GetItems()
        {
            return ItemsDb;
        }

        public void CheckItem(string itemId)
        {
            var existedItem = ItemsDb.FirstOrDefault(i => i.ItemId == itemId);

            if (existedItem != null)
            {
                existedItem.CheckedAt = DateTime.UtcNow;
                existedItem.IsChecked = true;
            }
            else
            {
                ItemsDb.Add(new CheckedItem
                {
                    ItemId = itemId,
                    CheckedAt = DateTime.UtcNow,
                    IsChecked = true
                });
            }
        }
    }
}
