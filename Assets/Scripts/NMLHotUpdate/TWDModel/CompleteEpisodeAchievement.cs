namespace TWDModel
{
	public class CompleteEpisodeAchievement : Achievement
	{
		public int EpisodeID;

		protected override bool InternalIsCompleted
		{
			get
			{
				if (base.GED.MissionSpawnPointData != null)
				{
					MissionSpawnPointGroup spawnPointGroup = base.GED.MissionSpawnPointData.GetSpawnPointGroup(EpisodeID);
					if (spawnPointGroup != null)
					{
						MapMissionGroupModel mapMissionGroupModel = ((Player.MapContainerModel != null) ? Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(spawnPointGroup) : null);
						if (mapMissionGroupModel != null)
						{
							return mapMissionGroupModel.AreAllStoryMissionsCompleted();
						}
					}
				}
				return false;
			}
		}

		protected override bool Init()
		{
			EpisodeID = 0;
			if (!int.TryParse(base.AchievementDefinition.Params, out EpisodeID))
			{
				return false;
			}
			return true;
		}

		public override int GetProgressStep()
		{
			if (!InternalIsCompleted)
			{
				return 0;
			}
			return 1;
		}

		public override int GetProgressTarget()
		{
			return 1;
		}
	}
}
