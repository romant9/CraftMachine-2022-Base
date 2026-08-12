namespace TWDModel
{
	public class Migration150 : TWDModelMigration
	{
		public Migration150()
		{
			base.Version = "1.5.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (player.SurvivorContainer.StoryTeller != null)
			{
				QuestModel currentQuest = player.SurvivorContainer.StoryTeller.CurrentQuest;
				if (currentQuest != null && string.IsNullOrEmpty(currentQuest.DefinitionID))
				{
					currentQuest.DefinitionID = player.SurvivorContainer.StoryTeller.CurrentQuestDefinition.Identifier;
				}
			}
			if (player.SurvivorContainer.StoryTeller2 != null)
			{
				QuestModel currentQuest2 = player.SurvivorContainer.StoryTeller2.CurrentQuest;
				if (currentQuest2 != null && string.IsNullOrEmpty(currentQuest2.DefinitionID))
				{
					currentQuest2.DefinitionID = player.SurvivorContainer.StoryTeller2.CurrentQuestDefinition.Identifier;
				}
			}
			return true;
		}
	}
}
