using Client.Utils;
using System.Collections.Generic;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class SurvivorBadgesIcon : NUIListItem<BadgeInfo>
{
	[Header("Tweens")]
	public int TweenGroupNewSelected = -1;

	[SerializeField]
	private GameObject tweenTarget;

	[Header("Icon Related")]
	[SerializeField]
	private UIButtonExtended iconButton;

	[SerializeField]
	private UISprite iconSprite;

	[SerializeField]
	private UISprite shapeSprite;

	[SerializeField]
	private UISprite raritySprite;

	[SerializeField]
	private UISprite rarityBorderSprite;

	[SerializeField]
	private UISprite equipmentRaritySprite;

	[SerializeField]
	private UISprite typeSprite;

	[Header("Rarity Stars")]
	[SerializeField]
	private GameObject[] starsArray;

	[SerializeField]
	private UIGrid starsGrid;

	[Header("Optional")]
	[Header("Effect Related")]
	[SerializeField]
	private GameObject effectParent;

	[SerializeField]
	private UIButtonExtended effectButton;

	[SerializeField]
	private UILabel effectNameLabel;

	[SerializeField]
	private UILabel effectNumberLabel;

	[SerializeField]
	private UILabel effectBonusLabel;

	[SerializeField]
	private GameObject capContainer;

	[SerializeField]
	private UILabel capDescriptionLabel;

	[SerializeField]
	private GameObject scrapContainer;

	[SerializeField]
	private GameObject scrapEquippedContainer;

	[SerializeField]
	private GameObject scrapSelectedContainer;

	[SerializeField]
	private UILabel scrapCostLabel;

	[SerializeField]
	private GameObject ownerContainer;

	[SerializeField]
	private UILabel ownerNameLabel;

	[SerializeField]
	private GameObject setBonusIndicator;

	[SerializeField]
	private GameObject badgeEffectParent;

	[SerializeField]
	private GameObject badgeEquippedEffect;

	[Header("Notification Related")]
	[SerializeField]
	private ThingsToDoIndicator badgesAvailableIndicator;

	[Header("Rotate Objects")]
	[SerializeField]
	private GameObject[] rotationObjects;

	[SerializeField]
	private GameObject selectedIndicator;

	[SerializeField]
	private GameObject unequippedButton;

	private int[] rotationBySlotIndex = new int[6] { 30, -30, -90, -150, 150, 90 };

	private bool scrapModeEnabled;

	private int currentSlotIndex = -1;

	private static List<BadgeModel> tempBadgeModelList;

	public void SetSlotIndex(int slotIndex)
	{
		currentSlotIndex = slotIndex;
	}

	public override void SetData(BadgeInfo data)
	{
		base.SetData(data);
		if (data != null && data.Model != null)
		{
			SetId(data.Model.SlotIndex.ToString());
			currentSlotIndex = data.Model.SlotIndex;
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		Helpers.GameObjectSetActive(iconButton, value: true);
		Helpers.GameObjectSetActive(effectButton, value: true);
		BadgeInfo data = GetData();
		Helpers.GameObjectSetActive(shapeSprite, value: true);
		if (data != null && data.Model != null)
		{
			if (OfflineManager.IsLoadDataManager)
			{
				if (BadgeLevelLabel != null)
				{
					BadgeLevelLabel.transform.parent.gameObject.SetActive(IsShowBadgeLevel);

					if (IsShowBadgeLevel)
					{
						BadgeLevelLabel.text = data.Model.EffectRoll.ToString();
					}
				}

				if (badgeRerolled != null)
				{
					badgeRerolled.SetActive(data.Model.HistoryBonus?.Count > 0 || data.Model.HistorySet?.Count > 0 || data.Model.HistorySlots?.Count > 0);
				}
				if (badgeFavourite != null)
				{
					badgeFavourite.SetActive(data.Model.IsFavorite);
				}
			}
			HelpersUI.SetSprite(iconSprite, HelpersGfx.GetBadgeEffectSprite(data.Model.EffectId));
			HelpersUI.SetSprite(raritySprite, HelpersGfx.GetBadgeRaritySprite(data.Model.Rarity));
			HelpersUI.SetSprite(rarityBorderSprite, HelpersGfx.GetRarityBorderSpriteName(data.Model.Rarity));
			HelpersUI.SetSprite(equipmentRaritySprite, HelpersGfx.GetEquipmentRaritySprite(data.Model.Rarity));
			HelpersUI.SetSprite(typeSprite, HelpersGfx.GetBadgeTypeSprite(data.Model.Type));
			if (starsArray != null)
			{
				for (int i = 0; i < starsArray.Length; i++)
				{
					Helpers.GameObjectSetActive(starsArray[i], data.Model.Rarity >= i);
				}
				if (starsGrid != null)
				{
					starsGrid.Reposition();
				}
			}
			Helpers.GameObjectSetActive(effectParent, value: true);
			HelpersUI.SetContentToLabel(effectNameLabel, HelpersLocalization.GetBadgeEffectTitle(data.Model));
			if (data.Model.BonusCondition is ConstantBonusCondition constantBonusCondition)
			{
				FixedPoint increment = data.Model.Increment;
				increment += FixedPoint.Max(1L, FixedPoint.Round(increment * (constantBonusCondition.BonusValue / 100.0)));
				if (data.SetBonusActive)
				{
					increment += increment * data.Model.GetBadgeSetBonus();
				}
				HelpersUI.SetContentToLabel(effectNumberLabel, HelpersLocalization.GetBadgeEffectDescription(data.Model, increment.UIRounding()));
				HelpersUI.SetContentToLabel(effectBonusLabel, "");
				EffectValue = (int)increment.UIRounding();
			}
			else
			{
				FixedPoint increment2 = data.Model.Increment;
				if (data.SetBonusActive)
				{
					increment2 += increment2 * data.Model.GetBadgeSetBonus();
				}
				HelpersUI.SetContentToLabel(effectNumberLabel, HelpersLocalization.GetBadgeEffectDescription(data.Model, increment2.UIRounding()));
				HelpersUI.SetContentToLabel(effectBonusLabel, HelpersLocalization.GetBadgeBonusDescription(data.Model));
				EffectValue = (int)increment2.UIRounding();
			}
			Helpers.GameObjectSetActive(capContainer, data.MaxSimilarBadgesReached);
			if (data.ScrapModeEnabled)
			{
				Helpers.GameObjectSetActive(scrapEquippedContainer, data.OwnerName != null);
				Helpers.GameObjectSetActive(scrapContainer, data.OwnerName == null);
				Helpers.GameObjectSetActive(scrapSelectedContainer, data.OwnerName == null && data.ScrapSelected);
				HelpersUI.SetContentToLabel(scrapCostLabel, data.Model.GetScrapCashier().GetTotalCost(CurrencyType.SurvivalPoints).ToString());
			}
			else
			{
				Helpers.GameObjectSetActive(scrapEquippedContainer, value: false);
				Helpers.GameObjectSetActive(scrapContainer, value: false);
				Helpers.GameObjectSetActive(scrapSelectedContainer, value: false);
			}
			Helpers.GameObjectSetActive(badgesAvailableIndicator, value: false);
			if (data.OwnerName != null)
			{
				Helpers.GameObjectSetActive(ownerContainer, value: true);
				HelpersUI.SetContentToLabel(ownerNameLabel, data.OwnerName);
			}
			else
			{
				Helpers.GameObjectSetActive(ownerContainer, value: false);
			}
			Helpers.GameObjectSetActive(setBonusIndicator, data.SetBonusActive);
		}
		else
		{
			Helpers.GameObjectSetActive(iconSprite, value: false);
			Helpers.GameObjectSetActive(raritySprite, value: false);
			Helpers.GameObjectSetActive(rarityBorderSprite, value: false);
			Helpers.GameObjectSetActive(equipmentRaritySprite, value: false);
			Helpers.GameObjectSetActive(typeSprite, value: false);
			if (starsArray != null)
			{
				for (int j = 0; j < starsArray.Length; j++)
				{
					Helpers.GameObjectSetActive(starsArray[j], value: false);
				}
			}
			Helpers.GameObjectSetActive(effectParent, value: false);
			Helpers.GameObjectSetActive(effectNameLabel, value: false);
			Helpers.GameObjectSetActive(effectNumberLabel, value: false);
			Helpers.GameObjectSetActive(effectBonusLabel, value: false);
			Helpers.GameObjectSetActive(capContainer, value: false);
			Helpers.GameObjectSetActive(ownerContainer, value: false);
			Helpers.GameObjectSetActive(setBonusIndicator, value: false);
			if (Helpers.GameObjectSetActive(badgesAvailableIndicator, currentSlotIndex != -1))
			{
				GameManager.Instance.playerModel.Equipment.GetBadgesWithSlotIndex(currentSlotIndex, ref tempBadgeModelList);
				int number = ((tempBadgeModelList != null) ? tempBadgeModelList.Count : 0);
				badgesAvailableIndicator.SetNumber(number);
			}
		}
		Vector3 staticVector3One = Helpers.staticVector3One;
		if (rotationObjects == null || rotationBySlotIndex == null || currentSlotIndex <= -1 || currentSlotIndex >= rotationBySlotIndex.Length)
		{
			return;
		}
		for (int k = 0; k < rotationObjects.Length; k++)
		{
			if (rotationObjects[k] != null && rotationObjects[k].transform != null)
			{
				staticVector3One = rotationObjects[k].transform.localEulerAngles;
				staticVector3One.z = rotationBySlotIndex[currentSlotIndex];
				rotationObjects[k].transform.localEulerAngles = staticVector3One;
			}
		}
	}

	public void SetId(string id)
	{
		if (iconButton != null)
		{
			iconButton.id = id;
		}
		if (effectButton != null)
		{
			effectButton.id = id;
		}
	}

	public void SetClickCallbacks(UIButtonExtended.OnClickCallback iconClickCallback, UIButtonExtended.OnClickCallback effectClickCallback)
	{
		if (iconButton != null)
		{
			if (iconClickCallback != null)
			{
				iconButton.SetClickCallback(iconClickCallback);
			}
			else
			{
				iconButton.Clear();
			}
		}
		if (effectButton != null)
		{
			if (effectClickCallback != null)
			{
				effectButton.SetClickCallback(effectClickCallback);
			}
			else
			{
				effectButton.Clear();
			}
		}
	}

	public void SetSelected(bool selected)
	{
		selectedIndicator.SetActive(selected);
	}

	public UIButtonExtended GetIconButton()
	{
		return iconButton;
	}

	public override void Clear()
	{
		base.Clear();
		if (iconButton != null)
		{
			iconButton.Clear();
		}
		if (effectButton != null)
		{
			effectButton.Clear();
		}
		currentSlotIndex = -1;
		if (tempBadgeModelList != null)
		{
			tempBadgeModelList.Clear();
		}
	}

	private void OnUIEvent(string type, object param)
	{
		if (type == "OnBadgeEquipped" && GetData() != null && GetData().Model != null && param is BadgeModel)
		{
			bool num = (param as BadgeModel).ModelId == GetData().Model.ModelId;
			if (num && TweenGroupNewSelected != -1 && tweenTarget != null)
			{
				TweenManager.PlayTweenGroup(tweenTarget, TweenGroupNewSelected);
			}
			if (num && badgeEquippedEffect != null && badgeEffectParent != null)
			{
				Helpers.InstantiateToParent(badgeEquippedEffect, badgeEffectParent);
			}
		}
	}

	private void OnEnable()
	{
		if (OfflineManager.IsLoadDataManager && BadgeCraft.Instance)
		{
			BadgeCraft.Instance.On_Fafourite += CheckFavorite;
		}
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		if (OfflineManager.IsLoadDataManager && BadgeCraft.Instance)
		{
			BadgeCraft.Instance.On_Fafourite -= CheckFavorite;
		}
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	public void ShowUnequippedButton(bool b)
	{
		if (unequippedButton != null)
		{
			unequippedButton.SetActive(b);
		}
	}



	#region myparams
	public int EffectValue { get; set; }
	[SerializeField]
	private UILabel BadgeLevelLabel;

	public bool IsShowBadgeLevel;
	public UILabel rerollIndex;
	public UILabel rerollPrice;
	public GameObject badgeRerolled;
	public GameObject badgeFavourite;
	#endregion

	#region mycode
	private void CheckFavorite(int modelId)
	{
		if (badgeFavourite != null)
		{
			var data = GetData();
			if (data != null && data.Model != null && data.Model.ModelId == modelId)
			{
				badgeFavourite.SetActive(data.Model.IsFavorite);
			}
		}
	}
	#endregion
}
