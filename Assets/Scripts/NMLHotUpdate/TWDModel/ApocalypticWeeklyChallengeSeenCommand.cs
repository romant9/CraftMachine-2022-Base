using BaseModel;

namespace TWDModel
{
	public class ApocalypticWeeklyChallengeSeenCommand : ModelCommand
	{
		public int PersonalStarsSeen { get; set; }

		public int DifficultySeen { get; set; }

		public FixedPoint DifficultyProgressionSeen { get; set; }

		public int CycleSeen { get; set; }

		public bool MarkActiveSkipTokensAsSeen { get; set; }

		public ApocalypticWeeklyChallengeSeenCommand()
		{
		}

		public ApocalypticWeeklyChallengeSeenCommand(ApocalypseWeeklyChallengeModel apocalypseWeeklyChallengeModel)
			: base(apocalypseWeeklyChallengeModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager { Player: not null } tWDModelManager)
			{
				ApocalypseWeeklyChallengeModel model = tWDModelManager.GetModel<ApocalypseWeeklyChallengeModel>(base.ModelId);
				if (model != null)
				{
					if (MarkActiveSkipTokensAsSeen)
					{
						model.MarkSkipTokensAvailableSeen();
					}
					if (PersonalStarsSeen > 0)
					{
						model.LastSeenNumberStars = PersonalStarsSeen;
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
