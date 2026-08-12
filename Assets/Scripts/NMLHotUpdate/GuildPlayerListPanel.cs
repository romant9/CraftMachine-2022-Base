using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GuildPlayerListPanel : ScrollableListPanel<ScoreDataEntry>
{
	[SerializeField]
	[Tooltip("Tabs for contributions, rank.")]
	private UITabs infoTypesTabs;

	private void OnEnable()
	{
		if (infoTypesTabs != null)
		{
			infoTypesTabs.OnNewTabSelectedEvent += OnNewTabSelected;
		}
		Setup();
		UIEvent.OnUIEvent += OnUiEvent;
	}

	private void OnDisable()
	{
		if (infoTypesTabs != null)
		{
			infoTypesTabs.OnNewTabSelectedEvent -= OnNewTabSelected;
		}
		UIEvent.OnUIEvent -= OnUiEvent;
	}

	private void OnUiEvent(string type, object parameter)
	{
		if (type == "SocialGuildPlayerChanged")
		{
			Setup();
		}
	}

	private void Setup()
	{
		if (GameManager.Instance != null && GameManager.Instance.guildModel != null)
		{
			List<ScoreDataEntry> list = new List<ScoreDataEntry>();
			for (int i = 0; i < GameManager.Instance.guildModel.GuildMembers.Count; i++)
			{
				GuildMemberInfo guildMemberInfo = GameManager.Instance.guildModel.GuildMembers[i];
				PlayerScoreDataEntry playerScoreDataEntry = new PlayerScoreDataEntry(guildMemberInfo, guildMemberInfo.GetMinutesSinceLastActive(GameManager.Instance.playerModel.UtcTimeStamp));
				if (guildMemberInfo.MemberId == GameManager.Instance.playerModel.HashedId)
				{
					playerScoreDataEntry.MemberInfo.PlayerLevel = GameManager.Instance.playerModel.Level;
					playerScoreDataEntry.MemberInfo.LastActiveDate = GameManager.Instance.playerModel.UtcTimeStamp;
					playerScoreDataEntry.Score = guildMemberInfo.GetMinutesSinceLastActive(GameManager.Instance.playerModel.UtcTimeStamp);
				}
				list.Add(playerScoreDataEntry);
			}
			SetCards(list);
		}
		else
		{
			SetCards(null);
		}
	}

	private void OnNewTabSelected(int tabindex)
	{
		Setup();
	}
}
