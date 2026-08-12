using BaseModel;

namespace TWDModel
{
	public class AssembleTalentTraitToSupportCommand : ModelCommand
	{
		public int TalentId { get; private set; }

		public int SlotIndex { get; private set; }

		public AssembleTalentTraitToSupportCommand()
		{
		}

		public AssembleTalentTraitToSupportCommand(int modelId, int talentId, int slotIndex)
			: base(modelId)
		{
			TalentId = talentId;
			SlotIndex = slotIndex;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (!(manager is TWDModelManager tWDModelManager))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			SupportModel model = manager.GetModel<SupportModel>(base.ModelId);
			if (model.SupportTalentSlot < SlotIndex)
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			if (model.SlotAssembledTalentIds.ContainsValue(TalentId))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			model.SlotAssembledTalentIds[SlotIndex] = TalentId;
			tWDModelManager.Metrics.AddFind().AddSupportAssembleTrait(SlotIndex, TalentId).Send();
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
