using AlbionMarket.Core.Configuration;
using AlbionMarket.Core.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace AlbionMarket.Services
{
    public class CityOrderInfoService
	{
		private readonly HttpClient _httpClient = new();
        private readonly AlbionMarketScanerOptions _albionMarketScanerOptions;

        public CityOrderInfoService(IOptions<AlbionMarketScanerOptions> albionMarketScanerOptions)
		{
            _albionMarketScanerOptions = albionMarketScanerOptions.Value;
            _httpClient.BaseAddress = new Uri(_albionMarketScanerOptions.ApiUrl);
        }

		public async Task<CityOrder[]?> GetCityOrders(int quality, IEnumerable<string> itemNames, IEnumerable<string> cities)
		{
			var requestUrl = $"stats/prices/{string.Join(',', itemNames)}?locations={string.Join(',', cities)}";
			var result = await _httpClient.GetFromJsonAsync<CityOrder[]>(requestUrl);

			return result;
		}
	}
}
