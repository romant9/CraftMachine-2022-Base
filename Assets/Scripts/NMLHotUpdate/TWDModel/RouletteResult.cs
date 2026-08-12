using System;
using System.Collections.Generic;
using System.Linq;

namespace TWDModel
{
	[Serializable]
	public class RouletteResult
	{
		public List<RouletteDefinition> MainRewards = new List<RouletteDefinition>();

		public List<RouletteDefinition> GetAllRewards = new List<RouletteDefinition>();

		public List<RouletteDefinition> Type2Rewards = new List<RouletteDefinition>();

		public bool HasGetAllReward;

		public int DrawnSlotIndex = -1;

		public int DrawnType2SlotIndex = -1;

		public bool IsEnd;

		public void AddMainReward(RouletteDefinition reward)
		{
			if (reward != null)
			{
				if (MainRewards == null)
				{
					MainRewards = new List<RouletteDefinition>();
				}
				MainRewards.Add(reward);
				DrawnSlotIndex = reward.SlotsIndex;
			}
			else
			{
				DrawnSlotIndex = -1;
			}
		}

		public void AddGetAllReward(RouletteDefinition reward)
		{
			if (reward != null)
			{
				if (GetAllRewards == null)
				{
					GetAllRewards = new List<RouletteDefinition>();
				}
				GetAllRewards.Add(reward);
				HasGetAllReward = true;
			}
		}

		public void AddType2Reward(RouletteDefinition reward)
		{
			if (reward != null)
			{
				if (Type2Rewards == null)
				{
					Type2Rewards = new List<RouletteDefinition>();
				}
				Type2Rewards.Add(reward);
				DrawnType2SlotIndex = reward.SlotsIndex;
			}
			else
			{
				DrawnType2SlotIndex = -1;
			}
		}

		public List<RouletteDefinition> GetAllRewardsList()
		{
			List<RouletteDefinition> list = new List<RouletteDefinition>();
			List<RouletteDefinition> mainRewards = MainRewards;
			if (mainRewards != null && mainRewards.Count > 0)
			{
				list.AddRange(MainRewards.Where((RouletteDefinition x) => x != null));
			}
			List<RouletteDefinition> getAllRewards = GetAllRewards;
			if (getAllRewards != null && getAllRewards.Count > 0)
			{
				list.AddRange(GetAllRewards.Where((RouletteDefinition x) => x != null));
			}
			List<RouletteDefinition> type2Rewards = Type2Rewards;
			if (type2Rewards != null && type2Rewards.Count > 0)
			{
				list.AddRange(Type2Rewards.Where((RouletteDefinition x) => x != null));
			}
			return list;
		}
	}
}
