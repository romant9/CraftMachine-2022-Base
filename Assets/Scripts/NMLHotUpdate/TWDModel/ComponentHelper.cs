using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class ComponentHelper
	{
		public static CurrencyType[][] ComponentCurrencies = new CurrencyType[5][]
		{
			new CurrencyType[5]
			{
				CurrencyType.Metal0,
				CurrencyType.Metal1,
				CurrencyType.Metal2,
				CurrencyType.Metal3,
				CurrencyType.Metal4
			},
			new CurrencyType[5]
			{
				CurrencyType.Food0,
				CurrencyType.Food1,
				CurrencyType.Food2,
				CurrencyType.Food3,
				CurrencyType.Food4
			},
			new CurrencyType[5]
			{
				CurrencyType.Chemicals0,
				CurrencyType.Chemicals1,
				CurrencyType.Chemicals2,
				CurrencyType.Chemicals3,
				CurrencyType.Chemicals4
			},
			new CurrencyType[5]
			{
				CurrencyType.Cloth0,
				CurrencyType.Cloth1,
				CurrencyType.Cloth2,
				CurrencyType.Cloth3,
				CurrencyType.Cloth4
			},
			new CurrencyType[5]
			{
				CurrencyType.Badge0,
				CurrencyType.Badge1,
				CurrencyType.Badge2,
				CurrencyType.Badge3,
				CurrencyType.Badge4
			}
		};

		private static bool FindCurrencyIndex(CurrencyType currency, out int arrayIndex, out int index)
		{
			for (int i = 0; i < ComponentCurrencies.Length; i++)
			{
				int num = Array.IndexOf(ComponentCurrencies[i], currency);
				if (num != -1)
				{
					arrayIndex = i;
					index = num;
					return true;
				}
			}
			arrayIndex = -1;
			index = -1;
			return false;
		}

		public static bool CanSubstituteComponent(CurrencyType baseCurrency, CurrencyType otherCurrency)
		{
			if (FindCurrencyIndex(baseCurrency, out var arrayIndex, out var index) && FindCurrencyIndex(otherCurrency, out var arrayIndex2, out var index2))
			{
				if (arrayIndex == arrayIndex2)
				{
					return index2 >= index;
				}
				return false;
			}
			return false;
		}

		public static int GetComponentRarityLevel(CurrencyType currency)
		{
			if (FindCurrencyIndex(currency, out var _, out var index))
			{
				return index;
			}
			return -1;
		}

		public static string GetComponentTypeName(CurrencyType currency)
		{
			if (IsComponentCurrency(currency))
			{
				string text = currency.ToString();
				return text.Substring(0, text.Length - 1);
			}
			return null;
		}

		public static CurrencyType GetComponentBaseCurrency(CurrencyType currency)
		{
			if (FindCurrencyIndex(currency, out var arrayIndex, out var _))
			{
				return ComponentCurrencies[arrayIndex][0];
			}
			return CurrencyType.None;
		}

		public static CurrencyType GetCurrencyFromBaseAndRarity(CurrencyType baseCurrency, int rarity)
		{
			if (FindCurrencyIndex(baseCurrency, out var arrayIndex, out var _) && rarity >= 0 && rarity < ComponentCurrencies[arrayIndex].Length)
			{
				return ComponentCurrencies[arrayIndex][rarity];
			}
			return CurrencyType.None;
		}

		public static bool IsComponentCurrency(CurrencyType c)
		{
			for (int i = 0; i < ComponentCurrencies.Length; i++)
			{
				if (Array.IndexOf(ComponentCurrencies[i], c) != -1)
				{
					return true;
				}
			}
			return false;
		}

		public static List<CurrencyType> GetAllComponentCurrencies()
		{
			List<CurrencyType> list = new List<CurrencyType>();
			for (int i = 0; i < ComponentCurrencies.Length; i++)
			{
				list.AddRange(ComponentCurrencies[i]);
			}
			return list;
		}

		public static List<CurrencyType> GetAllComponentBaseCurrencies()
		{
			List<CurrencyType> list = new List<CurrencyType>();
			for (int i = 0; i < ComponentCurrencies.Length; i++)
			{
				list.Add(ComponentCurrencies[i][0]);
			}
			return list;
		}

		public static bool IsSpeedUpToken(CurrencyType currencyType)
		{
			switch (currencyType)
			{
			case CurrencyType.BuildingTokenBP:
			case CurrencyType.SuperBuildingTokenBP:
			case CurrencyType.TrainingTokenBP:
			case CurrencyType.SuperTrainingTokenBP:
			case CurrencyType.EquipmentTokenBP:
			case CurrencyType.SuperEquipmentTokenBP:
			case CurrencyType.HealingTokenBP:
			case CurrencyType.BuildingToken10min:
			case CurrencyType.BuildingToken1h:
			case CurrencyType.BuildingToken6h:
			case CurrencyType.BuildingToken12h:
			case CurrencyType.BuildingToken24h:
			case CurrencyType.TrainingToken20min:
			case CurrencyType.TrainingToken1h:
			case CurrencyType.TrainingToken3h:
			case CurrencyType.TrainingToken8h:
			case CurrencyType.TrainingToken16h:
			case CurrencyType.EquipmentToken20min:
			case CurrencyType.EquipmentToken1h:
			case CurrencyType.EquipmentToken3h:
			case CurrencyType.EquipmentToken7h:
			case CurrencyType.EquipmentToken14h:
			case CurrencyType.HealingToken10min:
			case CurrencyType.HealingToken1h:
			case CurrencyType.HealingToken2h:
			case CurrencyType.HealingToken4h:
			case CurrencyType.TrainingTokenBP_N:
			case CurrencyType.EquipmentTokenBP_N:
			case CurrencyType.HealingTokenBP_N:
			case CurrencyType.BuildingTokenBP_N:
			case CurrencyType.BuildingToken1min:
			case CurrencyType.BuildingToken5min:
			case CurrencyType.BuildingToken30min:
			case CurrencyType.TrainingToken5min:
			case CurrencyType.EquipmentToken1min:
			case CurrencyType.EquipmentToken10min:
			case CurrencyType.HealingToken1min:
			case CurrencyType.HealingToken5min:
				return true;
			default:
				return false;
			}
		}
	}
}
