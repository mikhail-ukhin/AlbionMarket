using AlbionMarket.Core.Configuration;
using AlbionMarket.Core.Models;
using AlbionMarket.Services;
using Microsoft.Extensions.Options;
using System.Threading;

namespace AlbionMarket.Api
{
    public class AlbionMarketWorker : BackgroundService
    {
        private readonly ILogger<AlbionMarketWorker> _logger;
        private readonly CityOrderInfoService _cityOrderInfoService;
        private readonly AlbionItemsService _albionItemsService;
        private readonly MarketPairInfoService _marketPairInfoService;
        private readonly WorkerStateService _workerStateService;

        private readonly List<string> _armorTypes;
        private readonly List<string> _itemsList;
        private readonly List<string> _cities;

        private readonly SemaphoreSlim semaphore;

        private readonly AlbionMarketScanerSettings _albionMarketScanerSettings;
        private readonly AlbionWorkerSettings _albionWorkerSettings;

        private int ItemsFound = 0;

        public AlbionMarketWorker(
            ILogger<AlbionMarketWorker> logger,
            CityOrderInfoService cityOrderInfoService,
            AlbionItemsService albionItemsService,
            MarketPairInfoService marketPairInfoService,
            WorkerStateService workerStateService,
            IOptions<AlbionMarketScanerSettings> albionMarketScanerOptions,
            IOptions<AlbionWorkerSettings> albionWorkerSettings)
        {
            _albionMarketScanerSettings = albionMarketScanerOptions.Value;
            _albionWorkerSettings = albionWorkerSettings.Value;

            semaphore = new(_albionWorkerSettings.DegreeOfParallelism);

            _cityOrderInfoService = cityOrderInfoService;
            _albionItemsService = albionItemsService;
            _marketPairInfoService = marketPairInfoService;
            _workerStateService = workerStateService;

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
                _logger.LogInformation("Starting a market scan job");

                _workerStateService.LastScanStart = DateTime.UtcNow;
                _workerStateService.ScanInProgress = true;

                ItemsFound = 0;

                var items = new List<MarketPair>();

                await WarriorItemMarketPair(items);
                await HunterItemMarketPair(items);

                _marketPairInfoService.HandleNewData(items);

                await _marketPairInfoService.PrepareRecomendations();

                _workerStateService.LastScanFinished = DateTime.UtcNow;
                _workerStateService.ScanInProgress = false;
                _workerStateService.ItemsFound = ItemsFound;

                _logger.LogInformation("Worker finished job, awaiting timeout...");

                await Task.Delay(_albionMarketScanerSettings.MarketScanDelayMs, stoppingToken);
            }
        }

        private async Task<List<MarketPair>> ConvertAlbionItemsIntoMarketPairs(List<AlbionItem> items)
        {
            await semaphore.WaitAsync();

            Console.WriteLine($"starting handling albion items = {DateTime.Now.ToLocalTime()}");

            var itemNames = items.Select(v => v.UniqueName);
            var orders = await _cityOrderInfoService.GetCityOrders(1, itemNames, _cities);

            if (orders == null) return new List<MarketPair>();

            var marketPairs = _marketPairInfoService.ConvertOrdersToMarketPairs(orders);

            ItemsFound += marketPairs.Count;

            semaphore.Release();

            //await Task.Delay(_albionMarketScanerOptions.ItemCategoriesDelayMs);

            return marketPairs;
        }

        private async Task HunterItemMarketPair(List<MarketPair> marketPairs)
        {
            var bowItems = _albionItemsService.GetAllBows();
            var bowMarketPairs = await ConvertAlbionItemsIntoMarketPairs(bowItems);

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
                    var armorMarketPairs = await ConvertAlbionItemsIntoMarketPairs(armors);

                    marketPairs.AddRange(armorMarketPairs);
                }
            }

            // SWORDS

            var swordItems = _albionItemsService.GetAllSwords();
            var swordMarketPairsTask = ConvertAlbionItemsIntoMarketPairs(swordItems);

            // AXES

            var axeItems = _albionItemsService.GetAllAxes();
            var axeMarketPairsTask = ConvertAlbionItemsIntoMarketPairs(axeItems);

            // MACES

            var maceItems = _albionItemsService.GetAllMaces();
            var maceMarketPairsTask = ConvertAlbionItemsIntoMarketPairs(maceItems);

            // HAMMERS

            var hammerItems = _albionItemsService.GetAllHammers();
            var hammerMarketPairsTask = ConvertAlbionItemsIntoMarketPairs(hammerItems);

            // WAR GLOVES

            var glovesItems = _albionItemsService.GetAllWarGloves();
            var glovesMarketPairsTask = ConvertAlbionItemsIntoMarketPairs(glovesItems);

            // CROSSBOWS

            var crossbowItems = _albionItemsService.GetAllWarCrossbows();
            var crossbowMarketPairsTask = ConvertAlbionItemsIntoMarketPairs(crossbowItems);

            // SHIELDS

            var shieldItems = _albionItemsService.GetAllShields();
            var shieldIMarketPairsTask = ConvertAlbionItemsIntoMarketPairs(shieldItems);

            var getAllMarketPairTasks = new List<Task<List<MarketPair>>>()
            {
                shieldIMarketPairsTask,
                crossbowMarketPairsTask,
                glovesMarketPairsTask,
                hammerMarketPairsTask,
                maceMarketPairsTask,
                axeMarketPairsTask,
                swordMarketPairsTask
            };

            var customMarketPairLists = await Task.WhenAll(getAllMarketPairTasks);
            var customMarketPairs = customMarketPairLists.SelectMany(l => l);

            if (customMarketPairs != null)
            {
                marketPairs.AddRange(customMarketPairs);
            }
        }
    }
}