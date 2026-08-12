using System.Collections;
using TWDModel;
using UnityEngine;

public class OutpostPopup : HUDElement
{
	[SerializeField]
	private UIButtonToggle uiToggleLogTab;

	[SerializeField]
	private UIButtonToggle uiToggleCycleTab;

	public GameObject TradeGoodsAnimationPrefab;

	public GameObject RankingScoreAnimationPrefab;

	public UIPanel RewardAnimationParent;

	public override void Open()
	{
		UITypeOpenOnClose = UIType.MissionHubPopup;
		if ((bool)CampView.Instance && (bool)CampView.Instance.CampViewBuildings)
		{
			CampView.Instance.CampViewBuildings.UnselectBuilding();
		}
		OutpostPopup outpostPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OutpostPopup, null, createIfNotExist: false) as OutpostPopup;
		if (outpostPopup != null && !outpostPopup.IsOpen)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/open_outpostmanagement");
			HasBuildingAndCorrectLevelToEdit();
		}
		base.Open();
	}

	private void OnEnable()
	{
		if (uiToggleLogTab != null)
		{
			uiToggleLogTab.SetClickCallback(OnLogtabClick);
		}
	}

	private void OnDisable()
	{
		if (uiToggleLogTab != null)
		{
			uiToggleLogTab.RemoveClickCallback(OnLogtabClick);
		}
	}

	private void OnLogtabClick(UIButtonExtended button)
	{
		if (GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.NumNewDefenseLogEntries > 0)
		{
			Helpers.ExecuteCommand(new SetDefenseLogSeenCommand());
			UIEvent.Send("OnDefenseLogSeen");
		}
	}

	public static bool HasBuildingAndCorrectLevelToEdit()
	{
		BuildingModel building = GameManager.Instance.playerModel.Camp.GetBuilding("Outpost");
		if (building != null && building.Level >= GameManager.Instance.gameEconomyData.ConfigData.OutpostUnlockEditingAtBuilingLevel)
		{
			return true;
		}
		return false;
	}

	public override void Close()
	{
		base.Close();
	}

	public override void OnClickClose()
	{
		base.OnClickClose();
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_dialog_exit");
	}

	public void GoToLog()
	{
		StartCoroutine(GoToLogDelayed());
	}

	private IEnumerator GoToLogDelayed()
	{
		yield return null;
		uiToggleLogTab.ForceClick();
	}

	public void GoToCycles()
	{
		uiToggleCycleTab.ForceClick();
	}

	public IEnumerator Reward()
	{
		yield return new WaitForSeconds(1.5f);
		int num = (PlatformInfo.HasFlag(PlatformFlag.SlowGPU) ? 4 : 8);
		BattleLogListPanel componentInChildren = GetComponentInChildren<BattleLogListPanel>();
		OutpostPopupBattleLog componentInChildren2 = GetComponentInChildren<OutpostPopupBattleLog>();
		if (!(componentInChildren != null) || !(componentInChildren2 != null))
		{
			yield break;
		}
		BattleLogListCard battleLogListCard = componentInChildren.getCardAt(componentInChildren.GetCards().Count - 1) as BattleLogListCard;
		if (!(battleLogListCard != null) || battleLogListCard.Item.CombatResult != ECombatResult.Successful)
		{
			yield break;
		}
		battleLogListCard.SetSparkeEnabled(enabled: true);
		int rankingScoreChange = battleLogListCard.Item.RankingScoreChange;
		int resourcesStolen = battleLogListCard.Item.ResourcesStolen;
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		if (resourcesStolen > 0)
		{
			for (int i = 0; i < num; i++)
			{
				CollectAnimation component = Helpers.InstantiateToParent(TradeGoodsAnimationPrefab, RewardAnimationParent.gameObject).GetComponent<CollectAnimation>();
				if (component != null)
				{
					component.FollowTarget(battleLogListCard.TradeGoodsIcon.gameObject);
				}
				component.StartAnimation(resourcesStolen, CurrencyType.Outpost, campHUD.GetCollectAnimationDestination(CurrencyType.Outpost), null, isFirst: false);
			}
		}
		if (rankingScoreChange <= 0)
		{
			yield break;
		}
		for (int j = 0; j < num; j++)
		{
			CollectAnimation component2 = Helpers.InstantiateToParent(RankingScoreAnimationPrefab, RewardAnimationParent.gameObject).GetComponent<CollectAnimation>();
			if (component2 != null)
			{
				component2.FollowTarget(battleLogListCard.InfluenceIcon.gameObject);
			}
			component2.SetLabelVisible(visible: false);
			component2.StartAnimation(rankingScoreChange, componentInChildren2.InflucenIcon.gameObject.transform);
		}
	}
}
