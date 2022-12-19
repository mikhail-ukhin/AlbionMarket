using AlbionMarket.Core;
using AlbionMarket.Services;

namespace AlbionMarket.Worker
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private readonly CityOrderInfoService cityOrderInfoService;
		private readonly AlbionItemsService albionItemsService;
		private readonly MarketPairInfoService marketPairInfoService;

		public Worker(ILogger<Worker> logger, CityOrderInfoService cityOrderInfoService, AlbionItemsService albionItemsService, MarketPairInfoService marketPairInfoService)
		{
			_logger = logger;
			this.cityOrderInfoService = cityOrderInfoService;
			this.albionItemsService = albionItemsService;
			this.marketPairInfoService = marketPairInfoService;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				var armorTypes = new List<string>
				{
					"PLATE",
					"LEATHER",
					"CLOTH"
				};

				var itemsList = new List<string>
				{
					"HEAD",
					"SHOES",
					"ARMOR"
				};


				foreach (var armor in armorTypes)
				{
					foreach (var item in itemsList)
					{
						await ProcessItems($"{item}_{armor}");
						await Task.Delay(2500);
					}
				}
				

				var itemsToWork = marketPairInfoService.GetGoodPairs();

				await Task.Delay(60000, stoppingToken);
			}
		}

		private async Task ProcessItems(string ITEM)
		{
			var cities = new List<string>()
				{
					"Caerleon",
					"Blackmarket"
				};

			var items = albionItemsService.GetItems(ITEM, tier: 6, enchant: null);
			var itemNames = items.Select(v => v.UniqueName).ToArray();
			var orders = await cityOrderInfoService.GetCityOrderInfos(1, itemNames, cities.ToArray());
			var orderGroups = orders.ToList().GroupBy(v => $"{v.ItemId},{v.Quality}").ToList();
			var merketPairs = new List<MarketPair>();

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

					merketPairs.Add(marketPair);
				}
			}

			await marketPairInfoService.HandleNewData(merketPairs);
		}
	}
}