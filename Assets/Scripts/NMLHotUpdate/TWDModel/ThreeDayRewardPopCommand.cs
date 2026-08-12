using BaseModel;

namespace TWDModel
{
	public class ThreeDayRewardPopCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Player.ThreeDayModel.ClearRewardPop();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
