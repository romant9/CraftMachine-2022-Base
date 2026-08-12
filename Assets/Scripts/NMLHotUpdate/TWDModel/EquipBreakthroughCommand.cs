using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class EquipBreakthroughCommand : ModelCommand
	{
		public List<string> ConsumeEquipTokenIdList { get; set; }

		public EquipBreakthroughCommand()
		{
		}

		public EquipBreakthroughCommand(EquipmentItemModel equipmentItemModel)
			: base(equipmentItemModel)
		{
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			EquipmentItemModel model = manager.GetModel<EquipmentItemModel>(base.ModelId);
			TWDModelResult result = model.BreakthroughLevelUp(ConsumeEquipTokenIdList, model.GetBreakThroughWeaponFragmentsNumber());
			return new NGModelCommandRespond(this, result);
		}
	}
}
