using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class RouletteDefinition : IWeightedItem
	{
		public int UniqueId;

		public int EventPeriod;

		public int SlotsIndex;

		public int RouletteType;

		public int Limitation;

		public int SlotsWeight;

		public string Rewards;

		//[JsonIgnore]
		public Rewards RewardsObj { get; private set; }

		public void InitializeRewards(TWDModelManager manager, int playerLevel = 1)
		{
			if (manager == null || RewardsObj != null || string.IsNullOrEmpty(Rewards) || !(Rewards != "GetALL"))
			{
				return;
			}
			try
			{
				RewardsObj = new Rewards(Rewards, manager, playerLevel);
			}
			catch (Exception)
			{
				RewardsObj = null;
			}
		}

		public int GetWeight()
		{
			return SlotsWeight;
		}

		public bool ShouldIncludeWeight(int currentDrawCount)
		{
			if (Limitation == -1)
			{
				return true;
			}
			if (Limitation > 0)
			{
				return currentDrawCount > Limitation;
			}
			return false;
		}

		public override string ToString()
		{
			return $"RouletteDefinition[UniqueId={UniqueId}, EventPeriod={EventPeriod}, SlotsIndex={SlotsIndex}, Type={RouletteType}, Weight={SlotsWeight}, Limitation={Limitation}]";
		}

		public bool IsLeftRoulette()
		{
			return RouletteType == 2;
		}

		public bool IsRightRoulette()
		{
			return RouletteType == 1;
		}

		public string GetDisplayName()
		{
			if (IsRightRoulette())
			{
				return $"Right Slot {SlotsIndex}";
			}
			if (IsLeftRoulette())
			{
				return $"Left Slot {SlotsIndex}";
			}
			return $"Slot {SlotsIndex}";
		}

		public Dictionary<CurrencyType, int> GetRewardInfo()
		{
			Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
			if (string.IsNullOrEmpty(Rewards))
			{
				return dictionary;
			}
			string[] array = Rewards.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				if (string.IsNullOrEmpty(array[i]))
				{
					continue;
				}
				string[] array2 = array[i].Split('(');
				if (array2.Length < 2)
				{
					continue;
				}
				string text = array2[0].ToLowerInvariant();
				if (array2.Length > 1)
				{
					array2[1] = array2[1].Replace(")", "");
				}
				if (text == "gold")
				{
					try
					{
						if (array2.Length > 1)
						{
							dictionary.Add(CurrencyType.Diamonds, int.Parse(array2[1]));
						}
					}
					catch (Exception)
					{
					}
					continue;
				}
				try
				{
					CurrencyType key = (CurrencyType)Enum.Parse(typeof(CurrencyType), array2[0]);
					if (array2.Length > 1)
					{
						dictionary.Add(key, int.Parse(array2[1]));
					}
				}
				catch (Exception)
				{
				}
			}
			return dictionary;
		}

		public int GetRewardAmountByCurrencyType(CurrencyType currencyType)
		{
			Dictionary<CurrencyType, int> rewardInfo = GetRewardInfo();
			if (rewardInfo != null && rewardInfo.ContainsKey(currencyType))
			{
				return rewardInfo[currencyType];
			}
			return 0;
		}

		public bool ContainsCurrencyType(CurrencyType currencyType)
		{
			return GetRewardInfo()?.ContainsKey(currencyType) ?? false;
		}

		public List<CurrencyType> GetRewardCurrencyTypes()
		{
			Dictionary<CurrencyType, int> rewardInfo = GetRewardInfo();
			if (rewardInfo != null)
			{
				return new List<CurrencyType>(rewardInfo.Keys);
			}
			return new List<CurrencyType>();
		}
	}
}
