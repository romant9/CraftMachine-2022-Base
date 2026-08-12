using BaseModel;

namespace TWDModel
{
	public class CancelUpgradeCommand : ConsumeCurrencyCommand
	{
		public CancelUpgradeCommand()
		{
		}

		public CancelUpgradeCommand(BuildingModel building)
			: base(building)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = manager.GetModel<BuildingModel>(base.ModelId).CancelUpgrade();
			return new NGModelCommandRespond(this, result);
		}
	}
}
