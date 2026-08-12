using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WeeklyChallengeRoundPassConfig
	{
		public int FromRound;

		public int ToRound;

		public int RoundsToSkipToken;

		[JsonIgnore]
		public int FullCycleTokens => ToRound / RoundsToSkipToken;
	}
}
