using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class EquipmentTokenButton : MonoBehaviour
{
	[SerializeField]
	private UITexture icon;

	[SerializeField]
	private UILabel amountLabel;

	[SerializeField]
	private UILabel tokenAmountLabel;

	[SerializeField]
	private UILabel commonTokenAmountLabel;

	[SerializeField]
	private UITexture tokenIcon;

	[SerializeField]
	private GameObject tokenInfo;

	[SerializeField]
	private GameObject locked;

	[SerializeField]
	private UIButton assembleButton;

	[SerializeField]
	private UISprite[] traitsSprites;

	[SerializeField]
	private UISprite classIconSprite;

	[Header("Functionality indicators")]
	[SerializeField]
	private GameObject indicatorInfusedEquipment;

	[SerializeField]
	private GameObject indicatorSpecialFunctionalityEquipment;

	[SerializeField]
	private GameObject selectionHighlight;

	[SerializeField]
	private GameObject indicatorSpecialFunctionalityAndInfused;

	[SerializeField]
	private GameObject sp7Star;

	private EquipTokenItemModel _equipTokenItemModel;

	private bool canClick;

	private string onClickUIEvent;

	public void Setup(EquipTokenItemModel equipTokenItemModel)
	{
		_equipTokenItemModel = equipTokenItemModel;
		canClick = true;
		if (_equipTokenItemModel == null)
		{
			Debug.LogError("EquipTokenItemModel Null,Setup Failed");
			return;
		}

		if (RewardHighlight != null) RewardHighlight.SetActive(false);

		icon.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipTokenItemModel.EquipmentDefinition);
		icon.transform.localEulerAngles = new Vector3(0f, 0f, IsWeaponEquipment(equipTokenItemModel.EquipmentDefinition) ? 45f : 0f);
		if (tokenAmountLabel != null)
		{
			tokenAmountLabel.text = HelpersString.FormatNumberWithToken(equipTokenItemModel.OwnedTokensAmount, equipTokenItemModel.Definition.TokensToUnlock);
		}
		if (commonTokenAmountLabel != null)
		{
			commonTokenAmountLabel.text = HelpersString.FormatNumberWithToken(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.ApocalypticEquipToken).Value, equipTokenItemModel.Definition.ApocalypticEquipToken);
		}
		if (tokenIcon != null)
		{
			tokenIcon.mainTexture = HelpersGfx.GetEquipmentTokenIconTexture(equipTokenItemModel.Definition);
		}
		HelpersUI.SetSprite(classIconSprite, HelpersGfx.GetSurvivorClassSmallIconName(equipTokenItemModel.EquipmentDefinition.SurvivorClass));
		Helpers.GameObjectSetActive(locked, !equipTokenItemModel.CanUnlock());
		UpdateTraits(traitsSprites, equipTokenItemModel.EquipmentItemModel.UpgradeTraits);
		UpdateTraitIndicators(equipTokenItemModel.EquipmentDefinition);
		SetOkButtonState();
		Helpers.GameObjectSetActive(assembleButton, value: true);
		Helpers.GameObjectSetActive(tokenAmountLabel, value: true);
		Helpers.GameObjectSetActive(commonTokenAmountLabel, value: true);
		Helpers.GameObjectSetActive(tokenInfo, value: true);
		Helpers.GameObjectSetActive(amountLabel, value: false);
		Helpers.GameObjectSetActive(selectionHighlight, value: false);
		Helpers.GameObjectSetActive(sp7Star, value: false);
		if (equipTokenItemModel.Definition != null && equipTokenItemModel.Definition.Star == 6)
		{
			Helpers.GameObjectSetActive(sp7Star, value: true);
		}
	}

	public void SetUpForReward(EquipTokenItemModel equipTokenItemModel, string clickUIEvent = "")
	{
		DebugTWD.Log("Try setup For Reward: " + equipTokenItemModel.EquipTokenId);

		Setup(equipTokenItemModel);
		onClickUIEvent = clickUIEvent;
		canClick = false;
		Helpers.GameObjectSetActive(locked, value: false);
		Helpers.GameObjectSetActive(assembleButton, value: false);
		amountLabel.text = equipTokenItemModel.OwnedTokensAmount.ToString() ?? "";
		Helpers.GameObjectSetActive(tokenInfo, value: false);
		Helpers.GameObjectSetActive(amountLabel, value: true);
	}

	public void SetUpForCampaign(RewardEquipToken rewardEquipToken)
	{
		SetUpForReward(rewardEquipToken.FakeRewardEquipTokenItemModel(GameManager.Instance.modelManager));
		canClick = true;
	}

	public void SetUpForRemoldScrap(RewardEquipToken rewardEquipToken)
	{
		SetUpForReward(rewardEquipToken.FakeRewardEquipTokenItemModel(GameManager.Instance.modelManager));
		canClick = false;
	}

	public void SetUpForTrade(RewardEquipToken rewardEquipToken)
	{
		SetUpForReward(rewardEquipToken.FakeRewardEquipTokenItemModel(GameManager.Instance.modelManager));
		canClick = true;
		Helpers.GameObjectSetActive(amountLabel, value: false);
	}

	private void SetOkButtonState()
	{
		if (!(assembleButton == null))
		{
			if (_equipTokenItemModel.CanUnlock())
			{
				assembleButton.enabled = true;
				assembleButton.SetState(UIButtonColor.State.Normal, true);
			}
			else
			{
				assembleButton.enabled = false;
				assembleButton.SetState(UIButtonColor.State.Disabled, true);
			}
		}
	}

	private void UpdateTraits(UISprite[] traitsArray, List<UpgradeTraitsData> upgradeTraitsDataList)
	{
		DebugTWD.Log("UpgradeTraits Count: " + upgradeTraitsDataList.Count);

		if (traitsArray != null && traitsArray.Length != 0 && upgradeTraitsDataList == null)
		{
			for (int i = 0; i < traitsArray.Length; i++)
			{
				traitsArray[i].gameObject.SetActive(value: true);
				traitsArray[i].spriteName = HelpersGfx.GetEquipmentTraitIconName(null);
				traitsArray[i].color = Color.white;
			}
			return;
		}
		if (traitsArray != null && traitsArray.Length != 0 && upgradeTraitsDataList == null)
		{
			for (int j = 0; j < traitsArray.Length; j++)
			{
				traitsArray[j].gameObject.SetActive(value: false);
			}
		}
		if (traitsArray == null || traitsArray.Length == 0 || upgradeTraitsDataList == null)
		{
			return;
		}
		for (int k = 0; k < traitsArray.Length; k++)
		{
			if (traitsArray[k] != null)
			{
				if (upgradeTraitsDataList.Count > k + 1)
				{
					traitsArray[k].gameObject.SetActive(value: true);
					traitsArray[k].spriteName = HelpersGfx.GetEquipmentTraitIconName(upgradeTraitsDataList[k + 1]);
					traitsArray[k].color = Color.white;
				}
				else
				{
					traitsArray[k].gameObject.SetActive(value: false);
				}
			}
		}
	}

	private void UpdateTraitIndicators(EquipmentDefinition equipmentDefinition)
	{
		if (equipmentDefinition == null)
		{
			Debug.LogError("UpdateTraitIndicators Failed,equipmentDefinition is null");
			return;
		}
		bool flag = !string.IsNullOrEmpty(equipmentDefinition.InfusedTrait);
		bool flag2 = !string.IsNullOrEmpty(equipmentDefinition.SpecialTrait) || !string.IsNullOrEmpty(HelpersLocalization.GetEquipmentSpecialDescription(equipmentDefinition));
		Helpers.GameObjectSetActive(indicatorSpecialFunctionalityAndInfused, flag && flag2);
		Helpers.GameObjectSetActive(indicatorInfusedEquipment, flag && !flag2);
		Helpers.GameObjectSetActive(indicatorSpecialFunctionalityEquipment, flag2 && !flag);
	}

	private bool IsWeaponEquipment(EquipmentDefinition definition)
	{
		if (definition != null)
		{
			if (definition.Category != EquipmentCategory.MeleeWeapon)
			{
				return definition.Category == EquipmentCategory.RangeWeapon;
			}
			return true;
		}
		return false;
	}

	public void OnEquipmentButtonClicked()
	{
		if (IsBreakthroughMode())
		{
			OnEquipmentButtonClickedBT();
			return;
		}
		if (!string.IsNullOrEmpty(onClickUIEvent))
		{
			UIEvent.Send(onClickUIEvent, this);
		}
		if (canClick)
		{
			if (_equipTokenItemModel == null)
			{
				Debug.LogError("EquipTokenItemModel Null,OnEquipmentButtonClicked Failed");
				return;
			}
			RewardEquipment rewardEquipment = _equipTokenItemModel.RewardEquipment;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
			Helpers.OpenEquipmentUpgradePopupPreview(rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager), rewardEquipment.RarityLevel).ShowNextLevel = false;
			if (!OfflineManager.IsLoadDataManager) CampHUD.Get().PauseCurrencyMeters = false;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
		}
	}

	public void OnClaimButtonClicked()
	{
		if (OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (OfflineManager.IsLoadDataManager)");
			if (_equipTokenItemModel == null)
			{
				DebugTWD.LogError("EquipTokenItemModel Null,OnClaimButtonClicked Failed");
				return;
			}
			_equipTokenItemModel.UnlockEquip();
			OnBuyCommandCompleted(TWDModelResult.OK);
		}
		else
		{
			ConfirmationPopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ConfirmationPopup) as ConfirmationPopup;
			obj.SetContent("", LocalizationManager.GetText("Popup.EquipmentSynthesis.Tips1"));
			obj.SetCallbacks(delegate
			{
				if (_equipTokenItemModel == null)
				{
					DebugTWD.LogError("EquipTokenItemModel Null,OnClaimButtonClicked Failed");
				}
				else
				{
					UnlockEquipCommand command = new UnlockEquipCommand(_equipTokenItemModel);
					OnBuyCommandCompleted(Helpers.ExecuteCommand(command));
				}
			});
			obj.SetOkButtonLabel(LocalizationManager.GetText("Button.Ok"));
			obj.SetCancelButtonLabel(LocalizationManager.GetText("Button.Cancel"));
			obj.Open();
		}
	}

	private void OnBuyCommandCompleted(TWDModelResult result)
	{
		if (result != TWDModelResult.OK)
		{
			return;
		}
		Setup(_equipTokenItemModel);
		if (!OfflineManager.IsLoadDataManager)
		{
			DebugTWD.LogMycode("if (!OfflineManager.IsLoadDataManager)");
			IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
			if (iAPConfirmPopupNew != null)
			{
				iAPConfirmPopupNew.ShowShopWhenClosed = true;
				iAPConfirmPopupNew.OpenForRewards(new List<IReward> { _equipTokenItemModel.RewardEquipment });
				iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
				if (SingularityMonoBehaviour<AudioManager>.Instance != null)
				{
					SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/purchase");
				}
			}
		}
		UIEvent.Send("OnEquipTokenUnlockEvent");
	}

	private bool IsBreakthroughMode()
	{
		BreakThroughPopup breakThroughPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BreakThroughPopup) as BreakThroughPopup;
		if (breakThroughPopup != null && breakThroughPopup.IsOpen && selectionHighlight != null)
		{
			return true;
		}
		return false;
	}

	public void SetSelectState(bool select)
	{
		Helpers.GameObjectSetActive(selectionHighlight, select);
		if (select)
		{
			UIEvent.Send("BreakThroughSelected", _equipTokenItemModel.EquipTokenId);
		}
		else
		{
			UIEvent.Send("BreakThroughUnSelected", _equipTokenItemModel.EquipTokenId);
		}
	}

	public void SetSelectStateUI(bool select)
	{
		Helpers.GameObjectSetActive(selectionHighlight, select);
	}

	private void OnEquipmentButtonClickedBT()
	{
		BreakThroughPopup breakThroughPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BreakThroughPopup) as BreakThroughPopup;
		if (!(breakThroughPopup == null) && breakThroughPopup.IsOpen && !(selectionHighlight == null))
		{
			bool activeInHierarchy = selectionHighlight.activeInHierarchy;
			if (!breakThroughPopup.IsEnoughSelected() || activeInHierarchy)
			{
				SetSelectState(!activeInHierarchy);
			}
		}
	}


	#region myparams
	[SerializeField]
	private GameObject RewardHighlight;
	#endregion

	#region mycode
	public void SetAmount(int amount)
	{
		if (_equipTokenItemModel != null)
		{
			if (_equipTokenItemModel.OwnedTokensAmount < 1)
				_equipTokenItemModel.AddEquipToken(amount);
			var apoTokens = GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.ApocalypticEquipToken).Value;
			var apoBase = _equipTokenItemModel.Definition.ApocalypticEquipToken;
			commonTokenAmountLabel.text = HelpersString.FormatNumberWithToken(apoTokens, apoBase);
			Helpers.GameObjectSetActive(locked, apoTokens < apoBase);
		}
	}
	#endregion
}
