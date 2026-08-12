using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class Migration190 : TWDModelMigration
	{
		public Migration190()
		{
			base.Version = "1.9.0";
		}

		private static void FixDeadSurvivors(PlayerModel player)
		{
			if (player == null || player.SurvivorContainer == null)
			{
				return;
			}
			foreach (DeadSurvivorModel deadSurvivor in player.SurvivorContainer.DeadSurvivors)
			{
				deadSurvivor.SurvivorModel.ClearModelObjectReferences();
			}
		}

		private static void FixSurvivorFailMissionCondition(PlayerModel player)
		{
			if (player == null || player.SurvivorContainer == null)
			{
				return;
			}
			foreach (SurvivorModel survivor in player.SurvivorContainer.Survivors)
			{
				if (survivor.MissionFailCondition != MissionFailCondition.None)
				{
					survivor.MissionFailCondition = MissionFailCondition.None;
				}
			}
		}

		private static void FixDefenceLog(PlayerModel player)
		{
			if (player != null && player.DefenseOutpostVisitLog == null)
			{
				player.DefenseOutpostVisitLog = new List<OutpostVisitEntry>();
			}
			if (player != null && player.AttackOutpostVisitLog == null)
			{
				player.AttackOutpostVisitLog = new List<OutpostVisitEntry>();
			}
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			if (player != null && player.PhoneCall != null)
			{
				if (player.PhoneCall.LootsList == null)
				{
					player.PhoneCall.LootsList = new ModelList<LootEntry>();
				}
				if (player.PhoneCall.Loot != null)
				{
					player.PhoneCall.AddLoot(player.PhoneCall.Loot);
					player.PhoneCall.Loot = null;
				}
			}
			FixDefenceLog(player);
			FixSurvivorFailMissionCondition(player);
			FixDeadSurvivors(player);
			if (player != null && player.Combat == null && player.MapContainerModel != null && player.MapContainerModel.MapMissionGroups != null)
			{
				ModelList<MapMissionGroupModel> mapMissionGroups = player.MapContainerModel.MapMissionGroups;
				for (int i = 0; i < mapMissionGroups.Count; i++)
				{
					if (mapMissionGroups[i] == null || mapMissionGroups[i].Missions == null)
					{
						continue;
					}
					for (int j = 0; j < mapMissionGroups[i].Missions.Count; j++)
					{
						MapMissionModel mapMissionModel = mapMissionGroups[i].Missions[j];
						if (mapMissionModel != null && mapMissionModel.IsDeadly)
						{
							mapMissionModel.IsDeadly = false;
						}
					}
				}
			}
			return true;
		}
	}
}
