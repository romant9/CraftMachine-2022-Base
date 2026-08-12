using BaseModel;

namespace TWDModel
{
	public class ThreeDayNoPopCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			tWDModelManager.Player.ThreeDayModel.SetNoPopTime();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
