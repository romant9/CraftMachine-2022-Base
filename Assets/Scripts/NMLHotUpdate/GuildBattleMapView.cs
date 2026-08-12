using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GuildBattleMapView : MonoBehaviourExtended
{
	[Header("Drag")]
	public UIDraggableCamera DragCamera;

	public UIDragCamera backgroundDrag;

	[Header("Icon and lines")]
	public GameObject IconParent;

	public GuildBattleMapLineAssets LineAssets;

	public const string UIEventZoomIn = "UIEventZoomIn";

	public const string UIEventZoomOut = "UIEventZoomOut";

	public const string UIEventZoomInInstant = "UIEventZoomInInstant";

	public const string UIEventZoomOutInstant = "UIEventZoomOutInstant";

	public const string UIEventMoveCameraTo = "UIEventMoveCameraTo";

	private const string MapAssetFolder = "UI/GuildBattleMapIcons/";

	private Camera cameraRef;

	private MapGrid GridInstance;

	private GuildBattleMapButton currentTarget;

	private GuildBattleMapConfigResourcesMap guildBattleMapConfig;

	private TweenOrthoSize tweenOrthoSize;

	private GvgMapConfig mapConfig;

	private float currentCameraOrthoSize;

	public bool IsCleared
	{
		get
		{
			if (GridInstance?.Grid != null)
			{
				return GridInstance.Grid.Count == 0;
			}
			return true;
		}
	}

	private void Awake()
	{
		DebugIdString = "GuildBattleMapView";
		mapConfig = GameManager.Instance.gameEconomyData.GvgMapConfig;
		UIDragCamera.DragSpeed = mapConfig.CameraMoveSpeed;
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= UIEvents;
		UIEvent.OnUIEvent += UIEvents;
		guildBattleMapConfig = UnityUtils.LoadFromAssetBundle<GuildBattleMapConfigResourcesMap>("GuildBattleMapConfig", "scriptableobjects");
		DragCamera.transform.position = new Vector3(mapConfig.CameraStartX, mapConfig.CameraStartY, DragCamera.transform.position.z);
		if (OfflineManager.IsNoEffects) DragCamera.GetComponent<AmplifyColorEffect>().enabled = false;

	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= UIEvents;
	}

	private void Update()
	{
		if (IsLoadDataManager) NUITouchManagerUpdate();

		if (currentTarget == null && (!IsLoadDataManager ? NUITouchManager.PinchDeltaScale : PinchDeltaScale) != 0f && cameraRef != null)
		{
			if (tweenOrthoSize != null && tweenOrthoSize.enabled)
			{
				tweenOrthoSize.enabled = false;
			}
			float orthographicSize = cameraRef.orthographicSize;
			orthographicSize += mapConfig.PinchZoomSpeed * (!IsLoadDataManager ? NUITouchManager.PinchDeltaScale : PinchDeltaScale);
			cameraRef.orthographicSize = Mathf.Clamp(orthographicSize, mapConfig.MinZoom, mapConfig.MaxZoom);
			currentCameraOrthoSize = cameraRef.orthographicSize;
		}
	}

	private void UIEvents(string type, object parameter)
	{
		switch (type)
		{
		case "UIEventZoomOut":
		case "UIEventZoomOutInstant":
		case "UIEventZoomIn":
		case "UIEventZoomInInstant":
		{
			if (type == "UIEventZoomOut" || type == "UIEventZoomOutInstant")
			{
				currentTarget = null;
			}
			float cameraSize = ((!(type == "UIEventZoomOut") && !(type == "UIEventZoomOutInstant")) ? mapConfig.MinZoom : ((currentCameraOrthoSize == 0f) ? mapConfig.MaxZoom : currentCameraOrthoSize));
			UITweenCameraData uITweenCameraData = new UITweenCameraData(Helpers.staticVector3Zero, cameraSize, type == "UIEventZoomOutInstant" || type == "UIEventZoomInInstant");
			bool zoom = type == "UIEventZoomIn" || type == "UIEventZoomInInstant";
			TweenMapItems(zoom, uITweenCameraData.instant);
			TweenZoom(uITweenCameraData);
			break;
		}
		case "UIEventMoveCameraTo":
			MoveCamera(parameter as UITweenCameraData);
			break;
		}
	}

	private IEnumerator DelayHide(GameObject target, float delay)
	{
		yield return new WaitForSeconds(delay);
		if (target != null)
		{
			TooltipManager.HideAll(target);
		}
	}

	public void UpdateDataReference(GuildBattleModel guildBattleModel, GuildBattleMapMissionModel model)
	{
		TWDGroupChildModelList<GuildBattleMapSectorModel> sectors = guildBattleModel.CurrentMapModel.Sectors;
		GvgMapConfig gvgMapConfig = GameManager.Instance.gameEconomyData.GvgMapConfig;
		if (!IsNotNull(DragCamera, "GuildBattleModel") || !IsNotNull(guildBattleModel.CurrentMapModel, "CurrentMapModel") || !IsNotNull(sectors, "data") || !IsNotNull(gvgMapConfig, "config"))
		{
			return;
		}
		if (GridInstance == null)
		{
			GridInstance = new MapGrid(gvgMapConfig.GridcCellSizeX, gvgMapConfig.GridcCellSizeY);
		}
		for (int i = 0; i < sectors.Count; i++)
		{
			if (sectors[i] == null)
			{
				continue;
			}
			GvgMapIconConfig gvgMapIconConfig = sectors[i].MissionSectorDefinition?.MapIconConfig;
			if (gvgMapIconConfig == null)
			{
				Debug.LogWarning("Could not find Map data with SectorId: " + sectors[i].SectorId);
				continue;
			}
			if (string.IsNullOrEmpty(gvgMapIconConfig.MainPrefabSrc) || string.IsNullOrEmpty(gvgMapIconConfig.ArtPrefabSrc))
			{
				Debug.LogWarning("Could not prefab source for SectorId: " + sectors[i].SectorId);
				continue;
			}
			GuildBattleMapButton guildBattleMapButton = GridInstance.GetAt(gvgMapIconConfig.x, gvgMapIconConfig.y) as GuildBattleMapButton;
			if (!(guildBattleMapButton == null) && guildBattleMapButton.Button != null)
			{
				guildBattleMapButton.Model = sectors[i];
				if (GameManager.Instance.gameEconomyData.GetFeature("GvgEmbelDropOnMap").Enabled)
				{
					guildBattleMapButton.InitEmblems(guildBattleModel);
				}
			}
		}
		GridInstance.PositionItems();
	}

	public void LoadAndPositionIcons(GuildBattleModel guildBattleModel, GuildBattleMapMissionModel model)
	{
		TWDGroupChildModelList<GuildBattleMapSectorModel> sectors = guildBattleModel.CurrentMapModel.Sectors;
		GvgMapConfig gvgMapConfig = GameManager.Instance.gameEconomyData.GvgMapConfig;
		if (!IsNotNull(DragCamera, "GuildBattleModel") || !IsNotNull(guildBattleModel.CurrentMapModel, "CurrentMapModel") || !IsNotNull(DragCamera, "Camera") || !IsNotNull(IconParent, "IconParent") || !IsNotNull(sectors, "data") || !IsNotNull(gvgMapConfig, "config") || !IsNotNull(LineAssets, "LineAsseRefs"))
		{
			return;
		}
		if (GridInstance == null)
		{
			GridInstance = new MapGrid(gvgMapConfig.GridcCellSizeX, gvgMapConfig.GridcCellSizeY);
		}
		cameraRef = DragCamera.GetComponent<Camera>();
		DragCamera.rootForBounds = IconParent.transform;
		cameraRef.transform.localPosition = new Vector3(mapConfig.CameraStartX, mapConfig.CameraStartY, cameraRef.transform.position.z);
		GuildBattleMapButton guildBattleMapButton = null;
		GvgMapIconConfig gvgMapIconConfig = null;
		for (int i = 0; i < sectors.Count; i++)
		{
			if (sectors[i] == null)
			{
				continue;
			}
			gvgMapIconConfig = sectors[i].MissionSectorDefinition?.MapIconConfig;
			if (gvgMapIconConfig == null)
			{
				Debug.LogWarning("Could not find Map data with SectorId: " + sectors[i].SectorId);
			}
			else if (string.IsNullOrEmpty(gvgMapIconConfig.MainPrefabSrc) || string.IsNullOrEmpty(gvgMapIconConfig.ArtPrefabSrc))
			{
				Debug.LogWarning("Could not prefab source for SectorId: " + sectors[i].SectorId);
			}
			else
			{
				if (GridInstance.GetAt(gvgMapIconConfig.x, gvgMapIconConfig.y) != null)
				{
					continue;
				}
				guildBattleMapButton = Helpers.InstantiateFromAssetBundleToParent<GuildBattleMapButton>(gvgMapIconConfig.MainPrefabSrc, HUDElementConfig.BundleName, IconParent);
				if (guildBattleMapButton != null && guildBattleMapButton.Button != null)
				{
					GridInstance.AssignItemTo(guildBattleMapButton, gvgMapIconConfig.x, gvgMapIconConfig.y);
					GameObject gameObject = !IsLoadDataManager ? Helpers.InstantiateFromAssetBundleToParent(gvgMapIconConfig.ArtPrefabSrc.Split('/')[1], HUDElementConfig.BundleName, guildBattleMapButton.gameObject) : null;
					guildBattleMapButton.Model = sectors[i];
					guildBattleMapButton.Button.id = guildBattleMapButton.Model.SectorId.ToString();
					guildBattleMapButton.gameObject.name = guildBattleMapButton.Model.SectorId.ToString();
					guildBattleMapButton.Button.SetClickCallback(OnClickOpenMission);
					guildBattleMapButton.SetLineData(LineAssets);
					guildBattleMapButton.DragCamera.draggableCamera = DragCamera;
					guildBattleMapButton.UITextures = ((gameObject == null) ? new UITexture[0] : gameObject.GetComponentsInChildren<UITexture>());
					if (GameManager.Instance.gameEconomyData.GetFeature("GvgEmbelDropOnMap").Enabled && GuildWarHelper.IsBattleOngoingAndPlayerRegistered())
					{
						guildBattleMapButton.Button.SetOnPressAndHoldCallback(OnPressAndHoldOpenMission);
						guildBattleMapButton.InitEmblems(guildBattleModel);
					}
				}
			}
		}
		if (backgroundDrag != null)
		{
			backgroundDrag.draggableCamera = DragCamera;
		}
		GridInstance.PositionItems();
		SelectSector(FindButtonWithModel(model));
	}

	public GuildBattleMapButton FindButtonWithModel(GuildBattleMapMissionModel model)
	{
		if (GridInstance != null && GridInstance.Grid != null && model != null)
		{
			foreach (KeyValuePair<string, IMapGridItem> item in GridInstance.Grid)
			{
				GuildBattleMapButton guildBattleMapButton = item.Value as GuildBattleMapButton;
				if (!(guildBattleMapButton == null) && guildBattleMapButton.Model != null && guildBattleMapButton.Model.SectorId == model.SectorIdOwner)
				{
					return guildBattleMapButton;
				}
			}
		}
		return null;
	}

	public GuildBattleMapButton FindButtonWithId(string sectorId)
	{
		if (GridInstance != null && GridInstance.Grid != null)
		{
			foreach (KeyValuePair<string, IMapGridItem> item in GridInstance.Grid)
			{
				GuildBattleMapButton guildBattleMapButton = item.Value as GuildBattleMapButton;
				if (!(guildBattleMapButton == null) && guildBattleMapButton.Model != null && guildBattleMapButton.Model.SectorId.ToString() == sectorId)
				{
					return guildBattleMapButton;
				}
			}
		}
		return null;
	}

	public void OnClickOpenMission(UIButtonExtended button)
	{
		if (!(button == null))
		{
			DebugTWD.Log("OnClickOpenMission " + button.id, DebugType.Wars);

			GuildBattleMapButton guildBattleMapButton = FindButtonWithId(button.id);
			if (!(guildBattleMapButton == null))
			{
				SelectSector(guildBattleMapButton);
			}
		}
	}

	public void OnPressAndHoldOpenMission(UIButtonExtended button)
	{
		long milliSecondsLeft = 0L;
		if (!GuildManager.CanCreateGuildMapIndicator(out milliSecondsLeft))
		{
			AlertPopup.ShowPopup(LocalizationManager.GetText("GvG.DropMapEmblem.AlertPopup.Wait.Title"), LocalizationManager.GetText("GvG.DropMapEmblem.AlertPopup.Wait.Desc{SecondsLeft}", Helpers.FormatTime(milliSecondsLeft)), LocalizationManager.GetText("Button.Ok"));
		}
		else
		{
			if (button == null)
			{
				return;
			}
			GuildBattleMapButton guildBattleMapButton = FindButtonWithId(button.id);
			if (!(guildBattleMapButton == null) && guildBattleMapButton.Model != null)
			{
				Vector3 lastWorldPosition = UICamera.lastWorldPosition;
				Vector3 vector = button.gameObject.transform.InverseTransformPoint(lastWorldPosition);
				GuildBattleModel.GuildBattleIndicatorData guildBattleIndicatorData = GuildManager.CreateAndSendGuildMapIndicator(guildBattleMapButton.Model.SectorId, vector);
				if (guildBattleIndicatorData != null)
				{
					guildBattleMapButton.UpdatePlayerEmblems(guildBattleIndicatorData);
				}
			}
		}
	}

	public void SelectSector(GuildBattleMapButton button, bool instant = false)
	{
		if (DragCamera.GetComponent<SpringPosition>() != null)
		{
			DragCamera.GetComponent<SpringPosition>().enabled = false;
		}
		if (button == null)
		{
			cameraRef.orthographicSize = Mathf.Clamp(mapConfig.CameraStartZoom, mapConfig.MinZoom, mapConfig.MaxZoom);
			return;
		}
		UITweenCameraData parameter = new UITweenCameraData(button.transform.position, instant);
		if (button.IsUnlocked())
		{
			currentTarget = button;
			currentTarget.Select();
			if (instant)
			{
				UIEvent.Send("UIEventZoomInInstant");
			}
			else
			{
				UIEvent.Send("UIEventZoomIn");
			}
			UIEvent.Send("UIEventMoveCameraTo", parameter);
		}
		else
		{
			currentTarget = null;
			if (instant)
			{
				UIEvent.Send("UIEventZoomOutInstant");
			}
			else
			{
				cameraRef.orthographicSize = Mathf.Clamp(mapConfig.CameraStartZoom, mapConfig.MinZoom, mapConfig.MaxZoom);
			}
			UIEvent.Send("UIEventMoveCameraTo", parameter);
		}
	}

	public void MoveCamera(UITweenCameraData data)
	{
		if (!(DragCamera == null) && data != null)
		{
			if (data.instant)
			{
				DragCamera.transform.position = data.position;
				return;
			}
			TweenPosition component = DragCamera.gameObject.GetComponent<TweenPosition>();
			component.worldSpace = true;
			Vector3 position = data.position;
			position.z = component.from.z;
			component.from = DragCamera.transform.position;
			component.to = position;
			component.ResetToBeginning();
			component.PlayForward();
		}
	}

	public void TweenZoom(UITweenCameraData data)
	{
		if (!(DragCamera == null) && !(cameraRef == null))
		{
			if (data == null)
			{
				data = new UITweenCameraData(Helpers.staticVector3Zero, mapConfig.MaxZoom, instant: true);
			}
			if (data.instant)
			{
				cameraRef.orthographicSize = data.cameraSize;
				return;
			}
			tweenOrthoSize = Helpers.AddComponent<TweenOrthoSize>(DragCamera.gameObject);
			tweenOrthoSize.from = cameraRef.orthographicSize;
			tweenOrthoSize.to = data.cameraSize;
			tweenOrthoSize.ResetToBeginning();
			tweenOrthoSize.PlayForward();
		}
	}

	public void TweenMapItems(bool zoom, bool instant = false)
	{
		foreach (KeyValuePair<string, IMapGridItem> item in GridInstance.Grid)
		{
			GuildBattleMapButton guildBattleMapButton = item.Value as GuildBattleMapButton;
			if (!(guildBattleMapButton == null))
			{
				guildBattleMapButton.ZoomSelect(zoom, instant);
			}
		}
	}

	public override void Clear()
	{
		base.Clear();
		if (GridInstance != null && GridInstance.Grid != null)
		{
			foreach (KeyValuePair<string, IMapGridItem> item in GridInstance.Grid)
			{
				MapGridItem mapGridItem = item.Value as MapGridItem;
				if (!(mapGridItem == null))
				{
					Helpers.DestroyOrCache(mapGridItem);
				}
			}
			if (GridInstance != null)
			{
				GridInstance.Clear();
			}
		}
		currentTarget = null;
	}

	public Camera GetCamera()
	{
		return cameraRef;
	}

	public bool IsTweening()
	{
		if (tweenOrthoSize == null)
		{
			return false;
		}
		return tweenOrthoSize.isActiveAndEnabled;
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	public static float PinchDelta;
	public static float PinchDeltaScale;
	#endregion

	#region mycode
	private void NUITouchManagerUpdate()
	{
		if (Input.touchCount > 1)
		{
			Touch touch = Input.touches[0];
			Touch touch2 = Input.touches[1];
			Vector2 vector = touch.position - touch.deltaPosition;
			Vector2 vector2 = touch2.position - touch2.deltaPosition;
			float magnitude = (vector - vector2).magnitude;
			float magnitude2 = (touch.position - touch2.position).magnitude;
			PinchDelta = magnitude - magnitude2;
			PinchDeltaScale = magnitude / magnitude2 - 1f;
		}
		else
		{
			if (Input.mouseScrollDelta.magnitude > 0)
			{
				PinchDelta = Input.mouseScrollDelta.magnitude;
				PinchDeltaScale = PinchDelta / 10 * (Input.mouseScrollDelta.y >= 0 ? 1 : -1);
			}
			else
			{
				PinchDelta = 0f;
				PinchDeltaScale = 0f;
			}
		}
	}
	#endregion
}
