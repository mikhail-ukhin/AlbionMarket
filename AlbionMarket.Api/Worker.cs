using AlbionMarket.Core.Configuration;
using AlbionMarket.Core.Models;
using AlbionMarket.Services;
using Microsoft.Extensions.Options;

namespace AlbionMarket.Api
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CityOrderInfoService _cityOrderInfoService;
        private readonly AlbionItemsService _albionItemsService;
        private readonly MarketPairInfoService _marketPairInfoService;
        private readonly WorkerStateService _workerStateService;
        private readonly MarketPairStateService _marketPairStateService;

        private readonly List<string> _armorTypes;
        private readonly List<string> _itemsList;
        private readonly List<string> _cities;

        private readonly AlbionMarketScanerOptions _albionMarketScanerOptions;

        private int ItemsFound = 0;

        public Worker(
            ILogger<Worker> logger,
            CityOrderInfoService cityOrderInfoService,
            AlbionItemsService albionItemsService,
            MarketPairInfoService marketPairInfoService,
            WorkerStateService workerStateService,
            MarketPairStateService marketPairStateService,
            IOptions<AlbionMarketScanerOptions> albionMarketScanerOptions)
        {
            _albionMarketScanerOptions = albionMarketScanerOptions.Value;

            _cityOrderInfoService = cityOrderInfoService;
            _albionItemsService = albionItemsService;
            _marketPairInfoService = marketPairInfoService;
            _workerStateService = workerStateService;
            _marketPairStateService = marketPairStateService;

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
                _workerStateService.LastScanStart = DateTime.UtcNow;
                _workerStateService.ScanInProgress = true;

                ItemsFound = 0;

                var items = new List<MarketPair>();

                await WarriorItemMarketPair(items);
                await HunterItemMarketPair(items);

                _marketPairInfoService.HandleNewData(items);


                _workerStateService.LastScanFinished = DateTime.UtcNow;
                _workerStateService.ScanInProgress = false;
                _workerStateService.ItemsFound = ItemsFound;

                await Task.Delay(_albionMarketScanerOptions.MarketScanDelayMs, stoppingToken);
            }
        }

        private async Task<List<MarketPair>> ProcessItems(List<AlbionItem> items)
        {
            var itemNames = items.Select(v => v.UniqueName);
            var orders = await _cityOrderInfoService.GetCityOrderInfos(1, itemNames, _cities);

            if (orders == null) return new List<MarketPair>();

            var marketPairs = _marketPairInfoService.ConvertOrdersToMarketPairs(orders);

            ItemsFound += marketPairs.Count;

            await Task.Delay(_albionMarketScanerOptions.ItemCategoriesDelayMs);

            return marketPairs;
        }

        private async Task HunterItemMarketPair(List<MarketPair> marketPairs)
        {
            var bowItems = _albionItemsService.GetAllBows();
            var bowMarketPairs = await ProcessItems(bowItems);

            marketPairs.AddRange(bowMarketPairs);
        }

        private async Task WarriorItemMarketPair(List<MarketPair> marketPairs)
        {
            // ARMOR            

            foreach (var armor in _armorTypes)
            {
                foreach (var item in _itemsList)
                {
                    var armors = _albionItemsService.GetItems($"{item}_{armor}", 6, null);
                    var armorMarketPairs = await ProcessItems(armors);

                    marketPairs.AddRange(armorMarketPairs);
                }
            }

            // SWORDS

            var swordItems = _albionItemsService.GetAllSwords();
            var swordMarketPairs = await ProcessItems(swordItems);

            marketPairs.AddRange(swordMarketPairs);

            // AXES

            var axeItems = _albionItemsService.GetAllAxes();
            var axeMarketPairs = await ProcessItems(axeItems);

            marketPairs.AddRange(axeMarketPairs);

            // MACES

            var maceItems = _albionItemsService.GetAllMaces();
            var maceMarketPairs = await ProcessItems(maceItems);

            marketPairs.AddRange(maceMarketPairs);

            // HAMMERS

            var hammerItems = _albionItemsService.GetAllHammers();
            var hammerMarketPairs = await ProcessItems(hammerItems);

            marketPairs.AddRange(hammerMarketPairs);

            // WAR GLOVES

            var glovesItems = _albionItemsService.GetAllWarGloves();
            var glovesMarketPairs = await ProcessItems(glovesItems);

            marketPairs.AddRange(glovesMarketPairs);

            // CROSSBOWS

            var crossbowItems = _albionItemsService.GetAllWarCrossbows();
            var crossbowMarketPairs = await ProcessItems(crossbowItems);

            marketPairs.AddRange(crossbowMarketPairs);

            // SHIELDS

            var shieldItems = _albionItemsService.GetAllShields();
            var shieldIMarketPairs = await ProcessItems(shieldItems);

            marketPairs.AddRange(shieldIMarketPairs);
        }
    }
}