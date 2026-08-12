using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class DailyQuestItemModel : TWDModelObject
	{
		public const string CompleteAllQuestId = "Static_Quest_Type_CompleteAll";

		[JsonIgnore]
		private ModelRandom rewardsRandom;

		[JsonIgnore]
		public bool IsCompleteAllQuest => Id == "Static_Quest_Type_CompleteAll";

		[JsonIgnore]
		public bool IsNewbieSevenQuest => SlotIndex > 10;

		public string Id { get; set; }

		public string QuestSetId { get; set; }

		[JsonIgnore]
		public DailyQuestDefinition Definition { get; private set; }

		[JsonIgnore]
		public DailyQuestRule Rule { get; private set; }

		[JsonIgnore]
		public Rewards Rewards { get; private set; }

		public int RewardsRandomSeed { get; set; }

		public int SlotIndex { get; set; }

		public int PlayerLevel { get; set; }

		public int CompletedCount { get; private set; }

		public int CompletionPersonalCap { get; private set; }

		public int CompletionTotalCap { get; private set; }

		public int CompletedInWindow { get; private set; }

		public bool Claimed { get; private set; }

		[JsonIgnore]
		public int SortOrder
		{
			get
			{
				if (Claimed)
				{
					return 3;
				}
				if (IsCompleteAllQuest)
				{
					return 2;
				}
				if (IsCompleted && !Claimed)
				{
					return 0;
				}
				return 1;
			}
		}

		[JsonIgnore]
		public DailyQuestModel DailyQuestManager { get; private set; }

		public bool IsCompleted => CompletedCount >= CompletionTotalCap;

		public bool CompletedCountSeen { get; set; }

		public bool ClaimedSeen { get; set; }

		[JsonIgnore]
		public string DisplayName
		{
			get
			{
				if (IsCompleteAllQuest)
				{
					return "DailyQuest.CompleteAllQuests.Label";
				}
				if (Definition == null)
				{
					return "";
				}
				return Definition.DisplayName;
			}
		}

		[JsonIgnore]
		public string DisplayDescription
		{
			get
			{
				if (IsCompleteAllQuest)
				{
					return "DailyQuest.CompleteAllQuests.Description";
				}
				if (Definition == null)
				{
					return "";
				}
				return Definition.DisplayDescription;
			}
		}

		public void ForceComplete()
		{
			if (!IsCompleteAllQuest)
			{
				int delta = CompletionPersonalCap - CompletedCount;
				CompletedCount = CompletionPersonalCap;
				CompletedCountSeen = false;
				NotifyChange("CompletedCount");
				CompletedInWindow = 0;
				ReportAnalyticsProgress(delta);
			}
		}

		public void ResetIfInWindow(DailyQuestCompletionWindow window)
		{
			if (Definition != null && Definition.CompletionWindow == window)
			{
				CompletedInWindow = 0;
			}
		}

		public void CommitWindow(DailyQuestCompletionWindow window)
		{
			if (Definition != null && Definition.CompletionWindow == window)
			{
				if (CompletedInWindow >= Definition.CompletionMinCountInWindow && CompletedCount < CompletionPersonalCap)
				{
					int completedCount = CompletedCount + 1;
					CompletedCount = completedCount;
					CompletedCountSeen = false;
					NotifyChange("CompletedCount");
					ReportAnalyticsProgress(1);
				}
				CompletedInWindow = 0;
			}
		}

		public DailyQuestItemModel()
		{
		}

		public DailyQuestItemModel(string id, string questSetId, int completionPersonalCap, int completionTotalCap, int slotIndex)
		{
			Id = id;
			QuestSetId = questSetId;
			CompletionPersonalCap = completionPersonalCap;
			CompletionTotalCap = completionTotalCap;
			CompletedCountSeen = true;
			SlotIndex = slotIndex;
		}

		public override void Initialize()
		{
			base.Initialize();
			RewardsRandomSeed = base.manager.Player.PlayerRandom.State;
			PlayerLevel = base.manager.Player.Level;
		}

		private void ReportAnalyticsProgress(int delta)
		{
			base.manager.Metrics.AddProgress().AddNewDailyQuest(this, QuestSetId, SlotIndex, delta).Send();
		}

		public void UpdateWithContext(QuestCompleteContext context)
		{
			if (Rule != null && Rule.CountsTowardsCompletionRuleCheck != null && Rule.CountsTowardsCompletionRuleCheck.Evaluate(context) != 0L && CompletedCount < CompletionPersonalCap)
			{
				if (Definition != null && Definition.CompletionWindow != DailyQuestCompletionWindow.None)
				{
					int completedInWindow = CompletedInWindow + 1;
					CompletedInWindow = completedInWindow;
				}
				else
				{
					int completedInWindow = CompletedCount + 1;
					CompletedCount = completedInWindow;
					CompletedCountSeen = false;
					ReportAnalyticsProgress(1);
				}
				if (IsCompleted)
				{
					base.Debug.LogDebug($"Completed quest {Id}");
				}
				else
				{
					base.Debug.LogDebug($"Completed part of quest {Id} {CompletedCount}/{CompletionTotalCap}");
				}
				NotifyChange("CompletedCount");
			}
		}

		private void ClaimQuest()
		{
			if (Rewards != null)
			{
				Rewards.Give(base.manager);
				for (int i = 0; i < Rewards.RewardsList.Count; i++)
				{
					IReward rewardAt = Rewards.GetRewardAt(i);
					if (!(rewardAt is RewardCurrency))
					{
						if (!(rewardAt is RewardEquipment rewardEquipment))
						{
							if (rewardAt is RewardTimedBonus rewardTimedBonus)
							{
								base.manager.Metrics.AddFind().AddTimedBonus(rewardTimedBonus).AddNewDailyQuest(this, QuestSetId, SlotIndex, 0, DetermineQuestPointsFromComplete())
									.Send();
							}
						}
						else
						{
							EquipmentItemModel equipment = base.manager.Player.Equipment.GenerateAndInitializeEquipmentFromDefinition(rewardEquipment.EquipmentId);
							base.manager.Metrics.AddFind().AddEquipment(equipment, "Equipment", rewardEquipment.Amount).AddNewDailyQuest(this, QuestSetId, SlotIndex, 0, DetermineQuestPointsFromComplete())
								.Send();
						}
					}
					else
					{
						base.manager.Metrics.AddFind().AddReward(rewardAt).AddNewDailyQuest(this, QuestSetId, SlotIndex, 0, DetermineQuestPointsFromComplete())
							.Send();
					}
				}
			}
			else
			{
				base.manager.Metrics.AddFind().AddReward(null).AddNewDailyQuest(this, QuestSetId, SlotIndex, 0, DetermineQuestPointsFromComplete())
					.Send();
			}
		}

		public void UpdateQuestOnChange(bool generatingQuests = false)
		{
			if (!IsCompleteAllQuest)
			{
				return;
			}
			if (IsNewbieSevenQuest)
			{
				int num = SlotIndex / 10;
				int num2 = base.manager.Player.NewbieSenvenQuest.Quests[num - 1].Count - 1;
				int num3 = base.manager.Player.NewbieSenvenQuest.Quests[num - 1].Where((DailyQuestItemModel x) => !x.IsCompleteAllQuest).Count((DailyQuestItemModel x) => x.Claimed);
				if (num2 != CompletionPersonalCap || CompletedCount != num3)
				{
					int completionPersonalCap = (CompletionTotalCap = num2);
					CompletionPersonalCap = completionPersonalCap;
					CompletedCount = num3;
					if (!generatingQuests)
					{
						CompletedCountSeen = false;
						NotifyChange("CompletedCount");
					}
				}
				return;
			}
			int count = DailyQuestManager.ActiveQuests.Count;
			int num5 = 0;
			for (int num6 = 0; num6 < count; num6++)
			{
				DailyQuestItemModel dailyQuestItemModel = DailyQuestManager.ActiveQuests[num6];
				if (dailyQuestItemModel != this && dailyQuestItemModel.Claimed)
				{
					num5++;
				}
			}
			count--;
			if (count != CompletionPersonalCap || CompletedCount != num5)
			{
				int completionPersonalCap = (CompletionTotalCap = count);
				CompletionPersonalCap = completionPersonalCap;
				CompletedCount = num5;
				if (!generatingQuests)
				{
					CompletedCountSeen = false;
					NotifyChange("CompletedCount");
				}
			}
		}

		public int DetermineQuestPointsFromComplete()
		{
			if (IsCompleteAllQuest)
			{
				if (IsNewbieSevenQuest)
				{
					int id = SlotIndex / 10;
					NewbieSevenQuest newbieSenvenQuest = base.manager.GameEconomyData.GetNewbieSenvenQuest(id);
					if (newbieSenvenQuest != null)
					{
						return newbieSenvenQuest.PointsFromFinishAll;
					}
				}
				DailyQuestRewardSetDefinition dailyQuestRewardSetDefinition = base.manager.GameEconomyData.GetDailyQuestRewardSetDefinition(DailyQuestManager.RewardSetId);
				if (dailyQuestRewardSetDefinition != null)
				{
					return dailyQuestRewardSetDefinition.PointsFromFinishAll;
				}
			}
			return 1;
		}

		public void TryClaimQuest(out int questPointsGained)
		{
			questPointsGained = 0;
			if (!Claimed && CompletedCount >= CompletionTotalCap)
			{
				ClaimQuest();
				Claimed = true;
				ClaimedSeen = false;
				questPointsGained = DetermineQuestPointsFromComplete();
			}
		}

		private void GenerateRewards(DailyQuestModel dailyQuests)
		{
			GameEconomyData gameEconomyData = base.manager.GameEconomyData;
			if (gameEconomyData.DailyQuestRewardDefinitions == null || gameEconomyData.DailyQuestRewardDefinitions.Length == 0)
			{
				base.Debug.LogError("No daily quest rewards have been defined in the GED.");
				return;
			}
			if (string.IsNullOrEmpty(dailyQuests.RewardSetId))
			{
				base.Debug.LogError("No reward set defined for daily quests.");
				return;
			}
			DailyQuestRewardSetDefinition dailyQuestRewardSetDefinition = base.manager.GameEconomyData.GetDailyQuestRewardSetDefinition(dailyQuests.RewardSetId);
			if (dailyQuestRewardSetDefinition == null)
			{
				base.Debug.LogError($"Could not find reward for quest {Id}. Reward set ID {dailyQuests.RewardSetId}.");
				return;
			}
			string text = null;
			switch (SlotIndex)
			{
			case 0:
				text = dailyQuestRewardSetDefinition.Q1;
				break;
			case 1:
				text = dailyQuestRewardSetDefinition.Q2;
				break;
			case 2:
				text = dailyQuestRewardSetDefinition.Q3;
				break;
			case 3:
				text = dailyQuestRewardSetDefinition.Q4;
				break;
			case 4:
				text = dailyQuestRewardSetDefinition.Q5;
				break;
			}
			if (text == null)
			{
				base.Debug.LogError($"Could not find reward for for daily quest {Id} with slot index {SlotIndex}.");
			}
			else
			{
				Rewards = new Rewards(text, base.manager, PlayerLevel, EquipmentSource.Unknown, rewardsRandom);
			}
		}

		private void GenerateNewbieSenvenRewards()
		{
			GameEconomyData obj = base.manager.GameEconomyData;
			int id = SlotIndex / 10;
			int num = SlotIndex % 10;
			NewbieSevenQuest newbieSenvenQuest = obj.GetNewbieSenvenQuest(id);
			string text = null;
			switch (num)
			{
			case 1:
				text = newbieSenvenQuest.Reward1;
				break;
			case 2:
				text = newbieSenvenQuest.Reward2;
				break;
			case 3:
				text = newbieSenvenQuest.Reward3;
				break;
			case 4:
				text = newbieSenvenQuest.Reward4;
				break;
			case 5:
				text = newbieSenvenQuest.Reward5;
				break;
			}
			if (text == null)
			{
				base.Debug.LogError($"Could not find reward for for daily quest {Id} with slot index {SlotIndex}.");
			}
			else
			{
				Rewards = new Rewards(text, base.manager, PlayerLevel, EquipmentSource.Unknown, rewardsRandom);
			}
		}

		public void StartQuest(QuestCompleteContext context, DailyQuestModel dailyQuests, DailyQuestRule rule)
		{
			DailyQuestManager = dailyQuests;
			if (!IsCompleteAllQuest)
			{
				Definition = base.manager.GameEconomyData.GetDailyQuestDefinition(Id);
				Rule = rule;
				if (!IsNewbieSevenQuest)
				{
					GenerateRewards(dailyQuests);
				}
				else
				{
					GenerateNewbieSenvenRewards();
				}
			}
		}

		public override void Start()
		{
			base.Start();
			rewardsRandom = new ModelRandom(RewardsRandomSeed);
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
