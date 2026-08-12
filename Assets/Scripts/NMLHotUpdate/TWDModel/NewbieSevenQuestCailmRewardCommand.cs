using BaseModel;

namespace TWDModel
{
	public class NewbieSevenQuestCailmRewardCommand : ModelCommand
	{
		public int SlotIndex { get; set; }

		public NewbieSevenQuestCailmRewardCommand()
		{
		}

		public NewbieSevenQuestCailmRewardCommand(int slotIndex)
		{
			SlotIndex = slotIndex;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.Error;
			if (manager is TWDModelManager { Player: not null } tWDModelManager && tWDModelManager.Player.NewbieSenvenQuest != null)
			{
				tWDModelManager.Player.NewbieSenvenQuest.TryClaimQuest(SlotIndex);
				result = TWDModelResult.OK;
			}
			return new NGModelCommandRespond(this, result);
		}
	}
}
