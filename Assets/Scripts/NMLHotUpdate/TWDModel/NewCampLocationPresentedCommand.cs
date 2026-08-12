using BaseModel;

namespace TWDModel
{
	public class NewCampLocationPresentedCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			(manager.GetPlayer() as PlayerModel).Blackboard.ClearToggle("Toggle.NewCamp.Show");
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
