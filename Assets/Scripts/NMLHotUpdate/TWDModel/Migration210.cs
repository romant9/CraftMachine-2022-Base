namespace TWDModel
{
	public class Migration210 : TWDModelMigration
	{
		public Migration210()
		{
			base.Version = "2.1.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			DailyMultiKill dailyMultiKill = null;
			for (int i = 0; i < ((player.DailyQuests != null) ? player.DailyQuests.Count : 0); i++)
			{
				DailyQuest dailyQuest = player.DailyQuests[i];
				if (dailyQuest != null && dailyQuest.GetType() == typeof(DailyMultiKill))
				{
					dailyMultiKill = (DailyMultiKill)dailyQuest;
					break;
				}
			}
			if (dailyMultiKill != null)
			{
				dailyMultiKill.InitialValues = new int[10];
				for (int j = 0; j < dailyMultiKill.InitialValues.Length; j++)
				{
					dailyMultiKill.InitialValues[j] = player.MissionStatistics.GetMultiKillCount(j);
				}
				dailyMultiKill.InitialValue = 0;
			}
			return true;
		}
	}
}
