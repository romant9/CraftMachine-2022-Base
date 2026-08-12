using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CampViewBuildings : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Arrows that are shown when the building is selected to show that you can move the building")]
	private GameObject modelBuildingArrowsPrefab;

	[SerializeField]
	[Tooltip("The prefab placed below a building when you move the building. Shows if you can place the building there.")]
	private GameObject placeBuildingPrefab;

	[SerializeField]
	[Tooltip("Material for the place building object when you can place")]
	private Material placeBuildingGreenMaterial;

	[SerializeField]
	[Tooltip("Material for the place building object when you CANNOT place")]
	private Material placeBuildingRedMaterial;

	[SerializeField]
	[Tooltip("Material for other buildings' outline when moving a building")]
	private Material buildingOutlineMaterial;

	private GameObject moveBuildingArrows;

	private GameObject placeBuildingIndicator;

	private bool moved;

	private GridPosition moveGridPos;

	private GridPosition moveGridOffset;

	private GridPosition moveOriginalPosition;

	private CampView campView;

	private BuildingMenu buildingMenu;

	public bool Moving { get; private set; }

	public BuildingView SelectedBuilding { get; private set; }

	public List<BuildingView> Buildings { get; private set; }

	private void Awake()
	{
		campView = GetComponent<CampView>();
		Buildings = new List<BuildingView>();
	}

	private void OnDestroy()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.OnAnyHudElementClosed -= OnAnyHUDElementClosed;
		UnselectBuilding();
	}

	public void Initialize()
	{
		SingularityMonoBehaviour<HUDManager>.Instance.OnAnyHudElementClosed += OnAnyHUDElementClosed;
	}

	public BuildingView FindBuildingView(BuildingModel model)
	{
		for (int i = 0; i < Buildings.Count; i++)
		{
			if (Buildings[i].Model == model)
			{
				return Buildings[i];
			}
		}
		return null;
	}

	public BuildingView FindBuildingView(int modelId)
	{
		for (int i = 0; i < Buildings.Count; i++)
		{
			if (Buildings[i].Model.ModelId == modelId)
			{
				return Buildings[i];
			}
		}
		return null;
	}

	public BuildingView FindBuildingViewOfType<T>()
	{
		for (int i = 0; i < Buildings.Count; i++)
		{
			if (Buildings[i] != null && Buildings[i] is T)
			{
				return Buildings[i];
			}
		}
		return null;
	}

	public void StartBuildingMove()
	{
		if (TutorialView.Instance.MoveBuildingAllowed())
		{
			moved = false;
			GridPosition gridPosition = campView.TransformScreenToGridPosition(Input.mousePosition, floor: true);
			BuildingView building = GetBuilding(gridPosition);
			if (building != null && building == SelectedBuilding && building.IsMoveable)
			{
				UIEvent.Send("OnBuildingMoveStarted");
				Moving = true;
				moveGridPos = building.BuildingPosition;
				moveGridOffset = new GridPosition(moveGridPos.X - gridPosition.X, moveGridPos.Y - gridPosition.Y);
				moveOriginalPosition = building.BuildingPosition;
				PrepareCampForMovingBuilding();
			}
		}
	}

	public void PrepareCampForMovingBuilding()
	{
		if (buildingMenu != null)
		{
			if (SelectedBuilding.Model != null)
			{
				buildingMenu.OpenForModel(SelectedBuilding.Model);
			}
			buildingMenu.FollowTarget(SelectedBuilding.gameObject);
			buildingMenu.BuildingView = SelectedBuilding;
			buildingMenu.SetMovingBuildingButtons();
		}
		campView.EnableCameraControl(enable: false);
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/building_pick");
		moveGridPos = SelectedBuilding.BuildingPosition;
		UpdatePlaceBuildingIndicator();
		campView.ShowValidAreaGrid();
		for (int i = 0; i < Buildings.Count; i++)
		{
			if (Buildings[i] != SelectedBuilding)
			{
				Buildings[i].ShowOutline(placeBuildingPrefab, buildingOutlineMaterial);
			}
		}
	}

	public void UpdateBuildingMove()
	{
		GridPosition gridPosition = campView.TransformScreenToGridPosition(Input.mousePosition, floor: true);
		GridPosition gridPosition2 = new GridPosition(gridPosition.X + moveGridOffset.X, gridPosition.Y + moveGridOffset.Y);
		if (!(gridPosition2.X != moveGridPos.X) && !(gridPosition2.Y != moveGridPos.Y))
		{
			return;
		}
		moveGridPos = gridPosition2;
		MoveInGrid(SelectedBuilding.transform, SelectedBuilding);
		UpdatePlaceBuildingIndicator();
		moved = true;
		campView.ShowValidAreaGrid();
		for (int i = 0; i < Buildings.Count; i++)
		{
			if (Buildings[i] != SelectedBuilding)
			{
				Buildings[i].ShowOutline(placeBuildingPrefab, buildingOutlineMaterial);
			}
		}
	}

	public void NewBuildingLocationConfirmed()
	{
		EndBuildingMove();
		UnselectBuilding();
	}

	private void EndBuildingMove()
	{
		if (buildingMenu != null)
		{
			buildingMenu.Close();
		}
		if (Moving)
		{
			Moving = false;
			campView.EnableCameraControl(enable: true);
			UIEvent.Send("OnBuildingMoveEnded");
			if (moved)
			{
				MoveBuilding(SelectedBuilding);
				ResetMovingStateGraphics();
			}
		}
	}

	public void RequestNewBuildingCreation()
	{
		if (campView.Model.CanPlaceAtLocation(SelectedBuilding.BuildingType, SelectedBuilding.BuildingSize, moveGridPos, SelectedBuilding.Model))
		{
			ConsumeCurrencyCommandUtils.Execute(new CreateBuildingCommand(GameManager.Instance.playerModel.Camp)
			{
				GridPosition = moveGridPos,
				BuildingType = SelectedBuilding.BuildingType,
				Cashier = GameManager.Instance.playerModel.Camp.GetBuildingUpgradeCashier(SelectedBuilding.BuildingType, 1, instantUpgrade: false)
			}, BuildingCreationCallback);
		}
		else
		{
			HUDNotification.Error(LocalizationManager.GetText("Error.BuildingInvalidLocation"));
		}
	}

	public void BuildingCreationCallback(TWDModelResult result)
	{
		if ((result == TWDModelResult.Cancelled || result == TWDModelResult.NotEnoughCurrency) && SelectedBuilding != null)
		{
			UIEvent.Send("OnBuildingMoveCancelled", SelectedBuilding.Model);
		}
	}

	public Vector3 GetObjectLocation(BuildingModel model)
	{
		GridPosition gridPosition = new GridPosition(model.GridPosition);
		return campView.TransformGridToWorldPosition(gridPosition);
	}

	public void SelectBuilding(GameObject buildingGameObject, bool forcedSelection = false)
	{
		BuildingView buildingView = HelpersBuilding.GetBuildingView(buildingGameObject);
		bool flag = false;
		flag = buildingView.OnSelected(forcedSelection);
		if (!forcedSelection && !TutorialView.Instance.Allow(buildingView.BuildingType))
		{
			return;
		}
		UnselectBuilding();
		if (flag)
		{
			return;
		}
		SelectedBuilding = buildingView;
		if (!flag)
		{
			buildingMenu = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampBuildingMenu) as BuildingMenu;
			buildingMenu.FollowTarget(buildingView.gameObject);
			buildingMenu.BuildingView = buildingView;
			if (buildingView.Model != null)
			{
				buildingMenu.OpenForModel(buildingView.Model);
			}
			else
			{
				buildingMenu.SetMovingBuildingButtons();
			}
		}
		if (!forcedSelection)
		{
			if (!string.IsNullOrEmpty(buildingView.SelectBuildingAudioEvent))
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(buildingView.SelectBuildingAudioEvent);
			}
			else
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(BuildingView.DefaultSelectionAudioEvent);
			}
			EventManager.NotifyClick(buildingView.BuildingType);
		}
		if (SelectedBuilding.IsMoveable)
		{
			moveBuildingArrows = Helpers.InstantiateToParent(modelBuildingArrowsPrefab, SelectedBuilding.gameObject);
		}
	}

	public void CloseBuildingMenu()
	{
		if (buildingMenu != null)
		{
			buildingMenu.Close();
		}
	}

	public void UnselectBuilding()
	{
		if (SelectedBuilding != null)
		{
			ResetMovingStateGraphics();
			SelectedBuilding.OnUnselected();
			SelectedBuilding = null;
			CloseBuildingMenu();
			UIEvent.Send("OnBuildingMoveEnded");
		}
		Moving = false;
	}

	private void ResetMovingStateGraphics()
	{
		if (moveBuildingArrows != null)
		{
			CacheableObject component = moveBuildingArrows.GetComponent<CacheableObject>();
			if (component != null)
			{
				component.Destroy();
			}
			moveBuildingArrows = null;
		}
		DestroyPlaceBuildingIndicator();
		campView.EnableCameraControl(enable: true);
		campView.HideValidAreaGrid();
		for (int i = 0; i < Buildings.Count; i++)
		{
			if (Buildings[i] != null && Buildings[i] != SelectedBuilding)
			{
				Buildings[i].HideOutline();
			}
		}
	}

	private void MoveBuilding(BuildingView buildingView)
	{
		BuildingModel model = buildingView.Model;
		if (model == null)
		{
			return;
		}
		if (campView.Model.CanPlaceBuildingAtLocation(buildingView.Model, moveGridPos))
		{
			if (Helpers.ExecuteCommand(new MoveBuildingCommand(model)
			{
				GridPosition = moveGridPos
			}) != TWDModelResult.OK)
			{
				HUDNotification.Error(LocalizationManager.GetText("Error.BuildingInvalidLocation"));
			}
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/building_place");
		}
		else
		{
			HUDNotification.Error(LocalizationManager.GetText("Error.BuildingInvalidLocation"));
			moveGridPos = moveOriginalPosition;
			MoveInGrid(buildingView.transform, buildingView);
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/building_cannot_place");
		}
	}

	public void CancelBuildingMove()
	{
		if (Moving && SelectedBuilding != null)
		{
			moveGridPos = moveOriginalPosition;
			EndBuildingMove();
			MoveInGrid(SelectedBuilding.transform, SelectedBuilding);
			UnselectBuilding();
		}
	}

	private GameObject CreatePlaceBuildingIndicator(BuildingView building)
	{
		GameObject obj = Helpers.InstantiateToParent(placeBuildingPrefab, building.gameObject);
		Vector3 localScale = obj.transform.localScale;
		localScale.x = (float)building.BuildingSize.X * (float)campView.Model.Grid.CellSize.X;
		localScale.z = (float)building.BuildingSize.Y * (float)campView.Model.Grid.CellSize.Y;
		obj.transform.localScale = localScale;
		return obj;
	}

	private void DestroyPlaceBuildingIndicator()
	{
		if (placeBuildingIndicator != null)
		{
			Object.Destroy(placeBuildingIndicator);
			placeBuildingIndicator = null;
		}
	}

	private void UpdatePlaceBuildingIndicator()
	{
		if (placeBuildingIndicator == null)
		{
			placeBuildingIndicator = CreatePlaceBuildingIndicator(SelectedBuilding);
		}
		placeBuildingIndicator.GetComponentInChildren<Renderer>().material = (campView.Model.CanPlaceAtLocation(SelectedBuilding.BuildingType, SelectedBuilding.BuildingSize, moveGridPos, SelectedBuilding.Model) ? placeBuildingGreenMaterial : placeBuildingRedMaterial);
	}

	private BuildingView GetBuilding(GridPosition gridPosition)
	{
		if (SelectedBuilding != null)
		{
			GridPosition buildingPosition = SelectedBuilding.BuildingPosition;
			GridSize buildingSize = SelectedBuilding.BuildingSize;
			if (IsInside(gridPosition, buildingPosition, buildingSize))
			{
				return SelectedBuilding;
			}
		}
		for (int i = 0; i < Buildings.Count; i++)
		{
			BuildingView buildingView = Buildings[i];
			GridPosition buildingPosition2 = buildingView.BuildingPosition;
			GridSize buildingSize2 = buildingView.BuildingSize;
			if (IsInside(gridPosition, buildingPosition2, buildingSize2))
			{
				return buildingView;
			}
		}
		return null;
	}

	private void MoveInGrid(Transform objectTransform, BuildingView buildingView)
	{
		FixedVec2 cellSize = CampView.Instance.Model.Grid.CellSize;
		objectTransform.localPosition = new Vector3((float)cellSize.X * ((float)moveGridPos.X + (float)(buildingView.BuildingSize.X / 2)), 0f, (float)cellSize.X * ((float)moveGridPos.Y + (float)(buildingView.BuildingSize.Y / 2)));
	}

	private bool IsInside(GridPosition gridPosition, GridPosition pos, GridSize size)
	{
		if (gridPosition.X >= pos.X && gridPosition.X < pos.X + size.X && gridPosition.Y >= pos.Y)
		{
			return gridPosition.Y < pos.Y + size.Y;
		}
		return false;
	}

	public void RemoveBuilding(BuildingModel building)
	{
		BuildingView buildingView = FindBuildingView(building);
		if (buildingView != null)
		{
			RemoveBuildingView(buildingView);
		}
	}

	public void RemoveBuildingView(BuildingView buildingView)
	{
		if (buildingView != null)
		{
			Object.Destroy(buildingView.gameObject);
			Buildings.Remove(buildingView);
		}
	}

	public void UpdateBuildingIndicators()
	{
		for (int i = 0; i < Buildings.Count; i++)
		{
			Buildings[i].UpdateIndicators();
		}
	}

	private void OnAnyHUDElementClosed(HUDElement element, HUDElementConfig hudElementConfig)
	{
		if (!(SelectedBuilding != null) || SingularityMonoBehaviour<HUDManager>.Instance.NumberDialogsOpen != 0 || element.UIType == UIType.CampBuildingMenu || element.UIType == UIType.CampBuildMenu)
		{
			return;
		}
		bool flag = true;
		if (SelectedBuilding.Model is ModelUpgraderBuildingModel && (SelectedBuilding.Model as ModelUpgraderBuildingModel).UpgradedUnseenModel != null)
		{
			flag = false;
		}
		if (flag)
		{
			SelectBuilding(SelectedBuilding.gameObject, forcedSelection: true);
			if (SelectedBuilding != null)
			{
				SelectedBuilding.OnSelected();
			}
		}
	}
}
