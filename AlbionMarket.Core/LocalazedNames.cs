using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AlbionMarket.Core
{
	public class LocalazedNames
	{
		[JsonPropertyName("EN-US")]
		public string EN_US { get; set; }
	}
}
