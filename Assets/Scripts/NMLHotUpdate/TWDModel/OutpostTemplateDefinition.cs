using System;

namespace TWDModel
{
	[Serializable]
	public class OutpostTemplateDefinition
	{
		public string Id;

		public string MissionID;

		public string LocalizationKey;

		public int FirstSliceDeploymentPoints;

		public int SecondSliceDeploymentPoints;

		public int ThirdSliceDeploymentPoints;

		public string Cost;

		public int OutpostLevelRequirement;

		public int ResourceProductionPercentage;

		private CurrencyType cachedCostCurrencyType;

		private int cachedCostAmount;

		private void UpdateCachedCost()
		{
			string[] array = Cost.Split('(');
			string text = array[0].ToLowerInvariant();
			array[1] = array[1].Replace(")", "");
			cachedCostCurrencyType = CurrencyType.None;
			if (text == "gold")
			{
				cachedCostCurrencyType = CurrencyType.Diamonds;
			}
			else
			{
				cachedCostCurrencyType = (CurrencyType)Enum.Parse(typeof(CurrencyType), array[0]);
			}
			cachedCostAmount = int.Parse(array[1]);
		}

		public string GetNameLocalizationKey()
		{
			return LocalizationKey + ".Name";
		}

		public int GetResourceProduction(int baseProduction)
		{
			return baseProduction * ResourceProductionPercentage / 100;
		}

		public CurrencyType GetCostCurrencyType()
		{
			if (cachedCostCurrencyType == CurrencyType.None)
			{
				UpdateCachedCost();
			}
			return cachedCostCurrencyType;
		}

		public int GetCostAmount()
		{
			if (cachedCostCurrencyType == CurrencyType.None)
			{
				UpdateCachedCost();
			}
			return cachedCostAmount;
		}
	}
}
