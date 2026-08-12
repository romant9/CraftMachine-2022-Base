using System;
using System.Collections.Generic;
using BaseModel;
using Client.Connectivity;
using TWDModel;

public class GuildBattleGuildLeaderboardDataProvider : ScoreDataProvider
{
	protected string leaderboardName;

	protected int cacheDurationSeconds = 300;

	protected string max = "40";

	protected GuildModel guildModel;

	private Dictionary<string, string> playerToEntryIdsDictionary;

	private bool cached;

	public GuildBattleGuildLeaderboardDataProvider(string leaderboardName, GuildModel guildModel, Action<ScoreDataProvider, List<ScoreDataEntry>> onDataReceived, string max = "40", int cacheDurationSeconds = 300, bool cached = false)
	{
		this.leaderboardName = leaderboardName;
		base.OnDataReceived -= onDataReceived;
		base.OnDataReceived += onDataReceived;
		this.guildModel = guildModel;
		this.max = max;
		this.cacheDurationSeconds = cacheDurationSeconds;
		this.cached = cached;
	}

	public void Clear()
	{
		if (!cached)
		{
			guildModel = null;
			leaderboardName = "";
			playerToEntryIdsDictionary = null;
		}
	}

	protected override bool RequestInternal()
	{
		if (GameManager.Instance.IsConnectedToServer)
		{
			if (IsGuildBoard())
			{
				OnLeaderboardDataFromGuildModel();
				return true;
			}
			SignalRClient.Instance.RequestCommand("GetLeaderboard", leaderboardName, max, OnLeaderboardData, null, waitForResponse: true);
			return true;
		}
		return false;
	}

	public override int GetCacheDurationSeconds()
	{
		return cacheDurationSeconds;
	}

	protected void OnLeaderboardDataFromGuildModel()
	{
		if (!IsGuildBoard())
		{
			return;
		}
		int warDefinitionId = guildModel.GvGSeasonModel.GuildWarModel.WarDefinitionId;
		int seasonDefinitionId = guildModel.GvGSeasonModel.SeasonDefinitionId;
		string id = guildModel.Id;
		List<ScoreDataEntry> list = new List<ScoreDataEntry>();
		for (int i = 0; i < guildModel.GuildMembers.Count; i++)
		{
			GuildMemberInfo guildMemberInfo = guildModel.GuildMembers[i];
			if (guildMemberInfo != null)
			{
				GuildBattlePlayersScoreDataEntry guildBattlePlayersScoreDataEntry = new GuildBattlePlayersScoreDataEntry(guildMemberInfo.Name, guildMemberInfo.MemberId, guildModel.Id, guildMemberInfo.PlayerEmblem, 0);
				if (leaderboardName == Leaderboards.GetLeaderboardNameGuildMembersSeason(seasonDefinitionId, id))
				{
					guildBattlePlayersScoreDataEntry.Score = guildModel.GvGSeasonModel.GetSeasonVpTotalForPlayer(guildMemberInfo.MemberId);
				}
				else if (leaderboardName == Leaderboards.GetLeaderboardNameGuildMembersWar(warDefinitionId, id))
				{
					guildBattlePlayersScoreDataEntry.Score = guildModel.GvGSeasonModel.GuildWarModel.GetWarVpTotalForPlayer(guildMemberInfo.MemberId);
					guildBattlePlayersScoreDataEntry.PointsWithHeld = !GuildWarHelper.CanPlayerJoinWar(guildMemberInfo.MemberId);
				}
				else if (leaderboardName == Leaderboards.GetLeaderboardNameGuildMembersAlltime(id))
				{
					guildBattlePlayersScoreDataEntry.Score = guildModel.GetAllTimeVpTotalForPlayer(guildMemberInfo.MemberId);
				}
				list.Add(guildBattlePlayersScoreDataEntry);
			}
		}
		list.StableSort((ScoreDataEntry a, ScoreDataEntry b) => b.Score.CompareTo(a.Score));
		NotifyDataReceived(list);
	}

	protected void OnLeaderboardData(string response)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(response))
		{
			Debug.LogError("GetLeaderboard failed");
			SignalRClient.Instance.ClearError();
			NotifyDataReceived(null);
			return;
		}
		List<ScoreDataEntry> list = new List<ScoreDataEntry>();
		IEnumerable<LeaderboardEntry> enumerable = GameManager.Instance.jsonSerializer.DeserializeObject<IEnumerable<LeaderboardEntry>>(response);
		if (playerToEntryIdsDictionary == null)
		{
			playerToEntryIdsDictionary = new Dictionary<string, string>();
		}
		else
		{
			playerToEntryIdsDictionary.Clear();
		}
		foreach (LeaderboardEntry item in enumerable)
		{
			if (IsGuildBoard())
			{
				Leaderboards.GuildBattlePlayersScoreLeaderboardDetails guildBattlePlayersScoreLeaderboardDetails = GameManager.Instance.jsonSerializer.Deserialize<Leaderboards.GuildBattlePlayersScoreLeaderboardDetails>(item.Details);
				PlayerEmblem playerEmblem = GameManager.Instance.jsonSerializer.Deserialize<PlayerEmblem>(guildBattlePlayersScoreLeaderboardDetails.PlayerEmblem);
				list.Add(new GuildBattlePlayersScoreDataEntry(guildBattlePlayersScoreLeaderboardDetails.PlayerName, item.Id, guildBattlePlayersScoreLeaderboardDetails.GroupId, playerEmblem, (int)item.Score));
				playerToEntryIdsDictionary.Add(guildBattlePlayersScoreLeaderboardDetails.PlayerHashedId, item.Id);
			}
			else
			{
				Leaderboards.GuildBattleLiveScoreLeaderboardDetails guildBattleLiveScoreLeaderboardDetails = GameManager.Instance.jsonSerializer.Deserialize<Leaderboards.GuildBattleLiveScoreLeaderboardDetails>(item.Details);
				list.Add(new GuildBattleLiveScoreDataEntry(guildBattleLiveScoreLeaderboardDetails.GroupId, guildBattleLiveScoreLeaderboardDetails.GroupName, (int)item.Score));
			}
		}
		if (IsGuildBoard() && playerToEntryIdsDictionary != null && playerToEntryIdsDictionary.Count < guildModel.NumberMembers)
		{
			for (int i = 0; i < guildModel.GuildMembers.Count; i++)
			{
				GuildMemberInfo guildMemberInfo = guildModel.GuildMembers[i];
				if (guildMemberInfo != null && !playerToEntryIdsDictionary.ContainsKey(guildMemberInfo.MemberId))
				{
					list.Add(new GuildBattlePlayersScoreDataEntry(guildMemberInfo.Name, guildMemberInfo.MemberId, guildMemberInfo.GuildId, guildMemberInfo.PlayerEmblem, 0));
				}
			}
		}
		NotifyDataReceived(list);
	}

	public string GetLeaderboardName()
	{
		return leaderboardName;
	}

	public bool IsGuildBoard()
	{
		return guildModel != null;
	}
}
