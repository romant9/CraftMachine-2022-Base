using System;
using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class MissionView : MonoBehaviour
{
	[Tooltip("How much gap / margin between path start/end and the path points. Number between 0 and 0.5.")]
	public float PathMargin;

	[SerializeField]
	private GameObject backgroundContainer;

	[SerializeField]
	private GameObject missionIconContainer;

	[SerializeField]
	private AnimationCurve[] positionCurves;

	[SerializeField]
	private int topMargin;

	[SerializeField]
	private int bottomMargin;

	[SerializeField]
	private int leftMargin;

	[SerializeField]
	private int rightMargin;

	[SerializeField]
	private GameObject[] challengeSpawnPoints;

	[SerializeField]
	private GameObject[] challengeSpawnPointsWithMasterMission;

	[SerializeField]
	private GameObject[] seasonSpawnPoints;

	private GameObject currentBackground;

	private List<GameObject> scrollableBackgroundObjects;

	private List<ScrollableMapItemInstance> scrollableBackgroundPathItems;

	private Vector2 scrollableMapScale;

	private int scrollableMapStartDepth;

	private List<MissionIcon> missionIcons;

	private MapVisualData mapVisualData;

	private int screenHeight;

	private int screenWidth;

	private float nguiRatio;

	private Coroutine activeOpenChallengeRewardsAfterDelayCoroutine;

	private Coroutine activeOpenSurvivalRewardsAfterDelayCoroutine;

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (!(type == "SurvivalDoubleRewardsEnabled"))
		{
			return;
		}
		DetailMapPopUp detailMapPopUp = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.DetailMapPopUp) as DetailMapPopUp;
		if (!(detailMapPopUp != null))
		{
			return;
		}
		MapMissionGroupModel currentMap = detailMapPopUp.CurrentMap;
		if (currentMap != null && currentMap.IsWeeklySurvival)
		{
			for (int i = 0; i < ((missionIcons != null) ? missionIcons.Count : 0); i++)
			{
				missionIcons[i].EnableDoubleRewardsIcon(doubleRewardsEnabled: true);
			}
		}
	}

	private MapVisualData LoadMapVisualData()
	{
		return UnityUtils.LoadFromAssetBundle<MapVisualData>("MapVisualData", "scriptableobjects");
	}

	public bool LoadMap(MapMissionGroupModel mapMissionGroupModel, UIWidget scrollableMapSlotReference)
	{
		Vector2 vector = Helpers.CalculateNguiScreenSize(base.gameObject);
		screenHeight = (int)vector.y;
		screenWidth = (int)vector.x;
		mapVisualData = LoadMapVisualData();
		if (mapMissionGroupModel.IsWeeklySurvival)
		{
			if (!LoadScrollableBackground(mapMissionGroupModel.MissionSpawnPointGroup.BackgroundId, scrollableMapSlotReference))
			{
				return false;
			}
		}
		else if (!LoadBackground(mapMissionGroupModel.MissionSpawnPointGroup.BackgroundId))
		{
			return false;
		}
		if (!LoadMissions(mapMissionGroupModel))
		{
			return false;
		}
		if (GameManager.Instance.playerModel.MapContainerModel.LastPlayedMissionModel != null && GameManager.Instance.playerModel.MapContainerModel.LastPlayedMissionModel.IsInWeeklyChallenge)
		{
			if (activeOpenChallengeRewardsAfterDelayCoroutine != null)
			{
				StopCoroutine(activeOpenChallengeRewardsAfterDelayCoroutine);
			}
			activeOpenChallengeRewardsAfterDelayCoroutine = SingularityMonoBehaviour<HUDManager>.Instance.StartCoroutine(OpenChallengeRewardsAfterDelay(1f));
		}
		else if (mapMissionGroupModel != null && mapMissionGroupModel.IsWeeklyChallenge && GameManager.Instance.playerModel.WeeklyChallenge.CanCollectRewards)
		{
			if (activeOpenChallengeRewardsAfterDelayCoroutine != null)
			{
				StopCoroutine(activeOpenChallengeRewardsAfterDelayCoroutine);
			}
			activeOpenChallengeRewardsAfterDelayCoroutine = SingularityMonoBehaviour<HUDManager>.Instance.StartCoroutine(OpenChallengeRewardsAfterDelay(0f));
		}
		if (GameManager.Instance.playerModel.MapContainerModel.LastPlayedMissionModel != null && GameManager.Instance.playerModel.MapContainerModel.LastPlayedMissionModel.IsInApocalyptiWeeklyChallenge)
		{
			if (activeOpenChallengeRewardsAfterDelayCoroutine != null)
			{
				StopCoroutine(activeOpenChallengeRewardsAfterDelayCoroutine);
			}
			activeOpenChallengeRewardsAfterDelayCoroutine = SingularityMonoBehaviour<HUDManager>.Instance.StartCoroutine(OpenApocalypticChallengeRewardsAfterDelay(1f));
		}
		else if (mapMissionGroupModel != null && mapMissionGroupModel.IsInApocalyptiWeeklyChallenge && GameManager.Instance.playerModel.WeeklyChallenge.CanCollectApocalypticRewards)
		{
			if (activeOpenChallengeRewardsAfterDelayCoroutine != null)
			{
				StopCoroutine(activeOpenChallengeRewardsAfterDelayCoroutine);
			}
			activeOpenChallengeRewardsAfterDelayCoroutine = SingularityMonoBehaviour<HUDManager>.Instance.StartCoroutine(OpenApocalypticChallengeRewardsAfterDelay(0f));
		}
		if (GameManager.Instance.playerModel.MapContainerModel.LastPlayedMissionModel != null && GameManager.Instance.playerModel.MapContainerModel.LastPlayedMissionModel.IsInWeeklySurvival)
		{
			if (activeOpenSurvivalRewardsAfterDelayCoroutine != null)
			{
				StopCoroutine(activeOpenSurvivalRewardsAfterDelayCoroutine);
			}
			activeOpenSurvivalRewardsAfterDelayCoroutine = SingularityMonoBehaviour<HUDManager>.Instance.StartCoroutine(OpenSurvivalRewardsAfterDelay(1f));
		}
		else if (mapMissionGroupModel != null && mapMissionGroupModel.IsWeeklySurvival && GameManager.Instance.playerModel.WeeklySurvival.CanCollectRewards)
		{
			if (activeOpenSurvivalRewardsAfterDelayCoroutine != null)
			{
				StopCoroutine(activeOpenSurvivalRewardsAfterDelayCoroutine);
			}
			activeOpenSurvivalRewardsAfterDelayCoroutine = SingularityMonoBehaviour<HUDManager>.Instance.StartCoroutine(OpenSurvivalRewardsAfterDelay(0f));
		}
		GameManager.Instance.playerModel.MapContainerModel.LastPlayedMissionModel = null;
		return true;
	}

	private IEnumerator OpenChallengeRewardsAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (GameManager.Instance.playerModel.WeeklyChallenge.CanCollectRewards && !WeeklyChallengeRewardListPopup.TryOpenForGuildGifts())
		{
			while (SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.WeeklyChallengeNextCycle))
			{
				yield return null;
			}
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(GameManager.Instance.playerModel.WeeklyChallenge);
		}
		yield return null;
		activeOpenChallengeRewardsAfterDelayCoroutine = null;
	}

	private IEnumerator OpenApocalypticChallengeRewardsAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (GameManager.Instance.playerModel.WeeklyChallenge.CanCollectApocalypticRewards)
		{
			while (SingularityMonoBehaviour<HUDManager>.Instance.IsActive(UIType.WeeklyChallengeNextCycle))
			{
				yield return null;
			}
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(GameManager.Instance.playerModel.WeeklyChallenge);
		}
		yield return null;
		activeOpenChallengeRewardsAfterDelayCoroutine = null;
	}

	private IEnumerator OpenSurvivalRewardsAfterDelay(float delay)
	{
		yield return new WaitForSeconds(delay);
		if (GameManager.Instance.playerModel.WeeklySurvival.CanCollectRewards)
		{
			SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.OpenLootInUi).OpenForModel(GameManager.Instance.playerModel.WeeklySurvival);
		}
		yield return null;
		activeOpenSurvivalRewardsAfterDelayCoroutine = null;
	}

	private bool LoadMissions(MapMissionGroupModel mapMissionGroupModel)
	{
		if (mapMissionGroupModel.Missions == null)
		{
			Debug.LogError("No missions in map " + mapMissionGroupModel.MissionSpawnPointGroupId);
			return false;
		}
		if (positionCurves == null || positionCurves.Length == 0)
		{
			Debug.LogError("No missions position curves");
			return false;
		}
		if (mapMissionGroupModel.IsWeeklyChallenge && !WeeklyChallengeHelper.IsChallengeOngoing())
		{
			return false;
		}
		if (mapMissionGroupModel.IsInApocalyptiWeeklyChallenge && !WeeklyChallengeHelper.IsChallengeOngoing())
		{
			return false;
		}
		if (mapMissionGroupModel.IsWeeklyChallenge)
		{
			WeeklyChallengeStartSkippingPopup.TryOpenOnChallengeEnter();
			WeeklyChallengeInfoPopup.TryOpenOnChallengeEnter();
			WeeklyChallengeMasterMissionInfo.TryOpenOnChallengeEnter();
			PlightIntroductionPopup.TryOpenOnChallengeEnter();
			if (WeeklyChallengeHelper.HasCompletedTheFinalRound())
			{
				return true;
			}
		}
		if (mapMissionGroupModel.IsInApocalyptiWeeklyChallenge)
		{
			ApocalypticWeeklyChallengeStartSkippingPopup.TryOpenOnChallengeEnter();
			if (WeeklyChallengeHelper.HasCompletedTheFinalRound())
			{
				return true;
			}
		}
		bool isWeeklySurvival = mapMissionGroupModel.IsWeeklySurvival;
		if (isWeeklySurvival)
		{
			WeeklySurvivalInfoPopup.TryOpenOnSurvivalEnter();
		}
		int curveId = new System.Random(mapMissionGroupModel.MissionSpawnPointGroupId).Next(0, positionCurves.Length - 1);
		missionIcons = new List<MissionIcon>();
		GameObject path = null;
		for (int i = 0; i < mapMissionGroupModel.Missions.Count; i++)
		{
			MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[i];
			bool isLastOrLarge = i == mapMissionGroupModel.Missions.Count - 1;
			if (isWeeklySurvival)
			{
				isLastOrLarge = IsScrollableMapPathLarge(i);
				if (!mapMissionModel.IsInWeeklySurvival || mapMissionModel.GetDifficultyInWeeklySurvival() == SurvivalDifficulty.None)
				{
					continue;
				}
			}
			if (mapMissionModel.IsMasterMission && !GameManager.Instance.playerModel.IsMasterMissionUnlocked)
			{
				continue;
			}
			MissionIcon component = Helpers.InstantiateToParent(GetMissionIconPrefab(mapMissionModel, isLastOrLarge), missionIconContainer).GetComponent<MissionIcon>();
			component.SetMission(mapMissionModel, mapVisualData);
			if (mapMissionGroupModel.IsWeeklyChallenge || mapMissionModel.IsInApocalyptiWeeklyChallenge)
			{
				component.gameObject.transform.localPosition = GetChallengePosition(i);
			}
			else if (isWeeklySurvival)
			{
				component.gameObject.transform.localPosition = GetScrollableMapPathPosition(i);
				if (GameManager.Instance.playerModel.WeeklySurvival != null)
				{
					component.SetDifficulty(GameManager.Instance.playerModel.WeeklySurvival.CurrentDifficulty);
					component.EnableDoubleRewardsIcon(GameManager.Instance.playerModel.WeeklySurvival.DoubleRewardsEnabled);
				}
				component.SetPath(path);
			}
			else if (mapMissionGroupModel.MissionSpawnPointGroup.Category == MapCategory.Season)
			{
				component.gameObject.transform.localPosition = GetSeasonPosition(i);
				component.SetPath(path);
			}
			else
			{
				SetIconPosition(component.gameObject, curveId, (float)i / (float)mapMissionGroupModel.Missions.Count);
				component.SetPath(path);
			}
			missionIcons.Add(component);
			path = component.gameObject;
		}
		if (!mapMissionGroupModel.IsWeeklyChallenge && !mapMissionGroupModel.IsWeeklySurvival && !mapMissionGroupModel.IsInApocalyptiWeeklyChallenge && mapMissionGroupModel.MissionSpawnPointGroup.Category != MapCategory.Season)
		{
			missionIconContainer.transform.localPosition = new Vector3(0f - ((float)screenWidth / 2f - (float)leftMargin), 0f - ((float)screenHeight / 2f - (float)bottomMargin), 0f);
		}
		return true;
	}

	private void ConvertScrollableMapSizeOnScreen(int width, int height, out float screenWidth, out float screenHeight)
	{
		screenWidth = (float)width / 1024f * scrollableMapScale.x;
		screenHeight = (float)height / 1024f * scrollableMapScale.y;
	}

	private Vector3 ConvertScrollableMapPositionOnScreen(int globalX, int globalY)
	{
		return new Vector3((float)globalX / 1024f * scrollableMapScale.x, scrollableMapScale.y / 2f - (float)globalY / 1024f * scrollableMapScale.y, 0f);
	}

	private bool IsScrollableMapPathLarge(int index)
	{
		if (scrollableBackgroundPathItems == null || index >= scrollableBackgroundPathItems.Count)
		{
			return false;
		}
		return scrollableBackgroundPathItems[index].Item.Tex.EndsWith("Large");
	}

	private Vector3 GetScrollableMapPathPosition(int index)
	{
		if (scrollableBackgroundPathItems == null || scrollableBackgroundPathItems.Count == 0)
		{
			Debug.LogError("Attempt to get scrollable map icon position but no path items.");
			return new Vector3(0f, 0f, 0f);
		}
		if (index >= scrollableBackgroundPathItems.Count)
		{
			Debug.LogError("Attempt to get mission icon position for index that is beyond the amount of path items specified in the background map.");
			index = scrollableBackgroundPathItems.Count - 1;
		}
		return ConvertScrollableMapPositionOnScreen(scrollableBackgroundPathItems[index].GlobalX, scrollableBackgroundPathItems[index].GlobalY);
	}

	public float GetScrollableMapMissionPosition(int missionIndex)
	{
		if (scrollableBackgroundPathItems == null || scrollableBackgroundPathItems.Count == 0)
		{
			return 0f;
		}
		if (missionIndex < 0)
		{
			missionIndex = 0;
		}
		if (missionIndex >= scrollableBackgroundPathItems.Count)
		{
			missionIndex = scrollableBackgroundPathItems.Count - 1;
		}
		return UtilsMath.Map(scrollableBackgroundPathItems[missionIndex].GlobalX, 0f, 6144f, 0f, 1f);
	}

	private Vector3 GetChallengePosition(int index)
	{
		if (GameManager.Instance.playerModel.IsMasterMissionUnlocked)
		{
			return challengeSpawnPointsWithMasterMission[index].transform.localPosition;
		}
		return challengeSpawnPoints[index].transform.localPosition;
	}

	private Vector3 GetSeasonPosition(int index)
	{
		return seasonSpawnPoints[index].transform.localPosition;
	}

	private void SetIconPosition(GameObject icon, int curveId, float curvePosition)
	{
		AnimationCurve animationCurve = positionCurves[curveId];
		icon.transform.localPosition = new Vector3(curvePosition * (float)(screenWidth - (leftMargin + rightMargin)), animationCurve.Evaluate(curvePosition) * (float)(screenHeight - (topMargin + bottomMargin)), 0f);
	}

	public List<MissionIcon> GetMissionIcons()
	{
		return missionIcons;
	}

	public GameObject GetMissionIconPrefab(MapMissionModel mapMissionModel, bool isLastOrLarge)
	{
		if (mapMissionModel.MissionData == null)
		{
			Debug.LogWarning("No mission data for " + mapMissionModel.MissionId);
		}
		GameObject gameObject = null;
		bool flag = false;
		if (mapMissionModel != null && !mapMissionModel.IsInWeeklySurvival)
		{
			Rewards storyMissionRewards = mapMissionModel.GetStoryMissionRewards();
			flag = storyMissionRewards != null && storyMissionRewards.GetTotalCurrencyRewardAmount(CurrencyType.DarylToken) > 0;
		}
		if (mapMissionModel.IsInWeeklyChallenge || mapMissionModel.IsInApocalyptiWeeklyChallenge)
		{
			if (mapMissionModel.IsMasterMission)
			{
				return mapVisualData.MissionIconPrefabChallengeMaster;
			}
			return mapVisualData.MissionIconPrefabChallenge;
		}
		if (mapMissionModel.IsInWeeklySurvival)
		{
			if (isLastOrLarge)
			{
				return mapVisualData.MissionIconPrefabSurvivalLarge;
			}
			return mapVisualData.MissionIconPrefabSurvival;
		}
		if (mapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season && isLastOrLarge)
		{
			return mapVisualData.MissionIconPrefabTrial;
		}
		if (isLastOrLarge)
		{
			return mapVisualData.MissionIconPrefabFinalEpisode;
		}
		if ((mapMissionModel.MissionData != null && mapMissionModel.MissionData.MissionType == MissionType.Rescue) || mapMissionModel.HasStoryMissionRewardOfType(RewardType.Equipment) || mapMissionModel.HasStoryMissionRewardOfSpeedUpToken() || flag || mapMissionModel.MissionSpawnPointGroup.Category == MapCategory.Season)
		{
			return mapVisualData.MissionIconPrefabSpecial;
		}
		return mapVisualData.MissionIconPrefabStory;
	}

	private ScrollableMapResource LoadScrollableMapResource(string name)
	{
		string text = "Map/ScrollableMaps/" + name;
		ScrollableMapResource scrollableMapResource = UnityUtils.LoadFromAssetBundle<ScrollableMapResource>(text, "scriptableobjects");
		if (scrollableMapResource == null)
		{
			Debug.LogError("Could not load resource for scrollable map item: " + name + " (expected resource file name: " + text + ")");
		}
		return scrollableMapResource;
	}

	private void DestroyScrollableBackground()
	{
		if (scrollableBackgroundObjects != null)
		{
			for (int i = 0; i < scrollableBackgroundObjects.Count; i++)
			{
				UnityEngine.Object.Destroy(scrollableBackgroundObjects[i]);
			}
			scrollableBackgroundObjects.Clear();
		}
		scrollableBackgroundObjects = null;
		scrollableBackgroundPathItems = null;
	}

	private bool LoadScrollableBackground(string name, UIWidget scrollableMapSlotReference)
	{
		scrollableMapScale = new Vector2(scrollableMapSlotReference.width, scrollableMapSlotReference.height);
		scrollableMapStartDepth = scrollableMapSlotReference.depth;
		if (currentBackground != null)
		{
			UnityEngine.Object.Destroy(currentBackground);
			currentBackground = null;
		}
		DestroyScrollableBackground();
		scrollableBackgroundObjects = new List<GameObject>();
		List<ScrollableMapItemInstance>[] array = new List<ScrollableMapItemInstance>[3]
		{
			new List<ScrollableMapItemInstance>(),
			new List<ScrollableMapItemInstance>(),
			new List<ScrollableMapItemInstance>()
		};
		if (!ScrollableMapHelper.GetMapItems(name, ref array[0], ref array[1], ref array[2]))
		{
			Debug.LogError("Failed to get any scrollable map items for: " + name);
			return false;
		}
		ScrollableMapResource scrollableMapResource = LoadScrollableMapResource("GenericTextureBase");
		if (scrollableMapResource == null)
		{
			Debug.LogError("The GenericTextureBase scrollable map resource asset is missing.");
			return false;
		}
		GameObject prefab = scrollableMapResource.GetPrefab();
		int num = scrollableMapStartDepth;
		for (int i = 0; i < 3; i++)
		{
			if (i == 2)
			{
				continue;
			}
			for (int j = 0; j < array[i].Count; j++)
			{
				ScrollableMapItemInstance scrollableMapItemInstance = array[i][j];
				ScrollableMapResource scrollableMapResource2 = LoadScrollableMapResource(scrollableMapItemInstance.Item.Tex);
				if (scrollableMapResource2 == null)
				{
					return false;
				}
				GameObject gameObject = null;
				gameObject = (string.IsNullOrEmpty(scrollableMapResource2.PrefabName) ? Helpers.InstantiateToParentAndLayer(prefab, backgroundContainer) : Helpers.InstantiateToParentAndLayer(scrollableMapResource2.GetPrefab(), backgroundContainer));
				if (gameObject == null)
				{
					Debug.LogError("Scrollable map prefab instantiation failure for: " + scrollableMapItemInstance.Item.Tex);
					return false;
				}
				UITexture component = gameObject.GetComponent<UITexture>();
				if (!string.IsNullOrEmpty(scrollableMapResource2.TextureName))
				{
					if (component != null)
					{
						component.material = new Material(component.material);
						component.mainTexture = scrollableMapResource2.GetTexture();
					}
					else
					{
						Debug.LogError("Scrollable map resource specified a texture, but the prefab in the map resource has no UITexture to apply it to: " + scrollableMapItemInstance.Item.Tex);
					}
				}
				if (component != null)
				{
					int width = ((scrollableMapResource2.Width != 0) ? scrollableMapResource2.Width : 1024);
					int height = ((scrollableMapResource2.Height != 0) ? scrollableMapResource2.Height : 1024);
					ConvertScrollableMapSizeOnScreen(width, height, out var num2, out var num3);
					component.depth = num;
					float num4 = ((scrollableMapItemInstance.Item.ScaleX != 0f) ? scrollableMapItemInstance.Item.ScaleX : 1f);
					float num5 = ((scrollableMapItemInstance.Item.ScaleY != 0f) ? scrollableMapItemInstance.Item.ScaleY : 1f);
					component.width = (int)(num2 * num4);
					component.height = (int)(num3 * num5);
					if (scrollableMapItemInstance.Item.FlipX != 0 && scrollableMapItemInstance.Item.FlipY != 0)
					{
						component.flip = UIBasicSprite.Flip.Both;
					}
					else if (scrollableMapItemInstance.Item.FlipX != 0)
					{
						component.flip = UIBasicSprite.Flip.Horizontally;
					}
					else if (scrollableMapItemInstance.Item.FlipY != 0)
					{
						component.flip = UIBasicSprite.Flip.Vertically;
					}
					if (scrollableMapItemInstance.Item.TintG != 0 || scrollableMapItemInstance.Item.TintG != 0 || scrollableMapItemInstance.Item.TintB != 0 || scrollableMapItemInstance.Item.Alpha != 0)
					{
						float r = (float)(255 + scrollableMapItemInstance.Item.TintR) / 255f;
						float g = (float)(255 + scrollableMapItemInstance.Item.TintG) / 255f;
						float b = (float)(255 + scrollableMapItemInstance.Item.TintB) / 255f;
						float a = (float)(255 + scrollableMapItemInstance.Item.Alpha) / 255f;
						component.color = new Color(r, g, b, a);
					}
				}
				gameObject.transform.localPosition = ConvertScrollableMapPositionOnScreen(scrollableMapItemInstance.GlobalX, scrollableMapItemInstance.GlobalY);
				num++;
			}
		}
		scrollableBackgroundPathItems = array[2];
		return true;
	}

	private bool LoadBackground(string name)
	{
		DestroyScrollableBackground();
		if (currentBackground != null)
		{
			UnityEngine.Object.Destroy(currentBackground);
			currentBackground = null;
		}
		PrefabResource prefabResource = UnityUtils.LoadFromAssetBundle<PrefabResource>("Map/Backgrounds/" + name, "scriptableobjects");
		if (prefabResource == null)
		{
			Debug.LogError("Could not find map background " + name);
			return false;
		}
		currentBackground = Helpers.InstantiateToParentAndLayer(prefabResource.GetPrefab(), backgroundContainer);
		return true;
	}
}
