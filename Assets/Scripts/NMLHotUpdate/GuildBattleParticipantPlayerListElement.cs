using TWDModel;
using UnityEngine;

public class GuildBattleParticipantPlayerListElement : NUIListItem<string>
{
	[SerializeField]
	private UILabel playerNameLable;

	[SerializeField]
	private UILabel statusLabel;

	[SerializeField]
	private UISprite background;

	[SerializeField]
	private PlayerEmblemIcon playerEmblemIcon;

	public override void UpdateUI()
	{
		string data = GetData();
		GuildModel guildModel = GameManager.Instance.guildModel;
		if (guildModel == null)
		{
			return;
		}
		for (int i = 0; i < guildModel.GuildMembers.Count; i++)
		{
			GuildMemberInfo guildMemberInfo = guildModel.GuildMembers[i];
			if (guildMemberInfo.MemberId.Equals(data))
			{
				UpdateData(guildMemberInfo, guildMemberInfo.MemberId.Equals(GameManager.Instance.playerModel.HashedId));
			}
		}
	}

	private void UpdateData(GuildMemberInfo memberInfo, bool isPlayer)
	{
		HelpersUI.SetContentToLabel(playerNameLable, GameManager.Instance.GetFilteredText(memberInfo.Name));
		HelpersUI.SetContentToLabel(statusLabel, HelpersLocalization.GetGuildMemberRole(memberInfo));
		background.color = (isPlayer ? SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.IsPlayerColor : SingularityMonoBehaviour<GuildWarManager>.Instance.GuildBattleVisualConfig.IsNotPlayer);
		if (playerEmblemIcon != null)
		{
			playerEmblemIcon.SetEmblem(memberInfo.PlayerEmblem);
		}
	}
}
