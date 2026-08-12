using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SurvivalRewardBoosterPopup : HUDElement
{
	[SerializeField]
	private GameObject boosterPurchasedState;

	[SerializeField]
	private GameObject boosterNotPurchasedState;

	[SerializeField]
	private UILabel boosterCostLabel;

	[SerializeField]
	private UIGrid rewardsGrid;

	[SerializeField]
	private GameObject rewardPrefab;

	private List<DistanceBoosterRewardItem> boosterRewards = new List<DistanceBoosterRewardItem>();

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public override void Open()
	{
		base.Open();
		Init();
		UpdateUI();
	}

	private void Init()
	{
		boosterCostLabel.text = GameManager.Instance.gameEconomyData.ConfigData.SurvivalDoubleRewardsCost.ToString();
	}

	public override void UpdateUI()
	{
		WeeklySurvivalModel weeklySurvival = GameManager.Instance.playerModel.WeeklySurvival;
		boosterPurchasedState.SetActive(weeklySurvival.DoubleRewardsEnabled);
		boosterNotPurchasedState.SetActive(!weeklySurvival.DoubleRewardsEnabled);
		if (boosterRewards.Count == 0)
		{
			List<WeeklySurvivalReward> personalRewardsBetween = weeklySurvival.GetPersonalRewardsBetween(0, -1);
			int currentDifficulty = (int)weeklySurvival.CurrentDifficulty;
			int num = 0;
			foreach (WeeklySurvivalReward item in personalRewardsBetween)
			{
				for (int i = 0; i < item.RewardEntries[currentDifficulty].RewardsList.Count; i++)
				{
					DistanceBoosterRewardItem component = Helpers.InstantiateToParent(rewardPrefab, rewardsGrid.gameObject).GetComponent<DistanceBoosterRewardItem>();
					component.Setup(item.RewardEntries[currentDifficulty].RewardsList[i], num);
					boosterRewards.Add(component);
				}
				num++;
			}
			rewardsGrid.Reposition();
			BoxCollider component2 = rewardsGrid.GetComponent<BoxCollider>();
			component2.size = new Vector3((float)rewardsGrid.maxPerLine * rewardsGrid.cellWidth, rewardsGrid.cellHeight * Mathf.Ceil((float)boosterRewards.Count / (float)rewardsGrid.maxPerLine), 1f);
			component2.center = new Vector3(0f, (0f - component2.size.y) / 2f, 0f);
		}
		for (int j = 0; j < boosterRewards.Count; j++)
		{
			boosterRewards[j].UpdateRewardState();
		}
	}

	public void OnClickActivateBooster()
	{
		EnableDoubleSurvivalRewardsCommand enableDoubleSurvivalRewardsCommand = new EnableDoubleSurvivalRewardsCommand();
		WeeklySurvivalModel weeklySurvivalModel = WeeklySurvivalHelper.GetWeeklySurvivalModel();
		if (weeklySurvivalModel != null)
		{
			enableDoubleSurvivalRewardsCommand.Cashier = weeklySurvivalModel.GetDoubleRewardsCashier();
			ConsumeCurrencyCommandUtils.Execute(enableDoubleSurvivalRewardsCommand, VisualizeBoosterUnlockRewards);
		}
	}

	private void VisualizeBoosterUnlockRewards(TWDModelResult result)
	{
		if (result != TWDModelResult.OK)
		{
			return;
		}
		UIEvent.Send("SurvivalDoubleRewardsEnabled");
		UpdateUI();
		List<LootEntry> boosterDoubleRewards = GameManager.Instance.playerModel.WeeklySurvival.BoosterDoubleRewards;
		if (boosterDoubleRewards == null || boosterDoubleRewards.Count <= 0)
		{
			return;
		}
		IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
		BundleContentDefinition bundleContentDefinition = new BundleContentDefinition();
		bundleContentDefinition.RewardEntries = new Rewards();
		foreach (LootEntry boosterDoubleReward in GameManager.Instance.playerModel.WeeklySurvival.BoosterDoubleRewards)
		{
			if (boosterDoubleReward.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor || boosterDoubleReward.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon)
			{
				iAPConfirmPopupNew.AddEquipment(boosterDoubleReward.RewardedEquipment);
				bundleContentDefinition.RewardEntries.AddEquipmentClass(boosterDoubleReward.RewardedEquipment.EquipmentDefinitionIdentifier, boosterDoubleReward.RewardedEquipment.RarityLevel, boosterDoubleReward.RewardedEquipment.Level, 0);
			}
			else if (boosterDoubleReward.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Consumable)
			{
				bundleContentDefinition.RewardEntries.AddEquipmentConsumableClass(boosterDoubleReward.RewardedEquipment.EquipmentDefinitionIdentifier, boosterDoubleReward.RewardedAmount);
			}
			else
			{
				bundleContentDefinition.RewardEntries.AddRewardCurrency(boosterDoubleReward.RewardedCurrency, boosterDoubleReward.RewardedAmount, isDiamondExchange: false, canOverflowMax: false);
			}
		}
		iAPConfirmPopupNew.OpenForBundleContentDefinition(null, bundleContentDefinition, givenBySupport: false);
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnPopUpClose" && parameter is SurvivalRewardBoosterPopup)
		{
			ClearRewardItems();
		}
	}

	private void ClearRewardItems()
	{
		for (int i = 0; i < boosterRewards.Count; i++)
		{
			Helpers.DestroyOrCache(boosterRewards[i]);
		}
	}
}
