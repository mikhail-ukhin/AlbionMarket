namespace AlbionMarket.Core
{
    public class CheckedItem
    {
        public string ItemId { get; set; }

        public DateTime? CheckedAt { get; set; }

        public bool IsChecked { get; set; }
    }
}
