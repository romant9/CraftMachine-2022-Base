using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class PhoneCallDefinition
	{
		public string VisualOverride;

		public int SlotNumber;

		public SurvivorClass SurvivorClass;

		public string CurrencyTypes;

		public string CurrencyTypesDistribution;

		public DropType DropType;

		public bool HeroGuaranteed;

		public int Rerolls;

		public int InitialProbabilityPercentage;

		public int ProbabilityPercentageIncrease;

		public int Price;

		public string HeroTokensDropNumber;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string StartTimeUtc;

		[GEDType(GEDSpecialType.AddAdvanceSecs)]
		public string EndTimeUtc;

		public bool CanCallOwnedHeroes;

		private CurrencyType[] parsedCurrencyTypeValues;

		private int[] parsedCurrencyTypeDistributionValues;

		private int parsedTotalWeight = -1;

		[JsonIgnore]
		private long _StartTimeMilliseconds;

		[JsonIgnore]
		private long _EndTimeMilliseconds;

		[JsonIgnore]
		public long StartTimeMilliseconds
		{
			get
			{
				if (_StartTimeMilliseconds > 0)
				{
					return _StartTimeMilliseconds;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return _StartTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(StartTimeUtc) - dateTime).TotalSeconds * 1000;
			}
		}

		[JsonIgnore]
		public long EndTimeMilliseconds
		{
			get
			{
				if (_EndTimeMilliseconds > 0)
				{
					return _EndTimeMilliseconds;
				}
				DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();
				return _EndTimeMilliseconds = (long)(GameEconomyData.ParseDateTime(EndTimeUtc) - dateTime).TotalSeconds * 1000;
			}
		}

		[JsonIgnore]
		public PhoneCallDefinitionType Type
		{
			get
			{
				if (HeroGuaranteed)
				{
					return PhoneCallDefinitionType.GuaranteedHero;
				}
				CurrencyType[] array = GetParsedCurrencyTypeValues();
				if (array.Length >= 1 && (InitialProbabilityPercentage > 0 || ProbabilityPercentageIncrease > 0))
				{
					if (array.Length <= 1)
					{
						return PhoneCallDefinitionType.BetterChanceOfHero;
					}
					return PhoneCallDefinitionType.BetterChanceOfMultipleHeroes;
				}
				if (SurvivorClass != SurvivorClass.None && (InitialProbabilityPercentage > 0 || ProbabilityPercentageIncrease > 0))
				{
					return PhoneCallDefinitionType.BetterChanceOfSurvivor;
				}
				return PhoneCallDefinitionType.None;
			}
		}

		public CurrencyType[] GetParsedCurrencyTypeValues()
		{
			if (parsedCurrencyTypeValues == null)
			{
				bool parseError = false;
				parsedCurrencyTypeValues = ParseCurrencyTypeValues(out parseError);
			}
			return parsedCurrencyTypeValues;
		}

		public CurrencyType[] ParseCurrencyTypeValues(out bool parseError)
		{
			if (CurrencyTypes == null || CurrencyTypes == "")
			{
				parseError = false;
				return new CurrencyType[0];
			}
			string[] names = Enum.GetNames(typeof(CurrencyType));
			string[] array = CurrencyTypes.Split(';');
			CurrencyType[] array2 = new CurrencyType[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				_ = text == "";
				bool flag = false;
				for (int j = 0; j < names.Length; j++)
				{
					if (text == names[j])
					{
						array2[i] = (CurrencyType)j;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					parseError = true;
					return new CurrencyType[0];
				}
			}
			parseError = false;
			return array2;
		}

		public List<int> getHreoKensDropNumberValues(out bool parseError)
		{
			string[] array = HeroTokensDropNumber.Split(';');
			List<int> list = new List<int>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				if (int.TryParse(array2[i], out var result))
				{
					list.Add(result);
					continue;
				}
				parseError = true;
				return new List<int>();
			}
			parseError = false;
			return list;
		}

		public int[] GetParsedCurrencyTypeDistributionValues()
		{
			if (parsedCurrencyTypeDistributionValues == null)
			{
				bool parseError = false;
				int[] array = ParseCurrencyTypeDistributionValues(out parseError);
				int num = GetParsedCurrencyTypeValues().Length;
				if (array.Length == num)
				{
					parsedCurrencyTypeDistributionValues = array;
				}
				else
				{
					parsedCurrencyTypeDistributionValues = new int[num];
					int num2 = Math.Min(array.Length, parsedCurrencyTypeDistributionValues.Length);
					for (int i = 0; i < num2; i++)
					{
						parsedCurrencyTypeDistributionValues[i] = array[i];
					}
					for (int j = num2; j < parsedCurrencyTypeDistributionValues.Length; j++)
					{
						parsedCurrencyTypeDistributionValues[j] = 1;
					}
				}
			}
			return parsedCurrencyTypeDistributionValues;
		}

		public int GetParsedCurrencyTypeDistributionTotalWeight()
		{
			if (parsedTotalWeight < 0)
			{
				int num = 0;
				int[] array = GetParsedCurrencyTypeDistributionValues();
				for (int i = 0; i < array.Length; i++)
				{
					num += array[i];
				}
				parsedTotalWeight = num;
			}
			return parsedTotalWeight;
		}

		public int[] ParseCurrencyTypeDistributionValues(out bool parseError)
		{
			if (CurrencyTypesDistribution == null)
			{
				parseError = false;
				return new int[0];
			}
			string[] array = CurrencyTypesDistribution.Split(';');
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string s = array[i];
				int result = 1;
				if (!int.TryParse(s, out result))
				{
					parseError = true;
					return new int[0];
				}
				array2[i] = result;
			}
			parseError = false;
			return array2;
		}


		#region mycode
		public int[] ParseHeroTokensDropNumberValues(out bool parseError)
		{
			if (HeroTokensDropNumber == null)
			{
				parseError = false;
				return new int[0];
			}
			string[] array = HeroTokensDropNumber.Split(';');
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				string s = array[i];
				int result = 1;
				if (!int.TryParse(s, out result))
				{
					parseError = true;
					return new int[0];
				}
				array2[i] = result;
			}
			parseError = false;
			return array2;
		}
		#endregion
	}
}
