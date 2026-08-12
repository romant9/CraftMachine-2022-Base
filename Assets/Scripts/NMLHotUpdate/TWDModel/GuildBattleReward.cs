using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class GuildBattleReward
	{
		public enum GuildRewardType
		{
			None = 0,
			BattleWin = 1,
			BattleLost = 2,
			SectorCompletion = 3,
			SectorBonus = 4,
			MissionCompletion = 5
		}

		public enum GuildRewardControlType
		{
			None = 0,
			Column0 = 1,
			Column1 = 2,
			Column2 = 3,
			ColumnSuperHard = 4
		}

		private const string TraitBonus = "TraitBonus";

		public GuildRewardType RewardType;

		public string SetName;

		public int SectorId;

		public GuildRewardControlType ControlType;

		public string RewardsRandomPool;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;

		[NonSerialized]
		[JsonIgnore]
		public List<GuildBattleRewardEntry> RewardsPoolParsed;

		private List<GuildBattleRewardEntry> LoadDefinition(string definitionString)
		{
			List<GuildBattleRewardEntry> list = new List<GuildBattleRewardEntry>();
			if (string.IsNullOrEmpty(definitionString))
			{
				return null;
			}
			string[] array = definitionString.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				GuildBattleRewardEntry guildBattleRewardEntry = new GuildBattleRewardEntry();
				string[] array2 = array[i].Trim().Split(',');
				guildBattleRewardEntry.Reward = array2[0];
				guildBattleRewardEntry.Unique = array2[0].Contains("TraitBonus");
				guildBattleRewardEntry.Weight = 100;
				if (array2.Length == 2 && int.TryParse(array2[1], out var result))
				{
					guildBattleRewardEntry.Weight = result;
				}
				list.Add(guildBattleRewardEntry);
			}
			return list;
		}

		public void LoadDefinitions()
		{
			RewardsPoolParsed = LoadDefinition(RewardsRandomPool);
		}

		public FixedPoint[] GetRewardPoolWeights(List<int> filterRewardsIndex = null)
		{
			if (RewardsPoolParsed == null || RewardsPoolParsed.Count == 0)
			{
				return null;
			}
			FixedPoint[] array = new FixedPoint[RewardsPoolParsed.Count];
			for (int i = 0; i < array.Length; i++)
			{
				if (filterRewardsIndex != null && filterRewardsIndex.Contains(i))
				{
					array[i] = 0L;
				}
				else
				{
					array[i] = RewardsPoolParsed[i].Weight;
				}
			}
			return array;
		}
	}
}
