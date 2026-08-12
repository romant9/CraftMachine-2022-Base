using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class AnimateIapRewardsNew : MonoBehaviour
{
	public enum PanelType
	{
		Generic = 0,
		Equipment = 1,
		Outfit = 2,
		HeroSkin = 3,
		EquipmentToken = 4,
		Skill = 5
	}

	[Tooltip("For the code to know what type of rewards should be shown in the panel prefab")]
	[SerializeField]
	private PanelType CurrentPanelType;

	[Tooltip("Sprite to show what currency.")]
	[SerializeField]
	private UISprite CurrencySprite;

	[Tooltip("Sprite to show outfits")]
	[SerializeField]
	private UISprite OutfitSprite;

	[Tooltip("Used to show amount or other info about currency rewards.")]
	[SerializeField]
	private UILabel CurrencyLabel;

	[Tooltip("Texture to thow the consumable reward")]
	[SerializeField]
	private UITexture consumableTexture;

	[Tooltip("Texture to thow the sevenday reward")]
	[SerializeField]
	private UITexture centerTexture;

	[SerializeField]
	[Tooltip("Texture to thow the avatar reward")]
	private UITexture avatarTexture;

	[SerializeField]
	private UISprite avatarIconSprite;

	[SerializeField]
	private UISprite avatarIconEffect;

	[Tooltip("Texture to thow the border reward")]
	[SerializeField]
	private UITexture borderTexture;

	[SerializeField]
	private UISprite borderIconSprite;

	[SerializeField]
	private UISprite borderIconEffect;

	[Tooltip("Special case for equipments rewards")]
	[SerializeField]
	private EquipmentButton equipmentButton;

	[SerializeField]
	[Tooltip("Special case for equipments token rewards")]
	private EquipmentTokenButton equipmentTokenButton;

	[SerializeField]
	private GameObject skillParent;

	[SerializeField]
	private UISprite skillIcon;

	[SerializeField]
	private UISprite skillBgIcon;

	[SerializeField]
	private UISprite skillClassIcon;

	[SerializeField]
	private UIGrid starGrid;

	[SerializeField]
	private GameObject starEntryPrefab;

	[SerializeField]
	private GameObject skillCurrencyParent;

	[SerializeField]
	private UISprite skillCurrencyBg;

	[SerializeField]
	private UISprite skillCurrencyIcon;

	[SerializeField]
	private UILabel skillCurrencyLabel;

	[SerializeField]
	private UIAtlas monochromeAtlas;

	[SerializeField]
	private UIAtlas shopAtlas;

	[SerializeField]
	private UIAtlas campAtlas;

	[SerializeField]
	private float firstItemAnimationDelay = 1f;

	private List<RewardEntryViewData> RewardsList = new List<RewardEntryViewData>();

	private Animator animator;

	private Callback CompletedCallback;

	private bool IsLastPanel;

	private readonly List<GameObject> _starEntries = new List<GameObject>();

	private void Start()
	{
		animator = GetComponent<Animator>();
	}

	public int GetRewardsListCount()
	{
		if (RewardsList != null)
		{
			return RewardsList.Count;
		}
		return 0;
	}

	public PanelType GetPanelType()
	{
		return CurrentPanelType;
	}

	public void StartPlaying(Callback completedCallback = null, bool lastPanel = false, bool firstPanel = false)
	{
		IsLastPanel = lastPanel;
		CompletedCallback = completedCallback;
		if (firstPanel)
		{
			Invoke("AnimateNextResources", firstItemAnimationDelay);
		}
		else
		{
			AnimateNextResources();
		}
	}

	public void Hide()
	{
		CompletedCallback = null;
		base.gameObject.SetActive(value: false);
	}

	public void AddReward(IReward reward, EquipmentItemModel equipment = null, OutfitDefinition outfit = null, HeroSkinDefinition heroSkin = null, EquipTokenItemModel equipTokenItemModel = null)
	{
		RewardEntryViewData rewardEntryViewData = new RewardEntryViewData();
		rewardEntryViewData.Reward = reward;
		rewardEntryViewData.Equipment = equipment;
		rewardEntryViewData.EquipToken = equipTokenItemModel;
		rewardEntryViewData.Outfit = outfit;
		rewardEntryViewData.HeroSkin = heroSkin;
		RewardsList.Add(rewardEntryViewData);
	}

	public void ShowAnimationDone()
	{
		if (IsLastToBeLeftVisible())
		{
			CallPanelComplete();
		}
		else
		{
			SetAnimatorParam(show: false);
		}
	}

	public void HideAnimationDone()
	{
		if (RewardsList.Count > 0)
		{
			RewardsList.RemoveAt(0);
		}
		if (RewardsList.Count == 0)
		{
			CallPanelComplete();
		}
		else
		{
			AnimateNextResources();
		}
	}

	public void AnimateNextResources()
	{
		base.gameObject.SetActive(value: true);
		if (equipmentButton != null && equipmentButton.gameObject.activeSelf)
		{
			equipmentButton.gameObject.SetActive(value: false);
		}
		if (equipmentTokenButton != null && equipmentTokenButton.gameObject.activeSelf)
		{
			equipmentTokenButton.gameObject.SetActive(value: false);
		}
		if (RewardsList == null || RewardsList.Count <= 0 || RewardsList[0] == null || RewardsList[0].Reward == null)
		{
			return;
		}
		if (RewardsList[0].Equipment != null)
		{
			SetupEquipmentToUI(RewardsList[0].Equipment);
		}
		else if (RewardsList[0].EquipToken != null)
		{
			SetupEquipmentTokenToUI(RewardsList[0].EquipToken);
		}
		else if (RewardsList[0].Outfit != null)
		{
			SetupOutfitToUI(RewardsList[0].Outfit);
		}
		else if (RewardsList[0].HeroSkin != null)
		{
			SetupHeroSkinToUI(RewardsList[0].HeroSkin);
		}
		else
		{
			IReward reward = RewardsList[0].Reward;
			if (centerTexture != null)
			{
				centerTexture.gameObject.SetActive(value: false);
			}
			Helpers.GameObjectSetActive(avatarTexture, value: false);
			Helpers.GameObjectSetActive(avatarIconSprite, value: false);
			Helpers.GameObjectSetActive(avatarIconEffect, value: false);
			Helpers.GameObjectSetActive(borderTexture, value: false);
			Helpers.GameObjectSetActive(borderIconSprite, value: false);
			Helpers.GameObjectSetActive(borderIconEffect, value: false);
			Helpers.GameObjectSetActive(skillParent, value: false);
			Helpers.GameObjectSetActive(skillCurrencyParent, value: false);
			if (reward != null && CurrencySprite != null && CurrencyLabel != null)
			{
				if (reward.Type == RewardType.Equipment && reward is RewardEquipment rewardEquipment && rewardEquipment.IsConsumableReward(GameManager.Instance.modelManager))
				{
					consumableTexture.mainTexture = HelpersGfx.GetTextureForEquipmentReward(rewardEquipment);
					consumableTexture.gameObject.SetActive(value: true);
					CurrencyLabel.text = rewardEquipment.Amount.ToString();
					CurrencyLabel.gameObject.SetActive(value: true);
					CurrencySprite.gameObject.SetActive(value: false);
				}
				else if (reward.Type == RewardType.Avatars)
				{
					if (reward is RewardAvatars rewardAvatars)
					{
						CurrencyLabel.text = "";
						CurrencyLabel.gameObject.SetActive(value: true);
						CurrencySprite.gameObject.SetActive(value: false);
						consumableTexture.gameObject.SetActive(value: false);
						if (rewardAvatars.Avatar >= 0 && avatarTexture != null)
						{
							AvatarsDefinition avatarsDefinition = GameManager.Instance.gameEconomyData.GetAvatarsDefinition(rewardAvatars.Avatar);
							if (avatarsDefinition != null)
							{
								if (!string.IsNullOrEmpty(avatarsDefinition.LocalImg))
								{
									HelpersUI.SetSprite(avatarIconSprite, avatarsDefinition.LocalImg);
									Helpers.GameObjectSetActive(avatarIconSprite, value: true);
									Helpers.GameObjectSetActive(avatarIconEffect, avatarsDefinition.LocalEffectType > 0);
									LoadImageFromCdn.LoadImageToTarget(avatarTexture, "", clearLocalCachedUrls: false, 10);
								}
								else
								{
									LoadImageFromCdn.LoadImageToTarget(avatarTexture, avatarsDefinition?.Image, clearLocalCachedUrls: false, 10);
								}
							}
						}
						else if (rewardAvatars.Border >= 0 && borderTexture != null)
						{
							BordersDefinition bordersDefinition = GameManager.Instance.gameEconomyData.GetBordersDefinition(rewardAvatars.Border);
							if (bordersDefinition != null)
							{
								if (!string.IsNullOrEmpty(bordersDefinition.LocalImg))
								{
									HelpersUI.SetSprite(borderIconSprite, bordersDefinition.LocalImg);
									Helpers.GameObjectSetActive(borderIconSprite, value: true);
									Helpers.GameObjectSetActive(borderIconEffect, bordersDefinition.LocalEffectType > 0);
									LoadImageFromCdn.LoadImageToTarget(borderTexture, "", clearLocalCachedUrls: false, 10);
								}
								else
								{
									LoadImageFromCdn.LoadImageToTarget(borderTexture, bordersDefinition?.Image, clearLocalCachedUrls: false, 10);
								}
							}
						}
					}
				}
				else if (reward.Type == RewardType.SevenDayPremium)
				{
					Object obj = UnityUtils.LoadFromAssetBundle("Ui_Icon_Sevenday_Pass", "itemgraphics");
					if (obj != null && centerTexture != null)
					{
						centerTexture.mainTexture = (Texture)obj;
						centerTexture.gameObject.SetActive(value: true);
					}
					CurrencyLabel.text = "";
					CurrencyLabel.gameObject.SetActive(value: true);
					CurrencySprite.gameObject.SetActive(value: false);
					consumableTexture.gameObject.SetActive(value: false);
				}
				else if (reward is RewardRemoldSkill rewardRemoldSkill)
				{
					SPTraitsRemoldDefinitions minRemoldDefinitionForGroup = Helpers.GetMinRemoldDefinitionForGroup(rewardRemoldSkill.SpRemoldSkillType);
					if (minRemoldDefinitionForGroup != null && skillParent != null && skillIcon != null && skillClassIcon != null && skillBgIcon != null && starGrid != null)
					{
						CurrencySprite.gameObject.SetActive(value: false);
						CurrencyLabel.gameObject.SetActive(value: false);
						consumableTexture?.gameObject.SetActive(value: false);
						Helpers.GameObjectSetActive(skillParent, value: true);
						HelpersUI.SetTraitsIconOnSprite(skillIcon, minRemoldDefinitionForGroup.SPTraitsIcon, minRemoldDefinitionForGroup.SPTraitsIconOnCloud);
						skillClassIcon.spriteName = HelpersGfx.GetSurvivorClassSmallIconName(minRemoldDefinitionForGroup.AvailableClass);
						skillBgIcon.color = Helpers.HexToColor(minRemoldDefinitionForGroup.Color);
						SetupStarGrid(minRemoldDefinitionForGroup.Star);
					}
				}
				else
				{
					CurrencySprite.atlas = monochromeAtlas;
					CurrencySprite.gameObject.SetActive(value: true);
					CurrencyLabel.gameObject.SetActive(value: true);
					consumableTexture?.gameObject.SetActive(value: false);
					if (reward.Type == RewardType.Currency && reward is RewardCurrency)
					{
						RewardCurrency rewardCurrency = reward as RewardCurrency;
						if (rewardCurrency.CurrencyType.ToString().Contains("SkillToken"))
						{
							SPTraitsSkillKitTokenSet sPTraitsSkillKitTokenSetByID = GameManager.Instance.playerModel.gameEconomyData.GetSPTraitsSkillKitTokenSetByID(rewardCurrency.CurrencyType.ToString());
							if (sPTraitsSkillKitTokenSetByID != null)
							{
								CurrencySprite.gameObject.SetActive(value: false);
								CurrencyLabel.gameObject.SetActive(value: false);
								Helpers.GameObjectSetActive(skillCurrencyParent, value: true);
								HelpersUI.SetTraitsIconOnSprite(skillCurrencyIcon, sPTraitsSkillKitTokenSetByID.TopIcon, sPTraitsSkillKitTokenSetByID.TopIconOnCloud);
								HelpersUI.SetSprite(skillCurrencyBg, sPTraitsSkillKitTokenSetByID.BGIcon);
								HelpersUI.SetContentToLabel(skillCurrencyLabel, HelpersGfx.GetAmountForIReward(rewardCurrency).ToString());
							}
						}
						else
						{
							CurrencySprite.spriteName = HelpersGfx.GetCurrencyIconName(rewardCurrency.CurrencyType);
							if (rewardCurrency.CurrencyType == CurrencyType.Diamonds)
							{
								CurrencyLabel.text = rewardCurrency.Amount.ToString();
							}
							else if (rewardCurrency.Amount == -1 && rewardCurrency.CurrencyType == CurrencyType.ReplayToken)
							{
								CurrencyLabel.text = LocalizationManager.GetText("Bundle.Replay.Full");
							}
							else if (rewardCurrency.Amount == -1 && rewardCurrency.CurrencyType == CurrencyType.Supplies)
							{
								CurrencyLabel.text = LocalizationManager.GetText("Bundle.Supplies.Full");
							}
							else if (rewardCurrency.Amount == -1 && rewardCurrency.CurrencyType == CurrencyType.SurvivalPoints)
							{
								CurrencyLabel.text = LocalizationManager.GetText("Bundle.RewardCurrencyTitle.Experiences.Full");
							}
							else if (ComponentHelper.IsComponentCurrency(rewardCurrency.CurrencyType))
							{
								CurrencyLabel.text = HelpersLocalization.GetComponentRewardName(rewardCurrency.CurrencyType, rewardCurrency.Amount);
							}
							else
							{
								CurrencyLabel.text = Helpers.FormatNumber(rewardCurrency.Amount);
							}
							if (!OfflineManager.IsLoadDataManager && BuildingsHUD.Get() != null)
							{
								BuildingsHUD.Get().CreateCollectAnim(rewardCurrency.CurrencyType, base.gameObject, rewardCurrency.Amount, null, BuildingsHUD.CollectSoundTrigger.OnStart, base.gameObject);
							}
						}
					}
					else if (reward.Type == RewardType.WeeklySubscription)
					{
						CurrencySprite.atlas = campAtlas;
						CurrencySprite.spriteName = "UI_Icon_Subscription_Sliver";
						CurrencyLabel.text = LocalizationManager.GetText("Subscription.Weekly.Reward.Type");
					}
					else if (reward.Type == RewardType.MonthlySubscription)
					{
						CurrencySprite.atlas = campAtlas;
						CurrencySprite.spriteName = "UI_Icon_Subscription_Gold";
						CurrencyLabel.text = LocalizationManager.GetText("Subscription.Monthly.Reward.Type");
					}
					else if (reward.Type == RewardType.ThreeDayPremium)
					{
						CurrencySprite.atlas = campAtlas;
						CurrencySprite.spriteName = "UI_Icon_ActiveThreeDay";
						CurrencyLabel.text = LocalizationManager.GetText("ThreeDay.Premium.Reward.Type");
					}
					else if (reward.Type == RewardType.SurvivorSlot && reward is RewardSurvivorSlot)
					{
						RewardSurvivorSlot rewardSurvivorSlot = reward as RewardSurvivorSlot;
						CurrencySprite.gameObject.SetActive(value: false);
						CurrencyLabel.text = LocalizationManager.GetText("Bundle.Slots.Description{Parameter}", rewardSurvivorSlot.Amount);
					}
					else if (reward.Type == RewardType.SurvivorClass && reward is RewardSurvivorClass)
					{
						RewardSurvivorClass rewardSurvivorClass = reward as RewardSurvivorClass;
						CurrencySprite.gameObject.SetActive(value: false);
						CurrencyLabel.text = HelpersLocalization.GetSurvivorClassName(rewardSurvivorClass.SurvivorClass);
					}
					else if (reward.Type == RewardType.UnlockBuilding && reward is RewardUnlockBuilding)
					{
						RewardUnlockBuilding rewardUnlockBuilding = reward as RewardUnlockBuilding;
						CurrencySprite.gameObject.SetActive(value: false);
						CurrencyLabel.text = HelpersLocalization.GetBuildingName(rewardUnlockBuilding.BuildingTypeName);
					}
					else if (reward.Type == RewardType.TimedBonus && reward is RewardTimedBonus)
					{
						RewardTimedBonus rewardTimedBonus = reward as RewardTimedBonus;
						CurrencySprite.atlas = shopAtlas;
						CurrencySprite.spriteName = HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus);
						CurrencyLabel.text = HelpersLocalization.GetTimedBonusTitle(rewardTimedBonus.TimedBonusType, rewardTimedBonus.Duration);
						if (!OfflineManager.IsLoadDataManager)
						{
							if (rewardTimedBonus.TimedBonusType == TimedBonusType.UnlimitedGas)
							{
								if (BuildingsHUD.Get() != null)
								{
									BuildingsHUD.Get().CreateCollectAnim(CurrencyType.ReplayToken, base.gameObject, -1, null, BuildingsHUD.CollectSoundTrigger.OnStart, base.gameObject);
								}
							}
							else if (rewardTimedBonus.TimedBonusType == TimedBonusType.DoubleXp && BuildingsHUD.Get() != null)
							{
								BuildingsHUD.Get().CreateCollectAnim(CurrencyType.SurvivalPoints, base.gameObject, -1, null, BuildingsHUD.CollectSoundTrigger.OnStart, base.gameObject);
							}
						}
					}
				}
			}
		}
		SetAnimatorParam(show: true);
	}

	public void SkipCurrentBeingShown()
	{
		if (animator != null && animator.GetBool("Show") && !IsLastToBeLeftVisible())
		{
			HideAnimationDone();
		}
	}

	public bool IsLastToBeLeftVisible()
	{
		if (IsLastPanel)
		{
			return RewardsList.Count == 1;
		}
		return false;
	}

	private void SetupEquipmentToUI(EquipmentItemModel equipment)
	{
		if (equipmentButton != null)
		{
			if (!equipmentButton.gameObject.activeSelf)
			{
				equipmentButton.gameObject.SetActive(value: true);
			}
			equipmentButton.Setup(equipment, null, null, "OnNewEquipmentCardSelected", showOwnerAndUpgradeIndicator: false);
		}
	}

	private void SetupStarGrid(int count)
	{
		if (starGrid == null || starEntryPrefab == null)
		{
			return;
		}
		for (int i = 0; i < _starEntries.Count; i++)
		{
			if (_starEntries[i] != null)
			{
				NGUITools.Destroy(_starEntries[i]);
			}
		}
		_starEntries.Clear();
		for (int j = 0; j < count; j++)
		{
			GameObject gameObject = starGrid.gameObject.AddChild(starEntryPrefab);
			NGUITools.SetActive(gameObject, state: true);
			_starEntries.Add(gameObject);
		}
		starGrid.Reposition();
	}

	private void SetupEquipmentTokenToUI(EquipTokenItemModel equipment)
	{
		if (equipmentTokenButton != null)
		{
			if (!equipmentTokenButton.gameObject.activeSelf)
			{
				equipmentTokenButton.gameObject.SetActive(value: true);
			}
			equipmentTokenButton.SetUpForReward(equipment);
		}
	}

	private void SetupOutfitToUI(OutfitDefinition outfit)
	{
		if (equipmentButton != null && equipmentButton.gameObject.activeSelf)
		{
			equipmentButton.gameObject.SetActive(value: false);
		}
		if (equipmentTokenButton != null && equipmentTokenButton.gameObject.activeSelf)
		{
			equipmentTokenButton.gameObject.SetActive(value: false);
		}
		if (CurrencySprite != null)
		{
			CurrencySprite.gameObject.SetActive(value: false);
		}
		if (OutfitSprite != null)
		{
			OutfitSprite.gameObject.SetActive(value: true);
		}
		if (CurrencyLabel != null)
		{
			string text = LocalizationManager.GetText(outfit.TitleLocalizationKey);
			CurrencyLabel.text = LocalizationManager.GetText("Bundle.Outfit.Description{Parameter}", text);
		}
	}

	private void SetupHeroSkinToUI(HeroSkinDefinition heroSkin)
	{
		if (equipmentButton != null && equipmentButton.gameObject.activeSelf)
		{
			equipmentButton.gameObject.SetActive(value: false);
		}
		if (equipmentTokenButton != null && equipmentTokenButton.gameObject.activeSelf)
		{
			equipmentTokenButton.gameObject.SetActive(value: false);
		}
		if (CurrencySprite != null)
		{
			CurrencySprite.gameObject.SetActive(value: false);
		}
		if (OutfitSprite != null)
		{
			OutfitSprite.gameObject.SetActive(value: true);
		}
		if (CurrencyLabel != null)
		{
			CurrencyLabel.text = LocalizationManager.GetText(heroSkin.LocalizationKey);
		}
	}

	private void SetAnimatorParam(bool show)
	{
		if (animator != null)
		{
			animator.SetBool("Show", show);
		}
	}

	private void CallPanelComplete()
	{
		if (CompletedCallback != null)
		{
			CompletedCallback();
		}
	}

	public void ClearRewardsList()
	{
		RewardsList.Clear();
	}
}
