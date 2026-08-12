using BaseModel;

namespace TWDModel
{
	public class WeeklyChallengeSeenCommand : ModelCommand
	{
		public bool MarkActiveSkipTokensAsSeen { get; set; }

		public bool MarkChallengeStartedAsSeen { get; set; }

		public bool MarkChallengeEndedAsSeen { get; set; }

		public int PersonalStarsSeen { get; set; }

		public int GuildStarsSeen { get; set; }

		public int DifficultySeen { get; set; }

		public int CycleSeen { get; set; }

		public FixedPoint DifficultyProgressionSeen { get; set; }

		public WeeklyChallengeSeenCommand()
		{
		}

		public WeeklyChallengeSeenCommand(WeeklyChallengeModel weeklyChallengeModel)
			: base(weeklyChallengeModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager { Player: not null } tWDModelManager)
			{
				WeeklyChallengeModel model = tWDModelManager.GetModel<WeeklyChallengeModel>(base.ModelId);
				if (model != null)
				{
					if (MarkActiveSkipTokensAsSeen)
					{
						model.MarkSkipTokensAvailableSeen();
					}
					if (MarkChallengeStartedAsSeen)
					{
						model.MarkChallengeStartedAsSeen();
					}
					if (MarkChallengeEndedAsSeen)
					{
						model.MarkChallengeEndedAsSeen();
					}
					if (PersonalStarsSeen > 0)
					{
						model.LastSeenNumberStars = PersonalStarsSeen;
					}
					if (GuildStarsSeen > 0)
					{
						model.LastSeenNumberOfGuildStars = GuildStarsSeen;
					}
					if (DifficultySeen > 0)
					{
						model.LastSeenChallengeDifficulty = DifficultySeen;
					}
					if (CycleSeen > 0)
					{
						model.LastSeenCycleCount = CycleSeen;
					}
					if (DifficultyProgressionSeen > 0.0)
					{
						model.LastSeenChallengeDifficultyProgression = DifficultyProgressionSeen;
					}
					else if (DifficultyProgressionSeen < 0.0)
					{
						model.LastSeenChallengeDifficultyProgression = 0.0;
					}
				}
				else
				{
					tWDModelManager.Debug.Log("WeeklyChallengeSeenCommand: WeeklyChallengeModel is NULL!");
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
