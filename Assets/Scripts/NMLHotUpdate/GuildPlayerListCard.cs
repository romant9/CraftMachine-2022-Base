using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GuildPlayerListCard : GuildPlayerListCardBase
{
	[Header("GuildPlayerListCard")]
	[SerializeField]
	private UILabel rankLabel;

	[Header("Score")]
	[SerializeField]
	protected GameObject scoreContainer;

	[SerializeField]
	private GameObject noStarsLabel;

	[SerializeField]
	protected UILabel scoreLabel;

	[SerializeField]
	private UISprite defaultPortrait;

	private GameObject defaultPortraitGameObject;

	[SerializeField]
	private UITexture socialPortrait;

	[SerializeField]
	protected PlayerEmblemIcon playerEmblemIcon;

	[SerializeField]
	private GameObject[] endlessExpertModeTags;

	private GameObject socialPortraitGameObject;

	[SerializeField]
	private GameObject challengeDifficultyContainer;

	[SerializeField]
	private UILabel challengeDifficultyLabel;

	public List<string> Socials { get; set; }

	private void OnEnable()
	{
		if (socialPortrait != null)
		{
			socialPortraitGameObject = socialPortrait.gameObject;
		}
		if (defaultPortrait != null)
		{
			defaultPortraitGameObject = defaultPortrait.gameObject;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		HelpersUI.SetContentToLabel(scoreLabel, base.Item.Score.ToString());
		if (challengeDifficultyContainer != null)
		{
			challengeDifficultyContainer.gameObject.SetActive(value: false);
		}
		if (!(base.Item is PlayerScoreDataEntry playerScoreDataEntry))
		{
			levelLabel.text = "";
			memberTypeLabel.text = "";
			ClearProfilePicture();
			return;
		}
		if (playerEmblemIcon != null)
		{
			playerEmblemIcon.SetEmblem(playerScoreDataEntry.MemberInfo.PlayerEmblem);
		}
		if ((bool)levelLabel)
		{
			levelLabel.text = playerScoreDataEntry.MemberInfo.PlayerLevel.ToString();
		}
		ClearProfilePicture();
		if (playerScoreDataEntry.MemberInfo.ExcludedFromChallenge)
		{
			scoreContainer.SetActive(value: false);
			if (noStarsLabel != null)
			{
				noStarsLabel.SetActive(value: true);
			}
		}
		else
		{
			scoreContainer.SetActive(value: true);
			if (noStarsLabel != null)
			{
				noStarsLabel.SetActive(value: false);
			}
		}
		if (base.Type == GuildPlayerListCardType.PlayerList || base.Type == GuildPlayerListCardType.GuildPlayerList || base.Type == GuildPlayerListCardType.PlayerListEndless)
		{
			if (defaultPortraitGameObject != null)
			{
				defaultPortraitGameObject.SetActive(value: true);
			}
			if (socialPortraitGameObject != null)
			{
				socialPortraitGameObject.SetActive(value: false);
			}
			if (endlessExpertModeTags != null)
			{
				EndlessModePlayersScoreDataEntry endlessModePlayersScoreDataEntry = base.Item as EndlessModePlayersScoreDataEntry;
				for (int i = 0; i < endlessExpertModeTags.Length; i++)
				{
					endlessExpertModeTags[i].SetActive(i < endlessModePlayersScoreDataEntry?.EndlessModeExpertTagCount);
				}
			}
		}
		if ((bool)memberTypeLabel)
		{
			if (base.Type == GuildPlayerListCardType.MemberList)
			{
				memberTypeLabel.text = HelpersLocalization.GetGuildMemberRole(playerScoreDataEntry.MemberInfo);
			}
			else if (base.Type == GuildPlayerListCardType.FriendList)
			{
				memberTypeLabel.text = playerScoreDataEntry.MemberInfo.GuildLeaderboardName;
			}
			else if (base.Type == GuildPlayerListCardType.GuildList)
			{
				memberTypeLabel.text = "";
			}
		}
		if (!(GuildNameLabel != null))
		{
			return;
		}
		if (playerScoreDataEntry.MemberInfo.State == GuildMemberState.Normal || !string.IsNullOrEmpty(playerScoreDataEntry.MemberInfo.GuildLeaderboardName))
		{
			if (playerScoreDataEntry.MemberInfo.State == GuildMemberState.Normal)
			{
				GuildNameLabel.text = GameManager.Instance.GetFilteredText(GameManager.Instance.guildModel.Name);
			}
			else
			{
				GuildNameLabel.text = GameManager.Instance.GetFilteredText(playerScoreDataEntry.MemberInfo.GuildLeaderboardName);
			}
		}
		else
		{
			GuildNameLabel.text = "-";
		}
	}

	private void ClearProfilePicture()
	{
		if (defaultPortraitGameObject != null)
		{
			defaultPortraitGameObject.SetActive(value: true);
		}
		if (socialPortraitGameObject != null)
		{
			socialPortraitGameObject.SetActive(value: false);
		}
	}

	public void SetRank(int rank)
	{
		if ((bool)rankLabel)
		{
			rankLabel.text = rank + ".";
		}
	}

	public void SetRank(string text)
	{
		if ((bool)rankLabel)
		{
			rankLabel.text = text;
		}
	}

	public override int GetSortValue()
	{
		if (base.Type == GuildPlayerListCardType.PlayerList || base.Type == GuildPlayerListCardType.FriendList || base.Type == GuildPlayerListCardType.MemberList || base.Type == GuildPlayerListCardType.GuildPlayerList || base.Type == GuildPlayerListCardType.PlayerListEndless)
		{
			if (base.Item is PlayerScoreDataEntry { MemberInfo: not null } playerScoreDataEntry && playerScoreDataEntry.MemberInfo.ExcludedFromChallenge)
			{
				return 1000 - playerScoreDataEntry.MemberInfo.PlayerLevel;
			}
			return -(int)Math.Min(base.Item.Score, 2147483647L);
		}
		if (base.Type == GuildPlayerListCardType.GuildList)
		{
			return -(int)Math.Min(base.Item.Score, 2147483647L);
		}
		return base.GetSortValue();
	}

	public override long GetSortLongValue()
	{
		if (base.Type == GuildPlayerListCardType.PlayerList || base.Type == GuildPlayerListCardType.FriendList || base.Type == GuildPlayerListCardType.MemberList || base.Type == GuildPlayerListCardType.GuildPlayerList || base.Type == GuildPlayerListCardType.PlayerListEndless)
		{
			if (base.Item is PlayerScoreDataEntry { MemberInfo: not null } playerScoreDataEntry && playerScoreDataEntry.MemberInfo.ExcludedFromChallenge)
			{
				return 1000 - playerScoreDataEntry.MemberInfo.PlayerLevel;
			}
			return -Math.Min(base.Item.Score, long.MaxValue);
		}
		if (base.Type == GuildPlayerListCardType.GuildList)
		{
			return -Math.Min(base.Item.Score, long.MaxValue);
		}
		return base.GetSortValue();
	}



	#region myparams
	[Header("Score GW")]
	[SerializeField]
	private GameObject scoreGWContainer;
	[SerializeField]
	private GameObject noStarsGWLabel;
	[SerializeField]
	private UILabel scoreGWLabel1;
	[SerializeField]
	private UILabel scoreGWLabel2;
	[SerializeField]
	private UILabel daysGWLabel;
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion

	#region mycode
	public void SetGWData(List<int> gwScores, string gwDays)
	{
		if (scoreGWContainer && noStarsGWLabel)
		{
			if (gwScores.Count > 0)
			{
				scoreGWContainer.SetActive(true);
				noStarsGWLabel.SetActive(false);
				scoreGWLabel1.text = gwScores[0].ToString();
				scoreGWLabel2.text = gwScores.Count > 1 ? gwScores[1].ToString() : "0";
				daysGWLabel.text = gwDays;
			}
			else
			{
				scoreGWContainer.SetActive(false);
				noStarsGWLabel.SetActive(true);
			}
		}
	}
	#endregion
}
