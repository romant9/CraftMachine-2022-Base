using BaseModel;

namespace TWDModel.SqEquipmentRemold
{
	public class SqEquipmentRemoldTratsCancelCommand : ConsumeCurrencyCommand
	{
		public new int ModelId { get; set; }

		public string RemoldTraits { get; set; }

		public SqEquipmentRemoldTratsCancelCommand()
		{
		}

		public SqEquipmentRemoldTratsCancelCommand(int modelId)
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
			model.SpEquipmentRemoldModel.CancelRemold();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
