using AlbionMarket.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlbionMarket.Services
{
	public class AlbionItemsService
	{
		private readonly Dict<string, AlbionItem> albionItemsDb = new Dict<string, AlbionItem>();

		public AlbionItemsService()
		{
			PrepareItemsData();
		}

		private void PrepareItemsData() {
			var content = File.ReadAllText("C:\\Users\\user\\source\\repos\\AlbionMarket\\AlbionMarket.Services\\items.json");
			var originalItems = JsonSerializer.Deserialize<List<AlbionItem>>(content);

			foreach (var item in originalItems)
			{
				// if (item.UniqueName.Contains("ARTEFACT"))
				// {
				// 	continue;
				// }

				item.ParseName();

				albionItemsDb[item.UniqueName] = item;
			}
		}

		public List<AlbionItem> GetAllItems() => albionItems;

		public List<AlbionItem> GetItems(string category, int? tier, int? enchant)
		{
			var expression = albionItemsDb.Values().Where(i => i.UniqueName.Contains(category));

			if (tier != null)
			{
				expression = expression.Where(i => i.Tier >= tier);
			}

			if (enchant != null)
			{
				expression = expression.Where(i => i.EnchantLevel == enchant);
			}

			return expression.ToList();
		}

		public string GetItemFriendlyName(string itemId) => GetItemInfo(itemId).LocalizedNames.EN_US;

		public AlbionItem GetItemInfo(string itemId) => albionItemsDb[itemId];
	}
}
