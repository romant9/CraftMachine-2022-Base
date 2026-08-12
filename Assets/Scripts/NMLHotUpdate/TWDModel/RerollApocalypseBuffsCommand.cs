using BaseModel;

namespace TWDModel
{
	public class RerollApocalypseBuffsCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager { Player: not null } tWDModelManager && tWDModelManager.Player.WeeklyChallenge != null)
			{
				TWDModelResult result = tWDModelManager.Player.ApocalypseWeeklyChallenge.RerollApocalypse();
				return new NGModelCommandRespond(this, result);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
