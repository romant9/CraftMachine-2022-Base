using BaseModel;

namespace TWDModel
{
	public class EquipModSkillCommand : ModelCommand
	{
		public int SlotIndex { get; set; }

		public int equipmentItemModelId { get; set; }

		public int ModSkillModelId { get; set; }

		public EquipModSkillCommand()
		{
		}

		public EquipModSkillCommand(int slotIndex, int modSkillModelId, int equipmentItemModelId)
		{
			SlotIndex = slotIndex;
			ModSkillModelId = modSkillModelId;
			this.equipmentItemModelId = equipmentItemModelId;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager { Player: not null } tWDModelManager) || tWDModelManager.Player.gameEconomyData == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			EquipmentItemModel model = manager.GetModel<EquipmentItemModel>(equipmentItemModelId);
			if (model == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			ModSkillMode model2 = manager.GetModel<ModSkillMode>(ModSkillModelId);
			if (model2 == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			ModSkillManager modSkillManager = tWDModelManager.Player.ModSkillManager;
			if (modSkillManager == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			TWDModelResult tWDModelResult = modSkillManager.EquipModSkill(SlotIndex, model2, model);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return new NGModelCommandRespond(this, tWDModelResult);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
