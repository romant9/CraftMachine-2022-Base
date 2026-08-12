using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class InAppPurchaseProductApple
	{
		[GEDType(GEDSpecialType.InAppProductId)]
		public string Id;

		public int PriceTier;

		public float PriceUSD;

		public string Active;

		[JsonIgnore]
		public bool IsActive
		{
			get
			{
				if (Active != null)
				{
					return Active.ToLower() == "y";
				}
				return false;
			}
		}
	}
}
