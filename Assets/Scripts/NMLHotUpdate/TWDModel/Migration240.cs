using System;

namespace TWDModel
{
	public class Migration240 : TWDModelMigration
	{
		public Migration240()
		{
			base.Version = "2.4.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			MigrationUtils.AddNewCurrency(player, manager, CurrencyType.EugeneToken, CurrencyType.AaronToken, CurrencyType.GabrielToken, CurrencyType.EzekielToken, CurrencyType.DwightToken, CurrencyType.SashaToken);
			if (player.HighestWeeklyChallengeScore == 0 && player.WeeklyChallenge != null)
			{
				player.HighestWeeklyChallengeScore = Math.Max(player.WeeklyChallenge.NumberStars, player.WeeklyChallenge.PreviousNumberStars);
			}
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in manager.GameEconomyData.MissionSpawnPointData.MissionSpawnPointGroups)
			{
				player.MapContainerModel.SpawnMissionGroup(missionSpawnPointGroup);
			}
			if (player.Blackboard.IsToggleOn("Toggle.ToggleUpdateInfoPopupShown"))
			{
				player.Blackboard.ClearToggle("Toggle.ToggleUpdateInfoPopupShown");
			}
			player.MapContainerModel.SpawnSeasonEpisodes();
			if (!player.Tutorial.HasCompletedPart("EndTutorial") && player.PendingVideoAdReward)
			{
				player.PendingVideoAdReward = false;
			}
			return true;
		}
	}
}
