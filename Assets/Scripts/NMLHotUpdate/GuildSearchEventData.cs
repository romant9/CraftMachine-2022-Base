using System.Collections.Generic;
using TWDModel;

public class GuildSearchEventData
{
	public GuildSearchInfo Info { get; set; }

	public GuildSearchEventData(GuildSearchInfo.SearchType searchType, string searchKeyword, PlayerModel player)
	{
		int playerLevel = 0;
		string playerCountryCode = "";
		if (player != null)
		{
			playerLevel = player.Level;
			playerCountryCode = player.Country;
		}
		Info = new GuildSearchInfo();
		Info.Initialize(searchType, searchKeyword, playerLevel, playerCountryCode);
	}

	public void FillSelected(List<GuildSearchResult> selected)
	{
		if (selected == null)
		{
			return;
		}
		Info.SelectedCount = selected.Count;
		Info.SelectedGuildSizes.Clear();
		Info.SelectedGuildQueryIds.Clear();
		Info.SelectedGuildCountryCodes.Clear();
		Info.SelectedGuildAvgPlayerLevels.Clear();
		foreach (GuildSearchResult item in selected)
		{
			if (item.model != null)
			{
				Info.SelectedGuildSizes.Add(item.model.NumberMembers);
				Info.SelectedGuildQueryIds.Add((int)item.source);
				Info.SelectedGuildCountryCodes.Add(item.model.CountryCode);
				Info.SelectedGuildAvgPlayerLevels.Add((int)item.model.AverageMemberLevel);
			}
		}
	}

	public void SetGuildCounts(GuildSearchResult.Source source, int queriedCount, int selectedCount)
	{
		switch (source)
		{
		case GuildSearchResult.Source.Ad:
		{
			GuildSearchInfo.CountStruct guildCountsQueried = Info.GuildCountsQueried;
			guildCountsQueried.Ad = queriedCount;
			Info.GuildCountsQueried = guildCountsQueried;
			guildCountsQueried = Info.GuildCountsSelected;
			guildCountsQueried.Ad = selectedCount;
			Info.GuildCountsSelected = guildCountsQueried;
			break;
		}
		case GuildSearchResult.Source.New:
		{
			GuildSearchInfo.CountStruct guildCountsQueried = Info.GuildCountsQueried;
			guildCountsQueried.New = queriedCount;
			Info.GuildCountsQueried = guildCountsQueried;
			guildCountsQueried = Info.GuildCountsSelected;
			guildCountsQueried.New = selectedCount;
			Info.GuildCountsSelected = guildCountsQueried;
			break;
		}
		case GuildSearchResult.Source.SameCountry:
		{
			GuildSearchInfo.CountStruct guildCountsQueried = Info.GuildCountsQueried;
			guildCountsQueried.SameCountry = queriedCount;
			Info.GuildCountsQueried = guildCountsQueried;
			guildCountsQueried = Info.GuildCountsSelected;
			guildCountsQueried.SameCountry = selectedCount;
			Info.GuildCountsSelected = guildCountsQueried;
			break;
		}
		case GuildSearchResult.Source.NearLevel:
		{
			GuildSearchInfo.CountStruct guildCountsQueried = Info.GuildCountsQueried;
			guildCountsQueried.NearLevel = queriedCount;
			Info.GuildCountsQueried = guildCountsQueried;
			guildCountsQueried = Info.GuildCountsSelected;
			guildCountsQueried.NearLevel = selectedCount;
			Info.GuildCountsSelected = guildCountsQueried;
			break;
		}
		case GuildSearchResult.Source.Fallback:
		{
			GuildSearchInfo.CountStruct guildCountsQueried = Info.GuildCountsQueried;
			guildCountsQueried.Fallback = queriedCount;
			Info.GuildCountsQueried = guildCountsQueried;
			guildCountsQueried = Info.GuildCountsSelected;
			guildCountsQueried.Fallback = selectedCount;
			Info.GuildCountsSelected = guildCountsQueried;
			break;
		}
		case GuildSearchResult.Source.Keyword:
		{
			GuildSearchInfo.CountStruct guildCountsQueried = Info.GuildCountsQueried;
			guildCountsQueried.Keyword = queriedCount;
			Info.GuildCountsQueried = guildCountsQueried;
			guildCountsQueried = Info.GuildCountsSelected;
			guildCountsQueried.Keyword = selectedCount;
			Info.GuildCountsSelected = guildCountsQueried;
			break;
		}
		}
	}

	public void SetSearchDuration(long startTS)
	{
		if (startTS > 0)
		{
			long num = GameManager.Instance.playerModel.UtcTimeStamp - startTS;
			Info.SearchDuration = (int)num;
		}
	}

	public SendSearchGuildMetricCommand ToSendMetricCommand()
	{
		return new SendSearchGuildMetricCommand
		{
			Info = Info
		};
	}
}
