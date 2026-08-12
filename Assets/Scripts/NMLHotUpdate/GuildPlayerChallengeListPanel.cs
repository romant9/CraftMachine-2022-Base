using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class GuildPlayerChallengeListPanel : ScrollableListPanel<ScoreDataEntry>
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
				PlayerScoreDataEntry playerScoreDataEntry = new PlayerScoreDataEntry(guildMemberInfo, guildMemberInfo.CurrentChallengeStars);
				if (guildMemberInfo.MemberId == GameManager.Instance.playerModel.HashedId)
				{
					playerScoreDataEntry.MemberInfo.PlayerLevel = GameManager.Instance.playerModel.Level;
					playerScoreDataEntry.Score = GameManager.Instance.playerModel.WeeklyChallenge.NumberStarsInCurrentGuild;
				}
				list.Add(playerScoreDataEntry);
			}
			SetCards(list);

			GuildWarModel model = OfflineManager.Instance.CurrentGuildModel?.GuildWarModel;
			for (int j = 0; j < cards.Count; j++)
			{
				((GuildPlayerListCard)cards[j]).SetRank(cards.Count - j);

				if (OfflineManager.IsLoadDataManager && model != null)
				{
					var id = ((GuildPlayerListCard)cards[j]).Item.Id;
					List<int> gwscores = new List<int>();
					string days = "";
					var regDays = model.RegisteredPlayersForBattleSlot.Values.ToList();
					for (int i = 0; i < regDays.Count; i++)
					{
						var regPlayers = regDays[i];
						if (regPlayers != null && regPlayers.Contains(id))
						{
							days += (i + 1).ToString();
						}
					}

					var gwResults = model.GuildBattleResults.Values.ToList();
					for (int i = 0; i < gwResults.Count; i++)
					{
						var gwResult = gwResults[i];
						if (gwResult != null && gwResult.RegisteredPlayers.Contains(id))
						{
							var scoreData = gwResult.PlayerScores.FirstOrDefault(x => x.Id == id);
							if (scoreData != null)
							{
								gwscores.Add((int)scoreData.Score);
							}
						}
					}

					((GuildPlayerListCard)cards[j]).SetGWData(gwscores, days);
				}
			}
		}
		else
		{
			SetCards(null);
		}
	}

	protected override void SetCard(UIListCard<ScoreDataEntry> card)
	{
		base.SetCard(card);
	}

	private void OnNewTabSelected(int tabindex)
	{
		Setup();
	}
}
