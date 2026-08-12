using BaseModel;
using TWDModel;
using UnityEngine;

public class GuildPlayerGuildWarListCard : GuildPlayerListCardBase
{
	[Header("GuildPlayerGuildWarListCard")]
	[SerializeField]
	private UISprite activityStatus;

	[SerializeField]
	private PlayerEmblemIcon playerEmblemIcon;

	[SerializeField]
	private ShowTooltip noStarsIcon;

	[SerializeField]
	private Color onlineColor;

	[SerializeField]
	private Color offlineColor;

	private void OnEnable()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null)
		{
			GuildModel guildModel = playerModel.GuildModel;
			if (guildModel != null)
			{
				guildModel.Changed -= OnGuildChanged;
				guildModel.Changed += OnGuildChanged;
			}
		}
	}

	private void OnDisable()
	{
		PlayerModel playerModel = GameManager.Instance.playerModel;
		if (playerModel != null)
		{
			GuildModel guildModel = playerModel.GuildModel;
			if (guildModel != null)
			{
				guildModel.Changed -= OnGuildChanged;
			}
		}
	}

	private void OnGuildChanged(GroupModelBase model, string changed, object memberId)
	{
		if (changed == "MemberActivityStatusChanged")
		{
			string text = memberId as string;
			if (!string.IsNullOrEmpty(text) && base.Item is PlayerScoreDataEntry playerScoreDataEntry && playerScoreDataEntry.MemberInfo.MemberId == text)
			{
				UpdateUI();
			}
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (!(base.Item is PlayerScoreDataEntry playerScoreDataEntry))
		{
			levelLabel.text = "";
			memberTypeLabel.text = "";
			return;
		}
		if (playerEmblemIcon != null)
		{
			playerEmblemIcon.SetEmblem(playerScoreDataEntry.MemberInfo.PlayerEmblem);
		}
		memberTypeLabel.text = HelpersLocalization.GetGuildMemberRole(playerScoreDataEntry.MemberInfo);
		bool flag = playerScoreDataEntry.MemberInfo.IsOnline(GameManager.Instance.playerModel.UtcTimeStamp);
		activityStatus.color = (flag ? onlineColor : offlineColor);
		Helpers.GameObjectSetActive(noStarsIcon, playerScoreDataEntry.MemberInfo.ExcludedFromChallenge);
	}

	public override int GetSortValue()
	{
		if (base.Item is PlayerScoreDataEntry playerScoreDataEntry)
		{
			int num = 0;
			if (playerScoreDataEntry.MemberInfo.IsOnline(GameManager.Instance.playerModel.UtcTimeStamp))
			{
				num = -1000;
				if (playerScoreDataEntry.MemberInfo.Role == GuildMemberRole.Leader)
				{
					num -= 400;
				}
				else if (playerScoreDataEntry.MemberInfo.Role == GuildMemberRole.CoLeader)
				{
					num -= 300;
				}
				else if (playerScoreDataEntry.MemberInfo.Role == GuildMemberRole.Elder)
				{
					num -= 200;
				}
				else if (playerScoreDataEntry.MemberInfo.Role == GuildMemberRole.Normal)
				{
					num -= 100;
				}
				return num;
			}
			int minutesSinceLastActive = playerScoreDataEntry.MemberInfo.GetMinutesSinceLastActive(GameManager.Instance.playerModel.UtcTimeStamp);
			if (minutesSinceLastActive < 0)
			{
				return 1000;
			}
			return minutesSinceLastActive;
		}
		return base.GetSortValue();
	}
}
