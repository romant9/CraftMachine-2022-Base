using System;
using System.Collections.Generic;

namespace TWDModel
{
	[Serializable]
	public class GuildBattleProgressSnapshot
	{
		public Dictionary<string, int> SectorCompletedDictionary { get; set; }

		public Dictionary<string, int> MissionCompletionDictionary { get; set; }

		public Dictionary<string, int> SectorBonusAnimationSeen { get; set; }

		public Dictionary<string, int> SectorStatesSeenDictionary { get; set; }

		public GuildBattleProgressSnapshot()
		{
			SectorCompletedDictionary = new Dictionary<string, int>();
			MissionCompletionDictionary = new Dictionary<string, int>();
			SectorBonusAnimationSeen = new Dictionary<string, int>();
			SectorStatesSeenDictionary = new Dictionary<string, int>();
		}

		public void CopyProgressFromSector(GuildBattleMapSectorModel sectorModel)
		{
			if (sectorModel == null || MissionCompletionDictionary == null)
			{
				return;
			}
			for (int i = 0; i < sectorModel.RandomizedMissions.Count; i++)
			{
				int completionAmount = sectorModel.RandomizedMissions[i].CompletionAmount;
				string id = sectorModel.RandomizedMissions[i].Id;
				if (completionAmount > 0)
				{
					MissionCompletionDictionary.Remove(id);
					MissionCompletionDictionary.Add(id, completionAmount);
				}
				else
				{
					MissionCompletionDictionary.Remove(id);
				}
			}
		}

		public bool IsSectorSeen(GuildBattleMapSectorModel sectorModel)
		{
			for (int i = 0; i < sectorModel.RandomizedMissions.Count; i++)
			{
				int completionAmount = sectorModel.RandomizedMissions[i].CompletionAmount;
				_ = sectorModel.RandomizedMissions[i].Id;
				int value = 0;
				MissionCompletionDictionary.TryGetValue(sectorModel.RandomizedMissions[i].Id, out value);
				if (completionAmount > 0 && value != completionAmount)
				{
					return false;
				}
			}
			return true;
		}

		public int GetSeenPvPCompletedMissions(GuildBattleMapSectorModel sectorModel)
		{
			int num = 0;
			for (int i = 0; i < sectorModel.RandomizedMissions.Count; i++)
			{
				if (GuildBattleMapMissionModel.IsMissionCompleted(GetCompletionCount(sectorModel.RandomizedMissions[i])))
				{
					num++;
				}
			}
			return num;
		}

		public int GetCompletionCount(GuildBattleMapMissionModel model)
		{
			return GetCompletionCount(model.Id);
		}

		public int GetCompletionCount(string missionId)
		{
			int value = 0;
			MissionCompletionDictionary.TryGetValue(missionId, out value);
			return value;
		}

		public bool IsMissionCompletionSeen(GuildBattleMapMissionModel model)
		{
			if (model == null)
			{
				return false;
			}
			return GuildBattleMapMissionModel.IsMissionCompleted(GetCompletionCount(model));
		}

		public bool IsPveCompletionSeen(GuildBattleMapMissionModel model)
		{
			if (model == null)
			{
				return false;
			}
			return GuildBattleMapMissionModel.IsMissionCompleted(GetCompletionCount(model));
		}

		public bool IsMissionEnemySeen(GuildBattleMapMissionModel model)
		{
			return model?.IsEnemyUnlocked() ?? false;
		}

		public void UpdateAnimateSectorAnimationSeen(string bonusName, int stackedBuffsNum)
		{
			if (SectorBonusAnimationSeen.ContainsKey(bonusName))
			{
				SectorBonusAnimationSeen[bonusName] = stackedBuffsNum;
			}
			else
			{
				SectorBonusAnimationSeen.Add(bonusName, stackedBuffsNum);
			}
		}

		public void SectorStateSeenUpdate(string sectorId, int state)
		{
			if (SectorStatesSeenDictionary.ContainsKey(sectorId))
			{
				SectorStatesSeenDictionary[sectorId] = state;
			}
			else
			{
				SectorStatesSeenDictionary.Add(sectorId, state);
			}
		}

		public int GetSectorStateSeenValue(string sectorId)
		{
			int value = 0;
			SectorStatesSeenDictionary.TryGetValue(sectorId, out value);
			return value;
		}

		public void Clear()
		{
			if (SectorCompletedDictionary != null)
			{
				SectorCompletedDictionary.Clear();
			}
			if (MissionCompletionDictionary != null)
			{
				MissionCompletionDictionary.Clear();
			}
			if (SectorBonusAnimationSeen != null)
			{
				SectorBonusAnimationSeen.Clear();
			}
			if (SectorStatesSeenDictionary != null)
			{
				SectorStatesSeenDictionary.Clear();
			}
		}
	}
}
