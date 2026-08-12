using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TWDModel
{
	public class NewbieSevenQuestModel : TWDModelObject
	{
		public const int SlotMul = 10;

		public long StartTime { get; set; }

		public long LastRefreshTime { get; set; }

		public List<List<DailyQuestItemModel>> Quests { get; set; }

		public int QuestPoints { get; set; }

		public int UnlockDay { get; set; }

		public List<int> HadRewardedStage { get; set; }

		[JsonIgnore]
		public bool IsOpen => StartTime + base.manager.GameEconomyData.ConfigData.NewbieSevenQuestDuration > base.manager.Player.UtcTimeStamp;

		public override void Initialize()
		{
			base.Initialize();
			QuestPoints = 0;
			UnlockDay = 0;
			StartTime = 0L;
			Quests = new List<List<DailyQuestItemModel>>();
			LastRefreshTime = 0L;
			HadRewardedStage = new List<int>();
		}

		public void OnCouncilLevelUp(int level)
		{
			if (StartTime <= 0 && level == base.manager.GameEconomyData.ConfigData.NewbieCouncilUnlock && base.manager.GameEconomyData.NewbieSevenQuests != null)
			{
				long lastRefreshTime = (StartTime = base.manager.Player.UtcTimeStamp);
				LastRefreshTime = lastRefreshTime;
				UnlockDay = 1;
				for (int i = 0; i < base.manager.GameEconomyData.NewbieSevenQuests.Length; i++)
				{
					List<DailyQuestItemModel> list = new List<DailyQuestItemModel>();
					NewbieSevenQuest newbieSevenQuest = base.manager.GameEconomyData.NewbieSevenQuests[i];
					list.Add(GenerateQuest(newbieSevenQuest.Q1, GetSlotIndex(i + 1, 1)));
					list.Add(GenerateQuest(newbieSevenQuest.Q2, GetSlotIndex(i + 1, 2)));
					list.Add(GenerateQuest(newbieSevenQuest.Q3, GetSlotIndex(i + 1, 3)));
					list.Add(GenerateQuest(newbieSevenQuest.Q4, GetSlotIndex(i + 1, 4)));
					list.Add(GenerateQuest(newbieSevenQuest.Q5, GetSlotIndex(i + 1, 5)));
					DailyQuestItemModel dailyQuestItemModel = new DailyQuestItemModel("Static_Quest_Type_CompleteAll", null, 0, 0, GetSlotIndex(i + 1, 6));
					dailyQuestItemModel.SetManager(base.manager);
					dailyQuestItemModel.Initialize();
					dailyQuestItemModel.Start();
					list.Add(dailyQuestItemModel);
					Quests.Add(list);
				}
				base.manager.Player.DailyQuestManager.StartNewbieSenvenQuests();
				base.Debug.LogInfo($"NewbieSevenQuest Started UnlockDay :{UnlockDay},LastRefreshTime:{LastRefreshTime},StartTime:{StartTime}");
			}
		}

		public long GetDayUnlockLeftTime(int day)
		{
			if (day <= UnlockDay)
			{
				return 0L;
			}
			long num = (day - UnlockDay) * base.manager.GameEconomyData.ConfigData.NewbieSevenQuestRefresh;
			long num2 = base.manager.Player.UtcTimeStamp - LastRefreshTime;
			return num - num2;
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (IsOpen)
			{
				long num = base.manager.Player.UtcTimeStamp - LastRefreshTime;
				long newbieSevenQuestRefresh = base.manager.GameEconomyData.ConfigData.NewbieSevenQuestRefresh;
				int num2 = (int)(num / newbieSevenQuestRefresh);
				if (num2 > 0)
				{
					UnlockDay += num2;
					LastRefreshTime += base.manager.GameEconomyData.ConfigData.NewbieSevenQuestRefresh * num2;
					base.Debug.LogInfo($"NewbieSevenQuest UnlockDay :{UnlockDay},LastRefreshTime:{LastRefreshTime},PassedTime:{num},refreshTime:{newbieSevenQuestRefresh}");
				}
			}
		}

		private int GetSlotIndex(int day, int slot)
		{
			return day * 10 + slot;
		}

		private DailyQuestItemModel GenerateQuest(string questInfo, int slotIndex)
		{
			string[] array = questInfo.Split(';');
			DailyQuestDefinition dailyQuestDefinition = base.manager.GameEconomyData.GetDailyQuestDefinition(array[0]);
			DailyQuestDefinitionSize dailyQuestDefinitionSize = (DailyQuestDefinitionSize)Enum.Parse(typeof(DailyQuestDefinitionSize), array[1]);
			int index = (int)dailyQuestDefinitionSize;
			int num = dailyQuestDefinition.GetSizeWithIndex(index);
			if (num < 0)
			{
				base.Debug.LogError($"Quest with ID {dailyQuestDefinition.Id} does not define size {dailyQuestDefinitionSize}.");
				num = 1;
			}
			DailyQuestItemModel dailyQuestItemModel = new DailyQuestItemModel(dailyQuestDefinition.Id, string.Empty, num, num, slotIndex);
			dailyQuestItemModel.SetManager(base.manager);
			dailyQuestItemModel.Initialize();
			dailyQuestItemModel.Start();
			return dailyQuestItemModel;
		}

		public void UpdateWithContext(QuestCompleteContext context)
		{
			if (!IsOpen)
			{
				return;
			}
			for (int i = 0; i < Quests.Count && i < UnlockDay; i++)
			{
				foreach (DailyQuestItemModel item in Quests[i])
				{
					item.UpdateWithContext(context);
				}
			}
		}

		public void CommitWindow(DailyQuestCompletionWindow window)
		{
			if (!IsOpen)
			{
				return;
			}
			for (int i = 0; i < Quests.Count && i < UnlockDay; i++)
			{
				foreach (DailyQuestItemModel item in Quests[i])
				{
					item.CommitWindow(window);
				}
			}
		}

		public void ResetIfInWindow(DailyQuestCompletionWindow window)
		{
			if (!IsOpen)
			{
				return;
			}
			for (int i = 0; i < Quests.Count && i < UnlockDay; i++)
			{
				foreach (DailyQuestItemModel item in Quests[i])
				{
					item.ResetIfInWindow(window);
				}
			}
		}

		public override bool IsValid()
		{
			return true;
		}

		public void CompleteAllNewbieQuests(int day)
		{
			if (day <= UnlockDay && day > 0)
			{
				for (int i = 0; i < Quests[day - 1].Count; i++)
				{
					Quests[day - 1][i].ForceComplete();
				}
				for (int j = 0; j < Quests[day - 1].Count; j++)
				{
					Quests[day - 1][j].UpdateQuestOnChange();
				}
			}
		}

		public bool TryClaimQuest(int slotIndex)
		{
			if (!IsOpen)
			{
				return false;
			}
			int num = slotIndex / 10;
			DailyQuestItemModel dailyQuestItemModel = Quests[num - 1].Find((DailyQuestItemModel x) => x.SlotIndex == slotIndex);
			int questPointsGained = 0;
			base.manager.Metrics.ResourceChangeUsedReason = "NewbieSevenDay";
			dailyQuestItemModel.TryClaimQuest(out questPointsGained);
			if (questPointsGained > 0)
			{
				QuestPoints += questPointsGained;
				NotifyChange("QuestPoints");
				Quests[num - 1].Last().UpdateQuestOnChange();
				return true;
			}
			return false;
		}

		public bool TryClaimStageReward(int point)
		{
			NewbieStageReward newbieSenvenStageReward = base.manager.GameEconomyData.GetNewbieSenvenStageReward(point);
			if (!IsOpen)
			{
				return false;
			}
			if (newbieSenvenStageReward == null)
			{
				return false;
			}
			if (point > QuestPoints)
			{
				return false;
			}
			if (HadRewardedStage.Contains(point))
			{
				return false;
			}
			HadRewardedStage.Add(point);
			newbieSenvenStageReward.RewardEntries.Give(base.manager);
			return true;
		}
	}
}
