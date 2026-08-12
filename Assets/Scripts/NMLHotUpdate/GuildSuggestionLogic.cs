using System;
using System.Collections;
using TWDModel;
using UnityEngine;

public class GuildSuggestionLogic
{
	private enum State
	{
		Initial = 0,
		Triggering = 1,
		Querying = 2,
		GuildFound = 3,
		WaitingToShow = 4
	}

	private GuildManager guildManager;

	private TWDModelManager modelManager;

	private int queryMaxDelay = 15;

	private long popupDelayPeriod = 86400000L;

	private State state;

	private bool disabledForSession;

	private GuildModel guildModelToShow;

	public GuildSuggestionLogic(GuildManager guildManager, TWDModelManager modelManager)
	{
		this.guildManager = guildManager;
		this.modelManager = modelManager;
		if (modelManager != null)
		{
			if (modelManager.GameEconomyData.ConfigData.GuildSuggestionPopupQueryMaxDelay > 0)
			{
				queryMaxDelay = modelManager.GameEconomyData.ConfigData.GuildSuggestionPopupQueryMaxDelay;
			}
			if (modelManager.GameEconomyData.ConfigData.GuildSuggestionPopupShowBasePeriod > 0)
			{
				popupDelayPeriod = modelManager.GameEconomyData.ConfigData.GuildSuggestionPopupShowBasePeriod;
			}
		}
	}

	public IEnumerator GuildSuggestionCheck(MonoBehaviour caller, bool forceShow = false)
	{
		if (caller == null)
		{
			yield break;
		}
		if (state < State.Querying)
		{
			if (forceShow || ShouldTriggerGuildSuggestion())
			{
				state = State.Triggering;
				if (!forceShow)
				{
					int queryDelay = GetQueryDelay();
					yield return new WaitForSeconds(queryDelay);
				}
				if (!QueryGuild())
				{
					state = State.Initial;
					yield break;
				}
				state = State.Querying;
				if (modelManager != null)
				{
					disabledForSession = true;
				}
				yield return caller.StartCoroutine(GuildSuggestionShowPopupCheck(caller, forceShow));
			}
		}
		else
		{
			yield return caller.StartCoroutine(GuildSuggestionShowPopupCheck(caller, forceShow));
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
	}

	public void OnStop()
	{
		switch (state)
		{
		case State.Triggering:
			state = State.Initial;
			break;
		case State.WaitingToShow:
			state = State.GuildFound;
			break;
		default:
			state = State.Initial;
			break;
		case State.Initial:
		case State.Querying:
		case State.GuildFound:
			break;
		}
	}

	public IEnumerator GuildSuggestionShowPopupCheck(MonoBehaviour caller = null, bool forceShow = false)
	{
		if (state < State.Querying || state == State.WaitingToShow)
		{
			yield break;
		}
		while (state == State.Querying && guildModelToShow == null)
		{
			yield return null;
		}
		if (ValidateGuild(guildModelToShow))
		{
			state = State.WaitingToShow;
			while (!forceShow && !CanShowPopup())
			{
				yield return null;
			}
			Helpers.ExecuteCommand(new MarkGuildSuggestionPopupShownCommand());
			GuildModelWrapper model = new GuildModelWrapper(guildModelToShow);
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildSuggestionPopup) as GuildSuggestionPopup).OpenForModel(model);
		}
		else
		{
			if (caller.GetComponent<GuildMenu>() != null)
			{
				AlertPopup.ShowPopupGetText("Popup.Guild.GuildNotFound.Title", "Popup.Guild.GuildNotFound", "Button.Ok", null);
			}
			Debug.LogWarning("Got invalid guild from query for Guild Suggestion Popup, guild " + ((guildModelToShow == null) ? "is null" : ("id: " + guildModelToShow.Id)));
		}
		guildModelToShow = null;
		state = State.Initial;
	}

	private bool ShouldTriggerSinceLastShowing()
	{
		int guildSuggestionPopupShownCount = modelManager.Player.GuildSuggestionPopupShownCount;
		long guildSuggestionPopupLastShownTime = modelManager.Player.GuildSuggestionPopupLastShownTime;
		int num = 0;
		switch (guildSuggestionPopupShownCount)
		{
		case 0:
			return true;
		case 1:
			num = 1;
			break;
		case 2:
			num = 2;
			break;
		default:
			num = (int)Math.Pow(2.0, guildSuggestionPopupShownCount - 1);
			break;
		}
		long num2 = num * popupDelayPeriod;
		return GameManager.Instance.playerModel.UtcTimeStamp > guildSuggestionPopupLastShownTime + num2;
	}

	private bool ShouldTriggerGuildSuggestion()
	{
		if (modelManager == null)
		{
			return false;
		}
		if (disabledForSession)
		{
			return false;
		}
		if (!modelManager.GameEconomyData.ConfigData.GuildSuggestionPopupEnabled)
		{
			return false;
		}
		if (modelManager.Player.HasGuild)
		{
			return false;
		}
		if (modelManager.Player.CouncilLevel < modelManager.GameEconomyData.ConfigData.GuildSuggestionPopupCouncilLevelMin)
		{
			return false;
		}
		if (!ShouldTriggerSinceLastShowing())
		{
			return false;
		}
		if (SingularityMonoBehaviour<HUDManager>.Instance.NumberDialogsOpen > 0)
		{
			return false;
		}
		if (state != State.Initial)
		{
			return false;
		}
		if (guildModelToShow != null)
		{
			return false;
		}
		return true;
	}

	private bool CanShowPopup()
	{
		if (TutorialView.Instance.Running)
		{
			return false;
		}
		if (CampView.Instance == null)
		{
			return false;
		}
		if (!CampView.Instance.IsShown)
		{
			return false;
		}
		if (SingularityMonoBehaviour<HUDManager>.Instance.NumberDialogsOpen > 0)
		{
			return false;
		}
		return true;
	}

	private int GetQueryDelay()
	{
		return UnityEngine.Random.Range(0, queryMaxDelay);
	}

	private bool ValidateGuild(GuildModel guild)
	{
		if (guild == null)
		{
			Debug.LogWarning("GuildSuggestionPopup: Guild is null, no guild to join");
			return false;
		}
		if (guild.JoinType != GuildJoinType.Open)
		{
			Debug.LogWarning("GuildSuggestionPopup: Immediate join to guild is not possible, guild join type = " + guild.JoinType);
			return false;
		}
		if (guild.NumberMembers >= 20)
		{
			Debug.LogWarning("GuildSuggestionPopup: Guild is full, guild member count = " + guild.NumberMembers);
			return false;
		}
		if (!guild.CanReceiveRequest)
		{
			Debug.LogWarning("GuildSuggestionPopup: Guild is full, guild can not receive membership request");
			return false;
		}
		return true;
	}

	public bool QueryGuild()
	{
		guildManager.guildSuggestFinishedEvent += OnGuildSuggestion;
		guildManager.guildSearchFailedEvent += OnGuildSearchFailed;
		bool num = guildManager.SuggestGuild(1);
		if (num)
		{
			state = State.Querying;
		}
		return num;
	}

	private void OnGuildSuggestion(GuildModel model)
	{
		guildManager.guildSuggestFinishedEvent -= OnGuildSuggestion;
		guildManager.guildSearchFailedEvent -= OnGuildSearchFailed;
		guildModelToShow = model;
		if (guildModelToShow != null)
		{
			state = State.GuildFound;
		}
		else
		{
			state = State.Initial;
		}
	}

	private void OnGuildSearchFailed(string message)
	{
		Debug.LogWarning("Guild suggestion query failed: " + message);
		guildManager.guildSuggestFinishedEvent -= OnGuildSuggestion;
		guildManager.guildSearchFailedEvent -= OnGuildSearchFailed;
		guildModelToShow = null;
		state = State.Initial;
		if (modelManager != null)
		{
			disabledForSession = true;
		}
	}
}
