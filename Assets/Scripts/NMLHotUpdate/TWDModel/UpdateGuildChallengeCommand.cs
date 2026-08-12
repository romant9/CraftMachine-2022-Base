using BaseModel;

namespace TWDModel
{
	public class UpdateGuildChallengeCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			WeeklyChallengeModel weeklyChallenge = ((TWDModelManager)manager).Player.WeeklyChallenge;
			TWDModelResult result = TWDModelResult.OK;
			weeklyChallenge?.UpdateGuildChallenge();
			return new NGModelCommandRespond(this, result);
		}
	}
}
