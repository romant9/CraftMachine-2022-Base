using Newtonsoft.Json;

namespace TWDModel
{
	public class MissionQuest : QuestModel
	{
		public string MapId;

		public int SpawnPointGroupId { get; private set; }

		[JsonIgnore]
		public override int CompletedSteps
		{
			get
			{
				MissionSpawnPointGroup spawnPointGroupByMapId = base.manager.Player.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroupByMapId(MapId);
				if (spawnPointGroupByMapId == null)
				{
					if (string.IsNullOrEmpty(MapId))
					{
						base.Debug.LogWarning("Empty MapId in mission quest " + base.QuestDefinition.Identifier);
					}
					else
					{
						base.Debug.LogWarning("Can not find spawnpoint in mission quest " + base.QuestDefinition.Identifier);
					}
					return 0;
				}
				MissionSpawnPoint missionSpawnPoint = spawnPointGroupByMapId.MissionSpawnPoints[spawnPointGroupByMapId.MissionSpawnPoints.Count - 1];
				MapMissionModel mapMissionModel = ((missionSpawnPoint != null) ? base.manager.Player.MapContainerModel.GetMissionModelForSpawnPoint(missionSpawnPoint) : null);
				if (mapMissionModel != null && (mapMissionModel.State == MapMissionState.Completed || mapMissionModel.State == MapMissionState.Respawning))
				{
					return 1;
				}
				return 0;
			}
		}

		public MissionQuest()
		{
		}

		public MissionQuest(string paramValues)
		{
			MapId = paramValues;
		}

		public override void Initialize()
		{
			base.Initialize();
			MissionSpawnPointGroup spawnPointGroupByMapId = base.manager.Player.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroupByMapId(MapId);
			if (spawnPointGroupByMapId == null)
			{
				base.manager.Debug.LogError("Could not find map  '" + MapId + "'!");
				return;
			}
			if (spawnPointGroupByMapId.MissionSpawnPoints.Count == 0)
			{
				base.manager.Debug.LogError("Map '" + MapId + "' has no missions!");
				return;
			}
			MissionSpawnPoint missionSpawnPoint = spawnPointGroupByMapId.MissionSpawnPoints[spawnPointGroupByMapId.MissionSpawnPoints.Count - 1];
			if (spawnPointGroupByMapId != null && missionSpawnPoint != null && base.QuestDefinition.IsAvailable)
			{
				base.manager.Player.MapContainerModel.SpawnMissionsForGroup(spawnPointGroupByMapId);
			}
		}

		public MapMissionGroupModel GetUnlockedEpisode()
		{
			if (base.QuestDefinition == null)
			{
				return null;
			}
			return base.QuestDefinition.GetUnlockedEpisode(base.manager);
		}
	}
}
