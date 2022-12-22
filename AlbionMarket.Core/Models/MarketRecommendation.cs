using AlbionMarket.Core.Enums;

namespace AlbionMarket.Core.Models
{
    public class MarketRecommendation
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }

        public int? PotentialProfit { get; set; }

        public int? PriceBlackMarket { get; set; }

        public int? PriceCaerleon { get; set; }

        public int Tier { get; set; }

        public int ItemQuality { get; set; }

        public int EnchantLevel { get; set; }

        public DateTime? SellDateBlackMarket { get; set; }

        public DateTime? SellDateCaerlion { get; set; }

        public MarketPairStatus Status { get; set; }
    }
}
