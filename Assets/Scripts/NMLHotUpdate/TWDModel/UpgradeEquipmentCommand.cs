using BaseModel;

namespace TWDModel
{
	public class UpgradeEquipmentCommand : ConsumeCurrencyCommand
	{
		public bool Instant { get; set; }

		public UpgradeEquipmentCommand()
		{
		}

		public UpgradeEquipmentCommand(EquipmentItemModel equipmentItemModel)
			: base(equipmentItemModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			EquipmentItemModel equipmentItemModel = (EquipmentItemModel)manager.GetModel(base.ModelId);
			TWDModelResult tWDModelResult;
			if (Instant && base.Cashier.useTokensForPayment)
			{
				tWDModelResult = equipmentItemModel.UpgradeInstant(base.Cashier);
			}
			else if (Instant)
			{
				tWDModelResult = equipmentItemModel.UpgradeInstant();
			}
			else if ((equipmentItemModel.CanUpgrade || equipmentItemModel.CanUpgradeWithEquipmentUpgradeToken) && equipmentItemModel.TimedActionModel != null)
			{
				Cashier upgradeCashier = equipmentItemModel.GetUpgradeCashier(instantUpgrade: false, addInitialSurvivorPoints: false, equipmentItemModel.CanUpgradeWithEquipmentUpgradeToken);
				upgradeCashier.UseDiamondsAmount = base.UseDiamondsAmount;
				tWDModelResult = equipmentItemModel.TimedActionModel.StartActionInstant(upgradeCashier, equipmentItemModel);
			}
			else
			{
				tWDModelResult = TWDModelResult.Error;
			}
			if (tWDModelResult == TWDModelResult.OK)
			{
				equipmentItemModel.TriggerUpgradedDailyQuestAction();
			}
			return new NGModelCommandRespond(this, tWDModelResult);
		}
	}
}
