using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildMenu : UIToggleContent
{
	[SerializeField]
	private GameObject notMemberContainer;

	[SerializeField]
	private GameObject memberContainer;

	[Header("Non Member View")]
	[SerializeField]
	private PayButton createGuildButton;

	[Header("Member View")]
	[SerializeField]
	private UILabel guildNameLabel;

	[SerializeField]
	private UILabel guildDescLabel;

	[SerializeField]
	private UILabel guildTypeDescLabel;

	[SerializeField]
	private UILabel membersNumberLabel;

	[Header("Leader View")]
	[SerializeField]
	private UILabel adRemainingTimeLabel;

	[SerializeField]
	private UIButton advertiseGuildButton;

	[SerializeField]
	private UIButton requestButton;

	[SerializeField]
	private UIButton inviteButton;

	[Header("Guild Gifts")]
	[SerializeField]
	private UIButton GiftButton;

	[SerializeField]
	private UIButton ReceiveGiftButton;

	[SerializeField]
	private GameObject freeGuildGiftNotification;

	private UIToggleContent UIToggleContentRef;

	private float refreshTimer;

	public void OnRequestsClicked()
	{
		new GuildModelWrapper(GameManager.Instance.guildModel);
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildRequestsPopup) as GuildRequestsPopup).Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	public void OnGuildReceiveGiftButtonClicked()
	{
		if (GameManager.Instance.guildModel != null && GameManager.Instance.playerModel.HasPendingGuildGiftsToOpen())
		{
			OpenLootInUi openLootInUi = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi;
			if (openLootInUi != null)
			{
				openLootInUi.SetupForGuildGift();
			}
		}
	}

	public void OnGuildGiftButtonClicked()
	{
		if (GameManager.Instance.guildModel != null && !GameManager.Instance.playerModel.HasPendingGuildGiftsToOpen())
		{
			HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialSendGuildGift);
			hUDElement.Open();
			hUDElement.GetDefaultPopup().SetInstantPayPanel(active: false);
		}
		Helpers.GameObjectSetActive(freeGuildGiftNotification, GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.FreeGuildGiftPerk) > 0);
	}

	public void OnCreateGuild()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialCreateGuild).Open();
	}

	public void OnGuildInfo()
	{
		GuildModelWrapper model = new GuildModelWrapper(GameManager.Instance.guildModel);
		GuildInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialGuildInfoPopup) as GuildInfoPopup;
		obj.GuildInfoPopupType = GuildInfoPopup.GuildPopupType.OwnGuild;
		obj.OpenForModel(model);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnEnable()
	{
		Setup();
		UIEvent.OnUIEvent += OnUiEvent;
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.WeeklyChallenge.Changed += OnWeeklyChallengeChanged;
		}
		if (GameManager.Instance != null && GameManager.Instance.playerModel.GuildModel != null)
		{
			GameManager.Instance.playerModel.GuildModel.Changed += OnGuildChanged;
		}
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			GameManager.Instance.playerModel.Changed += OnPlayerModelChange;
		}
		UIToggleContentRef = GetComponent<UIToggleContent>();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUiEvent;
		if (GameManager.Instance != null)
		{
			GameManager.Instance.playerModel.WeeklyChallenge.Changed -= OnWeeklyChallengeChanged;
		}
		if (GameManager.Instance != null && GameManager.Instance.playerModel.GuildModel != null)
		{
			GameManager.Instance.playerModel.GuildModel.Changed -= OnGuildChanged;
		}
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null)
		{
			GameManager.Instance.playerModel.Changed -= OnPlayerModelChange;
		}
		UIToggleContentRef = null;
	}

	private void OnWeeklyChallengeChanged(ModelObject model, string changed, object args)
	{
		if (changed == "GuildRewardAdded")
		{
			Setup();
		}
	}

	private void OnGuildChanged(GroupModelBase model, string changed, object args)
	{
		Setup();
	}

	public void OnPlayerModelChange(ModelObject m, string changed, object args)
	{
		if (changed == "guildGiftClaimed" || changed == "guildChanged")
		{
			Setup();
		}
	}

	private void OnUiEvent(string type, object parameter)
	{
		switch (type)
		{
		case "SocialGuildPlayerChanged":
		case "OnGuildGiftSent":
		case "SocialMembershipAccepted":
		case "OnGuildGiftReceived":
			Setup();
			break;
		}
	}

	private void Setup()
	{
		if (!(GameManager.Instance == null))
		{
			if (!GameManager.Instance.playerModel.IsGuildMember && !OfflineManager.IsLoadDataManager)
			{
				SetupNotMemberView();
			}
			else
			{
				SetupMemberView();
			}
		}
	}

	private void Update()
	{
		if (OfflineManager.IsLoadDataManager) return;
		refreshTimer -= Time.deltaTime;
		if (refreshTimer < 0f)
		{
			UpdateAdButton();
			refreshTimer = 1f;
		}
	}

	private void UpdateAdButton()
	{
		if (adRemainingTimeLabel != null)
		{
			GuildModel guildModel = GameManager.Instance.guildModel;
			bool flag = GameManager.Instance.gameEconomyData.ConfigData.GuildAdEnabled && guildModel != null && guildModel.AdAvailableTimeSeconds > 0;
			adRemainingTimeLabel.gameObject.SetActive(flag);
			if (flag)
			{
				adRemainingTimeLabel.text = LocalizationManager.GetText("Generic.Guild.AdRemainingTime{Param}", Helpers.FormatTimeNoZero(guildModel.AdAvailableTimeSeconds * 1000));
			}
			if (advertiseGuildButton != null)
			{
				GuildMemberInfo guildMemberInfo = guildModel?.GetMemberInfo(GameManager.Instance.playerModel.HashedId);
				bool flag2 = guildMemberInfo != null && guildMemberInfo.Role > GuildMemberRole.Normal;
				bool active = GameManager.Instance.gameEconomyData.ConfigData.GuildAdEnabled && guildModel != null && guildModel.AdAvailableTimeSeconds <= 0 && flag2;
				advertiseGuildButton.gameObject.SetActive(active);
			}
		}
	}

	private void SetupMemberView()
	{
		if (notMemberContainer) notMemberContainer.SetActive(value: false);
		memberContainer.SetActive(value: true);
		GameEconomyData gameEconomyData = GameManager.Instance.playerModel.gameEconomyData;
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel != null)
		{
			DebugTWD.Log("Setup MemberView " + guildModel.Name);
			guildNameLabel.text = GameManager.Instance.GetFilteredText(guildModel.Name);
			membersNumberLabel.text = guildModel.NumberMembers + "/" + 20;
			string hashedId = GameManager.Instance.playerModel.HashedId;
			GuildMemberInfo memberInfo = guildModel.GetMemberInfo(hashedId);
			if (GameManager.Instance.gameEconomyData.GetFeature("InviteGuild").Enabled)
			{
				_ = guildModel.JoinType != GuildJoinType.Closed;
			}
			else
				_ = 0;
			if (!OfflineManager.IsLoadDataManager)
			{
				bool num = memberInfo.Role > GuildMemberRole.Normal;
				Helpers.GameObjectSetActive(inviteButton, value: false);
				requestButton.isEnabled = false;
				Helpers.GameObjectSetActive(requestButton, value: false);
				if (num && guildModel.JoinType == GuildJoinType.Invite && guildModel.GuildMembersPending.Count > 0)
				{
					requestButton.isEnabled = true;
					Helpers.GameObjectSetActive(requestButton, value: true);
					Helpers.GameObjectSetActive(inviteButton, value: false);
				}
				if (GameManager.Instance.playerModel.HasPendingGuildGiftsToOpen())
				{
					Helpers.GameObjectSetActive(ReceiveGiftButton, gameEconomyData.ConfigData.GuildGiftsEnabled);
					Helpers.GameObjectSetActive(GiftButton, value: false);
				}
				else
				{
					Helpers.GameObjectSetActive(ReceiveGiftButton, value: false);
					Helpers.GameObjectSetActive(GiftButton, gameEconomyData.ConfigData.GuildGiftsEnabled);
					GiftButton.isEnabled = guildModel.CanGiveGift() && GameManager.Instance.playerModel.CanGiveGuildGift();
				}
			}
			string filteredText = GameManager.Instance.GetFilteredText(guildModel.Description);
			HelpersUI.SetContentToLabel(guildDescLabel, string.IsNullOrEmpty(filteredText) ? SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Generic.Guild.NoDescription") : filteredText);
			if (guildTypeDescLabel != null)
			{
				guildTypeDescLabel.text = LocalizationManager.GetText("Generic.Guild.JoinType." + guildModel.JoinType);
			}
			Helpers.GameObjectSetActive(freeGuildGiftNotification, GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.FreeGuildGiftPerk) > 0);
		}
	}

	private void SetupNotMemberView()
	{
		if (notMemberContainer) notMemberContainer.SetActive(value: true);
		memberContainer.SetActive(value: false);
		createGuildButton.UpdateUI(GameManager.Instance.GuildManager.GetCreateGuildCashier(), LocalizationManager.GetText("Popup.CreateGuild.Button.Create"));
		if (GiftButton != null)
		{
			GiftButton.gameObject.SetActive(value: false);
		}
		if (ReceiveGiftButton != null)
		{
			ReceiveGiftButton.gameObject.SetActive(value: false);
		}
	}

	public void OnClickFindGuild()
	{
		IngameLoading obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading) as IngameLoading;
		obj.Open();
		obj.SetText(LocalizationManager.GetText("Popup.Guild.SearchingGuild"));
		StartCoroutine(GameManager.Instance.GuildManager.SuggestionLogic.GuildSuggestionCheck(this, forceShow: true));
	}

	public void OnClickAdvertise()
	{
		GuildAdvertisePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildAdvertisePopup) as GuildAdvertisePopup;
		obj.Open();
		obj.SetContent("", "", 0);
		obj.SetCallbacks(delegate
		{
			ConsumeCurrencyCommandUtils.ExecuteForSocialCommands(GameManager.Instance.playerModel.GetCashierForGuildAd(), CreateGuildAdCall);
		});
		obj.Open();
	}

	private void CreateGuildAdCall(TWDModelResult result)
	{
		if (result == TWDModelResult.OK)
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			GuildModel guildModel = GameManager.Instance.guildModel;
			if (playerModel != null && guildModel != null)
			{
				Helpers.ExecuteCommand(new CreateGuildAdCommand
				{
					AdCreatorId = playerModel.HashedId,
					ExpirationTimeSeconds = GameManager.Instance.gameEconomyData.ConfigData.GuildAdExpirationTime,
					AdBucket = Random.Range(0, GameManager.Instance.gameEconomyData.ConfigData.GuildAdBucketCount),
					AdUniqueId = playerModel.GuildId + playerModel.UtcTimeStamp
				});
				SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildAdvertisePopup);
				Setup();
			}
		}
	}

	public void OnInviteClick()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildInvitePopup).Open();
	}


	#region myparams
	public UILabel guildNameLabelMain;
	#endregion
}
