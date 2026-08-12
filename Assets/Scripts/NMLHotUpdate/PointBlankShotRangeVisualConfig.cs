using UnityEngine;

[CreateAssetMenu(menuName = "Point Blank Shot Range Visual Configuration", fileName = "PointBlankShotRangeVisualConfig")]
public class PointBlankShotRangeVisualConfig : ScriptableObject
{
	public Color assaultWeaponRangeColor;

	public Color shooterWeaponRangeColor;

	public Color hunterWeaponRangeColor;

	public Color GetColorForShape(WeaponRangeVisualizationShape shape)
	{
		switch (shape)
		{
		case WeaponRangeVisualizationShape.Circle:
			return shooterWeaponRangeColor;
		case WeaponRangeVisualizationShape.Line:
		case WeaponRangeVisualizationShape.BrokenLine:
			return hunterWeaponRangeColor;
		case WeaponRangeVisualizationShape.Sector:
		case WeaponRangeVisualizationShape.BrokenSector:
			return assaultWeaponRangeColor;
		default:
			return Color.green;
		}
	}
}
