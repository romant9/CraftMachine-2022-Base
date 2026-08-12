using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class GuildInviteFlow
{
	private GuildModel guildToJoinModel;

	public string GuildToJoinId { get; set; }

	public string InviterHashedId { get; set; }

	public bool HasDeeplink
	{
		get
		{
			if (GuildToJoinId != null)
			{
				return InviterHashedId != null;
			}
			return false;
		}
	}

	private void ResetGuildToJoin()
	{
		GuildToJoinId = null;
		PlayerPrefs.DeleteKey("GuildToJoinId");
		PlayerPrefs.DeleteKey("InviterHashedId");
	}

	public void StartJoinGuildFlow()
	{
		if (!HasDeeplink)
		{
			return;
		}
		if (TutorialView.Instance != null && TutorialView.Instance.Model != null && !TutorialView.Instance.Model.StaticTutorialComplete)
		{
			if (GameManager.Instance.gameEconomyData.GetFeature("InviteGuildDeeplink").Enabled && GameManager.Instance.gameEconomyData.GetFeature("InviteGuildTutorialFlow").Enabled)
			{
				Helpers.ExecuteCommandDelayed(new SendGuildInviteMetricsCommand(SendGuildInviteMetricsCommand.EventType.ReceivedTutorial, GuildToJoinId, InviterHashedId));
				SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.HUDNotification).Open();
				HUDNotification.Info(LocalizationManager.GetText("Popup.GuildInvite.TutorialInfo.Message"));
				ResetGuildToJoin();
			}
		}
		else
		{
			StartJoinGuildFlowAfterTutorial();
		}
	}

	public void StartJoinGuildFlowAfterTutorial()
	{
		if (GameManager.Instance.gameEconomyData.GetFeature("InviteGuildDeeplink").Enabled && GameManager.Instance.GuildManager != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
			guildToJoinModel = null;
			SignalRClient.Instance.RequestCommand("TryGetGroupInfo", GuildToJoinId, OnGuildReceived, waitForResponse: true);
		}
	}

	private void OnGuildReceived(string message)
	{
		bool flag = false;
		GuildModel guildModel = null;
		if (string.IsNullOrEmpty(message) || message == "null")
		{
			flag = true;
		}
		else
		{
			guildModel = GameManager.Instance.modelManager.GetMessageSerializer().DeserializeObject<GuildModel>(message);
			if (guildModel == null)
			{
				flag = true;
			}
		}
		if (flag)
		{
			AlertPopup.ShowPopupGetText("Error.ErrorGeneric", "Popup.GuildInvite.GuildNotFound", "Button.Ok", null);
		}
		else
		{
			ResetGuildToJoin();
			guildToJoinModel = guildModel;
			JoinGuildEnterName();
		}
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
	}

	private void JoinGuildEnterName()
	{
		if (string.IsNullOrEmpty(GameManager.Instance.playerModel.Name))
		{
			EnterNamePopup enterNamePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialEnterName) as EnterNamePopup;
			if (enterNamePopup != null)
			{
				enterNamePopup.OnSubmitCallback = JoinGuildShowGuildInvitePopup;
				enterNamePopup.Open();
			}
		}
		else
		{
			JoinGuildShowGuildInvitePopup(UIType.None);
		}
	}

	private void JoinGuildShowGuildInvitePopup(UIType uiType)
	{
		GuildModelWrapper model = new GuildModelWrapper(guildToJoinModel);
		GuildInvitedInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialGuildInvitedInfoPopup) as GuildInvitedInfoPopup;
		obj.InviterId = InviterHashedId;
		obj.OpenForModel(model);
	}

	public void StartJoinGuildInCombat()
	{
		if (!HasDeeplink)
		{
			return;
		}
		Helpers.ExecuteCommandDelayed(new SendGuildInviteMetricsCommand(SendGuildInviteMetricsCommand.EventType.ReceivedCombat, GuildToJoinId, InviterHashedId));
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.HUDNotification).Open();
		if (TutorialView.Instance != null && TutorialView.Instance.Model != null && !TutorialView.Instance.Model.StaticTutorialComplete)
		{
			if (GameManager.Instance.gameEconomyData.GetFeature("InviteGuildTutorialFlow").Enabled)
			{
				HUDNotification.Info(LocalizationManager.GetText("Popup.GuildInvite.TutorialInfo.Message"));
			}
		}
		else
		{
			HUDNotification.Info(LocalizationManager.GetText("Popup.GuildInvite.CombatInfo.Message"));
		}
		ResetGuildToJoin();
	}

	public void StartJoinGuildAfterResumeGame()
	{
		if (HasDeeplink && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.Combat == null)
		{
			SaveCurrentDeeplink();
			GameManager.Instance.ReloadGame();
		}
	}

	public static void InviteToMyGuild()
	{
		string shareText = "Please join my group in The Walking Dead: No Man's Land! Click here to join: ";
		string bundleURLScheme = GameConfiguration.Instance.Config.BundleURLScheme;
		string id = GameManager.Instance.guildModel.Id;
		string hashedId = GameManager.Instance.playerModel.HashedId;
		Helpers.ExecuteCommandDelayed(new SendGuildInviteMetricsCommand(SendGuildInviteMetricsCommand.EventType.InviteSent));
		string url = ((!(bundleURLScheme != "twdnomansland") || !(bundleURLScheme != "twdnomanslandlv")) ? $"http://www.thewalkingdeadnomansland.com/guildinvite/?g={id}&p={hashedId}&l={SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage}" : $"http://www.thewalkingdeadnomansland.com/guildinvite/?g={id}&p={hashedId}&l={SingularityMonoBehaviour<LocalizationManager>.Instance.CurrentLanguage}&s={bundleURLScheme}");
		NativeShare nativeShare = Helpers.AddComponent<NativeShare>(GameManager.Instance.gameObject);
		GameManager.Instance.RequestPltv();
		nativeShare.Share(shareText, null, url);
	}

	private void SaveCurrentDeeplink()
	{
		PlayerPrefs.SetString("GuildToJoinId", GuildToJoinId);
		PlayerPrefs.SetString("InviterHashedId", InviterHashedId);
	}

	public static void TryRestoreDeeplink()
	{
		if (PlayerPrefs.HasKey("GuildToJoinId") && PlayerPrefs.HasKey("InviterHashedId"))
		{
			GameManager.Instance.GuildInviteFlow = new GuildInviteFlow();
			GameManager.Instance.GuildInviteFlow.GuildToJoinId = PlayerPrefs.GetString("GuildToJoinId");
			GameManager.Instance.GuildInviteFlow.InviterHashedId = PlayerPrefs.GetString("InviterHashedId");
		}
	}
}
