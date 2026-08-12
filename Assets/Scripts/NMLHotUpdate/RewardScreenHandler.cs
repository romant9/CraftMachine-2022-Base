using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class RewardScreenHandler : MonoBehaviour
{
	public delegate void RewardBoxCallback(GameObject box, LootEntry reward, LootEntry reward2);

	public enum LootCardPlacement
	{
		Single = 0,
		DualFirst = 1,
		DualSecond = 2,
		ThreeByThree = 3
	}

	[Tooltip("Loot box locations")]
	[SerializeField]
	public List<GameObject> lootBoxLocations;

	[Tooltip("Loot box prefab for common box")]
	[SerializeField]
	public GameObject lootBoxPrefabCommon;

	[Tooltip("Loot box prefab for uncommon box")]
	[SerializeField]
	public GameObject lootBoxPrefabUncommon;

	[Tooltip("Loot box prefab for rare box")]
	[SerializeField]
	public GameObject lootBoxPrefabRare;

	[Tooltip("Loot box prefab for legendary box")]
	[SerializeField]
	public GameObject lootBoxPrefabLegendary;

	[Tooltip("Loot box prefab for golden challenge box")]
	[SerializeField]
	public GameObject lootBoxPrefabChallengeGold;

	[Tooltip("Loot box prefab for silver challenge box")]
	[SerializeField]
	public GameObject lootBoxPrefabChallengeSilver;

	[Tooltip("Loot box prefab for apocalyptic challenge box")]
	[SerializeField]
	public GameObject lootBoxPrefabChallengeApocalyptic;

	[Tooltip("Loot box prefab for token challenge box")]
	[SerializeField]
	public GameObject lootBoxPrefabChallengeToken;

	[Tooltip("Loot box prefab for golden survival box")]
	[SerializeField]
	public GameObject lootBoxPrefabSurvivalGold;

	[Tooltip("Loot box prefab for silver survival box")]
	[SerializeField]
	public GameObject lootBoxPrefabSurvivalSilver;

	[Tooltip("Loot box prefab for token survival box")]
	[SerializeField]
	public GameObject lootBoxPrefabSurvivalToken;

	[Tooltip("Loot box prefab for golden daily quest box")]
	[SerializeField]
	public GameObject lootBoxPrefabDailyQuestGold;

	[Tooltip("Loot box prefab for silver daily quest box")]
	[SerializeField]
	public GameObject lootBoxPrefabDailyQuestSilver;

	[Tooltip("Loot box prefab for daily quest class token box")]
	[SerializeField]
	public GameObject lootBoxPrefabDailyQuestClassToken;

	[Tooltip("Loot box prefab for daily quest hero token box")]
	[SerializeField]
	public GameObject lootBoxPrefabDailyQuestHeroToken;

	[SerializeField]
	private GameObject lootCardPrefab;

	[Header("Combat screen")]
	[Tooltip("All stuff specific to combat screen")]
	[SerializeField]
	public GameObject combatScreenContainer;

	[SerializeField]
	[Tooltip("The camera showing this screen for combat screen.")]
	public Camera screenCameraCombat;

	[Header("Videa ads screen")]
	[SerializeField]
	private Vector3 cameraStartPosition;

	[SerializeField]
	private Vector3 cameraEndPosition;

	[SerializeField]
	private float cameraMoveSpeed;

	[Header("In Ui screen")]
	[SerializeField]
	public GameObject inUiScreenContainer;

	[SerializeField]
	public Camera screenCameraInUi;

	[SerializeField]
	public Transform inUiBoxLocation;

	[Header("Margin for tree by tree")]
	[Tooltip("Grid horizontal and vertical margin (of card size)")]
	[SerializeField]
	private Vector2 marginSetting = new Vector2(0.05f, 0.1f);

	private List<GameObject> lootBoxes = new List<GameObject>();

	private LootScreenType screenType;

	private LootEntry lootEntry;

	private bool _isApocalypticCrate;

	private int numberBoxOpened;

	private int numberBoxOpening;

	private Camera currentCamera;

	private bool showAdsRewards;

	private List<LootEntry> boxesAlreadyOpened = new List<LootEntry>();

	private Ray ray;

	private Vector3 BoxesCenterRelativeToUI = default(Vector2);

	private bool BoxesCenterCalculated;

	private GameObject videoAdsScreenContainer;

	private Camera screenCameraVideoAds;

	private static Dictionary<DropCurrenciesProbabilitiesDefinition.DropCurrency, string> dropCurrencySounds;

	public static RewardScreenHandler Instance { get; private set; }

	public event RewardBoxCallback OnRewardBoxClicked;

	public event RewardBoxCallback OnRewardBoxOpened;

	public event Callback OnRewardBoxOpenedAnimationOver;

	public RewardScreenHandler()
	{
		Instance = this;
	}

	public void Update()
	{
		if ((screenType == LootScreenType.InUi || screenType == LootScreenType.InUiSurvival || screenType == LootScreenType.InUIPlayer || screenType == LootScreenType.Ad || screenType == LootScreenType.GuildGift || screenType == LootScreenType.TradeCrate || screenType == LootScreenType.IAPBonusGift || screenType == LootScreenType.Quiz || screenType == LootScreenType.DailyQuestChest || screenType == LootScreenType.BattlePassBonusChest || !UICamera.isOverUI) && Input.GetMouseButtonDown(0) && currentCamera != null)
		{
			ray = currentCamera.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hitInfo, float.PositiveInfinity, 524288))
			{
				RewardBox componentInChildren = hitInfo.collider.gameObject.GetComponentInChildren<RewardBox>();
				if (componentInChildren != null && !componentInChildren.Opened)
				{
					RewardBoxClicked(componentInChildren);
				}
			}
		}
		if (screenType == LootScreenType.Ad && showAdsRewards && currentCamera != null)
		{
			currentCamera.transform.localPosition = Vector3.Lerp(currentCamera.transform.localPosition, cameraEndPosition, cameraMoveSpeed * Time.deltaTime);
			Vector3.Distance(currentCamera.transform.position, cameraEndPosition);
			_ = 1f;
		}
	}

	public void ShowScene(LootScreenType screenType, LootEntry lootEntry = null, bool isApocalypticCrate = false)
	{
		this.screenType = screenType;
		this.lootEntry = lootEntry;
		_isApocalypticCrate = isApocalypticCrate;
		if (screenType == LootScreenType.Ad)
		{
			CheckCreateAdScreen();
			currentCamera = screenCameraVideoAds;
			currentCamera.transform.localPosition = cameraStartPosition;
			showAdsRewards = false;
		}
		else if (IsInUiScreen(screenType))
		{
			currentCamera = screenCameraInUi;
			if (SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/bundle_intro");
			}
		}
		else
		{
			currentCamera = screenCameraCombat;
		}
		combatScreenContainer.SetActive(screenType == LootScreenType.Combat);
		if (videoAdsScreenContainer != null)
		{
			videoAdsScreenContainer.SetActive(screenType == LootScreenType.Ad);
		}
		inUiScreenContainer.SetActive(IsInUiScreen(screenType));
		numberBoxOpened = 0;
		numberBoxOpening = 0;
		base.gameObject.SetActive(value: true);
		HideCombat();
	}

	private static bool IsInUiScreen(LootScreenType screenType)
	{
		if (screenType != LootScreenType.InUi && screenType != LootScreenType.InUiSurvival && screenType != LootScreenType.InUIPlayer && screenType != LootScreenType.GuildGift && screenType != LootScreenType.TradeCrate && screenType != LootScreenType.IAPBonusGift && screenType != LootScreenType.Quiz && screenType != LootScreenType.DailyQuestChest)
		{
			return screenType == LootScreenType.BattlePassBonusChest;
		}
		return true;
	}

	private void CheckCreateAdScreen()
	{
		if (!(videoAdsScreenContainer != null))
		{
			PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("VideoAdScreen/VideoAdScreen", "scriptableobjects");
			videoAdsScreenContainer = Object.Instantiate(prefabResource.GetPrefab(), base.transform);
			screenCameraVideoAds = videoAdsScreenContainer.GetComponentInChildren<Camera>();
		}
	}

	public void HideScene()
	{
		base.gameObject.SetActive(value: false);
		DestroyRewardBoxes();
	}

	public void HideCombat()
	{
		Scenario[] array = Object.FindObjectsOfType<Scenario>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
	}

	public int GetKeysLeft()
	{
		return GameManager.Instance.playerModel.LootManager.AvailableKeys;
	}

	public int GetUnopenedRewardsLeft()
	{
		int num = 0;
		LootManagerModel lootManager = GameManager.Instance.playerModel.LootManager;
		for (int i = 0; i < lootManager.Loots.Count; i++)
		{
			if (lootManager.Loots[i] != null && !lootManager.Loots[i].Opened)
			{
				num++;
			}
		}
		return num;
	}

	public bool CanClickRewardBox()
	{
		if (screenType != LootScreenType.Combat)
		{
			return numberBoxOpened == 0;
		}
		return GameManager.Instance.playerModel.LootManager.CanOpenLootBox();
	}

	public List<LootEntry> GetOpenedRewardBoxes()
	{
		if (screenType == LootScreenType.InUi || screenType == LootScreenType.InUiSurvival || screenType == LootScreenType.InUIPlayer || screenType == LootScreenType.Ad || screenType == LootScreenType.GuildGift || screenType == LootScreenType.TradeCrate || screenType == LootScreenType.IAPBonusGift || screenType == LootScreenType.DailyQuestChest || screenType == LootScreenType.BattlePassBonusChest)
		{
			return null;
		}
		return GameManager.Instance.playerModel.LootManager.GetOpenedLoots();
	}

	public void PlayRewardBoxEffect(GameObject box, LootEntry reward, LootEntry reward2)
	{
		GameObject gameObject = box;
		if (reward != null && !reward.IsChallengeReward() && !reward.IsSurvivalReward() && reward.DropType != DropType.Regular)
		{
			Vector3 position = box.transform.position;
			lootBoxes.Remove(box);
			Object.Destroy(box);
			gameObject = Object.Instantiate((reward.DropType == DropType.Gold) ? lootBoxPrefabRare : lootBoxPrefabUncommon, position, Quaternion.identity);
			lootBoxes.Add(gameObject);
		}
		Animator componentInChildren = gameObject.GetComponentInChildren<Animator>();
		if (componentInChildren != null && !componentInChildren.enabled)
		{
			componentInChildren.enabled = true;
		}
		RewardBox componentInChildren2 = gameObject.GetComponentInChildren<RewardBox>();
		if (componentInChildren2 != null)
		{
			componentInChildren2.VisualEffect(screenType);
		}
		NotifyLootBoxOpened(gameObject, reward, reward2);
	}

	private void OnEnable()
	{
		CreateRewardBoxes();
	}

	private void OnDisable()
	{
		BoxesCenterCalculated = false;
		DestroyRewardBoxes();
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public Vector3 CalculateBoxesCenterRelativeToUI(Transform relativeUI, bool update = false)
	{
		BoxesCenterCalculated = !update && BoxesCenterCalculated;
		if (lootBoxLocations != null && !BoxesCenterCalculated)
		{
			for (int i = 0; i < lootBoxLocations.Count; i++)
			{
				relativeUI.OverlayPosition(GetBoxPosition(i).transform.position, currentCamera);
				BoxesCenterRelativeToUI += relativeUI.localPosition;
			}
			BoxesCenterRelativeToUI = ((lootBoxLocations.Count > 0) ? (BoxesCenterRelativeToUI / lootBoxLocations.Count) : new Vector3(0f, 0f, 0f));
			BoxesCenterCalculated = true;
		}
		return BoxesCenterRelativeToUI;
	}

	public GameObject CreateLootCard(GameObject box, LootEntry reward, Transform parent, LootCardPlacement placement = LootCardPlacement.Single)
	{
		GameObject gameObject = Object.Instantiate(lootCardPrefab);
		if (gameObject != null)
		{
			gameObject.transform.SetParent(parent, worldPositionStays: false);
			LootCard component = gameObject.GetComponent<LootCard>();
			Vector3 vector = default(Vector3);
			Vector3 vector2 = new Vector3(0f, 0f, 0f);
			gameObject.transform.OverlayPosition(box.transform.position, currentCamera);
			vector = gameObject.transform.localPosition;
			vector.z = 0f;
			gameObject.transform.localPosition = vector;
			if (placement == LootCardPlacement.DualFirst && component != null)
			{
				component.SetAnimationParameters(moveToLeft: true, moveToRight: false);
			}
			if (placement == LootCardPlacement.DualSecond && component != null)
			{
				component.SetAnimationParameters(moveToLeft: false, moveToRight: true);
			}
			if (placement == LootCardPlacement.ThreeByThree)
			{
				Vector2 vector3 = marginSetting;
				vector3.x += 1f;
				vector3.y += 1f;
				vector2 = CalculateBoxesCenterRelativeToUI(gameObject.transform);
				Vector3 vector4 = default(Vector2);
				Vector3 vector5 = default(Vector2);
				if (component != null)
				{
					vector4 = component.GetSize();
					vector5 = component.GetSize() * 0.5f;
					vector4.x *= vector3.x;
					vector4.y *= vector3.y;
				}
				if (vector.x < vector2.x - vector5.x)
				{
					vector.x = vector2.x - vector4.x;
				}
				else if (vector.x > vector2.x + vector5.x)
				{
					vector.x = vector2.x + vector4.x;
				}
				else
				{
					vector.x = vector2.x;
				}
				if (vector.y > vector2.y + vector5.y)
				{
					vector.y = vector2.y + vector4.y;
				}
				else if (vector.y < vector2.y - vector5.y)
				{
					vector.y = vector2.y - vector4.y;
				}
				else
				{
					vector.y = vector2.y;
				}
				vector.z = 0f;
				gameObject.transform.localPosition = vector;
			}
			if (component != null)
			{
				component.SetReward(reward, GameManager.Instance.CheckConnectionReachability());
				for (int i = 0; i < boxesAlreadyOpened.Count; i++)
				{
					if (boxesAlreadyOpened[i] == reward)
					{
						component.ShowFlyingCurrencies = false;
					}
				}
				TweenManager.PlayTweenGroup(component.gameObject, 0);
			}
			string soundString = "";
			if (TryGetCurrencySound(reward.DropCurrencyType, out soundString))
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(soundString);
			}
		}
		return gameObject;
	}

	private Transform GetBoxPosition(int index)
	{
		if (screenType == LootScreenType.InUi || screenType == LootScreenType.InUiSurvival || screenType == LootScreenType.InUIPlayer || screenType == LootScreenType.GuildGift || screenType == LootScreenType.TradeCrate || screenType == LootScreenType.IAPBonusGift || screenType == LootScreenType.DailyQuestChest || screenType == LootScreenType.BattlePassBonusChest)
		{
			return inUiBoxLocation;
		}
		return lootBoxLocations[index].transform;
	}

	private void CreateRewardBoxes()
	{
		boxesAlreadyOpened.Clear();
		int num = 9;
		if (screenType == LootScreenType.Ad)
		{
			num = 3;
		}
		else if (IsInUiScreen(screenType))
		{
			num = 1;
		}
		List<LootEntry> openedRewardBoxes = GetOpenedRewardBoxes();
		if (openedRewardBoxes != null)
		{
			for (int i = 0; i < openedRewardBoxes.Count; i++)
			{
				boxesAlreadyOpened.Add(openedRewardBoxes[i]);
			}
		}
		for (int j = 0; j < num; j++)
		{
			GameObject gameObject = Object.Instantiate(GetBoxPrefabByType(lootEntry), GetBoxPosition(j).position, Quaternion.identity);
			lootBoxes.Add(gameObject);
			RewardBox componentInChildren = gameObject.GetComponentInChildren<RewardBox>();
			if (openedRewardBoxes == null)
			{
				continue;
			}
			foreach (LootEntry item in openedRewardBoxes)
			{
				if (item.BoxIndex == j)
				{
					Animator componentInChildren2 = gameObject.GetComponentInChildren<Animator>();
					if (componentInChildren2 != null && !componentInChildren2.enabled)
					{
						componentInChildren2.enabled = true;
					}
					componentInChildren.Reward = item;
					componentInChildren.Opened = true;
					break;
				}
			}
		}
	}

	private GameObject GetBoxPrefabByType(LootEntry lootEntry)
	{
		if (_isApocalypticCrate)
		{
			return lootBoxPrefabChallengeApocalyptic;
		}
		if (lootEntry != null)
		{
			if (lootEntry.IsChallengeReward() && lootEntry.DropEventDefinition != null)
			{
				if (lootEntry.DropEventDefinition.Tag == DropEventDefinition.DropEventTag.ChallengeCrateGold)
				{
					return lootBoxPrefabChallengeGold;
				}
				if (lootEntry.DropEventDefinition.Tag == DropEventDefinition.DropEventTag.ChallengeCrateSilver)
				{
					return lootBoxPrefabChallengeSilver;
				}
				if (lootEntry.DropEventDefinition.Tag == DropEventDefinition.DropEventTag.TokenCrate)
				{
					return lootBoxPrefabChallengeToken;
				}
			}
			if (lootEntry.IsSurvivalReward() && lootEntry.DropEventDefinition != null)
			{
				if (lootEntry.DropEventDefinition.Tag == DropEventDefinition.DropEventTag.SurvivalCrateGold)
				{
					return lootBoxPrefabSurvivalGold;
				}
				if (lootEntry.DropEventDefinition.Tag == DropEventDefinition.DropEventTag.SurvivalCrateSilver)
				{
					return lootBoxPrefabSurvivalSilver;
				}
				if (lootEntry.DropEventDefinition.Tag == DropEventDefinition.DropEventTag.TokenCrate)
				{
					return lootBoxPrefabSurvivalToken;
				}
			}
			if (lootEntry.IsDailyQuestReward() && lootEntry.DropEventDefinition != null)
			{
				if (lootEntry.DropEventDefinition.Tag == DropEventDefinition.DropEventTag.QuestChestGold)
				{
					return lootBoxPrefabDailyQuestGold;
				}
				if (lootEntry.DropEventDefinition.Tag == DropEventDefinition.DropEventTag.QuestChestSilver)
				{
					return lootBoxPrefabDailyQuestSilver;
				}
				if (lootEntry.DropEventDefinition.Tag == DropEventDefinition.DropEventTag.QuestChestClassToken)
				{
					return lootBoxPrefabDailyQuestClassToken;
				}
				if (lootEntry.DropEventDefinition.Tag == DropEventDefinition.DropEventTag.QuestChestHeroToken)
				{
					return lootBoxPrefabDailyQuestHeroToken;
				}
			}
		}
		return lootBoxPrefabCommon;
	}

	private void DestroyRewardBoxes()
	{
		foreach (GameObject lootBox in lootBoxes)
		{
			Object.Destroy(lootBox);
		}
		lootBoxes.Clear();
		lootEntry = null;
	}

	private void RewardBoxClicked(RewardBox rewardBox)
	{
		if (CanClickRewardBox() && !SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.CampSurvivorInfoPopup))
		{
			int rewardBoxIndex = GetRewardBoxIndex(rewardBox);
			if (rewardBox.Open(screenType, rewardBoxIndex))
			{
				numberBoxOpened++;
				numberBoxOpening++;
				NotifyLootBoxClicked(rewardBox.transform.parent.gameObject, rewardBox.Reward, rewardBox.Reward2);
			}
		}
	}

	private void NotifyLootBoxClicked(GameObject openedBox, LootEntry reward, LootEntry reward2)
	{
		this.OnRewardBoxClicked?.Invoke(openedBox, reward, reward2);
	}

	private void NotifyLootBoxOpened(GameObject openedBox, LootEntry reward, LootEntry reward2)
	{
		this.OnRewardBoxOpened?.Invoke(openedBox, reward, reward2);
	}

	public void BoxAnimationOver()
	{
		numberBoxOpening--;
		if (numberBoxOpening <= 0 && this.OnRewardBoxOpenedAnimationOver != null)
		{
			this.OnRewardBoxOpenedAnimationOver();
		}
	}

	private int GetRewardBoxIndex(RewardBox rewardBox)
	{
		for (int i = 0; i < lootBoxes.Count; i++)
		{
			if (lootBoxes[i] == rewardBox.transform.parent.gameObject)
			{
				return i;
			}
		}
		return -1;
	}

	public void ShowAdsRewards()
	{
		showAdsRewards = true;
	}

	private bool TryGetCurrencySound(DropCurrenciesProbabilitiesDefinition.DropCurrency currencyDefinition, out string soundString)
	{
		if (dropCurrencySounds == null || dropCurrencySounds.Count == 0)
		{
			dropCurrencySounds = new Dictionary<DropCurrenciesProbabilitiesDefinition.DropCurrency, string>();
			dropCurrencySounds[DropCurrenciesProbabilitiesDefinition.DropCurrency.Supplies] = "reward_screen/supplies_found";
			dropCurrencySounds[DropCurrenciesProbabilitiesDefinition.DropCurrency.Diamonds] = "reward_screen/diamonds_found";
			dropCurrencySounds[DropCurrenciesProbabilitiesDefinition.DropCurrency.SurvivalPoints] = "reward_screen/survival_points_found";
			dropCurrencySounds[DropCurrenciesProbabilitiesDefinition.DropCurrency.Inhabitant] = "reward_screen/inhabitant_found";
			dropCurrencySounds[DropCurrenciesProbabilitiesDefinition.DropCurrency.Weapon] = "reward_screen/weapon_found";
			dropCurrencySounds[DropCurrenciesProbabilitiesDefinition.DropCurrency.Armor] = "reward_screen/armor_found";
			dropCurrencySounds[DropCurrenciesProbabilitiesDefinition.DropCurrency.ReplayToken] = "reward_screen/replay_token_found";
			dropCurrencySounds[DropCurrenciesProbabilitiesDefinition.DropCurrency.Phone] = "reward_screen/phone_found";
		}
		if (dropCurrencySounds.TryGetValue(currencyDefinition, out soundString))
		{
			return true;
		}
		return false;
	}
}
