using TWDModel;
using UnityEngine;

public class BuildMenu : HUDElement
{
	[SerializeField]
	private BuildMenuListPanel buildingsList;

	public override void Open()
	{
		base.Open();
		if (CampView.Instance != null)
		{
			CampView.Instance.CampViewBuildings.UnselectBuilding();
		}
		if (buildingsList != null)
		{
			buildingsList.SetupCardsByFiltering(BuildingCategory.Building);
		}
	}

	public override void Close()
	{
		UIEvent.Send("OnSurvivorInfoClosed");
		base.Close();
		SingularityMonoBehaviour<HUDManager>.Instance.CloseIfExists(UIType.IngameLoading);
	}

	public override void OnClickClose()
	{
		if (TutorialView.Allowed("Close"))
		{
			Close();
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		UpdateUI();
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "OnBuildingConstructionRequested" && parameter != null)
		{
			BuildingConstructionData buildingConstructionData = parameter as BuildingConstructionData;
			BuildingType buildingType = GameManager.Instance.playerModel.gameEconomyData.GetBuildingType(buildingConstructionData.BuildingType);
			GridPosition initialPosition = CampView.Instance.TransformScreenToGridPosition(new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f), floor: true);
			GameEconomyData gameEconomyData = GameManager.Instance.playerModel.gameEconomyData;
			GridSize size = new GridSize((int)Mathf.Ceil((float)gameEconomyData.ScaleToGrid(buildingType.Size.X) * 0.5f) * 2, (int)Mathf.Ceil((float)gameEconomyData.ScaleToGrid(buildingType.Size.Y) * 0.5f) * 2);
			if (TutorialView.Instance.Running && TutorialView.Instance.BuildingGridPosition != null)
			{
				initialPosition = TutorialView.Instance.BuildingGridPosition;
			}
			GridPosition gridPosition;
			if (buildingConstructionData.BuildingType == "Cage")
			{
				FixedVec2 fixedVec = GameManager.Instance.playerModel.Camp.TransformGroundToGridPosition(new FixedVec2(8L, -1L));
				gridPosition = new GridPosition(fixedVec.X, fixedVec.Y);
			}
			else
			{
				gridPosition = GameManager.Instance.playerModel.Camp.GetFreePositionToPlaceBuilding(initialPosition, size);
			}
			if (gridPosition != null)
			{
				UIEvent.Send("OnBuildingConstructionStartPlacing", parameter);
				BuildingView buildingView = CampView.Instance.CreateBuildingViewWithoutModel(buildingConstructionData.BuildingType, 1, gridPosition);
				CampView.Instance.SetRequestedConstructionBuilding(buildingView);
				CampView.Instance.CameraController.StartPan(buildingView.gameObject.transform.position, CampView.Instance.CameraController.Distance, 0.5f);
				Close();
			}
			else
			{
				HUDNotification.Error(LocalizationManager.GetText("Error.NoBuildingSpace"));
			}
		}
	}
}
