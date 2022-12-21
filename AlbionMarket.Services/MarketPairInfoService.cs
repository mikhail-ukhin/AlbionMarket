using AlbionMarket.Core.Configuration;
using AlbionMarket.Core.Models;
using Microsoft.Extensions.Options;

namespace AlbionMarket.Services
{
    public class MarketPairInfoService
    {
        // temp solution to store data in service
        private List<MarketPair> marketPairsDb = new();

        private readonly AlbionItemsService _albionItemsService;
        private readonly MarketPairStateService _marketPairStateService;

        private readonly AlbionMarketScanerOptions _albionMarketScanerOptions;

        public MarketPairInfoService(
            AlbionItemsService albionItemsService,
            MarketPairStateService marketPairStateService,

            IOptions<AlbionMarketScanerOptions> albionMarketScanerOptions
        )
        {
            _albionItemsService = albionItemsService;
            _marketPairStateService = marketPairStateService;

            _albionMarketScanerOptions = albionMarketScanerOptions.Value;
        }

        public void HandleNewData(IEnumerable<MarketPair> marketPairs)
        {
            // TDB Come up with cleverer algorithm to update items (or not?)

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

        public async Task<List<MarketRecommendation>> GetProfitableMarketPairs()
        {
            if (marketPairsDb.Count == 0)
            {
                return new List<MarketRecommendation>();
            }

            var marketPairStateItems = await _marketPairStateService.GetAllAsync();

            var filteredMarketPairs = marketPairsDb
                .Where(p => p.Profit > _albionMarketScanerOptions.MinProfit)
                .Where(p => p.CaerleonOrder.SellPriceMin > p.BlackMarketOrder.SellPriceMin);

            var result = new List<MarketRecommendation>();

            foreach (var marketPair in filteredMarketPairs)
            {
                var existedMarketPairState = marketPairStateItems.FirstOrDefault(p => p.ItemId == marketPair.ItemId && p.Quality == marketPair.Quality);
                var recommendation = MapMarketPairToRecommendation(marketPair);

                if (existedMarketPairState != null)
                {
                    // checked item, but updated profit and dates
                    if (marketPair.Profit > existedMarketPairState.LastProfit && (existedMarketPairState.StatusUpdatedAt < marketPair.BlackMarketOrder.SellPriceMinDate || existedMarketPairState.StatusUpdatedAt < marketPair.CaerleonOrder.SellPriceMinDate))
                    {
                        existedMarketPairState.StatusUpdatedAt = DateTime.UtcNow;
                        existedMarketPairState.LastProfit = marketPair.Profit;
                        existedMarketPairState.Status = Core.Enums.MarketPairStatus.None;

                        await _marketPairStateService.UpdateAsync(existedMarketPairState.Id, existedMarketPairState);

                        result.Add(recommendation);
                    }
                }
                else
                {
                    var newMarketPairState = new MarketPairState
                    {
                        ItemId = marketPair.ItemId,
                        LastProfit = marketPair.Profit,
                        Quality = marketPair.Quality,
                        Status = Core.Enums.MarketPairStatus.None,
                        StatusUpdatedAt = DateTime.UtcNow
                    };

                    await _marketPairStateService.CreateAsync(newMarketPairState);

                    result.Add(recommendation);
                }
            }

            result = result.OrderByDescending(r => r.PotentialProfit).ToList();

            return result;
        }

        private MarketRecommendation MapMarketPairToRecommendation(MarketPair marketPair)
        {
            var item = _albionItemsService.GetItemInfo(marketPair.ItemId);

            return new MarketRecommendation
            {
                ItemName = item.LocalizedNames.EN_US,
                PotentialProfit = marketPair.Profit,
                EnchantLevel = item.EnchantLevel,
                ItemQuality = MapItemQuality(marketPair.Quality),
                Tier = item.Tier,
                SellDateBlackMarket = marketPair.BlackMarketOrder.SellPriceMinDate,
                SellDateCaerlion = marketPair.CaerleonOrder.SellPriceMinDate,
                PriceBlackMarket = marketPair.BlackMarketOrder.SellPriceMin,
                PriceCaerleon = marketPair.CaerleonOrder.SellPriceMin,
                ItemId = marketPair.ItemId
            };
        }

        private string MapItemQuality(int quality) =>
            quality switch
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
