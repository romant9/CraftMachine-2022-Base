using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class GuildProtDetailsListCard : GuildPlayerListCardBase
{
    [Header("GuildProtDetailsListCard")]
    [SerializeField]
    private UILabel rankLabel;

    public UILabel TeamSign;
    public UILabel BaseIndex;
    public UILabel AdjustedLevel;
    public UILabel LocCount;
    public UILabel DateJoin;
    public UILabel Locations;
    public Color emptyDataColor;

    private GuildModel guildModel => GWTeamUtils.Instance.IsOpponentGuild ? GWTeamUtils.Instance.OpponentGuilModel : GWTeamUtils.Instance.GuildModel;


    private void OnEnable()
    {
    }

    public override void UpdateUI()
    {
        base.UpdateUI();

        if (Type == GuildPlayerListCardType.TeamLocations)
        {
            var item = (SectorsDataEntry)Item;

            if (item.SectorNames != null && item.SectorNames.Count > 0)
                Locations.text = string.Join(", ", item.SectorNames);
            else 
            {
                Locations.text = string.Empty;
                backgroundSprite.color = emptyDataColor;
            } 

            TeamSign.text = item.TeamIndex;
            BaseIndex.text = (item.BasePlayerIndex + 1).ToString() + " / " + guildModel.GuildBattleMatchmakingInfo.PlayerInfoSnapshot.Count;
            AdjustedLevel.text = item.AdjustedLevel.ToString();
            LocCount.text = item.SectorNames.Count + " / " + item.TeamInfo.SectorNamesAll.Count;
            DateJoin.text = item.GuildJoinedDate;
            rankLabel.text = (item.TeamSortedPlayerIndex + 1).ToString();

        }
    }

    public void SetRank(int rank)
    {
        rankLabel.text = rank.ToString();
        UpdateUI();
    }

    public override int GetSortValue()
    {
        if (base.Type == GuildPlayerListCardType.TeamLocations)
        {
            //Debug.Log("internal sort TeamLocations");
            var sortedIndex = ((SectorsDataEntry)base.Item).TeamSortedPlayerIndex;
            return (int)Math.Min(sortedIndex, 2147483647L);
            //return (int)Math.Max(sortedIndex, 2147483647L);
        }
        return base.GetSortValue();
    }

}
