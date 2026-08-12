using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class WeeklySurvivalModel : TWDModelObject
	{
		public const string SurvivalCycleStarted = "SurvivalCycleStarted";

		public const string RewardsDedicatedRandomId = "WeeklySurvivalDedicatedRandom";

		private Dictionary<string, List<WeeklySurvivalReward>> orderedWeeklySurvivalRewardsBySet = new Dictionary<string, List<WeeklySurvivalReward>>();

		public int Id { get; set; }

		public int NextMissionOrderNumber { get; set; }

		public bool IsRestAvailable { get; set; }

		public int NumberCompleted { get; private set; }

		public int LastSeenNumberCompleted { get; set; }

		public FixedPoint PersonalHighScoreGrantedCompletionRatio { get; set; }

		public int PersonalHighScoreAtBeginningOfSurvival { get; set; }

		public int AllTimeNumberCompletions { get; private set; }

		public int PreviousNumberCompleted { get; private set; }

		public int PreviousSurvivalHighestDifficulty { get; set; }

		public ModelList<LootEntry> Rewards { get; set; }

		[JsonIgnore]
		public List<LootEntry> BoosterDoubleRewards { get; set; }

		public List<int> RewardMissionNumberHints { get; set; }

		public int LastSeenResetCount { get; set; }

		public int CurrentMapRestarts { get; set; }

		public bool SurvivalStartedSeen { get; set; }

		public bool SurvivalEndedSeen { get; set; }

		public SurvivalDifficulty CurrentDifficulty { get; set; }

		public bool DoubleRewardsEnabled { get; set; }

		[JsonIgnore]
		public WeeklySurvival CurrentDefinition => base.gameEconomyData.GetWeeklySurvival(Id);

		[JsonIgnore]
		public bool Finished
		{
			get
			{
				if (Id == -1)
				{
					return true;
				}
				if (CurrentDefinition == null)
				{
					return true;
				}
				return CurrentDefinition.EndTimeMilliseconds < base.manager.Player.UtcTimeStamp;
			}
		}

		[JsonIgnore]
		public bool CanCollectRewards => Rewards.Count > 0;

		[JsonIgnore]
		public bool IsOutOfSurvivors => base.manager.Player.SurvivorContainer.SurvivalCharacters.GetNumSurvivorsAvailableForAction() == 0;

		[JsonIgnore]
		public bool IsCompleted
		{
			get
			{
				if (CurrentDefinition != null)
				{
					return NumberCompleted == CurrentDefinition.TotalMissionCount;
				}
				return false;
			}
		}

		[JsonIgnore]
		public LootEntry FirstCollectablePersonalReward
		{
			get
			{
				if (Rewards != null)
				{
					for (int i = 0; i < Rewards.Count; i++)
					{
						if (Rewards[i] != null && Rewards[i].Type == LootEntryType.SurvivalPersonalReward)
						{
							return Rewards[i];
						}
					}
				}
				return null;
			}
		}

		[JsonIgnore]
		public WeeklySurvival NextWeeklySurvival => base.gameEconomyData.GetNextWeeklySurvival((CurrentDefinition == null) ? 0 : CurrentDefinition.EndTimeMilliseconds, base.manager.Player.UtcTimeStamp);

		[JsonIgnore]
		public bool CanPlayWeeklySurvival
		{
			get
			{
				if (!Finished)
				{
					return true;
				}
				if (CanPlayNextWeeklySurvival)
				{
					return true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanPlayNextWeeklySurvival
		{
			get
			{
				WeeklySurvival nextWeeklySurvival = NextWeeklySurvival;
				if (nextWeeklySurvival != null && base.manager.Player.UtcTimeStamp >= nextWeeklySurvival.StartTimeMilliseconds)
				{
					return base.manager.Player.UtcTimeStamp < nextWeeklySurvival.EndTimeMilliseconds;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsLockedByCouncilLevel => base.manager.CampModel.GetCouncilLevel() < base.manager.Player.gameEconomyData.ConfigData.SurvivalUnlockAtCouncilLevel;

		[JsonIgnore]
		public bool HasShownCycleEndedOnClient { get; set; }

		[JsonIgnore]
		public bool IsDifficultySelected => CurrentDifficulty != SurvivalDifficulty.None;

		public bool IsDifficultyLocked(SurvivalDifficulty difficulty)
		{
			if (IsLockedByCouncilLevel)
			{
				return true;
			}
			switch (difficulty)
			{
			case SurvivalDifficulty.None:
				return false;
			case SurvivalDifficulty.Normal:
				return false;
			case SurvivalDifficulty.Hard:
				return base.manager.CampModel.GetCouncilLevel() < base.manager.Player.gameEconomyData.ConfigData.SurvivalHardUnlockAtCouncilLevel;
			case SurvivalDifficulty.Nightmare:
				return base.manager.CampModel.GetCouncilLevel() < base.manager.Player.gameEconomyData.ConfigData.SurvivalNightmareUnlockAtCouncilLevel;
			default:
				base.Debug.LogError("Missing SurvivalDifficulty case.");
				return true;
			}
		}

		public override void Initialize()
		{
			base.Initialize();
			Id = -1;
			Rewards = new ModelList<LootEntry>();
			Rewards.SetManager(base.manager);
			RewardMissionNumberHints = null;
			PersonalHighScoreAtBeginningOfSurvival = 0;
			CurrentMapRestarts = 0;
			DoubleRewardsEnabled = false;
			CurrentDifficulty = SurvivalDifficulty.None;
			RestartMap();
		}

		public override void Start()
		{
			base.Start();
			RewardSetup();
		}

		public override bool IsValid()
		{
			return true;
		}

		public MissionSpawnPointGroup GetMissionSpawnPointGroup()
		{
			if (CurrentDefinition != null)
			{
				MissionSpawnPointGroup spawnPointGroup = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(CurrentDefinition.DetailMapId);
				if (spawnPointGroup == null)
				{
					base.manager.Debug.LogError("Could not find spawn point group for '" + CurrentDefinition.DetailMapId + "' cannot start survival!");
				}
				return spawnPointGroup;
			}
			return null;
		}

		public MapMissionGroupModel GetMapMissionGroupModel()
		{
			MissionSpawnPointGroup missionSpawnPointGroup = GetMissionSpawnPointGroup();
			if (missionSpawnPointGroup == null)
			{
				return null;
			}
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(missionSpawnPointGroup);
			if (missionGroupModelForSpawnPointGroup == null)
			{
				base.manager.Debug.LogError("Could not find group model for detailmap id = " + CurrentDefinition.DetailMapId);
			}
			return missionGroupModelForSpawnPointGroup;
		}

		[ModelAvailableTimer]
		public long TimeLeftToNextSurvival()
		{
			if (NextWeeklySurvival != null)
			{
				return NextWeeklySurvival.StartTimeMilliseconds - base.manager.Player.UtcTimeStamp;
			}
			return 0L;
		}

		public MapMissionGroupModel GetCurrentOrNextMapMissionGroupModel()
		{
			MapMissionGroupModel mapMissionGroupModel = GetMapMissionGroupModel();
			if (mapMissionGroupModel == null)
			{
				WeeklySurvival nextWeeklySurvival = NextWeeklySurvival;
				if (nextWeeklySurvival != null)
				{
					MissionSpawnPointGroup spawnPointGroup = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(nextWeeklySurvival.DetailMapId);
					if (spawnPointGroup != null)
					{
						mapMissionGroupModel = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(spawnPointGroup);
					}
				}
			}
			return mapMissionGroupModel;
		}

		public void ResetForNewIdentifier(int identifier)
		{
			CurrentMapRestarts = 0;
			DoubleRewardsEnabled = false;
			Reset(identifier, SurvivalDifficulty.None);
		}

		public void ResetCurrentToDifficultySelection()
		{
			CurrentMapRestarts++;
			Reset(Id, SurvivalDifficulty.None);
		}

		public void EnableDoubleRewards()
		{
			List<WeeklySurvivalReward> personalRewardsBetween = GetPersonalRewardsBetween(0, NumberCompleted);
			if (personalRewardsBetween != null)
			{
				for (int i = 0; i < personalRewardsBetween.Count; i++)
				{
					WeeklySurvivalReward survivalReward = personalRewardsBetween[i];
					AddReward(survivalReward, -1);
				}
				GivePendingDoubleRewards();
			}
			DoubleRewardsEnabled = true;
		}

		public void ResetCurrentForDifficulty(SurvivalDifficulty toDifficulty)
		{
			if (toDifficulty == SurvivalDifficulty.None)
			{
				base.Debug.LogError("Attempt to reset survival model to None difficulty, resetting to Normal instead.");
				toDifficulty = SurvivalDifficulty.Normal;
			}
			Reset(Id, toDifficulty);
		}

		private void Reset(int identifier, SurvivalDifficulty toDifficulty)
		{
			CurrentDifficulty = toDifficulty;
			SurvivalStartedSeen = false;
			SurvivalEndedSeen = false;
			if (Id != -1)
			{
				GetMapMissionGroupModel()?.RemoveMissions();
			}
			Id = identifier;
			NextMissionOrderNumber = 0;
			IsRestAvailable = false;
			PreviousNumberCompleted = NumberCompleted;
			NumberCompleted = 0;
			LastSeenNumberCompleted = 0;
			LastSeenResetCount = 0;
			PersonalHighScoreGrantedCompletionRatio = 0L;
			MissionSpawnPointGroup missionSpawnPointGroup = GetMissionSpawnPointGroup();
			if (missionSpawnPointGroup != null)
			{
				MapMissionGroupModel mapMissionGroupModel = GetMapMissionGroupModel();
				mapMissionGroupModel.RemoveMissions();
				base.manager.Player.MapContainerModel.SpawnMissionsForGroup(missionSpawnPointGroup);
				foreach (MapMissionModel mission in mapMissionGroupModel.Missions)
				{
					mission.SurvivalId = Id;
					mission.State = MapMissionState.Locked;
				}
				if (mapMissionGroupModel.Missions.Count > 0)
				{
					mapMissionGroupModel.Missions[0].State = MapMissionState.Unlocked;
				}
			}
			HasShownCycleEndedOnClient = false;
			RestartMap();
		}

		private void RewardSetup()
		{
			SetupWeeklySurvivalRewardEntries();
			SetupOrderedWeeklySurvivalRewards();
		}

		private void RestartMap()
		{
			RewardSetup();
			base.manager.Debug.Log("StartNewSurvivalCycle (" + CurrentMapRestarts + ")");
			if (GetMissionSpawnPointGroup() != null)
			{
				foreach (MapMissionModel mission in GetMapMissionGroupModel().Missions)
				{
					if (mission != null)
					{
						if (mission.SolveOrderNumberInGroup() == 0)
						{
							mission.State = MapMissionState.Unlocked;
						}
						else
						{
							mission.State = MapMissionState.Locked;
						}
						mission.RecalculateWeeklySurvivalMissionLevel();
					}
				}
			}
			HasShownCycleEndedOnClient = false;
			base.manager.Player.SavedSurvivalMissionData.ClearSavedState();
			base.manager.Player.SurvivorContainer.UpdateSurvivalSurvivorsList();
			NotifyChange("SurvivalCycleStarted");
		}

		public bool CanRestartMapOrDoubleRewards()
		{
			if (!DoubleRewardsEnabled)
			{
				return CurrentMapRestarts < 1;
			}
			return false;
		}

		public Cashier GetRestartCashier()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.SurvivalRestart);
			cashierItem.SetCost(CurrencyType.Diamonds, base.manager.GameEconomyData.ConfigData.SurvivalRestartCost);
			cashier.AddItem(cashierItem);
			return cashier;
		}

		public Cashier GetDoubleRewardsCashier()
		{
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.SurvivalDoubleRewards);
			cashierItem.SetCost(CurrencyType.Diamonds, base.manager.GameEconomyData.ConfigData.SurvivalDoubleRewardsCost);
			cashier.AddItem(cashierItem);
			return cashier;
		}

		public void AddPersonalCompletions(int amount)
		{
			int numberCompleted = NumberCompleted;
			List<WeeklySurvivalReward> personalRewardsBetween = GetPersonalRewardsBetween(NumberCompleted, NumberCompleted + amount);
			NumberCompleted += amount;
			AllTimeNumberCompletions += amount;
			if (personalRewardsBetween != null)
			{
				for (int i = 0; i < personalRewardsBetween.Count; i++)
				{
					WeeklySurvivalReward survivalReward = personalRewardsBetween[i];
					AddReward(survivalReward, numberCompleted);
				}
			}
		}

		public void AddFullCompletions(int amount)
		{
			if (base.manager != null && base.manager.GameEconomyData != null && base.manager.Player != null)
			{
				WeeklySurvivalReward weeklySurvivalReward = GetWeeklySurvivalReward(WeeklySurvivalReward.SurvivalRewardType.FullCompletion, CurrentDefinition.RewardSetName, CurrentMapRestarts + 1, controlExactMatch: false);
				if (weeklySurvivalReward != null)
				{
					AddReward(weeklySurvivalReward, -1);
				}
			}
		}

		private void AddReward(WeeklySurvivalReward survivalReward, int missionNumberHint)
		{
			int currentDifficulty = (int)CurrentDifficulty;
			if (survivalReward == null || survivalReward.RewardEntries == null || survivalReward.RewardEntries.Length <= currentDifficulty || survivalReward.RewardEntries[currentDifficulty] == null || survivalReward.RewardEntries[currentDifficulty].RewardsList == null)
			{
				return;
			}
			LootEntry lootEntry = null;
			for (int i = 0; i < survivalReward.RewardEntries[currentDifficulty].RewardsList.Count; i++)
			{
				int num = ((!DoubleRewardsEnabled) ? 1 : 2);
				for (int j = 0; j < num; j++)
				{
					IReward reward = survivalReward.RewardEntries[currentDifficulty].RewardsList[i];
					if (reward == null)
					{
						continue;
					}
					if (reward.Type == RewardType.TradeCrate && reward is RewardTradeCrate)
					{
						RewardTradeCrate rewardTradeCrate = reward as RewardTradeCrate;
						lootEntry = base.manager.Player.LootManager.CreateTradeCrateLoot(rewardTradeCrate.TradeCrateId, DropEventDefinition.DropEventType.MissionSurvival, ignoreCummulativeProbability: true, "WeeklySurvivalDedicatedRandom");
					}
					else if (reward.Type == RewardType.RandomEquipment)
					{
						RewardRandomEquipment reward2 = reward as RewardRandomEquipment;
						lootEntry = base.manager.Player.LootManager.CreateRandomEquipmentLoot(reward2);
						lootEntry.DropEventDefinition = base.manager.GameEconomyData.GetDropEvent(DropEventDefinition.DropEventType.MissionSurvival, DropEventDefinition.DropEventContext.Normal, DropEventDefinition.DropEventTag.ChallengeCrateSilver);
					}
					else if (reward.Type == RewardType.Equipment)
					{
						RewardEquipment reward3 = reward as RewardEquipment;
						lootEntry = base.manager.Player.LootManager.CreateConsumablesLoot(reward3, DropType.Gold);
						lootEntry.DropEventDefinition = base.manager.GameEconomyData.GetDropEvent(DropEventDefinition.DropEventType.MissionSurvival, DropEventDefinition.DropEventContext.Normal, DropEventDefinition.DropEventTag.ChallengeCrateGold);
					}
					else
					{
						lootEntry = base.manager.Player.LootManager.CreateCurrencyLoot(reward, DropType.Gold, DropCurrenciesProbabilitiesDefinition.DropCurrency.AnyCurrency);
					}
					if (lootEntry != null)
					{
						if (DoubleRewardsEnabled && reward.Type != RewardType.RandomEquipment && (reward.Type != RewardType.Equipment || (reward is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(base.manager))))
						{
							lootEntry.RewardedAmount *= 2;
							j++;
						}
						lootEntry.Type = LootManagerModel.GetLootEntryTypeFromSurvivalReward(survivalReward);
						lootEntry.Control = survivalReward.Control;
						if (RewardMissionNumberHints == null)
						{
							ResetRewardMissionNumberHints();
						}
						if (RewardMissionNumberHints.Count != Rewards.Count)
						{
							base.Debug.LogError("Survival model reward list and reward mission number hint list count mismatch at pending reward add, resetting the mission number hints.");
							ResetRewardMissionNumberHints();
						}
						RewardMissionNumberHints.Add(missionNumberHint);
						Rewards.Add(lootEntry);
					}
				}
			}
		}

		private bool CanLootTypesBeGivenAsDualDrop(LootEntry loot1, LootEntry loot2)
		{
			if (loot1 == null || loot2 == null)
			{
				return false;
			}
			bool num = loot1.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor || loot1.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon;
			bool flag = loot2.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor || loot2.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon;
			if (!num)
			{
				return !flag;
			}
			return false;
		}

		public bool CanNextRewardsBeGivenAsDualDrop()
		{
			if (Rewards.Count < 2)
			{
				return false;
			}
			if (RewardMissionNumberHints == null || RewardMissionNumberHints.Count < 2)
			{
				return false;
			}
			if (RewardMissionNumberHints[0] != RewardMissionNumberHints[1] || RewardMissionNumberHints[0] == -1)
			{
				return false;
			}
			return CanLootTypesBeGivenAsDualDrop(Rewards[0], Rewards[1]);
		}

		private LootEntry GiveNextRewardFromQueue()
		{
			LootEntry lootEntry = null;
			bool partiallyDoubleRewards = false;
			bool flag = DoubleRewardsEnabled;
			if (Rewards.Count > 0)
			{
				lootEntry = Rewards[0];
				if (flag)
				{
					if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor || lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon)
					{
						if (Rewards.Count > 1)
						{
							flag = false;
						}
					}
					else
					{
						partiallyDoubleRewards = true;
					}
				}
				base.manager.Player.LootManager.GiveLoot(lootEntry);
				ReportSurvivalAnalytics(lootEntry, flag, partiallyDoubleRewards);
				Rewards.RemoveAt(0);
				if (RewardMissionNumberHints != null)
				{
					if (RewardMissionNumberHints.Count == Rewards.Count + 1)
					{
						RewardMissionNumberHints.RemoveAt(0);
					}
					else
					{
						base.Debug.LogError("Survival model reward list and reward mission number hint list count mismatch while giving a reward, resetting the mission number hints.");
						ResetRewardMissionNumberHints();
					}
				}
			}
			return lootEntry;
		}

		public bool GiveReward(out LootEntry loot1, out LootEntry loot2)
		{
			if (CanNextRewardsBeGivenAsDualDrop())
			{
				loot1 = GiveNextRewardFromQueue();
				loot2 = GiveNextRewardFromQueue();
			}
			else
			{
				loot1 = GiveNextRewardFromQueue();
				loot2 = null;
			}
			return loot1 != null;
		}

		private void ReportSurvivalAnalytics(LootEntry lootEntry, bool doubleRewardsEnabled, bool partiallyDoubleRewards)
		{
			if (base.manager == null || base.manager.Player == null || lootEntry == null)
			{
				return;
			}
			if (lootEntry.DropEventDefinition != null && lootEntry.DropEventDefinition.EventType == DropEventDefinition.DropEventType.MissionSurvival)
			{
				base.manager.Metrics.AddFind().AddLoot(lootEntry).AddDistance()
					.AddSurvivalReward(lootEntry)
					.AddLootCrate(lootEntry);
				if (doubleRewardsEnabled)
				{
					base.manager.Metrics.DoubleRewards(partiallyDoubleRewards);
				}
				base.manager.Metrics.Send();
			}
			else if (lootEntry.Type == LootEntryType.SurvivalFullCompletionReward)
			{
				if (lootEntry.DropEventDefinition != null && lootEntry.DropEventDefinition.EventType == DropEventDefinition.DropEventType.MissionSurvival)
				{
					base.manager.Metrics.AddFind().AddLoot(lootEntry).AddDistance()
						.AddSurvivalRoundReward()
						.AddLootCrate(lootEntry)
						.Send();
				}
				else
				{
					base.manager.Metrics.AddFind().AddLoot(lootEntry).AddDistance()
						.AddSurvivalRoundReward()
						.Send();
				}
			}
			else
			{
				base.manager.Metrics.AddFind().AddLoot(lootEntry).AddMission()
					.AddDistance()
					.AddStaticReward();
				if (doubleRewardsEnabled)
				{
					base.manager.Metrics.DoubleRewards(partiallyDoubleRewards);
				}
				base.manager.Metrics.Send();
			}
		}

		public List<WeeklySurvivalReward> GetPersonalRewardsBetween(int fromCompletions, int toCompletions)
		{
			List<WeeklySurvivalReward> list = new List<WeeklySurvivalReward>();
			for (int i = 0; i < ((base.gameEconomyData.WeeklySurvivalRewards != null) ? base.gameEconomyData.WeeklySurvivalRewards.Length : 0); i++)
			{
				WeeklySurvivalReward weeklySurvivalReward = base.gameEconomyData.WeeklySurvivalRewards[i];
				if (weeklySurvivalReward.SetName == CurrentDefinition.RewardSetName && weeklySurvivalReward.RewardType == WeeklySurvivalReward.SurvivalRewardType.MissionCompletions && weeklySurvivalReward.Control > fromCompletions && (weeklySurvivalReward.Control <= toCompletions || toCompletions == -1))
				{
					list.Add(weeklySurvivalReward);
				}
			}
			return list;
		}

		public WeeklySurvivalReward GetNextReward()
		{
			for (int i = 0; i < ((base.gameEconomyData.WeeklySurvivalRewards != null) ? base.gameEconomyData.WeeklySurvivalRewards.Length : 0); i++)
			{
				WeeklySurvivalReward weeklySurvivalReward = base.gameEconomyData.WeeklySurvivalRewards[i];
				if (weeklySurvivalReward.SetName == CurrentDefinition.RewardSetName && weeklySurvivalReward.RewardType == WeeklySurvivalReward.SurvivalRewardType.MissionCompletions && weeklySurvivalReward.Control > NumberCompleted)
				{
					return weeklySurvivalReward;
				}
			}
			return null;
		}

		public void MarkSurvivalStartedAsSeen()
		{
			SurvivalStartedSeen = true;
		}

		public void MarkSurvivalEndedAsSeen()
		{
			SurvivalEndedSeen = true;
		}

		public bool HasSeenLatestCompletions()
		{
			return NumberCompleted == LastSeenNumberCompleted;
		}

		public List<LootEntry> GetRewardsPerType(LootEntryType type)
		{
			List<LootEntry> list = new List<LootEntry>();
			if (Rewards != null)
			{
				for (int i = 0; i < Rewards.Count; i++)
				{
					if (Rewards[i] != null && Rewards[i].Type == type)
					{
						list.Add(Rewards[i]);
					}
				}
			}
			return list;
		}

		private void ResetRewardMissionNumberHints()
		{
			RewardMissionNumberHints = new List<int>();
			for (int i = 0; i < Rewards.Count; i++)
			{
				RewardMissionNumberHints.Add(-1);
			}
		}

		public void MoveToNextMission()
		{
			NextMissionOrderNumber++;
			IsRestAvailable = true;
		}

		public void DEBUG_giveReward(WeeklySurvivalReward.SurvivalRewardType rewardType, int control)
		{
			WeeklySurvivalReward weeklySurvivalReward = GetWeeklySurvivalReward(rewardType, CurrentDefinition.RewardSetName, control, controlExactMatch: false);
			if (weeklySurvivalReward != null)
			{
				AddReward(weeklySurvivalReward, -1);
				base.manager.Player.LootManager.GiveLoot(Rewards[Rewards.Count - 1]);
			}
		}

		public void DEBUG_clearAllPendingRewards()
		{
			Rewards.Clear();
			RewardMissionNumberHints.Clear();
		}

		public WeeklySurvivalReward GetWeeklySurvivalReward(WeeklySurvivalReward.SurvivalRewardType rewardType, string setName, int control, bool controlExactMatch)
		{
			WeeklySurvivalReward result = null;
			string key = rewardType.ToString() + "_" + setName;
			if (orderedWeeklySurvivalRewardsBySet != null && orderedWeeklySurvivalRewardsBySet.ContainsKey(key))
			{
				List<WeeklySurvivalReward> list = orderedWeeklySurvivalRewardsBySet[key];
				if (list != null)
				{
					for (int i = 0; i < list.Count; i++)
					{
						WeeklySurvivalReward weeklySurvivalReward = list[i];
						if (weeklySurvivalReward != null)
						{
							if (controlExactMatch && weeklySurvivalReward.Control == control)
							{
								return weeklySurvivalReward;
							}
							if (control < weeklySurvivalReward.Control)
							{
								return result;
							}
							result = weeklySurvivalReward;
						}
					}
				}
			}
			return result;
		}

		private void SetupWeeklySurvivalRewardEntries()
		{
			if (base.gameEconomyData.WeeklySurvivalRewards == null)
			{
				return;
			}
			for (int i = 0; i < base.gameEconomyData.WeeklySurvivalRewards.Length; i++)
			{
				WeeklySurvivalReward weeklySurvivalReward = base.gameEconomyData.WeeklySurvivalRewards[i];
				if (weeklySurvivalReward == null || weeklySurvivalReward.RewardEntries != null)
				{
					continue;
				}
				int controlVariable = 0;
				if (weeklySurvivalReward.RewardType == WeeklySurvivalReward.SurvivalRewardType.MissionCompletions)
				{
					int missionOrderNumber = weeklySurvivalReward.Control - 1;
					controlVariable = SurvivalMissionDifficultyLevelHelper.CalculateResultingSurvivalMissionLevel(base.gameEconomyData, missionOrderNumber, base.manager.Player.CouncilLevel, CurrentDifficulty);
				}
				int length = Enum.GetValues(typeof(SurvivalDifficulty)).Length;
				weeklySurvivalReward.RewardEntries = new Rewards[length];
				for (int j = 0; j < length; j++)
				{
					string text = null;
					switch ((SurvivalDifficulty)j)
					{
					case SurvivalDifficulty.Normal:
						text = weeklySurvivalReward.RewardsNormal;
						break;
					case SurvivalDifficulty.Hard:
						text = weeklySurvivalReward.RewardsHard;
						break;
					case SurvivalDifficulty.Nightmare:
						text = weeklySurvivalReward.RewardsNightmare;
						break;
					}
					if (text != null)
					{
						try
						{
							weeklySurvivalReward.RewardEntries[j] = new Rewards(text, base.manager, controlVariable, EquipmentSource.MissionLoot);
						}
						catch (Exception)
						{
							weeklySurvivalReward.RewardEntries[j] = new Rewards();
						}
					}
				}
			}
		}

		private void SetupOrderedWeeklySurvivalRewards()
		{
			if (orderedWeeklySurvivalRewardsBySet == null || orderedWeeklySurvivalRewardsBySet.Count != 0)
			{
				return;
			}
			for (int i = 0; i < base.gameEconomyData.WeeklySurvivalRewards.Length; i++)
			{
				WeeklySurvivalReward weeklySurvivalReward = base.gameEconomyData.WeeklySurvivalRewards[i];
				if (weeklySurvivalReward != null)
				{
					string key = weeklySurvivalReward.RewardType.ToString() + "_" + weeklySurvivalReward.SetName;
					if (!orderedWeeklySurvivalRewardsBySet.ContainsKey(key) || orderedWeeklySurvivalRewardsBySet[key] == null)
					{
						orderedWeeklySurvivalRewardsBySet[key] = new List<WeeklySurvivalReward>();
					}
					orderedWeeklySurvivalRewardsBySet[key].Add(weeklySurvivalReward);
				}
			}
			foreach (KeyValuePair<string, List<WeeklySurvivalReward>> item in orderedWeeklySurvivalRewardsBySet)
			{
				item.Value.StableSort((WeeklySurvivalReward a, WeeklySurvivalReward b) => (a != null && b != null) ? Math.Sign(a.Control - b.Control) : 0);
			}
		}

		private void GivePendingDoubleRewards()
		{
			if (Rewards.Count > 0)
			{
				BoosterDoubleRewards = new List<LootEntry>();
			}
			while (Rewards.Count > 0)
			{
				LootEntry lootEntry = Rewards[0];
				base.manager.Player.LootManager.GiveLoot(lootEntry);
				ReportSurvivalAnalytics(lootEntry, doubleRewardsEnabled: true, partiallyDoubleRewards: false);
				Rewards.RemoveAt(0);
				BoosterDoubleRewards.Add(lootEntry);
				if (RewardMissionNumberHints != null)
				{
					if (RewardMissionNumberHints.Count == Rewards.Count + 1)
					{
						RewardMissionNumberHints.RemoveAt(0);
						continue;
					}
					base.Debug.LogError("Survival model reward list and reward mission number hint list count mismatch while giving a reward, resetting the mission number hints.");
					ResetRewardMissionNumberHints();
				}
			}
		}
	}
}
