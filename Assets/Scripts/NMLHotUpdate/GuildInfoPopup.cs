using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using System.Collections;
using TWDModel;
using UnityEngine;
using TwdCustomMod;

public class GuildInfoPopup : HUDElement
{
	public enum GuildPopupType
	{
		GuildSearch = 0,
		OwnGuild = 1
	}

	[Header("Profile")]
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UILabel numberMembersLabel;

	[SerializeField]
	private UILabel joinTypeLabel;

	[SerializeField]
	private UILabel purposeTypeLabel;

	[SerializeField]
	private UILabel challengesCompletedLabel;

	[SerializeField]
	private UILabel allChallengeStarsAmountLabel;

	[SerializeField]
	private UILabel challengeStarsAmountLabel;

	[SerializeField]
	private UILabel currentSeasonVpLabel;

	[SerializeField]
	private UILabel allTimeVpLabel;

	[SerializeField]
	private UILabel adRemainingTimeLabel;

	[Header("Leader Options")]
	[SerializeField]
	private UIButton editButton;

	[SerializeField]
	private GameObject leaderOptionsContainer;

	[SerializeField]
	private UIInput descriptionInput;

	[SerializeField]
	private UIToggle publicToggle;

	[SerializeField]
	private UIPopupList joinTypeDropDown;

	[SerializeField]
	private UILabel joinTypeDropDownLabel;

	[SerializeField]
	private UIPopupList purposeTypeDropDown;

	[SerializeField]
	private UILabel purposeTypeDropDownLabel;

	[SerializeField]
	private UIButton shareGuildButton;

	[SerializeField]
	private UIButton changeNameButton;

	[SerializeField]
	private UILabel changeNameColdTimeLabel;

	private GuildModel guild;

	private bool updateUI;

	private GuildJoinType currentGuildJoinType;

	private string description;

	private string currentGuildPurpose;

	public GuildPopupType GuildInfoPopupType { get; set; }

	public static void OpenForGuildId(string guildId)
	{
		DebugTWD.Log("Пытаюсь открыть другую гильдию", DebugType.Guild);

		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		SignalRClient.Instance.RequestCommand("GetGroupInfo", guildId, OnGuildReceived, waitForResponse: true);
	}

	private static void OnGuildReceived(string message)
	{
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
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
			AlertPopup.ShowPopupGetText("Error.Error", "Generic.Guild.NoGuildFound", "Button.Ok", null);
			SignalRClient.Instance.ClearError();
			return;
		}
		if (GameManager.Instance.IsWriteData)
		{
			GameManager.WriteDataToDisk("guild-" + guildModel.Id, message);
        }
		GuildModelWrapper guildModelWrapper = new GuildModelWrapper(guildModel);
		GuildInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialGuildInfoPopup) as GuildInfoPopup;
		obj.GuildInfoPopupType = GuildPopupType.GuildSearch;
		obj.OpenForModel(guildModelWrapper);
	}

	public override void OpenForModel(ModelObject model)
	{
		base.OpenForModel(model);
		defaultPopup.GetComponent<UIPanel>().depth = DefaultPopup.GuildInfoPopupDepth;
		if (IsLoadDataManager)
		{
			defaultPopup.transform.localScale = Vector3.one * 1.25f;
			this.transform.localScale = Vector3.one * 1.25f;
		}
		guild = ((GuildModelWrapper)model).GuildModel;
		guild.Changed += OnGuildChanged;
		description = guild.Description;
		descriptionInput.characterLimit = 200;
		updateUI = false;
		UpdateUI();
	}

	public override void Close()
	{
		base.Close();
		guild.Changed -= OnGuildChanged;
	}

	private void OnGuildChanged(GroupModelBase groupModelBase, string changed, object args)
	{
		UpdateUI();
	}

	public void OnJoinTypeChanged()
	{
		if (joinTypeDropDownLabel != null)
		{
			joinTypeDropDownLabel.text = joinTypeDropDown.value;
			int num = joinTypeDropDown.items.IndexOf(joinTypeDropDown.value);
			if (num > -1)
			{
				currentGuildJoinType = (GuildJoinType)num;
			}
		}
	}

	public void OnPurposeTypeChanged()
	{
		if (purposeTypeDropDownLabel != null)
		{
			purposeTypeDropDownLabel.text = purposeTypeDropDown.value;
			int num = purposeTypeDropDown.items.IndexOf(purposeTypeDropDown.value);
			if (num > -1 && GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes != null && GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes.Count > num)
			{
				currentGuildPurpose = GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes[num];
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		updateUI = false;
		GuildModel guildModel = GameManager.Instance.guildModel;
		nameLabel.text = guild.Name;
		descriptionLabel.gameObject.SetActive(value: true);
		descriptionLabel.text = (string.IsNullOrEmpty(description) ? SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Generic.Guild.NoDescription") : description);
		descriptionInput.value = description;
		if (joinTypeLabel != null)
		{
			joinTypeLabel.gameObject.SetActive(value: true);
			joinTypeLabel.text = LocalizationManager.GetText("Generic.Guild.JoinType." + guild.JoinType);
		}
		if (purposeTypeLabel != null)
		{
			Helpers.GameObjectSetActive(purposeTypeLabel, value: true);
			string text = guild.Purpose;
			if (text == null)
			{
				text = GuildModel.GetDefaultPurpose(GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes);
			}
			if (text != null)
			{
				purposeTypeLabel.text = HelpersLocalization.GetGuildPurpose(text);
			}
		}
		numberMembersLabel.text = guild.NumberMembers + "/" + 20;
		challengesCompletedLabel.text = guild.NumberChallengeStarted.ToString();
		allChallengeStarsAmountLabel.text = guild.TotalChallengeStars.ToString();
		challengeStarsAmountLabel.text = guild.CurrentChallengeStars.ToString();
		HelpersUI.SetContentToLabel(allTimeVpLabel, guild.TotalAllTimeAccumulatedVp.ToString());
		HelpersUI.SetContentToLabel(currentSeasonVpLabel, guild.GuildInfoCurrentVP.ToString());
		leaderOptionsContainer.SetActive(value: false);
		GuildModel guildModel2 = GameManager.Instance.guildModel;
		GuildMemberInfo guildMemberInfo = guildModel2?.GetMemberInfo(GameManager.Instance.playerModel.HashedId);
		bool flag = guildMemberInfo != null && guildMemberInfo.Role == GuildMemberRole.Leader;
		if (GameManager.Instance.gameEconomyData.ConfigData.GuildAdEnabled && guildModel != null && guildModel.AdAvailableTimeSeconds > 0)
		{
			_ = GuildInfoPopupType == GuildPopupType.OwnGuild;
		}
		else
			_ = 0;
		currentGuildJoinType = guild.JoinType;
		currentGuildPurpose = guild.Purpose;
		if (currentGuildPurpose == null)
		{
			currentGuildPurpose = GuildModel.GetDefaultPurpose(GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes);
		}
		if (joinTypeDropDown != null)
		{
			joinTypeDropDown.gameObject.SetActive(value: true);
			joinTypeDropDown.items = new List<string>();
			for (int i = 0; i <= 2; i++)
			{
				GuildJoinType guildJoinType = (GuildJoinType)i;
				joinTypeDropDown.items.Add(LocalizationManager.GetText("Generic.Guild.JoinType." + guildJoinType));
			}
		}
		if (purposeTypeDropDown != null)
		{
			List<string> guildPurposeTypes = GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes;
			if (guildPurposeTypes != null)
			{
				purposeTypeDropDown.items = new List<string>();
				for (int j = 0; j < guildPurposeTypes.Count; j++)
				{
					string purpose = guildPurposeTypes[j];
					purposeTypeDropDown.items.Add(HelpersLocalization.GetGuildPurpose(purpose));
				}
			}
		}
		if (shareGuildButton != null)
		{
			shareGuildButton.isEnabled = true;
			shareGuildButton.gameObject.SetActive(GameManager.Instance.gameEconomyData.GetFeature("GuildSharing").Enabled && flag);
		}
		defaultPopup.ShowSimpleButtons(show: false);
		if (GuildInfoPopupType == GuildPopupType.GuildSearch)
		{
			nameLabel.text = GameManager.Instance.GetFilteredText(guild.Name);
			string filteredText = GameManager.Instance.GetFilteredText(description);
			descriptionLabel.text = (string.IsNullOrEmpty(filteredText) ? SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Generic.Guild.NoDescription") : filteredText);
			editButton.gameObject.SetActive(value: false);
			if (IsLoadDataManager)
			{
				JoinLabel.text = LocalizationManager.GetText("Popup.Guild.Button.Join");
				defaultPopup.SetQuestion(LocalizationManager.GetText("Popup.Guild.WannaJoin"));
			}
			else
			{
				bool flag2 = guild.JoinType != GuildJoinType.Closed && !GameManager.Instance.playerModel.IsGuildMember;
				bool flag3 = guildModel2 != null && guildModel2.Id == guild.Id && guild.GetMemberPendingInfo(GameManager.Instance.playerModel.HashedId) != null;
				defaultPopup.ShowSimpleButtons(flag2 && !flag3);
				bool num = guild.JoinType == GuildJoinType.Open;
				string textId = (num ? "Popup.Guild.WannaJoin" : "Popup.Guild.WannaRequestMembership");
				string textId2 = (num ? "Popup.Guild.Button.Join" : "Popup.Guild.Button.RequestMembership");
				defaultPopup.SetSimplePositiveButton(available: true, LocalizationManager.GetText(textId2), OnRequestMembership);
				defaultPopup.SetSimpleNegativeButton(available: true, LocalizationManager.GetText("Popup.Guild.Button.DontRequestMembership"), OnDontRequestMembership);
				defaultPopup.SetQuestion(LocalizationManager.GetText(textId));
			}
		}
		else if (GuildInfoPopupType == GuildPopupType.OwnGuild)
		{
			if (IsLoadDataManager)
			{
				JoinLabel.text = LocalizationManager.GetText("Popup.Guild.Button.Join");
				defaultPopup.SetQuestion(LocalizationManager.GetText("Popup.Guild.WannaJoin"));
			}
			else
			{
				editButton.gameObject.SetActive(guildModel == guild && guildModel.CanEdit(GameManager.Instance.playerModel.HashedId));
				changeNameButton.gameObject.SetActive(guildModel == guild && guildModel.CanEdit(GameManager.Instance.playerModel.HashedId));
				defaultPopup.ShowSimpleButtons(show: true);
				if (guildMemberInfo != null && guildMemberInfo.Role == GuildMemberRole.Leader && guildModel2.NumberMembers > 1)
				{
					defaultPopup.SetSimplePositiveButton(available: true, LocalizationManager.GetText("Popup.Guild.Button.LeaveGuild"), OnLeaveGuildDenied);
				}
				else
				{
					defaultPopup.SetSimplePositiveButton(available: true, LocalizationManager.GetText("Popup.Guild.Button.LeaveGuild"), OnLeaveGuild);
				}
				defaultPopup.SetSimpleNegativeButton(available: false);
				defaultPopup.SetQuestion(null);
			}
		}
		Helpers.GameObjectSetActive(joinTypeDropDown, value: false);
		Helpers.GameObjectSetActive(purposeTypeDropDown, value: false);
		Helpers.GameObjectSetActive(publicToggle, value: false);
	}

	public void OnGuildShareClicked()
	{
		GuildSharePopup guildSharePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildSharePopup) as GuildSharePopup;
		if (guildSharePopup != null)
		{
			GuildModelWrapper guildModelWrapper = new GuildModelWrapper(guild);
			guildSharePopup.OpenForModel(guildModelWrapper);
		}
	}

	public override void Update()
	{
		base.Update();
		if (updateUI)
		{
			UpdateUI();
		}
		if (adRemainingTimeLabel != null)
		{
			GuildModel guildModel = GameManager.Instance.guildModel;
			bool flag = GameManager.Instance.gameEconomyData.ConfigData.GuildAdEnabled && guildModel != null && guildModel.AdAvailableTimeSeconds > 0 && GuildInfoPopupType == GuildPopupType.OwnGuild;
			adRemainingTimeLabel.gameObject.SetActive(flag);
			if (flag)
			{
				adRemainingTimeLabel.text = LocalizationManager.GetText("Generic.Guild.AdRemainingTime{Param}", Helpers.FormatTimeNoZero(guildModel.AdAvailableTimeSeconds * 1000));
			}
			if (shareGuildButton != null)
			{
				GuildMemberInfo guildMemberInfo = guildModel?.GetMemberInfo(GameManager.Instance.playerModel.HashedId);
				bool flag2 = guildMemberInfo != null && guildMemberInfo.Role == GuildMemberRole.Leader;
				bool active = GameManager.Instance.gameEconomyData.GetFeature("GuildSharing").Enabled && guildModel != null && GuildInfoPopupType == GuildPopupType.OwnGuild && flag2;
				shareGuildButton.gameObject.SetActive(active);
			}
		}
		if (changeNameColdTimeLabel != null)
		{
			GuildModel guildModel2 = GameManager.Instance.guildModel;
			bool flag3 = guildModel2 != null && guildModel2 == guild && guildModel2.ChangeNameColdTimeSeconds > 0 && guildModel2.CanEdit(GameManager.Instance.playerModel.HashedId) && GuildInfoPopupType == GuildPopupType.OwnGuild;
			changeNameColdTimeLabel.gameObject.SetActive(flag3);
			if (flag3)
			{
				changeNameColdTimeLabel.text = LocalizationManager.GetText("Popup.GuildName.ColdTime", Helpers.FormatTimeDayOrMin(guildModel2.ChangeNameColdTimeSeconds * 1000));
			}
		}
	}

	private void OnDontRequestMembership()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		Close();
	}

	private void OnRequestMembership()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			DebugTWD.Log("Загружаю гильдию");
			if (DataManager.Instance.ProGuild)
			{
				StartCoroutine(LoadGuild());
			}
			else
			{
				string message;
				if (DataManager.Instance.language == DataManager.Language.Ru)
				{
					message = "Необходим статус: PRO-GUILD!";
				}
				else
				{
					message = "Required status: PRO-GUILD!";
				}
				MyTools.OpenAlert(message);
				return;
			}
		}
		else
		{
			if (guild.NumberMembers >= 20)
			{
				AlertPopup.ShowPopup("", LocalizationManager.GetText("Popup.Guild.DenyRequestGuildFull"), LocalizationManager.GetText("Button.Ok"));
				return;
			}
			if (!guild.CanReceiveRequest)
			{
				AlertPopup.ShowPopup("", LocalizationManager.GetText("Popup.Guild.DenyRequestGuildMaxRequests"), LocalizationManager.GetText("Button.Ok"));
				return;
			}
			GuildModel guildModel = GameManager.Instance.guildModel;
			bool flag = guild.JoinType == GuildJoinType.Open;
			if (guildModel != null && !flag)
			{
				if (guildModel.Id == guild.Id)
				{
					AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.Guild.AlreadyRequested.Title"), LocalizationManager.GetText("Popup.Guild.AlreadyRequested.Message"), LocalizationManager.GetText("Button.Ok"));
					return;
				}
				ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.ConfirmationCancelOtherGuildRequest.Title{GuildToLeave}{NewGuild}", guildModel.Name, guild.Name), LocalizationManager.GetText("Popup.ConfirmationCancelOtherGuildRequest.Message{GuildToLeave}{NewGuild}", guildModel.Name, guild.Name), LocalizationManager.GetText("Button.Ok"), OnRequestMembershipConfirmed, LocalizationManager.GetText("Button.Cancel"));
			}
			else
			{
				OnRequestMembershipConfirmed();
			}
		}
	}

	private async void OnRequestMembershipConfirmed()
	{
		GameManager.Instance.GuildManager.JoinGuild(guild.Id);
		bool immediateJoin = guild.JoinType == GuildJoinType.Open;
		defaultPopup.transform.Find("Container_Simple_Buttons/Button_Yes").GetComponent<UIButton>().isEnabled = false;
		GuildModel guildModel = await GuildManager.GetGuild(guild.Id);
		defaultPopup.transform.Find("Container_Simple_Buttons/Button_Yes").GetComponent<UIButton>().isEnabled = true;
		PlayerModel playerModel = GameManager.Instance.playerModel;
		GuildManager.ShowGuildJoinResultMessage(immediateJoin, immediateJoin && (guildModel?.IsBanned(playerModel.HashedId, playerModel.UtcTimeStamp) ?? false));
		Close();
	}

	private void OnLeaveGuild()
	{
		if (!HandleLeaveGuildBehaviourDuringGuildBattle())
		{
			ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.ConfirmLeaveGuild.Title"), LocalizationManager.GetText("Popup.ConfirmLeaveGuild.Message"), LocalizationManager.GetText("Button.Yes"), OnLeaveGuildConfirmed, LocalizationManager.GetText("Button.Cancel"));
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	private void OnLeaveGuildDenied()
	{
		if (IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (IsLoadDataManager)");
			Close();
			return;
		}
		if (!HandleLeaveGuildBehaviourDuringGuildBattle())
		{
			AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.DeniedLeaveGuild.Title"), LocalizationManager.GetText("Popup.DeniedLeaveGuild.Message"), LocalizationManager.GetText("Button.Ok"), OnDoNothing);
		}
	}

	private bool HandleLeaveGuildBehaviourDuringGuildBattle()
	{
		bool result = false;
		if (GuildWarHelper.IsPlayerRegisteredForBattle())
		{
			if (GuildWarHelper.IsLockdownTimeForCurrentBattle())
			{
				AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.DeniedLeaveGuild.Title"), LocalizationManager.GetText("Popup.DeniedLeaveGuildBattleLockdownMessage"), LocalizationManager.GetText("Button.Ok"), OnDoNothing);
			}
			else if (GuildWarHelper.IsBattleOnGoing())
			{
				AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.DeniedLeaveGuild.Title"), LocalizationManager.GetText("Popup.DeniedLeaveGuildBattleActive.Message"), LocalizationManager.GetText("Button.Ok"), OnDoNothing);
			}
			else
			{
				ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.ConfirmLeaveGuild.Title"), LocalizationManager.GetText("Popup.ConfirmLeaveGuildRegisteredForBattle.Message"), LocalizationManager.GetText("Button.Yes"), OnLeaveGuildConfirmed, LocalizationManager.GetText("Button.Cancel"));
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			}
			result = true;
		}
		return result;
	}

	private void OnDoNothing()
	{
	}

	private void OnLeaveGuildConfirmed()
	{
		if (GameManager.Instance.GuildManager.CheckCanLeaveGuild())
		{
			GameManager.Instance.GuildManager.LeaveGuild();
		}
		else
		{
			AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.DeniedLeaveGuild.Title"), LocalizationManager.GetText("Popup.DeniedLeaveGuild.Message"), LocalizationManager.GetText("Button.Ok"));
		}
		Close();
	}

	public void OnChangeNameClicked()
	{
		if (GameManager.Instance.guildModel.ChangeNameColdTimeSeconds <= 0)
		{
			GuildNamePopup guildNamePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialGuildName) as GuildNamePopup;
			if (guildNamePopup != null)
			{
				guildNamePopup.Open();
			}
		}
	}

	public void OnEdit()
	{
		defaultPopup.ShowSimpleButtons(show: true);
		if (JoinButton && JoinButton.gameObject.activeSelf) JoinButton.gameObject.SetActive(false);
		defaultPopup.SetSimplePositiveButton(available: true, LocalizationManager.GetText("Button.Save"), CheckDescriptionAndJoinType);
		defaultPopup.SetSimpleNegativeButton(available: true, LocalizationManager.GetText("Button.Cancel"), OnCancelChanges);
		defaultPopup.SetQuestion(LocalizationManager.GetText("Popup.GuildInfo.SaveChanges"));
		leaderOptionsContainer.SetActive(value: true);
		editButton.gameObject.SetActive(value: false);
		descriptionLabel.gameObject.SetActive(value: false);
		Helpers.GameObjectSetActive(descriptionInput, value: true);
		descriptionInput.value = guild.Description;
		if (shareGuildButton != null)
		{
			shareGuildButton.isEnabled = false;
		}
		if (joinTypeDropDown != null)
		{
			Helpers.GameObjectSetActive(joinTypeLabel, value: false);
			joinTypeDropDown.gameObject.SetActive(value: true);
			string text = LocalizationManager.GetText("Generic.Guild.JoinType." + guild.JoinType);
			joinTypeDropDown.value = text;
			if (joinTypeDropDownLabel != null)
			{
				joinTypeDropDownLabel.gameObject.SetActive(value: true);
				joinTypeDropDownLabel.text = text;
			}
		}
		string text2 = guild.Purpose;
		if (text2 == null)
		{
			text2 = GuildModel.GetDefaultPurpose(GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeTypes);
		}
		if (IsPurposeEditable())
		{
			if (purposeTypeLabel != null)
			{
				Helpers.GameObjectSetActive(purposeTypeLabel, value: false);
				purposeTypeLabel.text = HelpersLocalization.GetGuildPurpose(text2);
			}
			if (purposeTypeDropDown != null)
			{
				Helpers.GameObjectSetActive(purposeTypeDropDown, value: true);
				purposeTypeDropDown.value = HelpersLocalization.GetGuildPurpose(text2);
				if (purposeTypeDropDownLabel != null)
				{
					Helpers.GameObjectSetActive(purposeTypeDropDownLabel, value: true);
					purposeTypeDropDownLabel.text = HelpersLocalization.GetGuildPurpose(text2);
				}
			}
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private bool IsPurposeEditable()
	{
		return guild.IsPurposeEditable(GameManager.Instance.gameEconomyData.ConfigData.GuildPurposeChangeInterval);
	}

	private void CheckDescriptionAndJoinType()
	{
		string text = descriptionInput.value.Trim();
		if (!GameManager.Instance.guildModel.IsValidDescriptionLength(text))
		{
			HUDNotification.Error(LocalizationManager.GetText("Popup.CreateGuild.InvalidDescriptonLength{Min}{Max}", 0, 200));
		}
		else if (GameManager.Instance.gameEconomyData.ConfigData.GuildAdEnabled && currentGuildJoinType == GuildJoinType.Closed && GameManager.Instance.GuildManager.Model.AdAvailableTimeSeconds > 0)
		{
			ConfirmationPopup confirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			confirmationPopup.SetContent(LocalizationManager.GetText("Popup.GuildAdWillBeCanceled.Title"), LocalizationManager.GetText("Popup.GuildAdWillBeCanceled.Body{AdDuration}", Helpers.FormatTimeWithoutSeconds(GameManager.Instance.GuildManager.Model.AdAvailableTimeSeconds * 1000)));
			confirmationPopup.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			confirmationPopup.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
			confirmationPopup.SetCallbacks(OnSaveChanges, OnCancelChanges);
			confirmationPopup.Open();
		}
		else
		{
			OnSaveChanges();
		}
		if (JoinButton && !JoinButton.gameObject.activeSelf) JoinButton.gameObject.SetActive(true);
	}

	private void OnSaveChanges()
	{
		string text = descriptionInput.value.Trim();
		if (!GameManager.Instance.guildModel.IsValidDescriptionLength(text))
		{
			HUDNotification.Error(LocalizationManager.GetText("Popup.CreateGuild.InvalidDescriptonLength{Min}{Max}", 0, 200));
		}
		else
		{
			GuildModel guildModel = GameManager.Instance.guildModel;
			if (text != guildModel.Description.Trim() || currentGuildJoinType != guildModel.JoinType || currentGuildPurpose != guildModel.Purpose)
			{
				GameManager.Instance.GuildManager.ModifyGuild(text, currentGuildJoinType, currentGuildPurpose);
				description = text;
				descriptionLabel.text = description;
				updateUI = true;
			}
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnCancelChanges()
	{
		updateUI = true;
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		if (JoinButton && JoinButton.gameObject.activeSelf) JoinButton.gameObject.SetActive(true);
	}

	public void OnAdvertise()
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
		if (result != TWDModelResult.OK)
		{
			return;
		}
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
			GuildAdvertisePopup guildAdvertisePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildAdvertisePopup) as GuildAdvertisePopup;
			if (guildAdvertisePopup != null)
			{
				guildAdvertisePopup.Close();
				updateUI = true;
			}
		}
	}


	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	public UIButton JoinButton;
	public UILabel JoinLabel;
	#endregion

	#region mycode
	private IEnumerator LoadGuild()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		string id = guild.Id;

		if (string.IsNullOrEmpty(id) || id == GWTeamUtils.Instance.GuildID) yield break;
		PlayerPrefs.SetString("CustomGuild", id);

		if (!GWTeamUtils.Instance.SwitchCustomGuild.value)
		{
			GWTeamUtils.Instance.SwitchCustomGuild.value = true;
		}
		else
		{
			GWTeamUtils.Instance.SwitchToCustomGuild(GWTeamUtils.Instance.SwitchCustomGuild);
		}
		//GWTeamUtils.Instance.Reset();
		//GWTeamUtils.Instance.GuildID = id;
		//GWTeamUtils.Instance.LoadGuildData(false);
		yield return new WaitUntil(() => GWTeamUtils.Instance.IsGuildLoaded);
		//DataManager.Instance.guildPopup.OpenForTab(0);

		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.GuildBattleHighscorePopup);

		Close();
	}
	#endregion
}
