using BaseModel;

namespace TWDModel
{
	public abstract class PhoneCallBaseCommand : ModelCommand
	{
		public PhoneCallBaseCommand()
		{
		}

		public PhoneCallBaseCommand(ModelObject model)
			: base(model)
		{
		}

		protected bool CheckMatchAgainstPhoneCall(TWDModelManager twdModelManager, SurvivorModel survivorToCheck)
		{
			bool result = false;
			ModelList<LootEntry> lootsList = twdModelManager.Player.PhoneCall.LootsList;
			for (int i = 0; i < (lootsList?.Count ?? 0); i++)
			{
				LootEntry lootEntry = lootsList[i];
				if (lootEntry != null && lootEntry.GeneratedSurvivor != null && lootEntry.GeneratedSurvivor == survivorToCheck)
				{
					result = true;
					break;
				}
			}
			return result;
		}
	}
}
