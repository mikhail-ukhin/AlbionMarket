namespace AlbionMarket.Core.Models
{
    public class CheckedItem
    {
        public string ItemId { get; set; }

        public DateTime? CheckedAt { get; set; }

        public bool IsChecked { get; set; }
    }
}
