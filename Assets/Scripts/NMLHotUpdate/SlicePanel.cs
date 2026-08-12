using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SlicePanel : ButtonBase
{
	public const string EventNextClicked = "NextClicked";

	public const string EventPrevClicked = "PrevClicked";

	public const string EventHotspotClicked = "HotspotClicked";

	public const string EventSliceClicked = "SliceClicked";

	public const string LeftEdgeBackgroundSpriteName = "Ui_Map_Bg_Left";

	public const string MiddleBackgroundSpriteName = "Ui_Map_Bg_Center";

	public const string RightEdgeBackgroundSpriteName = "Ui_Map_Bg_Right";

	public int EdgeMarginSize = 20;

	public int CellWidth = 100;

	public int CellHeight = 100;

	public GameObject CellContainer;

	public GameObject CellPrefab;

	public GameObject HotspotPrefab;

	public GameObject Shadowplane;

	public UILabel Label;

	public UISprite Background;

	public GameObject NextSliceButton;

	public GameObject PrevSliceButton;

	public GameObject NextPanelButton;

	public GameObject PrevPanelButton;

	private List<OutpostSliceHotspot> Hotspots = new List<OutpostSliceHotspot>();

	public SlicePosition Position { get; set; }

	public bool ShowHotspotBackground { get; set; }

	public bool EnableSliceSelectClick { get; set; }

	private bool UpDownArrowsEnabled { get; set; }

	private bool SidewaysArrowsEnabled { get; set; }

	private OutpostLevelModel LevelModel { get; set; }

	public float Scale { get; set; }

	public RunLocationModel OutpostTemplateModel { get; set; }

	public string SliceViewId { get; set; }

	public event SliceInteractionHandler OnSliceInteraction;

	public static SlicePanel CreateSlicePanel(RunLocationModel outpostTemplateModel, GameObject slicePrefab, GameObject container, float scale, float marginScale, int index, int max, SlicePosition slicePosition, string sliceViewId, OutpostLevelModel levelModel, bool isCurrentEditSlice = false)
	{
		float num = (float)(max - 1) / 2f;
		GameObject gameObject = Object.Instantiate(slicePrefab);
		SlicePanel component = gameObject.GetComponent<SlicePanel>();
		if (component != null)
		{
			gameObject.transform.parent = container.transform;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			component.LevelModel = levelModel;
			component.OutpostTemplateModel = outpostTemplateModel;
			component.Position = slicePosition;
			component.SliceViewId = sliceViewId;
			component.Scale = scale;
			component.ShowHotspotBackground = isCurrentEditSlice;
			component.UpdateSlice();
			Vector2 dimensions = component.GetDimensions();
			dimensions.x *= scale + marginScale;
			dimensions.y *= scale;
			gameObject.transform.localPosition = new Vector3(((float)index - num) * dimensions.x, 0f, 0f);
			component.SetHotspotInteraction(enabled: false);
		}
		else
		{
			Object.Destroy(gameObject);
		}
		return component;
	}

	public void SetLabel(string label)
	{
		if (Label != null)
		{
			Label.text = label;
			Label.gameObject.SetActive(label != null && label.Length > 0);
		}
	}

	public void ShowArrows(bool enabled)
	{
		UpDownArrowsEnabled = enabled;
		if (NextSliceButton != null)
		{
			NextSliceButton.SetActive(enabled);
		}
		if (PrevSliceButton != null)
		{
			PrevSliceButton.SetActive(enabled);
		}
	}

	public void ShowSidewaysArrows(bool enabled)
	{
		SidewaysArrowsEnabled = enabled;
		bool active = false;
		bool active2 = false;
		if (!SidewaysArrowsEnabled)
		{
			active = false;
			active2 = false;
		}
		else if (Position == SlicePosition.First)
		{
			active = true;
		}
		else if (Position == SlicePosition.Second)
		{
			active = true;
			active2 = true;
		}
		else if (Position == SlicePosition.Third)
		{
			active2 = true;
		}
		NextPanelButton.SetActive(active);
		PrevPanelButton.SetActive(active2);
	}

	public void OnNextClicked()
	{
		NotifySliceClicked(null, "NextClicked");
	}

	public void OnPrevClicked()
	{
		NotifySliceClicked(null, "PrevClicked");
	}

	private void NotifySliceClicked(OutpostSliceHotspot hotspot, string eventId)
	{
		this.OnSliceInteraction?.Invoke(this, hotspot, eventId);
	}

	public void DeselectAllHotspots()
	{
		for (int i = 0; i < Hotspots.Count; i++)
		{
			Hotspots[i].SetSelected(selected: false);
		}
	}

	public void SelectHotspot(OutpostSliceHotspot hotspot)
	{
		for (int i = 0; i < Hotspots.Count; i++)
		{
			if (Hotspots[i] != null)
			{
				if (hotspot != null && Hotspots[i] == hotspot)
				{
					Hotspots[i].SetSelected(selected: true);
				}
				else
				{
					Hotspots[i].SetSelected(selected: false);
				}
			}
		}
	}

	public void HighlightAllThatCanAccept(OutpostDeploymentMarker marker)
	{
		for (int i = 0; i < Hotspots.Count; i++)
		{
			if (Hotspots[i] != null)
			{
				if (marker != null && marker.CanPlaceOnTarget(Hotspots[i]))
				{
					Hotspots[i].SetHighlight(value: true);
				}
				else
				{
					Hotspots[i].SetHighlight(value: false);
				}
			}
		}
	}

	public Vector2 GetDimensions()
	{
		return new Vector2(CellWidth * OutpostTemplateModel.Grid.Width / 3, CellHeight * OutpostTemplateModel.Grid.Height);
	}

	private OutpostSliceCell CreateSliceCell(int x, int y)
	{
		if (CellPrefab != null && CellContainer != null)
		{
			GameObject gameObject = Object.Instantiate(CellPrefab);
			if (gameObject != null)
			{
				gameObject.transform.parent = CellContainer.transform;
				gameObject.transform.localPosition = new Vector3(x * CellWidth + CellWidth / 2, -y * CellHeight - CellHeight / 2, 0f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				OutpostSliceCell component = gameObject.GetComponent<OutpostSliceCell>();
				if (component != null)
				{
					component.Depth = y;
				}
				return component;
			}
		}
		return null;
	}

	public void SetHotspotInteraction(bool enabled)
	{
		for (int i = 0; i < Hotspots.Count; i++)
		{
			if (Hotspots[i] != null)
			{
				Hotspots[i].EnableColliders(enabled);
			}
		}
	}

	private void OnHotspotClicked(OutpostSliceHotspot hotspot)
	{
		NotifySliceClicked(hotspot, "HotspotClicked");
	}

	private OutpostSliceHotspot CreateSliceHotspot(int x, int y, int width, int height, OutpostHotspotModel hotspotModel)
	{
		if (HotspotPrefab != null && CellContainer != null)
		{
			GameObject gameObject = Object.Instantiate(HotspotPrefab);
			if (gameObject != null)
			{
				gameObject.transform.parent = CellContainer.transform;
				gameObject.transform.localPosition = new Vector3(((float)x + (float)(width - 1) / 2f) * (float)CellWidth + (float)(CellWidth / 2), (0f - ((float)y + (float)(height - 1) / 2f)) * (float)CellHeight - (float)(CellHeight / 2), 0f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				HotspotInfo hotspotInfo = LevelModel.FindHotspotInfo(hotspotModel.ViewId);
				OutpostSliceHotspot component = gameObject.GetComponent<OutpostSliceHotspot>();
				if (component != null)
				{
					component.GetComponent<UIWidget>().SetDimensions(width * CellWidth, height * CellHeight);
					component.Set(hotspotModel);
					component.OnHotspotClicked += OnHotspotClicked;
					component.Background.gameObject.SetActive(ShowHotspotBackground);
					if (hotspotInfo != null)
					{
						component.SetState(hotspotInfo.State, hotspotInfo.WalkerType, hotspotInfo.Count);
					}
				}
				return component;
			}
		}
		return null;
	}

	private void SetSliceDimensions(int width, int height)
	{
		UIWidget component = GetComponent<UIWidget>();
		if (component != null)
		{
			component.SetDimensions((int)((float)(width * CellWidth) * Scale), (int)((float)(height * CellHeight) * Scale));
			if (CellContainer != null)
			{
				CellContainer.transform.localScale = new Vector3(Scale, Scale, 1f);
			}
			if (Shadowplane != null)
			{
				Shadowplane.transform.localScale = new Vector3(Scale * 100f, Scale * 100f, Scale * 100f);
			}
		}
	}

	public void OnEnable()
	{
		UpdateSlice();
	}

	public void SetViewId(string viewId)
	{
		SliceViewId = viewId;
		UpdateSlice();
	}

	public void UpdateSlice()
	{
		ShowArrows(UpDownArrowsEnabled);
		ShowSidewaysArrows(SidewaysArrowsEnabled);
		DeleteCellSprites();
		if (OutpostTemplateModel == null)
		{
			return;
		}
		int num = OutpostTemplateModel.Grid.Width / 3;
		int height = OutpostTemplateModel.Grid.Height;
		int num2 = (int)Position * num;
		new GridCoordinate(0, 0);
		SetSliceDimensions(num, height);
		List<GridCoordinate> list = new List<GridCoordinate>();
		List<OutpostHotspotModel> hotspotModels = OutpostTemplateModel.GetSliceModel(SliceViewId).GetHotspotModels();
		for (int i = 0; i < hotspotModels.Count; i++)
		{
			OutpostHotspotModel outpostHotspotModel = hotspotModels[i];
			if (outpostHotspotModel.Type == HotspotType.MultiActor)
			{
				continue;
			}
			GridCoordinate position = outpostHotspotModel.Position;
			int width = 0;
			int height2 = 0;
			outpostHotspotModel.GetDimensions(out width, out height2);
			OutpostSliceHotspot outpostSliceHotspot = CreateSliceHotspot(position.X - num2, position.Y, width, height2, outpostHotspotModel);
			_ = outpostSliceHotspot.HotspotModel.State;
			_ = 5;
			Hotspots.Add(outpostSliceHotspot);
			for (int j = position.Y; j < position.Y + height2; j++)
			{
				for (int k = position.X; k < position.X + width; k++)
				{
					list.Add(new GridCoordinate(k, j));
				}
			}
		}
		List<GridCoordinate> outpostObjectiveLocations = OutpostTemplateModel.GetOutpostObjectiveLocations(SliceViewId, OutpostObjectiveType.Flag);
		List<GridCoordinate> outpostObjectiveLocations2 = OutpostTemplateModel.GetOutpostObjectiveLocations(SliceViewId, OutpostObjectiveType.ResourceContainer);
		for (int l = 0; l < num; l++)
		{
			for (int m = 0; m < height; m++)
			{
				GridCoordinate gridCoordinate = new GridCoordinate(l + num2, m);
				if (outpostObjectiveLocations.Contains(gridCoordinate))
				{
					OutpostSliceCell outpostSliceCell = CreateSliceCell(l, m);
					if (outpostSliceCell != null)
					{
						outpostSliceCell.Set(0, 0, 0, 0, isStartLocation: false, isThreatSpawn: false, isFlagPosition: true, isResourceContainerPosition: false);
					}
					continue;
				}
				if (outpostObjectiveLocations2.Contains(gridCoordinate))
				{
					OutpostSliceCell outpostSliceCell2 = CreateSliceCell(l, m);
					if (outpostSliceCell2 != null)
					{
						outpostSliceCell2.Set(0, 0, 0, 0, isStartLocation: false, isThreatSpawn: false, isFlagPosition: false, isResourceContainerPosition: true);
					}
					continue;
				}
				OutpostSliceCell outpostSliceCell3 = CreateSliceCell(l, m);
				if (outpostSliceCell3 != null)
				{
					int moveBlockedBits = OutpostTemplateModel.GetMoveBlockedBits(gridCoordinate, SliceViewId, staticBits: true);
					int visibilityBlockedBits = OutpostTemplateModel.GetVisibilityBlockedBits(gridCoordinate, SliceViewId, staticBits: true);
					int dynamicMoveBlockedBits = 0;
					int dynamicVisibilityBlockedBits = 0;
					outpostSliceCell3.Set(moveBlockedBits, visibilityBlockedBits, dynamicMoveBlockedBits, dynamicVisibilityBlockedBits, isStartLocation: false, isThreatSpawn: false, isFlagPosition: false, isResourceContainerPosition: false);
				}
			}
		}
		List<GridCoordinate> outpostStartLocations = OutpostTemplateModel.GetOutpostStartLocations();
		for (int n = 0; n < outpostStartLocations.Count; n++)
		{
			GridCoordinate gridCoordinate2 = outpostStartLocations[n];
			if (gridCoordinate2.X >= num2 && gridCoordinate2.X < num2 + num)
			{
				OutpostSliceCell outpostSliceCell4 = CreateSliceCell(gridCoordinate2.X - num2, gridCoordinate2.Y);
				if (outpostSliceCell4 != null)
				{
					outpostSliceCell4.Set(0, 0, 0, 0, isStartLocation: true, isThreatSpawn: false, isFlagPosition: false, isResourceContainerPosition: false);
				}
			}
		}
		if (Background != null)
		{
			if (Position == SlicePosition.First)
			{
				Background.spriteName = "Ui_Map_Bg_Left";
				Background.SetAnchor(Background.leftAnchor.target.gameObject, -EdgeMarginSize, Background.bottomAnchor.absolute, 0, Background.topAnchor.absolute);
			}
			else if (Position == SlicePosition.Second)
			{
				Background.spriteName = "Ui_Map_Bg_Center";
				Background.SetAnchor(Background.leftAnchor.target.gameObject, 0, Background.bottomAnchor.absolute, 0, Background.topAnchor.absolute);
			}
			else if (Position == SlicePosition.Third)
			{
				Background.spriteName = "Ui_Map_Bg_Right";
				Background.SetAnchor(Background.leftAnchor.target.gameObject, 0, Background.bottomAnchor.absolute, EdgeMarginSize, Background.topAnchor.absolute);
			}
		}
	}

	private void DeleteCellSprites()
	{
		if (!(CellContainer != null))
		{
			return;
		}
		Hotspots.Clear();
		foreach (Transform item in CellContainer.transform)
		{
			OutpostSliceHotspot component = item.gameObject.GetComponent<OutpostSliceHotspot>();
			if ((bool)component)
			{
				component.OnHotspotClicked -= OnHotspotClicked;
			}
			Object.Destroy(item.gameObject);
		}
	}

	public void OnDisable()
	{
		DeleteCellSprites();
	}

	public void OnSliceClicked()
	{
		if (EnableSliceSelectClick)
		{
			NotifySliceClicked(null, "SliceClicked");
		}
	}

	private static bool HasBit(int bits, int bitIndex)
	{
		return (bits & (1 << bitIndex)) != 0;
	}

	private static void SetBit(ref int bits, int bitIndex)
	{
		bits |= 1 << bitIndex;
	}
}
