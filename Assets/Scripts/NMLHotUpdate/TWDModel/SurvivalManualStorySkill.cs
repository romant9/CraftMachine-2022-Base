using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SurvivalManualStorySkill
	{
		public int ID;

		public string Type;

		public int Level;

		public string SkillName;

		public string UpgradeCost;

		public int UnlockLevel;

		public string UpgradeTraits;

		public string Icon;

		public string TraitsDesc;

		[JsonIgnore]
		public Dictionary<CurrencyType, int> Cost;

		public Dictionary<CurrencyType, int> GetUpgradCostInfo()
		{
			if (Cost == null)
			{
				Cost = new Dictionary<CurrencyType, int>();
				string[] array = UpgradeCost.Split(';');
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
