using AlbionMarket.Core;
using AlbionMarket.Services;

namespace AlbionMarket.Worker
{
	public class Worker : BackgroundService
	{
		private readonly ILogger<Worker> _logger;
		private readonly CityOrderInfoService _cityOrderInfoService;
		private readonly AlbionItemsService _albionItemsService;
		private readonly MarketPairInfoService _marketPairInfoService;

		private readonly List<string> _armorTypes;
		private readonly List<string> _itemsList;
		private readonly List<string> _cities;

		public Worker(ILogger<Worker> logger, CityOrderInfoService cityOrderInfoService, AlbionItemsService albionItemsService, MarketPairInfoService marketPairInfoService)
		{	
			_cityOrderInfoService = cityOrderInfoService;
			_albionItemsService = albionItemsService;
			_marketPairInfoService = marketPairInfoService;

			_armorTypes = new List<string>
			{
				"PLATE",
				"LEATHER",
				"CLOTH"
			};

			_itemsList = new List<string>
			{
				"HEAD",
				"SHOES",
				"ARMOR"
			};

            _cities = new List<string>()
            {
                "Caerleon",
                "Blackmarket"
            };

            _logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				foreach (var armor in _armorTypes)
				{
					foreach (var item in _itemsList)
					{
						await ProcessItems($"{item}_{armor}");
						await Task.Delay(2500, stoppingToken);
					}
				}
				
				var itemsToWork = _marketPairInfoService.GetGoodPairs();

				await Task.Delay(60000, stoppingToken);
			}
		}

		private async Task ProcessItems(string itemName)
		{
			var items = _albionItemsService.GetItems(itemName, tier: 6, enchant: null);
			var itemNames = items.Select(v => v.UniqueName);
			var orders = await _cityOrderInfoService.GetCityOrderInfos(1, itemNames, _cities);

			if (orders == null) return;

			var marketPairs = _marketPairInfoService.ConvertOrdersToMarketPairs(orders);

			await _marketPairInfoService.HandleNewData(marketPairs);
		}
	}
}