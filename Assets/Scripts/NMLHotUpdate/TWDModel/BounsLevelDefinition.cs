using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class BounsLevelDefinition
	{
		public int ItemID;

		public int Level;

		public string Cost_Item;

		public string TraitsLevel;

		public string QualityLevel;

		public Dictionary<CurrencyType, int> Cost;

		public Dictionary<CurrencyType, int> GetCostInfo()
		{
			if (Cost == null)
			{
				Cost = new Dictionary<CurrencyType, int>();
				string[] array = Cost_Item.Split(';');
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split('(');
					string text = array2[0].ToLowerInvariant();
					array2[1] = array2[1].Replace(")", "");
					CurrencyType currencyType = CurrencyType.None;
					currencyType = ((!(text == "gold")) ? ((CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0])) : CurrencyType.Diamonds);
					Cost.Add(currencyType, int.Parse(array2[1]));
				}
			}
			return Cost;
		}
	}
}
