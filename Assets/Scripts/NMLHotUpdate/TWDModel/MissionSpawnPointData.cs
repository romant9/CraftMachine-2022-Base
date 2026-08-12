using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class MissionSpawnPointData
	{
		public List<MissionSpawnPointGroup> MissionSpawnPointGroups;

		public Dictionary<int, MissionSpawnPointGroup> MissionSpawnPointGroupsById;

		public MissionSpawnPointData()
		{
			MissionSpawnPointGroups = new List<MissionSpawnPointGroup>();
			MissionSpawnPointGroupsById = new Dictionary<int, MissionSpawnPointGroup>();
		}

		public MissionSpawnPointGroup GetSpawnPointGroup(int spawnPointGroupId)
		{
			if (MissionSpawnPointGroupsById.TryGetValue(spawnPointGroupId, out var value))
			{
				return value;
			}
			return null;
		}

		public MissionSpawnPointGroup GetSpawnPointGroupByMapId(string mapId)
		{
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in MissionSpawnPointGroups)
			{
				if (missionSpawnPointGroup.MapId == mapId)
				{
					return missionSpawnPointGroup;
				}
			}
			return null;
		}

		public MissionSpawnPointGroup GetSpawnPointGroupForDifficultyLevel1(MissionSpawnPointGroup missionSpawnPointGroup)
		{
			foreach (MissionSpawnPointGroup missionSpawnPointGroup2 in MissionSpawnPointGroups)
			{
				if (missionSpawnPointGroup2.DisplayName == missionSpawnPointGroup.DisplayName)
				{
					return missionSpawnPointGroup2;
				}
			}
			return null;
		}

		public MissionSpawnPoint GetSpawnPoint(string mapId, int index)
		{
			MissionSpawnPointGroup spawnPointGroupByMapId = GetSpawnPointGroupByMapId(mapId);
			if (index < 0 || index >= spawnPointGroupByMapId.MissionSpawnPoints.Count)
			{
				return null;
			}
			return spawnPointGroupByMapId.MissionSpawnPoints[index];
		}

		public MissionSpawnPoint FindFirstSpawnPointByMissionId(string missionId)
		{
			for (int i = 0; i < MissionSpawnPointGroups.Count; i++)
			{
				MissionSpawnPointGroup missionSpawnPointGroup = MissionSpawnPointGroups[i];
				for (int j = 0; j < missionSpawnPointGroup.MissionSpawnPoints.Count; j++)
				{
					MissionSpawnPoint missionSpawnPoint = missionSpawnPointGroup.MissionSpawnPoints[j];
					if (missionSpawnPoint.MissionId == missionId)
					{
						return missionSpawnPoint;
					}
				}
			}
			return null;
		}

		public static void CreateHarderDetailMaps(List<MissionSpawnPointGroup> groups, int numberDifficultyLevels, int missionLevelToAdd, int harderEpisodeGrindLevelIncrease)
		{
			if (groups == null)
			{
				return;
			}
			MissionSpawnPointGroup[] array = groups.ToArray();
			foreach (MissionSpawnPointGroup missionSpawnPointGroup in array)
			{
				if (!missionSpawnPointGroup.DisplayName.StartsWith("Episode") || !(missionSpawnPointGroup.DisplayName != "Episode 1"))
				{
					continue;
				}
				for (int j = 0; j < numberDifficultyLevels; j++)
				{
					MissionSpawnPointGroup missionSpawnPointGroup2 = new MissionSpawnPointGroup();
					missionSpawnPointGroup2.Id = missionSpawnPointGroup.Id + 1 + j;
					missionSpawnPointGroup2.MapId = missionSpawnPointGroup.MapId;
					missionSpawnPointGroup2.BackgroundId = missionSpawnPointGroup.BackgroundId;
					missionSpawnPointGroup2.EpisodeDifficultyLevel = 2 + j;
					missionSpawnPointGroup2.DisplayName = missionSpawnPointGroup.DisplayName;
					missionSpawnPointGroup2.CostIndex = missionSpawnPointGroup.CostIndex;
					missionSpawnPointGroup2.Category = missionSpawnPointGroup.Category;
					missionSpawnPointGroup2.MissionSpawnPoints = new List<MissionSpawnPoint>();
					missionSpawnPointGroup2.MissionSpawnPointsById = new Dictionary<string, MissionSpawnPoint>();
					foreach (MissionSpawnPoint missionSpawnPoint2 in missionSpawnPointGroup.MissionSpawnPoints)
					{
						MissionSpawnPoint missionSpawnPoint = new MissionSpawnPoint();
						missionSpawnPoint.IsDeadly = missionSpawnPoint2.IsDeadly;
						missionSpawnPoint.MissionId = missionSpawnPoint2.MissionId;
						missionSpawnPoint.MissionLevel = missionSpawnPoint2.MissionLevel + (j + 1) * missionLevelToAdd;
						missionSpawnPoint.LootTag = missionSpawnPoint2.LootTag;
						missionSpawnPoint.OwningGroup = missionSpawnPointGroup2;
						missionSpawnPoint.MapId = missionSpawnPointGroup2.MapId;
						missionSpawnPointGroup2.MissionSpawnPoints.Add(missionSpawnPoint);
						if (!missionSpawnPointGroup2.MissionSpawnPointsById.ContainsKey(missionSpawnPoint.MissionId))
						{
							missionSpawnPointGroup2.MissionSpawnPointsById.Add(missionSpawnPoint.MissionId, missionSpawnPoint);
						}
					}
					groups.Add(missionSpawnPointGroup2);
				}
			}
		}
	}
}
