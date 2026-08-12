using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class DailyQuestModel : TWDModelObject
	{
		[JsonIgnore]
		private Dictionary<string, DailyQuestRule> allDailyQuestRules = new Dictionary<string, DailyQuestRule>();

		[JsonIgnore]
		private QuestCompleteContext Context;

		private bool doingAction;

		[JsonIgnore]
		private DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToUniversalTime();

		[JsonIgnore]
		private TimeSpan TimeWrapThreshold = new TimeSpan(1, 0, 0, 0);

		[JsonIgnore]
		private TimeSpan TimeWrapThresholdCurrent = new TimeSpan(1, 0, 0, 0);

		public ModelList<DailyQuestItemModel> ActiveQuests { get; set; }

		public int QuestPoints { get; set; }

		public string CurrentQuestChestId { get; set; }

		[JsonIgnore]
		public DateTime LastDailyQuestResetDate { get; set; }

		[JsonIgnore]
		public DateTime NextQuestRefreshTimeUtc => LastDailyQuestResetDate + base.manager.GameEconomyData.ConfigData.DailyQuestsResetTime + new TimeSpan(1, 0, 0, 0);

		public string RewardSetId { get; set; }

		public ModelList<LootEntry> QuestChestRewards { get; set; }

		public long LastDailyQuestResetDateTimeStamp { get; set; }

		public int QuestsRefreshCount { get; set; }

		public bool QuestsInitialized { get; set; }

		[JsonIgnore]
		public DailyQuestChestDefinition CurrentQuestChestDefinition { get; private set; }

		[ModelAvailableTimer]
		public long TimeLeftToNextRefresh()
		{
			return (long)(NextQuestRefreshTimeUtc - base.manager.Player.UtcTime).TotalMilliseconds;
		}

		public static bool GetIsSupported(GameEconomyData ged)
		{
			if (ged == null)
			{
				return false;
			}
			return ged.ConfigData.DailyQuestsVersion > 0;
		}

		public void BeginWindow(DailyQuestCompletionWindow window)
		{
			for (int i = 0; i < ActiveQuests.Count; i++)
			{
				ActiveQuests[i].ResetIfInWindow(window);
			}
			base.manager.Player.NewbieSenvenQuest.ResetIfInWindow(window);
		}

		public void EndWindow(DailyQuestCompletionWindow window)
		{
			for (int i = 0; i < ActiveQuests.Count; i++)
			{
				ActiveQuests[i].CommitWindow(window);
			}
			base.manager.Player.NewbieSenvenQuest.CommitWindow(window);
		}

		public override void Initialize()
		{
			QuestsInitialized = false;
			QuestChestRewards = new ModelList<LootEntry>();
			QuestChestRewards.SetManager(base.manager);
			QuestChestRewards.Initialize();
			ActiveQuests = new ModelList<DailyQuestItemModel>();
			ActiveQuests.SetManager(base.manager);
			ActiveQuests.Initialize();
			LastDailyQuestResetDate = origin;
		}

		public bool TryInitializeQuests()
		{
			if (!GetIsSupported(base.manager.GameEconomyData))
			{
				return false;
			}
			if (!QuestsInitialized)
			{
				RandomizeQuestChest();
				TryGenerateQuests();
				CurrentQuestChestDefinition = base.manager.GameEconomyData.GetDailyQuestChest(CurrentQuestChestId);
				QuestsInitialized = true;
			}
			return true;
		}

		public override void Start()
		{
			base.Start();
			UpdateTimeWrapThreshold();
			LastDailyQuestResetDate = origin.AddMilliseconds(LastDailyQuestResetDateTimeStamp);
			Context = new QuestCompleteContext(base.manager);
			DailyQuestRuleFunctions.RegisterRuleFunctions(this);
			DailyQuestDefinition[] dailyQuestDefinitions = base.manager.GameEconomyData.DailyQuestDefinitions;
			allDailyQuestRules.Clear();
			foreach (DailyQuestDefinition dailyQuestDefinition in dailyQuestDefinitions)
			{
				if (allDailyQuestRules.ContainsKey(dailyQuestDefinition.Id))
				{
					base.Debug.LogError($"Found two daily quest definitions with the same ID {dailyQuestDefinition.Id}. Ignoring the duplicates.");
				}
				DailyQuestRule dailyQuestRule = new DailyQuestRule();
				if (!dailyQuestRule.LoadRule(dailyQuestDefinition, Context))
				{
					base.Debug.LogError($"Failed to load rule for daily quest {dailyQuestDefinition.Id}.");
				}
				else
				{
					allDailyQuestRules.Add(dailyQuestDefinition.Id, dailyQuestRule);
				}
			}
			for (int j = 0; j < ActiveQuests.Count; j++)
			{
				DailyQuestRule rule = null;
				if (!ActiveQuests[j].IsCompleteAllQuest)
				{
					if (!allDailyQuestRules.ContainsKey(ActiveQuests[j].Id))
					{
						base.Debug.LogWarning($"Can't start quest with ID {ActiveQuests[j].Id} because quest definition was not found. Removing quest from player.");
						ActiveQuests.RemoveAt(j);
						j--;
						continue;
					}
					rule = allDailyQuestRules[ActiveQuests[j].Id];
				}
				ActiveQuests[j].StartQuest(Context, this, rule);
			}
			StartNewbieSenvenQuests();
			if (CurrentQuestChestId != null)
			{
				CurrentQuestChestDefinition = base.manager.GameEconomyData.GetDailyQuestChest(CurrentQuestChestId);
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (QuestsInitialized)
			{
				TryGenerateQuests();
			}
		}

		public int CalculateUnclaimedCount()
		{
			if (ActiveQuests == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < ActiveQuests.Count; i++)
			{
				if (ActiveQuests[i] != null && ActiveQuests[i].IsCompleted && !ActiveQuests[i].Claimed)
				{
					num++;
				}
			}
			return num;
		}

		private void TryGenerateQuests(bool force = false)
		{
			DateTime utcTime = base.manager.Player.UtcTime;
			if (utcTime > NextQuestRefreshTimeUtc || force)
			{
				LastDailyQuestResetDate = new DateTime(utcTime.Year, utcTime.Month, utcTime.Day);
				if (TimeSpan.Compare(NextQuestRefreshTimeUtc - utcTime, TimeWrapThresholdCurrent) >= 0)
				{
					LastDailyQuestResetDate -= new TimeSpan(1, 0, 0, 0);
				}
				LastDailyQuestResetDateTimeStamp = (long)(LastDailyQuestResetDate - origin).TotalMilliseconds;
				GenerateQuests();
				UpdateTimeWrapThreshold();
			}
		}

		private void UpdateTimeWrapThreshold()
		{
			TimeWrapThresholdCurrent = TimeWrapThreshold;
			if (QuestsRefreshCount <= 1)
			{
				TimeWrapThresholdCurrent += base.manager.GameEconomyData.ConfigData.DailyQuestsExtraTimeWrapThreshold;
			}
		}

		public void RegisterRuleFunction(string name, QuestCompleteContext.Function function)
		{
			if (Context.Functions.ContainsKey(name))
			{
				base.Debug.LogWarning($"Trying to reregister the rule function {name}.");
			}
			else
			{
				Context.Functions.Add(name, function);
			}
		}

		private void PopulateContextVariablesWithDefaults(string operation)
		{
			Context.Variables.Operation = operation;
			MapMissionModel mapMissionModel = ((base.manager.Player.MapContainerModel != null) ? base.manager.Player.MapContainerModel.AttackTargetMissionModel : null);
			if (mapMissionModel != null)
			{
				MapCategory mapCategory = MapCategory.None;
				if (mapMissionModel.MissionSpawnPointGroup != null)
				{
					mapCategory = mapMissionModel.MissionSpawnPointGroup.Category;
				}
				Context.Variables.MissionKind = mapCategory.ToString();
			}
			Context.Variables.CouncilLevel = base.manager.Player.CouncilLevel;
			CombatModel combat = base.manager.Player.Combat;
			if (combat != null && combat.MissionRoster != null)
			{
				for (int i = 0; i < combat.MissionRoster.Count; i++)
				{
					SurvivorModel survivorModel = combat.MissionRoster[i];
					Context.Variables.SurvivorClass.Add(survivorModel.SurvivorClass.ToString());
					if (survivorModel.IsHero || !string.IsNullOrEmpty(survivorModel.Definition.AltOf))
					{
						Context.Variables.Hero.Add(survivorModel.Definition.GetNonAlternativeHeroDefinition());
					}
				}
			}
			Context.Variables.GameMode = Context.Variables.MissionKind;
			Context.Variables.CurrentTime = base.manager.Player.UtcTimeStamp;
			Context.Variables.EquipmentCount = base.manager.Player.Equipment.GetAllEquipmentCount();
			WeeklyChallengeModel weeklyChallenge = base.manager.Player.WeeklyChallenge;
			if (weeklyChallenge != null)
			{
				Context.Variables.ChallengeRoundsComplete = weeklyChallenge.CurrentCycle;
			}
		}

		public QuestVariables StartAction(string operation)
		{
			if (doingAction)
			{
				base.Debug.LogError("Trying to start quest action when already doing action.");
				return null;
			}
			Context.Variables.Clear();
			doingAction = true;
			if (!TryInitializeQuests())
			{
				return Context.Variables;
			}
			PopulateContextVariablesWithDefaults(operation);
			return Context.Variables;
		}

		public void CommitAction()
		{
			bool flag = doingAction;
			doingAction = false;
			if (!QuestsInitialized)
			{
				return;
			}
			if (!flag)
			{
				base.Debug.LogError("Trying to commit action when not currently doing any.");
				return;
			}
			for (int i = 0; i < ActiveQuests.Count; i++)
			{
				ActiveQuests[i].UpdateWithContext(Context);
			}
			base.manager.Player.NewbieSenvenQuest.UpdateWithContext(Context);
		}

		private void GenerateQuestsFromList(DailyQuestSetDefinition setDefinition, List<DailyQuestSelectionDefinition> setQuests, int slotIndex)
		{
			if (setQuests.Count == 0)
			{
				base.Debug.LogError("The quest definition set does not have quest definitions.");
				return;
			}
			int councilLevel = base.manager.Player.CouncilLevel;
			List<DailyQuestSelectionDefinition> list = new List<DailyQuestSelectionDefinition>(setQuests.OrderBy((DailyQuestSelectionDefinition x) => x.Weight));
			int num = 0;
			for (int num2 = 0; num2 < list.Count; num2++)
			{
				if (string.Compare(list[num2].QuestCategory, "Empty", ignoreCase: true) == 0)
				{
					list.RemoveAt(num2);
					num2--;
				}
				else
				{
					num += list[num2].Weight;
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			int count = ActiveQuests.Count;
			while (list.Count > 0)
			{
				int num3 = base.manager.Player.PlayerRandom.Next(num);
				DailyQuestSelectionDefinition dailyQuestSelectionDefinition = null;
				int num4 = 0;
				for (int num5 = 0; num5 < list.Count; num5++)
				{
					int num6 = num4 + list[num5].Weight;
					if (num3 >= num4 && num3 < num6)
					{
						dailyQuestSelectionDefinition = list[num5];
						break;
					}
					num4 = num6;
				}
				if (dailyQuestSelectionDefinition == null)
				{
					break;
				}
				GameEconomyData gameEconomyData = base.manager.GameEconomyData;
				string questCategory = dailyQuestSelectionDefinition.QuestCategory;
				DailyQuestDefinitionSize size = dailyQuestSelectionDefinition.Size;
				List<DailyQuestDefinition> list2 = new List<DailyQuestDefinition>();
				for (int num7 = 0; num7 < gameEconomyData.DailyQuestDefinitions.Length; num7++)
				{
					DailyQuestDefinition dailyQuestDefinition = gameEconomyData.DailyQuestDefinitions[num7];
					if (!(dailyQuestDefinition.Category == questCategory) || councilLevel < dailyQuestDefinition.CouncilLevelMin || councilLevel > dailyQuestDefinition.CouncilLevelMax || dailyQuestDefinition.GetSize(size) < 1)
					{
						continue;
					}
					if (!allDailyQuestRules.ContainsKey(dailyQuestDefinition.Id))
					{
						base.Debug.LogError($"Could not consider daily quest with ID {dailyQuestDefinition.Id} for selection. The rule for the quest is not loaded.");
						continue;
					}
					DailyQuestRule dailyQuestRule = allDailyQuestRules[dailyQuestDefinition.Id];
					Context.Variables.Clear();
					PopulateContextVariablesWithDefaults("IsRuleAvailable");
					if (dailyQuestRule.IsAvailableRuleCheck != null && dailyQuestRule.IsAvailableRuleCheck.Evaluate(Context) == 0L)
					{
						continue;
					}
					bool flag = false;
					for (int num8 = 0; num8 < ActiveQuests.Count; num8++)
					{
						if (ActiveQuests[num8].Definition == gameEconomyData.DailyQuestDefinitions[num7])
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						list2.Add(gameEconomyData.DailyQuestDefinitions[num7]);
					}
				}
				if (list2.Count == 0)
				{
					list.Remove(dailyQuestSelectionDefinition);
					num -= dailyQuestSelectionDefinition.Weight;
					continue;
				}
				int index = base.manager.Player.PlayerRandom.Next(list2.Count);
				DailyQuestDefinition dailyQuestDefinition2 = list2[index];
				int index2 = (int)size;
				int sizeWithIndex = dailyQuestDefinition2.GetSizeWithIndex(index2);
				if (sizeWithIndex < 0)
				{
					base.Debug.LogError($"Quest with ID {dailyQuestDefinition2.Id} does not define size {size}.");
					return;
				}
				DailyQuestItemModel dailyQuestItemModel = new DailyQuestItemModel(dailyQuestDefinition2.Id, setDefinition.Id, sizeWithIndex, sizeWithIndex, slotIndex);
				dailyQuestItemModel.SetManager(base.manager);
				dailyQuestItemModel.Initialize();
				dailyQuestItemModel.Start();
				dailyQuestItemModel.StartQuest(Context, this, allDailyQuestRules[dailyQuestDefinition2.Id]);
				ActiveQuests.Add(dailyQuestItemModel);
				base.manager.Metrics.AddGenerate().AddNewDailyQuest(dailyQuestItemModel, setDefinition.Id, slotIndex).Send();
				break;
			}
			if (count == ActiveQuests.Count)
			{
				base.Debug.LogError($"No quests found for slot {slotIndex} and player council level {councilLevel} in quest set {setDefinition.Id}.");
				base.manager.Metrics.AddGenerate().AddNewDailyQuestGenerationFailed(setDefinition.Id, slotIndex).Send();
			}
		}

		private void GenerateQuestsForSet(DailyQuestSetDefinition setDefinition)
		{
			if (string.IsNullOrEmpty(setDefinition.RewardSets))
			{
				base.Debug.LogError($"The daily quest set {setDefinition.Id} does not have any reward sets.");
				return;
			}
			string[] array = setDefinition.RewardSets.Split(';');
			List<DailyQuestRewardSetDefinition> list = new List<DailyQuestRewardSetDefinition>();
			int num = 0;
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				DailyQuestRewardSetDefinition dailyQuestRewardSetDefinition = base.manager.GameEconomyData.GetDailyQuestRewardSetDefinition(text);
				if (dailyQuestRewardSetDefinition == null)
				{
					base.Debug.LogError($"The reward set {text} used by daily quest set {setDefinition.Id} was not found in the GED.");
					continue;
				}
				list.Add(dailyQuestRewardSetDefinition);
				num += dailyQuestRewardSetDefinition.Chance;
			}
			if (list.Count == 0)
			{
				base.Debug.LogError($"The daily quest set {setDefinition.Id} has an invalid reward set string. Could not select any of the potential reward sets.");
				return;
			}
			int num2 = base.manager.Player.PlayerRandom.Next(num);
			int num3 = 0;
			string text2 = null;
			for (int j = 0; j < list.Count; j++)
			{
				DailyQuestRewardSetDefinition dailyQuestRewardSetDefinition2 = list[j];
				if (num2 >= num3 && num2 < num3 + dailyQuestRewardSetDefinition2.Chance)
				{
					text2 = dailyQuestRewardSetDefinition2.Id;
					break;
				}
				num3 += dailyQuestRewardSetDefinition2.Chance;
			}
			if (text2 == null)
			{
				base.Debug.LogError($"Failed to randomize reward set for daily quest set {setDefinition.Id}.");
				return;
			}
			RewardSetId = text2;
			GenerateQuestsFromList(setDefinition, setDefinition.Q5Definition, 4);
			GenerateQuestsFromList(setDefinition, setDefinition.Q4Definition, 3);
			GenerateQuestsFromList(setDefinition, setDefinition.Q3Definition, 2);
			GenerateQuestsFromList(setDefinition, setDefinition.Q2Definition, 1);
			GenerateQuestsFromList(setDefinition, setDefinition.Q1Definition, 0);
			DailyQuestItemModel dailyQuestItemModel = new DailyQuestItemModel("Static_Quest_Type_CompleteAll", null, 0, 0, 5);
			dailyQuestItemModel.SetManager(base.manager);
			dailyQuestItemModel.Initialize();
			dailyQuestItemModel.Start();
			dailyQuestItemModel.StartQuest(Context, this, null);
			ActiveQuests.Add(dailyQuestItemModel);
			for (int k = 0; k < ActiveQuests.Count; k++)
			{
				ActiveQuests[k].UpdateQuestOnChange(generatingQuests: true);
			}
		}

		private void VerifyQuestSets()
		{
			for (int i = 0; i < base.gameEconomyData.DailyQuestSetDefinitions.Length; i++)
			{
				DailyQuestSetDefinition dailyQuestSetDefinition = base.gameEconomyData.DailyQuestSetDefinitions[i];
				for (int j = 0; j < 5; j++)
				{
					List<DailyQuestSelectionDefinition> list = null;
					switch (j)
					{
					case 0:
						list = dailyQuestSetDefinition.Q1Definition;
						break;
					case 1:
						list = dailyQuestSetDefinition.Q2Definition;
						break;
					case 2:
						list = dailyQuestSetDefinition.Q3Definition;
						break;
					case 3:
						list = dailyQuestSetDefinition.Q4Definition;
						break;
					case 4:
						list = dailyQuestSetDefinition.Q5Definition;
						break;
					}
					for (int k = 0; k < dailyQuestSetDefinition.Q1Definition.Count; k++)
					{
						DailyQuestSelectionDefinition dailyQuestSelectionDefinition = list[k];
						for (int l = 0; l < base.gameEconomyData.DailyQuestDefinitions.Length; l++)
						{
							DailyQuestDefinition dailyQuestDefinition = base.gameEconomyData.DailyQuestDefinitions[l];
							if (!(dailyQuestDefinition.Category != dailyQuestSelectionDefinition.QuestCategory) && dailyQuestDefinition.GetSizeWithIndex((int)dailyQuestSelectionDefinition.Size) <= 0)
							{
								base.Debug.LogError($"The daily quest {dailyQuestDefinition.Id} does not have the size {dailyQuestSelectionDefinition.Size} required by set {dailyQuestSetDefinition.Id}.");
							}
						}
					}
				}
			}
		}

		private void GenerateQuests()
		{
			ActiveQuests.Clear();
			int councilLevel = base.manager.Player.CouncilLevel;
			DailyQuestSetDefinition[] dailyQuestSetDefinitions = base.manager.GameEconomyData.DailyQuestSetDefinitions;
			foreach (DailyQuestSetDefinition dailyQuestSetDefinition in dailyQuestSetDefinitions)
			{
				if (councilLevel >= dailyQuestSetDefinition.CouncilLevelMin && councilLevel <= dailyQuestSetDefinition.CouncilLevelMax)
				{
					GenerateQuestsForSet(dailyQuestSetDefinition);
					break;
				}
			}
			if (ActiveQuests.Count > 0)
			{
				QuestsRefreshCount++;
			}
			NotifyChange("ActiveQuests");
		}

		private void RandomizeQuestChest()
		{
			int num = base.manager.Player.PlayerRandom.Next(base.manager.GameEconomyData.DailyQuestChestDefinitions.Length);
			CurrentQuestChestDefinition = base.manager.GameEconomyData.DailyQuestChestDefinitions[num];
			CurrentQuestChestId = CurrentQuestChestDefinition.Id;
			NotifyChange("CurrentQuestChest");
		}

		public bool TryClaimQuestChest()
		{
			if (!TryInitializeQuests())
			{
				return false;
			}
			if (CurrentQuestChestDefinition != null && QuestPoints >= CurrentQuestChestDefinition.QuestPointsRequired)
			{
				int level = base.manager.Player.Level;
				LootEntry lootEntry = base.manager.Player.LootManager.ShuffleOneLoot(new LootEntryGenParams
				{
					eventType = CurrentQuestChestDefinition.EventType,
					context = CurrentQuestChestDefinition.DropContext,
					tag = CurrentQuestChestDefinition.Tag,
					targetLevel = level,
					random = base.manager.Player.PlayerRandom
				});
				if (lootEntry == null)
				{
					base.Debug.LogError($"Failed to shuffle reward for chest {CurrentQuestChestDefinition.Id}.");
					return false;
				}
				lootEntry.Type = LootEntryType.DailyQuest;
				QuestChestRewards.Add(lootEntry);
				QuestPoints -= CurrentQuestChestDefinition.QuestPointsRequired;
				NotifyChange("QuestPoints");
				RandomizeQuestChest();
				return true;
			}
			return false;
		}

		public LootEntry GiveChestReward()
		{
			if (QuestChestRewards.Count == 0)
			{
				return null;
			}
			LootEntry lootEntry = QuestChestRewards[0];
			QuestChestRewards.RemoveAt(0);
			base.manager.Player.LootManager.GiveLoot(lootEntry);
			base.manager.Metrics.AddFind().AddLoot(lootEntry).AddDailyQuestChest(CurrentQuestChestDefinition.QuestPointsRequired, CurrentQuestChestDefinition.Id)
				.AddLootCrate(lootEntry)
				.Send();
			return lootEntry;
		}

		public bool TryClaimQuest(int questModelId)
		{
			if (!TryInitializeQuests())
			{
				return false;
			}
			DailyQuestItemModel model = base.manager.GetModel<DailyQuestItemModel>(questModelId);
			int questPointsGained = 0;
			model.TryClaimQuest(out questPointsGained);
			if (questPointsGained > 0)
			{
				QuestPoints += questPointsGained;
				NotifyChange("QuestPoints");
				for (int i = 0; i < ActiveQuests.Count; i++)
				{
					ActiveQuests[i].UpdateQuestOnChange();
				}
				return true;
			}
			return false;
		}

		public void StartNewbieSenvenQuests()
		{
			foreach (List<DailyQuestItemModel> quest in base.manager.Player.NewbieSenvenQuest.Quests)
			{
				for (int i = 0; i < quest.Count; i++)
				{
					DailyQuestItemModel dailyQuestItemModel = quest[i];
					if (dailyQuestItemModel.ModelId <= 0)
					{
						dailyQuestItemModel.SetManager(base.manager);
						dailyQuestItemModel.Initialize();
						dailyQuestItemModel.Start();
					}
					if (i < quest.Count - 1)
					{
						dailyQuestItemModel.StartQuest(Context, this, allDailyQuestRules[dailyQuestItemModel.Id]);
						continue;
					}
					dailyQuestItemModel.StartQuest(Context, this, null);
					dailyQuestItemModel.UpdateQuestOnChange(generatingQuests: true);
				}
			}
		}
	}
}
