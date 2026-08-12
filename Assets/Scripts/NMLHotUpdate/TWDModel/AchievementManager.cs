using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class AchievementManager
	{
		public List<Achievement> Achievements;

		private Dictionary<string, Achievement> AchievementsById;

		public PlayerModel Player { get; private set; }

		public GameEconomyData GED => Player.manager.GameEconomyData;

		public BlackboardModel Blackboard => Player.Blackboard;

		public bool HasNewAchievement
		{
			get
			{
				for (int i = 0; i < Achievements.Count; i++)
				{
					if (Achievements[i].ViewState == AchievementViewState.New)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool HasNewQuest
		{
			get
			{
				List<DailyQuest> dailyQuests = Player.DailyQuests;
				if (dailyQuests != null)
				{
					for (int i = 0; i < dailyQuests.Count; i++)
					{
						if (dailyQuests[i].ViewState == AchievementViewState.New)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public bool HasAchievementCompleted
		{
			get
			{
				for (int i = 0; i < Achievements.Count; i++)
				{
					if (Achievements[i].ViewState == AchievementViewState.Completed)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool HasQuestCompleted
		{
			get
			{
				List<DailyQuest> dailyQuests = Player.DailyQuests;
				if (dailyQuests != null)
				{
					for (int i = 0; i < dailyQuests.Count; i++)
					{
						if (dailyQuests[i].ViewState == AchievementViewState.Completed)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public bool CanDiscardDailyQuest
		{
			get
			{
				int num = (int)((Player.manager.Time - Player.LastDailyQuestDiscardTime) / 1000);
				if (Player.LastDailyQuestDiscardTime != 0L)
				{
					return num >= Player.gameEconomyData.ConfigData.DailyQuestDiscardCooldown;
				}
				return true;
			}
		}

		public bool CanCreateNewDailyQuest
		{
			get
			{
				int num = (int)((Player.manager.Time - Player.LastDailyQuestCreationTime) / 1000);
				if (Player.LastDailyQuestCreationTime != 0L)
				{
					return num >= Player.gameEconomyData.ConfigData.DailyQuestSpawnInterval;
				}
				return true;
			}
		}

		public event AchievementsChangedHandler OnAchievementsChanged;

		public event DailyQuestsChangedHandler OnDailyQuestsChanged;

		public AchievementManager(PlayerModel player)
		{
			Player = player;
			Achievements = new List<Achievement>();
			AchievementsById = new Dictionary<string, Achievement>();
			StartDailyQuests();
		}

		private void NotifyAchievementsChanged()
		{
			this.OnAchievementsChanged?.Invoke();
		}

		private void NotifyDailyQuestsChanged()
		{
			this.OnDailyQuestsChanged?.Invoke();
		}

		public void MarkChallengeBonusValidity()
		{
			if (Player.DailyQuests != null)
			{
				for (int i = 0; i < Player.DailyQuests.Count; i++)
				{
					DailyQuest dailyQuest = Player.DailyQuests[i];
					dailyQuest.CanGiveChallengeBonus = dailyQuest.ChallengeBonusStars > 0 && !dailyQuest.IsCompleted;
				}
			}
		}

		public int GetQuestChallengeBonusStars()
		{
			int num = 0;
			if (Player.DailyQuests != null)
			{
				for (int i = 0; i < Player.DailyQuests.Count; i++)
				{
					DailyQuest dailyQuest = Player.DailyQuests[i];
					if (dailyQuest.IsValidForBonusStars && dailyQuest.CanGiveChallengeBonus)
					{
						num++;
					}
				}
			}
			return num;
		}

		private void InstantiateAchievementClasses()
		{
			for (int i = 0; i < GED.AchievementDefinitions.Length; i++)
			{
				AchievementDefinition achievementDefinition = GED.AchievementDefinitions[i];
				if (achievementDefinition.AchievementType == AchievementType.Achievement && GetAchievement(achievementDefinition) == null)
				{
					AchievementDefinition achievementDefinition2 = (achievementDefinition.HasDependsOn ? Player.gameEconomyData.GetAchievementDefinition(achievementDefinition.DependsOn) : null);
					if (achievementDefinition2 == null || Blackboard.IsToggleOn(achievementDefinition2.BlackboardRewardClaimedKey))
					{
						InstantiateAchievementClass(achievementDefinition);
					}
				}
			}
		}

		private void InstantiateAchievementClass(AchievementDefinition achievementDefinition)
		{
			Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(Achievement), achievementDefinition.Class);
			if (!(type != null))
			{
				return;
			}
			object obj = null;
			try
			{
				obj = Activator.CreateInstance(type);
			}
			catch (Exception)
			{
			}
			if (obj != null)
			{
				Achievement achievement = obj as Achievement;
				achievement.AchievementDefinitionID = achievementDefinition.ID;
				achievement.Player = Player;
				achievement.Initialize();
				AddAchievement(achievement);
				if (!Blackboard.HasCounter(achievementDefinition.BlackboardCounterKey))
				{
					Blackboard.SetCounter(achievementDefinition.BlackboardCounterKey, 0);
					achievement.ViewState = AchievementViewState.New;
				}
				else if (!Blackboard.IsToggleOn(achievement.AchievementDefinition.BlackboardCompletedKey))
				{
					achievement.ViewState = AchievementViewState.NewViewed;
				}
				else
				{
					achievement.ViewState = AchievementViewState.CompletedViewed;
				}
			}
		}

		private void AddAchievement(Achievement achievement)
		{
			Achievements.Add(achievement);
			if (!AchievementsById.ContainsKey(achievement.AchievementDefinitionID))
			{
				AchievementsById.Add(achievement.AchievementDefinitionID, achievement);
			}
		}

		private void RemoveAchievement(Achievement achievement)
		{
			Achievements.Remove(achievement);
			if (AchievementsById.ContainsKey(achievement.AchievementDefinitionID))
			{
				AchievementsById.Remove(achievement.AchievementDefinitionID);
			}
		}

		public Achievement GetAchievement(AchievementDefinition achievementDefinition)
		{
			if (AchievementsById.TryGetValue(achievementDefinition.ID, out var value))
			{
				return value;
			}
			return null;
		}

		public DailyQuest GetDailyQuest(AchievementDefinition achievementDefinition)
		{
			for (int i = 0; i < Player.DailyQuests.Count; i++)
			{
				DailyQuest dailyQuest = Player.DailyQuests[i];
				if (dailyQuest.AchievementDefinitionID == achievementDefinition.ID)
				{
					return dailyQuest;
				}
			}
			return null;
		}

		public void CheckAchievements()
		{
			if (!Player.gameEconomyData.ConfigData.EnableAchievements)
			{
				return;
			}
			for (int i = 0; i < Achievements.Count; i++)
			{
				Achievement achievement = Achievements[i];
				if (achievement.RewardClaimed)
				{
					continue;
				}
				if (achievement.IsCompleted)
				{
					if (!Blackboard.IsToggleOn(achievement.AchievementDefinition.BlackboardCompletedKey))
					{
						achievement.ViewState = AchievementViewState.Completed;
						Blackboard.SetToggle(achievement.AchievementDefinition.BlackboardCompletedKey);
					}
				}
				else
				{
					int progress = achievement.GetProgress();
					if (Blackboard.GetCounter(achievement.AchievementDefinition.BlackboardCounterKey) != progress)
					{
						Blackboard.SetCounter(achievement.AchievementDefinition.BlackboardCounterKey, progress);
					}
				}
			}
			InstantiateAchievementClasses();
			NotifyAchievementsChanged();
			UpdateDailyQuests();
		}

		public int TimeToNextDailyQuest()
		{
			int num = (int)((Player.DailyQuests != null) ? ((Player.manager.Time - Player.LastDailyQuestCreationTime) / 1000) : (-1));
			return Player.gameEconomyData.ConfigData.DailyQuestSpawnInterval - num;
		}

		public bool HasMaxDailyQuest()
		{
			int num = ((Player.DailyQuests != null) ? Player.DailyQuests.Count : 0);
			int maxDailyQuests = Player.gameEconomyData.ConfigData.MaxDailyQuests;
			return num >= maxDailyQuests;
		}

		private void UpdateDailyQuests()
		{
			int num = ((Player.DailyQuests != null) ? Player.DailyQuests.Count : 0);
			int maxDailyQuests = Player.gameEconomyData.ConfigData.MaxDailyQuests;
			int max = UtilsMath.Clamp(maxDailyQuests - num, 0, maxDailyQuests);
			int num2 = (int)((Player.DailyQuests != null) ? ((Player.manager.Time - Player.LastDailyQuestCreationTime) / 1000) : int.MaxValue);
			int num3 = UtilsMath.Clamp((Player.gameEconomyData.ConfigData.DailyQuestSpawnInterval > 0) ? (num2 / Player.gameEconomyData.ConfigData.DailyQuestSpawnInterval) : 0, 0, max);
			if (num3 <= 0)
			{
				return;
			}
			if (Player.DailyQuests == null)
			{
				Player.DailyQuests = new List<DailyQuest>();
			}
			for (int i = 0; i < Player.DailyQuests.Count; i++)
			{
				if (Player.DailyQuests[i].IsCompleted && Player.DailyQuests[i].ViewState < AchievementViewState.Completed)
				{
					Player.DailyQuests[i].ViewState = AchievementViewState.Completed;
				}
			}
			List<string> list = new List<string>();
			for (int j = 0; j < Player.DailyQuests.Count; j++)
			{
				list.Add(Player.DailyQuests[j].AchievementDefinitionID);
			}
			int num4 = 0;
			for (int k = 0; k < num3; k++)
			{
				if (TryToCreateDailyQuest(list, updateCreationTime: true))
				{
					num4++;
				}
			}
			if (num4 > 0)
			{
				NotifyDailyQuestsChanged();
			}
		}

		public bool TryToCreateDailyQuest(List<string> ignoreList, bool updateCreationTime)
		{
			for (int i = 0; i < 100; i++)
			{
				AchievementDefinition randomDailyQuestDefinition = Player.gameEconomyData.GetRandomDailyQuestDefinition(Player.PlayerRandom.GetRandomInRange(0, 1000000), ignoreList);
				if (randomDailyQuestDefinition == null)
				{
					break;
				}
				ignoreList.Add(randomDailyQuestDefinition.ID);
				if (CreateDailyQuest(randomDailyQuestDefinition, updateCreationTime))
				{
					return true;
				}
			}
			return false;
		}

		private bool CreateDailyQuest(AchievementDefinition achievementDefinition, bool updateDailyQuestCreationTime)
		{
			Type type = ReflectionUtils.FindDerivedTypeStartingWith(typeof(DailyQuest), achievementDefinition.Class);
			if (type != null)
			{
				object obj = null;
				try
				{
					obj = Activator.CreateInstance(type);
				}
				catch (Exception)
				{
				}
				if (obj != null)
				{
					DailyQuest dailyQuest = obj as DailyQuest;
					dailyQuest.AchievementDefinitionID = achievementDefinition.ID;
					StartDailyQuest(dailyQuest);
					dailyQuest.Initialize();
					if (dailyQuest.CanComplete)
					{
						Player.DailyQuests.Add(dailyQuest);
						if (updateDailyQuestCreationTime)
						{
							Player.LastDailyQuestCreationTime = Player.manager.Time;
						}
						return true;
					}
				}
			}
			return false;
		}

		public bool DiscardDailyQuest(string achievementDefinitionID)
		{
			if (CanDiscardDailyQuest && Player.DailyQuests != null)
			{
				bool flag = false;
				for (int i = 0; i < Player.DailyQuests.Count; i++)
				{
					if (Player.DailyQuests[i].AchievementDefinitionID == achievementDefinitionID)
					{
						Player.DailyQuests.RemoveAt(i);
						flag = true;
						Player.LastDailyQuestDiscardTime = Player.manager.Time;
						break;
					}
				}
				if (flag)
				{
					List<string> list = new List<string>();
					for (int j = 0; j < Player.DailyQuests.Count; j++)
					{
						list.Add(Player.DailyQuests[j].AchievementDefinitionID);
					}
					list.Add(achievementDefinitionID);
					TryToCreateDailyQuest(list, updateCreationTime: false);
					NotifyDailyQuestsChanged();
					return true;
				}
			}
			return false;
		}

		public bool CompleteDailyQuest(string achievementDefinitionID)
		{
			if (Player.DailyQuests != null)
			{
				bool flag = false;
				for (int i = 0; i < Player.DailyQuests.Count; i++)
				{
					if (Player.DailyQuests[i].AchievementDefinitionID == achievementDefinitionID)
					{
						Player.DailyQuests.RemoveAt(i);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					if (CanCreateNewDailyQuest)
					{
						List<string> list = new List<string>();
						for (int j = 0; j < Player.DailyQuests.Count; j++)
						{
							list.Add(Player.DailyQuests[j].AchievementDefinitionID);
						}
						list.Add(achievementDefinitionID);
						TryToCreateDailyQuest(list, updateCreationTime: true);
					}
					NotifyDailyQuestsChanged();
					return true;
				}
			}
			return false;
		}

		private void StartDailyQuest(DailyQuest dailyQuest)
		{
			dailyQuest.Start(Player);
		}

		private void StartDailyQuests()
		{
			if (Player.DailyQuests != null)
			{
				for (int i = 0; i < Player.DailyQuests.Count; i++)
				{
					StartDailyQuest(Player.DailyQuests[i]);
				}
			}
		}

		public void SetAchievementClaimed(AchievementDefinition definition)
		{
			if (!Player.Blackboard.IsToggleOn(definition.BlackboardRewardClaimedKey))
			{
				Player.Blackboard.SetToggle(definition.BlackboardRewardClaimedKey);
				NotifyAchievementsChanged();
				CheckAchievements();
			}
		}

		public int GetClaimCount(bool includeAchievements, bool includeDailyQuest)
		{
			int num = 0;
			if (includeAchievements)
			{
				for (int i = 0; i < Achievements.Count; i++)
				{
					Achievement achievement = Achievements[i];
					if (!achievement.RewardClaimed && achievement.IsCompleted)
					{
						num++;
					}
				}
			}
			if (includeDailyQuest)
			{
				List<DailyQuest> list = ((Player != null) ? Player.DailyQuests : null);
				if (list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						if (list[j].IsCompleted)
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		public bool IsAchievementCompleted(AchievementDefinition achievementDefinition)
		{
			return Blackboard.IsToggleOn(achievementDefinition.BlackboardCompletedKey);
		}

		public int GetProgress(AchievementDefinition achievementDefinition)
		{
			if (achievementDefinition != null)
			{
				if (IsAchievementCompleted(achievementDefinition))
				{
					return 100;
				}
				return GetAchievement(achievementDefinition)?.GetProgress() ?? Blackboard.GetCounter(achievementDefinition.BlackboardCounterKey);
			}
			return 0;
		}
	}
}
