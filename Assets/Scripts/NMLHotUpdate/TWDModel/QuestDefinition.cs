using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class QuestDefinition
	{
		public const string MissionQuest = "MissionQuest";

		public string Identifier;

		public bool IsAvailable;

		public int Giver;

		public int Order;

		[GEDType(GEDSpecialType.TimeMilliseconds)]
		public int StartDelay;

		public string LocalisationKey;

		public string FailureKey;

		public int TimeLimit;

		public string ClassName;

		public List<string> ConstructionParameters;

		public string Rewards;

		public string AdditionalActor;

		[JsonIgnore]
		public string TitleKey => LocalisationKey + ".Title";

		[JsonIgnore]
		public string BriefingKey => LocalisationKey + ".Briefing";

		[JsonIgnore]
		public string CompletionKey => LocalisationKey + ".Completion";

		[JsonIgnore]
		public string DebriefingKey => LocalisationKey + ".Debriefing";

		[JsonIgnore]
		public string IntroKey => LocalisationKey + ".Intro";

		public Rewards GetRewards()
		{
			return new Rewards(Rewards);
		}

		public string GetMissionQuestMapId()
		{
			return ConstructionParameters[0];
		}

		public MapMissionGroupModel GetUnlockedEpisode(TWDModelManager manager)
		{
			if (ClassName != "MissionQuest")
			{
				return null;
			}
			MissionSpawnPointGroup spawnPointGroupByMapId = manager.Player.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroupByMapId(GetMissionQuestMapId());
			if (spawnPointGroupByMapId == null)
			{
				return null;
			}
			return manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(spawnPointGroupByMapId);
		}
	}
}
