using TWDModel;
using UnityEngine;

public class CivilianSpawnPointConfig : PlaceableRunLocationItem
{
	public override TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		GridView gridView = Object.FindObjectOfType<GridView>();
		Vector3 vector = base.transform.position - gridView.transform.position;
		Vector2 vector2 = new Vector2(vector.x / gridView.ConfiguredCellSize.X, (0f - vector.z) / gridView.ConfiguredCellSize.Y);
		GridCoordinate gridCoordinate = new GridCoordinate((int)vector2.x, (int)vector2.y);
		if (!gridView.IsValidCoordinate(gridCoordinate))
		{
			string[] obj = new string[5] { "Civilian spawn point ", base.name, " has invalid spawn position (", null, null };
			GridCoordinate gridCoordinate2 = gridCoordinate;
			obj[3] = gridCoordinate2.ToString();
			obj[4] = ")";
			errors.ReportError(string.Concat(obj));
			return null;
		}
		if (gridView != null)
		{
			CivilianSpawnPoint civilianSpawnPoint = new CivilianSpawnPoint();
			civilianSpawnPoint.Coordinate = gridCoordinate;
			runLocation.AddModelObject(civilianSpawnPoint);
			return civilianSpawnPoint;
		}
		return null;
	}

	private void OnDrawGizmos()
	{
	}

	public bool IsValid()
	{
		return true;
	}
}
