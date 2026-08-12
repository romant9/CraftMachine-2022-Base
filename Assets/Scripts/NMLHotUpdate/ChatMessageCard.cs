using System.Linq;
using TWDModel;
using UnityEngine;

public class ChatMessageCard : UIListCard<ChatMessage>
{
	[SerializeField]
	[Tooltip("The color of the background. 0 & 1: The normal message background. 2: special message background.")]
	private Color[] backgroundColors;

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UILabel playerNameLabel;

	[SerializeField]
	private UILabel playerRoleLabel;

	[SerializeField]
	private UILabel messageLabel;

	[SerializeField]
	private UILabel timePostedLabel;

	[SerializeField]
	private GameObject containerNormalMessage;

	[SerializeField]
	private GameObject pinMessageContainer;

	[SerializeField]
	private Color pinMessageColor;

	[SerializeField]
	private GameObject pinIndicatorContainer;

	public override void UpdateUI()
	{
		base.UpdateUI();
		string filteredText = GameManager.Instance.GetFilteredText(base.Item.Name);
		string filteredText2 = GameManager.Instance.GetFilteredText(base.Item.SenderName);
		playerNameLabel.text = filteredText;
		GuildMemberInfo memberInfo = GameManager.Instance.guildModel.GetMemberInfo(base.Item.PlayerId);
		if (memberInfo == null)
		{
			playerRoleLabel.text = "";
		}
		else
		{
			playerRoleLabel.text = HelpersLocalization.GetGuildMemberRole(memberInfo);
		}
		if (GameManager.Instance.guildModel != null)
		{
			timePostedLabel.text = Helpers.FormatTimeAgo(GameManager.Instance.guildModel.LifeTime, base.Item.Time);
		}
		if (base.Item.IsBothTypesNone)
		{
			containerNormalMessage.SetActive(value: true);
			messageLabel.text = base.Item.Message;
		}
		else
		{
			background.color = backgroundColors[2];
			containerNormalMessage.SetActive(value: false);
			if (!string.IsNullOrEmpty(filteredText2))
			{
				messageLabel.text = LocalizationManager.GetText("Generic.Guild.Notification." + base.Item.EitherTypeAsString + "{SenderName}{TargetName}", filteredText2, filteredText);
			}
			else
			{
				messageLabel.text = LocalizationManager.GetText("OLD_Generic.Guild.Notification." + base.Item.EitherTypeAsString + "{PlayerName}", filteredText);
			}
		}
		GuildMemberInfo memberInfo2 = GameManager.Instance.guildModel.GetMemberInfo(GameManager.Instance.playerModel.HashedId);
		bool active = memberInfo2.Role == GuildMemberRole.Leader || memberInfo2.Role == GuildMemberRole.CoLeader;
		pinMessageContainer.SetActive(active);
		pinIndicatorContainer.SetActive(base.Item.IsPinned);
		background.ResetAndUpdateAnchors();
	}

	public void SetIndex(int index)
	{
		Color color = backgroundColors[0];
		color = ((!base.Item.IsBothTypesNone) ? backgroundColors[2] : backgroundColors[index % 2]);
		if (base.Item.IsPinned)
		{
			color = pinMessageColor;
		}
		background.color = color;
		UIButton component = GetComponent<UIButton>();
		if (component != null)
		{
			component.defaultColor = color;
			component.hover = color;
			component.pressed = color;
		}
	}

	public void OnClickInfo()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		GuildPlayerInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialGuildPlayerInfoPopup) as GuildPlayerInfoPopup;
		obj.GuildMemberId = base.Item.PlayerId;
		obj.Open();
	}

	public void OnClickPin()
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		if (base.Item.IsPinned)
		{
			((PinMessageConfirmationPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ChatPinnedMessageConfirmationPopup)).Unpin(base.Item);
			return;
		}
		ChatMessage chatMessage = GameManager.Instance.guildModel.ChatMessages.FirstOrDefault((ChatMessage x) => x.IsPinned);
		if (chatMessage != null)
		{
			((PinMessageConfirmationPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ChatPinnedMessageConfirmationPopup)).Replace(chatMessage, base.Item);
			return;
		}
		Helpers.ExecuteCommand(new TogglePinnedChatMessageCommand
		{
			SenderName = base.Item.SenderName,
			MsgTime = base.Item.Time,
			Message = base.Item.Message
		});
	}

	public void UpdateUI(ChatMessage item)
	{
		base.Item = item;
		UpdateUI();
	}

	public void HidePinIndicators()
	{
		pinMessageContainer.SetActive(value: false);
		pinIndicatorContainer.SetActive(value: false);
	}
}
