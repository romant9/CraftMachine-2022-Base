using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class CashierItem
	{
		[JsonIgnore]
		public List<CurrencyType> CurrencyTypesExisted = new List<CurrencyType>();

		public int[] Cost { get; protected set; }

		public PurchaseType PurchaseType { get; protected set; }

		public CashierItem(PurchaseType purchaseType)
		{
			Cost = new int[(int)CurrencyType.Count];
			PurchaseType = purchaseType;
		}

		public int GetCost(CurrencyType currencyType)
		{
			return Cost[(int)currencyType];
		}

		public void SetCost(CurrencyType currencyType, int cost)
		{
			if (!CurrencyTypesExisted.Contains(currencyType))
			{
				CurrencyTypesExisted.Add(currencyType);
			}
			Cost[(int)currencyType] = cost;
		}
	}
}
