using BaseModel;

namespace TWDModel
{
	public class CollectBuildingCommand : ModelCommand
	{
		public CollectBuildingCommand()
		{
		}

		public CollectBuildingCommand(BuildingModel building)
			: base(building)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			BuildingModel model = manager.GetModel<BuildingModel>(base.ModelId);
			TWDModelResult result = TWDModelResult.Error;
			if (model.CanCollect)
			{
				result = model.Collect();
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
