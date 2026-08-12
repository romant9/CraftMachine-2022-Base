using BaseModel;

namespace TWDModel
{
	public class MoveFinishedCommand : ModelCommand
	{
		public MoveFinishedCommand()
		{
		}

		public MoveFinishedCommand(BuildingModel building)
			: base(building)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			manager.GetModel<BuildingModel>(base.ModelId).CampMoved = false;
			(manager.GetPlayer() as PlayerModel).Blackboard.ClearToggle("Toggle.CampMoved");
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
