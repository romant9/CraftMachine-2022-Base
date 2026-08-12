using BaseModel;

namespace TWDModel
{
	public class MoveBuildingCommand : ModelCommand
	{
		public GridPosition GridPosition { get; set; }

		public MoveBuildingCommand()
		{
		}

		public MoveBuildingCommand(BuildingModel building)
			: base(building)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = manager.GetModel<BuildingModel>(base.ModelId).MoveBuilding(GridPosition);
			return new NGModelCommandRespond(this, result);
		}
	}
}
