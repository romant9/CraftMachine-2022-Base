using BaseModel;
using TWDModel;
using UnityEngine;

namespace Client.BlackMarket
{
	public class BlackMarketCardUnlockHeroController : MonoBehaviour
	{
		private const string LocalizationKeyUnlockHeroTitle = "Popup.Shop.BlackMarket.LockedDeal.Unlock{Name}";

		private const string LocalizationKeyUpgradeHeroTitle = "Popup.Shop.BlackMarket.LockedDeal.Upgrade1{Name}";

		private const string LocalizationKeyUnlockHeroTokens = "Popup.Shop.BlackMarket.LockedDeal.TokensToUnlock{Name}";

		private const string LocalizationKeyTokensToUpgrade = "Popup.Shop.BlackMarket.LockedDeal.TokensToUpgrade";

		[SerializeField]
		private UIButton unlockHeroButton;

		[SerializeField]
		private UIButton moreTokensButton;

		[SerializeField]
		private UIButton upgradeHeroButton;

		[SerializeField]
		private UILabel unlockTitleText;

		[SerializeField]
		private UILabel upgradeTitleText;

		[SerializeField]
		private UILabel tokensText;

		[SerializeField]
		private UISprite currencyIcon;

		[SerializeField]
		private UILabel currencyAmount;

		[SerializeField]
		private UILabel upgradeLabel;

		[SerializeField]
		private GameObject[] stars;

		private ActorDefinition actorDefinition;

		public void SetActorDefinition(ActorDefinition actorDefinition)
		{
			this.actorDefinition = actorDefinition;
		}

		public void OnUnlockHeroButtonClickEventHandler()
		{
			if (actorDefinition != null && actorDefinition.ID.ToLower().Contains("hero") && !GameManager.Instance.playerModel.SurvivorContainer.HasHero(actorDefinition.ID))
			{
				SurvivorInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
				int num = GameManager.Instance.playerModel.SurvivorContainer.GetHighestLevelSurvivor() + actorDefinition.InitialLevelOffset;
				SurvivorModel survivorModel = GameManager.Instance.playerModel.SurvivorContainer.CreateSurvivorFromDefinition(actorDefinition.ID, num, num, actorDefinition.RarityLevel, num, actorDefinition.InitialEquipmentRarityLevel, new ModelRandom(), actorDefinition.InitialEquipmentsData[0].ID, actorDefinition.InitialEquipmentsData[1].ID, isMock: true);
				survivorModel.SetupMockTraits();
				ActorView.PrepareActor(survivorModel, isTransient: true);
				obj.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorHeroPreview;
				obj.OpenForModel(survivorModel);
			}
		}

		public void OnRadioCallButtonClickEventHandler()
		{
			SingularityMonoBehaviour<HUDManager>.Instance.CloseAllOpenPopupsAndDialogs();
			NewPhonePopup obj = (NewPhonePopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.NewRadioPopup);
			obj.OpenBlackMarketOnNextClose();
			obj.Open();
		}

		public void OnUpgradeHeroButtonClickEventHandler()
		{
			SurvivorModel survivorById = GameManager.Instance.playerModel.SurvivorContainer.GetSurvivorById(actorDefinition.ID);
			if (survivorById != null)
			{
				SurvivorInfoPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampSurvivorInfoPopup) as SurvivorInfoPopup;
				obj.currentStateMachineState = SurvivorInfoStateBase.States.SurvivorOverview;
				obj.OpenForModel(survivorById);
			}
		}

		public void ShowUpgradeRequirement(int minStars)
		{
			unlockTitleText.transform.parent.gameObject.SetActive(value: false);
			upgradeTitleText.transform.parent.gameObject.SetActive(value: true);
			HelpersUI.SetContentToLabel(upgradeTitleText, LocalizationManager.GetText("Popup.Shop.BlackMarket.LockedDeal.Upgrade1{Name}", actorDefinition.Name));
			string text = LocalizationManager.GetText("Popup.Shop.BlackMarket.LockedDeal.TokensToUpgrade");
			HelpersUI.SetContentToLabel(tokensText, text);
			for (int i = 0; i < stars.Length; i++)
			{
				stars[i].SetActive(i <= minStars);
			}
		}

		public void ShowUnlockRequirement()
		{
			unlockTitleText.transform.parent.gameObject.SetActive(value: true);
			upgradeTitleText.transform.parent.gameObject.SetActive(value: false);
			HelpersUI.SetContentToLabel(unlockTitleText, LocalizationManager.GetText("Popup.Shop.BlackMarket.LockedDeal.Unlock{Name}", actorDefinition.Name));
			string text = LocalizationManager.GetText("Popup.Shop.BlackMarket.LockedDeal.TokensToUnlock{Name}", actorDefinition.Name);
			HelpersUI.SetContentToLabel(tokensText, text);
		}

		public void HideRequirements()
		{
			unlockTitleText.transform.parent.gameObject.SetActive(value: false);
			upgradeTitleText.transform.parent.gameObject.SetActive(value: false);
		}

		public void ShowUpgradeBottomPanel()
		{
			CommonUI();
			CurrencyType traitUpgradeCurrency = actorDefinition.TraitUpgradeCurrency;
			SurvivorModel survivorById = GameManager.Instance.playerModel.SurvivorContainer.GetSurvivorById(actorDefinition.ID);
			int totalCost = survivorById.GetUpgradeTraitCashier().GetTotalCost(traitUpgradeCurrency);
			int value = GameManager.Instance.playerModel.GetCurrency(traitUpgradeCurrency).Value;
			bool flag = survivorById.CanUpgradeSurvivorRarity();
			bool flag2 = survivorById.CanUpgradeTraitRarity();
			string content = value + "/" + totalCost;
			HelpersUI.SetContentToLabel(currencyAmount, content);
			if (flag || flag2)
			{
				moreTokensButton.gameObject.SetActive(value: false);
				upgradeHeroButton.gameObject.SetActive(value: true);
				HelpersUI.SetContentToLabel(upgradeLabel, flag ? LocalizationManager.GetText("Popup.SurvivorInfo.Button.Promote") : LocalizationManager.GetText("Popup.SurvivorInfo.Button.Upgrade"));
			}
			else
			{
				moreTokensButton.gameObject.SetActive(value: true);
				upgradeHeroButton.gameObject.SetActive(value: false);
			}
			unlockHeroButton.gameObject.SetActive(value: false);
		}

		public void ShowUnlockBottomPanel()
		{
			CommonUI();
			CurrencyType traitUpgradeCurrency = actorDefinition.TraitUpgradeCurrency;
			Cashier heroUnlockCashier = GameManager.Instance.playerModel.SurvivorContainer.GetHeroUnlockCashier(traitUpgradeCurrency);
			bool flag = heroUnlockCashier.CanAfford();
			int totalCost = heroUnlockCashier.GetTotalCost(traitUpgradeCurrency);
			string content = GameManager.Instance.playerModel.GetCurrency(traitUpgradeCurrency).Value + "/" + totalCost;
			HelpersUI.SetContentToLabel(currencyAmount, content);
			if (flag)
			{
				unlockHeroButton.gameObject.SetActive(value: true);
				moreTokensButton.gameObject.SetActive(value: false);
			}
			else
			{
				unlockHeroButton.gameObject.SetActive(value: false);
				moreTokensButton.gameObject.SetActive(value: true);
			}
			upgradeHeroButton.gameObject.SetActive(value: false);
		}

		private void CommonUI()
		{
			CurrencyType traitUpgradeCurrency = actorDefinition.TraitUpgradeCurrency;
			currencyIcon.spriteName = HelpersGfx.GetCurrencyIconName(traitUpgradeCurrency);
		}
	}
}
