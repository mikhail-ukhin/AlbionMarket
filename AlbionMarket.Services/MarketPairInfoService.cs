using AlbionMarket.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionMarket.Services
{
    public class MarketPairInfoService
	{
		// temp solution to store data in service
		private List<MarketPair> marketPairsDb = new();
		private readonly AlbionItemsService albionItemsService;

		public MarketPairInfoService(AlbionItemsService albionItemsService)
		{
			this.albionItemsService = albionItemsService;
		}

		public async Task HandleNewData(IEnumerable<MarketPair> marketPairs)
		{
			foreach (var item in marketPairs)
			{
				var existedPair = marketPairsDb.SingleOrDefault(p => p.Quality == item.Quality && p.ItemId == item.ItemId);

				if (existedPair != null)
				{

				}
				else
				{
					item.Profit = item.BlackMarketOrder.SellPriceMin - item.CaerleonOrder.SellPriceMin;
					marketPairsDb.Add(item);
				}
			}
		}

		public List<MarketPair> ConvertOrdersToMarketPairs(IEnumerable<CityOrder> orders)
		{
            var marketPairs = new List<MarketPair>();
            var orderGroups = orders.GroupBy(v => $"{v.ItemId},{v.Quality}");

            foreach (var item in orderGroups)
            {
                var groupsList = item.ToList();

                if (groupsList.Count == 2)
                {
                    var caerlionOrder = groupsList.First(g => g.City == "Caerleon");
                    var blackmarketOrder = groupsList.First(g => g.City == "Black Market");

                    if (caerlionOrder.SellPriceMin == 0 || blackmarketOrder.SellPriceMin == 0)
                    {
                        continue;
                    }

                    var marketPair = new MarketPair
                    {
                        CaerleonOrder = caerlionOrder,
                        BlackMarketOrder = blackmarketOrder,
                        ItemId = caerlionOrder.ItemId,
                        Quality = caerlionOrder.Quality
                    };

                    marketPairs.Add(marketPair);
                }
            }

			return marketPairs;
        }

		public MarketRecommendation[] GetGoodPairs()
		{
			if (marketPairsDb.Count == 0)
			{
				return Array.Empty<MarketRecommendation>();
			}

			return marketPairsDb
				.Where(p => p.CaerleonOrder.SellPriceMin < p.BlackMarketOrder.SellPriceMin)
				.Where(p => p.BlackMarketOrder.SellPriceMinDate >= DateTime.UtcNow.AddHours(-3))
				.Where(p => p.BlackMarketOrder.SellPriceMinDate >= p.CaerleonOrder.SellPriceMinDate)
				.Where(p => p.Profit > 1000)
				.Select(pair =>
				{
					var item = albionItemsService.GetItemInfo(pair.ItemId);

					return new MarketRecommendation
					{
						ItemName = item.LocalizedNames.EN_US,
						PotentialProfit = pair.Profit,
						EnchantLevel = item.EnchantLevel,
						ItemQuality = pair.Quality,
						Tier = item.Tier,
						SellDateBlackMarket = pair.BlackMarketOrder.SellPriceMinDate,
						SellDateCaerlion = pair.CaerleonOrder.SellPriceMinDate,
						PriceBlackMarket = pair.BlackMarketOrder.SellPriceMin,
						PriceCaerleon = pair.CaerleonOrder.SellPriceMin
					};
				})
				.ToArray();
		}
	}
}
