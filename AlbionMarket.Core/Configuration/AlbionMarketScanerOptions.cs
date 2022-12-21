namespace AlbionMarket.Core.Configuration
{
    public class AlbionMarketScanerOptions
    {
        public int MinProfit { get; set; }

        public int BlackMarketDelayHours { get; set; }

        public int DefaultMinItemTier { get; set; }

        public string ApiUrl { get; set; }

        public int MarketScanDelayMs { get; set; }

        public int ItemCategoriesDelayMs { get; set; }
    }
}
