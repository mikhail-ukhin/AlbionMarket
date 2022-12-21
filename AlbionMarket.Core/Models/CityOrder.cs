using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AlbionMarket.Core.Models
{
    public class CityOrder
    {
        public CityOrder()
        {

        }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("quality")]
        public int Quality { get; set; }

        [JsonPropertyName("item_id")]
        public string ItemId { get; set; }

        [JsonPropertyName("sell_price_min")]
        public int? SellPriceMin { get; set; }

        [JsonPropertyName("sell_price_min_date")]
        public DateTime? SellPriceMinDate { get; set; }

        [JsonPropertyName("sell_price_max")]
        public int? SellPriceMax { get; set; }

        [JsonPropertyName("sell_price_max_date")]
        public DateTime? SellPriceMaxDate { get; set; }

        //[JsonPropertyName("buy_price_min")]
        //public int? BuyPriceMin { get; set; }

        //[JsonPropertyName("buy_price_min_date")]
        //public DateTimeOffset? BuyPriceMinDate { get; set; }

        //[JsonPropertyName("buy_price_max")]
        //public int? BuyPriceMax { get; set; }

        //[JsonPropertyName("buy_price_max_date")]
        //public DateTimeOffset? BuyPriceMaxDate { get; set; }
    }
}
