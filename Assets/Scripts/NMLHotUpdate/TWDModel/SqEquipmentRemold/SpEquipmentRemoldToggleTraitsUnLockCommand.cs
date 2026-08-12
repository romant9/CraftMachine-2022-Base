using BaseModel;

namespace TWDModel.SqEquipmentRemold
{
	public class SpEquipmentRemoldToggleTraitsUnLockCommand : ConsumeCurrencyCommand
	{
		public new int ModelId { get; set; }

		public string RemoldTraits { get; set; }

		public SpEquipmentRemoldToggleTraitsUnLockCommand()
		{
		}

		public SpEquipmentRemoldToggleTraitsUnLockCommand(string remoldTraits, int modelId)
		{
			RemoldTraits = remoldTraits;
			ModelId = modelId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager) || tWDModelManager.Player.gameEconomyData == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			EquipmentItemModel model = manager.GetModel<EquipmentItemModel>(ModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (model.SpEquipmentRemoldModel.UnlockTrait(RemoldTraits))
			{
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
