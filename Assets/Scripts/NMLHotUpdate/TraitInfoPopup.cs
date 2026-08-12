using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class TraitInfoPopup : HUDElement
{
	private TraitDefinition trait;

	private SurvivorModel survivor;

	[SerializeField]
	private UISprite traitIcon;

	[SerializeField]
	private UILabel traitName;

	[SerializeField]
	private UILabel traitLevel;

	[SerializeField]
	private UILabel traitDescription;

	[SerializeField]
	private UILabel failReason;

	[SerializeField]
	private UILabel amountLeftLabel;

	[SerializeField]
	private GameObject traitRerollContainer;

	[SerializeField]
	private GameObject traitRerollNotAvailableContainer;

	[SerializeField]
	private UIButtonWithLabelAndIcon rerollButton;

	[SerializeField]
	private UISprite currencyIcon;

	[SerializeField]
	private GameObject sfxGlint;

	[SerializeField]
	private UIScrollView desScrollView;

	private bool hasConfirmedPurchase;

	public void OpenForModel(SurvivorModel survivorModel, TraitDefinition traitDefinition, SurvivorInfoStateBase.States state)
	{
		if (IsLoadDataManager) Helpers.GameObjectSetActive(sfxGlint, value: false);
		trait = traitDefinition;
		survivor = survivorModel;
		traitIcon.spriteName = HelpersGfx.GetSurvivorTraitIconName(traitDefinition);
		HelpersUI.SetContentToLabel(traitName, HelpersLocalization.GetTraitName(traitDefinition));
		HelpersUI.SetContentToLabel(traitLevel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Generic.Level{Level}", UpgradeTraitsData.GetTraitLevelIdentifier(trait.Identifier) + 1));
		HelpersUI.SetContentToLabel(traitDescription, HelpersLocalization.GetTraitDescription(trait));
		if (desScrollView) desScrollView.UpdatePosition();
		bool flag = survivor.CanRerollTrait && !traitDefinition.HasTag("FactionBuffTrait");
		Helpers.GameObjectSetActive(rerollButton, flag);
		HelpersUI.SetContentToLabel(amountLeftLabel, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TrainingGround.TokenAmount", GameManager.Instance.playerModel.GetCurrencyAmount(CurrencyType.TraitRerollToken)));
		if (state == SurvivorInfoStateBase.States.SurvivorOverview)
		{
			if (flag)
			{
				Helpers.GameObjectSetActive(traitRerollContainer, value: true);
				Helpers.GameObjectSetActive(traitRerollNotAvailableContainer, value: false);
				Cashier traitRerollCashier = survivor.GetTraitRerollCashier(trait.Identifier);
				CurrencyType survivorTraitUpgradeCurrencyType = SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(survivor);
				rerollButton.SetContentToIconOne(HelpersGfx.GetCurrencyIconName(CurrencyType.TraitRerollToken));
				rerollButton.SetContentToIconTwo(HelpersGfx.GetCurrencyIconName(survivorTraitUpgradeCurrencyType));
				rerollButton.SetContentToLabelTwo(traitRerollCashier.GetTotalCost(CurrencyType.TraitRerollToken).ToString());
				rerollButton.SetContentToLabelThree(traitRerollCashier.GetTotalCost(survivorTraitUpgradeCurrencyType).ToString());
				if (IsLoadDataManager && (DataManager.Instance.SurvivorManagementPopUp.IsTraitRerollFree || OfflineManager.IsFreeAll))
				{
					rerollButton.isEnabled = true;
					rerollButton.SetClickCallback(RerollTrait);
				}
				else
				{
					if (!traitRerollCashier.CanAfford())
					{
						rerollButton.isEnabled = false;
						Helpers.GameObjectSetActive(sfxGlint, value: false);
						if (traitRerollCashier.CanPay(CurrencyType.TraitRerollToken))
						{
							rerollButton.SetStateToLabelTwo(UIButtonColor.State.Normal, immediate: false, lockstate: true);
						}
						else if (traitRerollCashier.CanPay(survivorTraitUpgradeCurrencyType))
						{
							rerollButton.SetStateToLabelThree(UIButtonColor.State.Normal, immediate: false, lockstate: true);
						}
					}
					else
					{
						rerollButton.SetClickCallback(RerollTrait);
					}
				}
			}
			else
			{
				Helpers.GameObjectSetActive(traitRerollContainer, value: false);
				Helpers.GameObjectSetActive(traitRerollNotAvailableContainer, value: true);
				if (traitDefinition.HasTag("FactionBuffTrait"))
				{
					HelpersUI.SetContentToLabel(failReason, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TrainingGround.Reroll.LeaderFail"));
				}
				else if (survivor.IsUpgrading())
				{
					HelpersUI.SetContentToLabel(failReason, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TrainingGround.Reroll.UpgradeFail"));
				}
				else
				{
					HelpersUI.SetContentToLabel(failReason, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TrainingGround.Reroll.NeedLegendary"));
				}
			}
		}
		else
		{
			HelpersUI.SetContentToLabel(failReason, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.TrainingGround.Reroll.NeedUnlock"));
			Helpers.GameObjectSetActive(traitRerollContainer, value: false);
			Helpers.GameObjectSetActive(traitRerollNotAvailableContainer, value: true);
		}
		base.Open();
	}

	private void OpenConfirmationPopup()
	{
		Cashier traitRerollCashier = survivor.GetTraitRerollCashier(trait.Identifier);
		BuyResourcesPopup obj = (BuyResourcesPopup)SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BuyResourcesPopup);
		obj.SetConfirmContent(LocalizationManager.GetText("Popup.BuyResources.TradeCrate"), LocalizationManager.GetText("Popup.Badges.Details.RerollTitle"), traitRerollCashier.GetTotalCost(CurrencyType.TraitRerollToken), CurrencyType.TraitRerollToken);
		obj.SetCallbacks(ConfirmationCallback);
		obj.Open();
	}

	private void ConfirmationCallback()
	{
		hasConfirmedPurchase = true;
		RerollTrait(rerollButton);
	}

	//кнопка Подтвердить
	private void RerollTrait(UIButtonExtended button)
	{
		hasConfirmedPurchase = true;
		bool CanRerollTrait = IsLoadDataManager ? true : survivor.CanRerollTrait;

		if (CanRerollTrait && hasConfirmedPurchase)
		{
			if (!IsLoadDataManager)
			{
				GameManager.Instance.CheckConnectionReachability(showPopup: true, "RerollSurvivorTraitCommand");
				UIEvent.Send("SurvivorTraitRerolled", trait.Identifier);
				hasConfirmedPurchase = false;
				Close();
			}
			else
			{
				DebugTWD.Log("OnRerollClick");
				var survivorManagementPopUp = DataManager.Instance.SurvivorManagementPopUp;

				int index = survivorManagementPopUp.rerollTraitIndexCurrent - 1;

				if (index >= 0)
				{
					survivorManagementPopUp.BackupTraitsData(survivor, trait, index);
				}

				survivorManagementPopUp.SurvivorInfoPopupCurrent.OnTraitRerollButtonClicked(trait.Identifier);
				hasConfirmedPurchase = false;
				gameObject.SetActive(false);
			}
		}
		else if (survivor.CanRerollTrait)
		{
			OpenConfirmationPopup();
		}
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion

	#region mycode
	public override void Close()
	{
		base.Close();
	}
	#endregion
}