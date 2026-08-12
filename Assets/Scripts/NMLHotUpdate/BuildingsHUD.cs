using System.Collections.Generic;
using BaseModel;
using TWDModel;
using UnityEngine;

public class BuildingsHUD : MonoBehaviour
{
	public enum CollectSoundTrigger
	{
		OnStart = 0,
		OnFinished = 1
	}

	[Tooltip("Effect shows when the building upgrade has been completed")]
	public GameObject upgradeCompleteEffect;

	[Tooltip("Effect shows when the vegetation has been cut")]
	public GameObject effectVegetationDisappear;

	[Tooltip("Effect shows when the vegetation has been cut")]
	public GameObject effectBigVegetationDisappear;

	[Space(5f)]
	[SerializeField]
	[Tooltip("The GameObject that will holds all the hud GameObject.")]
	private GameObject buildingParent;

	[SerializeField]
	[Tooltip("The GameObject that will holds all the hud collect animations.")]
	private GameObject collectParent;

	[SerializeField]
	[Tooltip("The upgrade indicator timer")]
	[Space(5f)]
	private GameObject upgradeIndicatorPrefab;

	[Space(5f)]
	[Tooltip("The survivor upgrade indicator timer")]
	[SerializeField]
	private GameObject survivorUpgradeIndicatorPrefab;

	[SerializeField]
	[Tooltip("The survivor upgrade done indicator prefab")]
	[Space(5f)]
	private GameObject survivorUpgradeDoneIndicatorPrefab;

	[SerializeField]
	[Tooltip("The radio tent survivor search indicator timer")]
	[Space(5f)]
	private GameObject searchSurvivorIndicator;

	[Tooltip("How many seconds producers need to collect before presenting collect indicator.")]
	[Space(5f)]
	public float collectThresholdTime;

	[SerializeField]
	[Tooltip("Collect currency indicators. Icon shown on top of the building")]
	private GameObject[] collectIndicators;

	[Space(5f)]
	[SerializeField]
	[Tooltip("Collect currency animation. This includes the currency flying to the hud and the floating text with the amount collected ")]
	private GameObject[] collectAnimations;

	[SerializeField]
	private GameObject tokenCollectAnimation;

	[SerializeField]
	private GameObject fairMoneyCollectAnimation;

	[SerializeField]
	private GameObject bluePrintCollectAnimation;

	[SerializeField]
	private GameObject hillTopCoinCollectAnimation;

	[SerializeField]
	private GameObject goldRadioCollectAnimation;

	[SerializeField]
	private GameObject componentCollectAnimation;

	[SerializeField]
	private GameObject currencyToSuppliersCollectAnimation;

	[Space(5f)]
	[SerializeField]
	[Tooltip("Xp received animation. This includes the currency flying to the hud and the floating text with the amount collected ")]
	private GameObject bpReceivedAnimation;

	[Tooltip("Quest Point received animation. This includes the currency flying to the hud and the floating text with the amount collected ")]
	[Space(5f)]
	[SerializeField]
	private GameObject questPointAnimation;

	[Space(5f)]
	[SerializeField]
	[Tooltip("NewBie Point received animation. This includes the currency flying to the hud and the floating text with the amount collected ")]
	private GameObject newBiePointAnimation;

	[Space(5f)]
	[SerializeField]
	[Tooltip("Stars received animation. This includes the currency flying to the hud and the floating text with the amount collected ")]
	private GameObject starsReceivedAnimation;

	[Space(5f)]
	[SerializeField]
	[Tooltip("spTraitsUpgradeTokens received animation. This includes the currency flying to the hud and the floating text with the amount collected ")]
	private GameObject spTraitsUpgradeTokensReceivedAnimation;

	[Space(5f)]
	[SerializeField]
	[Tooltip("Collect currency effect. Small currencies flying.")]
	private GameObject[] collectEffects;

	[SerializeField]
	[Tooltip("Indicator to show that a player has the resources to upgrade a building.")]
	[Space(5f)]
	private GameObject buildingUpgradeAvailableIndicator;

	[SerializeField]
	[Tooltip("Indicator to show that a player has the resources to upgrade something inside a building.")]
	private GameObject buildingUpgradeInsideAvailableIndicator;

	[SerializeField]
	[Tooltip("Indicator to show the timer left for the free radio call.")]
	private GameObject freeRadioCallIndicator;

	[SerializeField]
	[Tooltip("锚定在扩地植被上方的绿色\"扩建\"按钮 (Council 30 解锁的扩地)。")]
	private GameObject expansionButtonIndicatorPrefab;

	private Dictionary<int, ExpansionButtonIndicator> activeExpansionIndicators = new Dictionary<int, ExpansionButtonIndicator>();

	public static BuildingsHUD Get()
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		if (campHUD != null)
		{
			return campHUD.GetComponent<BuildingsHUD>();
		}
		return null;
	}

	public BuildingUpgradeIndicator CreateUpgradeIndicator(BuildingView building)
	{
		BuildingUpgradeIndicator obj = CreateBuildingIndicator(upgradeIndicatorPrefab, building, followBasicIndicatorPosition: false) as BuildingUpgradeIndicator;
		obj.FollowTarget(building.BuildingGameObject);
		return obj;
	}

	public SurvivorUpgradeIndicator CreateSurvivorUpgradeIndicator(BuildingView building)
	{
		SurvivorUpgradeIndicator obj = CreateBuildingIndicator(survivorUpgradeIndicatorPrefab, building, followBasicIndicatorPosition: false) as SurvivorUpgradeIndicator;
		obj.FollowTarget(building.BuildingGameObject);
		return obj;
	}

	public SurvivorUpgradeDoneIndicator CreateSurvivorUpgradeDoneIndicator(BuildingView building)
	{
		SurvivorUpgradeDoneIndicator obj = CreateBuildingIndicator(survivorUpgradeDoneIndicatorPrefab, building, followBasicIndicatorPosition: false) as SurvivorUpgradeDoneIndicator;
		obj.FollowTarget(building.BuildingGameObject);
		return obj;
	}

	public CollectIndicator CreateCollectIndicator(BuildingView building)
	{
		if (building.Model.Producer.CurrencyType == CurrencyType.SPTraitsUpgradeToken)
		{
			return CreateBuildingIndicator(collectIndicators[11], building) as CollectIndicator;
		}
		return CreateBuildingIndicator(collectIndicators[(int)(building.Model.Producer.CurrencyType - 1)], building) as CollectIndicator;
	}

	protected BuildingIndicator CreateBuildingIndicator(GameObject prefab, BuildingView building, bool followBasicIndicatorPosition = true)
	{
		BuildingIndicator component = Helpers.InstantiateToParent(prefab, buildingParent).GetComponent<BuildingIndicator>();
		if (followBasicIndicatorPosition)
		{
			component.FollowBasicIndicatorPosition(building);
		}
		component.Building = building;
		return component;
	}

	public AdAvailableIndicator CreateAdAvailableIndicator(GameObject prefab)
	{
		return Helpers.InstantiateToParent(prefab, buildingParent).GetComponent<AdAvailableIndicator>();
	}

	public void CreateCollectAnim(Cashier cashier, GameObject startGameObject = null)
	{
		for (int i = 0; i < (int)CurrencyType.Count; i++)
		{
			int totalCost = cashier.GetTotalCost((CurrencyType)i);
			if (totalCost > 0)
			{
				CreateCollectAnim((CurrencyType)i, startGameObject, totalCost);
			}
		}
	}

	private GameObject GetCollectAnimationPrefab(CurrencyType currencyType)
	{
		if (GameManager.Instance.gameEconomyData.IsToken(currencyType) || currencyType == CurrencyType.EndlessPassToken || currencyType == CurrencyType.EndlessPassExpertToken || GameManager.Instance.gameEconomyData.IsSpeedUpTokenCurrencyType(currencyType) || currencyType == CurrencyType.BattlePassPoints || currencyType == CurrencyType.BounsItem || currencyType == CurrencyType.ApocalypticEquipToken || currencyType == CurrencyType.EquipTraitsRemodelToken || currencyType == CurrencyType.MTToken || currencyType == CurrencyType.EXToken || currencyType == CurrencyType.PrimarySupportTalentToken || currencyType == CurrencyType.AdvancedSupportTalentToken || currencyType == CurrencyType.SPTraitsUpgradeToken || currencyType == CurrencyType.SPTraitsRemoldToken)
		{
			return tokenCollectAnimation;
		}
		if (ComponentHelper.IsComponentCurrency(currencyType))
		{
			return componentCollectAnimation;
		}
		switch (currencyType)
		{
		case CurrencyType.CampaignToken:
		case CurrencyType.GuildBattleRP:
		case CurrencyType.EquipmentUpgradeToken:
		case CurrencyType.TraitRerollToken:
			return currencyToSuppliersCollectAnimation;
		case CurrencyType.GvGGas:
			return currencyToSuppliersCollectAnimation;
		case CurrencyType.FreeGuildGiftPerk:
			return tokenCollectAnimation;
		case CurrencyType.GoldRadio:
			return goldRadioCollectAnimation;
		case CurrencyType.Fairmoney:
			return fairMoneyCollectAnimation;
		case CurrencyType.HillTopCoin:
			return hillTopCoinCollectAnimation;
		case CurrencyType.BulePrintToken:
			return bluePrintCollectAnimation;
		case CurrencyType.SPTraitsUpgradeToken:
			return spTraitsUpgradeTokensReceivedAnimation;
		default:
		{
			int num = (int)(currencyType - 1);
			if (currencyType == CurrencyType.None || collectAnimations.Length <= num || collectAnimations[num] == null)
			{
				Debug.LogError("Could not find collectAnimation for currency: " + currencyType);
				return null;
			}
			return collectAnimations[num];
		}
		}
	}

	public CollectAnimation CreateCollectAnim(CurrencyType currencyType, GameObject gameObjectToFollow, int amount, AnimComplete animComplete = null, CollectSoundTrigger soundTrigger = CollectSoundTrigger.OnStart, GameObject container = null)
	{
		if ((SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD).GetCollectAnimationDestination(currencyType) == null)
		{
			return null;
		}
		int num = (PlatformInfo.HasFlag(PlatformFlag.SlowGPU) ? 4 : 8);
		if (soundTrigger == CollectSoundTrigger.OnStart)
		{
			PlayCollectSound(currencyType);
		}
		else if (animComplete == null)
		{
			animComplete = OnCollectAnimComplete;
		}
		GameObject collectAnimationPrefab = GetCollectAnimationPrefab(currencyType);
		CollectAnimation collectAnimation = null;
		int num2 = ((amount == -1) ? num : amount);
		for (int i = 0; i < num2 && i < num; i++)
		{
			collectAnimation = Helpers.InstantiateToParent(collectAnimationPrefab, (container != null) ? container : collectParent).GetComponent<CollectAnimation>();
			if (gameObjectToFollow == null)
			{
				collectAnimation.transform.position = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);
			}
			else
			{
				collectAnimation.FollowTarget(gameObjectToFollow);
			}
			bool isFirst = i == 0;
			collectAnimation.StartAnimation(amount, currencyType, animComplete, isFirst);
		}
		return collectAnimation;
	}

	public void CreateCollectAnim(Rewards rewards, GameObject gameObjectToFollow)
	{
		if (rewards == null)
		{
			return;
		}
		for (int i = 0; i < rewards.Count; i++)
		{
			if (rewards.GetRewardAt(i) is RewardCurrency rewardCurrency)
			{
				CreateCollectAnim(rewardCurrency.CurrencyType, gameObjectToFollow, rewardCurrency.Amount);
			}
		}
	}

	public void OnCollectAnimComplete(bool completed, CurrencyType currencyType)
	{
		PlayCollectSound(currencyType);
	}

	private void PlayCollectSound(CurrencyType currencyType)
	{
		Dictionary<CurrencyType, string> dictionary = new Dictionary<CurrencyType, string>();
		dictionary[CurrencyType.Supplies] = "global/collect_supplies";
		dictionary[CurrencyType.Diamonds] = "global/collect_diamonds";
		dictionary[CurrencyType.Inhabitants] = "global/collect_inhabitants";
		dictionary[CurrencyType.ReplayToken] = "global/collect_replay_tokens";
		dictionary[CurrencyType.SurvivalPoints] = "global/collect_survival_points";
		dictionary[CurrencyType.Phone] = "global/collect_phone";
		dictionary[CurrencyType.Outpost] = "global/collect_outpost";
		if (dictionary.ContainsKey(currencyType))
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(dictionary[currencyType]);
		}
		else if (currencyType.ToString().Contains("Token"))
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/collect_token");
		}
	}

	public CollectAnimation CreateBpReceivedAnim()
	{
		return Helpers.InstantiateToParent(bpReceivedAnimation, collectParent).GetComponent<CollectAnimation>();
	}

	public CollectAnimation InstantiateQuestPoint()
	{
		CollectAnimation result = null;
		if (questPointAnimation == null)
		{
			Debug.LogWarning("Could not instantiate questPointAniamtion!");
		}
		else
		{
			GameObject gameObject = Helpers.InstantiateToParent(questPointAnimation, collectParent);
			if (gameObject != null)
			{
				result = gameObject.GetComponent<CollectAnimation>();
			}
		}
		return result;
	}

	public CollectAnimation InstantiateNewBiePoint()
	{
		CollectAnimation result = null;
		if (newBiePointAnimation == null)
		{
			Debug.LogWarning("Could not instantiate questPointAniamtion!");
		}
		else
		{
			GameObject gameObject = Helpers.InstantiateToParent(newBiePointAnimation, collectParent);
			if (gameObject != null)
			{
				result = gameObject.GetComponent<CollectAnimation>();
			}
		}
		return result;
	}

	public CollectAnimation CreateStarsReceivedAnim(int amount, GameObject startPosition, Transform destination)
	{
		CollectAnimation component = Helpers.InstantiateToParent(starsReceivedAnimation, collectParent).GetComponent<CollectAnimation>();
		component.FollowTarget(startPosition);
		component.StartAnimation(amount, destination);
		return component;
	}

	public void CreateCollectEffect(CurrencyType currencyType, GameObject parent)
	{
		int num = (int)(currencyType - 1);
		if (num > -1 && num < collectEffects.Length)
		{
			GameObject gameObject = ((num < collectEffects.Length) ? collectEffects[(int)(currencyType - 1)] : null);
			if (gameObject != null && parent != null)
			{
				Helpers.InstantiateToParent(gameObject, parent);
			}
		}
	}

	public GameObject CreateBuildingUpgradeAvailableIndicator(BuildingView building)
	{
		Transform transform = building.BuildingGameObject.transform.Find("UpgradeParent");
		if (transform != null)
		{
			return Helpers.InstantiateToParent(buildingUpgradeAvailableIndicator, transform.gameObject);
		}
		Debug.LogWarning("Cannot find the UpgradeParent for the BuildingUpgradeAvailableIndicator");
		return null;
	}

	public BuildingUpgradeInsideIndicator CreateBuildingUpgradeInsideAvailableIndicator(BuildingView building)
	{
		BuildingUpgradeInsideIndicator buildingUpgradeInsideIndicator = CreateBuildingIndicator(buildingUpgradeInsideAvailableIndicator, building, followBasicIndicatorPosition: false) as BuildingUpgradeInsideIndicator;
		buildingUpgradeInsideIndicator.SetBuildingView(building);
		buildingUpgradeInsideIndicator.FollowTarget(building.gameObject, 0, buildingUpgradeInsideIndicator.AnchorY, 0, buildingUpgradeInsideIndicator.AnchorY);
		return buildingUpgradeInsideIndicator;
	}

	public RadioTentFreeCallIndicator CreateBuildingFreeCallIndicator(BuildingView building)
	{
		RadioTentFreeCallIndicator radioTentFreeCallIndicator = CreateBuildingIndicator(freeRadioCallIndicator, building, followBasicIndicatorPosition: false) as RadioTentFreeCallIndicator;
		radioTentFreeCallIndicator.SetBuildingView(building);
		radioTentFreeCallIndicator.FollowTarget(building.gameObject, 0, radioTentFreeCallIndicator.AnchorY, 0, radioTentFreeCallIndicator.AnchorY);
		return radioTentFreeCallIndicator;
	}

	public void RefreshExpansionIndicator(CampModel campModel)
	{
		if (expansionButtonIndicatorPrefab == null)
		{
			return;
		}
		List<int> list = null;
		foreach (KeyValuePair<int, ExpansionButtonIndicator> activeExpansionIndicator in activeExpansionIndicators)
		{
			if (activeExpansionIndicator.Value == null || activeExpansionIndicator.Value.gameObject == null)
			{
				if (list == null)
				{
					list = new List<int>();
				}
				list.Add(activeExpansionIndicator.Key);
			}
		}
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				activeExpansionIndicators.Remove(list[i]);
			}
		}
		Dictionary<int, List<VegetationModel>> dictionary = null;
		ModelList<BuildingModel> buildings = campModel.Buildings;
		for (int j = 0; j < buildings.Count; j++)
		{
			if (buildings[j] is VegetationModel { CutDependencyLevelRequired: >0, IsBeingCut: false, CutDependencyLevelRequired: var cutDependencyLevelRequired } vegetationModel)
			{
				if (dictionary == null)
				{
					dictionary = new Dictionary<int, List<VegetationModel>>();
				}
				if (!dictionary.TryGetValue(cutDependencyLevelRequired, out var value))
				{
					value = (dictionary[cutDependencyLevelRequired] = new List<VegetationModel>());
				}
				value.Add(vegetationModel);
			}
		}
		if (dictionary == null)
		{
			return;
		}
		foreach (KeyValuePair<int, List<VegetationModel>> item in dictionary)
		{
			int key = item.Key;
			if (!activeExpansionIndicators.ContainsKey(key))
			{
				GameObject gameObject = Helpers.InstantiateToParent(expansionButtonIndicatorPrefab, buildingParent);
				ExpansionButtonIndicator component = gameObject.GetComponent<ExpansionButtonIndicator>();
				if (component == null)
				{
					Debug.LogError("expansionButtonIndicatorPrefab 上缺少 ExpansionButtonIndicator 组件。");
					Object.Destroy(gameObject);
					break;
				}
				component.Init(item.Value);
				activeExpansionIndicators[key] = component;
			}
		}
	}
}
