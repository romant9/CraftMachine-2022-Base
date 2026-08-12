using BaseModel;

namespace TWDModel
{
	public class EquipItemCommand : ModelCommand
	{
		public int EquipmentItemId { get; protected set; }

		public EquipItemCommand()
		{
		}

		public EquipItemCommand(SurvivorModel survivor, EquipmentItemModel equipmentItem)
			: base(survivor)
		{
			EquipmentItemId = equipmentItem.ModelId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			EquipmentItemModel model2 = manager.GetModel<EquipmentItemModel>(EquipmentItemId);
			TWDModelResult result = model.Equip(model2);
			model.ConfigureBaseAttributes();
			return new NGModelCommandRespond(this, result);
		}
	}
}
