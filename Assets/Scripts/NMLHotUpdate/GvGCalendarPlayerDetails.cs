using TWDModel;
using UnityEngine;

public class GvGCalendarPlayerDetails : MonoBehaviour
{
	[SerializeField]
	private PlayerEmblemIcon playerEmblemIcon;

	[SerializeField]
	private UILabel playerName;

	[SerializeField]
	private GameObject currentPlayerHighlight;

	[SerializeField]
	private GameObject onlineStatus;

	[SerializeField]
	private GameObject emptySlotGameObject;

	[SerializeField]
	private GameObject playerDetails;

	[SerializeField]
	private UIButtonExtended playerKickButton;

	private GuildMemberInfo playerMemberInfo;

	private GuildMemberInfo guildMemberInfo;

	private long currentSelectedTimeslot;

	public void Awake()
	{
		currentSelectedTimeslot = -1L;
		playerMemberInfo = GameManager.Instance.guildModel.GetMemberInfo(GameManager.Instance.playerModel.HashedId);
	}

	public void SetPlayerInfo(string playerHashedId, long selectedTimeslot)
	{
		guildMemberInfo = GameManager.Instance.guildModel.GetMemberInfo(playerHashedId);
		currentSelectedTimeslot = selectedTimeslot;
		if (guildMemberInfo != null && playerMemberInfo != null)
		{
			bool flag = GameManager.Instance.playerModel.HashedId == playerHashedId;
			Helpers.GameObjectSetActive(playerDetails, value: true);
			Helpers.GameObjectSetActive(emptySlotGameObject, value: false);
			HelpersUI.SetContentToLabel(playerName, flag ? guildMemberInfo.Name : GameManager.Instance.GetFilteredText(guildMemberInfo.Name));
			Helpers.GameObjectSetActive(currentPlayerHighlight, flag);
			Helpers.GameObjectSetActive(onlineStatus, guildMemberInfo.IsOnline(GameManager.Instance.playerModel.UtcTimeStamp));
			playerEmblemIcon.SetEmblem(guildMemberInfo.PlayerEmblem);
			if (playerMemberInfo.Role > GuildMemberRole.Elder && playerMemberInfo.Role > guildMemberInfo.Role && !flag && !GuildWarHelper.IsLockDownTimeForTimeSlotClientSide(selectedTimeslot))
			{
				Helpers.GameObjectSetActive(playerKickButton.gameObject, value: true);
				playerKickButton.SetClickCallback(OnClickRemove);
			}
			else
			{
				Helpers.GameObjectSetActive(playerKickButton.gameObject, value: false);
			}
		}
		else
		{
			SetFreeSlot();
		}
	}

	public void SetFreeSlot()
	{
		Helpers.GameObjectSetActive(playerDetails, value: false);
		Helpers.GameObjectSetActive(emptySlotGameObject, value: true);
		playerKickButton.RemoveClickCallback(OnClickRemove);
		Helpers.GameObjectSetActive(playerKickButton.gameObject, value: false);
	}

	private void OnClickRemove(UIButtonExtended button)
	{
		GvGRemovePlayerConfirmationPopup gvGRemovePlayerConfirmationPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.GvGRemovePlayerConfirmationPopUp) as GvGRemovePlayerConfirmationPopup;
		if (gvGRemovePlayerConfirmationPopup != null)
		{
			gvGRemovePlayerConfirmationPopup.Open();
			gvGRemovePlayerConfirmationPopup.SetContent(guildMemberInfo, currentSelectedTimeslot);
		}
	}
}
