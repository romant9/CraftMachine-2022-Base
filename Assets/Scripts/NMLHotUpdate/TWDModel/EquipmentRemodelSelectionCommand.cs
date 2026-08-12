using BaseModel;

namespace TWDModel
{
	public class EquipmentRemodelSelectionCommand : ModelCommand
	{
		public string TraitId { get; set; }

		public int SelectIndex { get; set; }

		public EquipmentRemodelSelectionCommand()
		{
		}

		public EquipmentRemodelSelectionCommand(EquipmentItemModel equipmentItemModel, string traitId, int selectIndex)
			: base(equipmentItemModel)
		{
			TraitId = traitId;
			SelectIndex = selectIndex;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			EquipmentItemModel model = manager.GetModel<EquipmentItemModel>(base.ModelId);
			TWDModelResult result = TWDModelResult.Error;
			if (model.IsValid())
			{
				result = model.SelectRemodeId(TraitId.ToString(), SelectIndex);
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
