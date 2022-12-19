﻿using AlbionMarket.Core;
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

		public MarketRecommendation[] GetGoodPairs()
		{
			return marketPairsDb
				.Where(p => p.CaerleonOrder.SellPriceMin < p.BlackMarketOrder.SellPriceMin)
				.Where(p => p.BlackMarketOrder.SellPriceMinDate >= DateTime.UtcNow.AddHours(-3))
				.Where(p => p.BlackMarketOrder.SellPriceMinDate >= p.CaerleonOrder.SellPriceMinDate)
				.Where(p => p.Profit > 4000)
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

	public class MarketRecommendation
	{
		public string ItemName { get; set; }

		public int? PotentialProfit { get; set; }

		public int? PriceBlackMarket { get; set; }

		public int? PriceCaerleon { get; set; }

		public int Tier { get; set; }

		public int ItemQuality { get; set; }

		public int EnchantLevel { get; set; }

		public DateTime? SellDateBlackMarket { get; set; }

		public DateTime? SellDateCaerlion { get; set; }
	}
}