public class EventManager
{
	public delegate void ClickDelegate(string clickType);

	public enum EventType
	{
		StateTransitionCompleted = 0,
		CampVisualizationChanged = 1,
		CampMovedNotVisited = 2,
		CampMovedVisited = 3,
		BuildingPhotoRendered = 4,
		StartMission = 5,
		LoadingStepComplete = 6,
		AcceptSurvivor = 7,
		RejectSurvivor = 8,
		AcceptHeroTokens = 9,
		SurvivorPostponed = 10,
		StartCutVegetation = 11,
		CombatStart = 12,
		CombatStartTutorial = 13,
		VideoWatched = 14,
		CinematicWatched = 15,
		MapWorldMapShown = 16,
		MapDetailMapShown = 17,
		ShowMap = 18,
		ShowCamp = 19,
		SocialScoreLoaded = 20,
		SocialGlobalHighscoreLoaded = 21,
		SocialCountryHighscoreLoaded = 22,
		SocialGuildGlobalHighscoresLoaded = 23,
		SocialGuildLocalHighscoresLoaded = 24,
		TutorialPartOver = 25,
		TutorialEvent = 26,
		OutpostTemplateLoaded = 27,
		OutpostTemplateLoadFailed = 28,
		AnalyticsSent = 29,
		PromoteOkShown = 30,
		GuildBattleLockdownTimeEvent = 31,
		GroupModelLoaded = 32,
		None = 33
	}

	public enum EventTypeClick
	{
		None = 0,
		Shop = 1,
		MissionHub = 2
	}

	public delegate void EventDelegate(EventType eventType, object parameter = null);

	public static event ClickDelegate OnClick;

	public static event EventDelegate OnEvent;

	public static void NotifyClick(EventTypeClick clickType)
	{
		if (EventManager.OnClick != null)
		{
			NotifyClick(clickType.ToString());
		}
	}

	public static void NotifyClick(string clickType)
	{
		if (EventManager.OnClick != null)
		{
			EventManager.OnClick(clickType);
		}
	}

	public static void NotifyEvent(EventType eventType, object parameter = null)
	{
		if (EventManager.OnEvent != null)
		{
			EventManager.OnEvent(eventType, parameter);
		}
	}
}
