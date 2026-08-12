using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using UnityEngine;

namespace TWDModel
{
	public class DebugCheatCommand : ModelCommand
	{
		private class LeaderboardData
		{
			public SurvivorClass SurvivorClass { get; set; }

			public string BoardName { get; set; }

			public LeaderboardPosition PlayerPosition { get; set; }

			public List<LeaderboardEntry> Top100Entries { get; set; }
		}

		public DebugCheatCommandType CommandType;

		public int CommandParameter;

		public int CommandParameter1;

		public int SurvivorLevel;

		public int TraitStarLevel;

		public string CheatParameter;

		public string LevelName;

		public int EquipmentTier;

		public int EquipmentRarityLevel;

		public int AuxInt;

		public StorePurchaseInfo FakeStorePurchaseInfo;

		public WalkerType Walker;

		public SurvivorClass SurvivorType;

		public CurrencyType Currency;

		public SurvivorModel Survivor;

		public int CommandSkillPerformSurvivorIndex;

		public int CommandSkillPerformSurvivorSkillIndex;

		public int CommandSkillTargetSurvivorIndex;

		public int SurvivalSlot;

		public int RestoreIndex;

		public AttributeType PlayerAttributeType;

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			TWDModelResult result = TWDModelResult.OK;
			TWDModelManager tWDModelManager = manager as TWDModelManager;
			manager.Debug.Log("Applying cheat: " + CommandType);
			switch (CommandType)
			{
			case DebugCheatCommandType.GiveFullCurrencies:
				DoGiveFullCurrencies(tWDModelManager);
				break;
			case DebugCheatCommandType.GiveFullGas:
				DoGiveFullGas(tWDModelManager);
				break;
			case DebugCheatCommandType.ConsumeAllGas:
				DoConsumeAllGas(tWDModelManager);
				break;
			case DebugCheatCommandType.ConsumeAllSupplies:
				DoConsumeAllSupplies(tWDModelManager);
				break;
			case DebugCheatCommandType.ConsumeAllSP:
				DoConsumeAllSurvivalPoints(tWDModelManager);
				break;
			case DebugCheatCommandType.ConsumeAllTradeGoods:
				DoConsumeAllTradeGoods(tWDModelManager);
				break;
			case DebugCheatCommandType.ConsumeAllGold:
				DoConsumeAllGold(tWDModelManager);
				break;
			case DebugCheatCommandType.WinCombat:
				DoWinCombat(tWDModelManager);
				break;
			case DebugCheatCommandType.LoseCombat:
				DoLoseCombat(tWDModelManager);
				break;
			case DebugCheatCommandType.UnlockAll:
				DoUnlockAll(tWDModelManager);
				break;
			case DebugCheatCommandType.CompleteCurrenEpisode:
				DoCompleteCurrentEpisode(tWDModelManager);
				break;
			case DebugCheatCommandType.CompleteChallengeRound:
				DoCompleteChallengeRound(tWDModelManager);
				break;
			case DebugCheatCommandType.CompleteSurvivalMission:
				DoCompleteSurvivalMission(tWDModelManager);
				break;
			case DebugCheatCommandType.CompleteManySurvivalMissions:
				DoCompleteManySurvivalMissions(tWDModelManager);
				break;
			case DebugCheatCommandType.ResetSurvivalToUncompleted:
				DoResetSurvivalToUncompleted(tWDModelManager);
				break;
			case DebugCheatCommandType.ResetSurvivalToDifficultySelection:
				DoResetSurvivalToDifficultySelection(tWDModelManager);
				break;
			case DebugCheatCommandType.ToggleSurvivalDifficulty:
				DoToggleSurvivalDifficulty(tWDModelManager);
				break;
			case DebugCheatCommandType.RecreateWeeklySurvival:
				DoRecreateWeeklySurvival(tWDModelManager);
				break;
			case DebugCheatCommandType.CompleteTutorial:
				DoCompleteTutorial(tWDModelManager);
				break;
			case DebugCheatCommandType.CompleteCombatTutorial:
				DoCompleteCombatTutorial(tWDModelManager);
				break;
			case DebugCheatCommandType.AdvanceToEarlyGame:
				DoCompleteTutorial(tWDModelManager);
				DoAdvanceGame(tWDModelManager, 30, useForOutpost: true);
				break;
			case DebugCheatCommandType.AdvanceToLateGame:
				DoCompleteTutorial(tWDModelManager);
				DoAdvanceGame(tWDModelManager, 30);
				GiveLateGameTeam(tWDModelManager);
				DoCompleteCurrentEpisode(tWDModelManager);
				break;
			case DebugCheatCommandType.AdvanceToMaxCouncil:
				DoCompleteTutorial(tWDModelManager);
				DoAdvanceGame(tWDModelManager, tWDModelManager.GameEconomyData.ConfigData.ForceCouncilMaxLevel);
				GiveLateGameTeam(tWDModelManager);
				DoCompleteCurrentEpisode(tWDModelManager);
				break;
			case DebugCheatCommandType.GiveFlipflops:
				DoGiveFlipFlops(tWDModelManager);
				break;
			case DebugCheatCommandType.CombatKillAllWalkers:
				DoKillAllWalkers(tWDModelManager);
				break;
			case DebugCheatCommandType.SetAllWalkersOnFire:
				DoSetAllWalkersOnFire(tWDModelManager);
				break;
			case DebugCheatCommandType.SetAllWalkerBleeding:
				DoSetAllWalkersBleeding(tWDModelManager);
				break;
			case DebugCheatCommandType.StunAllEnemies:
				DoStunAllEnemies(tWDModelManager);
				break;
			case DebugCheatCommandType.ElectronShockAllEnemies:
				DoElectronShockAllEnemies(tWDModelManager);
				break;
			case DebugCheatCommandType.ElectronShockLeaderSurvivor:
				DoElectronShockLeaderSurvivor(tWDModelManager);
				break;
			case DebugCheatCommandType.RootLeaderSurvivor:
				DoRootLeaderSurvivor(tWDModelManager);
				break;
			case DebugCheatCommandType.CombatKillAllEnemies:
				DoKillAllEnemies(tWDModelManager);
				break;
			case DebugCheatCommandType.Add10Stars:
				if (!tWDModelManager.Player.WeeklyChallenge.Finished)
				{
					tWDModelManager.Player.WeeklyChallenge.AddPersonalStars(10);
				}
				break;
			case DebugCheatCommandType.Add100Stars:
				if (!tWDModelManager.Player.WeeklyChallenge.Finished)
				{
					tWDModelManager.Player.WeeklyChallenge.AddPersonalStars(100);
				}
				break;
			case DebugCheatCommandType.Give15Equipments:
				DoGive15Equipments(tWDModelManager, EquipmentTier, EquipmentRarityLevel);
				break;
			case DebugCheatCommandType.MinorInjuryCurrentTeam:
				DoInjuryCurrentTeam(tWDModelManager, InjuryType.Minor);
				break;
			case DebugCheatCommandType.MajorInjuryCurrentTeam:
				DoInjuryCurrentTeam(tWDModelManager, InjuryType.Major);
				break;
			case DebugCheatCommandType.CriticalInjuryCurrentTeam:
				DoInjuryCurrentTeam(tWDModelManager, InjuryType.Critical);
				break;
			case DebugCheatCommandType.PassTime:
				DoPassTime(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.Attack:
				DoGiveFullCurrencies(tWDModelManager);
				DoUnlockAll(tWDModelManager);
				result = DoAttackLevel(tWDModelManager, LevelName);
				break;
			case DebugCheatCommandType.AttackPVP:
			{
				List<MatchMakingInfo> list3 = new List<MatchMakingInfo>();
				if (string.IsNullOrEmpty(LevelName))
				{
					list3.Add(new MatchMakingInfo
					{
						PlayerHashedId = tWDModelManager.Player.HashedId
					});
				}
				else
				{
					list3.Add(new MatchMakingInfo
					{
						PlayerHashedId = LevelName
					});
				}
				tWDModelManager.SetMatchData("", list3);
				tWDModelManager.Player.OutpostModel.MatchMakingPaid = true;
				result = TWDModelResult.OK;
				break;
			}
			case DebugCheatCommandType.MakeLimitedBundleAvailable:
				tWDModelManager.Player.BundleManager.DEBUG_forceHighestPriorityOfferState(forceAvailable: true, 5000L);
				break;
			case DebugCheatCommandType.MakeLimitedBundleUnavailable:
				tWDModelManager.Player.BundleManager.DEBUG_forceHighestPriorityOfferState(forceAvailable: false, 5000L);
				break;
			case DebugCheatCommandType.ExpireLimitedBundleTimestamp:
				tWDModelManager.Player.BundleManager.DEBUG_forceHighestPriorityOfferEndTimestamp(10000L);
				break;
			case DebugCheatCommandType.SpawnWalker:
				DoSpawnWalker(tWDModelManager);
				break;
			case DebugCheatCommandType.CompleteNextMission:
				DoCompleteNextMission(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.CombatSetTimeToMin:
				DoSetCombatTimer(tWDModelManager, 60);
				break;
			case DebugCheatCommandType.CombatSetTimeTo5Min:
				DoSetCombatTimer(tWDModelManager, 300);
				break;
			case DebugCheatCommandType.AddDefenseLogEntry1:
				DoAddDefenseLogEntries(tWDModelManager, 1);
				break;
			case DebugCheatCommandType.AddDefenseLogEntry5:
				DoAddDefenseLogEntries(tWDModelManager, 5);
				break;
			case DebugCheatCommandType.SetOutpostRepairTimer1min:
				DoSetOutpostRepairTimer(tWDModelManager, 60);
				break;
			case DebugCheatCommandType.SetOutpostRepairTimer60min:
				DoSetOutpostRepairTimer(tWDModelManager, 3600);
				break;
			case DebugCheatCommandType.AddAttackLogEntry1:
				DoAddAttackLogEntries(tWDModelManager, 1);
				break;
			case DebugCheatCommandType.AddAttackLogEntry5:
				DoAddAttackLogEntries(tWDModelManager, 5);
				break;
			case DebugCheatCommandType.GiveInfluence:
				DoAddInfluence(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.ReduceInfluence:
				DoAddInfluence(tWDModelManager, -CommandParameter);
				break;
			case DebugCheatCommandType.GiveChallengeDebugTeam:
				DoGiveChallengeDebugTeam(tWDModelManager);
				break;
			case DebugCheatCommandType.RerollDailyQuests:
				DoRerollDailyQuests(tWDModelManager);
				break;
			case DebugCheatCommandType.GiveFullClassTokens:
				DoGiveFullClassTokens(tWDModelManager);
				break;
			case DebugCheatCommandType.GiveFullHeroTokens:
				DoGiveFullHeroTokens(tWDModelManager);
				break;
			case DebugCheatCommandType.GiveEquipTokens:
				DoGiveEquipTokens(tWDModelManager, CheatParameter);
				break;
			case DebugCheatCommandType.ResetChallengeApocalypseBuffs:
				ResetChallengeApocalypseBuffs(tWDModelManager, CheatParameter);
				break;
			case DebugCheatCommandType.CompleteChallengeRoundsSkipReward:
				CompleteChallengeRoundsSkipReward(tWDModelManager, CommandParameter, isApolytic: false);
				break;
			case DebugCheatCommandType.CompleteApolyticChallengeRoundsSkipReward:
				CompleteChallengeRoundsSkipReward(tWDModelManager, CommandParameter, isApolytic: true);
				break;
			case DebugCheatCommandType.SubscriptionWeekly:
				AddSubscription(tWDModelManager, isWeek: true);
				break;
			case DebugCheatCommandType.SubscriptionMonthly:
				AddSubscription(tWDModelManager, isWeek: false);
				break;
			case DebugCheatCommandType.SubscriptionClear:
				ClearSubscription(tWDModelManager, isWeek: false);
				break;
			case DebugCheatCommandType.GiveCurrency:
				DoGiveCurrency(tWDModelManager, Currency, CommandParameter);
				break;
			case DebugCheatCommandType.ActivateShieldFor10Min:
				DoActivateShieldFor10Minutes(tWDModelManager);
				break;
			case DebugCheatCommandType.RefreshTradeShop:
				DoRefreshTrafeShop(tWDModelManager);
				break;
			case DebugCheatCommandType.ResetSeasonCooldowns:
				ResetSeasonCooldowns(tWDModelManager);
				break;
			case DebugCheatCommandType.GenerateChallengePersonalRewards:
				DoGiveRewards(tWDModelManager, 50, WeeklyChallengeReward.ChallengeRewardType.PersonalStars, tWDModelManager.Player.WeeklyChallenge.NumberStars);
				break;
			case DebugCheatCommandType.GenerateChallengeRoundCompleteRewards:
				DoGiveRewards(tWDModelManager, 50, WeeklyChallengeReward.ChallengeRewardType.RoundCompletion, tWDModelManager.Player.WeeklyChallenge.CurrentRequiredSurvivorLevel);
				break;
			case DebugCheatCommandType.CollectAllPendingChallengeRewards:
				tWDModelManager?.Player.WeeklyChallenge?.DEBUG_clearAllPendingRewards();
				break;
			case DebugCheatCommandType.AddXPBooster:
				AddBooster(tWDModelManager, TimedBonusType.DoubleXp);
				break;
			case DebugCheatCommandType.AddUnlimitedGas:
				AddBooster(tWDModelManager, TimedBonusType.UnlimitedGas);
				break;
			case DebugCheatCommandType.GiveComponents:
				DoGiveComponents(tWDModelManager);
				break;
			case DebugCheatCommandType.ConsumeComponents:
				DoConsumeComponents(tWDModelManager);
				break;
			case DebugCheatCommandType.CreateRandomBadges:
				CreateRandomBadges(tWDModelManager);
				break;
			case DebugCheatCommandType.EquipOneBadgePerSurvivor:
				EquipOneBadgePerSurvivor(tWDModelManager);
				break;
			case DebugCheatCommandType.SurvivalModeOutOfAction:
				DoSurvivalModeOutOfAction(tWDModelManager);
				break;
			case DebugCheatCommandType.SkipToSurvivalMissionX:
				DoSkipToSurvivalMapX(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.RegenerateDailyQuests:
				RegenerateDailyQuests(tWDModelManager);
				break;
			case DebugCheatCommandType.CompleteAllDailyQuests:
				CompleteAllDailyQuests(tWDModelManager);
				break;
			case DebugCheatCommandType.Add10VictoryPoints:
				if (!tWDModelManager.Player.GuildWarModel.CurrentBattle.IsBiggerThanEndBattleTimeStamp(tWDModelManager.Player.UtcTimeStamp))
				{
					tWDModelManager.Player.GuildWarModel.CurrentBattle.UpdateMissionProgressionRewardsForGuildBattle(tWDModelManager, 0, "", "", 10);
				}
				break;
			case DebugCheatCommandType.CompleteAllSectors:
				CompleteAllSectors(tWDModelManager);
				break;
			case DebugCheatCommandType.CompleteNextSector:
				CompleteNextSector(tWDModelManager);
				break;
			case DebugCheatCommandType.FindNextEnemyInSector:
				FindNextEnemyInNextSector(tWDModelManager);
				break;
			case DebugCheatCommandType.FindAndKillNextEnemyInSector:
				FindAndKillNextEnemyInNextSector(tWDModelManager);
				break;
			case DebugCheatCommandType.AddCampaignTokens:
				AddCampaignTokens(tWDModelManager);
				break;
			case DebugCheatCommandType.GiveFullGvGGas:
				DoGiveFullGvGGas(tWDModelManager);
				break;
			case DebugCheatCommandType.GiveFullGvGBattleKeys:
				DoGiveFullGvGBattleKeys(tWDModelManager);
				break;
			case DebugCheatCommandType.Give100RewardPoints:
				DoGive100RewardPoints(tWDModelManager);
				break;
			case DebugCheatCommandType.RestockGuildShop:
				RestockGuildShop(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.ConsumeAllGvGGas:
				DoConsumeAllGvGGas(tWDModelManager);
				break;
			case DebugCheatCommandType.ConsumeAllBattlePasses:
				DoConsumeAllBattlePasses(tWDModelManager);
				break;
			case DebugCheatCommandType.GiveEquipment:
				DoGiveEquipment(tWDModelManager, CheatParameter, EquipmentTier, EquipmentRarityLevel, CommandParameter);
				break;
			case DebugCheatCommandType.ConsumeAllEquipmentUpgradeTokens:
				DoConsumeAllEquipmentUpgrateTokens(tWDModelManager);
				break;
			case DebugCheatCommandType.GiveEquipmentUpgradeTokens:
				GiveEquipmentUpgradeTokens(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.GiveTraitRerollTokens:
				GiveTraitRerollToken(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.ConsumeAllTraitRerollTokens:
				DoConsumeAllTraitRerollTokens(tWDModelManager);
				break;
			case DebugCheatCommandType.PrintGuildModelJSON:
				PrintGuildModel(tWDModelManager);
				break;
			case DebugCheatCommandType.InjureAllSurvivorsToOneHealth:
				InjureAllSurvivorsToMinGreenHealth(tWDModelManager);
				break;
			case DebugCheatCommandType.HealAllSurvivorsToMaxHealth:
				HealAllSurvivorsToMaxHealth(tWDModelManager);
				break;
			case DebugCheatCommandType.InjureAllWalkersToOneHealth:
				InjureAllWalkersToMinHp(tWDModelManager);
				break;
			case DebugCheatCommandType.SetTeamOnFire:
				SetTeamOnFire(tWDModelManager);
				break;
			case DebugCheatCommandType.SetTeamBleeding:
				SetTeamBleeding(tWDModelManager);
				break;
			case DebugCheatCommandType.RefreshBlackMarketHero:
				RefreshBlackMarketHero(tWDModelManager);
				break;
			case DebugCheatCommandType.BuyDebugIAPProduct:
				DebugBuyIAPProduct(tWDModelManager, FakeStorePurchaseInfo);
				break;
			case DebugCheatCommandType.DebugBuyCustomBundle:
				DebugBuyCustomIAPProduct(tWDModelManager, FakeStorePurchaseInfo);
				break;
			case DebugCheatCommandType.DebugAddAttempDataNormal:
			{
				List<int> list2 = (from x in CheatParameter.Split(',')
					select int.Parse(x)).ToList();
				if (list2.Count == 3 && tWDModelManager.Player.EndlessModeManager.CheckCanScanNormal())
				{
					tWDModelManager.Player.EndlessModeManager.DebugAddAttempDataNormal(list2[0], list2[1], list2[2]);
				}
				break;
			}
			case DebugCheatCommandType.DebugAddAttempDataExpert:
			{
				List<int> list = (from x in CheatParameter.Split(',')
					select int.Parse(x)).ToList();
				if (list.Count == 3 && tWDModelManager.Player.EndlessModeManager.CheckCanScanExpert())
				{
					tWDModelManager.Player.EndlessModeManager.DebugAddAttempDataExpert(list[0], list[1], list[2]);
				}
				break;
			}
			case DebugCheatCommandType.AddEntriesToEndlessLeaderboard:
				AddEntriesToEndlessLeaderboard(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.CompleteAllStoryMissionsAndGrinds:
				DoCompleteAllEpisodesAndGrinds(tWDModelManager);
				break;
			case DebugCheatCommandType.DoInjury:
				DoInjury(tWDModelManager);
				break;
			case DebugCheatCommandType.DoCombatSpawnWalkers:
				DoCombatSpawnWalkers(tWDModelManager, Walker, CommandParameter, AuxInt);
				break;
			case DebugCheatCommandType.AddSurvivalPass:
				AddBattlePassTokens(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.MaximizeSurvivalPassThroughKills:
				MaximizePassTokensThroughKills(tWDModelManager);
				break;
			case DebugCheatCommandType.MaximizeSurvivalPass:
				MaximizeSurvivalPassTokens(tWDModelManager);
				break;
			case DebugCheatCommandType.ActivateBattlePassPremium:
				ActivateBattlePassPremium(tWDModelManager);
				break;
			case DebugCheatCommandType.FakeBattlePassSeasonEnd:
				FakeBattlePassSeasonEnd(tWDModelManager);
				break;
			case DebugCheatCommandType.RemoveSurvivorEquipment:
				RemoveSurvivorEquipment(tWDModelManager, CheatParameter);
				break;
			case DebugCheatCommandType.RemoveSurvivor:
				RemoveSurvivor(tWDModelManager, Survivor);
				break;
			case DebugCheatCommandType.GiveRewards:
				GiveRewards(tWDModelManager, CheatParameter);
				break;
			case DebugCheatCommandType.Charge1AllAllSurvivors:
				ChargeAllAllSurvivors(tWDModelManager, 1);
				break;
			case DebugCheatCommandType.Charge3AllAllSurvivors:
				ChargeAllAllSurvivors(tWDModelManager, 3);
				break;
			case DebugCheatCommandType.ModifyHillTopToken:
				DoModifyHillTopToken(tWDModelManager, CheatParameter);
				break;
			case DebugCheatCommandType.ActiveFoundationDayPremium:
				DoActiveFoundationDayPremium(tWDModelManager);
				break;
			case DebugCheatCommandType.CompleteNewbieDayQuests:
				CompleteNewbieDayQuests(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.CustomBundle:
				ByCustomBundle(tWDModelManager, 1);
				break;
			case DebugCheatCommandType.CostLeaderMoveCompleted:
				DebugTestLeaderAPCost(tWDModelManager, 1);
				break;
			case DebugCheatCommandType.CostLeaderSecondMoveCompleted:
				DebugTestLeaderAPCost(tWDModelManager, 2);
				break;
			case DebugCheatCommandType.RestoreLeaderAction:
				DebugRestoreLeaderAction(tWDModelManager, SurvivalSlot, RestoreIndex);
				break;
			case DebugCheatCommandType.CostLeaderAbilityCompleted:
				DebugTestLeaderAPCost(tWDModelManager, 3);
				break;
			case DebugCheatCommandType.PerformLeaderCommandSkillToSecondSurvivor:
				PerformCommandSkillToTarget(tWDModelManager, CommandSkillPerformSurvivorIndex, CommandSkillPerformSurvivorSkillIndex, CommandSkillTargetSurvivorIndex);
				break;
			case DebugCheatCommandType.SetSurLevelAndStar:
				SetSurvivorLevelAndStarLevel(tWDModelManager, CheatParameter, SurvivorLevel, TraitStarLevel);
				break;
			case DebugCheatCommandType.ClearPhone:
				ClearPhone(tWDModelManager);
				break;
			case DebugCheatCommandType.SetActiveFoundationPremium:
				SetActiveFoundationPremium(tWDModelManager);
				break;
			case DebugCheatCommandType.SetApocalypticSkipToken:
				SetApocalypticSkipToken(tWDModelManager);
				break;
			case DebugCheatCommandType.InjureAllCivilianAndRaiderToOneHealth:
				InjureAlCivilianAndRaiderToMinHp(tWDModelManager);
				break;
			case DebugCheatCommandType.DoCombatSpawnRaider:
				DoCombatSpawnRaiders(tWDModelManager, SurvivorType, CommandParameter, AuxInt);
				break;
			case DebugCheatCommandType.ClearFullCurrencies:
				DoClearFullCurrencies(tWDModelManager);
				break;
			case DebugCheatCommandType.PlayerAttribute:
				DoGetPlayerAttribute(tWDModelManager);
				break;
			case DebugCheatCommandType.UpgradeSurvivalManualAttributeLeve:
				UpgradeSurvivalManualAttributelevel(tWDModelManager);
				break;
			case DebugCheatCommandType.UpgradeSurvivalManualStorySkill:
				UpgradeSurvivalManualStorySkill(tWDModelManager);
				break;
			case DebugCheatCommandType.UpgradeSurvivalManualActor:
				UpgradeSurvivalManualActor(tWDModelManager);
				break;
			case DebugCheatCommandType.UnlockSurvivalManualActorStory:
				DoUnlockSurvivalManualActorStory(tWDModelManager);
				break;
			case DebugCheatCommandType.InitializeRoulette:
				DoInitializeRoulette(tWDModelManager);
				break;
			case DebugCheatCommandType.ResetRoulette:
				DoResetRoulette(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.FreeRouletteDraw:
				DoFreeRouletteDraw(tWDModelManager, CommandParameter, isMultiDraw: false);
				break;
			case DebugCheatCommandType.FreeRouletteMultiDraw:
				DoFreeRouletteDraw(tWDModelManager, CommandParameter, isMultiDraw: true);
				break;
			case DebugCheatCommandType.GetRouletteStatus:
				DoGetRouletteStatus(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.AddRouletteCurrency:
				DoAddRouletteCurrency(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.SetRouletteSystemEnable:
				DoSetRouletteSystemEnable(tWDModelManager, CommandParameter);
				break;
			case DebugCheatCommandType.SetRouletteActivityTime:
				DoSetRouletteActivityTime(tWDModelManager, CheatParameter);
				break;
			case DebugCheatCommandType.ListRouletteConfigs:
				DoListRouletteConfigs(tWDModelManager);
				break;
			case DebugCheatCommandType.ResetRouletteOpenLevel:
				DoResetRouletteOpenLevel(tWDModelManager, CommandParameter, CommandParameter1);
				break;
			case DebugCheatCommandType.FetchEndlessSurvivorClassLeaderboard:
				DoFetchEndlessSurvivorClassLeaderboard(tWDModelManager);
				break;
			}
			return new NGModelCommandRespond(this, result);
		}

		private void DoModifyHillTopToken(TWDModelManager twdModelManager, string cheatParameter)
		{
			int value = int.Parse(cheatParameter);
			twdModelManager?.Player.GetCurrency(CurrencyType.HillTopCoin).SetValue(value);
		}

		private void DoCombatSpawnWalkers(TWDModelManager twdModelManager, WalkerType walker, int amount, int level)
		{
			if ((twdModelManager?.CombatModel)?.OrderedSpawnPoints.FirstOrDefault((ActorSpawnPointModel x) => x.GetType() == typeof(WalkerSpawnPointModel)) is WalkerSpawnPointModel walkerSpawnPointModel)
			{
				walkerSpawnPointModel.OverrideWalkerType = walker;
				walkerSpawnPointModel.SpawnCountPerAction = amount;
				walkerSpawnPointModel.OverrideWalkerLevel = level;
				walkerSpawnPointModel.UseOverrideWalkerType = true;
				walkerSpawnPointModel.Activate(instant: true);
				walkerSpawnPointModel.State = SpawnPointState.Deactive;
			}
		}

		private void DoCombatSpawnRaiders(TWDModelManager twdModelManager, SurvivorClass survivorClass, int amount, int level)
		{
			RaiderSpawnPointModel raiderSpawnPointModel = (twdModelManager?.CombatModel)?.OrderedSpawnPoints.FirstOrDefault((ActorSpawnPointModel x) => x.GetType() == typeof(RaiderSpawnPointModel)) as RaiderSpawnPointModel;
			if (raiderSpawnPointModel == null)
			{
				raiderSpawnPointModel = new RaiderSpawnPointModel();
			}
			raiderSpawnPointModel.Class = survivorClass;
			raiderSpawnPointModel.SpawnCountPerAction = amount;
			for (int num = 0; num < amount; num++)
			{
				raiderSpawnPointModel.LevelOffset = level;
				raiderSpawnPointModel.State = SpawnPointState.Deactive;
				raiderSpawnPointModel.Activate(instant: true);
				raiderSpawnPointModel.State = SpawnPointState.Deactive;
			}
		}

		private void AddEntriesToEndlessLeaderboard(TWDModelManager twdModelManager, int entries)
		{
			if (twdModelManager.ServerService == null)
			{
				return;
			}
			PlayerModel player = twdModelManager.Player;
			if (player.EndlessModeManager.CurrentEndlessModeCalendarDefinition != null && player.EndlessModeManager.CurrentEndlessModeCalendarDefinition.EndTimeMilliseconds >= player.UtcTimeStamp)
			{
				for (int i = 0; i < entries; i++)
				{
					int num = entries - i;
					LeaderboardEntry leaderboardEntry = new LeaderboardEntry();
					leaderboardEntry.Id = "PlayerId_" + num;
					leaderboardEntry.Tags = null;
					leaderboardEntry.ScoreAt = player.UtcTimeStamp;
					Leaderboards.EndlessModeLeaderBoardDetails endlessModeLeaderBoardDetails = new Leaderboards.EndlessModeLeaderBoardDetails();
					endlessModeLeaderBoardDetails.Name = "Player " + num;
					endlessModeLeaderBoardDetails.Level = 99;
					leaderboardEntry.Score = (i + 1) * 500;
					leaderboardEntry.Details = player.manager.GetMessageSerializer().SerializeObject(endlessModeLeaderBoardDetails);
					twdModelManager.ServerService.SaveLeaderboardEntry(player.EndlessModeManager.CurrentLeaderBoardName, leaderboardEntry);
				}
			}
		}

		private void RefreshBlackMarketHero(TWDModelManager twdModelManager)
		{
			string activeActorDefinitionID = twdModelManager.Player.BlackMarket.Slots[AuxInt].ActiveActorDefinitionID;
			twdModelManager.Player.BlackMarket.RefreshHero(activeActorDefinitionID, forceRefresh: true);
		}

		private void SetTeamOnFire(TWDModelManager twdModelManager)
		{
			foreach (ActorModel model in twdModelManager.CombatModel.Survivors.Models)
			{
				model.AddTemporaryTrait("Burning", default(FixedPoint), null, 0L);
			}
		}

		private void SetTeamBleeding(TWDModelManager twdModelManager)
		{
			foreach (ActorModel model in twdModelManager.CombatModel.Survivors.Models)
			{
				model.AddTemporaryTrait("Bleeding", default(FixedPoint), null, 0L);
			}
		}

		private void HealAllSurvivorsToMaxHealth(TWDModelManager twdModelManager)
		{
			foreach (ActorModel model in twdModelManager.Player.Combat.Survivors.Models)
			{
				model.StrugglesLeft = 1;
				model.OnRedHealthBar = true;
				model.SetHitpoints(model.MaxHitPoints);
				model.NotifyChange("ActorHealthChanged");
			}
		}

		private void InjureAllSurvivorsToMinGreenHealth(TWDModelManager twdModelManager)
		{
			foreach (ActorModel model in twdModelManager.Player.Combat.Survivors.Models)
			{
				model.StrugglesLeft = 1;
				model.OnRedHealthBar = true;
				model.DealDamage(model.Hitpoints - 1, null, DamageType.Melee);
				model.NotifyChange("ActorHealthChanged");
			}
		}

		private void InjureAllWalkersToMinHp(TWDModelManager twdModelManager)
		{
			foreach (WalkerModel walker in twdModelManager.Player.Combat.Walkers)
			{
				walker.DealDamage(walker.Hitpoints - 1, null, DamageType.Melee);
				walker.NotifyChange("ActorHealthChanged");
			}
		}

		private void InjureAlCivilianAndRaiderToMinHp(TWDModelManager twdModelManager)
		{
			CombatModel combat = twdModelManager.Player.Combat;
			foreach (RaiderModel raider in combat.Raiders)
			{
				raider.DealDamage(raider.Hitpoints - 1, null, DamageType.Melee);
				raider.NotifyChange("ActorHealthChanged");
			}
			foreach (CivilianModel civilian in combat.Civilians)
			{
				civilian.DealDamage(civilian.Hitpoints - 1, null, DamageType.Melee);
				civilian.NotifyChange("ActorHealthChanged");
			}
		}

		private void AddCampaignTokens(TWDModelManager twdModelManager)
		{
			twdModelManager.Player.GetCurrency(CurrencyType.CampaignToken)?.Add(CommandParameter);
		}

		private void DoSkipToSurvivalMapX(TWDModelManager twdModelManager, int index)
		{
			WeeklySurvivalModel weeklySurvival = twdModelManager.Player.WeeklySurvival;
			if (weeklySurvival == null)
			{
				return;
			}
			MapMissionGroupModel mapMissionGroupModel = weeklySurvival.GetMapMissionGroupModel();
			int num = 0;
			if (mapMissionGroupModel != null)
			{
				num = ((mapMissionGroupModel.Missions != null) ? mapMissionGroupModel.Missions.Count : 0);
			}
			if (index < num)
			{
				weeklySurvival.ResetCurrentForDifficulty((weeklySurvival.CurrentDifficulty == SurvivalDifficulty.None) ? SurvivalDifficulty.Normal : weeklySurvival.CurrentDifficulty);
				for (int i = 0; i < index; i++)
				{
					DoCompleteSurvivalMission(twdModelManager);
				}
			}
		}

		private void DoSurvivalModeOutOfAction(TWDModelManager mgr)
		{
			bool flag = true;
			foreach (SurvivalCharacterStateModel survivalModeState in mgr.Player.SurvivorContainer.SurvivalCharacters.SurvivalModeStates)
			{
				if (flag)
				{
					flag = false;
					continue;
				}
				survivalModeState.OutOfAction = true;
				survivalModeState.HealthPercentage = 0L;
			}
		}

		private void EquipOneBadgePerSurvivor(TWDModelManager manager)
		{
			ModelList<SurvivorModel> survivors = manager.Player.SurvivorContainer.Survivors;
			List<BadgeModel> list = new List<BadgeModel>(manager.Player.Equipment.Badges);
			for (int i = 0; i < (survivors?.Count ?? 0) && i < (list?.Count ?? 0); i++)
			{
				survivors[i].EquipBadge(list[i]);
			}
		}

		private void CreateRandomBadges(TWDModelManager manager)
		{
			List<CurrencyType> baseComponents = new List<CurrencyType>
			{
				CurrencyType.Badge0,
				CurrencyType.Metal0,
				CurrencyType.Cloth0,
				CurrencyType.Chemicals0,
				CurrencyType.Food0
			};
			int[] rarities = new int[5] { 0, 1, 2, 3, 4 };
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 6; j++)
				{
					string analyticsId = Guid.NewGuid().ToString();
					manager.Player.LootManager.CraftBadge(CreateListOfCurrencies(manager.Player.PlayerRandom, baseComponents, rarities), analyticsId);
				}
			}
		}

		private List<CurrencyType> CreateListOfCurrencies(ModelRandom random, List<CurrencyType> baseComponents, int[] rarities)
		{
			List<CurrencyType> list = new List<CurrencyType>();
			list.Add(ComponentHelper.GetCurrencyFromBaseAndRarity(CurrencyType.Badge0, rarities[random.Next(5)]));
			for (int i = 0; i < 4; i++)
			{
				int randomInRange = random.GetRandomInRange(1, 4);
				int num = random.Next(5);
				list.Add(ComponentHelper.GetCurrencyFromBaseAndRarity(baseComponents[randomInRange], rarities[num]));
			}
			return list;
		}

		private void DoConsumeComponents(TWDModelManager manager)
		{
			List<CurrencyType> allComponentCurrencies = ComponentHelper.GetAllComponentCurrencies();
			for (int i = 0; i < allComponentCurrencies.Count; i++)
			{
				manager.Player.GetCurrency(allComponentCurrencies[i])?.SetValue(0);
			}
		}

		private void DoGiveComponents(TWDModelManager manager)
		{
			List<CurrencyType> allComponentCurrencies = ComponentHelper.GetAllComponentCurrencies();
			for (int i = 0; i < allComponentCurrencies.Count; i++)
			{
				manager.Player.GetCurrency(allComponentCurrencies[i])?.Add(10);
			}
		}

		private void ResetSeasonCooldowns(TWDModelManager twdModelManager)
		{
			for (int i = 0; i < twdModelManager.Player.MapContainerModel.MapMissionGroups.Count; i++)
			{
				MapMissionGroupModel mapMissionGroupModel = twdModelManager.Player.MapContainerModel.MapMissionGroups[i];
				if (mapMissionGroupModel.MissionSpawnPointGroup == null || mapMissionGroupModel.MissionSpawnPointGroup.Category != MapCategory.Season)
				{
					continue;
				}
				for (int j = 0; j < mapMissionGroupModel.Missions.Count; j++)
				{
					MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[j];
					if (mapMissionModel.State == MapMissionState.Respawning)
					{
						mapMissionModel.RespawnTimer = 1;
					}
				}
			}
		}

		private void DoRefreshTrafeShop(TWDModelManager twdModelManager)
		{
			twdModelManager.Player.RefreshTradeSlotsAndItems();
		}

		private void DoAddInfluence(TWDModelManager twdModelManager, int amount)
		{
			twdModelManager.Player.SetRankingScore(twdModelManager.Player.RankingScore + amount);
		}

		private void DoAdvanceGame(TWDModelManager twdModelManager, int councilLevel, bool useForOutpost = false)
		{
			DoUnlockAll(twdModelManager);
			PlayerModel player = twdModelManager.Player;
			CampModel camp = player.Camp;
			SurvivorContainerModel survivorContainer = player.SurvivorContainer;
			GameEconomyData gameEconomyData = twdModelManager.GameEconomyData;
			DoGiveFullCurrencies(twdModelManager);
			while (survivorContainer.BuyNextSetOfSurvivorSlots() == TWDModelResult.OK)
			{
			}
			foreach (VegetationModel model in twdModelManager.GetModels<VegetationModel>())
			{
				model.CutInstant();
			}
			camp.Tick(0L);
			BuildingModel building = player.Camp.GetBuilding("Council");
			List<BuildingModel> list = new List<BuildingModel>();
			int num = 100;
			for (int i = 0; i < num; i++)
			{
				bool flag = false;
				if (building.Level < councilLevel && building.CanUpgrade)
				{
					building.UpgradeInstant();
					flag = true;
					foreach (VegetationModel model2 in twdModelManager.GetModels<VegetationModel>())
					{
						model2.CutInstant();
					}
					camp.Tick(0L);
				}
				BuildingsAmountsDefinition buildingsAmountsAtCouncilLevel = gameEconomyData.GetBuildingsAmountsAtCouncilLevel(building.Level);
				if (buildingsAmountsAtCouncilLevel != null)
				{
					BuildingType[] buildingTypes = gameEconomyData.BuildingTypes;
					foreach (BuildingType buildingType in buildingTypes)
					{
						if (buildingType.Category == BuildingCategory.BuffBuilding || buildingType.Category == BuildingCategory.Vegetation || gameEconomyData.GetBuildingUpgradeLevel(buildingType.Name, 1) == null)
						{
							continue;
						}
						int amountsForBuilding = buildingsAmountsAtCouncilLevel.GetAmountsForBuilding(buildingType.Name);
						for (int k = camp.GetBuildingCount(buildingType.Name); k < amountsForBuilding; k++)
						{
							GridSize size = new GridSize((int)Math.Ceiling((float)gameEconomyData.ScaleToGrid(buildingType.Size.X) * 0.5f) * 2, (int)Math.Ceiling((float)gameEconomyData.ScaleToGrid(buildingType.Size.Y) * 0.5f) * 2);
							GridPosition initialPosition = new GridPosition(camp.GridWidth / 2, camp.GridHeight / 2);
							GridPosition gridPosition = camp.GetFreePositionToPlaceBuilding(initialPosition, size);
							if (buildingType.Name == "Cage")
							{
								FixedVec2 fixedVec = player.Camp.TransformGroundToGridPosition(new FixedVec2(8L, -1L));
								gridPosition = new GridPosition(fixedVec.X, fixedVec.Y);
							}
							if (gridPosition != null)
							{
								BuildingModel outNewBuilding = null;
								camp.CreateNewBuilding(buildingType.Name, gridPosition, ref outNewBuilding);
								flag = true;
								if (outNewBuilding != null)
								{
									outNewBuilding.SpeedUpUpgrade();
									list.Add(outNewBuilding);
								}
							}
						}
					}
				}
				if (building.Level == councilLevel)
				{
					break;
				}
				foreach (BuildingModel building3 in camp.Buildings)
				{
					while (building3.CanUpgrade && building3 != building && building3.UpgradeInstant() == TWDModelResult.OK)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					break;
				}
			}
			BuildingModel building2 = camp.GetBuilding("Outpost");
			if (building2 != null && building2.Level > 0)
			{
				twdModelManager.Player.Blackboard.SetToggle("Toggle.ToggleOutpostUnlockedSeen");
				twdModelManager.Player.Blackboard.SetToggle("Toggle.ToggleOutpostEditUnlockedSeen");
				twdModelManager.Player.OutpostTutorialState = OutpostTutorialState.Done;
				twdModelManager.Player.GiveExtraOutpostSurvivorsAndSlots();
				twdModelManager.Player.Name = Environment.UserName;
				OutpostLevelModel outpostLevelModel = new OutpostLevelModel();
				outpostLevelModel.SetManager(twdModelManager);
				OutpostTemplateDefinition outpostTemplateDefinition = twdModelManager.GameEconomyData.OutpostTemplateDefinitions[0];
				if (outpostTemplateDefinition != null)
				{
					twdModelManager.Player.SetSelectedOutpostTemplateDefinitionId(outpostTemplateDefinition.Id);
					outpostLevelModel.BaseRunLocationID = outpostTemplateDefinition.MissionID;
				}
				twdModelManager.Player.OutpostModel.StoredLevelModel = outpostLevelModel;
				RunLocationModel outpostTemplate = twdModelManager.Player.GetOutpostTemplate(outpostTemplateDefinition.MissionID);
				if (outpostTemplate != null)
				{
					twdModelManager.Player.OutpostModel.StoredLevelModel.SetSlice(SlicePosition.First, outpostTemplate.GetSliceViewIds(SlicePosition.First)[0]);
					twdModelManager.Player.OutpostModel.StoredLevelModel.SetSlice(SlicePosition.Second, outpostTemplate.GetSliceViewIds(SlicePosition.Second)[0]);
					twdModelManager.Player.OutpostModel.StoredLevelModel.SetSlice(SlicePosition.Third, outpostTemplate.GetSliceViewIds(SlicePosition.Third)[0]);
				}
				List<KeyValuePair<string, OutpostHotspotModel>> list2 = new List<KeyValuePair<string, OutpostHotspotModel>>();
				for (int l = 0; l < twdModelManager.Player.OutpostModel.StoredLevelModel.ChosenSlices.Count; l++)
				{
					OutpostSliceModel sliceModel = outpostTemplate.GetSliceModel(twdModelManager.Player.OutpostModel.StoredLevelModel.ChosenSlices[l].ViewId);
					foreach (TWDModelObject model3 in sliceModel.Models)
					{
						if (model3 is OutpostHotspotModel value)
						{
							list2.Add(new KeyValuePair<string, OutpostHotspotModel>(sliceModel.ViewId, value));
						}
					}
				}
				int num2 = 0;
				int num3 = 0;
				bool flag2 = false;
				bool flag3 = false;
				ModelRandom random = new ModelRandom((int)twdModelManager.Player.LifeTime);
				UtilsArray.ShuffleList(list2, random);
				for (int m = 0; m < list2.Count; m++)
				{
					string key = list2[m].Key;
					OutpostHotspotModel value2 = list2[m].Value;
					if (twdModelManager.Player.OutpostModel.StoredLevelModel.FindHotspotInfo(value2.ViewId) == null)
					{
						if (value2.Type == HotspotType.Goal && !flag2)
						{
							twdModelManager.Player.OutpostModel.StoredLevelModel.SetHotspotInfo(key, value2.ViewId, HotspotState.Flag, WalkerType.WalkerNormal, 1, AIMode.None);
							flag2 = true;
						}
						else if (value2.Type == HotspotType.Goal && !flag3)
						{
							twdModelManager.Player.OutpostModel.StoredLevelModel.SetHotspotInfo(key, value2.ViewId, HotspotState.ResourceContainer, WalkerType.WalkerNormal, 1, AIMode.None);
							flag3 = true;
						}
						else if (value2.CanAssignDefender && num2 < 3)
						{
							twdModelManager.Player.OutpostModel.StoredLevelModel.SetHotspotInfo(key, value2.ViewId, (HotspotState)(num2 + 2), WalkerType.WalkerNormal, 1, AIMode.Aggressive);
							num2++;
						}
						else if (value2.CanAssignWalker && num3 < 2)
						{
							twdModelManager.Player.OutpostModel.StoredLevelModel.SetHotspotInfo(key, value2.ViewId, HotspotState.Walker, WalkerType.WalkerArmored, 1, AIMode.Aggressive);
							num3++;
						}
					}
				}
				twdModelManager.Player.OutpostModel.OutpostRunLocation = twdModelManager.Player.OutpostModel.StoredLevelModel.GenerateOutpost(outpostTemplate);
				twdModelManager.Player.OutpostModel.PublishedLevelDataVersion = twdModelManager.GameEconomyData.ConfigData.OutpostLevelDataVersion;
				if (twdModelManager.Player.OutpostModel.OutpostRunLocation != null)
				{
					if (player.CurrentOutpostSeasonId == -1)
					{
						player.UpdateOutpostSeason();
					}
					twdModelManager.UpdateOutpostLeaderboardEntry();
				}
			}
			if (useForOutpost)
			{
				int[] array = new int[3] { 4, 5, 6 };
				for (int n = 0; n < 3; n++)
				{
					int startingLevel = array[n];
					for (int num4 = 0; num4 < 10; num4++)
					{
						EquipmentItemModel equipmentItemModel = twdModelManager.Player.Equipment.GenerateRandomEquipment(EquipmentCategory.None, startingLevel);
						if (equipmentItemModel != null)
						{
							twdModelManager.Player.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Debug);
						}
					}
				}
			}
			foreach (SurvivorModel survivor in survivorContainer.Survivors)
			{
				while (survivor.CanUpgrade && survivor.UpgradeInstant() == TWDModelResult.OK)
				{
				}
			}
			foreach (EquipmentItemModel meleeWeapon in player.Equipment.MeleeWeapons)
			{
				while (meleeWeapon.CanUpgrade && meleeWeapon.UpgradeInstant() == TWDModelResult.OK)
				{
				}
			}
			foreach (EquipmentItemModel rangeWeapon in player.Equipment.RangeWeapons)
			{
				while (rangeWeapon.CanUpgrade && rangeWeapon.UpgradeInstant() == TWDModelResult.OK)
				{
				}
			}
			foreach (EquipmentItemModel armor in player.Equipment.Armors)
			{
				while (armor.CanUpgrade && armor.UpgradeInstant() == TWDModelResult.OK)
				{
				}
			}
			if (player.OutpostModel != null)
			{
				foreach (OutpostWalkerModel walkerModel in player.OutpostModel.WalkerModels)
				{
					walkerModel.Unlock();
					while (walkerModel.CanUpgradeAmount && walkerModel.UpgradeAmount(0) == TWDModelResult.OK)
					{
					}
					while (walkerModel.CanUpgrade && walkerModel.UpgradeInstant() == TWDModelResult.OK)
					{
					}
				}
			}
			if (useForOutpost)
			{
				twdModelManager.Player.GetCurrency(CurrencyType.Diamonds).SetValue(1000);
				twdModelManager.Player.GetCurrency(CurrencyType.Phone).SetValue(100);
			}
			foreach (VegetationModel model4 in twdModelManager.GetModels<VegetationModel>())
			{
				model4.CutInstant();
			}
			camp.Tick(0L);
			DoGiveComponents(twdModelManager);
			player?.BundleManager?.ResetNewBundlesCheckTimer();
			if (gameEconomyData.BattlePassConfig.CouncilLockLevel <= councilLevel)
			{
				player.BeginnerBattlePassInfo.State = BeginnerBattlePassState.Skipped;
			}
		}

		private void DoCompleteTutorial(TWDModelManager manager)
		{
			manager.Player.SurvivorContainer.StoryTeller?.AcceptQuest();
			manager.Player.Tutorial.SetAllPartsCompleted();
		}

		private void DoCompleteCombatTutorial(TWDModelManager manager)
		{
			manager.Player.Tutorial.SetPartCompleted("InitialCombat");
		}

		private void DoWinCombat(TWDModelManager manager)
		{
			manager.CombatModel?.ForceEndMissionVictory();
		}

		private void DoLoseCombat(TWDModelManager manager)
		{
			manager.CombatModel?.ForceEndMissionFailure();
		}

		private void DoGiveFullCurrencies(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.Supplies).SetValue(manager.Player.GetCurrency(CurrencyType.Supplies).Max);
			manager.Player.GetCurrency(CurrencyType.SurvivalPoints).SetValue(manager.Player.GetCurrency(CurrencyType.SurvivalPoints).Max);
			manager.Player.GetCurrency(CurrencyType.Diamonds).SetValue(manager.Player.GetCurrency(CurrencyType.Diamonds).Max);
			manager.Player.GetCurrency(CurrencyType.Inhabitants).SetValue(manager.Player.GetCurrency(CurrencyType.Inhabitants).Max);
			manager.Player.GetCurrency(CurrencyType.Phone).SetValue(manager.Player.GetCurrency(CurrencyType.Phone).Max);
			manager.Player.GetCurrency(CurrencyType.ReplayToken).SetValue(manager.Player.GetCurrency(CurrencyType.ReplayToken).Max);
			manager.Player.GetCurrency(CurrencyType.Outpost).SetValue(20000);
			manager.Player.GetCurrency(CurrencyType.GvGGas).SetValue(manager.Player.GetCurrency(CurrencyType.GvGGas).Max);
			manager.Player.GetCurrency(CurrencyType.GuildBattleRP).SetValue(manager.Player.GetCurrency(CurrencyType.GuildBattleRP).Max);
			manager.Player.GetCurrency(CurrencyType.BattlePass).SetValue(manager.Player.GetCurrency(CurrencyType.BattlePass).Max);
			manager.Player.GetCurrency(CurrencyType.EquipmentUpgradeToken).SetValue(2);
			manager.Player.GetCurrency(CurrencyType.TraitRerollToken).SetValue(100);
			manager.Player.GetCurrency(CurrencyType.BlackMarketToken).SetValue(999999);
			manager.Player.GetCurrency(CurrencyType.BuildingTokenBP).SetValue(manager.Player.GetCurrency(CurrencyType.BuildingTokenBP).Max);
			manager.Player.GetCurrency(CurrencyType.SuperBuildingTokenBP).SetValue(manager.Player.GetCurrency(CurrencyType.SuperBuildingTokenBP).Max);
			manager.Player.GetCurrency(CurrencyType.TrainingTokenBP).SetValue(manager.Player.GetCurrency(CurrencyType.TrainingTokenBP).Max);
			manager.Player.GetCurrency(CurrencyType.SuperTrainingTokenBP).SetValue(manager.Player.GetCurrency(CurrencyType.SuperTrainingTokenBP).Max);
			manager.Player.GetCurrency(CurrencyType.EquipmentTokenBP).SetValue(manager.Player.GetCurrency(CurrencyType.EquipmentTokenBP).Max);
			manager.Player.GetCurrency(CurrencyType.SuperEquipmentTokenBP).SetValue(manager.Player.GetCurrency(CurrencyType.SuperEquipmentTokenBP).Max);
			manager.Player.GetCurrency(CurrencyType.HealingTokenBP).SetValue(manager.Player.GetCurrency(CurrencyType.HealingTokenBP).Max);
			manager.Player.GetCurrency(CurrencyType.Fairmoney).SetValue(manager.Player.GetCurrency(CurrencyType.Fairmoney).Max);
			manager.Player.GetCurrency(CurrencyType.HillTopCoin).SetValue(manager.Player.GetCurrency(CurrencyType.HillTopCoin).Max);
			manager.Player.GetCurrency(CurrencyType.ShivaToken).SetValue(manager.Player.GetCurrency(CurrencyType.ShivaToken).Max);
			manager.Player.GetCurrency(CurrencyType.PrimarySupportTalentToken).SetValue(manager.Player.GetCurrency(CurrencyType.PrimarySupportTalentToken).Max);
			manager.Player.GetCurrency(CurrencyType.AdvancedSupportTalentToken).SetValue(manager.Player.GetCurrency(CurrencyType.AdvancedSupportTalentToken).Max);
		}

		private void DoClearFullCurrencies(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.Supplies).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.SurvivalPoints).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.Diamonds).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.Inhabitants).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.Phone).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.ReplayToken).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.Outpost).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.GvGGas).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.GuildBattleRP).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.BattlePass).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.EquipmentUpgradeToken).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.TraitRerollToken).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.BlackMarketToken).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.BuildingTokenBP).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.SuperBuildingTokenBP).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.TrainingTokenBP).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.SuperTrainingTokenBP).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.EquipmentTokenBP).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.SuperEquipmentTokenBP).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.HealingTokenBP).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.Fairmoney).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.HillTopCoin).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.ShivaToken).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.PrimarySupportTalentToken).SetValue(0);
			manager.Player.GetCurrency(CurrencyType.AdvancedSupportTalentToken).SetValue(0);
		}

		private void DoGiveEquipment(TWDModelManager manager, string definition, int rarityLevel = -1, int itemLevel = -1, int amount = 1)
		{
			for (int i = 0; i < amount; i++)
			{
				int rarityLevel2 = ((rarityLevel < 0) ? 5 : Math.Min(rarityLevel, 5));
				int num = manager.Player.Camp.GetTrainingGroundLevel() + 1;
				int startingLevel = ((itemLevel <= 0) ? num : Math.Min(itemLevel, num));
				EquipmentItemModel equipmentItemModel = manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(definition, rarityLevel2, startingLevel);
				if (equipmentItemModel != null)
				{
					manager.Player.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Debug);
					if (CommandParameter1 < 1 || CommandParameter1 > 6)
					{
						continue;
					}
					for (int j = 0; j < 3; j++)
					{
						equipmentItemModel.UpgradeInstant();
					}
					if (equipmentItemModel.EquipmentBreakthrough == null)
					{
						equipmentItemModel.EquipmentBreakthrough = new EquipmentBreakthroughModel();
						equipmentItemModel.EquipmentBreakthrough.SetLevel(CommandParameter1);
						equipmentItemModel.EquipmentBreakthrough.SetManager(manager);
						equipmentItemModel.EquipmentBreakthrough.Start();
					}
					List<string> equipmentPassiveTraits = equipmentItemModel.GetEquipmentPassiveTraits();
					EquipBreakthroughDefinition equipBreakthroughDefinitionByRarityAndLevel = manager.GameEconomyData.GetEquipBreakthroughDefinitionByRarityAndLevel(equipmentItemModel.RarityLevel, equipmentItemModel.EquipmentBreakthrough.Level);
					int[] overrideTraitsLevels = new int[4] { equipBreakthroughDefinitionByRarityAndLevel.Traits1QualityLevel, equipBreakthroughDefinitionByRarityAndLevel.Traits2QualityLevel, equipBreakthroughDefinitionByRarityAndLevel.Traits3QualityLevel, equipBreakthroughDefinitionByRarityAndLevel.Traits4QualityLevel };
					if (!equipmentItemModel.UpgradeTraitsLevelByBreakthrough(overrideTraitsLevels))
					{
						break;
					}
					if (equipmentItemModel.Owner != null)
					{
						for (int k = 0; k < equipmentPassiveTraits.Count; k++)
						{
							string traitIdentifier = equipmentPassiveTraits[k];
							if (manager.GameEconomyData.GetTraitDefinition(traitIdentifier).IsApocalypticTrait)
							{
								equipmentItemModel.Owner.RemoveTrait(traitIdentifier);
								string traitIdentifier2 = UpgradeTraitsData.CompileUpgradeTraitIdentifier(UpgradeTraitsData.StripTraitLevelIdentifier(traitIdentifier), equipBreakthroughDefinitionByRarityAndLevel.ApocalypticTraitLevel, isLocked: false);
								equipmentItemModel.Owner.AddTrait(traitIdentifier2);
							}
						}
					}
					equipmentItemModel.RefreshModifiers();
				}
				else
				{
					manager.Debug.LogError("Debug DoGiveSpecificWeapon, equipment not found " + definition);
				}
			}
		}

		private void DoSpawnWalker(TWDModelManager manager)
		{
			manager.CampModel.CampDefenseModel?.CreateWalker();
		}

		private void DoGive100RewardPoints(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.GuildBattleRP).SetValue(100);
		}

		private void RestockGuildShop(TWDModelManager manager, int parameter = 3)
		{
			switch (parameter)
			{
			case 1:
				manager.Player.GuildShopModel.RestockGuildShopItems(onNewTier: true, onNewWar: false);
				break;
			case 2:
				manager.Player.GuildShopModel.RestockGuildShopItems(onNewTier: false, onNewWar: true);
				break;
			default:
				manager.Player.GuildShopModel.RestockGuildShopItems(onNewTier: true, onNewWar: true);
				break;
			}
		}

		private void DoGiveFullGvGGas(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.GvGGas).SetValue(manager.Player.GetCurrency(CurrencyType.GvGGas).Max);
		}

		private void DoGiveFullGvGBattleKeys(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.GvGMissionKey).SetValue(manager.Player.GetCurrency(CurrencyType.GvGMissionKey).Max);
		}

		private void DoGiveFullGas(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.ReplayToken).SetValue(manager.Player.GetCurrency(CurrencyType.ReplayToken).Max);
		}

		private void DoConsumeAllGas(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.ReplayToken).SetValue(0);
		}

		private void DoConsumeAllGvGGas(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.GvGGas).SetValue(0);
		}

		private void DoConsumeAllBattlePasses(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.BattlePass).SetValue(0);
		}

		private void DoConsumeAllSupplies(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.Supplies).SetValue(0);
		}

		private void DoConsumeAllSurvivalPoints(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.SurvivalPoints).SetValue(0);
		}

		private void DoConsumeAllTradeGoods(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.Outpost).SetValue(0);
		}

		private void DoConsumeAllGold(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.Diamonds).SetValue(0);
		}

		private void DoConsumeAllEquipmentUpgrateTokens(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.EquipmentUpgradeToken).SetValue(0);
		}

		private void GiveEquipmentUpgradeTokens(TWDModelManager manager, int newAmount)
		{
			int value = manager.Player.GetCurrency(CurrencyType.EquipmentUpgradeToken).Value;
			manager.Player.GetCurrency(CurrencyType.EquipmentUpgradeToken).SetValue(value + newAmount);
		}

		private void GiveTraitRerollToken(TWDModelManager manager, int amount)
		{
			manager.Player.GetCurrency(CurrencyType.TraitRerollToken).Add(amount);
		}

		private void DoConsumeAllTraitRerollTokens(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.TraitRerollToken).SetValue(0);
		}

		private void DoGive15Equipments(TWDModelManager manager, int tier, int rarityLevel)
		{
			EquipmentCategory[] array = new EquipmentCategory[3]
			{
				EquipmentCategory.MeleeWeapon,
				EquipmentCategory.RangeWeapon,
				EquipmentCategory.Armor
			};
			for (int i = 0; i < 3; i++)
			{
				EquipmentCategory category = array[i];
				for (int j = 0; j < 5; j++)
				{
					EquipmentItemModel equipmentItemModel = manager.Player.Equipment.GenerateRandomEquipment(category, tier, rarityLevel);
					if (equipmentItemModel != null)
					{
						manager.Player.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Debug);
					}
				}
			}
		}

		private void DoInjuryCurrentTeam(TWDModelManager manager, InjuryType injuryType)
		{
			if (injuryType == InjuryType.None || ((PlayerModel)manager.GetPlayer()).Camp == null || !(((PlayerModel)manager.GetPlayer()).Camp.GetBuilding("MedicTent") is MedicTentModel medicTentModel) || manager.Player.SurvivorContainer == null || manager.Player.SurvivorContainer.CombatSurvivors == null)
			{
				return;
			}
			GameEconomyData gameEconomyData = manager.GameEconomyData;
			foreach (SurvivorModel combatSurvivor in manager.Player.SurvivorContainer.CombatSurvivors)
			{
				int num = 0;
				combatSurvivor.StrugglesLeft = 1;
				FixedPoint fixedPoint = injuryType switch
				{
					InjuryType.Critical => new FixedPoint((float)gameEconomyData.ConfigData.InjuryCriticalBelowHealthPercentage / 100f), 
					InjuryType.Major => new FixedPoint((float)gameEconomyData.ConfigData.InjuryMajorBelowHealthPercentage / 100f), 
					InjuryType.Minor => new FixedPoint((float)gameEconomyData.ConfigData.InjuryMinorBelowHealthPercentage / 100f), 
					_ => new FixedPoint(1.0), 
				};
				num = combatSurvivor.MaxHitPoints + combatSurvivor.MaxHitPoints * combatSurvivor.StrugglesLeft - (int)new FixedPoint((combatSurvivor.MaxHitPoints + combatSurvivor.MaxHitPoints * combatSurvivor.StrugglesLeft) * fixedPoint);
				if (num >= combatSurvivor.Hitpoints && combatSurvivor.StrugglesLeft > 0)
				{
					combatSurvivor.StrugglesLeft = 0;
					num -= combatSurvivor.MaxHitPoints;
				}
				combatSurvivor.SetHitpoints(combatSurvivor.MaxHitPoints - num - 1);
				combatSurvivor.MinHitpoints = combatSurvivor.Hitpoints;
				combatSurvivor.InjuryType = injuryType;
				if (manager.Player.Combat == null)
				{
					FixedPoint healingTimeModifier = 1.0;
					int missionLevel = combatSurvivor.Level * 3;
					medicTentModel.NewSurvivorInjured(combatSurvivor, missionLevel, healingTimeModifier);
				}
			}
		}

		private void DoPassTime(TWDModelManager manager, int minutes)
		{
			long deltaTime = minutes * 1000;
			manager.TickModel(deltaTime);
		}

		private void DoUnlockAll(TWDModelManager manager)
		{
			MapContainerModel mapContainerModel = manager.Player.MapContainerModel;
			foreach (MapMissionGroupModel mapMissionGroup in mapContainerModel.MapMissionGroups)
			{
				if (mapMissionGroup.IsDisabledOnGED || mapMissionGroup.MissionSpawnPointGroup == null || mapMissionGroup.MissionSpawnPointGroup.Category == MapCategory.Season || mapMissionGroup.MissionSpawnPointGroup.Category == MapCategory.Survival || mapMissionGroup.MissionSpawnPointGroup.Category == MapCategory.GuildBattle)
				{
					continue;
				}
				mapContainerModel.SpawnMissionsForGroup(mapMissionGroup.MissionSpawnPointGroup);
				foreach (MapMissionModel mission in mapMissionGroup.Missions)
				{
					if (!mission.IsGrindMission)
					{
						mapContainerModel.CompleteMission(mission);
						mission.State = MapMissionState.Unlocked;
						mission.RespawnTimer = 0;
					}
				}
			}
			foreach (SurvivorClass value in Enum.GetValues(typeof(SurvivorClass)))
			{
				manager.Player.SurvivorContainer.UnlockSurvivorClass(value);
			}
		}

		private void DoCompleteNextMission(TWDModelManager manager, int spawnGroupId)
		{
			MapContainerModel mapContainerModel = manager.Player.MapContainerModel;
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = mapContainerModel.GetMissionGroupModelForSpawnPointGroup(spawnGroupId);
			if (missionGroupModelForSpawnPointGroup == null)
			{
				return;
			}
			foreach (MapMissionModel mission in missionGroupModelForSpawnPointGroup.Missions)
			{
				if (!mission.IsGrindMission && !mission.IsCompleted)
				{
					mapContainerModel.CompleteMission(mission);
					break;
				}
			}
		}

		private void DoCompleteCurrentEpisode(TWDModelManager manager)
		{
			MapContainerModel mapContainerModel = manager.Player.MapContainerModel;
			if (!(manager.Player.SurvivorContainer.StoryTeller.CurrentQuest is MissionQuest { HasCompleted: false } missionQuest))
			{
				return;
			}
			MapMissionGroupModel unlockedEpisode = missionQuest.GetUnlockedEpisode();
			if (unlockedEpisode == null)
			{
				return;
			}
			mapContainerModel.SpawnMissionsForGroup(unlockedEpisode.MissionSpawnPointGroup);
			foreach (MapMissionModel mission in unlockedEpisode.Missions)
			{
				if (!mission.IsGrindMission)
				{
					mapContainerModel.CompleteMission(mission);
					mission.State = MapMissionState.Completed;
					mission.RespawnTimer = 0;
				}
			}
		}

		private void DoCompleteAllEpisodesAndGrinds(TWDModelManager manager)
		{
			MapContainerModel mapContainerModel = manager.Player.MapContainerModel;
			foreach (MissionSpawnPointGroup mapDefinition in manager.GameEconomyData.MapDefinitions)
			{
				if (mapDefinition == null || mapDefinition.Category != MapCategory.Story)
				{
					continue;
				}
				foreach (MissionSpawnPoint missionSpawnPoint in mapDefinition.MissionSpawnPoints)
				{
					MapMissionModel missionModelForSpawnPoint = mapContainerModel.GetMissionModelForSpawnPoint(missionSpawnPoint);
					if (missionModelForSpawnPoint != null)
					{
						mapContainerModel.CompleteMission(missionModelForSpawnPoint);
						missionModelForSpawnPoint.State = MapMissionState.Completed;
						missionModelForSpawnPoint.RespawnTimer = 0;
					}
				}
			}
		}

		private void DoCompleteChallengeRound(TWDModelManager manager)
		{
			MapMissionGroupModel currentOrNextMapMissionGroupModel = manager.Player.WeeklyChallenge.GetCurrentOrNextMapMissionGroupModel();
			if (currentOrNextMapMissionGroupModel == null)
			{
				return;
			}
			foreach (MapMissionModel mission in currentOrNextMapMissionGroupModel.Missions)
			{
				mission.Stars.Stars = new bool[3] { true, true, true };
				mission.Stars.TotalStars = 3;
				int amount = 3;
				if (!mission.IsMasterMission)
				{
					manager.Player.WeeklyChallenge.AddPersonalStars(amount);
					manager.Player.MissionStatistics.AddStars(amount);
				}
				mission.NotifyChange("StateChanged");
			}
		}

		private void DoCompleteSurvivalMission(TWDModelManager manager)
		{
			manager.Player.WeeklySurvival.AddPersonalCompletions(1);
			manager.Player.WeeklySurvival.MoveToNextMission();
			MapMissionGroupModel currentOrNextMapMissionGroupModel = manager.Player.WeeklySurvival.GetCurrentOrNextMapMissionGroupModel();
			if (currentOrNextMapMissionGroupModel == null)
			{
				return;
			}
			foreach (MapMissionModel mission in currentOrNextMapMissionGroupModel.Missions)
			{
				mission.UpdateSurvivalMapState();
				mission.NotifyChange("StateChanged");
			}
		}

		private void DoCompleteManySurvivalMissions(TWDModelManager manager)
		{
			for (int i = 0; i < 6; i++)
			{
				DoCompleteSurvivalMission(manager);
			}
		}

		private void DoResetSurvivalToDifficultySelection(TWDModelManager manager)
		{
			manager.Player.WeeklySurvival.CurrentMapRestarts = -1;
			manager.Player.WeeklySurvival.ResetCurrentToDifficultySelection();
		}

		private void DoToggleSurvivalDifficulty(TWDModelManager manager)
		{
			if (manager.Player.WeeklySurvival.CurrentDifficulty == SurvivalDifficulty.Hard)
			{
				manager.Player.WeeklySurvival.ResetCurrentForDifficulty(SurvivalDifficulty.Nightmare);
			}
			else if (manager.Player.WeeklySurvival.CurrentDifficulty == SurvivalDifficulty.Nightmare)
			{
				manager.Player.WeeklySurvival.ResetCurrentForDifficulty(SurvivalDifficulty.Normal);
			}
			else
			{
				manager.Player.WeeklySurvival.ResetCurrentForDifficulty(SurvivalDifficulty.Hard);
			}
		}

		private void DoResetSurvivalToUncompleted(TWDModelManager manager)
		{
			manager.Player.WeeklySurvival.ResetCurrentForDifficulty(manager.Player.WeeklySurvival.CurrentDifficulty);
		}

		private void DoRecreateWeeklySurvival(TWDModelManager manager)
		{
			manager.Player.WeeklySurvival.ResetForNewIdentifier(manager.Player.WeeklySurvival.CurrentDefinition.Identifier);
		}

		private void DoGiveFlipFlops(TWDModelManager manager)
		{
			EquipmentItemModel equipmentItemModel = manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition("Armor_Flipflops", 4, 1);
			if (equipmentItemModel != null)
			{
				manager.Player.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Debug);
				manager.Debug.Log("Generated item " + equipmentItemModel.Definition.ID + " rarityLevel " + equipmentItemModel.RarityLevel + " tier " + equipmentItemModel.Level + " damage " + equipmentItemModel.Damage);
			}
		}

		private void DoKillAllWalkers(TWDModelManager manager)
		{
			CombatModel combat = manager.Player.Combat;
			List<ActorModel> list = new List<ActorModel>();
			list.AddRange(combat.Walkers);
			list.AddRange(combat.Dormants);
			SurvivorModel attacker = combat.GetFactionActors(Faction.Survivor)[0] as SurvivorModel;
			for (int i = 0; i < list.Count; i++)
			{
				list[i].DealDamage(int.MaxValue, attacker, DamageType.Ranged);
			}
		}

		private void DoSetAllWalkersOnFire(TWDModelManager manager)
		{
			CombatModel combat = manager.Player.Combat;
			List<ActorModel> list = new List<ActorModel>();
			list.AddRange(combat.Walkers);
			list.AddRange(combat.Dormants);
			_ = combat.GetFactionActors(Faction.Survivor)[0];
			for (int i = 0; i < list.Count; i++)
			{
				list[i].AddTemporaryTrait("Burning", default(FixedPoint), null, 0L);
			}
		}

		private void DoSetAllWalkersBleeding(TWDModelManager manager)
		{
			CombatModel combat = manager.Player.Combat;
			List<ActorModel> list = new List<ActorModel>();
			list.AddRange(combat.Walkers);
			list.AddRange(combat.Dormants);
			_ = combat.GetFactionActors(Faction.Survivor)[0];
			for (int i = 0; i < list.Count; i++)
			{
				list[i].AddTemporaryTrait("Bleeding", default(FixedPoint), null, 0L);
			}
		}

		private void DoKillAllEnemies(TWDModelManager manager)
		{
			CombatModel combat = manager.Player.Combat;
			List<ActorModel> enemyFactionsActors = combat.GetEnemyFactionsActors(Faction.Survivor);
			SurvivorModel attacker = combat.GetFactionActors(Faction.Survivor)[0] as SurvivorModel;
			foreach (ActorModel item in enemyFactionsActors)
			{
				item.DealDamage(int.MaxValue, attacker, DamageType.Ranged);
			}
		}

		private void DoStunAllEnemies(TWDModelManager manager)
		{
			CombatModel combat = manager.Player.Combat;
			List<ActorModel> enemyFactionsActors = combat.GetEnemyFactionsActors(Faction.Survivor);
			SurvivorModel instigator = combat.GetFactionActors(Faction.Survivor)[0] as SurvivorModel;
			foreach (ActorModel item in enemyFactionsActors)
			{
				item.Stun(3, instigator);
			}
		}

		private void DoElectronShockAllEnemies(TWDModelManager manager)
		{
			CombatModel combat = manager.Player.Combat;
			List<ActorModel> enemyFactionsActors = combat.GetEnemyFactionsActors(Faction.Survivor);
			SurvivorModel instigator = combat.GetFactionActors(Faction.Survivor)[0] as SurvivorModel;
			foreach (ActorModel item in enemyFactionsActors)
			{
				item.StartElectricShock(3, instigator, 3);
			}
		}

		private void DoElectronShockLeaderSurvivor(TWDModelManager manager)
		{
			CombatModel combat = manager.Player.Combat;
			List<ActorModel> enemyFactionsActors = combat.GetEnemyFactionsActors(Faction.Survivor);
			SurvivorModel survivorModel = combat.GetFactionActors(Faction.Survivor)[0] as SurvivorModel;
			if (enemyFactionsActors.Count > 0)
			{
				ActorModel instigator = enemyFactionsActors[0];
				survivorModel?.StartElectricShock(3, instigator, 3);
			}
			else
			{
				survivorModel?.StartElectricShock(3, survivorModel, 3);
			}
		}

		private void DoRootLeaderSurvivor(TWDModelManager manager)
		{
			CombatModel combat = manager.Player.Combat;
			List<ActorModel> enemyFactionsActors = combat.GetEnemyFactionsActors(Faction.Survivor);
			SurvivorModel survivorModel = combat.GetFactionActors(Faction.Survivor)[0] as SurvivorModel;
			if (enemyFactionsActors.Count > 0)
			{
				ActorModel instigator = enemyFactionsActors[0];
				survivorModel?.Root(3, instigator);
			}
			else
			{
				survivorModel?.Root(3, survivorModel);
			}
		}

		private void DoSetCombatTimer(TWDModelManager manager, int timeInSeconds)
		{
			manager.Player.Combat.MaxTime = timeInSeconds;
		}

		private void DoAddDefenseLogEntries(TWDModelManager manager, int count)
		{
			for (int i = 0; i < count; i++)
			{
				manager.Player.OutpostDefenseLogDebug();
			}
		}

		private void DoAddAttackLogEntries(TWDModelManager manager, int count)
		{
			for (int i = 0; i < count; i++)
			{
				manager.Player.OutpostAttackLogDebug();
			}
		}

		private void DoSetOutpostRepairTimer(TWDModelManager manager, int timeInSeconds)
		{
			manager.Player.Camp.GetBuilding("Outpost").Producer.ProductionHaltedTimer = timeInSeconds * 1000;
		}

		private void GiveLateGameTeam(TWDModelManager manager)
		{
			List<SurvivorClass> classes = new List<SurvivorClass>
			{
				SurvivorClass.Assault,
				SurvivorClass.Bruiser,
				SurvivorClass.Hunter,
				SurvivorClass.Scout,
				SurvivorClass.Shooter,
				SurvivorClass.Warrior
			};
			GiveTeam(manager, classes, 25, 25, 6, 4);
			GiveTeam(manager, classes, 21, 21, 4, 4);
			GiveTeam(manager, classes, 17, 17, 4, 4);
			DoGiveFullHeroTokens(manager, unlockHero: true);
			DoGiveFullSupportTokens(manager);
			DoUnlockAllSupports(manager.Player);
			foreach (SurvivorModel item in manager.Player.SurvivorContainer.Survivors.Where((SurvivorModel x) => x.IsHero))
			{
				GiveMaxedOutItem(manager, item, EquipmentCategory.Weapon, 4);
				GiveMaxedOutItem(manager, item, EquipmentCategory.Armor, 4);
				while (item.SurvivorRarityLevel < 4)
				{
					bool flag = false;
					if (!((!item.CanUpgradeSurvivorRarity()) ? item.UpgradeLowestLevelTrait() : item.UpgradeSurvivorRarity()))
					{
						break;
					}
				}
			}
			manager.Player?.BundleManager?.ResetNewBundlesCheckTimer();
		}

		private void DoGiveChallengeDebugTeam(TWDModelManager manager)
		{
			List<SurvivorClass> classes = new List<SurvivorClass>
			{
				SurvivorClass.Bruiser,
				SurvivorClass.Hunter,
				SurvivorClass.Scout,
				SurvivorClass.Shooter
			};
			GiveTeam(manager, classes, 4, 7, 0, 4);
		}

		private void GiveMaxedOutItem(TWDModelManager manager, SurvivorModel survivor, EquipmentCategory category, int equipmentRarity)
		{
			EquipmentItemModel equipmentItemModel = manager.Player.Equipment.GenerateRandomEquipment(category, survivor.Level, equipmentRarity, useSpecialization: false, Faction.Survivor, survivor.SurvivorClass);
			if (survivor.CanEquip(equipmentItemModel))
			{
				survivor.Equip(equipmentItemModel);
			}
			manager.Player.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Debug);
			while (equipmentItemModel.CanUpgrade && equipmentItemModel.UpgradeInstant() == TWDModelResult.OK)
			{
			}
		}

		private void GiveTeam(TWDModelManager manager, List<SurvivorClass> classes, int fromLevel, int toLevel, int survivorRarity, int equipmentRarity)
		{
			SurvivorContainerModel survivorContainer = manager.Player.SurvivorContainer;
			while (survivorContainer.BuyNextSetOfSurvivorSlots() == TWDModelResult.OK)
			{
			}
			for (int i = fromLevel; i <= toLevel; i++)
			{
				for (int j = 0; j < classes.Count; j++)
				{
					if (survivorContainer.CanAddSurvivor())
					{
						int num = Math.Min(i, manager.Player.Camp.GetTrainingGroundLevel() + 1);
						SurvivorModel survivor = survivorContainer.CreateRandomSurvivor(0, num, num, survivorRarity, classes[j]);
						survivorContainer.AddSurvivor(survivor);
						GiveMaxedOutItem(manager, survivor, EquipmentCategory.Weapon, equipmentRarity);
						GiveMaxedOutItem(manager, survivor, EquipmentCategory.Armor, equipmentRarity);
					}
				}
			}
		}

		private void DoRerollDailyQuests(TWDModelManager manager)
		{
			PlayerModel player = manager.Player;
			player.DailyQuests.Clear();
			List<string> ignoreList = new List<string>();
			for (int i = 0; i < 3; i++)
			{
				player.AchievementManager.TryToCreateDailyQuest(ignoreList, updateCreationTime: true);
			}
		}

		private TWDModelResult DoAttackLevel(TWDModelManager manager, string levelName)
		{
			if (manager.Player.SurvivorContainer.CombatSurvivors.Count <= 0)
			{
				return TWDModelResult.NotEnoughSurvivors;
			}
			MapMissionModel mapMissionModel = null;
			foreach (MapMissionGroupModel mapMissionGroup in manager.Player.MapContainerModel.MapMissionGroups)
			{
				foreach (MapMissionModel mission in mapMissionGroup.Missions)
				{
					if (mission.MissionData.Id == levelName)
					{
						mapMissionModel = mission;
						break;
					}
				}
				if (mapMissionModel != null)
				{
					break;
				}
			}
			if (mapMissionModel == null)
			{
				return TWDModelResult.Error;
			}
			TWDModelResult tWDModelResult = TWDModelResult.Error;
			manager.ServerService?.Save(SaveType.Player);
			int num = (manager.Player.Blackboard.IsToggleOn("Toggle.OutpostGiftSurvivorsGiven") ? 6 : 3);
			num += mapMissionModel.MissionData.MaxTeamSize;
			bool num2 = !mapMissionModel.IsDeadly || manager.Player.SurvivorContainer.Survivors.Count >= num;
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(mapMissionModel.MissionSpawnPointGroup);
			if (num2)
			{
				tWDModelResult = manager.Player.MapContainerModel.AttackMission(mapMissionModel, missionGroupModelForSpawnPointGroup);
				bool flag = missionGroupModelForSpawnPointGroup.MissionSpawnPointGroup.Category != MapCategory.Endless;
				if (tWDModelResult == TWDModelResult.OK && flag)
				{
					DropEventDefinition.DropEventType eventType = ((mapMissionModel.MissionData.MissionType == MissionType.Rescue) ? DropEventDefinition.DropEventType.MissionRescue : DropEventDefinition.DropEventType.MissionScavenge);
					DropEventDefinition.DropEventContext context = (mapMissionModel.IsDeadly ? DropEventDefinition.DropEventContext.Deadly : mapMissionModel.DropContext);
					manager.Player.LootManager.ShuffleRewards(new LootEntryGenParams
					{
						eventType = eventType,
						targetLevel = mapMissionModel.MissionLevel,
						tag = mapMissionModel.LootTag,
						context = context
					});
				}
			}
			return tWDModelResult;
		}

		private void DoGiveFullClassTokens(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.AssaultToken).SetValue(manager.Player.GetCurrency(CurrencyType.AssaultToken).Max);
			manager.Player.GetCurrency(CurrencyType.BruiserToken).SetValue(manager.Player.GetCurrency(CurrencyType.BruiserToken).Max);
			manager.Player.GetCurrency(CurrencyType.WarriorToken).SetValue(manager.Player.GetCurrency(CurrencyType.WarriorToken).Max);
			manager.Player.GetCurrency(CurrencyType.HunterToken).SetValue(manager.Player.GetCurrency(CurrencyType.HunterToken).Max);
			manager.Player.GetCurrency(CurrencyType.ScoutToken).SetValue(manager.Player.GetCurrency(CurrencyType.ScoutToken).Max);
			manager.Player.GetCurrency(CurrencyType.ShooterToken).SetValue(manager.Player.GetCurrency(CurrencyType.ShooterToken).Max);
		}

		private void DoGiveFullHeroTokens(TWDModelManager manager, bool unlockHero = false)
		{
			foreach (CurrencyType value in Enum.GetValues(typeof(CurrencyType)))
			{
				if (manager.GameEconomyData.IsHeroToken(value))
				{
					manager.Player.GetCurrency(value).SetValue(manager.Player.GetCurrency(value).Max);
					if (unlockHero)
					{
						manager.Player.SurvivorContainer.UnlockHero(value);
					}
				}
			}
		}

		private void DoGiveEquipTokens(TWDModelManager manager, string equipTokenRewardString)
		{
			string[] array = equipTokenRewardString.Split('(');
			array[1] = array[1].Replace(")", "");
			string[] array2 = array[1].Split(',');
			string equipTokenId = array2[0];
			int amount = int.Parse(array2[1]);
			manager.Player.EquipTokenContainer.AddEquipToken(equipTokenId, amount);
		}

		private void ResetChallengeApocalypseBuffs(TWDModelManager manager, string ids)
		{
			string[] lst = ids.Split(',');
			manager.Player.WeeklyChallenge.weeklyChallengeApocalypseBuffs = manager.GameEconomyData.WeeklyChallengeApocalypseBuffs.Where((WeeklyChallengeApocalypseBuff x) => lst.Contains(x.Identifier)).ToList();
		}

		private void CompleteChallengeRoundsSkipReward(TWDModelManager manager, int circle, bool isApolytic)
		{
			if (manager.Player.Combat != null)
			{
				manager.Player.DeleteCombatModel(notify: false);
			}
			if (manager.Player != null && manager.Player.WeeklyChallenge != null)
			{
				if (isApolytic)
				{
					manager.Player.WeeklyChallenge.OpenedApocalypseWeeklyChallenge = true;
					manager.Player.ApocalypseWeeklyChallenge.SkipToCircle(circle);
				}
				else if (manager.Player.WeeklyChallenge.CurrentCycle + 1 < circle)
				{
					manager.Player.WeeklyChallenge.SkipToCircle(circle);
				}
				LootEntry lootEntry;
				do
				{
					lootEntry = manager.Player.WeeklyChallenge.GiveReward();
				}
				while (lootEntry != null);
			}
			else
			{
				manager.Debug.LogError("Bundle reward failed, missing bundle id or invalid player.");
			}
		}

		private void SetSurvivorLevelAndStarLevel(TWDModelManager manager, string SurvivorName, int SurvivorLevel, int TraitStarLevel)
		{
			SurvivorModel survivorModel = null;
			for (int i = 0; i < manager.Player.SurvivorContainer.Survivors.Models.Count; i++)
			{
				SurvivorModel survivorModel2 = manager.Player.SurvivorContainer.Survivors.Models[i];
				if (survivorModel2.Name == SurvivorName)
				{
					survivorModel = survivorModel2;
				}
			}
			if (survivorModel == null)
			{
				return;
			}
			manager.Player.Camp.GetBuilding("Council");
			while (survivorModel.Level < SurvivorLevel && survivorModel.Level <= survivorModel.MaxUpgradeLevel)
			{
				Cashier upgradeCashier = survivorModel.GetUpgradeCashier(instantUpgrade: true, !survivorModel.IsUpgrading());
				upgradeCashier.UsedReason = "UpgradeSurvivorInstant";
				List<CashierItem> cashierItems = upgradeCashier.GetCashierItems();
				for (int j = 0; j < cashierItems.Count; j++)
				{
					for (int k = 0; k < 174; k++)
					{
						cashierItems[j].SetCost((CurrencyType)k, 0);
					}
				}
				if (survivorModel.TimedActionModel.StartActionInstant(upgradeCashier, survivorModel) != TWDModelResult.OK)
				{
					break;
				}
			}
			if (TraitStarLevel < 1 || TraitStarLevel > 10)
			{
				return;
			}
			bool flag = false;
			while (!flag)
			{
				bool flag2 = false;
				if (survivorModel.CanUpgradeSurvivorRarity())
				{
					if (survivorModel.SurvivorRarityLevel >= TraitStarLevel - 1)
					{
						flag = true;
					}
					else
					{
						flag2 = survivorModel.UpgradeSurvivorRarity();
					}
				}
				else
				{
					flag2 = survivorModel.UpgradeLowestLevelTrait();
				}
				if (!flag2)
				{
					break;
				}
			}
		}

		private void ClearPhone(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.Phone)?.SetValue(0);
		}

		private void SetActiveFoundationPremium(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.ActiveFoundationPremium).SetValue(CommandParameter);
		}

		private void SetApocalypticSkipToken(TWDModelManager manager)
		{
			manager.Player.GetCurrency(CurrencyType.ApocalypticEquipToken).SetValue(CommandParameter);
		}

		private void AddSubscription(TWDModelManager manager, bool isWeek)
		{
			if (manager.Player.SubscriptionManager != null && manager.Player.Combat == null)
			{
				string text = manager.GameEconomyData.SubscriptionConfig.MonthlySubscriptionPrice;
				if (isWeek)
				{
					text = manager.GameEconomyData.SubscriptionConfig.WeeklySubscriptionPrice;
				}
				if (!manager.Player.SubscriptionManager.SubscriptionExpireDictionary.TryGetValue(text, out var value) || manager.Player.UtcTimeStamp > value)
				{
					value = manager.Player.UtcTimeStamp;
				}
				manager.Player.SubscriptionManager.SyncSubscriptionExpireDictionary(text, value + (long)(isWeek ? 7 : 30) * 24L * 3600 * 1000);
			}
		}

		private void ClearSubscription(TWDModelManager manager, bool isWeek)
		{
			if (manager.Player.SubscriptionManager != null)
			{
				manager.Player.SubscriptionManager.ClearSubscriptionExpireDictionary();
			}
		}

		private void DoGiveCurrency(TWDModelManager manager, CurrencyType currencyType, int amount)
		{
			foreach (CurrencyType value in Enum.GetValues(typeof(CurrencyType)))
			{
				if (value.Equals(currencyType))
				{
					manager.Player.GetCurrency(value).SetValue(amount);
				}
			}
		}

		private void DoGiveFullSupportTokens(TWDModelManager manager)
		{
			foreach (string supportDefinitionId in manager.GameEconomyData.SupportDefinitionIds)
			{
				SupportDefinition supportDefinition = manager.GameEconomyData.GetSupportDefinition(supportDefinitionId);
				CurrencyModel currency = manager.Player.GetCurrency(supportDefinition.Currency);
				currency.SetValue(currency.Max);
			}
		}

		private void DoUnlockAllSupports(PlayerModel player)
		{
			foreach (SupportModel supportModel in player.SupportModels)
			{
				supportModel.Level = Math.Min(supportModel.MaxLevel, 3);
			}
		}

		private void DoActivateShieldFor10Minutes(TWDModelManager manager)
		{
			manager.Player.SetOutpostShieldDebug(manager.Player.UtcTimeStamp + 600000);
		}

		private void DoGiveRewards(TWDModelManager manager, int amount, WeeklyChallengeReward.ChallengeRewardType rewardType, int control)
		{
			if (manager.Player.WeeklyChallenge != null)
			{
				for (int i = 0; i < amount; i++)
				{
					manager.Player.WeeklyChallenge.DEBUG_giveReward(rewardType, control);
				}
			}
		}

		private void DoInjury(TWDModelManager manager)
		{
			foreach (SurvivorModel combatSurvivor in manager.Player.SurvivorContainer.CombatSurvivors)
			{
				if (manager.Player.Camp.GetBuilding("MedicTent") is MedicTentModel medicTentModel)
				{
					combatSurvivor.SetHitpoints(combatSurvivor.MaxHitPoints / 3);
					combatSurvivor.MinHitpoints = combatSurvivor.Hitpoints;
					combatSurvivor.InjuryType = InjuryType.Major;
					if (manager.Player.Combat == null)
					{
						FixedPoint healingTimeModifier = 1.0;
						int missionLevel = combatSurvivor.Level * 3;
						medicTentModel.NewSurvivorInjured(combatSurvivor, missionLevel, healingTimeModifier);
					}
				}
			}
		}

		private void AddBooster(TWDModelManager manager, TimedBonusType type)
		{
			manager?.Player?.AddTimedBonus(type, 1.0);
		}

		private void RegenerateDailyQuests(TWDModelManager manager)
		{
		}

		private void CompleteAllDailyQuests(TWDModelManager manager)
		{
		}

		private void CompleteNewbieDayQuests(TWDModelManager manager, int day)
		{
		}

		private void ByCustomBundle(TWDModelManager manager, int day)
		{
		}

		private void PerformCommandSkillToTarget(TWDModelManager twdModelManager, int commandSkillPerformSurvivorIndex, int commandSkillPerformSurvivorSkillIndex, int commandSkillTargetSurvivorIndex)
		{
			List<ActorModel> factionActors = twdModelManager.Player.Combat.GetFactionActors(Faction.Survivor);
			try
			{
				ActorModel actorModel = factionActors[commandSkillPerformSurvivorIndex];
				GridCoordinate gridCoordinate = factionActors[commandSkillTargetSurvivorIndex].GridCoordinate;
				BaseCommandSkill commandSkill = actorModel.CommandSkillModelManager.CommandSkills[commandSkillPerformSurvivorSkillIndex];
				PerformCommandSkillCommand.PerformCommandSkill(twdModelManager, commandSkill, gridCoordinate);
			}
			catch (ArgumentOutOfRangeException)
			{
			}
		}

		private void DebugTestLeaderAPCost(TWDModelManager twdModelManager, int param)
		{
			if (twdModelManager.Player.Combat.GetFactionActors(Faction.Survivor)[0] is SurvivorModel survivorModel)
			{
				switch (param)
				{
				case 1:
					survivorModel.MoveCompleted = true;
					survivorModel.NotifyChange("actorMoveCompleted");
					break;
				case 2:
					survivorModel.SecondMoveCompleted = true;
					survivorModel.NotifyChange("actorSecondMoveCompleted");
					break;
				case 3:
					survivorModel.AbilityCompleted = true;
					break;
				}
			}
		}

		private void DebugRestoreLeaderAction(TWDModelManager twdModelManager, int survivalSlot, int restoreIndex)
		{
			SurvivorModel survivorModel = twdModelManager.Player.Combat.GetFactionActors(Faction.Survivor)[survivalSlot] as SurvivorModel;
			if (restoreIndex == 1 || restoreIndex == 0)
			{
				survivorModel.MoveCompleted = false;
			}
			if (restoreIndex == 2 || restoreIndex == 0)
			{
				survivorModel.SecondMoveCompleted = false;
			}
			if (restoreIndex == 3 || restoreIndex == 0)
			{
				survivorModel.AbilityCompleted = false;
			}
			if (restoreIndex == 4 || restoreIndex == 0)
			{
				survivorModel.TurnState = TurnState.Idle;
			}
			survivorModel.NotifyChange("actorExtraAbilityAction");
		}

		private void CompleteAllSectors(TWDModelManager manager)
		{
		}

		private void CompleteNextSector(TWDModelManager manager)
		{
		}

		private void FindNextEnemyInNextSector(TWDModelManager manager)
		{
		}

		private void FindAndKillNextEnemyInNextSector(TWDModelManager manager)
		{
		}

		private void PrintGuildModel(TWDModelManager manager)
		{
			string arg = manager.GetMessageSerializer().Serialize(manager.Player.GuildModel.GvGSeasonModel);
			UnityEngine.Debug.LogError($"[JSON-CLIENT] - \n {arg}");
		}

		private void DebugBuyIAPProduct(TWDModelManager twdModelManager, StorePurchaseInfo fakeStorePurchaseInfo)
		{
			PlayerModel player = twdModelManager.Player;
			BundleStoreDefinition bundleStoreDefinition = player.gameEconomyData.GetBundleStoreDefinition(fakeStorePurchaseInfo.BundleId);
			player.BundleManager.BuyBundle(bundleStoreDefinition, givenBySupport: false, Metrics.BundleSource.Cheat, 0L);
			twdModelManager.Save(SaveType.Player);
			player.BundleManager.SetInitiatedBundlePurchase(null);
		}

		private void DebugBuyCustomIAPProduct(TWDModelManager twdModelManager, StorePurchaseInfo fakeStorePurchaseInfo)
		{
			PlayerModel player = twdModelManager.Player;
			player.gameEconomyData.GetCustomBundleDefinition(fakeStorePurchaseInfo.BundleId);
			player.CustomizedBundleManager.CustomizedBundleClaimReward(fakeStorePurchaseInfo.BundleId);
			twdModelManager.Save(SaveType.Player);
			player.BundleManager.SetInitiatedBundlePurchase(null);
		}

		private void AddBattlePassTokens(TWDModelManager modelManager, int amount)
		{
			modelManager.Player.BattlePass.BattleCurrency.Add(amount);
		}

		private void MaximizePassTokensThroughKills(TWDModelManager modelManager)
		{
			modelManager.Player.BattlePass.AttemptToEarnCurrencyThroughKill(int.MaxValue);
		}

		private void MaximizeSurvivalPassTokens(TWDModelManager modelManager)
		{
			CurrencyModel battleCurrency = modelManager.Player.BattlePass.BattleCurrency;
			battleCurrency.SetValue(battleCurrency.Max);
		}

		private void ActivateBattlePassPremium(TWDModelManager modelManager)
		{
			modelManager.Player.BattlePass.ActivatePremium();
		}

		private void FakeBattlePassSeasonEnd(TWDModelManager modelManager)
		{
			BattlePassModel battlePass = modelManager.Player.BattlePass;
			if (battlePass.IsBeginnerBattlePass)
			{
				modelManager.Player.BeginnerBattlePassInfo.EndTimestamp = modelManager.Player.UtcTimeStamp - 1;
				battlePass.RefreshActiveSeason();
			}
			else
			{
				battlePass.FakeBattlePassSeasonEnd();
			}
		}

		private void RemoveSurvivorEquipment(TWDModelManager modelManager, string definitionID)
		{
			SurvivorModel survivorModel = null;
			foreach (SurvivorModel survivor in modelManager.Player.SurvivorContainer.Survivors)
			{
				foreach (EquipmentItemModel equipmentItem2 in survivor.EquipmentItems)
				{
					if (equipmentItem2.Definition.ID == definitionID)
					{
						survivorModel = survivor;
					}
				}
			}
			EquipmentItemModel equipmentItem = survivorModel?.EquipmentItems.First((EquipmentItemModel x) => x.Definition.ID == definitionID);
			survivorModel?.Unequip(equipmentItem);
		}

		private void RemoveSurvivor(TWDModelManager modelManager, SurvivorModel survivorModel)
		{
			modelManager.Player.SurvivorContainer.RemoveSurvivor(survivorModel);
		}

		private void GiveRewards(TWDModelManager modelManager, string rewards)
		{
			modelManager.Player.BundleManager.GiveRewardsGivenBySupport(rewards, 0L);
		}

		private void ChargeAllAllSurvivors(TWDModelManager twdModelManager, int num)
		{
			foreach (ActorModel model in twdModelManager.CombatModel.Survivors.Models)
			{
				model.AddChargePoints(num);
			}
		}

		private void DoActiveFoundationDayPremium(TWDModelManager modelManager)
		{
			modelManager.Player.ActiveFoundationManager.ActivatePremium();
		}

		private void DoGetPlayerAttribute(TWDModelManager modelManager)
		{
			modelManager.Player.PlayerAttributeContainer.GetAttributeValueByAttributeType(PlayerAttributeType);
		}

		private void UpgradeSurvivalManualAttributelevel(TWDModelManager modelManager)
		{
			modelManager.Player.SurvivalManualManager.UpgradeSurvivalManualAttributeLeve();
		}

		private void UpgradeSurvivalManualActor(TWDModelManager modelManager)
		{
			List<string> list = new List<string>();
			list.Add("Hero_Rick_Base");
			modelManager.Player.SurvivalManualManager.GetSurvivalManualModel(101).OneClickUpgradeActors(list);
		}

		private void UpgradeSurvivalManualStorySkill(TWDModelManager modelManager)
		{
			modelManager.Player.SurvivalManualManager.UpgradeSurvivalManualStorySkill(101);
		}

		private void DoUnlockSurvivalManualActorStory(TWDModelManager modelManager)
		{
			modelManager.Player.SurvivalManualManager.GetSurvivalManualModel(101).UnlockSurvivalManualActorStory("Hero_Aaron_Base", 1);
		}

		private void DoInitializeRoulette(TWDModelManager modelManager)
		{
			new DebugInitializeRouletteCommand().Execute(modelManager);
		}

		private void DoResetRoulette(TWDModelManager modelManager, int configId)
		{
			new DebugResetRouletteCommand(configId).Execute(modelManager);
		}

		private void DoFreeRouletteDraw(TWDModelManager modelManager, int configId, bool isMultiDraw)
		{
			new DebugFreeRouletteDrawCommand(configId, isMultiDraw).Execute(modelManager);
		}

		private void DoGetRouletteStatus(TWDModelManager modelManager, int configId)
		{
			new DebugGetRouletteStatusCommand(configId).Execute(modelManager);
		}

		private void DoAddRouletteCurrency(TWDModelManager modelManager, int amount)
		{
			new DebugAddRouletteCurrencyCommand(amount).Execute(modelManager);
		}

		private void DoSetRouletteSystemEnable(TWDModelManager modelManager, int enable)
		{
			new DebugSetRouletteSystemEnableCommand(enable).Execute(modelManager);
		}

		private void DoSetRouletteActivityTime(TWDModelManager modelManager, string parameter)
		{
			new DebugSetRouletteActivityTimeCommand(parameter).Execute(modelManager);
		}

		private void DoListRouletteConfigs(TWDModelManager modelManager)
		{
			new DebugListRouletteConfigsCommand().Execute(modelManager);
		}

		private void DoResetRouletteOpenLevel(TWDModelManager modelManager, int configId, int newOpenLevel)
		{
			new DebugResetRouletteOpenLevelCommand(configId, newOpenLevel).Execute(modelManager);
		}

		private void DoFetchEndlessSurvivorClassLeaderboard(TWDModelManager modelManager)
		{
			if (modelManager.ServerService == null)
			{
				modelManager.Debug.LogError("ServerService is null, cannot fetch leaderboard data");
				return;
			}
			PlayerModel player = modelManager.Player;
			if (player.EndlessModeManager.CurrentEndlessModeCalendarDefinition == null)
			{
				modelManager.Debug.LogError("No active endless mode calendar definition");
				return;
			}
			int identifier = player.EndlessModeManager.CurrentEndlessModeCalendarDefinition.Identifier;
			string hashedId = player.HashedId;
			modelManager.Debug.Log("=== Fetching Endless Mode Survivor Class Leaderboard Data ===");
			modelManager.Debug.Log($"Leaderboard ID: {identifier}");
			modelManager.Debug.Log("Player ID: " + hashedId);
			SurvivorClass[] obj = new SurvivorClass[6]
			{
				SurvivorClass.Scout,
				SurvivorClass.Hunter,
				SurvivorClass.Assault,
				SurvivorClass.Bruiser,
				SurvivorClass.Shooter,
				SurvivorClass.Warrior
			};
			Dictionary<SurvivorClass, LeaderboardData> dictionary = new Dictionary<SurvivorClass, LeaderboardData>();
			SurvivorClass[] array = obj;
			foreach (SurvivorClass survivorClass in array)
			{
				string endlessModeLeaderboardNameByClass = Leaderboards.GetEndlessModeLeaderboardNameByClass(identifier, survivorClass);
				LeaderboardPosition leaderboardPosition = modelManager.ServerService.GetLeaderboardPosition(endlessModeLeaderboardNameByClass, hashedId);
				List<LeaderboardEntry> leaderboard = modelManager.ServerService.GetLeaderboard(endlessModeLeaderboardNameByClass, "100");
				LeaderboardData value = new LeaderboardData
				{
					SurvivorClass = survivorClass,
					BoardName = endlessModeLeaderboardNameByClass,
					PlayerPosition = leaderboardPosition,
					Top100Entries = leaderboard
				};
				dictionary[survivorClass] = value;
				if (leaderboardPosition != null)
				{
					modelManager.Debug.Log($"\n--- {survivorClass} Leaderboard ---");
					modelManager.Debug.Log("Board Name: " + endlessModeLeaderboardNameByClass);
					modelManager.Debug.Log($"Position: {leaderboardPosition.Position + 1}");
					modelManager.Debug.Log($"Total Entries: {leaderboardPosition.LeaderboardCount}");
					if (leaderboard != null && leaderboard.Count > 0)
					{
						modelManager.Debug.Log($"Top 100 Entries Count: {leaderboard.Count}");
						modelManager.Debug.Log("Top 10 Preview:");
						for (int j = 0; j < Math.Min(10, leaderboard.Count); j++)
						{
							LeaderboardEntry leaderboardEntry = leaderboard[j];
							modelManager.Debug.Log($"  #{j + 1}: ID={leaderboardEntry.Id}, Score={leaderboardEntry.Score}");
						}
					}
					else
					{
						modelManager.Debug.LogWarning($"{survivorClass}: No top 100 entries found");
					}
				}
				else
				{
					modelManager.Debug.LogWarning($"{survivorClass}: No leaderboard data found");
				}
			}
			modelManager.Debug.Log("\n=== Fetch Complete ===");
		}
	}
}
