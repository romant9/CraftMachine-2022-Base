using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class GuildBattleTierList : HUDElement
{
	public NUIScrollableList list;

	private const string ListItemSrc = "GvG_Tier_List_Item";

	private List<GuildTierDefinition> internalList;

	[SerializeField]
	private UIOnClickRequest guildShopButton;

	private void OnEnable()
	{
		if (list == null || GameManager.Instance == null)
		{
			return;
		}
		if (internalList == null)
		{
			GuildTierDefinition[] guildTierDefinitions = GameManager.Instance.gameEconomyData.GuildTierDefinitions;
			internalList = guildTierDefinitions.ToList();
		}
		list.UpdateWithList(internalList, "GvG_Tier_List_Item", "GvG_Tier_List_Item", callUpdateUI: true);
		list.SortAndReset();
		if (guildShopButton != null)
		{
			PlayerModel playerModel = GameManager.Instance.playerModel;
			if (!playerModel.IsGuildMember && !playerModel.GuildShopModel.InitializedThisSeason)
			{
				guildShopButton.OnClickClose = UIType.None;
			}
		}
	}

	private void OnDisable()
	{
		internalList = null;
	}
}
