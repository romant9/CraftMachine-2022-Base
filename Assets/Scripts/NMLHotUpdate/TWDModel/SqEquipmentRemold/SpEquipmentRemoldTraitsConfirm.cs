using BaseModel;

namespace TWDModel.SqEquipmentRemold
{
	public class SpEquipmentRemoldTraitsConfirm : ConsumeCurrencyCommand
	{
		public new int ModelId { get; set; }

		public string RemoldTraits { get; set; }

		public SpEquipmentRemoldTraitsConfirm()
		{
		}

		public SpEquipmentRemoldTraitsConfirm(int modelId)
		{
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
			if (model.SpEquipmentRemoldModel.GetPassiveTraits() != null)
			{
				model.RemoveModSkillPassiveTraits();
			}
			model.SpEquipmentRemoldModel.ConfirmRemold();
			if (model.SpEquipmentRemoldModel.GetPassiveTraits() != null)
			{
				model.ApplyModSkillPassiveTraitsToOwner();
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
