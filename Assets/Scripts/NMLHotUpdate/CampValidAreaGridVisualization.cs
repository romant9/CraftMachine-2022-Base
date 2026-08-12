using TWDModel;
using UnityEngine;

public class CampValidAreaGridVisualization : SimpleGridAreaVisualization
{
	private CampModel campModel;

	private GridModel gridModel;

	private GridField<bool> gridField;

	public void Initialize(CampModel model)
	{
		campModel = model;
		gridModel = model.Grid;
		gridField = new GridField<bool>(gridModel.Width, gridModel.Height, defaultValue: false);
		UpdateGrid();
		base.Initialize(gridModel, gridField);
	}

	public GameObject GetMesh()
	{
		return ShapeFill;
	}

	private void OnDisable()
	{
		UpdateGrid();
		SetGridField(gridField);
	}

	private void UpdateGrid()
	{
		if (gridField != null && gridField.Length != gridModel.Width * gridModel.Height)
		{
			gridField = new GridField<bool>(gridModel.Width, gridModel.Height, defaultValue: false);
		}
		if (gridModel == null || gridModel.Coordinates == null)
		{
			return;
		}
		foreach (GridCoordinate coordinate in gridModel.Coordinates)
		{
			gridField[coordinate] = campModel.IsValidPosition(coordinate.X, coordinate.Y);
		}
	}
}
