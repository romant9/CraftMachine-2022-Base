using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildPlayerInfoPopup : HUDElement
{
	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private UILabel guildNameLabel;

	[SerializeField]
	private UILabel levelLabel;

	[SerializeField]
	private UILabel roleLabel;

	[SerializeField]
	private UILabel bestSurvivorLabel;

	[SerializeField]
	private UILabel allChallengeStarsAmountLabel;

	[SerializeField]
	private GameObject allTimeStarsParent;

	[SerializeField]
	private LocalizationUIUpdater allTimeStarsLocalizationUpdater;

	[SerializeField]
	private LocalizationUIUpdater currentStarsLocalizationUpdater;

	[SerializeField]
	private UILabel challengeStarsAmountLabel;

	[SerializeField]
	private GameObject currentChallengeStarsParent;

	[SerializeField]
	private UILabel noStarsLabel;

	[SerializeField]
	private UILabel lastActiveDateLabel;

	[SerializeField]
	private UISprite tierEmblem;

	[SerializeField]
	private PlayerEmblemIcon playerEmblemIcon;

	[SerializeField]
	private UILabel allTimeVPAmount;

	[SerializeField]
	private UILabel currentSeasonVPAmount;

	private RefuseRequestMembershipAction refuseRequestMembershipAction;

	private AcceptRequestMembershipAction acceptRequestMembershipAction;

	public GuildPlayerListCardBase.GuildPlayerListCardType Type;

	public GuildMemberInfo GuildMemberInfo { get; set; }

	public string GuildMemberId { get; set; }

	public GuildModel CurrentGuild { get; set; }

	public bool IsInMyGuild
	{
		get
		{
			if (CurrentGuild != null)
			{
				return CurrentGuild.Id == GuildMemberInfo.GuildId;
			}
			return false;
		}
	}

	public override void Open()
	{
		base.Open();
		if (defaultPopup != null)
		{
			defaultPopup.AllowNormalClosing(active: false);
		}
		CurrentGuild = GameManager.Instance.guildModel;
		if (CurrentGuild != null)
		{
			CurrentGuild.Changed += OnGuildChanged;
			if (GuildMemberInfo == null && GuildMemberId != null)
			{
				GuildMemberInfo = CurrentGuild.GetMemberInfo(GuildMemberId);
			}
		}
		if (GuildMemberInfo == null)
		{
			HUDNotification.Error(LocalizationManager.GetText("Error.PlayerGuildNotFound"));
			Close();
			return;
		}
		if (lastActiveDateLabel != null)
		{
			bool flag = GuildMemberInfo.MemberId == GameManager.Instance.playerModel.HashedId;
			lastActiveDateLabel.gameObject.SetActive(!flag);
			if (!flag)
			{
				int num = 0;
				string text = "";
				if (GuildMemberInfo.LastActiveDate > 0)
				{
					num = (int)((GameManager.Instance.playerModel.UtcTimeStamp - GuildMemberInfo.LastActiveDate) / 86400000);
					if (num == 0)
					{
						text = LocalizationManager.GetText("Popup.GuildPlayerInfo.LastActiveDateLessThanAday");
					}
					else if (num == 1)
					{
						text = LocalizationManager.GetText("Popup.GuildPlayerInfo.LastActiveDateOneDay");
					}
					else if (num > 1)
					{
						text = LocalizationManager.GetText("Popup.GuildPlayerInfo.LastActiveDateMultipleDays{days}", num);
					}
				}
				lastActiveDateLabel.text = text;
			}
			else
			{
				GameManager.Instance.GuildManager?.UpdateGvGRelatedInfo();
			}
		}
		UpdateUI();
	}

	public override void Close()
	{
		base.Close();
		refuseRequestMembershipAction = null;
		acceptRequestMembershipAction = null;
		GuildMemberInfo = null;
		GuildMemberId = null;
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
		if (CurrentGuild != null)
		{
			CurrentGuild.Changed -= OnGuildChanged;
		}
	}

	private void OnGuildChanged(GroupModelBase groupModelBase, string changed, object args)
	{
		switch (changed)
		{
		case "MemberAdded":
		case "MemberRemoved":
		case "MemberAccepted":
		case "MemberRefused":
		case "MemberChanged":
			UIEvent.Send("SocialGuildPlayerChanged");
			Close();
			break;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (GuildMemberInfo != null)
		{
			nameLabel.text = GameManager.Instance.GetFilteredText(GuildMemberInfo.Name);
		}
		bool flag = GuildMemberInfo.State == GuildMemberState.Normal || !string.IsNullOrEmpty(GuildMemberInfo.GuildLeaderboardName);
		roleLabel.gameObject.SetActive(flag);
		if (flag)
		{
			if (GuildMemberInfo.State == GuildMemberState.Normal)
			{
				guildNameLabel.text = GameManager.Instance.GetFilteredText(GameManager.Instance.guildModel.Name);
			}
			else
			{
				guildNameLabel.text = GameManager.Instance.GetFilteredText(GuildMemberInfo.GuildLeaderboardName);
			}
			roleLabel.text = HelpersLocalization.GetGuildMemberRole(GuildMemberInfo);
		}
		else
		{
			guildNameLabel.text = "-";
		}
		noStarsLabel.gameObject.SetActive(GuildMemberInfo.ExcludedFromChallenge);
		if (GuildMemberInfo.PlayerLevel == 0)
		{
			levelLabel.text = "";
		}
		else
		{
			levelLabel.text = LocalizationManager.GetText("Generic.Level{Level}", GuildMemberInfo.PlayerLevel);
		}
		allChallengeStarsAmountLabel.text = GuildMemberInfo.TotalChallengeStars.ToString();
		challengeStarsAmountLabel.text = GuildMemberInfo.CurrentChallengeStars.ToString();
		int value = 0;
		if (CurrentGuild != null && CurrentGuild.GvGSeasonModel != null)
		{
			CurrentGuild.GvGSeasonModel.SeasonTotalVpAccumulatedPerPlayer.TryGetValue(GuildMemberInfo.MemberId, out value);
		}
		currentSeasonVPAmount.text = value.ToString();
		allTimeVPAmount.text = GuildMemberInfo.TotalVP.ToString();
		if (Type == GuildPlayerListCardBase.GuildPlayerListCardType.FriendList)
		{
			Helpers.GameObjectSetActive(currentChallengeStarsParent, value: false);
			allTimeStarsParent.transform.localPosition = new Vector3(0f, -40f, 0f);
		}
		else
		{
			Helpers.GameObjectSetActive(currentChallengeStarsParent, value: true);
			allTimeStarsParent.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		if (Type == GuildPlayerListCardBase.GuildPlayerListCardType.FriendList || Type == GuildPlayerListCardBase.GuildPlayerListCardType.PlayerList)
		{
			allTimeStarsLocalizationUpdater.LocalizationKey = "Popup.GuildPlayerInfo.ChallengeStars";
			currentStarsLocalizationUpdater.LocalizationKey = "Popup.GuildPlayerInfo.CurrentChallengeStars";
		}
		else
		{
			allTimeStarsLocalizationUpdater.LocalizationKey = "Popup.GuildPlayerInfo.ChallengeStarsInGuild";
			currentStarsLocalizationUpdater.LocalizationKey = "Popup.GuildPlayerInfo.CurrentChallengeStarsInGuild";
		}
		defaultPopup.ShowSimpleButtons(show: false);
		if (GuildMemberInfo.State == GuildMemberState.PendingRequest)
		{
			if (CurrentGuild.CanAcceptRequests(GameManager.Instance.playerModel.HashedId))
			{
				defaultPopup.ShowSimpleButtons(show: true);
				defaultPopup.SetSimplePositiveButton(available: true, LocalizationManager.GetText("Popup.Guild.Button.AcceptRequestMembership"), OnAcceptRequestMembership);
				defaultPopup.SetSimpleNegativeButton(available: true, LocalizationManager.GetText("Popup.Guild.Button.RefuseRequestMembership"), OnRefuseRequestMembership);
				defaultPopup.SetQuestion(LocalizationManager.GetText("Popup.Guild.AcceptRequestMembership"));
			}
		}
		else if (GuildMemberInfo != null)
		{
			if (GameManager.Instance.playerModel.HashedId == GuildMemberInfo.MemberId)
			{
				defaultPopup.ShowSimpleButtons(show: false);
			}
			else if (!IsInMyGuild)
			{
				defaultPopup.ShowSimpleButtons(show: false);
			}
			else if (CurrentGuild != null && CurrentGuild.CanKickOut(GameManager.Instance.playerModel.HashedId))
			{
				defaultPopup.ShowSimpleButtons(show: true);
				if (GuildMemberInfo.Role == GuildMemberRole.Normal)
				{
					defaultPopup.SetSimplePositiveButton(available: true, LocalizationManager.GetText("Popup.Guild.Button.Promote"), delegate
					{
						OnPromote(GuildMemberRole.Elder);
					});
					defaultPopup.SetSimpleNegativeButton(available: true, LocalizationManager.GetText("Popup.Guild.Button.KickOut"), OnKickOut);
				}
				else if (GuildMemberInfo.Role == GuildMemberRole.Elder)
				{
					if (CurrentGuild.GetMemberRole(GameManager.Instance.playerModel.HashedId) == GuildMemberRole.Leader)
					{
						defaultPopup.SetSimplePositiveButton(available: true, LocalizationManager.GetText("Popup.Guild.Button.Promote"), delegate
						{
							OnPromote(GuildMemberRole.CoLeader);
						});
					}
					else
					{
						defaultPopup.SetSimplePositiveButton(available: false);
					}
					defaultPopup.SetSimpleNegativeButton(available: true, LocalizationManager.GetText("Popup.Guild.Button.Demote"), delegate
					{
						OnDemote(GuildMemberRole.Normal);
					});
				}
				else if (GuildMemberInfo.Role == GuildMemberRole.CoLeader)
				{
					if (CurrentGuild.GetMemberRole(GameManager.Instance.playerModel.HashedId) == GuildMemberRole.Leader)
					{
						defaultPopup.SetSimplePositiveButton(available: true, LocalizationManager.GetText("Popup.Guild.Button.PromoteLeader"), OnPromoteToLeader);
						defaultPopup.SetSimpleNegativeButton(available: true, LocalizationManager.GetText("Popup.Guild.Button.Demote"), delegate
						{
							OnDemote(GuildMemberRole.Elder);
						});
					}
					else
					{
						defaultPopup.SetSimplePositiveButton(available: false);
						defaultPopup.SetSimpleNegativeButton(available: false);
					}
				}
				else
				{
					defaultPopup.SetSimplePositiveButton(available: false);
					defaultPopup.SetSimpleNegativeButton(available: false);
				}
				defaultPopup.SetQuestion(null);
			}
		}
		if (tierEmblem != null)
		{
			string tierEmblemIconName = HelpersGfx.GetTierEmblemIconName((GameManager.Instance.playerModel.CurrentOutpostTier != null) ? GameManager.Instance.playerModel.CurrentOutpostTier.Id : "");
			if (!string.IsNullOrEmpty(tierEmblemIconName) && IsInMyGuild)
			{
				tierEmblem.spriteName = tierEmblemIconName;
				tierEmblem.gameObject.SetActive(value: true);
			}
			else
			{
				tierEmblem.gameObject.SetActive(value: false);
			}
		}
		if (playerEmblemIcon != null)
		{
			playerEmblemIcon.SetEmblem(GuildMemberInfo.PlayerEmblem);
		}
	}

	private void OnPromote(GuildMemberRole memberRole)
	{
		ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.GuildPromoteConfirmation.Title"), LocalizationManager.GetText($"Popup.GuildPromoteConfirmation.{memberRole}.Message{{Name}}", GameManager.Instance.GetFilteredText(GuildMemberInfo.Name)), LocalizationManager.GetText("Button.Yes"), delegate
		{
			OnPromoteConfirmed(memberRole);
		}, LocalizationManager.GetText("Button.Cancel"));
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnPromoteConfirmed(GuildMemberRole memberRole)
	{
		GameManager.Instance.GuildManager.ModifyGuildMemberRole(GuildMemberInfo.MemberId, memberRole);
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/member_promote");
	}

	private void OnPromoteToLeader()
	{
		ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.GuildPromoteLeaderConfirmation.Title"), LocalizationManager.GetText("Popup.GuildPromoteConfirmation.Leader.Message{Name}", GameManager.Instance.GetFilteredText(GuildMemberInfo.Name)), LocalizationManager.GetText("Button.Yes"), OnPromoteLeaderConfirmed, LocalizationManager.GetText("Button.Cancel"));
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnPromoteLeaderConfirmed()
	{
		GameManager.Instance.GuildManager.PromoteMemberToLeader(GuildMemberInfo.MemberId);
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/member_promote");
	}

	private void OnDemote(GuildMemberRole memberRole)
	{
		ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.GuildDemoteConfirmation.Title"), LocalizationManager.GetText($"Popup.GuildDemoteConfirmation.{memberRole}.Message{{Name}}", GameManager.Instance.GetFilteredText(GuildMemberInfo.Name)), LocalizationManager.GetText("Button.Yes"), delegate
		{
			OnDemoteConfirmed(memberRole);
		}, LocalizationManager.GetText("Button.Cancel"));
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
	}

	private void OnDemoteConfirmed(GuildMemberRole memberRole)
	{
		GameManager.Instance.GuildManager.ModifyGuildMemberRole(GuildMemberInfo.MemberId, memberRole);
		SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/member_demote");
	}

	private void OnKickOut()
	{
		if (!HandleLeaveGuildBehaviourDuringGuildBattle(GuildMemberInfo))
		{
			StartKickOutPopupFlow("Popup.GuildKickOutConfirmation.Message");
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		}
	}

	private void OnRefuseRequestMembership()
	{
		refuseRequestMembershipAction = new RefuseRequestMembershipAction(GuildMemberInfo.MemberId);
	}

	private void OnAcceptRequestMembership()
	{
		acceptRequestMembershipAction = new AcceptRequestMembershipAction(GuildMemberInfo.MemberId);
	}

	public void OnOpenGuild()
	{
		if (!IsInMyGuild && !string.IsNullOrEmpty(GuildMemberInfo.GuildId))
		{
			string guildId = GuildMemberInfo.GuildId;
			Close();
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			GuildInfoPopup.OpenForGuildId(guildId);
		}
	}

	private bool HandleLeaveGuildBehaviourDuringGuildBattle(GuildMemberInfo memberInfo)
	{
		bool result = false;
		if (memberInfo != null && GuildWarHelper.IsPlayerRegisteredForBattle(memberInfo.MemberId))
		{
			if (GuildWarHelper.IsLockdownTimeForCurrentBattle())
			{
				AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.DeniedKickOut.Title"), LocalizationManager.GetText("Popup.DeniedKickOutGuildBattleLockdown.Message"), LocalizationManager.GetText("Button.Ok"));
			}
			else if (GuildWarHelper.IsBattleOnGoing())
			{
				AlertPopup.ShowPopup(LocalizationManager.GetText("Popup.DeniedKickOut.Title"), LocalizationManager.GetText("Popup.DeniedKickOutGuildBattleActive.Message"), LocalizationManager.GetText("Button.Ok"));
			}
			else
			{
				StartKickOutPopupFlow("Popup.GuildKickOutConfirmationRegisteredForBattle.Message");
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			}
			result = true;
		}
		return result;
	}

	private void StartKickOutPopupFlow(string messageKey)
	{
		GuildKickPopup guildKickPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GuildKickPopup) as GuildKickPopup;
		if ((bool)guildKickPopup)
		{
			guildKickPopup.Show(LocalizationManager.GetText(messageKey), GuildMemberInfo);
		}
	}
}
