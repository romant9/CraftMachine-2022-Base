using System.Collections.Generic;
using System.Text;
using TWDModel;

public class DropRatesNamesHelper
{
	public static void GetNamesForDropCurrencies(ref List<ItemAmountProbabilityData> probabilities)
	{
		for (int i = 0; i < ((probabilities != null) ? probabilities.Count : 0); i++)
		{
			if (FixedPoint.Round(probabilities[i].Probability * 100.0) > 0L && probabilities[i].ItemEnumType == typeof(DropCurrenciesProbabilitiesDefinition.DropCurrency))
			{
				probabilities[i].Name = HelpersLocalization.GetDropCurrencyName((DropCurrenciesProbabilitiesDefinition.DropCurrency)probabilities[i].ItemEnumValue);
			}
		}
	}

	public static void GetNamesForBadges(ref List<ItemAmountProbabilityData> probabilities)
	{
		for (int i = 0; i < ((probabilities != null) ? probabilities.Count : 0); i++)
		{
			if (FixedPoint.Round(probabilities[i].Probability * 100.0) > 0L)
			{
				probabilities[i].Name = LocalizationManager.GetText("Droptype.Badge");
			}
		}
	}

	public static void GetNameForComponents(ref List<ItemAmountProbabilityData> probabilities)
	{
		for (int i = 0; i < probabilities.Count; i++)
		{
			if (probabilities[i].Probability * 100.0 > 0L && probabilities[i].ItemEnumType == typeof(CurrencyType))
			{
				probabilities[i].Name = HelpersLocalization.GetComponentName((CurrencyType)probabilities[i].ItemEnumValue);
			}
		}
	}

	public static void GetRadioCallNames(ref List<ItemAmountProbabilityData> probabilities, DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag dropTag, int controlLevel)
	{
		if (GameManager.Instance.gameEconomyData == null)
		{
			return;
		}
		Dictionary<int, List<CurrencyType>> rarityToHeroTokensMapping = GameManager.Instance.gameEconomyData.GetRarityToHeroTokensMapping(eventType, dropType, dropTag, controlLevel);
		for (int i = 0; i < ((probabilities != null) ? probabilities.Count : 0); i++)
		{
			ItemAmountProbabilityData itemAmountProbabilityData = probabilities[i];
			if (itemAmountProbabilityData.ItemEnumType == typeof(DropCurrenciesProbabilitiesDefinition.DropCurrency))
			{
				if (itemAmountProbabilityData.ItemEnumValue == 10)
				{
					itemAmountProbabilityData.Name = GetTokenNames(rarityToHeroTokensMapping[itemAmountProbabilityData.Rarity]);
				}
				else
				{
					itemAmountProbabilityData.Name = GetClassNames();
				}
			}
		}
	}

	private static string GetTokenNames(List<CurrencyType> heroTokens)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < heroTokens.Count; i++)
		{
			stringBuilder.Append(heroTokens[i].ToString());
			if (i < heroTokens.Count - 1)
			{
				stringBuilder.Append(",");
			}
		}
		return stringBuilder.ToString();
	}

	private static string GetClassNames()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (GameManager.Instance.gameEconomyData != null && GameManager.Instance.playerModel != null)
		{
			SurvivorClass[] array = GameManager.Instance.gameEconomyData.ConfigData.ParseSurvivorClassUnlockOrder();
			for (int i = 0; i < array.Length; i++)
			{
				if (GameManager.Instance.playerModel.SurvivorContainer.IsSurvivorClassUnlocked(array[i]))
				{
					stringBuilder.Append(array[i].ToString());
					if (i < array.Length - 1)
					{
						stringBuilder.Append(",");
					}
				}
			}
		}
		return stringBuilder.ToString();
	}
}
