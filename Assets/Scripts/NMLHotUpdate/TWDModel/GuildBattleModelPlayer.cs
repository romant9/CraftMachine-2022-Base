using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class GuildBattleModelPlayer : TWDModelObject
	{
		public const string GuildBattleStarted = "GuildBattleStarted";

		public const string GuildBattleEnded = "GuildBattleEnded";

		public GuildBattleAttackTargetMissionData AttackTargetMission { get; set; }

		public long StartBattleTimestamp { get; set; }

		public Dictionary<int, IReward> PersonalSectorRewards { get; set; }

		public int CurrentBattleWarId { get; set; }

		public string CurrentBattleId { get; set; }

		public long CurrentBattleTimeSlot { get; set; }

		public int PersonalRewardPoints { get; set; }

		public int PersonalVictoryPoints { get; set; }

		public float VictoryRewardPointsMultiplier { get; set; }

		public float DrawRewardPointsMultiplier { get; set; }

		public int MissionCompletedAmount { get; set; }

		public bool IsFakeBattle { get; set; }

		public GuildBattleProgressSnapshot CurrentCompletionSnapshot { get; set; }

		[JsonIgnore]
		public GuildBattleMapMissionModel AttackTargetMissionModel => AttackTargetMission.MissionModel;

		public int CurrentMissionRetriedAttempts { get; set; }

		public int TotalMissionsRetried { get; set; }

		public bool RetryMission { get; set; }

		public override bool IsValid()
		{
			return true;
		}

		public GuildBattleModelPlayer()
		{
			CurrentCompletionSnapshot = new GuildBattleProgressSnapshot();
			PersonalSectorRewards = new Dictionary<int, IReward>();
			VictoryRewardPointsMultiplier = 0f;
			DrawRewardPointsMultiplier = 0f;
			PersonalRewardPoints = 0;
			PersonalVictoryPoints = 0;
			CurrentBattleWarId = -1;
			CurrentBattleTimeSlot = -1L;
			CurrentBattleId = "";
			AttackTargetMission = new GuildBattleAttackTargetMissionData();
			CurrentMissionRetriedAttempts = 0;
			TotalMissionsRetried = 0;
		}

		public override void Start()
		{
			base.Start();
			AttackTargetMission.Setup(base.manager);
		}

		public bool IsCurrentBattleActiveForPlayer()
		{
			if (base.manager.Player.IsGuildMember && base.manager.Player.GuildWarModel != null && base.manager.Player.GuildWarModel.CurrentBattle.HasStarted())
			{
				return CurrentBattleId.Equals(base.manager.Player.GuildWarModel.CurrentBattle.BattleId);
			}
			return false;
		}

		public bool IsCurrentGuildBattle()
		{
			if (base.manager.Player.IsGuildMember)
			{
				return CurrentBattleId.Equals(base.manager.Player.GuildWarModel.CurrentBattle.BattleId);
			}
			return false;
		}

		public bool IsOngoingForPlayer()
		{
			return StartBattleTimestamp > 0;
		}

		public bool HasSeenBattleStart()
		{
			return base.manager.Player.Blackboard.IsToggleOn("HasSeenGuildBattleStart");
		}

		public void SavePersonalRewardsToSnapshot()
		{
			if (base.manager.Player.GuildWarModel == null)
			{
				return;
			}
			GuildBattleModel currentBattle = base.manager.Player.GuildWarModel.CurrentBattle;
			Dictionary<int, IReward> dictionary = new Dictionary<int, IReward>();
			int count = currentBattle.CurrentMapModel.Sectors.Count;
			GuildBattleMapSectorModel guildBattleMapSectorModel = null;
			for (int i = 0; i < count; i++)
			{
				guildBattleMapSectorModel = currentBattle.CurrentMapModel.Sectors[i];
				IReward personalGuildBattleSectorCompletionBonus = currentBattle.GetPersonalGuildBattleSectorCompletionBonus(guildBattleMapSectorModel.SectorId);
				if (personalGuildBattleSectorCompletionBonus != null)
				{
					dictionary.Add(guildBattleMapSectorModel.SectorId, personalGuildBattleSectorCompletionBonus);
				}
			}
			PersonalSectorRewards = dictionary;
			VictoryRewardPointsMultiplier = currentBattle.GetGuildBattleVictoryRewardPointsMultiplier();
			DrawRewardPointsMultiplier = currentBattle.GetGuildBattleDrawRewardPointsMultiplier();
		}

		public void ResetProgressionSnapshot()
		{
			CurrentCompletionSnapshot = new GuildBattleProgressSnapshot();
		}

		public void ResetBattle()
		{
			StartBattleTimestamp = 0L;
			base.manager.Player.Blackboard.ClearToggle("HasSeenGuildBattleStart");
			ResetProgression();
		}

		public void ResetProgression()
		{
			ResetProgressionSnapshot();
			PersonalSectorRewards.Clear();
			VictoryRewardPointsMultiplier = 0f;
			DrawRewardPointsMultiplier = 0f;
			PersonalVictoryPoints = 0;
			PersonalRewardPoints = 0;
			CurrentBattleWarId = -1;
			CurrentBattleId = "";
			IsFakeBattle = false;
			CurrentBattleTimeSlot = 0L;
			MissionCompletedAmount = 0;
			CurrentMissionRetriedAttempts = 0;
			TotalMissionsRetried = 0;
			RetryMission = false;
			ClearMissionModelReferences();
		}

		public void StartBattle(long startBattleTimestamp)
		{
			ResetProgression();
			SavePersonalRewardsToSnapshot();
			PlayerModel player = base.manager.Player;
			GuildModel guildModel = player.GuildModel;
			player.GetCurrency(CurrencyType.GvGMissionKey).SetValue(base.gameEconomyData.GuildWarConfig.KeysPerBattle);
			StartBattleTimestamp = startBattleTimestamp;
			CurrentBattleId = player.GuildWarModel.CurrentBattle.BattleId;
			CurrentBattleTimeSlot = player.GuildWarModel.CurrentBattle.TimeSlot;
			CurrentBattleWarId = player.GuildWarModel.WarDefinitionId;
			IsFakeBattle = player.GuildWarModel.CurrentBattle.IsFakeBattle;
			IServerService serverService = base.manager.ServerService;
			if (serverService != null)
			{
				string playerEmblem = base.manager.GetMessageSerializer().Serialize(player.PlayerEmblem);
				int totalVictoryPointsForPlayer = guildModel.GuildWarModel.CurrentBattle.GetTotalVictoryPointsForPlayer(player.HashedId);
				LeaderboardEntry entry = Leaderboards.CreateGuildBattlePlayersScoreLeaderboardEntry(base.manager, player.Name, player.HashedId, guildModel.Id, playerEmblem, totalVictoryPointsForPlayer);
				serverService.SaveLeaderboardEntry(CurrentBattleId, entry);
			}
			base.manager.Player.Blackboard.ClearToggle("HasSeenGuildBattleEnd");
			NotifyChange("GuildBattleStarted");
		}

		public void EndBattle()
		{
			Metrics metrics = base.manager.Metrics;
			metrics.AddEnd().AddGvG().AddPlayer(null)
				.AddGvGBattle();
			if (IsCurrentGuildBattle())
			{
				metrics.AddGvGBattleResult();
				if (base.manager.Player.IsGuildMember)
				{
					int allTimeVpTotalForPlayer = base.manager.Player.GuildModel.GetAllTimeVpTotalForPlayer(base.manager.Player.HashedId);
					base.manager.Player.UpdatePersonalTotalVpForGuild(base.manager.Player.GuildId, allTimeVpTotalForPlayer);
				}
			}
			metrics.Send();
			ResetBattle();
			NotifyChange("GuildBattleEnded");
		}

		public TWDModelResult AttackMission(GuildBattleMapModel mapModel, GuildBattleMapMissionModel mapMissionModel, GuildBattleMapSectorModel sectorModel, SurvivorContainerModel container)
		{
			if (container.CombatSurvivors.Count < 1)
			{
				return TWDModelResult.NotEnoughSurvivors;
			}
			Cashier cashier = mapMissionModel?.GetStartMissionCashier(base.manager);
			if (cashier != null && !cashier.CanAffordWithDiamonds())
			{
				return TWDModelResult.NotEnoughCurrency;
			}
			if (mapMissionModel != null)
			{
				AttackTargetMission.AttackMission(mapMissionModel);
				if (AttackTargetMission.MissionModel == null)
				{
					return TWDModelResult.Error;
				}
				if (!CheckSurvivorsForAttack())
				{
					return TWDModelResult.Error;
				}
				if (!AttackTargetMission.MissionModel.SectorModelOwner.CanBeUnlocked(mapModel))
				{
					return TWDModelResult.Error;
				}
				base.manager.Player.ShouldConsumeMissionCurrency = true;
				CurrentMissionRetriedAttempts = 0;
				RetryMission = false;
				return TWDModelResult.OK;
			}
			return TWDModelResult.InvalidPosition;
		}

		public TWDModelResult ReplayMission(GuildBattleMapModel mapModel, GuildBattleMapMissionModel mapMissionModel, SurvivorContainerModel container)
		{
			if (container.CombatSurvivors.Count < 1)
			{
				return TWDModelResult.NotEnoughSurvivors;
			}
			Cashier cashier = mapMissionModel?.GetRetryGvGMissionCashier(base.manager);
			if (cashier != null && !cashier.CanAffordWithDiamonds())
			{
				return TWDModelResult.NotEnoughCurrency;
			}
			if (mapMissionModel != null)
			{
				AttackTargetMission.AttackMission(mapMissionModel);
				if (AttackTargetMission.MissionModel == null)
				{
					return TWDModelResult.Error;
				}
				if (!CheckSurvivorsForAttack())
				{
					return TWDModelResult.Error;
				}
				if (!CanRetryMission())
				{
					return TWDModelResult.Error;
				}
				if (!AttackTargetMission.MissionModel.SectorModelOwner.CanBeUnlocked(mapModel))
				{
					return TWDModelResult.Error;
				}
				base.manager.Player.ShouldConsumeMissionCurrency = true;
				RetryMission = true;
				return TWDModelResult.OK;
			}
			return TWDModelResult.InvalidPosition;
		}

		public void UpdateRetriedMissionAttempts()
		{
			CurrentMissionRetriedAttempts++;
			TotalMissionsRetried++;
		}

		public bool CanRetryMission()
		{
			return base.manager.GameEconomyData.GuildWarConfig.MaxAmountOfRetries > CurrentMissionRetriedAttempts;
		}

		private bool CheckSurvivorsForAttack()
		{
			bool flag = true;
			bool disableOutpostHeroLimits = base.manager.GameEconomyData.ConfigData.DisableOutpostHeroLimits;
			SurvivorContainerModel survivorContainer = base.manager.Player.SurvivorContainer;
			for (int i = 0; i < survivorContainer.CombatSurvivors.Count; i++)
			{
				SurvivorModel survivorModel = survivorContainer.CombatSurvivors[i];
				flag &= disableOutpostHeroLimits || !survivorContainer.IsOutpostDefending(survivorModel);
				flag &= survivorModel.InjuryType == InjuryType.None;
				flag &= survivorContainer.Survivors.Contains(survivorModel);
				flag &= !survivorModel.IsUpgrading();
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		public void ClearMissionModelReferences()
		{
			AttackTargetMission.Clear();
			if (base.manager != null && base.manager.Player != null)
			{
				base.manager.Player.ResetIAttackTargetMapMission();
			}
		}

		public void ReturnFromCombat()
		{
			AttackTargetMission.ReturnFromCombat();
			if (base.manager != null && base.manager.Player != null)
			{
				base.manager.Player.ResetIAttackTargetMapMission();
			}
			if (base.manager.Player.IsGuildMember)
			{
				int allTimeVpTotalForPlayer = base.manager.Player.GuildModel.GetAllTimeVpTotalForPlayer(base.manager.Player.HashedId);
				base.manager.Player.UpdatePersonalTotalVpForGuild(base.manager.Player.GuildId, allTimeVpTotalForPlayer);
			}
		}

		public void AddPersonalMissionProgression()
		{
			if (IsOngoingForPlayer())
			{
				MissionCompletedAmount++;
			}
		}

		public TWDModelResult GiveSectorBonusRewards(int sectorId)
		{
			if (PersonalSectorRewards.TryGetValue(sectorId, out var value))
			{
				value.Give(base.manager);
				PersonalSectorRewards.Remove(sectorId);
				GuildBattleMapSectorModel sectorModel = base.manager.Player.GuildModel.GuildWarModel.CurrentBattle.CurrentMapModel.GetSectorModel(sectorId);
				int amount = ((RewardCurrency)value).Amount;
				PersonalRewardPoints += amount;
				base.manager.Metrics.PushResource(CurrencyType.GuildBattleRP, amount);
				base.manager.Metrics.AddFind().AddResources().AddSector(sectorModel)
					.AddGvG()
					.AddGvGBattle()
					.Send();
				return TWDModelResult.OK;
			}
			base.manager.GvGLogWarning($"No sector rewards available for Battle {CurrentBattleId} -Sector {sectorId}");
			return TWDModelResult.Skip;
		}

		public IReward GetSectorRewardForCurrentBattle(int sectorId, bool claimableOnly = false)
		{
			IReward value = null;
			if (!claimableOnly || base.manager.Player.GuildWarModel.CurrentBattle.CompletedSectors.Contains(sectorId))
			{
				PersonalSectorRewards.TryGetValue(sectorId, out value);
			}
			return value;
		}

		public IReward GetSectorRewardForFinishedBattle(int warId, string battleId, int sectorId)
		{
			if (base.manager.Player.GuildModel == null)
			{
				return null;
			}
			IReward value = null;
			GvGSeasonModel.GuildBattleLogEntry battleLogEntry = base.manager.Player.GuildModel.GvGSeasonModel.GetBattleLogEntry(warId, battleId);
			if (battleLogEntry != null && battleLogEntry.CompletedSectors.Contains(sectorId))
			{
				PersonalSectorRewards.TryGetValue(sectorId, out value);
			}
			return value;
		}

		public void UpdatePersonalRewardPointsWithPendingRewards()
		{
			if (base.manager.Player.GuildModel == null)
			{
				return;
			}
			foreach (int key in PersonalSectorRewards.Keys)
			{
				if (GetSectorRewardForFinishedBattle(CurrentBattleWarId, CurrentBattleId, key) is RewardCurrency { CurrencyType: CurrencyType.GuildBattleRP } rewardCurrency)
				{
					PersonalRewardPoints += rewardCurrency.Amount;
				}
			}
		}

		public List<RewardCurrency> GetClaimableBattleRewards()
		{
			List<RewardCurrency> list = new List<RewardCurrency>();
			if (base.manager.Player.GuildModel != null)
			{
				foreach (int key in PersonalSectorRewards.Keys)
				{
					if (GetSectorRewardForFinishedBattle(CurrentBattleWarId, CurrentBattleId, key) is RewardCurrency item)
					{
						list.Add(item);
					}
				}
				int battleBonusRewardPointsAmount = GetBattleBonusRewardPointsAmount();
				if (battleBonusRewardPointsAmount > 0)
				{
					RewardCurrency rewardCurrency = new RewardCurrency();
					rewardCurrency.CurrencyType = CurrencyType.GuildBattleRP;
					rewardCurrency.Amount = battleBonusRewardPointsAmount;
					list.Add(rewardCurrency);
				}
			}
			return list;
		}

		public int GetBattleBonusRewardPointsAmount()
		{
			if (base.manager.Player.GuildModel == null)
			{
				return 0;
			}
			GvGSeasonModel.GuildBattleLogEntry battleLogEntry = base.manager.Player.GuildModel.GvGSeasonModel.GetBattleLogEntry(CurrentBattleWarId, CurrentBattleId);
			if (battleLogEntry == null || battleLogEntry.Result == 2)
			{
				return 0;
			}
			if (battleLogEntry.IsVictory)
			{
				return (int)((float)PersonalRewardPoints * VictoryRewardPointsMultiplier);
			}
			return (int)((float)PersonalRewardPoints * DrawRewardPointsMultiplier);
		}

		public void GiveBattleRewards()
		{
			UpdatePersonalRewardPointsWithPendingRewards();
			List<RewardCurrency> claimableBattleRewards = GetClaimableBattleRewards();
			if (claimableBattleRewards.Count <= 0)
			{
				return;
			}
			foreach (RewardCurrency item in claimableBattleRewards)
			{
				item.Give(base.manager);
				base.manager.Metrics.AddFind().AddResources(item.CurrencyType, item.Amount, item.AmountActuallyAdded).AddEnd()
					.AddGvG()
					.AddGvGBattle()
					.Send();
			}
		}
	}
}
