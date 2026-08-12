using System.Collections.Generic;

namespace TWDModel
{
	public class Migration170 : TWDModelMigration
	{
		public Migration170()
		{
			base.Version = "1.7.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (player.NewsLetterItemsRead == null)
			{
				player.NewsLetterItemsRead = new List<string>();
			}
			if (player.RankingScore <= 0)
			{
				player.RankingScore = manager.GameEconomyData.ConfigData.InitialRankingScore;
			}
			bool flag = true;
			for (int i = 0; i < player.LootManager.DropCummulativeProbabilities.Count; i++)
			{
				if (player.LootManager.DropCummulativeProbabilities[i].EventType == DropEventDefinition.DropEventType.TradeCrate)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				LootCummulativeProbabilityEntry lootCummulativeProbabilityEntry = new LootCummulativeProbabilityEntry();
				lootCummulativeProbabilityEntry.EventType = DropEventDefinition.DropEventType.TradeCrate;
				player.LootManager.DropCummulativeProbabilities.Add(lootCummulativeProbabilityEntry);
			}
			return true;
		}
	}
}
