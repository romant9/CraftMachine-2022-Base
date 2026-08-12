using System;
using TWDModel;
using UnityEngine;

public class GuildKickPopup : HUDElement
{
	[SerializeField]
	private Checkbox banCheckbox;

	[SerializeField]
	private UILabel banCheckboxLabel;

	[SerializeField]
	private UILabel messageLabel;

	private GuildMemberInfo guildMemberInfo;

	public void Show(string message, GuildMemberInfo member)
	{
		messageLabel.text = message;
		guildMemberInfo = member;
		TimeSpan duration = TimeSpan.FromMinutes(GameManager.Instance.gameEconomyData.ConfigData.GWKickSoftBanDurationMinutes);
		banCheckboxLabel.text = LocalizationManager.GetText("Popup.GuildSoftBan.Message", Helpers.FormatReadableTime(duration));
		Open();
	}

	public void ConfirmClick()
	{
		OnKickOutConfirmed(banCheckbox.IsOn);
		Close();
	}

	private void OnKickOutConfirmed(bool softBan)
	{
		if (guildMemberInfo != null)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IngameLoading).Open();
			GameManager.Instance.GuildManager.KickOutGuildMember(guildMemberInfo.MemberId, softBan);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/member_kick");
		}
	}
}
