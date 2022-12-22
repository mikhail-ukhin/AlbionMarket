namespace AlbionMarket.Core.Configuration
{
    public class AlbionMarketScanerSettings
    {
        public int MinProfit { get; set; }

        public int BlackMarketDelayHours { get; set; }

        public int DefaultMinItemTier { get; set; }

        public string ApiUrl { get; set; }

        public int MarketScanDelayMs { get; set; }

        public int ItemCategoriesDelayMs { get; set; }

        public int MarketCatalogPageUpdateInterval { get; set; }
    }
}
