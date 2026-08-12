using TWDModel;
using UnityEngine;

public class GvGRemovePlayerConfirmationPopup : HUDElement
{
	[SerializeField]
	private UIButtonExtended closeButton;

	[SerializeField]
	private UIButtonExtended removeButton;

	[SerializeField]
	private UILabel playerNickNameLabel;

	[SerializeField]
	private UILabel battleSlotTitle;

	[SerializeField]
	private PlayerEmblemIcon removedPlayerEmblem;

	private GuildMemberInfo removedPlayerMemberInfo;

	private long CurrentTimeSlot;

	public override void Start()
	{
		base.Start();
		closeButton.SetClickCallback(OnClickClose);
	}

	public void SetContent(GuildMemberInfo removedPlayer, long timeSlot)
	{
		int num = GuildWarHelper.GetWarDayIndexByTimeslot(timeSlot) + 1;
		removedPlayerMemberInfo = removedPlayer;
		CurrentTimeSlot = timeSlot;
		battleSlotTitle.text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.Hub.Calendar.RemovePlayer.Title{BattleNumber}", num);
		playerNickNameLabel.text = removedPlayer.Name;
		removeButton.SetClickCallback(OnClickRemove);
		removedPlayerEmblem.SetEmblem(removedPlayerMemberInfo.PlayerEmblem);
	}

	private void OnClickClose(UIButtonExtended uiButtonExtended)
	{
		removeButton.RemoveClickCallback(OnClickRemove);
		Close();
	}

	private void OnClickRemove(UIButtonExtended uiButtonExtended)
	{
		SingularityMonoBehaviour<GuildWarManager>.Instance.RemovePlayerFromGuildBattle(CurrentTimeSlot, removedPlayerMemberInfo.MemberId);
		Close();
	}
}
