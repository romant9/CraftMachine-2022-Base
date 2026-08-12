using TWDModel;
using UnityEngine;

public class StartingLocationConfig : PlaceableRunLocationItem
{
	public int ActorTagHash;

	public int Order = -1;

	public override bool ShouldReturnModel => false;

	public override TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		GridView gridView = Object.FindObjectOfType<GridView>();
		Vector3 vector = base.transform.position - gridView.transform.position;
		Vector2 vector2 = new Vector2(vector.x / gridView.ConfiguredCellSize.X, (0f - vector.z) / gridView.ConfiguredCellSize.Y);
		GridCoordinate gridCoordinate = new GridCoordinate((int)vector2.x, (int)vector2.y);
		if (gridCoordinate.X >= 0 && gridCoordinate.Y >= 0 && gridCoordinate.X < gridView.ConfiguredWidth && gridCoordinate.Y < gridView.ConfiguredHeight)
		{
			CombatStartLocationModel obj = new CombatStartLocationModel(gridCoordinate, ActorTagHash, Order);
			runLocation.AddModelObject(obj);
		}
		else
		{
			string[] obj2 = new string[5] { "Start Location ", base.name, " has invalid position (", null, null };
			GridCoordinate gridCoordinate2 = gridCoordinate;
			obj2[3] = gridCoordinate2.ToString();
			obj2[4] = ")";
			errors.ReportError(string.Concat(obj2));
		}
		return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawIcon(base.transform.position, "Icon_StartLocation");
	}
}
