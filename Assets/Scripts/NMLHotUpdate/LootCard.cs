using System;
using TWDModel;
using UnityEngine;

public class LootCard : MonoBehaviour
{
	[Tooltip("Label for gained currency amount.")]
	[SerializeField]
	private UILabel amountLabel;

	[Tooltip("Icon for gained currency icon.")]
	[SerializeField]
	private UISprite currencyIcon;

	[Tooltip("Texture for gained consumable icon.")]
	[SerializeField]
	private UITexture consumableTexture;

	[Tooltip("Prefab for equipment info.")]
	[SerializeField]
	private GameObject equipmentCardPrefab;

	[SerializeField]
	private GameObject avatarCardPrefab;

	[Tooltip("Container for equipment info.")]
	[SerializeField]
	private GameObject cardContainer;

	[Tooltip("Label to indicate the weapon has been scrapped.")]
	[SerializeField]
	private UILabel scrappedLabel;

	[Tooltip("Rarity border.")]
	[SerializeField]
	private UISprite border;

	[Tooltip("Gradient.")]
	[SerializeField]
	private GameObject gradient;

	[Tooltip("Background object of gold loot.")]
	[SerializeField]
	private GameObject goldBackground;

	[Tooltip("Background object of silver loot.")]
	[SerializeField]
	private GameObject silverBackground;

	[Tooltip("Background object of regular loot.")]
	[SerializeField]
	private GameObject regularBackground;

	[Tooltip("Background object of gold loot card back.")]
	[SerializeField]
	private GameObject goldBackgroundCardBack;

	[Tooltip("Background object of silver loot card back.")]
	[SerializeField]
	private GameObject silverBackgroundCardBack;

	[Tooltip("Background object of regular loot card back.")]
	[SerializeField]
	private GameObject regularBackgroundCardBack;

	[Tooltip("Card reveal effect for common loot")]
	[SerializeField]
	private GameObject commonRarityEffect;

	[Tooltip("Card reveal effect for common loot")]
	[SerializeField]
	private GameObject uncommonRarityEffect;

	[Tooltip("Card reveal effect for common loot")]
	[SerializeField]
	private GameObject rareRarityEffect;

	[Tooltip("Card reveal effect for common loot")]
	[SerializeField]
	private GameObject epicRarityEffect;

	[Tooltip("Card reveal effect for common loot")]
	[SerializeField]
	private GameObject legendaryRarityEffect;

	[Tooltip("Container for the double xp effect")]
	[SerializeField]
	private GameObject doubleXpContainer;

	[Tooltip("Container for the double reward effect for distance missions")]
	[SerializeField]
	private GameObject survivalDoubleRewardContainer;

	[Tooltip("Collider of the card. Used to determin the size of card for placement")]
	[SerializeField]
	private BoxCollider boxCollider;

	[Tooltip("Animator component of the loot card.")]
	[SerializeField]
	private Animator animator;

	[Tooltip("Parent object of the boosted reward banner")]
	[SerializeField]
	private GameObject boostedRewardParent;

	[Tooltip("Booster tween group")]
	[SerializeField]
	private int boosterTweenGroup = 5;

	private LootEntry rewardLootEntry;

	private GameObject instantiatedEquipmentCard;

	private bool showEffects = true;

	public EquipmentItemModel rewardEquipment { get; set; }

	public bool ShowFlyingCurrencies { get; set; }

	public void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	public void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		UpdateCampHudPhoneNumber();
	}

	public Vector3 GetSize()
	{
		if (boxCollider != null)
		{
			return boxCollider.size;
		}
		Debug.LogError("Loot card has no collider to determin its size!");
		return Vector2.zero;
	}

	private void OnUIEvent(string type, object parameter)
	{
		switch (type)
		{
		case "OnEquipmentUpdated":
			if (rewardLootEntry != null && rewardLootEntry.RewardedCurrency == CurrencyType.None)
			{
				bool flag = IsEquipmentAvailable();
				if (instantiatedEquipmentCard != null)
				{
					instantiatedEquipmentCard.SetActive(flag);
				}
				if (scrappedLabel != null)
				{
					scrappedLabel.gameObject.SetActive(!flag);
				}
				border.gameObject.SetActive(!flag);
				goldBackground.gameObject.SetActive(!flag);
				silverBackground.gameObject.SetActive(!flag);
				regularBackground.gameObject.SetActive(!flag);
				gradient.SetActive(!flag);
			}
			break;
		case "OnNewEquipmentCardSelected":
		{
			EquipmentButton equipmentButton = parameter as EquipmentButton;
			if (equipmentButton != null && equipmentButton.GetEquipment() == rewardEquipment)
			{
				OnEquipmentCardClicked();
			}
			break;
		}
		case "OnPopUpOpen":
			if (parameter is IngameLoading { isShowLootCard: false })
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			break;
		}
	}

	private bool IsEquipmentAvailable()
	{
		if (rewardEquipment != null && rewardEquipment.manager != null)
		{
			return rewardEquipment.manager.Player.Equipment.Contains(rewardEquipment);
		}
		return false;
	}

	private bool IsAvatarAvailable()
	{
		if (rewardLootEntry != null)
		{
			return rewardLootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Avatars;
		}
		return false;
	}

	public void SetAnimationParameters(bool moveToLeft, bool moveToRight)
	{
		if (animator != null)
		{
			animator.SetBool("MoveToLeft", moveToLeft);
			animator.SetBool("MoveToRight", moveToRight);
		}
	}

	public void SetReward(LootEntry reward, bool showEffects)
	{
		this.showEffects = showEffects;
		rewardLootEntry = reward;
		goldBackground.SetActive(value: false);
		silverBackground.SetActive(value: false);
		regularBackground.SetActive(value: false);
		goldBackgroundCardBack.SetActive(value: false);
		silverBackgroundCardBack.SetActive(value: false);
		regularBackgroundCardBack.SetActive(value: false);
		Helpers.GameObjectSetActive(boostedRewardParent, value: false);
		if (reward.DropType == DropType.Gold)
		{
			goldBackground.SetActive(value: true);
			goldBackgroundCardBack.SetActive(value: true);
			EffectSparkle component = goldBackground.GetComponent<EffectSparkle>();
			if (component != null)
			{
				component.enabled = true;
			}
		}
		else if (reward.DropType == DropType.Silver)
		{
			silverBackground.SetActive(value: true);
			silverBackgroundCardBack.SetActive(value: true);
			EffectSparkle component2 = silverBackground.GetComponent<EffectSparkle>();
			if (component2 != null)
			{
				component2.enabled = true;
			}
		}
		else
		{
			regularBackground.SetActive(value: true);
			regularBackgroundCardBack.SetActive(value: true);
		}
		bool flag = rewardLootEntry.RewardedCurrency == CurrencyType.SurvivalPoints && GameManager.Instance.playerModel.IsTimedBonusActive(TimedBonusType.DoubleXp);
		bool flag2 = false;
		if (reward.RewardedCurrency != CurrencyType.None)
		{
			amountLabel.gameObject.SetActive(value: true);
			currencyIcon.gameObject.SetActive(value: true);
			Helpers.GameObjectSetActive(consumableTexture, value: false);
			amountLabel.text = reward.RewardedAmount.ToString();
			flag2 = GameManager.Instance.playerModel.WeeklySurvival.DoubleRewardsEnabled && rewardLootEntry.IsSurvivalReward();
			if (flag)
			{
				amountLabel.text = (reward.RewardedAmount / (2 + (flag2 ? 2 : 0))).ToString() ?? "";
			}
			else if (flag2)
			{
				amountLabel.text = (reward.RewardedAmount / 2).ToString() ?? "";
			}
			currencyIcon.spriteName = HelpersGfx.GetCurrencyIconName(reward.RewardedCurrency, GameManager.Instance.playerModel);
		}
		else if (reward.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.ChallengeSkipToken)
		{
			amountLabel.gameObject.SetActive(value: true);
			currencyIcon.gameObject.SetActive(value: true);
			Helpers.GameObjectSetActive(consumableTexture, value: false);
			amountLabel.text = reward.ChallengeSkipToken.ToString();
			flag2 = GameManager.Instance.playerModel.WeeklySurvival.DoubleRewardsEnabled && rewardLootEntry.IsSurvivalReward();
			if (flag)
			{
				amountLabel.text = (reward.ChallengeSkipToken / (2 + (flag2 ? 2 : 0))).ToString() ?? "";
			}
			else if (flag2)
			{
				amountLabel.text = (reward.ChallengeSkipToken / 2).ToString() ?? "";
			}
			currencyIcon.spriteName = "Ui_Icon_Round_Pass";
		}
		else if (reward.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Consumable)
		{
			Helpers.GameObjectSetActive(currencyIcon, value: false);
			Helpers.GameObjectSetActive(amountLabel, value: true);
			Helpers.GameObjectSetActive(consumableTexture, value: true);
			consumableTexture.mainTexture = HelpersGfx.GetEquipmentIconTexture(reward.RewardedEquipment);
			flag2 = GameManager.Instance.playerModel.WeeklySurvival.DoubleRewardsEnabled && rewardLootEntry.IsSurvivalReward();
			amountLabel.text = (reward.RewardedAmount / ((!flag2) ? 1 : 2)).ToString();
		}
		else
		{
			amountLabel.gameObject.SetActive(value: false);
			currencyIcon.gameObject.SetActive(value: false);
			Helpers.GameObjectSetActive(consumableTexture, value: false);
			if (scrappedLabel != null)
			{
				scrappedLabel.gameObject.SetActive(value: false);
			}
			rewardEquipment = reward.RewardedEquipment;
			bool flag3 = IsEquipmentAvailable();
			border.gameObject.SetActive(!flag3);
			goldBackground.gameObject.SetActive(!flag3);
			silverBackground.gameObject.SetActive(!flag3);
			regularBackground.gameObject.SetActive(!flag3);
			gradient.SetActive(!flag3);
			if (flag3)
			{
				if (instantiatedEquipmentCard == null)
				{
					instantiatedEquipmentCard = Helpers.InstantiateToParentAndLayer(equipmentCardPrefab, cardContainer);
					instantiatedEquipmentCard.GetComponent<EquipmentButton>().Setup(rewardEquipment, null, null, "OnNewEquipmentCardSelected", showOwnerAndUpgradeIndicator: false);
				}
			}
			else if (IsAvatarAvailable() && avatarCardPrefab != null)
			{
				AvatarListCard component3 = Helpers.InstantiateToParentAndLayer(avatarCardPrefab, cardContainer).GetComponent<AvatarListCard>();
				if (component3 != null)
				{
					if (reward.IconIndex >= 0)
					{
						component3.UpdateAvatar(GameManager.Instance.gameEconomyData?.GetAvatarsDefinition(reward.IconIndex), isForceHideLockIcon: true);
					}
					else if (reward.BorderIndex >= 0)
					{
						component3.UpdateAvatar(GameManager.Instance.gameEconomyData?.GetBordersDefinition(reward.BorderIndex), isForceHideLockIcon: true);
					}
					else if (reward.ColorIndex >= 0)
					{
						component3.UpdateAvatar(GameManager.Instance.gameEconomyData?.GetAvatarColorsDefinition(reward.ColorIndex), isForceHideLockIcon: true);
					}
				}
			}
			else if (scrappedLabel != null)
			{
				scrappedLabel.gameObject.SetActive(value: true);
			}
		}
		if (!string.IsNullOrEmpty(reward.ModifiedByTrait) && Helpers.GameObjectSetActive(boostedRewardParent, value: true))
		{
			TweenManager.PlayTweenGroup(boostedRewardParent, boosterTweenGroup);
		}
		border.spriteName = "Ui_Border_4pt_" + Enum.GetName(typeof(DropType), reward.DropType);
		ShowFlyingCurrencies = true;
		if (!(doubleXpContainer != null))
		{
			return;
		}
		doubleXpContainer.SetActive(flag);
		survivalDoubleRewardContainer.SetActive(flag2);
		if (!(flag || flag2))
		{
			return;
		}
		AnimateNumberFromTo component4 = GetComponent<AnimateNumberFromTo>();
		if (!(component4 != null))
		{
			return;
		}
		component4.SetIgnoreTimeScale(ignoreTimeScale: true);
		int num = rewardLootEntry.RewardedAmount / (2 + ((flag && flag2) ? 2 : 0));
		int num2 = ((flag2 && flag) ? (rewardLootEntry.RewardedAmount / 2) : rewardLootEntry.RewardedAmount);
		component4.Animate(num, num2);
		if (!(flag2 && flag))
		{
			return;
		}
		UITweener[] componentsInChildren = doubleXpContainer.GetComponentsInChildren<UITweener>(includeInactive: false);
		float num3 = 0f;
		UITweener uITweener = componentsInChildren[0];
		UITweener[] array = componentsInChildren;
		foreach (UITweener uITweener2 in array)
		{
			if (uITweener2.duration + uITweener2.delay > num3)
			{
				uITweener = uITweener2;
				num3 = uITweener2.duration + uITweener2.delay;
			}
		}
		uITweener.SetOnFinished(new EventDelegate(delegate
		{
			doubleXpContainer.SetActive(value: false);
		}));
		array = survivalDoubleRewardContainer.GetComponentsInChildren<UITweener>(includeInactive: false);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].delay += num3;
		}
		component4.AddDelayToStart(num3);
		component4.Animate(num2, num2 * 2);
	}

	public void OnEquipmentCardClicked()
	{
		EquipmentItemModel model = rewardEquipment;
		if (IsEquipmentAvailable())
		{
			EquipmentUpgradePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
			obj.ShowNextLevel = false;
			obj.OpenForModel(model);
			obj.ShowEquipmentReceivedVersion();
			CampHUD.Get().PauseCurrencyMeters = false;
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
		}
	}

	public void OnClick()
	{
		if (IsEquipmentAvailable())
		{
			OnEquipmentCardClicked();
		}
	}

	public void RevealAnimationOver()
	{
		if (showEffects && rewardLootEntry.RewardedCurrency == CurrencyType.None && rewardLootEntry.DropCurrencyType != DropCurrenciesProbabilitiesDefinition.DropCurrency.Consumable && rewardLootEntry.DropCurrencyType != DropCurrenciesProbabilitiesDefinition.DropCurrency.Avatars)
		{
			LootRarityEffect(rewardLootEntry.RewardedRarityLevel);
		}
		if (!ShowFlyingCurrencies || (rewardLootEntry.RewardedCurrency != CurrencyType.Inhabitants && rewardLootEntry.RewardedCurrency != CurrencyType.Supplies && (rewardLootEntry.RewardedCurrency != CurrencyType.Phone || GameManager.Instance.playerModel.Combat != null) && rewardLootEntry.RewardedCurrency != CurrencyType.SurvivalPoints && rewardLootEntry.RewardedCurrency != CurrencyType.Diamonds && rewardLootEntry.RewardedCurrency != CurrencyType.ReplayToken && rewardLootEntry.RewardedCurrency != CurrencyType.Outpost && rewardLootEntry.RewardedCurrency != CurrencyType.CampaignToken && rewardLootEntry.RewardedCurrency != CurrencyType.GvGGas && rewardLootEntry.RewardedCurrency != CurrencyType.TraitRerollToken && rewardLootEntry.RewardedCurrency != CurrencyType.EquipmentUpgradeToken && rewardLootEntry.RewardedCurrency != CurrencyType.Fairmoney && rewardLootEntry.RewardedCurrency != CurrencyType.ApocalypticEquipToken))
		{
			return;
		}
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		if (campHUD != null)
		{
			campHUD.AddToMeter(rewardLootEntry.RewardedCurrency, rewardLootEntry.RewardedAmount);
			if (showEffects)
			{
				campHUD.GetComponent<BuildingsHUD>().CreateCollectAnim(rewardLootEntry.RewardedCurrency, base.gameObject, rewardLootEntry.RewardedAmount, OnAnimationComplete, BuildingsHUD.CollectSoundTrigger.OnStart, base.gameObject);
			}
		}
	}

	private void OnAnimationComplete(bool iscomplete, CurrencyType currencytype)
	{
		UpdateCampHudPhoneNumber();
	}

	private void UpdateCampHudPhoneNumber()
	{
		if (rewardLootEntry != null && rewardLootEntry.RewardedCurrency == CurrencyType.Phone)
		{
			CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.CampCampMapHud) as CampHUD;
			if (campHUD != null)
			{
				campHUD.SetPhonesNumber();
			}
		}
	}

	public void LootRarityEffect(int rarityLevel)
	{
		GameObject gameObject = null;
		if (rarityLevel < 5)
		{
			switch (rarityLevel)
			{
			case 0:
				gameObject = Helpers.InstantiateToParent(commonRarityEffect, base.gameObject);
				break;
			case 1:
				gameObject = Helpers.InstantiateToParent(uncommonRarityEffect, base.gameObject);
				break;
			case 2:
				gameObject = Helpers.InstantiateToParent(rareRarityEffect, base.gameObject);
				break;
			case 3:
				gameObject = Helpers.InstantiateToParent(epicRarityEffect, base.gameObject);
				break;
			case 4:
				gameObject = Helpers.InstantiateToParent(legendaryRarityEffect, base.gameObject);
				break;
			default:
				Debug.LogWarning("RewardLoot Currency Type was None, but RewardLoot Rarity was not set");
				break;
			}
		}
		else
		{
			gameObject = Helpers.InstantiateToParent(legendaryRarityEffect, base.gameObject);
		}
		if (gameObject != null)
		{
			int layer = base.gameObject.layer;
			Transform[] componentsInChildren = base.gameObject.GetComponentsInChildren<Transform>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].gameObject.layer = layer;
			}
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_shimmer_1");
			}
		}
	}
}
