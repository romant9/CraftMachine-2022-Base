using System.Collections.Generic;

namespace TWDModel
{
	public class WorldBossAttackTargetData
	{
		public WorldBossMissionModel MissionModel { get; private set; }

		public int SeasonId { get; private set; }

		public int CycleId { get; private set; }

		public string BossBattleId { get; private set; }

		public string CapturePoint { get; private set; }

		public string Cell { get; private set; }

		public bool IsPVECapturePoint { get; private set; }

		public List<string> ParticipantSurvivorIds { get; private set; }

		public int TimeLimitMs { get; private set; }

		public int KilledDefenderCount { get; set; }

		public List<SurvivorMockData> DefenderTeam { get; set; }

		public bool HasResult { get; private set; }

		public bool IsWin { get; private set; }

		public bool IsTimeout { get; private set; }

		public bool HasSettled { get; set; }

		public long BossScore { get; private set; } = -1L;

		public long BossDamage { get; private set; } = -1L;

		public bool IsBossBattle => BossScore >= 0;

		public bool IsActive => MissionModel != null;

		public void SetBossScore(long score)
		{
			BossScore = ((score > 0) ? score : 0);
		}

		public void SetBossDamage(long damage)
		{
			BossDamage = ((damage > 0) ? damage : 0);
		}

		public void AttackCell(WorldBossMissionModel missionModel, int seasonId, int cycleId, string capturePoint, string cell, bool isPVECapturePoint, List<string> participantSurvivorIds, int timeLimitMs, string bossBattleId = null)
		{
			MissionModel = missionModel;
			SeasonId = seasonId;
			CycleId = cycleId;
			BossBattleId = bossBattleId;
			CapturePoint = capturePoint;
			Cell = cell;
			IsPVECapturePoint = isPVECapturePoint;
			ParticipantSurvivorIds = ((participantSurvivorIds != null) ? new List<string>(participantSurvivorIds) : new List<string>());
			TimeLimitMs = timeLimitMs;
			KilledDefenderCount = 0;
			DefenderTeam = null;
			HasResult = false;
			IsWin = false;
			IsTimeout = false;
			HasSettled = false;
			BossScore = -1L;
			BossDamage = -1L;
		}

		public void AddKilledDefenders(int count)
		{
			if (count > 0)
			{
				KilledDefenderCount += count;
			}
		}

		public void SetResult(bool isWin, bool isTimeout)
		{
			HasResult = true;
			IsWin = isWin;
			IsTimeout = isTimeout;
		}

		public void Clear()
		{
			MissionModel = null;
			SeasonId = 0;
			CycleId = 0;
			BossBattleId = null;
			CapturePoint = null;
			Cell = null;
			IsPVECapturePoint = false;
			ParticipantSurvivorIds = null;
			TimeLimitMs = 0;
			KilledDefenderCount = 0;
			DefenderTeam = null;
			HasResult = false;
			IsWin = false;
			IsTimeout = false;
			HasSettled = false;
			BossScore = -1L;
			BossDamage = -1L;
		}
	}
}
