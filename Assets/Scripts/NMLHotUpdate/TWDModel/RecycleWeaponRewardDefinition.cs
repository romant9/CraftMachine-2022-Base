using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class RecycleWeaponRewardDefinition
	{
		public int Identifier;

		public int Type;

		public int Level;

		public string CBP;

		public List<string> SPSkillPackage;

		public int ApoToken;

		public string RewardShow;

		public List<string> RewardShowPic;

		[NonSerialized]
		[JsonIgnore]
		public List<SPSkillPackageEntry> SPSkillPackageEntries;

		[NonSerialized]
		[JsonIgnore]
		public List<RewardShowPicEntry> RewardShowPicEntries;

		public Rewards CbpRewards;

		public void CalcReward()
		{
			SPSkillPackageEntries = new List<SPSkillPackageEntry>();
			if (SPSkillPackage != null)
			{
				foreach (string item in SPSkillPackage)
				{
					if (!string.IsNullOrEmpty(item))
					{
						string[] array = item.Split(':');
						if (array.Length >= 2 && int.TryParse(array[1], out var result))
						{
							SPSkillPackageEntries.Add(new SPSkillPackageEntry(array[0], result));
						}
					}
				}
			}
			RewardShowPicEntries = new List<RewardShowPicEntry>();
			if (RewardShowPic != null)
			{
				foreach (string item2 in RewardShowPic)
				{
					if (!string.IsNullOrEmpty(item2))
					{
						string[] array2 = item2.Split(':');
						if (array2.Length >= 3 && int.TryParse(array2[1], out var result2) && int.TryParse(array2[2], out var result3))
						{
							RewardShowPicEntries.Add(new RewardShowPicEntry(array2[0], result2, result3));
						}
					}
				}
			}
			if (CBP != null)
			{
				CbpRewards = new Rewards(CBP);
			}
		}
	}
}
