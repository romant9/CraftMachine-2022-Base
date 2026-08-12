using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class SPTraitsRemoldDefinitions
	{
		public string ID;

		public string Type;

		public bool Available;

		public string MakingCost;

		public string UpgradeCost;

		public SurvivorClass AvailableClass;

		public List<string> SurvivorClass;

		public List<string> EquipType;

		public List<string> ExclusionForSp;

		public List<string> TagMatch;

		public int Star;

		public string Color;

		public int Value;

		public int UpgradeType;

		public bool Locked;

		public int Level;

		public int MaxLevel;

		public List<string> ActiveTraits;

		public List<string> PassiveTraits;

		public List<string> ActiveTraitsForCharge;

		public string SPTraitsIcon;

		public string SPTraitsName;

		public List<string> SPTraitsLcValue;

		public string SPTraitsDesc;

		public string SPTraitsIconOnCloud;

		[JsonIgnore]
		public Dictionary<CurrencyType, int> UpgradeCosts;

		[JsonIgnore]
		public Dictionary<CurrencyType, int> MakingCosts;

		public Dictionary<CurrencyType, int> GetUpgradeCost()
		{
			if (UpgradeCosts == null)
			{
				UpgradeCosts = new Dictionary<CurrencyType, int>();
				if (!string.IsNullOrEmpty(UpgradeCost))
				{
					string[] array = UpgradeCost.Split(';');
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split('(');
						string text = array2[0].ToLowerInvariant();
						array2[1] = array2[1].Replace(")", "");
						CurrencyType currencyType = CurrencyType.None;
						currencyType = ((!(text == "gold")) ? ((CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0])) : CurrencyType.Diamonds);
						UpgradeCosts.Add(currencyType, int.Parse(array2[1]));
					}
				}
			}
			return UpgradeCosts;
		}

		public Dictionary<CurrencyType, int> GetMakingCost()
		{
			if (MakingCosts == null)
			{
				MakingCosts = new Dictionary<CurrencyType, int>();
				if (!string.IsNullOrEmpty(MakingCost))
				{
					string[] array = MakingCost.Split(';');
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split('(');
						string text = array2[0].ToLowerInvariant();
						array2[1] = array2[1].Replace(")", "");
						CurrencyType currencyType = CurrencyType.None;
						currencyType = ((!(text == "gold")) ? ((CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0])) : CurrencyType.Diamonds);
						MakingCosts.Add(currencyType, int.Parse(array2[1]));
					}
				}
			}
			return MakingCosts;
		}
	}
}
