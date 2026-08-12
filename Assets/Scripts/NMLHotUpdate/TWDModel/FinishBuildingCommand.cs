using BaseModel;

namespace TWDModel
{
	public class FinishBuildingCommand : ConsumeCurrencyCommand
	{
		public FinishBuildingCommand()
		{
		}

		public FinishBuildingCommand(BuildingModel building)
			: base(building)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = manager.GetModel<BuildingModel>(base.ModelId).SpeedUpUpgrade(base.Cashier);
			return new NGModelCommandRespond(this, result);
		}
	}
}
