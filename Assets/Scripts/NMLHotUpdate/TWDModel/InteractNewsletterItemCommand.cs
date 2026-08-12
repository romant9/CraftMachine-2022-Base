using BaseModel;

namespace TWDModel
{
	public class InteractNewsletterItemCommand : ModelCommand
	{
		public string ItemId { get; set; }

		public string DeepLinkType { get; set; }

		public int ButtonPressedIndex { get; set; }

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			PlayerModel player = tWDModelManager.Player;
			if (player.NewsLetterItemsInteracted.Contains(ItemId))
			{
				return new NGModelCommandRespond(this, TWDModelResult.Error);
			}
			player.NewsLetterItemsInteracted.Add(ItemId);
			if (DeepLinkType == "QUIZ")
			{
				if (ItemId != tWDModelManager.GameEconomyData.ConfigData.QuizItemId)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Error);
				}
				if (ButtonPressedIndex != tWDModelManager.GameEconomyData.ConfigData.QuizAnswerIndex)
				{
					return new NGModelCommandRespond(this, TWDModelResult.Wrong);
				}
				LootEntry lootEntry = player.LootManager.ShuffleOneLootWithoutTag(new LootEntryGenParams
				{
					eventType = DropEventDefinition.DropEventType.Quiz,
					targetLevel = player.Level,
					dropType = DropType.Gold
				});
				lootEntry.Type = LootEntryType.Quiz;
				player.AddLootBoxToOpen(lootEntry);
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
