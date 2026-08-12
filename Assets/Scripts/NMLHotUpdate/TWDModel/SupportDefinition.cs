using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SupportDefinition
	{
		private const int MaxExpectedLevel = 10;

		private readonly int[] tokensToUnlocks;

		private readonly FixedPoint[,] parameters;

		private readonly int[] cooldowns;

		private readonly int[,] supportTalentTrees;

		private readonly int[] supportTalentSlots;

		public int Category;

		private readonly int[] ChallengeCooldowns;

		private readonly int[] DistanceCooldowns;

		private readonly int[] GVGCooldowns;

		private readonly string[] UpgradeCost;

		private readonly int[] InnerCooldown;

		[JsonIgnore]
		public Dictionary<CurrencyType, int> Cost;

		public string Identifier { get; }

		public int Index { get; }

		public CurrencyType Currency { get; }

		public int ParameterCount { get; }

		public int MaxLevel { get; private set; }

		public int SupportTalentTreeCount { get; }

		public Dictionary<CurrencyType, int> GetUpgradCostInfo(int level)
		{
			if (UpgradeCost != null)
			{
				Cost = new Dictionary<CurrencyType, int>();
				int num = Math.Min(level, UpgradeCost.Length - 1);
				if (num <= 0)
				{
					num = 0;
				}
				if (level == MaxLevel)
				{
					return Cost;
				}
				string[] array = UpgradeCost[num].Split(';');
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

		public SupportDefinition(string identifier, int index, int parameterCount, int supportTalentTreeCount)
		{
			Index = index;
			Identifier = identifier;
			ParameterCount = parameterCount;
			cooldowns = new int[10];
			tokensToUnlocks = new int[10];
			UpgradeCost = new string[10];
			parameters = new FixedPoint[10, parameterCount];
			Currency = (CurrencyType)Enum.Parse(typeof(CurrencyType), identifier + "Token");
			SupportTalentTreeCount = supportTalentTreeCount;
			supportTalentTrees = new int[10, supportTalentTreeCount];
			supportTalentSlots = new int[10];
			ChallengeCooldowns = new int[10];
			DistanceCooldowns = new int[10];
			GVGCooldowns = new int[10];
			InnerCooldown = new int[10];
		}

		public void SetLevelData(int level, int tokensToUnlock, int cooldown, string[] rawParameters, string[] rawSupportTalentTrees, int supportTalentSlot, int ChallengeCooldown, int DistanceCooldown, int GVGCooldown, int category, string upgradeCost, int innerCooldown)
		{
			tokensToUnlocks[level - 1] = tokensToUnlock;
			cooldowns[level - 1] = cooldown;
			ChallengeCooldowns[level - 1] = ChallengeCooldown;
			DistanceCooldowns[level - 1] = DistanceCooldown;
			GVGCooldowns[level - 1] = GVGCooldown;
			Category = category;
			UpgradeCost[level - 1] = upgradeCost;
			InnerCooldown[level - 1] = innerCooldown;
			for (int i = 0; i < rawParameters.Length; i++)
			{
				parameters[level - 1, i] = new FixedPoint(rawParameters[i]);
			}
			MaxLevel = Math.Max(MaxLevel, level);
			for (int j = 0; j < rawSupportTalentTrees.Length; j++)
			{
				supportTalentTrees[level - 1, j] = int.Parse(rawSupportTalentTrees[j]);
			}
			supportTalentSlots[level - 1] = supportTalentSlot;
		}

		public FixedPoint GetParameter(int level, int parameterIndex)
		{
			return parameters[Math.Max(level - 1, 0), parameterIndex];
		}

		public int GetTokensToUnlock(int level)
		{
			return tokensToUnlocks[Math.Min(level, tokensToUnlocks.Length - 1)];
		}

		public int GetCooldown(int level)
		{
			return cooldowns[Math.Max(level - 1, 0)];
		}

		public int GetChallengeCooldown(int level)
		{
			return ChallengeCooldowns[Math.Max(level - 1, 0)];
		}

		public int GetDistanceCooldown(int level)
		{
			return DistanceCooldowns[Math.Max(level - 1, 0)];
		}

		public int GetGVGCooldown(int level)
		{
			return GVGCooldowns[Math.Max(level - 1, 0)];
		}

		public int GetInnerCooldown(int level)
		{
			return InnerCooldown[Math.Max(level - 1, 0)];
		}

		public int[] GetSupportTalentTreesByLevel(int level)
		{
			int[] array = new int[SupportTalentTreeCount];
			int num = Math.Max(level - 1, 0);
			for (int i = 0; i < SupportTalentTreeCount; i++)
			{
				array[i] = supportTalentTrees[num, i];
			}
			return array;
		}

		public int GetSupportTalentSlotByLevel(int level)
		{
			return supportTalentSlots[Math.Max(level - 1, 0)];
		}

		public int[] GetSupportTalentSlots()
		{
			return supportTalentSlots;
		}
	}
}
