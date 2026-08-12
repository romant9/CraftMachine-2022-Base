using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SurvivalManualSkill
	{
		public int ID;

		public string Type;

		public int Level;

		public string SkillName;

		public string UpgradeCost;

		public int UnlockLevel;

		public int Attribute_attack_ratio;

		public int Attribute_hp_ratio;

		public int Attribute_hitrate_melee;

		public int Attribute_hitrate_range;

		public int Attribute_critical_ref;

		public int Attribute_dmg_critical_ratio_ref;

		public string NextLevelValue;

		[JsonIgnore]
		public Dictionary<CurrencyType, int> Cost;

		public Dictionary<CurrencyType, int> GetUpgradCostInfo()
		{
			if (Cost == null && UpgradeCost != null)
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
