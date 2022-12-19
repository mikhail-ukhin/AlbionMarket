using AlbionMarket.Services;
using AlbionMarket.Worker;

IHost host = Host
	.CreateDefaultBuilder(args)
	.ConfigureServices(services =>
	{
		services.AddSingleton<CityOrderInfoService>();
		services.AddSingleton<AlbionItemsService>();
		services.AddSingleton<MarketPairInfoService>();
		services.AddHostedService<Worker>();
	})
	.Build();

host.Run();
