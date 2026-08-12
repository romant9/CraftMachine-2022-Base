using TwdCustomMod;
using UnityEngine;

public class GuildPlayerListCardBase : UIListCard<ScoreDataEntry>
{
	public enum GuildPlayerListCardType
	{
		MemberList = 0,
		GuildPlayerList = 1,
		PlayerList = 2,
		FriendList = 3,
		GuildList = 4,
		PlayerListEndless = 5,
		PlayerLocations = 6,
		TeamLocations = 7
	}

	[SerializeField]
	protected UILabel nameLabel;

	[SerializeField]
	protected UILabel memberTypeLabel;

	[SerializeField]
	protected UILabel levelLabel;

	[SerializeField]
	protected UILabel GuildNameLabel;

	[SerializeField]
	protected UISprite backgroundSprite;

	[SerializeField]
	protected Color defaultBgColor;

	[SerializeField]
	protected Color highlightBgColor;

	public GuildPlayerListCardType Type { get; set; }

	public override void UpdateUI()
	{
		base.UpdateUI();
		HelpersUI.SetContentToLabel(nameLabel, GameManager.Instance.GetFilteredText(base.Item.Name));
		if (backgroundSprite != null)
		{
			if (OfflineManager.IsLoadDataManager && (Type == GuildPlayerListCardType.PlayerLocations || Type == GuildPlayerListCardType.TeamLocations))
			{
				DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager && (Type == GuildPlayerListCardType.PlayerLocations || Type == GuildPlayerListCardType.TeamLocations))");
				bool flag = GWTeamsManager.Instance.currentPlayerID == base.Item.Id;
				backgroundSprite.color = flag ? highlightBgColor : defaultBgColor;
				if (flag) GWTeamsManager.Instance.GWProtList.currentCardPosition = transform.localPosition.y;
			}
			else
			{
				bool flag = false;
				if (Type == GuildPlayerListCardType.GuildList && GameManager.Instance.playerModel.IsGuildMember && base.Item.Id == GameManager.Instance.playerModel.GuildId)
				{
					flag = true;
				}
				else if (Type != GuildPlayerListCardType.GuildList && base.Item.Id == GameManager.Instance.playerModel.HashedId)
				{
					flag = true;
				}
				backgroundSprite.color = (flag ? highlightBgColor : defaultBgColor);
			}
		}
	}

	public void OnClick()
	{
		if (Type == GuildPlayerListCardType.FriendList || Type == GuildPlayerListCardType.PlayerList || Type == GuildPlayerListCardType.MemberList || Type == GuildPlayerListCardType.GuildPlayerList)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			GuildPlayerInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialGuildPlayerInfoPopup) as GuildPlayerInfoPopup;
			obj.GuildMemberInfo = (base.Item as PlayerScoreDataEntry)?.MemberInfo;
			obj.Type = Type;
			obj.Open();
		}
		else if (Type == GuildPlayerListCardType.GuildList && base.Item is GuildScoreDataEntry guildScoreDataEntry && !string.IsNullOrEmpty(guildScoreDataEntry.Id))
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			GuildInfoPopup.OpenForGuildId(guildScoreDataEntry.Id);
		}
		else if (OfflineManager.IsLoadDataManager && (Type == GuildPlayerListCardType.PlayerLocations || Type == GuildPlayerListCardType.TeamLocations))
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager && (Type == GuildPlayerListCardType.PlayerLocations || Type == GuildPlayerListCardType.TeamLocations))");
			GWTeamsManager.Instance.currentPlayerID = base.Item.Id;
			int tgIndex;
			if (Type == GuildPlayerListCardType.TeamLocations)
			{
				tgIndex = GWTeamUtils.GetTeamIndex(((SectorsDataEntry)Item).TeamIndex);
				GWTeamsManager.Instance.teamSortedPlayerIndex = ((SectorsDataEntry)Item).TeamSortedPlayerIndex;
				GWTeamsManager.Instance.pvpTeamsToggleSet.SetSelectedIndex(tgIndex);
				DebugTWD.Log("TeamSortedPlayerIndex is " + GWTeamsManager.Instance.teamSortedPlayerIndex);
			}
			else
			{
				GWTeamsManager.Instance.currentPlayerBaseIndex = ((SectorsDataEntry)Item).BasePlayerIndex;
				tgIndex = GWTeamsManager.Instance.pvpTeamsToggleSet.GetSelectedIndex();
				DebugTWD.Log("BasePlayerIndex is " + ((SectorsDataEntry)Item).BasePlayerIndex);
			}

			UIButtonToggle tg = GWTeamsManager.Instance.pvpTeamsToggleSet.GetButton(tgIndex);
			GWTeamsManager.Instance.SetTeamDataFromToggle(tg, true);
			GWTeamsManager.Instance.GWProtList.UpdateUIImmediate();
		}
	}
}
