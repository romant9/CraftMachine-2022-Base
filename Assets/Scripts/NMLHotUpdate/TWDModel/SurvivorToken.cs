using System;

namespace TWDModel
{
	[Serializable]
	public class SurvivorToken
	{
		public CurrencyType Type;

		public int Amount;

		public int AmountRarityLevel;

		public static string GetHeroId(CurrencyType type)
		{
			string text = type.ToString();
			if (text.Contains("Token"))
			{
				return "Hero_" + text.Substring(0, text.Length - "Token".Length);
			}
			return "";
		}

		public static CurrencyType GetClassAsCurrency(SurvivorClass SurvivorClass)
		{
			return (CurrencyType)Enum.Parse(typeof(CurrencyType), SurvivorClass.ToString() + "Token");
		}
	}
}
