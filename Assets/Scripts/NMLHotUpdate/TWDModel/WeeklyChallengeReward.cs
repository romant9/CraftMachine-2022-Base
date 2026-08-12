using System;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class WeeklyChallengeReward
	{
		public enum ChallengeRewardType
		{
			None = 0,
			PersonalStars = 1,
			GuildStars = 2,
			RoundCompletion = 3,
			PersonalHighScore = 4,
			GuildAchiever = 5,
			ApocalypticStars = 6,
			ApocalypticRoundStars = 7
		}

		public ChallengeRewardType RewardType;

		public int Control;

		public string Rewards;

		public FixedPoint BonusStarsMultiplier;

		[NonSerialized]
		[JsonIgnore]
		public Rewards RewardEntries;
	}
}
