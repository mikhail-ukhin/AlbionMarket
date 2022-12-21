using AlbionMarket.Core.Configuration;
using AlbionMarket.Core.Models;
using Microsoft.Extensions.Options;
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
		private readonly AlbionItemsService _albionItemsService;
        private readonly CheckedItemsService _checkedItemsService;

		private readonly AlbionMarketScanerOptions _albionMarketScanerOptions;

        public MarketPairInfoService(
			AlbionItemsService albionItemsService, 
			CheckedItemsService checkedItemsService,
			IOptions<AlbionMarketScanerOptions> albionMarketScanerOptions)
		{
			_albionItemsService = albionItemsService;
            _checkedItemsService = checkedItemsService;

			_albionMarketScanerOptions = albionMarketScanerOptions.Value;
        }

		public void HandleNewData(IEnumerable<MarketPair> marketPairs)
		{
			// TDB Come up with cleverer algorithm to update items

			var newDb = new List<MarketPair>();

			foreach (var item in marketPairs)
			{
                item.Profit = item.BlackMarketOrder.SellPriceMin - item.CaerleonOrder.SellPriceMin;

				newDb.Add(item);
            }

			marketPairsDb = newDb;
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

			var itemsToFilter = _checkedItemsService.GetItems();

			return marketPairsDb
				.Where(p => p.CaerleonOrder.SellPriceMin < p.BlackMarketOrder.SellPriceMin)
				//.Where(p => p.BlackMarketOrder.SellPriceMinDate >= DateTime.UtcNow.AddHours(-6))
				//.Where(p => p.BlackMarketOrder.SellPriceMinDate >= p.CaerleonOrder.SellPriceMinDate)
				.Where(p =>
				{
					var checkedItem = itemsToFilter.FirstOrDefault(v => v.ItemId == p.ItemId);

					if (checkedItem == null) return true;

					if (checkedItem.IsChecked == false) return true;

					var itemChanged = (checkedItem.CheckedAt < p.BlackMarketOrder.SellPriceMinDate || checkedItem.CheckedAt < p.CaerleonOrder.SellPriceMinDate);

					if (itemChanged)
					{
						checkedItem.IsChecked = false;

						return true;	
					}

					return false;
				})
				.Where(p => p.Profit > _albionMarketScanerOptions.MinProfit)
				.Select(pair =>
				{
					var item = _albionItemsService.GetItemInfo(pair.ItemId);

					return new MarketRecommendation
					{
						ItemName = item.LocalizedNames.EN_US,
						PotentialProfit = pair.Profit,
						EnchantLevel = item.EnchantLevel,
						ItemQuality = MapItemQuality(pair.Quality),
						Tier = item.Tier,
						SellDateBlackMarket = pair.BlackMarketOrder.SellPriceMinDate,
						SellDateCaerlion = pair.CaerleonOrder.SellPriceMinDate,
						PriceBlackMarket = pair.BlackMarketOrder.SellPriceMin,
						PriceCaerleon = pair.CaerleonOrder.SellPriceMin,
						ItemId = pair.ItemId
					};
				})
				.OrderByDescending(p => p.PotentialProfit)
				.ToArray();
		}

		private string MapItemQuality(int quality) {
			return quality switch
			{
				1 => "Normal",
				2 => "Good",
				3 => "Outstanding",
				4 => "Excellent",
				5 => "Masterpiece",
				_ => throw new ArgumentOutOfRangeException(nameof(quality))
			};
		}	
	}
}
