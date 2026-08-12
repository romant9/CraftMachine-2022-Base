using System;
using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class DailyQuestRuleFunctions
	{
		private static long IsClassAvailable(QuestCompleteContext context, List<QuestDefinitionOperator> arguments)
		{
			if (arguments.Count == 0)
			{
				return 0L;
			}
			QuestDefinitionOperator questDefinitionOperator = arguments[0];
			string text = context.MapValueToString(questDefinitionOperator.Value);
			try
			{
				SurvivorClass survivorClass = (SurvivorClass)Enum.Parse(typeof(SurvivorClass), text);
				ModelList<SurvivorModel> survivors = context.ModelManager.Player.SurvivorContainer.Survivors;
				for (int i = 0; i < survivors.Count; i++)
				{
					if (survivors[i].SurvivorClass == survivorClass)
					{
						return 1L;
					}
				}
			}
			catch (Exception ex)
			{
				context.ModelManager.Debug.LogError($"Failed to check for availability of survivor class {text}. Exception was thrown: {ex.ToString()}");
			}
			return 0L;
		}

		private static long IsHeroAvailable(QuestCompleteContext context, List<QuestDefinitionOperator> arguments)
		{
			if (arguments.Count == 0)
			{
				return 0L;
			}
			QuestDefinitionOperator questDefinitionOperator = arguments[0];
			string text = context.MapValueToString(questDefinitionOperator.Value);
			ModelList<SurvivorModel> survivors = context.ModelManager.Player.SurvivorContainer.Survivors;
			for (int i = 0; i < survivors.Count; i++)
			{
				SurvivorModel survivorModel = survivors[i];
				if (survivorModel.IsHero && survivorModel.Definition != null && survivorModel.Definition.ID == text)
				{
					return 1L;
				}
			}
			return 0L;
		}

		private static long IsMissionKindAvailable(QuestCompleteContext context, List<QuestDefinitionOperator> arguments)
		{
			if (arguments.Count == 0)
			{
				return 0L;
			}
			QuestDefinitionOperator questDefinitionOperator = arguments[0];
			string text = context.MapValueToString(questDefinitionOperator.Value);
			try
			{
				switch ((MapCategory)Enum.Parse(typeof(MapCategory), text))
				{
				case MapCategory.Grind:
					return 1L;
				case MapCategory.Story:
				{
					ModelList<MapMissionGroupModel> mapMissionGroups = context.ModelManager.Player.MapContainerModel.MapMissionGroups;
					for (int j = 0; j < mapMissionGroups.Count; j++)
					{
						MapMissionGroupModel mapMissionGroupModel = mapMissionGroups[j];
						if (mapMissionGroupModel.MissionSpawnPointGroup != null && mapMissionGroupModel.MissionSpawnPointGroup.Category == MapCategory.Story && !mapMissionGroupModel.AreAllStoryMissionsCompleted())
						{
							return 1L;
						}
					}
					return 0L;
				}
				case MapCategory.Season:
				{
					StoryTellerModel storyTeller = context.ModelManager.Player.SurvivorContainer.StoryTeller;
					bool flag = storyTeller.GetCurrentUncompletedQuestDefinition() != null && storyTeller.GetCurrentUncompletedQuestDefinition().Order > 0;
					GameEconomyData gameEconomyData = context.ModelManager.GameEconomyData;
					long utcTimeStamp = context.ModelManager.Player.UtcTimeStamp;
					bool flag2 = false;
					for (int i = 0; i < gameEconomyData.MissionHighlights.Length; i++)
					{
						if (gameEconomyData.MissionHighlights[i].IsActive(utcTimeStamp))
						{
							flag2 = true;
							break;
						}
					}
					return (flag && flag2) ? 1 : 0;
				}
				case MapCategory.Challenge:
				case MapCategory.ApocalypticChallenge:
					return (context.ModelManager.GameEconomyData.GetWeeklyChallengePlayableWhen(context.ModelManager.Player.UtcTimeStamp, (long)new TimeSpan(12, 0, 0).TotalMilliseconds) != null) ? 1 : 0;
				case MapCategory.Survival:
					return (context.ModelManager.GameEconomyData.GetSurvivalPlayableWhen(context.ModelManager.Player.UtcTimeStamp, (long)new TimeSpan(12, 0, 0).TotalMilliseconds) != null) ? 1 : 0;
				case MapCategory.Outpost:
					return 1L;
				}
			}
			catch (Exception ex)
			{
				context.ModelManager.Debug.LogError($"Failed to check for availability of mission kind {text}. Exception was thrown: {ex.ToString()}");
			}
			return 0L;
		}

		private static long IsInGuild(QuestCompleteContext context, List<QuestDefinitionOperator> arguments)
		{
			return context.ModelManager.Player.HasGuild ? 1 : 0;
		}

		private static long IsHighlightedSeasonMission(QuestCompleteContext context, List<QuestDefinitionOperator> arguments)
		{
			TWDModelManager modelManager = context.ModelManager;
			MapMissionModel mapMissionModel = ((modelManager.Player.MapContainerModel != null) ? modelManager.Player.MapContainerModel.AttackTargetMissionModel : null);
			if (mapMissionModel == null)
			{
				return 0L;
			}
			MapMissionGroupModel missionGroupModelForSpawnPointGroup = modelManager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(mapMissionModel.MissionSpawnPointGroup);
			if (missionGroupModelForSpawnPointGroup == null)
			{
				return 0L;
			}
			return (missionGroupModelForSpawnPointGroup.IsFeaturedData != null && missionGroupModelForSpawnPointGroup.IsFeaturedData.IsActive(modelManager.Player.UtcTimeStamp)) ? 1 : 0;
		}

		private static long IsTimeInRange(QuestCompleteContext context, List<QuestDefinitionOperator> arguments)
		{
			if (arguments.Count < 3)
			{
				return 0L;
			}
			QuestDefinitionOperator questDefinitionOperator = arguments[0];
			QuestDefinitionOperator questDefinitionOperator2 = arguments[0];
			QuestDefinitionOperator questDefinitionOperator3 = arguments[0];
			string text = context.MapValueToString(questDefinitionOperator.Value);
			string text2 = context.MapValueToString(questDefinitionOperator2.Value);
			string text3 = context.MapValueToString(questDefinitionOperator3.Value);
			if (string.IsNullOrEmpty(text))
			{
				return 0L;
			}
			DateTime result;
			if (text == "CurrentTime")
			{
				result = context.ModelManager.Player.UtcTime;
			}
			else if (!DateTime.TryParse(text, out result))
			{
				return 0L;
			}
			if (string.IsNullOrEmpty(text2) || !DateTime.TryParse(text2, out var result2))
			{
				return 0L;
			}
			if (string.IsNullOrEmpty(text3) || !DateTime.TryParse(text3, out var result3))
			{
				return 0L;
			}
			return (result >= result2 && result <= result3) ? 1 : 0;
		}

		public static void RegisterRuleFunctions(DailyQuestModel questModel)
		{
			questModel.RegisterRuleFunction("IsClassAvailable", IsClassAvailable);
			questModel.RegisterRuleFunction("IsHeroAvailable", IsHeroAvailable);
			questModel.RegisterRuleFunction("IsMissionKindAvailable", IsMissionKindAvailable);
			questModel.RegisterRuleFunction("IsInGuild", IsInGuild);
			questModel.RegisterRuleFunction("IsHighlightedSeasonMission", IsHighlightedSeasonMission);
			questModel.RegisterRuleFunction("IsTimeInRange", IsTimeInRange);
		}
	}
}
