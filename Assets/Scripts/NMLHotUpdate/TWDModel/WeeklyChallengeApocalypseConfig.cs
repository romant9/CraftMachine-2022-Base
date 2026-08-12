using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WeeklyChallengeApocalypseConfig
	{
		public int Round;

		public int Difficulty;

		public int MissionLevel;

		public IncrementalDifficultyEffect IncrementalDifficulty;

		public int ConstructionParameters;

		public List<string> Debuff;

		public List<string> BaseDebuff;

		public List<string> LTDebuff;

		public string Buff;

		public int GasCost;

		private List<DifficultyIncrementalDebuff> debuffconfs;

		private List<DifficultyIncrementalDebuff> basedebuffconfs;

		private List<DifficultyIncrementalDebuff> lTDebuffs;

		[JsonIgnore]
		public List<DifficultyIncrementalDebuff> DebuffConfigs => debuffconfs;

		[JsonIgnore]
		public List<DifficultyIncrementalDebuff> BaseDebuffConfigs => basedebuffconfs;

		[JsonIgnore]
		public List<DifficultyIncrementalDebuff> LTDebuffs => lTDebuffs;

		public void SetDebuffConfs(List<DifficultyIncrementalDebuff> debuffs)
		{
			debuffconfs = debuffs;
		}

		public void SetBaseDebuffConfs(List<DifficultyIncrementalDebuff> debuffs)
		{
			basedebuffconfs = debuffs;
		}

		public void SetlTDebuffss(List<DifficultyIncrementalDebuff> debuffs)
		{
			lTDebuffs = debuffs;
		}
	}
}
