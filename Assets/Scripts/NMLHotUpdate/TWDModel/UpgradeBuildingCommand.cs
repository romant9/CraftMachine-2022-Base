using BaseModel;

namespace TWDModel
{
	public class UpgradeBuildingCommand : ConsumeCurrencyCommand
	{
		public bool Instant { get; set; }

		public UpgradeBuildingCommand()
		{
		}

		public UpgradeBuildingCommand(BuildingModel building)
			: base(building)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			BuildingModel buildingModel = (BuildingModel)manager.GetModel(base.ModelId);
			TWDModelResult result = ((!Instant) ? buildingModel.StartUpgrade(base.UseDiamondsAmount) : buildingModel.UpgradeInstant(base.UseDiamondsAmount, base.Cashier));
			return new NGModelCommandRespond(this, result);
		}
	}
}
