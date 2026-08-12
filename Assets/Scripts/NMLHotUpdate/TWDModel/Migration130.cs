using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class Migration130 : TWDModelMigration
	{
		public Migration130()
		{
			base.Version = "1.3.0";
		}

		public override bool Migrate(PlayerModel player, TWDModelManager manager)
		{
			if (player.Combat != null)
			{
				player.DeleteCombatModel(notify: false);
			}
			TutorialModel tutorial = player.Tutorial;
			TutorialPartDefinition currentPartDefinition = tutorial.CurrentPartDefinition;
			if (currentPartDefinition != null)
			{
				if (currentPartDefinition.Id == "Tutorial_Training_Ground" && tutorial.CurrentStep >= 9)
				{
					tutorial.CurrentStep = 9;
				}
				if (currentPartDefinition.Id == "EndTutorial")
				{
					tutorial.SetPartCompleted("EndTutorial");
				}
			}
			ModelList<MapMissionGroupModel> mapMissionGroups = player.MapContainerModel.MapMissionGroups;
			for (int i = 0; i < mapMissionGroups.Count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = mapMissionGroups[i];
				if (mapMissionGroupModel.IsLocked)
				{
					continue;
				}
				List<MapMissionModel> list = new List<MapMissionModel>();
				for (int j = 0; j < mapMissionGroupModel.Missions.Count; j++)
				{
					MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[j];
					if (mapMissionModel != null && mapMissionModel.IsGrindMission)
					{
						list.Add(mapMissionModel);
					}
				}
				for (int k = 0; k < list.Count; k++)
				{
					if (mapMissionGroupModel.Missions.Contains(list[k]))
					{
						mapMissionGroupModel.Missions.Remove(list[k]);
					}
				}
				list.Clear();
			}
			if (player.DailyQuests == null)
			{
				player.DailyQuests = new List<DailyQuest>();
			}
			player.Camp.CampDefenseModel.Reset();
			return true;
		}
	}
}
