using BaseModel;

namespace TWDModel
{
	public class UpdateChallengeRewardsCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager.GetPlayer() as PlayerModel).WeeklyChallenge.CheckGuildStarsReward();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
