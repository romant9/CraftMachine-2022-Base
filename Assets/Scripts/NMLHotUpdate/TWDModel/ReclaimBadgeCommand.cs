using BaseModel;

namespace TWDModel
{
	public class ReclaimBadgeCommand : ConsumeCurrencyCommand
	{
		public int slotIndex;

		public ReclaimBadgeCommand()
		{
		}

		public ReclaimBadgeCommand(SurvivorModel survivorModel, int slotIndex)
			: base(survivorModel)
		{
			this.slotIndex = slotIndex;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			SurvivorModel model = manager.GetModel<SurvivorModel>(base.ModelId);
			BadgeModel badgeWithSlotIndex = model.GetBadgeWithSlotIndex(slotIndex);
			TWDModelResult result = TWDModelResult.Error;
			if (badgeWithSlotIndex != null && model != null)
			{
				result = model.ReclaimBadge(badgeWithSlotIndex, pay: true, returnBadgeInventory: true);
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
