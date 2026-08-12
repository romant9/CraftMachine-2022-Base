using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class BadgeRerollConfig
	{
		public string Type;

		public string PriceString;

		[JsonIgnore]
		public int[] Price;
	}
}
