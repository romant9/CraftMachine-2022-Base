using System;

namespace TWDModel
{
	public static class ReturnQuestRuleHelper
	{
		public static int GetRequiredAmount(string paramsValue, int defaultValue = 1)
		{
			if (string.IsNullOrEmpty(paramsValue))
			{
				return defaultValue;
			}
			string[] array = paramsValue.Split(',');
			for (int num = array.Length - 1; num >= 0; num--)
			{
				if (int.TryParse(array[num].Trim(), out var result))
				{
					return Math.Max(result, 0);
				}
			}
			return defaultValue;
		}

		public static bool TryGetCurrencyType(ReturnQuestType questType, out CurrencyType currencyType)
		{
			switch (questType)
			{
			case ReturnQuestType.SpendDiamonds:
				currencyType = CurrencyType.Diamonds;
				return true;
			case ReturnQuestType.SpendPhone:
				currencyType = CurrencyType.Phone;
				return true;
			default:
				currencyType = CurrencyType.None;
				return false;
			}
		}
	}
}
