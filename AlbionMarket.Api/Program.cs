using AlbionMarket.Api;
using AlbionMarket.Core.Configuration;
using AlbionMarket.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder
    .Services
	.AddSingleton<CityOrderInfoService>()
    .AddSingleton<MarketPairInfoService>()
    .AddSingleton<WorkerStateService>()
    .AddSingleton<CheckedItemsService>()
    .AddSingleton<AlbionItemsService>()
    .AddSingleton<MarketPairStateService>();

builder.Services.Configure<AlbionMarketScanerSettings>(
	builder.Configuration.GetSection("AlbionMarketScanner"));

builder.Services.Configure<AlbionDatabaseSettings>(
    builder.Configuration.GetSection("AlbionDatabase"));

builder.Services.Configure<AlbionWorkerSettings>(
    builder.Configuration.GetSection("AlbionWorker"));

builder.Services.AddHostedService<AlbionMarketWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
