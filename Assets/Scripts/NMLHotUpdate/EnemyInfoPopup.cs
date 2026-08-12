using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class EnemyInfoPopup : HUDElement
{
	[SerializeField]
	private GameObject infoContainer;

	[SerializeField]
	private GameObject itemPrefab;

	[SerializeField]
	private UIGrid enemyInfoGrid;

	private List<EnemyInfoItem> items;

	private List<string> shown;

	public void SetContent(MissionData missionData, SurvivalMissionConfig survivalMissionConfig, bool randomWalkers, bool isEndlessMode)
	{
		items = new List<EnemyInfoItem>();
		shown = new List<string>();
		if (survivalMissionConfig == null && missionData != null && missionData.HasEnemyTrait("Burning"))
		{
			CreateInfoItem("Ui_Icon_Class_Burning", HelpersLocalization.GetActorClassDescrption("Walker", "Burning"), HelpersLocalization.GetActorClassName("Walker", "Burning"));
		}
		Array values = Enum.GetValues(typeof(SurvivorClass));
		for (int i = 0; i < values.Length; i++)
		{
			if (i != 6)
			{
				SurvivorClass cls = (SurvivorClass)values.GetValue(i);
				if ((survivalMissionConfig != null && survivalMissionConfig.HasRaider(cls)) || (survivalMissionConfig == null && missionData != null && missionData.HasRaider(cls)))
				{
					CreateInfoItem("Ui_Icon_Class_Raider", HelpersLocalization.GetActorClassDescrption("Raider", cls.ToString()), HelpersLocalization.GetActorClassName("Raider", cls.ToString()));
					break;
				}
			}
		}
		Array values2 = Enum.GetValues(typeof(WalkerType));
		List<WalkerType> list = new List<WalkerType>();
		if (isEndlessMode)
		{
			list = ((!EndlessModeHelpers.IsEndlessExpertMode()) ? EndlessModeHelpers.GetEndlessBattleMissionWalkerTypes() : EndlessModeHelpers.GetEndlessExpertBattleMissionWalkerTypes());
		}
		for (int j = 0; j < values2.Length; j++)
		{
			WalkerType walkerType = (WalkerType)values2.GetValue(j);
			if (walkerType != WalkerType.WalkerNormal && ((survivalMissionConfig != null && survivalMissionConfig.HasWalker(walkerType)) || (survivalMissionConfig == null && missionData != null && missionData.HasWalker(walkerType)) || (isEndlessMode && list.Contains(walkerType))))
			{
				CreateInfoItem(HelpersGfx.GetWalkerIconName(walkerType), HelpersLocalization.GetActorClassDescrption("Walker", walkerType.ToString()), HelpersLocalization.GetActorClassName("Walker", walkerType.ToString()));
			}
		}
		if (randomWalkers)
		{
			CreateInfoItem(HelpersGfx.GetWalkerIconName(WalkerType.WalkerRandom), HelpersLocalization.GetActorClassDescrption("Walker", WalkerType.WalkerRandom.ToString()), HelpersLocalization.GetActorClassName("Walker", WalkerType.WalkerRandom.ToString()));
		}
	}

	private void CreateInfoItem(string spriteName, string description, string title)
	{
		EnemyInfoItem component = Helpers.InstantiateToParentAndLayer(itemPrefab, infoContainer).GetComponent<EnemyInfoItem>();
		if (component != null)
		{
			component.SetVisuals(spriteName, title, description);
			items.Add(component);
		}
		enemyInfoGrid.Reposition();
	}

	public override void Close()
	{
		ClearItems();
		base.Close();
	}

	private void ClearItems()
	{
		if (items != null)
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (items[i] != null && items[i].gameObject != null)
				{
					UnityEngine.Object.Destroy(items[i].gameObject);
				}
			}
			items.Clear();
			items = null;
		}
		if (shown != null)
		{
			shown.Clear();
			shown = null;
		}
	}
}
