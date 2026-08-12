using BaseModel;

namespace TWDModel
{
	public class CreateBuildingCommand : ConsumeCurrencyCommand
	{
		public GridPosition GridPosition { get; set; }

		public string BuildingType { get; set; }

		public CreateBuildingCommand()
		{
		}

		public CreateBuildingCommand(CampModel camp)
			: base(camp)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			CampModel model = manager.GetModel<CampModel>(base.ModelId);
			BuildingModel outNewBuilding = null;
			TWDModelResult result = model.CreateNewBuilding(BuildingType, GridPosition, ref outNewBuilding, base.UseDiamondsAmount);
			return new NGModelCommandRespond(this, result);
		}
	}
}
