using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ClaimChallengeRewardsCommand : ModelCommand
	{
		[JsonIgnore]
		public List<LootEntry> LootEntries { get; set; }

		public LootEntryType[] LootEntryType { get; private set; }

		public bool ClaimWeeklyChallengeClassTeamSkipRewards { get; set; }

		public ClaimChallengeRewardsCommand()
		{
			LootEntries = new List<LootEntry>();
		}

		public ClaimChallengeRewardsCommand(params LootEntryType[] type)
		{
			LootEntryType = type;
			LootEntries = new List<LootEntry>();
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			PlayerModel obj = manager.GetPlayer() as PlayerModel;
			WeeklyChallengeModel weeklyChallenge = obj.WeeklyChallenge;
			WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivity = obj.WeeklyChallengeClassTeamActivity;
			LootEntryType[] lootEntryType = LootEntryType;
			TWDModelResult tWDModelResult = TWDModelResult.OK;
			if (weeklyChallenge != null && (weeklyChallenge.CanCollectRewards || weeklyChallenge.CanCollectApocalypticRewards))
			{
				tWDModelResult = weeklyChallenge.GiveRewardsPerType(lootEntryType, LootEntries);
			}
			if (tWDModelResult == TWDModelResult.OK && weeklyChallengeClassTeamActivity != null && ClaimWeeklyChallengeClassTeamSkipRewards)
			{
				weeklyChallengeClassTeamActivity.ClaimPendingSkipRewards(LootEntries);
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
