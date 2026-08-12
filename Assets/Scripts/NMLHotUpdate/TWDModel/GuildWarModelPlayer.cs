using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class GuildWarModelPlayer : TWDModelObject
	{
		[JsonIgnore]
		private Dictionary<long, GuildBattleLogPlayerEntry> battleParticipationLogFastLookup;

		[JsonIgnore]
		private DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();

		public GuildBattleModelPlayer GuildBattleModel { get; set; }

		public int StartedWarId { get; set; }

		public List<GuildBattleLogPlayerEntry> BattleParticipationLog { get; set; }

		public List<long> RegisteredBattleSlots { get; set; }

		public long LastOpponentRequestTime { get; set; }

		[JsonIgnore]
		public int GetBattlePassRefreshAmount => base.manager.GameEconomyData.GuildWarConfig.BattlePassRefreshAmount;

		public GuildWarModelPlayer()
		{
			BattleParticipationLog = new List<GuildBattleLogPlayerEntry>();
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			GuildBattleModel = new GuildBattleModelPlayer();
			GuildBattleModel.SetManager(base.manager);
			GuildBattleModel.Initialize();
		}

		public override void Start()
		{
			base.Start();
			if (RegisteredBattleSlots == null)
			{
				RegisteredBattleSlots = new List<long>();
			}
			SetupBattleLogEntryLookup();
		}

		public void StartWar(int warId)
		{
			StartedWarId = warId;
			base.manager.Player.Blackboard.ClearToggle("HasSeenGuildWarStart");
			RefreshBattlePass();
			if (GuildBattleModel.IsOngoingForPlayer())
			{
				EndBattle();
			}
			else
			{
				GuildBattleModel.ResetBattle();
			}
			NotifyChange("GuildWarStarted");
		}

		public void StartBattle(long battleTimeSlot)
		{
			GuildBattleModel.StartBattle(battleTimeSlot);
		}

		public void EndBattle()
		{
			GuildBattleModel.GiveBattleRewards();
			if (battleParticipationLogFastLookup.TryGetValue(GuildBattleModel.StartBattleTimestamp, out var value))
			{
				value.RP = GuildBattleModel.PersonalRewardPoints + GuildBattleModel.GetBattleBonusRewardPointsAmount();
				value.VP = GuildBattleModel.PersonalVictoryPoints;
			}
			GuildBattleModel.EndBattle();
		}

		public bool HasParticipatedInBattle()
		{
			string guildId = base.manager.Player.GuildId;
			foreach (GuildBattleLogPlayerEntry value in battleParticipationLogFastLookup.Values)
			{
				if (value.IsValidated && value.GuildId == guildId)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasBattlePassWaitingToValidateForCurrentBattle()
		{
			GuildBattleModel currentBattle = base.manager.Player.GuildModel.GuildWarModel.CurrentBattle;
			if (currentBattle.RegisteredPlayers.Contains(base.manager.Player.HashedId))
			{
				GuildBattleLogPlayerEntry battleLogEntry = GetBattleLogEntry(currentBattle.TimeSlot);
				if (battleLogEntry != null)
				{
					return !battleLogEntry.IsValidated;
				}
				return false;
			}
			return false;
		}

		public bool HasBattlePassValidatedForCurrentBattle()
		{
			GuildBattleModel currentBattle = base.manager.Player.GuildModel.GuildWarModel.CurrentBattle;
			if (currentBattle.RegisteredPlayers.Contains(base.manager.Player.HashedId))
			{
				GuildBattleLogPlayerEntry battleLogEntry = GetBattleLogEntry(currentBattle.TimeSlot);
				if (battleLogEntry != null)
				{
					return !battleLogEntry.IsValidated;
				}
				return false;
			}
			return false;
		}

		public bool HasWarStarted()
		{
			if (base.manager.Player.IsGuildMember && base.manager.Player.GuildWarModel != null && base.manager.Player.GuildWarModel.IsCurrentWarOpen(base.manager.Player.UtcTimeStamp))
			{
				return base.manager.Player.GuildWarModel.WarDefinitionId == StartedWarId;
			}
			return false;
		}

		public bool HasSeenWarStart()
		{
			return base.manager.Player.Blackboard.IsToggleOn("HasSeenGuildWarStart");
		}

		public void GiveMissionPersonalRewards(GuildBattleMapMissionModel missionModel)
		{
			if (base.manager.Player.GuildWarModel != null)
			{
				GuildBattleModel currentBattle = base.manager.Player.GuildModel.GuildWarModel.CurrentBattle;
				bool isPvPCombat = GuildBattleModel.AttackTargetMission.IsPvPCombat;
				bool num = GuildBattleModel.IsCurrentGuildBattle();
				int personalGuildBattleMissionRewardPoints = currentBattle.GetPersonalGuildBattleMissionRewardPoints(missionModel.SectorIdOwner, isPvPCombat, missionModel.AreaIndex);
				int num2 = ((GuildBattleModel.IsCurrentGuildBattle() && !currentBattle.HasEnded() && !currentBattle.IsBiggerThanEndBattleTimeStamp(base.manager.Player.UtcTimeStamp)) ? currentBattle.GetGuildBattleMissionVictoryPoints(missionModel.SectorIdOwner, isPvPCombat, missionModel.AreaIndex) : 0);
				if (base.manager.Player.Combat != null && base.manager.Player.Combat.RetryMission)
				{
					int num3 = (int)FixedPoint.Round(num2 * (base.manager.Player.gameEconomyData.GuildWarConfig.RetryMissionPenalty + 0.0001));
					num2 -= num3;
				}
				base.manager.Player.GetCurrency(CurrencyType.GuildBattleRP).Add(personalGuildBattleMissionRewardPoints);
				GuildBattleModel.PersonalRewardPoints += personalGuildBattleMissionRewardPoints;
				GuildBattleModel.PersonalVictoryPoints += num2;
				Metrics metrics = base.manager.Metrics;
				metrics.PushResource(CurrencyType.GuildBattleRP, personalGuildBattleMissionRewardPoints);
				if (num)
				{
					metrics.AddFind().AddResources().AddMission()
						.AddGvG()
						.AddGvGBattle()
						.AddGvGPvPInfoIfNeeded()
						.Send();
				}
				else
				{
					metrics.AddFind().AddResources().Send();
				}
			}
		}

		public void AddPersonalMissionProgression()
		{
			GuildBattleModel.AddPersonalMissionProgression();
		}

		public void RefreshBattlePass()
		{
			CurrencyModel currency = base.manager.Player.GetCurrency(CurrencyType.BattlePass);
			int value = currency.Value;
			currency.SetValue(GetBattlePassRefreshAmount);
			base.manager.Metrics.AddFind().AddResources(CurrencyType.BattlePass, currency.Value, currency.Value - value).AddGvG()
				.AddBattlePassRefresh()
				.Send();
			GroupCommandBase command = SetMemberGvGInfoCommand.SetMemberGvGInfo(base.manager);
			HelpersModel.ExecuteGroupCommand(base.manager, command);
		}

		public void UpdateBattleLogEntryOnStart(long startBattle, GuildBattleLogPlayerEntry.Status status)
		{
			GuildBattleLogPlayerEntry guildBattleLogPlayerEntry = GetBattleLogEntry(startBattle);
			if (guildBattleLogPlayerEntry == null)
			{
				guildBattleLogPlayerEntry = new GuildBattleLogPlayerEntry();
				guildBattleLogPlayerEntry.GuildId = base.manager.Player.GuildId;
				BattleParticipationLog.Add(guildBattleLogPlayerEntry);
				battleParticipationLogFastLookup.Add(startBattle, guildBattleLogPlayerEntry);
				guildBattleLogPlayerEntry.RegisteredBattleTimeSlot = startBattle;
			}
			if (!guildBattleLogPlayerEntry.IsValidated)
			{
				guildBattleLogPlayerEntry.BattleStatus = status;
			}
			guildBattleLogPlayerEntry.BattleTimeSlot = startBattle;
		}

		public void RemoveBattleLogEntry(long registrationBattleTimeSlot)
		{
			GuildBattleLogPlayerEntry battleLogEntry = GetBattleLogEntry(registrationBattleTimeSlot);
			if (battleLogEntry != null && !battleLogEntry.IsValidated)
			{
				BattleParticipationLog.Remove(battleLogEntry);
				battleParticipationLogFastLookup.Remove(registrationBattleTimeSlot);
			}
		}

		public GuildBattleLogPlayerEntry GetBattleLogEntry(long registrationBattleTimeSlot)
		{
			GuildBattleLogPlayerEntry value = null;
			if (battleParticipationLogFastLookup.TryGetValue(registrationBattleTimeSlot, out value))
			{
				return value;
			}
			return null;
		}

		private void SetupBattleLogEntryLookup()
		{
			battleParticipationLogFastLookup = new Dictionary<long, GuildBattleLogPlayerEntry>();
			for (int i = 0; i < BattleParticipationLog.Count; i++)
			{
				GuildBattleLogPlayerEntry guildBattleLogPlayerEntry = BattleParticipationLog[i];
				battleParticipationLogFastLookup.Add(guildBattleLogPlayerEntry.RegisteredBattleTimeSlot, guildBattleLogPlayerEntry);
			}
		}

		public int GetBattleParticipationsOnPreviousGuilds()
		{
			int num = 0;
			GuildWarModel guildWarModel = base.manager.Player.GuildWarModel;
			if (guildWarModel != null)
			{
				foreach (GuildBattleLogPlayerEntry item in BattleParticipationLog)
				{
					if (item.GuildId != base.manager.Player.GuildId && guildWarModel.RegisteredPlayersForBattleSlot.ContainsKey(item.BattleTimeSlot))
					{
						num++;
					}
				}
			}
			return num;
		}

		public int GetRPGainedInWar()
		{
			int num = 0;
			GuildWarModel guildWarModel = base.manager.Player.GuildWarModel;
			if (guildWarModel != null)
			{
				foreach (GuildBattleLogPlayerEntry item in BattleParticipationLog)
				{
					if (guildWarModel.RegisteredPlayersForBattleSlot.ContainsKey(item.BattleTimeSlot))
					{
						num += item.RP;
					}
				}
			}
			return num;
		}

		public int GetVPGainedInWarForGuild()
		{
			int num = 0;
			GuildWarModel guildWarModel = base.manager.Player.GuildWarModel;
			if (guildWarModel != null)
			{
				foreach (GuildBattleLogPlayerEntry item in BattleParticipationLog)
				{
					if (guildWarModel.RegisteredPlayersForBattleSlot.ContainsKey(item.BattleTimeSlot) && item.GuildId == base.manager.Player.GuildId)
					{
						num += item.VP;
					}
				}
			}
			return num;
		}

		public void SeasonReset()
		{
			BattleParticipationLog.Clear();
			battleParticipationLogFastLookup.Clear();
		}

		public void RemovePlayerFromBattleQueue()
		{
			NotifyChange("PlayerRemovedFromQueue");
			base.manager.GvGLog("RemovePlayerFromBattleQueue: Removed player from battle queue", base.manager.Player);
		}
	}
}
