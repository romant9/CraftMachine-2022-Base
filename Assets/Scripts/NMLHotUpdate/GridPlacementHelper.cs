using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class GridPlacementHelper
{
	public static void StripToDirection(Direction direction, List<GridCoordinate> coordinates)
	{
		List<GridCoordinate> list = new List<GridCoordinate>();
		switch (direction)
		{
		case Direction.Up:
		case Direction.Down:
		{
			int num2 = ((direction == Direction.Up) ? int.MaxValue : int.MinValue);
			foreach (GridCoordinate coordinate in coordinates)
			{
				if (direction == Direction.Down && coordinate.Y > num2)
				{
					num2 = coordinate.Y;
				}
				else if (direction == Direction.Up && coordinate.Y < num2)
				{
					num2 = coordinate.Y;
				}
			}
			foreach (GridCoordinate coordinate2 in coordinates)
			{
				if (coordinate2.Y != num2)
				{
					list.Add(coordinate2);
				}
			}
			break;
		}
		case Direction.Right:
		case Direction.Left:
		{
			int num = ((direction == Direction.Left) ? int.MaxValue : int.MinValue);
			foreach (GridCoordinate coordinate3 in coordinates)
			{
				if (direction == Direction.Right && coordinate3.X > num)
				{
					num = coordinate3.X;
				}
				else if (direction == Direction.Left && coordinate3.X < num)
				{
					num = coordinate3.X;
				}
			}
			foreach (GridCoordinate coordinate4 in coordinates)
			{
				if (coordinate4.X != num)
				{
					list.Add(coordinate4);
				}
			}
			break;
		}
		}
		foreach (GridCoordinate item in list)
		{
			coordinates.Remove(item);
		}
	}

	public static void GetPlacement(Vector3 position, List<GridCoordinate> outCells, List<GridCoordinate[]> outEdges, bool requireCellPlacement = false)
	{
		GridView.ActiveInstance.GetConfiguredCellOrEdge(position, out var outCoordinate, out var edgeNeighborCoordinate);
		if (!edgeNeighborCoordinate.IsValid)
		{
			outCells.Add(outCoordinate);
			return;
		}
		outEdges.Add(new GridCoordinate[2] { outCoordinate, edgeNeighborCoordinate });
	}

	public static void GetPlacement(BoxCollider collider, List<GridCoordinate> outCells, List<GridCoordinate[]> outEdges, bool requireCellPlacement = false)
	{
		if (!collider.gameObject.activeInHierarchy)
		{
			Debug.LogWarning("GetPlacement() called on disabled collider " + collider.gameObject.name);
		}
		GridView activeInstance = GridView.ActiveInstance;
		Vector3 colliderPosition = GetColliderPosition(collider, new Vector3(0.5f, 0.5f, 0.5f));
		HashSet<GridCoordinate> hashSet = new HashSet<GridCoordinate>();
		int num = Mathf.Max(Mathf.RoundToInt(collider.bounds.size.x / activeInstance.ConfiguredCellSize.X), 1);
		int num2 = Mathf.Max(Mathf.RoundToInt(collider.bounds.size.z / activeInstance.ConfiguredCellSize.Y), 1);
		Vector2 configuredCellOffset = activeInstance.GetConfiguredCellOffset(colliderPosition);
		bool num3 = requireCellPlacement || GetCellPlacement(num, num2, configuredCellOffset);
		GridCoordinate configuredCoordinate = activeInstance.GetConfiguredCoordinate(GetColliderPosition(collider, new Vector3(0f, 0f, 0f)));
		GridCoordinate configuredCoordinate2 = activeInstance.GetConfiguredCoordinate(GetColliderPosition(collider, new Vector3(0f, 0f, 1f)));
		GridCoordinate configuredCoordinate3 = activeInstance.GetConfiguredCoordinate(GetColliderPosition(collider, new Vector3(1f, 0f, 0f)));
		GridCoordinate configuredCoordinate4 = activeInstance.GetConfiguredCoordinate(GetColliderPosition(collider, new Vector3(1f, 0f, 1f)));
		GridCoordinate configuredCoordinate5 = activeInstance.GetConfiguredCoordinate(GetColliderPosition(collider, new Vector3(0.5f, 0f, 0.5f)));
		int num4 = Mathf.Min(configuredCoordinate.Y, Mathf.Min(configuredCoordinate2.Y, Mathf.Min(configuredCoordinate3.Y, configuredCoordinate4.Y)));
		int num5 = Mathf.Max(configuredCoordinate.Y, Mathf.Max(configuredCoordinate2.Y, Mathf.Max(configuredCoordinate3.Y, configuredCoordinate4.Y)));
		int num6 = Mathf.Min(configuredCoordinate.X, Mathf.Min(configuredCoordinate2.X, Mathf.Min(configuredCoordinate3.X, configuredCoordinate4.X)));
		int num7 = Mathf.Max(configuredCoordinate.X, Mathf.Max(configuredCoordinate2.X, Mathf.Max(configuredCoordinate3.X, configuredCoordinate4.X)));
		Vector3 vector = new Vector3(0f, 0.2f, 0f);
		if (num3)
		{
			Bounds bounds = new Bounds(collider.center, collider.size);
			for (int i = num4; i <= num5; i++)
			{
				for (int j = num6; j <= num7; j++)
				{
					GridCoordinate gridCoordinate = new GridCoordinate(j, i);
					Vector3 configuredPosition = activeInstance.GetConfiguredPosition(gridCoordinate);
					Vector3 point = collider.transform.worldToLocalMatrix.MultiplyPoint(configuredPosition);
					if (bounds.Contains(point) && activeInstance.IsValidCoordinate(gridCoordinate))
					{
						hashSet.Add(gridCoordinate);
					}
				}
			}
			outCells.AddRange(hashSet);
			if (outCells.Count == 0)
			{
				GridCoordinate configuredCoordinate6 = activeInstance.GetConfiguredCoordinate(colliderPosition);
				if (activeInstance.IsValidCoordinate(configuredCoordinate6))
				{
					outCells.Add(configuredCoordinate6);
				}
			}
			return;
		}
		if (num == 1 && num2 == 1)
		{
			activeInstance.GetConfiguredCellOrEdge(colliderPosition, out var outCoordinate, out var edgeNeighborCoordinate, allowCell: false);
			if (activeInstance.IsValidCoordinate(outCoordinate) || activeInstance.IsValidCoordinate(edgeNeighborCoordinate))
			{
				outEdges.Add(new GridCoordinate[2] { outCoordinate, edgeNeighborCoordinate });
			}
			return;
		}
		if (num == 1)
		{
			int num8 = ((configuredCellOffset.x > 0.5f) ? 1 : (-1));
			for (int k = num4; k <= num5; k++)
			{
				GridCoordinate gridCoordinate2 = new GridCoordinate(configuredCoordinate5.X, k);
				GridCoordinate gridCoordinate3 = new GridCoordinate(configuredCoordinate5.X + num8, k);
				Vector3 vector2 = activeInstance.GetConfiguredPosition(gridCoordinate2) + vector;
				Vector3 vector3 = activeInstance.GetConfiguredPosition(gridCoordinate3) + vector;
				Vector3 vector4 = vector3 - vector2;
				float magnitude = vector4.magnitude;
				vector4.Normalize();
				Vector3 vector5 = new Vector3(0f, 0f, activeInstance.ConfiguredCellSize.Y * 0.2f);
				if ((collider.Raycast(new Ray(vector2, vector4), out var hitInfo, magnitude) | collider.Raycast(new Ray(vector2 + vector5, vector4), out hitInfo, magnitude) | collider.Raycast(new Ray(vector2 - vector5, vector4), out hitInfo, magnitude) | collider.Raycast(new Ray(vector3, -vector4), out hitInfo, magnitude) | collider.Raycast(new Ray(vector3 + vector5, -vector4), out hitInfo, magnitude) | collider.Raycast(new Ray(vector3 - vector5, -vector4), out hitInfo, magnitude)) && (activeInstance.IsValidCoordinate(gridCoordinate2) || activeInstance.IsValidCoordinate(gridCoordinate3)))
				{
					outEdges.Add(new GridCoordinate[2] { gridCoordinate2, gridCoordinate3 });
				}
			}
			return;
		}
		int num9 = ((configuredCellOffset.y > 0.5f) ? 1 : (-1));
		for (int l = num6; l <= num7; l++)
		{
			GridCoordinate gridCoordinate4 = new GridCoordinate(l, configuredCoordinate5.Y);
			GridCoordinate gridCoordinate5 = new GridCoordinate(l, configuredCoordinate5.Y + num9);
			Vector3 vector6 = activeInstance.GetConfiguredPosition(gridCoordinate4) + vector;
			Vector3 vector7 = activeInstance.GetConfiguredPosition(gridCoordinate5) + vector;
			Vector3 vector8 = vector7 - vector6;
			float magnitude2 = vector8.magnitude;
			vector8.Normalize();
			Vector3 vector9 = new Vector3(activeInstance.ConfiguredCellSize.X * 0.2f, 0f, 0f);
			if ((collider.Raycast(new Ray(vector6, vector8), out var hitInfo2, magnitude2) | collider.Raycast(new Ray(vector6 + vector9, vector8), out hitInfo2, magnitude2) | collider.Raycast(new Ray(vector6 - vector9, vector8), out hitInfo2, magnitude2) | collider.Raycast(new Ray(vector7, -vector8), out hitInfo2, magnitude2) | collider.Raycast(new Ray(vector7 + vector9, -vector8), out hitInfo2, magnitude2) | collider.Raycast(new Ray(vector7 - vector9, -vector8), out hitInfo2, magnitude2)) && (activeInstance.IsValidCoordinate(gridCoordinate4) || activeInstance.IsValidCoordinate(gridCoordinate5)))
			{
				outEdges.Add(new GridCoordinate[2] { gridCoordinate4, gridCoordinate5 });
			}
		}
	}

	private static Vector3 GetColliderPosition(BoxCollider collider, Vector3 relativePosition)
	{
		Vector3 vector = relativePosition - new Vector3(0.5f, 0.5f, 0.5f);
		vector.Scale(collider.size);
		Vector3 vector2 = collider.center + vector;
		return collider.transform.localToWorldMatrix.MultiplyVector(vector2) + collider.transform.position;
	}

	private static bool GetCellPlacement(int width, int height, Vector2 uv)
	{
		bool flag = uv.y > 0.25f && uv.y < 0.75f;
		bool flag2 = uv.x > 0.25f && uv.x < 0.75f;
		if (width == 1 && height == 1)
		{
			return flag2 && flag;
		}
		if (width == 1)
		{
			return flag2;
		}
		if (height == 1)
		{
			return flag;
		}
		return true;
	}
}
