using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class EndlessModeExpertDebuffConfig
	{
		public int Wave;

		public List<string> Debuff;

		private List<DifficultyIncrementalDebuff> endLessDebuffs;

		[JsonIgnore]
		public List<DifficultyIncrementalDebuff> EndLessDebuffs => endLessDebuffs;

		public void SetDebuffss(List<DifficultyIncrementalDebuff> debuffs)
		{
			endLessDebuffs = debuffs;
		}
	}
}
