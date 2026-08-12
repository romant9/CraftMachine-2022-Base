using NextGames.Sdk.AssetBundleManager;
using TWDModel;
using UnityEngine;

public class CampaignRewardButton : MonoBehaviour
{
	[SerializeField]
	private UILabel rewardAmountLabel;

	[SerializeField]
	private UISprite rewardIcon;

	[SerializeField]
	private UISprite timedRewardIcon;

	[SerializeField]
	private UILabel controlAmountLabel;

	[SerializeField]
	private UISprite tokenIcon;

	[SerializeField]
	private UIButton claimButton;

	[SerializeField]
	private GameObject randomEquipmentButtonPrefab;

	[SerializeField]
	private GameObject equipmentButtonPrefab;

	[SerializeField]
	private GameObject equipmentTokenButtonPrefab;

	[SerializeField]
	public GameObject currencyParent;

	[SerializeField]
	public GameObject equipmentParent;

	[SerializeField]
	public GameObject equipmentTokenParent;

	[SerializeField]
	private GameObject consumableParent;

	[SerializeField]
	private GameObject timedRewardParent;

	[SerializeField]
	private UITexture consumableTexture;

	[SerializeField]
	private UILabel consumableAmount;

	[SerializeField]
	private GameObject claimedParent;

	[SerializeField]
	public UITexture rewardTexture;

	[SerializeField]
	private int TweenGroupForClaiming = 11;

	[SerializeField]
	private GameObject mainRewardContainer;

	public string CampaignTokenIcon;

	[SerializeField]
	public Vector3 equipmentCardScale = new Vector3(0.5f, 0.5f, 1f);

	[Header("Visual Config Objects")]
	[SerializeField]
	private UIButton rewardButton;

	[SerializeField]
	private UISprite[] backgroundSprites;

	[SerializeField]
	private UILabel currencyLabel;

	public CampaignRewardItem Item { get; set; }

	public int Order { get; set; }

	public bool IsMainReward { get; set; }

	public void UpdateUI()
	{
		if (Item == null)
		{
			return;
		}
		HelpersUI.SetContentToLabel(rewardAmountLabel, HelpersGfx.GetAmountForIReward(Item.Reward).ToString());
		HelpersUI.SetContentToLabel(controlAmountLabel, Item.Control.ToString());
		Helpers.GameObjectSetActive(claimedParent, Item.Claimed);
		if (Item.Claimed)
		{
			LoadAndSetRewardMaterial();
		}
		Helpers.GameObjectSetActive(mainRewardContainer, !Item.Claimed && IsMainReward);
		Helpers.GameObjectSetActive(claimButton, Item.Claimable);
		Helpers.GameObjectSetActive(currencyParent, !Item.Claimed);
		Helpers.GameObjectSetActive(equipmentParent, !Item.Claimed);
		Helpers.GameObjectSetActive(equipmentTokenParent, !Item.Claimed);
		Helpers.GameObjectSetActive(consumableParent, value: false);
		Helpers.GameObjectSetActive(currencyParent, Item.Reward is RewardCurrency);
		Helpers.GameObjectSetActive(timedRewardParent, Item.Reward is RewardTimedBonus);
		if (Item.Reward is RewardCurrency)
		{
			string spriteName = "";
			HelpersGfx.GetIconNameForIReward(Item.Reward, out spriteName, null, null, null);
			HelpersUI.SetSprite(rewardIcon, spriteName);
		}
		else if (Item.Reward is RewardTimedBonus rewardTimedBonus)
		{
			HelpersUI.SetSprite(timedRewardIcon, HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus));
			UIButtonExtended component = timedRewardParent.GetComponent<UIButtonExtended>();
			component.Clear();
			component.SetClickCallback(delegate
			{
				TooltipManager.OpenTextBoxWithText(timedRewardParent, HelpersLocalization.GetShopTooltipForIReward(Item.Reward));
			});
		}
		HelpersUI.SetSprite(tokenIcon, CampaignTokenIcon);
		CreateEquipmentCardsAndSetActive();
		UpdateVisuals();
	}

	private void CreateEquipmentCardsAndSetActive()
	{
		if (Item == null || (!OfflineManager.IsLoadDataManager && Item.Claimed))
		{
			return;
		}
		if (Item.Reward is RewardRandomEquipment && randomEquipmentButtonPrefab != null)
		{
			Helpers.GameObjectSetActive(equipmentParent, value: true);
			GameObject gameObject = Helpers.InstantiateToParent(randomEquipmentButtonPrefab, equipmentParent);
			if (gameObject != null)
			{
				gameObject.transform.localScale = equipmentCardScale;
				EquipmentRandomButton component = gameObject.GetComponent<EquipmentRandomButton>();
				if (component != null)
				{
					component.Setup((RewardRandomEquipment)Item.Reward);
				}
			}
		}
		else if (Item.Reward is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
		{
			Helpers.GameObjectSetActive(consumableParent, value: true);
			consumableAmount.text = rewardEquipment.Amount.ToString();
			consumableTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
			UIButtonExtended component2 = consumableParent.GetComponent<UIButtonExtended>();
			component2.Clear();
			component2.SetClickCallback(delegate
			{
				TooltipManager.OpenTextBoxWithText(consumableParent, HelpersLocalization.GetShopTooltipForIReward(Item.Reward));
			});
		}
		else if (Item.Reward is RewardEquipment && equipmentButtonPrefab != null)
		{
			Helpers.GameObjectSetActive(equipmentParent, value: true);
			GameObject gameObject2 = Helpers.InstantiateToParent(equipmentButtonPrefab, equipmentParent);
			if (gameObject2 != null)
			{
				gameObject2.transform.localScale = equipmentCardScale;
				EquipmentButton component3 = gameObject2.GetComponent<EquipmentButton>();
				if (component3 != null)
				{
					component3.Setup((RewardEquipment)Item.Reward);
				}
			}
		}
		else
		{
			if (!(Item.Reward is RewardEquipToken) || !(equipmentTokenButtonPrefab != null))
			{
				return;
			}
			Helpers.GameObjectSetActive(equipmentTokenParent, value: true);
			GameObject gameObject3 = Helpers.InstantiateToParent(equipmentTokenButtonPrefab, equipmentTokenParent);
			if (gameObject3 != null)
			{
				gameObject3.transform.localScale = equipmentCardScale;
				EquipmentTokenButton component4 = gameObject3.GetComponent<EquipmentTokenButton>();
				if (component4 != null)
				{
					component4.SetUpForCampaign((RewardEquipToken)Item.Reward);
				}
			}
		}
	}

	public void SetClaimable()
	{
		Helpers.GameObjectSetActive(claimButton, value: true);
	}

	public void OnClaimRewardClick()
	{
		if (!Item.Claimable)
		{
			return;
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/reward_claim");
		if (!(Item is CampaignRewardModelItem))
		{
			return;
		}
		CampaignRewardModelItem campaignRewardModelItem = (CampaignRewardModelItem)Item;
		if (Helpers.ExecuteCommand(new ClaimCampaignRewardCommand(campaignRewardModelItem.ModelId)) != TWDModelResult.OK)
		{
			return;
		}
		BuildingsHUD buildingsHUD = BuildingsHUD.Get();
		if (buildingsHUD != null)
		{
			IReward reward = Item.Reward;
			if (reward != null)
			{
				if (reward is RewardCurrency)
				{
					RewardCurrency rewardCurrency = (RewardCurrency)reward;
					buildingsHUD.CreateCollectAnim(rewardCurrency.CurrencyType, currencyParent, rewardCurrency.Amount);
				}
				else if (reward is RewardTimedBonus rewardTimedBonus)
				{
					ShowTimedReward(rewardTimedBonus);
				}
				else if (reward is RewardRandomEquipment || reward is RewardEquipment)
				{
					if (reward is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
					{
						ShowConsumableReward(rewardEquipment);
					}
					else
					{
						ShowEquipmentRewards(campaignRewardModelItem.LastRewardedEquipment);
					}
				}
				else if (reward is RewardEquipToken)
				{
					ShowEquipmentRewardsToken(campaignRewardModelItem.LastRewardedEquipmentToken);
				}
			}
		}
		LoadAndSetRewardMaterial();
		Helpers.GameObjectSetActive(claimedParent, value: true);
		TweenManager.PlayTweenGroup(base.gameObject, TweenGroupForClaiming);
		Helpers.GameObjectSetActive(claimButton, value: false);
		Helpers.GameObjectSetActive(currencyParent, value: false);
		Helpers.GameObjectSetActive(equipmentParent, value: false);
		Helpers.GameObjectSetActive(equipmentTokenParent, value: false);
	}

	private void ShowEquipmentRewards(EquipmentItemModel lastRewardedEquipment)
	{
		if (lastRewardedEquipment != null)
		{
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew).OpenForEquipment(lastRewardedEquipment, "Popup.IAPConfirm.Title.GenericReward");
		}
	}

	private void ShowEquipmentRewardsToken(EquipTokenItemModel equipTokenItemModel)
	{
		if (equipTokenItemModel != null)
		{
			(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew).OpenForEquipmentToken(equipTokenItemModel, "Popup.IAPConfirm.Title.GenericReward");
		}
	}

	private void ShowConsumableReward(RewardEquipment consumable)
	{
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew).OpenForConsumable(consumable, "Popup.IAPConfirm.Title.GenericReward");
	}

	private void ShowTimedReward(RewardTimedBonus rewardTimedBonus)
	{
		(SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew).OpenForTimedReward(rewardTimedBonus, "Popup.IAPConfirm.Title.GenericReward");
	}

	private void LoadAndSetRewardMaterial()
	{
		if (rewardTexture != null)
		{
			Material material = AssetBundleManager.Instance.LoadAsset<Material>("ui_texture_campaignreward_" + Order, "uimaterials");
			if (material != null)
			{
				rewardTexture.material = material;
			}
		}
	}

	private void UpdateVisuals()
	{
		CampaignVisualConfig campaignVisualConfig = SingularityMonoBehaviour<HUDManager>.Instance.CampaignVisualConfig;
		UISprite[] array = backgroundSprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = campaignVisualConfig.rewardBgColor;
		}
		rewardButton.defaultColor = campaignVisualConfig.rewardBgColor;
		rewardButton.hover = campaignVisualConfig.rewardBgColor;
		rewardButton.pressed = campaignVisualConfig.rewardBgColor;
		rewardButton.disabledColor = campaignVisualConfig.rewardBgColor;
		rewardButton.UpdateColor(instant: true);
		currencyLabel.color = campaignVisualConfig.currencyTextColor;
		currencyLabel.effectColor = campaignVisualConfig.currencyTextShadowColor;
	}
}
