using TWDModel;
using UnityEngine;

public class GuildBattleHighscoreListItem : NUIListItem<ScoreDataEntry>
{
	[SerializeField]
	private UILabel positionLabel;

	[SerializeField]
	private UILabel nameLabel;

	[SerializeField]
	private GameObject scoreContainer;

	[SerializeField]
	private UILabel vpAmountLabel;

	[SerializeField]
	private UILabel noVpLabel;

	[SerializeField]
	private PlayerEmblemIcon playerEmblem;

	[Space(20f)]
	[Header("Optional features")]
	[Header("Player")]
	[SerializeField]
	private GameObject isPlayerContainer;

	[Header("Guild")]
	[SerializeField]
	private GameObject isGuildContainer;

	[Header("Else")]
	[SerializeField]
	private GameObject regularContainer;

	[SerializeField]
	private UIButtonExtended button;

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (GetData() == null)
		{
			return;
		}
		int num = GetIndexValue() + 1;
		bool flag = GetData() is GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry && guildBattlePlayersScoreDataEntry.PointsWithHeld;
		HelpersUI.SetContentToLabel(positionLabel, num.ToString());
		HelpersUI.SetContentToLabel(nameLabel, GameManager.Instance.GetFilteredText(GetData().Name));
		HelpersUI.SetContentToLabel(vpAmountLabel, GetData().Score.ToString());
		Helpers.GameObjectSetActive(scoreContainer, !flag);
		Helpers.GameObjectSetActive(noVpLabel, flag);
		bool flag2 = GameManager.Instance.playerModel.HashedId == GetData().Id;
		Helpers.GameObjectSetActive(isPlayerContainer, flag2);
		Helpers.GameObjectSetActive(regularContainer, !flag2);
		SetBadge(GetData());
		if (!flag2)
		{
			bool flag3 = false;
			if (GameManager.Instance.playerModel.IsGuildMember)
			{
				flag3 = GameManager.Instance.playerModel.GuildId == GetData().Id;
			}
			Helpers.GameObjectSetActive(isGuildContainer, flag3);
			Helpers.GameObjectSetActive(regularContainer, !flag3);
		}
	}

	public override void SetPosition(Vector3 newPosition, int newIndex = -1)
	{
		base.SetPosition(newPosition, newIndex);
		UpdateUI();
	}

	public void OnButtonClick(UIButtonExtended button)
	{
		if (GetData() == null || string.IsNullOrEmpty(GetData().Id) || !GameManager.Instance.playerModel.IsGuildMember || GameManager.Instance.guildModel == null)
		{
			return;
		}
		GuildMemberInfo memberInfo = GameManager.Instance.guildModel.GetMemberInfo(GetData().Id);
		if (memberInfo != null)
		{
			if (!SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.SocialGuildPlayerInfoPopup))
			{
				GuildPlayerInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.SocialGuildPlayerInfoPopup) as GuildPlayerInfoPopup;
				obj.GuildMemberId = memberInfo.MemberId;
				obj.Open();
			}
		}
		else
		{
			MyTools.CopyToClipboard(GetData().Name);
			GuildInfoPopup.OpenForGuildId(GetData().Id);
		}
	}

	private void SetBadge(ScoreDataEntry entry)
	{
		if (entry is GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry && playerEmblem != null)
		{
			playerEmblem.SetEmblem(guildBattlePlayersScoreDataEntry.PlayerEmblem);
		}
	}

	private void OnEnable()
	{
		if (button != null)
		{
			button.SetClickCallback(OnButtonClick);
		}
	}

	private void OnDisable()
	{
		if (button != null)
		{
			button.RemoveClickCallback(OnButtonClick);
		}
	}
}
