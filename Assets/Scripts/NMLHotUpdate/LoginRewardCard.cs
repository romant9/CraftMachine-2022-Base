using BaseModel;
using TWDModel;
using UnityEngine;

public class LoginRewardCard : MonoBehaviour
{
	[SerializeField]
	private UISprite background;

	[SerializeField]
	private UISprite border;

	[SerializeField]
	private UILabel dayLabel;

	[SerializeField]
	private UILabel rewardName;

	[SerializeField]
	private GameObject claimedGameobject;

	[SerializeField]
	private UITexture armorTexture;

	[SerializeField]
	private UITexture weaponTexture;

	[SerializeField]
	private UITexture consumableTexture;

	[SerializeField]
	private UITexture heroTexture;

	[SerializeField]
	private UISprite classIcon;

	[SerializeField]
	private UISprite currencySprite;

	[SerializeField]
	private UIButtonExtended button;

	private LoginRewardsVisualConfig loginRewardsVisualConfig;

	[SerializeField]
	private GameObject selectedGameObject;

	[SerializeField]
	private UIAtlas shopAtlas;

	private bool hasPlayedClaimedTween;

	private bool hasPlayedActiveDayTween;

	public DailyLoginCampaignRewardModelItem Item { get; set; }

	private void OnEnable()
	{
		if (loginRewardsVisualConfig == null)
		{
			loginRewardsVisualConfig = UnityUtils.LoadFromAssetBundle<LoginRewardsVisualConfig>("LoginRewardsVisualConfig", "scriptableobjects");
		}
	}

	public void UpdateUI(int day, int activeDay)
	{
		if (Item != null)
		{
			button.Clear();
			HelpersUI.SetContentToLabel(dayLabel, LocalizationManager.GetText("GvG.Hub.Calendar.SelectedDay{day}", day));
			HelpersUI.SetContentToLabel(rewardName, HelpersLocalization.GetBundleTitleForIReward(Item.Reward));
			Helpers.GameObjectSetActive(rewardName, !Item.Claimed);
			Helpers.GameObjectSetActive(claimedGameobject, Item.Claimed);
			Helpers.GameObjectSetActive(classIcon, value: false);
			Helpers.GameObjectSetActive(selectedGameObject, day == activeDay && !Item.Claimed);
			if (Item.Claimed && !hasPlayedClaimedTween)
			{
				TweenManager.PlayTweenGroup(base.gameObject, 4);
				hasPlayedClaimedTween = true;
			}
			else if (!Item.Claimed && day == activeDay && !hasPlayedActiveDayTween)
			{
				TweenManager.PlayTweenGroup(base.gameObject, 3);
				hasPlayedActiveDayTween = true;
			}
			Setup();
		}
	}

	private void Setup()
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool value = false;
		IReward reward = Item.Reward;
		RewardEquipment rewardEquipment = reward as RewardEquipment;
		if (rewardEquipment == null)
		{
			if (!(reward is RewardCurrency rewardCurrency))
			{
				if (reward is RewardTimedBonus rewardTimedBonus)
				{
					button.SetClickCallback(delegate
					{
						TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForIReward(Item.Reward));
					});
					value = true;
					currencySprite.atlas = shopAtlas;
					currencySprite.spriteName = HelpersGfx.GetRewardTimedBonusIcon(rewardTimedBonus);
					background.color = loginRewardsVisualConfig?.Currency.BackgroundColor ?? Color.white;
					border.color = loginRewardsVisualConfig?.Currency.BorderColor ?? Color.white;
				}
			}
			else
			{
				flag4 = GameManager.Instance.gameEconomyData.IsHeroToken(rewardCurrency.CurrencyType);
				value = !flag4;
				if (flag4)
				{
					ActorDefinition actor = GameManager.Instance.gameEconomyData.GetActorDefinitionForToken(rewardCurrency.CurrencyType);
					HelpersGfx.SetSeasonHeroMaterial(heroTexture, HelpersGfx.GetSeasonArtForHero(actor?.ID));
					if (actor.TokensToUnlock <= rewardCurrency.Amount && !GameManager.Instance.playerModel.SurvivorContainer.HasHero(actor.ID) && !Item.Claimed)
					{
						HelpersUI.SetContentToLabel(rewardName, SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.LoginReward.Unlock{HeroName}", actor.Name));
					}
					HelpersUI.SetSprite(classIcon, HelpersGfx.GetSurvivorClassSmallIconName(actor.Class));
					Helpers.GameObjectSetActive(classIcon, value: true);
					button.SetClickCallback(delegate
					{
						ShowHeroPreview(actor);
					});
				}
				else
				{
					HelpersGfx.GetIconNameForIReward(Item.Reward, out var spriteName, null, null, null);
					HelpersUI.SetSprite(currencySprite, spriteName);
					button.SetClickCallback(delegate
					{
						TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForIReward(Item.Reward));
					});
				}
				background.color = ((!flag4) ? (loginRewardsVisualConfig?.Currency.BackgroundColor ?? Color.white) : (loginRewardsVisualConfig?.Hero.BackgroundColor ?? Color.white));
				border.color = ((!flag4) ? (loginRewardsVisualConfig?.Currency.BorderColor ?? Color.white) : (loginRewardsVisualConfig?.Hero.BorderColor ?? Color.white));
			}
		}
		else
		{
			EquipmentDefinition equipmentDefinition = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(rewardEquipment.EquipmentId);
			flag3 = equipmentDefinition.Category == EquipmentCategory.Utility;
			flag = !flag3 && equipmentDefinition.Type == EquipmentType.Armor;
			flag2 = !flag3 && !flag;
			if (flag)
			{
				armorTexture.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipmentDefinition);
			}
			else if (flag2)
			{
				weaponTexture.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipmentDefinition);
			}
			else
			{
				consumableTexture.mainTexture = HelpersGfx.GetEquipmentIconTexture(equipmentDefinition);
			}
			if (flag3)
			{
				if ((bool)loginRewardsVisualConfig)
				{
					background.color = loginRewardsVisualConfig.Currency.BackgroundColor;
					border.color = loginRewardsVisualConfig.Currency.BorderColor;
				}
				else
				{
					background.color = Color.white;
					border.color = Color.white;
				}
				button.SetClickCallback(delegate
				{
					TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForIReward(Item.Reward));
				});
				Helpers.GameObjectSetActive(classIcon, value: false);
			}
			else
			{
				RarityVisualizationEntry rarityVisualizationEntry = loginRewardsVisualConfig?.GetRarityVisualization(rewardEquipment.RarityLevel);
				background.color = rarityVisualizationEntry?.BackgroundColor ?? Color.white;
				border.color = rarityVisualizationEntry?.BorderColor ?? Color.white;
				button.SetClickCallback(delegate
				{
					OpenForReward(rewardEquipment);
				});
				HelpersUI.SetSprite(classIcon, HelpersGfx.GetSurvivorClassSmallIconName(equipmentDefinition.SurvivorClass));
				Helpers.GameObjectSetActive(classIcon, value: true);
			}
		}
		Helpers.GameObjectSetActive(armorTexture, flag);
		Helpers.GameObjectSetActive(weaponTexture, flag2);
		Helpers.GameObjectSetActive(heroTexture, flag4);
		Helpers.GameObjectSetActive(currencySprite, value);
		Helpers.GameObjectSetActive(consumableTexture, flag3);
	}

	private void OpenForReward(RewardEquipment rewardEquipment)
	{
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		EquipmentUpgradePopup obj = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampEquipmentLevelUpPopup) as EquipmentUpgradePopup;
		obj.ShowNextLevel = false;
		obj.OpenForBundleReward(rewardEquipment);
		CampHUD.Get().PauseCurrencyMeters = false;
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/equipment_click");
	}

	private void ShowHeroPreview(ActorDefinition actorDefinition)
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
		else
		{
			TooltipManager.OpenTextBoxWithText(base.gameObject, HelpersLocalization.GetShopTooltipForIReward(Item.Reward));
		}
	}
}
