using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WeeklyChallengeDeBuffSet
	{
		public int Round;

		public int Difficulty;

		public List<string> Debuff;

		private List<DifficultyIncrementalDebuff> debuffconfs;

		[JsonIgnore]
		public List<DifficultyIncrementalDebuff> DebuffConfigs => debuffconfs;

		public void SetDebuffConfs(List<DifficultyIncrementalDebuff> debuffs)
		{
			debuffconfs = debuffs;
		}
	}
}
