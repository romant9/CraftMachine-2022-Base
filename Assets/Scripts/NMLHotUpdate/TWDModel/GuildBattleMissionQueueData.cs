using System.Collections.Generic;

namespace TWDModel
{
	public class GuildBattleMissionQueueData
	{
		private int max;

		private GuildBattleMapMissionModel enemyMission;

		public bool Last;

		private List<GuildBattleMapMissionModel> List;

		public int Count => List.Count;

		public bool IsFull => Count >= max;

		public bool IsComplete
		{
			get
			{
				if (EnemyMission == null)
				{
					return false;
				}
				return EnemyMission.IsPvpComplete();
			}
		}

		public bool PvPEnemyUnlocked
		{
			get
			{
				if (EnemyMission == null)
				{
					return false;
				}
				if (HelpersModel.IsUnlockPVP) return true;
				return EnemyMission.AllPvEMissionsInAreaCompleted();
			}
		}

		public GuildBattleMapMissionModel EnemyMission => enemyMission;

		public GuildBattleMapMissionModel this[int index] => List[index];

		public GuildBattleMissionQueueData(int max)
		{
			Clear();
			if (List == null)
			{
				List = new List<GuildBattleMapMissionModel>();
			}
			this.max = max;
		}

		public bool IsEnemySeen(GuildBattleProgressSnapshot progressSnapshot)
		{
			if (PvPEnemyUnlocked)
			{
				return progressSnapshot.IsMissionEnemySeen(EnemyMission);
			}
			return false;
		}

		public bool IsCompleteAndSeen(GuildBattleProgressSnapshot progressSnapshot)
		{
			if (IsComplete)
			{
				return progressSnapshot.IsMissionCompletionSeen(EnemyMission);
			}
			return false;
		}

		public void SetMax(int max)
		{
			this.max = max;
		}

		public bool Add(GuildBattleMapMissionModel item)
		{
			if (item == null)
			{
				return false;
			}
			if (List.Count + 1 > max)
			{
				return false;
			}
			List.Add(item);
			if (item.Type == GuildBattleMapMissionModel.MissionType.PVP)
			{
				enemyMission = item;
			}
			return true;
		}

		public void Clear()
		{
			if (List != null)
			{
				List.Clear();
			}
			max = 0;
			enemyMission = null;
			Last = false;
		}

		public override string ToString()
		{
			return $"[GuildBattleMissionQueueData: Count={Count}, IsFull={IsFull}, IsComplete={IsComplete}, IsEnemyFound={PvPEnemyUnlocked}, EnemyMission={EnemyMission}]";
		}
	}
}
