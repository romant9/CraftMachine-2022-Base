using System;
using System.Collections.Generic;
using UnityEngine;

public class GuildProtSectorsListCard : GuildPlayerListCardBase
{
	[Header("GuildProtSectorsListCard")]
	public UILabel rankLabel;

	public List<UILabel> sectorLabels;
	public List<UILabel> LevelLabels;

	public UILabel baseIndexAdd;

	public UIInput baseIndexAddInput;

	public EventDelegate typeDropDownCallback;


	public void Start()
	{
		typeDropDownCallback = new EventDelegate(OnBaseIndexChange);
		baseIndexAddInput.onChange.Add(typeDropDownCallback);
	}

	private void OnEnable()
	{
	}

	public void OnBaseIndexChange()
	{
		GWTeamsManager.Instance.GWProtList.SortByBaseIndex(baseIndexAddInput);
	}

	public override void UpdateUI()
	{
		base.UpdateUI();

		if (base.Type == GuildPlayerListCardType.PlayerLocations)
		{
			var item = (SectorsDataEntry)Item;
			List<List<string>> SectorNames = item.SectorNamesAll;
			var levels = item.AdjustedLevels;
			if (levels != null && levels.Count == 3)
			{
				LevelLabels[0].text = levels[0].ToString();
				LevelLabels[1].text = levels[1].ToString();
				LevelLabels[2].text = levels[2].ToString();
			}

			if (SectorNames != null)
			{
				if (SectorNames[0] != null )
					sectorLabels[0].text = string.Join(", ", SectorNames[0]);
				else sectorLabels[0].text = string.Empty;
				if (SectorNames[1] != null)
					sectorLabels[1].text = string.Join(", ", SectorNames[1]);
				else sectorLabels[1].text = string.Empty;
				if (SectorNames[2] != null)
					sectorLabels[2].text = string.Join(", ", SectorNames[2]);
				else sectorLabels[2].text = string.Empty;
			}

			var baseIndex = (item.BasePlayerIndex + 1).ToString();
			rankLabel.text = baseIndex;
			baseIndexAdd.text = baseIndex;
			baseIndexAddInput.value = baseIndex;
			//Debug.Log("Update label " + baseIndexAdd.text);
			//todo
		}
	}

	public void SetRank(int rank)
	{
		rankLabel.text = rank.ToString();// ((SectorsDataEntry)base.Item).SectorLevel.ToString();
		UpdateUI();
	}

	public override int GetSortValue()
	{
		if (base.Type == GuildPlayerListCardType.PlayerList || base.Type == GuildPlayerListCardType.MemberList || base.Type == GuildPlayerListCardType.GuildPlayerList || base.Type == GuildPlayerListCardType.PlayerListEndless)
		{
			if (base.Item is PlayerScoreDataEntry playerScoreDataEntry && playerScoreDataEntry.MemberInfo != null && playerScoreDataEntry.MemberInfo.ExcludedFromChallenge)
			{
				return 1000 - playerScoreDataEntry.MemberInfo.PlayerLevel;
			}
			return -(int)Math.Min(base.Item.Score, 2147483647L);
		}
		if (base.Type == GuildPlayerListCardType.GuildList)
		{
			return -(int)Math.Min(base.Item.Score, 2147483647L);
		}
		if (base.Type == GuildPlayerListCardType.PlayerLocations)
		{
			var baseIndex = ((SectorsDataEntry)base.Item).BasePlayerIndex;
			//Debug.Log("internal sort PlayerLocations " + baseIndex);

			//return (int)Math.Max(baseIndex, 2147483647L);
			return (int)Math.Min(baseIndex, 2147483647L);
		}
		return base.GetSortValue();
	}
}
