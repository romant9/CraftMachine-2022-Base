using TWDModel;

public class RewardLootEntry : IReward
{
	public DropType DropType { get; set; }

	public RewardType Type => RewardType.Loot;

	public object Give(TWDModelManager manager, object[] param = null)
	{
		LootEntry lootEntry = manager.Player.LootManager.ShuffleOneLootWithoutTag(new LootEntryGenParams
		{
			eventType = DropEventDefinition.DropEventType.MissionScavenge,
			targetLevel = manager.Player.Level,
			dropType = DropType
		});
		lootEntry.Type = LootEntryType.DailyQuest;
		manager.Player.AddLootBoxToOpen(lootEntry);
		return lootEntry;
	}
}
