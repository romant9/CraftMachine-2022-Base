using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ReturnRepeatQuestModel : TWDModelObject
	{
		public const string ReturnRepeatQuestChanged = "ReturnRepeatQuestChanged";

		public Dictionary<int, int> ClaimCounts { get; set; }

		public ModelList<ReturnRepeatQuestItemModel> Tasks { get; private set; }

		[JsonIgnore]
		public bool HasTask
		{
			get
			{
				if (Tasks != null && Tasks.Count > 0)
				{
					TWDModelManager tWDModelManager = base.manager;
					if (tWDModelManager == null)
					{
						return false;
					}
					return tWDModelManager.Player?.ReturnActivityManager?.IsReturnActivityAvailable() == true;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool CanClaimAnyReward
		{
			get
			{
				if (!HasTask)
				{
					return false;
				}
				for (int i = 0; i < Tasks.Count; i++)
				{
					if (CanClaimTask(Tasks[i]))
					{
						return true;
					}
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool HasRedDot => CanClaimAnyReward;

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
			EnsureRuntimeState();
		}

		public override void Start()
		{
			EnsureRuntimeState();
			base.Start();
			ensureCurrentTasks();
		}

		public void ResetForNewActivity()
		{
			EnsureRuntimeState();
			ClaimCounts.Clear();
			Tasks.Clear();
			if (ensureCurrentTasks())
			{
				NotifyChange("ReturnRepeatQuestChanged");
			}
		}

		public bool OnCouncilLevelUp()
		{
			bool num = ensureCurrentTasks();
			if (num)
			{
				NotifyChange("ReturnRepeatQuestChanged");
			}
			return num;
		}

		public bool OnLogin(long currentTimestamp)
		{
			if (currentTimestamp <= 0)
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnRepeatQuestItemModel returnRepeatQuestItemModel = Tasks[i];
				if (returnRepeatQuestItemModel != null && !IsClaimLimitReached(returnRepeatQuestItemModel))
				{
					ReturnRepeatQuestDefinition definition = returnRepeatQuestItemModel.Definition;
					if (definition != null && definition.QuestType == ReturnQuestType.DailyLogin)
					{
						returnRepeatQuestItemModel.CurrentProgress++;
						flag = true;
					}
				}
			}
			if (flag)
			{
				NotifyChange("ReturnRepeatQuestChanged");
			}
			return flag;
		}

		public bool OnCurrencySpent(CurrencyType currencyType, int amount)
		{
			if (amount <= 0)
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnRepeatQuestItemModel returnRepeatQuestItemModel = Tasks[i];
				if (returnRepeatQuestItemModel != null && !IsClaimLimitReached(returnRepeatQuestItemModel))
				{
					ReturnRepeatQuestDefinition definition = returnRepeatQuestItemModel.Definition;
					ReturnQuestType questType = definition?.QuestType ?? ReturnQuestType.None;
					if (definition != null && ReturnQuestRuleHelper.TryGetCurrencyType(questType, out var currencyType2) && currencyType2 == currencyType)
					{
						returnRepeatQuestItemModel.CurrentProgress += amount;
						flag = true;
					}
				}
			}
			if (flag)
			{
				NotifyChange("ReturnRepeatQuestChanged");
			}
			return flag;
		}

		public bool OnItemUpgraded(ReturnQuestType upgradeQuestType)
		{
			if (!upgradeQuestType.IsUpgradeQuest())
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnRepeatQuestItemModel returnRepeatQuestItemModel = Tasks[i];
				if (returnRepeatQuestItemModel != null && !IsClaimLimitReached(returnRepeatQuestItemModel))
				{
					ReturnRepeatQuestDefinition definition = returnRepeatQuestItemModel.Definition;
					if (definition != null && definition.QuestType == upgradeQuestType)
					{
						returnRepeatQuestItemModel.CurrentProgress++;
						flag = true;
					}
				}
			}
			if (flag)
			{
				NotifyChange("ReturnRepeatQuestChanged");
			}
			return flag;
		}

		public bool OnMissionCompleted()
		{
			bool flag = false;
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnRepeatQuestItemModel returnRepeatQuestItemModel = Tasks[i];
				if (returnRepeatQuestItemModel != null && !IsClaimLimitReached(returnRepeatQuestItemModel))
				{
					ReturnRepeatQuestDefinition definition = returnRepeatQuestItemModel.Definition;
					if (definition != null && definition.QuestType == ReturnQuestType.CompleteMission)
					{
						returnRepeatQuestItemModel.CurrentProgress++;
						flag = true;
					}
				}
			}
			if (flag)
			{
				NotifyChange("ReturnRepeatQuestChanged");
			}
			return flag;
		}

		public bool OnWalkersKilled(int amount)
		{
			if (amount <= 0)
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnRepeatQuestItemModel returnRepeatQuestItemModel = Tasks[i];
				if (returnRepeatQuestItemModel != null && !IsClaimLimitReached(returnRepeatQuestItemModel))
				{
					ReturnRepeatQuestDefinition definition = returnRepeatQuestItemModel.Definition;
					if (definition != null && definition.QuestType == ReturnQuestType.KillWalkers)
					{
						returnRepeatQuestItemModel.CurrentProgress += amount;
						flag = true;
					}
				}
			}
			if (flag)
			{
				NotifyChange("ReturnRepeatQuestChanged");
			}
			return flag;
		}

		public bool TryClaimReward()
		{
			for (int i = 0; i < Tasks.Count; i++)
			{
				if (TryClaimReward(Tasks[i]?.DefinitionId ?? 0))
				{
					return true;
				}
			}
			return false;
		}

		public bool TryClaimReward(int definitionId)
		{
			ReturnRepeatQuestItemModel returnRepeatQuestItemModel = Tasks?.Find((ReturnRepeatQuestItemModel x) => x.DefinitionId == definitionId);
			if (!CanClaimTask(returnRepeatQuestItemModel) || returnRepeatQuestItemModel.Definition?.RewardEntries == null)
			{
				return false;
			}
			returnRepeatQuestItemModel.Definition.RewardEntries.Give(base.manager);
			ClaimCounts.TryGetValue(definitionId, out var value);
			ClaimCounts[definitionId] = value + 1;
			returnRepeatQuestItemModel.CurrentProgress = GetRemainingProgressAfterClaim(returnRepeatQuestItemModel);
			ensureCurrentTasks();
			ReturnerAnalytics.SendTask(base.manager, definitionId);
			NotifyChange("ReturnRepeatQuestChanged");
			return true;
		}

		public int GetRemainingCount(int definitionId)
		{
			ReturnRepeatQuestDefinition returnRepeatQuestDefinition = base.gameEconomyData?.GetReturnRepeatQuestDefinition(definitionId);
			if (returnRepeatQuestDefinition == null)
			{
				return 0;
			}
			ClaimCounts.TryGetValue(definitionId, out var value);
			if (returnRepeatQuestDefinition.Time < 0)
			{
				return -1;
			}
			return Math.Max(returnRepeatQuestDefinition.Time - value, 0);
		}

		private bool ensureCurrentTasks()
		{
			TWDModelManager tWDModelManager = base.manager;
			if (tWDModelManager == null || tWDModelManager.Player?.ReturnActivityManager?.IsReturnActivityAvailable() != true)
			{
				return false;
			}
			List<ReturnRepeatQuestDefinition> availableDefinitions = GetAvailableDefinitions();
			if (availableDefinitions.Count == 0)
			{
				bool result = Tasks.Count > 0;
				Tasks.Clear();
				return result;
			}
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnRepeatQuestItemModel returnRepeatQuestItemModel = Tasks[i];
				if (returnRepeatQuestItemModel != null)
				{
					dictionary[returnRepeatQuestItemModel.DefinitionId] = returnRepeatQuestItemModel.CurrentProgress;
				}
			}
			bool flag = Tasks.Count != availableDefinitions.Count;
			if (!flag)
			{
				for (int j = 0; j < availableDefinitions.Count; j++)
				{
					if (Tasks[j]?.DefinitionId != availableDefinitions[j].Id)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				Tasks.Clear();
				for (int k = 0; k < availableDefinitions.Count; k++)
				{
					ReturnRepeatQuestDefinition returnRepeatQuestDefinition = availableDefinitions[k];
					if (returnRepeatQuestDefinition != null)
					{
						ReturnRepeatQuestItemModel returnRepeatQuestItemModel2 = new ReturnRepeatQuestItemModel(returnRepeatQuestDefinition.Id);
						returnRepeatQuestItemModel2.SetManager(base.manager);
						returnRepeatQuestItemModel2.Initialize();
						if (dictionary.TryGetValue(returnRepeatQuestDefinition.Id, out var value))
						{
							returnRepeatQuestItemModel2.CurrentProgress = value;
						}
						if (base.ModelId != 0)
						{
							returnRepeatQuestItemModel2.Start();
						}
						Tasks.Add(returnRepeatQuestItemModel2);
					}
				}
			}
			return flag;
		}

		private List<ReturnRepeatQuestDefinition> GetAvailableDefinitions()
		{
			List<ReturnRepeatQuestDefinition> list = base.gameEconomyData?.GetReturnRepeatQuestDefinitions(GetCurrentCouncilLevel()) ?? new List<ReturnRepeatQuestDefinition>();
			List<ReturnRepeatQuestDefinition> list2 = new List<ReturnRepeatQuestDefinition>();
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i < Tasks.Count; i++)
			{
				ReturnRepeatQuestItemModel returnRepeatQuestItemModel = Tasks[i];
				if (returnRepeatQuestItemModel?.Definition != null && returnRepeatQuestItemModel.Definition.Time != 0)
				{
					list2.Add(returnRepeatQuestItemModel.Definition);
					hashSet.Add(returnRepeatQuestItemModel.DefinitionId);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				ReturnRepeatQuestDefinition returnRepeatQuestDefinition = list[j];
				if (returnRepeatQuestDefinition != null && returnRepeatQuestDefinition.Time != 0 && !hashSet.Contains(returnRepeatQuestDefinition.Id))
				{
					list2.Add(returnRepeatQuestDefinition);
					hashSet.Add(returnRepeatQuestDefinition.Id);
				}
			}
			return list2;
		}

		private void EnsureRuntimeState()
		{
			if (Tasks == null)
			{
				Tasks = new ModelList<ReturnRepeatQuestItemModel>();
				Tasks.SetManager(base.manager);
				Tasks.Initialize();
			}
			if (ClaimCounts == null)
			{
				ClaimCounts = new Dictionary<int, int>();
			}
		}

		private bool IsCompleted(ReturnRepeatQuestItemModel task)
		{
			if (task != null && HasTask)
			{
				return task.CurrentProgress >= GetRequiredAmount(task);
			}
			return false;
		}

		private bool CanClaimTask(ReturnRepeatQuestItemModel task)
		{
			if (task != null && HasTask && IsCompleted(task))
			{
				return GetRemainingCount(task.DefinitionId) != 0;
			}
			return false;
		}

		private bool IsClaimLimitReached(ReturnRepeatQuestItemModel task)
		{
			if (task != null)
			{
				return GetRemainingCount(task.DefinitionId) == 0;
			}
			return true;
		}

		private int GetRequiredAmount(ReturnRepeatQuestItemModel task)
		{
			if (task?.Definition != null)
			{
				return ReturnQuestRuleHelper.GetRequiredAmount(task.Definition.Params);
			}
			return 0;
		}

		private int GetRemainingProgressAfterClaim(ReturnRepeatQuestItemModel task)
		{
			if (task == null)
			{
				return 0;
			}
			int requiredAmount = GetRequiredAmount(task);
			if (requiredAmount <= 0)
			{
				return 0;
			}
			return Math.Max(task.CurrentProgress - requiredAmount, 0);
		}

		private int GetCurrentCouncilLevel()
		{
			if (base.manager?.Player == null)
			{
				return 0;
			}
			return base.manager.Player.CouncilLevel;
		}
	}
}
