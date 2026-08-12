using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ExitGridAreaVisualization : GridAreaVisualization
{
	public Color EnabledColor;

	public Color DisabledColor;

	public Color EnabledOutlineColor;

	public Color DisabledOutlineColor;

	private GridField<bool> gridField;

	public void Initialize(GridModel gridModel, List<GridCoordinate> exitLocations)
	{
		base.gridModel = gridModel;
		gridField = new GridField<bool>(gridModel.Width, gridModel.Height, defaultValue: false);
		foreach (GridCoordinate exitLocation in exitLocations)
		{
			gridField[exitLocation] = true;
		}
		base.Initialize(gridModel, gridField);
	}

	private void Start()
	{
		base.transform.localPosition = -base.transform.parent.position;
	}
}
