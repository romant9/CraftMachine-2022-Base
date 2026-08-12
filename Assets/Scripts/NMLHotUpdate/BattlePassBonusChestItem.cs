using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class BattlePassBonusChestItem : MonoBehaviour
{
	[SerializeField]
	private GameObject readyObject;

	[SerializeField]
	private GameObject claimObject;

	[SerializeField]
	private UILabel progressLabel;

	[SerializeField]
	private UIProgressBar progressBar;

	[SerializeField]
	private GameObject chestLockedOverlay;

	[SerializeField]
	private GameObject notEnoughBcButton;

	[SerializeField]
	private GameObject freeBCIcon;

	[SerializeField]
	private GameObject premiumBCIcon;

	[SerializeField]
	private GameObject chestBlockedOverlay;

	private BattlePassModel battlePass;

	private void Awake()
	{
		battlePass = GameManager.Instance.modelManager.Player.BattlePass;
		Helpers.GameObjectSetActive(chestLockedOverlay, !battlePass.PremiumActive);
		Helpers.GameObjectSetActive(chestBlockedOverlay, !battlePass.PremiumActive);
	}

	private void OnEnable()
	{
		battlePass.Changed += OnChange;
		RefreshState();
	}

	private void OnDisable()
	{
		battlePass.Changed -= OnChange;
	}

	private void OnChange(ModelObject model, string changed, object args)
	{
		switch (changed)
		{
		case "BonusChestClaimed":
		case "PremiumActivated":
		case "TierIncreased":
			RefreshState();
			break;
		}
	}

	public async void ClaimRewardClick()
	{
		if (battlePass.PremiumActive)
		{
			ClaimReward();
		}
		else if (await ((BattlePassPurchaseInfoPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BattlePassPremiumPurchaseInfoPopup)).OpenWithConfirmationAsync())
		{
			ClaimReward();
		}
	}

	private void ClaimReward()
	{
		if (battlePass.CanClaimTheBonusChest)
		{
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi) as OpenLootInUi)?.OpenForModel(battlePass);
		}
	}

	private void RefreshState()
	{
		bool flag = battlePass.AtMaxTier && battlePass.BattleCurrency.Value >= battlePass.BonusChestCost;
		Helpers.GameObjectSetActive(readyObject, battlePass.CanClaimTheBonusChest);
		Helpers.GameObjectSetActive(claimObject, flag);
		Helpers.GameObjectSetActive(notEnoughBcButton, !flag);
		int num = Mathf.Min(battlePass.AtMaxTier ? battlePass.BattleCurrency.Value : 0, battlePass.BonusChestCost);
		int bonusChestCost = battlePass.BonusChestCost;
		progressLabel.text = $"{num}/{bonusChestCost}";
		progressBar.Set((float)num / (float)bonusChestCost);
		Helpers.GameObjectSetActive(freeBCIcon, !battlePass.PremiumActive);
		Helpers.GameObjectSetActive(premiumBCIcon, battlePass.PremiumActive);
		Helpers.GameObjectSetActive(chestLockedOverlay, !battlePass.PremiumActive);
		Helpers.GameObjectSetActive(chestBlockedOverlay, !battlePass.PremiumActive);
	}

	public void OnOverlayClicked()
	{
		TooltipManager.OpenTextBoxWithText(chestLockedOverlay, LocalizationManager.GetText("Tooltip.BattlePass.ActivateToUnlock"));
	}

	public void InfoClick()
	{
		DropType usedDropType;
		List<ItemAmountProbabilityData> probabilities = battlePass.manager.GameEconomyData.GetCurrencyProbabilities(battlePass.BonusChestDropEventType, DropType.Gold, DropEventDefinition.DropEventContext.Normal, DropEventDefinition.DropEventTag.BonusCrate, battlePass.manager.Player.Level, out usedDropType, GameManager.Instance.playerModel.ActivityManager);
		DropRatesNamesHelper.GetNamesForDropCurrencies(ref probabilities);
		DropRatesInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.DropRatesInfoPopup) as DropRatesInfoPopup;
		DropTableItem dropTableItem = new DropTableItem
		{
			DropName = LocalizationManager.GetText("Popup.BattlePass.ExtraCurrencyChest.Title"),
			Probabilities = probabilities
		};
		obj.TryOpenWithNormalData(dropTableItem);
	}
}
