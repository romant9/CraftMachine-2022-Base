using BaseModel;

namespace TWDModel
{
	public class SpeedUpUpgradeEquipmentCommand : ConsumeCurrencyCommand
	{
		public SpeedUpUpgradeEquipmentCommand()
		{
		}

		public SpeedUpUpgradeEquipmentCommand(EquipmentItemModel equipment)
			: base(equipment)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			EquipmentItemModel equipmentItemModel = (EquipmentItemModel)manager.GetModel(base.ModelId);
			TWDModelResult result = equipmentItemModel.TimedActionModel.SpeedUpEquipmentUpgradeAction(equipmentItemModel, base.Cashier);
			return new NGModelCommandRespond(this, result);
		}
	}
}
