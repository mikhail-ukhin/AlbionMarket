using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionMarket.Core
{
	public class MarketPair
	{
		public int Id { get; set; }

		public string ItemId { get; set; }

		public int Quality { get; set; }

		public int Tier { get; set; }

		public CityOrder CaerleonOrder { get; set; }

		public CityOrder BlackMarketOrder { get; set; }

		public int? Profit { get; set; }
	}
}
