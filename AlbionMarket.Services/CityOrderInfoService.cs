﻿using AlbionMarket.Core;
using System.Net.Http.Json;

namespace AlbionMarket.Services
{
	public class CityOrderInfoService
	{
		private readonly HttpClient _httpClient = new();

		public CityOrderInfoService()
		{
			_httpClient.BaseAddress = new Uri("https://www.albion-online-data.com/api/v2/");
		}

		public async Task<CityOrder[]?> GetCityOrderInfos(int quality, string[] itemNames, string[] cities)
		{
			var requestUrl = $"stats/prices/{string.Join(',', itemNames)}?locations={string.Join(',', cities)}&qualities={quality}";
			var result = await _httpClient.GetFromJsonAsync<CityOrder[]>(requestUrl);

			return result;
		}
	}
}