using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class ChangeAchievementViewState : ModelCommand
	{
		public ViewStateChangeScope ChangeScope { get; private set; }

		public string AchievementID { get; private set; }

		public AchievementViewState AchievementViewState { get; private set; }

		public ChangeAchievementViewState()
		{
		}

		public ChangeAchievementViewState(ViewStateChangeScope changeScope, AchievementViewState newViewState)
		{
			ChangeScope = changeScope;
			AchievementViewState = newViewState;
			AchievementID = null;
		}

		public ChangeAchievementViewState(string achievementID, AchievementViewState newViewState)
		{
			ChangeScope = ViewStateChangeScope.Single;
			AchievementViewState = newViewState;
			AchievementID = achievementID;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			if (manager is TWDModelManager tWDModelManager && tWDModelManager.Player.AchievementManager != null)
			{
				if (ChangeScope == ViewStateChangeScope.Single)
				{
					AchievementDefinition achievementDefinition = tWDModelManager.Player.gameEconomyData.GetAchievementDefinition(AchievementID);
					Achievement achievement = tWDModelManager.Player.AchievementManager.GetAchievement(achievementDefinition);
					if (achievement == null)
					{
						achievement = tWDModelManager.Player.AchievementManager.GetDailyQuest(achievementDefinition);
					}
					if (achievement != null)
					{
						achievement.ViewState = AchievementViewState;
					}
				}
				else if (ChangeScope == ViewStateChangeScope.AllAchievements)
				{
					AchievementManager achievementManager = tWDModelManager.Player.AchievementManager;
					for (int i = 0; i < achievementManager.Achievements.Count; i++)
					{
						Achievement achievement2 = achievementManager.Achievements[i];
						if (achievement2.ViewState < AchievementViewState)
						{
							achievement2.ViewState = AchievementViewState;
						}
					}
				}
				else if (ChangeScope == ViewStateChangeScope.AllDailyQuests)
				{
					List<DailyQuest> dailyQuests = tWDModelManager.Player.DailyQuests;
					for (int j = 0; j < dailyQuests.Count; j++)
					{
						DailyQuest dailyQuest = dailyQuests[j];
						if (dailyQuest.ViewState < AchievementViewState)
						{
							dailyQuest.ViewState = AchievementViewState;
						}
					}
				}
			}
			return new NGModelCommandRespond(this, TWDModelResult.OK);
		}
	}
}
