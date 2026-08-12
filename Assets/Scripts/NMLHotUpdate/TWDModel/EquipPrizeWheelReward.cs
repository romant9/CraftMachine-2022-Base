using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class EquipPrizeWheelReward
	{
		public string ID;

		public string Slot;

		public string Reward;

		public int bigPrize;

		public int bigPrizeTime;

		public string Rarity;

		public string Weight;

		private List<EquipPrizeWheelWeight> realWeight;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;

		public int GetWeight(int time)
		{
			return realWeight.Last((EquipPrizeWheelWeight x) => x.Time <= time).Weight;
		}

		public void SetupWeightAndReward()
		{
			RewardEntries = new Rewards(Reward);
			string[] source = Weight.Split(';');
			realWeight = (from x in source
				select EquipPrizeWheelWeight.Parse(x) into x
				orderby x.Time
				select x).ToList();
		}
	}
}
