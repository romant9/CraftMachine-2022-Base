using BaseModel;

namespace TWDModel
{
	public class UpdateSeasonTrialsCommand : ModelCommand
	{
		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager { Player: not null } tWDModelManager && tWDModelManager.Player.MapContainerModel != null)
			{
				for (int i = 0; i < tWDModelManager.Player.MapContainerModel.MapMissionGroups.Count; i++)
				{
					MapMissionGroupModel mapMissionGroupModel = tWDModelManager.Player.MapContainerModel.MapMissionGroups[i];
					if (mapMissionGroupModel == null || mapMissionGroupModel.MissionSpawnPointGroup == null || mapMissionGroupModel.MissionSpawnPointGroup.Category != MapCategory.Season)
					{
						continue;
					}
					MissionHighlight missionHighlight = mapMissionGroupModel.NewerFeaturedDataExist();
					if (missionHighlight == null)
					{
						continue;
					}
					for (int j = 0; j < mapMissionGroupModel.Missions.Count; j++)
					{
						MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[j];
						if (mapMissionModel != null && mapMissionModel.MissionSpawnPointGroup != null && mapMissionModel.ResetIfTrialMission())
						{
							tWDModelManager.Debug.Log("Trial Reset for MapId: " + mapMissionModel.MissionSpawnPointGroup.MapId + ". Old version: " + mapMissionGroupModel.MissionHighlightVersion + " New version: " + missionHighlight.Version);
						}
					}
					mapMissionGroupModel.MissionHighlightVersion = missionHighlight.Version;
				}
				return new NGModelCommandRespond(this, TWDModelResult.OK);
			}
			return new NGModelCommandRespond(this, TWDModelResult.Error);
		}
	}
}
