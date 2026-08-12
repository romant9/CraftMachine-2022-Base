namespace TWDModel
{
	public static class ReturnQuestTypeExtensions
	{
		public static bool IsUpgradeQuest(this ReturnQuestType questType)
		{
			if (questType != ReturnQuestType.UpgradeSurvivor && questType != ReturnQuestType.UpgradeBuilding)
			{
				return questType == ReturnQuestType.UpgradeEquipment;
			}
			return true;
		}
	}
}
