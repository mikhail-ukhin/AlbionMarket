using AlbionMarket.Api;
using AlbionMarket.Core;
using AlbionMarket.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<CityOrderInfoService>();
builder.Services.AddSingleton<AlbionItemsService>();
builder.Services.AddSingleton<MarketPairInfoService>();
builder.Services.AddSingleton<WorkerStateService>();
builder.Services.AddSingleton<CheckedItemsService>();

builder.Services.Configure<AlbionMarketScanerOptions>(
	builder.Configuration.GetSection("AlbionMarketScanner"));

builder.Services.AddHostedService<Worker>();

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
