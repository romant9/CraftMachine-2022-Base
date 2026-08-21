using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Client.Connectivity;
using TWDModel;
using UnityEngine;

public class WorldBossPVEDetailBackPopup : HUDElement
{
	[SerializeField]
	private GameObject loadingContainer;

	[Header("Bundle Items List")]
	[SerializeField]
	private NUIScrollableList scrollableList;

	[SerializeField]
	private GameObject titleBG;

	[SerializeField]
	private UILabel titleLabel;

	[SerializeField]
	private UILabel descriptionLabel;

	[SerializeField]
	private UILabel taskTitleLabel;

	[SerializeField]
	private UILabel taskValueLabel;

	[SerializeField]
	private UIButton refreshButton;

	[SerializeField]
	private UILabel refreshButtonLabel;

	[SerializeField]
	private UISprite CaptureSprite;

	private WorldBossGuildFullSnapshot worldBossGuildFullSnapshot;

	private WorldBossModelManager worldBossModelManager;

	private WorldBossCapturePointView capturePointView;

	private const float RefreshCooldownSeconds = 10f;

	private const string RefreshButtonLocalizationKey = "World.Boss.Cell.Refresh";

	private float refreshCooldownRemaining;

	private Coroutine restoreScrollCoroutine;

	private bool shouldShowLoadingOnRequest;

	private readonly List<GameObject> contentsHiddenForLoading = new List<GameObject>();

	public const string defaultItemPrefabName = "WorldBossPVEBack_List_Item";

	private static readonly string[] LoadingHiddenRootNames = new string[6] { "Overlay", "Close_Button", "Bg", "LeftContent", "RightContent", "UpdateBtn" };

	public string CapturePoint { get; private set; }

	public static void OpenPopup(string capturePoint)
	{
		WorldBossPVEDetailBackPopup worldBossPVEDetailBackPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.WorldBossPVEDetailBackPopup) as WorldBossPVEDetailBackPopup;
		if (!(worldBossPVEDetailBackPopup == null))
		{
			worldBossPVEDetailBackPopup.CapturePoint = capturePoint;
			worldBossPVEDetailBackPopup.Open();
		}
	}

	public override void Open()
	{
		refreshCooldownRemaining = 0f;
		shouldShowLoadingOnRequest = true;
		SetLoadingVisible(visible: true);
		base.Open();
		ResetRefreshButtonLabel();
		GetWorldBossFullSnapshot();
	}

	public override void Close()
	{
		RestoreContentHiddenForLoading();
		Helpers.GameObjectSetActive(loadingContainer, value: false);
		base.Close();
	}

	public void GetWorldBossFullSnapshot()
	{
		if (GameManager.Instance.playerModel == null || GameManager.Instance.playerModel.WorldBossModelManager == null)
		{
			SetLoadingVisible(visible: false);
			return;
		}
		WorldBossGetSnapshotRequest worldBossGetSnapshotRequest = null;
		worldBossGetSnapshotRequest = ((!GameManager.Instance.playerModel.WorldBossModelManager.IsOffSeason()) ? new WorldBossGetSnapshotRequest
		{
			GroupId = GameManager.Instance.playerModel.GuildId,
			SeasonId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId(),
			CycleId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentCycleId()
		} : new WorldBossGetSnapshotRequest
		{
			GroupId = GameManager.Instance.playerModel.GuildId,
			SeasonId = GameManager.Instance.playerModel.WorldBossModelManager.GetCurrentSeasonId(),
			CycleId = GameManager.Instance.playerModel.WorldBossModelManager.GetNextCycleId()
		});
		string arg = GameManager.Instance.jsonSerializer.Serialize(worldBossGetSnapshotRequest);
		SetLoadingVisible(visible: true);
		SignalRClient.Instance.RequestCommand("WorldBossFullSnapshot", arg, OnWorldBossFullSnapshotAsync, waitForResponse: true);
	}

	private void OnWorldBossFullSnapshotAsync(string responseJson)
	{
		if (SignalRClient.Instance.HasError || string.IsNullOrEmpty(responseJson))
		{
			SignalRClient.Instance.ClearError();
			SetLoadingVisible(visible: false);
			return;
		}
		worldBossGuildFullSnapshot = GameManager.Instance.jsonSerializer.Deserialize<WorldBossGuildFullSnapshot>(responseJson);
		if (worldBossGuildFullSnapshot == null)
		{
			SetLoadingVisible(visible: false);
			return;
		}
		GameManager.Instance.modelManager.SetWorldBossGuildFullSnapshot(worldBossGuildFullSnapshot);
		if (!IsPopupAlive())
		{
			SetLoadingVisible(visible: false);
			return;
		}
		Dictionary<string, WorldBossCapturePointView> allCapturePointStates = GameManager.Instance.playerModel.WorldBossModelManager.GetAllCapturePointStates();
		if (allCapturePointStates != null && allCapturePointStates.ContainsKey(CapturePoint))
		{
			capturePointView = allCapturePointStates[CapturePoint];
		}
		SetLoadingVisible(visible: false);
		UpdateUI();
	}

	public void OnRefreshButtonClicked()
	{
		if (!(refreshCooldownRemaining > 0f))
		{
			GetWorldBossFullSnapshot();
			StartRefreshCooldown();
		}
	}

	public override void Update()
	{
		base.Update();
		if (!(refreshCooldownRemaining <= 0f))
		{
			refreshCooldownRemaining -= Time.deltaTime;
			if (refreshCooldownRemaining <= 0f)
			{
				refreshCooldownRemaining = 0f;
				EndRefreshCooldown();
			}
			else
			{
				UpdateRefreshButtonLabel();
			}
		}
	}

	private void StartRefreshCooldown()
	{
		refreshCooldownRemaining = 10f;
		if (refreshButton != null)
		{
			refreshButton.isEnabled = false;
		}
		UpdateRefreshButtonLabel();
	}

	private void EndRefreshCooldown()
	{
		ResetRefreshButtonLabel();
	}

	private void ResetRefreshButtonLabel()
	{
		if (refreshButton != null)
		{
			refreshButton.isEnabled = true;
		}
		if (refreshButtonLabel != null)
		{
			refreshButtonLabel.text = LocalizationManager.GetText("World.Boss.Cell.Refresh");
		}
	}

	private void UpdateRefreshButtonLabel()
	{
		if (!(refreshButtonLabel == null))
		{
			int num = Mathf.CeilToInt(refreshCooldownRemaining);
			refreshButtonLabel.text = num + "s";
		}
	}

	public void Awake()
	{
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage += OnWorldBossFullSnapshotChanged;
		}
	}

	private void OnWorldBossFullSnapshotChanged(string message, string type)
	{
		if (IsPopupAlive())
		{
			GetWorldBossFullSnapshot();
		}
	}

	private bool IsPopupAlive()
	{
		if (this != null && base.gameObject != null)
		{
			return base.IsOpen;
		}
		return false;
	}

	private void SetLoadingVisible(bool visible)
	{
		if (visible)
		{
			if (shouldShowLoadingOnRequest)
			{
				HideAllContentForLoading();
				Helpers.GameObjectSetActive(loadingContainer, value: true);
			}
		}
		else
		{
			shouldShowLoadingOnRequest = false;
			RestoreContentHiddenForLoading();
			Helpers.GameObjectSetActive(loadingContainer, value: false);
		}
	}

	private void HideAllContentForLoading()
	{
		if (contentsHiddenForLoading.Count > 0)
		{
			return;
		}
		Transform transform = base.transform;
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject gameObject = transform.GetChild(i).gameObject;
			if ((!(loadingContainer != null) || !(gameObject == loadingContainer)) && gameObject.activeSelf)
			{
				contentsHiddenForLoading.Add(gameObject);
				Helpers.GameObjectSetActive(gameObject, value: false);
			}
		}
	}

	private void RestoreContentHiddenForLoading()
	{
		for (int i = 0; i < contentsHiddenForLoading.Count; i++)
		{
			Helpers.GameObjectSetActive(contentsHiddenForLoading[i], value: true);
		}
		contentsHiddenForLoading.Clear();
		for (int j = 0; j < LoadingHiddenRootNames.Length; j++)
		{
			Transform transform = base.transform.Find(LoadingHiddenRootNames[j]);
			if (transform != null)
			{
				Helpers.GameObjectSetActive(transform.gameObject, value: true);
			}
		}
	}

	private void OnDestroy()
	{
		if (restoreScrollCoroutine != null)
		{
			StopCoroutine(restoreScrollCoroutine);
			restoreScrollCoroutine = null;
		}
		if (SignalRClient.Instance != null)
		{
			SignalRClient.Instance.OnWorldBossFullSnapshotMessage -= OnWorldBossFullSnapshotChanged;
		}
	}

	public override void UpdateUI()
	{
		if (!IsPopupAlive())
		{
			return;
		}
		base.UpdateUI();
		if (scrollableList == null)
		{
			return;
		}
		if (scrollableList.currentItemsCount <= 0)
		{
			_ = Vector3.zero;
		}
		else
		{
			scrollableList.GetCurrentScrollPanelLocalPosition();
		}
		scrollableList.Clear();
		List<WorldBossCellDefinition> list = GameManager.Instance.gameEconomyData.GetWorldBossCellDefinitionsByCapturePoint(CapturePoint).ToList();
		WorldBossCapturePointSnapshot capturePointSnapshot = FindCapturePointSnapshot(CapturePoint);
		SortCellDefinitions(list, capturePointSnapshot);
		UpdateCaptureSprite(CapturePoint);
		WorldBossBattlegroundDefinition worldBossBattlegroundDefinition = GameManager.Instance.gameEconomyData.FindWorldBossBattlegroundDefinitionsByDifficulty(worldBossGuildFullSnapshot.GuildFullState.Difficulty).FirstOrDefault((WorldBossBattlegroundDefinition x) => x.CapturePoint == CapturePoint);
		if (worldBossBattlegroundDefinition != null && capturePointView != null)
		{
			if (titleBG != null)
			{
				UISprite component = titleBG.GetComponent<UISprite>();
				if (component != null)
				{
					if (CheckFinishedTask())
					{
						if (CheckRedAndBlueFlag() == "Red")
						{
							component.color = Helpers.HexToColor("#882F28");
						}
						else
						{
							component.color = Helpers.HexToColor("#3F65B1");
						}
					}
					else
					{
						component.color = Helpers.HexToColor("#525252");
					}
				}
			}
			titleLabel.text = LocalizationManager.GetText(worldBossBattlegroundDefinition.BuildingName);
			descriptionLabel.text = LocalizationManager.GetText(worldBossBattlegroundDefinition.BuildingDoneDesc);
			taskTitleLabel.text = LocalizationManager.GetText("World.Boss.PVE.Quest", capturePointView.ClearedCells, capturePointView.TotalCells);
			if (CheckFinishedTask())
			{
				taskTitleLabel.color = Helpers.HexToColor("#B4E52F");
			}
			taskValueLabel.text = LocalizationManager.GetText("World.Boss.PVEClean", capturePointView.ClearedCells, capturePointView.TotalCells);
			if (refreshCooldownRemaining <= 0f)
			{
				EndRefreshCooldown();
			}
		}
		for (int num = 0; num < list.Count; num += 2)
		{
			WorldBossPVEBackItem worldBossPVEBackItem = scrollableList.InstantiateAdd("WorldBossPVEBack_List_Item") as WorldBossPVEBackItem;
			if (!(worldBossPVEBackItem == null))
			{
				WorldBossCellDefinition cellDefinition = list[num];
				WorldBossCellDefinition worldBossCellDefinition = ((num + 1 < list.Count) ? list[num + 1] : null);
				List<WorldBossPVECellSlotData> list2 = new List<WorldBossPVECellSlotData>(2)
				{
					new WorldBossPVECellSlotData
					{
						CellDefinition = cellDefinition,
						CellStateSnapshot = FindCellState(capturePointSnapshot, cellDefinition)
					}
				};
				if (worldBossCellDefinition != null)
				{
					list2.Add(new WorldBossPVECellSlotData
					{
						CellDefinition = worldBossCellDefinition,
						CellStateSnapshot = FindCellState(capturePointSnapshot, worldBossCellDefinition)
					});
				}
				worldBossPVEBackItem.SetData(list2.ToArray());
			}
		}
		scrollableList.SortAndReset();
	}

	private IEnumerator RestoreScrollPanelLocalPositionAfterLayout(Vector3 savedPanelLocalPosition)
	{
		yield return null;
		restoreScrollCoroutine = null;
		if (!(this == null) && !(scrollableList == null))
		{
			scrollableList.RestoreScrollPanelLocalPosition(savedPanelLocalPosition);
		}
	}

	private WorldBossCapturePointSnapshot FindCapturePointSnapshot(string capturePoint)
	{
		if (worldBossGuildFullSnapshot?.CapturePoints == null || string.IsNullOrEmpty(capturePoint))
		{
			return null;
		}
		for (int i = 0; i < worldBossGuildFullSnapshot.CapturePoints.Count; i++)
		{
			WorldBossCapturePointSnapshot worldBossCapturePointSnapshot = worldBossGuildFullSnapshot.CapturePoints[i];
			if (worldBossCapturePointSnapshot != null && worldBossCapturePointSnapshot.CapturePoint == capturePoint)
			{
				return worldBossCapturePointSnapshot;
			}
		}
		return null;
	}

	public string CheckRedAndBlueFlag()
	{
		if (worldBossGuildFullSnapshot == null)
		{
			return "Red";
		}
		if (!(worldBossGuildFullSnapshot.Match.GroupIdA == GameManager.Instance.playerModel.GuildId))
		{
			return "Red";
		}
		return "Blue";
	}

	public bool CheckFinishedTask()
	{
		if (capturePointView == null)
		{
			return false;
		}
		return capturePointView.State == WorldBossCapturePointState.PveCleared;
	}

	private static void SortCellDefinitions(List<WorldBossCellDefinition> cellDefinitions, WorldBossCapturePointSnapshot capturePointSnapshot)
	{
		if (cellDefinitions != null && cellDefinitions.Count > 1)
		{
			cellDefinitions.Sort(delegate(WorldBossCellDefinition a, WorldBossCellDefinition b)
			{
				int num = GetCellDisplaySortOrder(GetCellStatus(capturePointSnapshot, a)).CompareTo(GetCellDisplaySortOrder(GetCellStatus(capturePointSnapshot, b)));
				return (num != 0) ? num : GetCellDefinitionSortId(a).CompareTo(GetCellDefinitionSortId(b));
			});
		}
	}

	private static int GetCellStatus(WorldBossCapturePointSnapshot capturePointSnapshot, WorldBossCellDefinition cellDefinition)
	{
		return FindCellState(capturePointSnapshot, cellDefinition)?.Status ?? 0;
	}

	private static int GetCellDisplaySortOrder(int status)
	{
		return status switch
		{
			0 => 0, 
			1 => 1, 
			2 => 2, 
			_ => 0, 
		};
	}

	private static int GetCellDefinitionSortId(WorldBossCellDefinition cellDefinition)
	{
		if (cellDefinition == null || string.IsNullOrEmpty(cellDefinition.Cell))
		{
			return int.MaxValue;
		}
		string cell = cellDefinition.Cell;
		int num = cell.LastIndexOf('-');
		if (num >= 0 && num + 1 < cell.Length && int.TryParse(cell.Substring(num + 1), out var result))
		{
			return result;
		}
		return int.MaxValue;
	}

	private static WorldBossCellStateSnapshot FindCellState(WorldBossCapturePointSnapshot capturePointSnapshot, WorldBossCellDefinition cellDefinition)
	{
		if (capturePointSnapshot?.CellStates == null || cellDefinition == null)
		{
			return null;
		}
		for (int i = 0; i < capturePointSnapshot.CellStates.Count; i++)
		{
			WorldBossCellStateSnapshot worldBossCellStateSnapshot = capturePointSnapshot.CellStates[i];
			if (worldBossCellStateSnapshot != null && worldBossCellStateSnapshot.Cell == cellDefinition.Cell)
			{
				return worldBossCellStateSnapshot;
			}
		}
		return null;
	}

	private void UpdateCaptureSprite(string capturePoint)
	{
		if (!(CaptureSprite == null))
		{
			string capturePointSpriteName = WorldBossCaptureBase.GetCapturePointSpriteName(capturePoint);
			if (!string.IsNullOrEmpty(capturePointSpriteName))
			{
				CaptureSprite.spriteName = capturePointSpriteName;
			}
		}
	}
}
