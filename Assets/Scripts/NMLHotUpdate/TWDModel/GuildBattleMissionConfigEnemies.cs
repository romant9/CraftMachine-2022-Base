using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class GuildBattleMissionConfigEnemies : GuildBattleMissionConfigBase
	{
		public const string BurningTypesKeyword = "BurningTypes";

		public const string BossTypesKeyKeyword = "BossTypes";

		public const string ColumnName = "Enemies";

		public Dictionary<WalkerType, int> WalkerAmount = new Dictionary<WalkerType, int>();

		public string BurningTypes;

		public string BossTypes;

		public int GetAmountForType(WalkerType type, string objectiveBossType = "")
		{
			int num = 0;
			bool flag = true;
			if (!string.IsNullOrEmpty(objectiveBossType))
			{
				flag = !type.ToString().Contains(objectiveBossType);
			}
			if (WalkerAmount.ContainsKey(type))
			{
				num = WalkerAmount[type];
			}
			if (!flag && num == 0)
			{
				num = 1;
			}
			return num;
		}

		public override bool Parse(ref string wrapperName, ref string stringParams, ref int[] intParams, ref string errorMessage)
		{
			if (!base.Parse(ref wrapperName, ref stringParams, ref intParams, ref errorMessage))
			{
				errorMessage = "Enemies, base NULL check";
				return false;
			}
			if (Enum.IsDefined(typeof(WalkerType), wrapperName) && intParams[0] != -1)
			{
				WalkerType key = (WalkerType)Enum.Parse(typeof(WalkerType), wrapperName);
				int value = 0;
				if (!WalkerAmount.TryGetValue(key, out value))
				{
					WalkerAmount.Add(key, 0);
				}
				value += intParams[0];
				WalkerAmount[key] = value;
			}
			else if (wrapperName == "BurningTypes")
			{
				stringParams = stringParams.Replace("Walker", "");
				if (!string.IsNullOrEmpty(BurningTypes))
				{
					BurningTypes = BurningTypes + ", " + stringParams;
				}
				else
				{
					BurningTypes = stringParams;
				}
			}
			else if (wrapperName == "BossTypes")
			{
				stringParams = stringParams.Replace("Walker", "");
				if (!string.IsNullOrEmpty(BossTypes))
				{
					BossTypes = BossTypes + ", " + stringParams;
				}
				else
				{
					BossTypes = stringParams;
				}
			}
			return IsValid();
		}

		public override bool IsValid()
		{
			return WalkerAmount.Count != 0;
		}
	}
}
