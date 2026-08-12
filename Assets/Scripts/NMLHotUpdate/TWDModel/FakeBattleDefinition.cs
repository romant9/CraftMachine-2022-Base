using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class FakeBattleDefinition
	{
		public string OpponentName;

		public string Tiers;

		public int TargetScore;

		[JsonIgnore]
		public int[] tiersDifficulty;
	}
}
