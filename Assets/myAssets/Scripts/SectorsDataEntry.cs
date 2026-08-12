using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;

public class SectorsDataEntry : ScoreDataEntry
{
    public GuildMemberInfo MemberInfo;
    public GuildBattleTeamInfo TeamInfo;

    public List<List<string>> SectorNamesAll { get; set; }
    public List<string> SectorNames { get; set; }
    public List<int> AdjustedLevels { get; set; }

    public int SectorLevel { get; set; }

    public int AdjustedLevel { get; set; }

    public string TeamIndex { get; set; }

    //индекс в изначальном PlayerInfoSnapshot
    public int BasePlayerIndex { get; set; }
    //индекс в AllTeamsSorted
    public int TeamSortedPlayerIndex { get; set; }
    public string GuildJoinedDate { get; set; }

    public SectorsDataEntry(GuildMemberInfo info)
    {
        Id = info.MemberId;
        Name = info.Name;
        MemberInfo = info;
    }

    public SectorsDataEntry(GuildBattleTeamInfo team)
    {
        TeamInfo = team;

        Id = team.HashID;
        Name = team.Name;
        TeamIndex = team.TeamIndex;
        AdjustedLevel = team.AdjustedLevel;
        BasePlayerIndex = team.BasePlayerIndex;
        TeamSortedPlayerIndex = team.TeamSortedPlayerIndex;
        GuildJoinedDate = team.GuildJoinedDate;
        SectorNames = team.SectorNames;
    }

    public SectorsDataEntry(string id, string name)
    {
        Id = id;
        Name = name;
    }
}

