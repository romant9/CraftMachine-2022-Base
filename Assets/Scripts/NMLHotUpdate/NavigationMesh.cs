using System;
using System.Collections.Generic;
using UnityEngine;

public class NavigationMesh
{
	private class Edge : IEquatable<Edge>
	{
		public int Index0;

		public int Index1;

		public float Cost;

		public Edge(int index0, int index1, float cost)
		{
			Index0 = ((index0 < index1) ? index0 : index1);
			Index1 = ((index0 < index1) ? index1 : index0);
			Cost = cost;
		}

		public bool Equals(Edge e)
		{
			if (Index0 == e.Index0)
			{
				return Index1 == e.Index1;
			}
			return false;
		}
	}

	private class Waypoint
	{
		public Vector2 Position;

		public List<Edge> edges = new List<Edge>();

		public float currentDistance;

		public Waypoint currentPrevious;

		public Waypoint(Vector2 position)
		{
			Position = position;
		}

		public Vector3 PositionToVector3(float y)
		{
			return Helpers.ToVector3(Position, y);
		}
	}

	public static float curvature = 0.3f;

	private Waypoint[] waypoints;

	private List<Edge> edges = new List<Edge>();

	private int survivorCollidersLayerMask;

	private LinkedList<Waypoint> searchUnvisited = new LinkedList<Waypoint>();

	public void RebuildMesh(Vector2[] points)
	{
		survivorCollidersLayerMask = 1 << LayerMask.NameToLayer("SurvivorColliders");
		List<Vector2> list = new List<Vector2>(points.Length);
		for (int i = 0; i < points.Length; i++)
		{
			if (Physics.OverlapSphere(Helpers.ToVector3(points[i], 1f), 0f, survivorCollidersLayerMask).Length == 0)
			{
				list.Add(points[i]);
			}
		}
		points = list.ToArray();
		int[] array = MIConvexHullWrapper.Triangulate(ref points);
		waypoints = new Waypoint[points.Length];
		for (int j = 0; j < points.Length; j++)
		{
			waypoints[j] = new Waypoint(points[j]);
		}
		edges.Clear();
		for (int k = 0; k < array.Length / 3; k++)
		{
			for (int l = 0; l < 3; l++)
			{
				int num = array[k * 3 + l];
				int num2 = array[k * 3 + (l + 1) % 3];
				Vector3 vector = waypoints[num].PositionToVector3(1f);
				Vector3 vector2 = waypoints[num2].PositionToVector3(1f);
				if (Physics.Linecast(vector, vector2, survivorCollidersLayerMask) || Physics.Linecast(vector2, vector, survivorCollidersLayerMask))
				{
					continue;
				}
				Collider[] array2 = Physics.OverlapSphere(vector, 0f, survivorCollidersLayerMask);
				Collider[] array3 = Physics.OverlapSphere(vector2, 0f, survivorCollidersLayerMask);
				if (array2.Length == 0 || array3.Length == 0 || !(array2[0] == array3[0]))
				{
					Edge item = new Edge(num, num2, Vector2.Distance(points[num], points[num2]));
					if (!edges.Contains(item))
					{
						edges.Add(item);
					}
				}
			}
		}
		for (int m = 0; m < edges.Count; m++)
		{
			Edge edge = edges[m];
			if (!waypoints[edge.Index0].edges.Contains(edge))
			{
				waypoints[edge.Index0].edges.Add(edge);
			}
			if (!waypoints[edge.Index1].edges.Contains(edge))
			{
				waypoints[edge.Index1].edges.Add(edge);
			}
		}
	}

	public Vector2 GetRandomWaypointPosition()
	{
		int num = UnityEngine.Random.Range(0, waypoints.Length);
		if (num >= waypoints.Length)
		{
			return Vector2.zero;
		}
		return waypoints[num].Position;
	}

	public PolylinePath FindPathToRandomWaypoint(Vector2 from)
	{
		Waypoint waypoint = FindNearestWaypointWithLineOfSight(from);
		Waypoint waypoint2 = null;
		PolylinePath path = null;
		int num = 10;
		do
		{
			int num2 = UnityEngine.Random.Range(0, waypoints.Length);
			waypoint2 = waypoints[num2];
			if (waypoint != waypoint2)
			{
				path = FindPath(from, waypoint, waypoint2);
				num--;
			}
		}
		while (path == null && num > 0);
		FixNullPath(ref path, waypoint, waypoint2);
		return path;
	}

	public PolylinePath FindPath(Vector2 from, Vector2 to)
	{
		Waypoint waypoint = FindNearestWaypointWithLineOfSight(from);
		Waypoint waypoint2 = FindNearestWaypoint(to);
		PolylinePath path = FindPath(from, waypoint, waypoint2);
		FixNullPath(ref path, waypoint, waypoint2);
		return path;
	}

	public void CreateDebugMesh(MeshRenderer renderer)
	{
		int[] array = new int[edges.Count * 2];
		for (int i = 0; i < edges.Count; i++)
		{
			array[i * 2] = edges[i].Index0;
			array[i * 2 + 1] = edges[i].Index1;
		}
		Vector3[] array2 = new Vector3[waypoints.Length];
		for (int j = 0; j < waypoints.Length; j++)
		{
			array2[j] = waypoints[j].PositionToVector3(0f);
		}
		Vector2[] uv = new Vector2[array2.Length];
		Mesh mesh = renderer.GetComponent<MeshFilter>().mesh;
		mesh.Clear();
		mesh.vertices = array2;
		mesh.uv = uv;
		mesh.normals = null;
		mesh.colors = null;
		mesh.SetIndices(array, MeshTopology.Lines, 0);
		mesh.RecalculateBounds();
	}

	private PolylinePath FindPath(Vector2 additionalStartPoint, Waypoint waypointFrom, Waypoint waypointTo)
	{
		if (waypointFrom == waypointTo)
		{
			PolylinePath polylinePath = new PolylinePath();
			polylinePath.AddSegment(new LineSegment(inEnd: waypointFrom.PositionToVector3(0f), inStart: Helpers.ToVector3(additionalStartPoint), inUp: Vector3.up));
			return polylinePath;
		}
		searchUnvisited.Clear();
		for (int i = 0; i < waypoints.Length; i++)
		{
			waypoints[i].currentDistance = float.MaxValue;
			waypoints[i].currentPrevious = null;
			searchUnvisited.AddLast(waypoints[i]);
		}
		waypointFrom.currentDistance = 0f;
		Waypoint waypoint = waypointFrom;
		searchUnvisited.Remove(waypoint);
		while (searchUnvisited.Count > 0)
		{
			for (int j = 0; j < waypoint.edges.Count; j++)
			{
				Waypoint neighbour = GetNeighbour(waypoint, waypoint.edges[j]);
				float num = waypoint.currentDistance + waypoint.edges[j].Cost;
				if (num < neighbour.currentDistance)
				{
					neighbour.currentDistance = num;
					neighbour.currentPrevious = waypoint;
				}
			}
			waypoint = null;
			float num2 = float.MaxValue;
			foreach (Waypoint item in searchUnvisited)
			{
				if (item.currentDistance < num2)
				{
					waypoint = item;
					num2 = item.currentDistance;
				}
			}
			if (waypoint == null)
			{
				return null;
			}
			searchUnvisited.Remove(waypoint);
			if (waypoint == waypointTo)
			{
				break;
			}
		}
		List<Waypoint> list = new List<Waypoint>();
		waypoint = waypointTo;
		while (waypoint.currentPrevious != null)
		{
			list.Add(waypoint);
			waypoint = waypoint.currentPrevious;
		}
		if (waypoint != waypointFrom || list.Count == 0)
		{
			return null;
		}
		list.Add(waypointFrom);
		if (Vector2.Distance(waypointFrom.Position, additionalStartPoint) > 0.1f)
		{
			list.Add(new Waypoint(additionalStartPoint));
		}
		list.Reverse();
		PolylinePath polylinePath2 = new PolylinePath();
		if (list.Count == 2)
		{
			Vector3 inStart = list[0].PositionToVector3(0f);
			Vector3 inEnd = list[1].PositionToVector3(0f);
			polylinePath2.AddSegment(new LineSegment(inStart, inEnd, Vector3.up));
			return polylinePath2;
		}
		Vector3[] array = new Vector3[list.Count];
		for (int k = 0; k < list.Count; k++)
		{
			if (k == 0)
			{
				Vector2 vector = list[k + 1].Position - list[k].Position;
				array[k] = new Vector3(vector.x, 0f, vector.y);
			}
			else if (k == list.Count - 1)
			{
				Vector2 vector2 = list[k].Position - list[k - 1].Position;
				array[k] = new Vector3(vector2.x, 0f, vector2.y);
			}
			else
			{
				Vector2 vector3 = list[k + 1].Position - list[k].Position;
				Vector2 vector4 = list[k].Position - list[k - 1].Position;
				Vector2 vector5 = (vector3 + vector4) * 0.5f;
				array[k] = new Vector3(vector5.x, 0f, vector5.y);
			}
			array[k] *= curvature;
		}
		for (int l = 1; l < list.Count; l++)
		{
			Waypoint waypoint2 = list[l - 1];
			Waypoint waypoint3 = list[l];
			Vector3 start = waypoint2.PositionToVector3(0f);
			Vector3 end = waypoint3.PositionToVector3(0f);
			Vector3 startTangent = array[l - 1];
			Vector3 endTangent = array[l];
			CurveSegment segment = new CurveSegment(start, end, startTangent, endTangent, Vector3.up);
			polylinePath2.AddSegment(segment);
		}
		return polylinePath2;
	}

	private Waypoint FindNearestWaypoint(Vector2 position)
	{
		float num = float.MaxValue;
		Waypoint result = null;
		for (int i = 0; i < waypoints.Length; i++)
		{
			float num2 = (position - waypoints[i].Position).SqrMagnitude();
			if (num2 < num)
			{
				result = waypoints[i];
				num = num2;
			}
		}
		return result;
	}

	private int WaypointDistanceComparer(Waypoint a, Waypoint b)
	{
		return a.currentDistance.CompareTo(b.currentDistance);
	}

	private Waypoint FindNearestWaypointWithLineOfSight(Vector2 position)
	{
		List<Waypoint> list = new List<Waypoint>(waypoints);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].currentDistance = (position - list[i].Position).SqrMagnitude();
		}
		list.Sort(WaypointDistanceComparer);
		Vector3 start = Helpers.ToVector3(position, 1f);
		for (int j = 0; j < list.Count; j++)
		{
			if (!Physics.Linecast(start, list[j].PositionToVector3(1f), survivorCollidersLayerMask))
			{
				return list[j];
			}
		}
		if (list.Count <= 0)
		{
			return new Waypoint(position);
		}
		return list[0];
	}

	private Waypoint GetNeighbour(Waypoint waypoint, Edge edge)
	{
		if (waypoints[edge.Index0] == waypoint)
		{
			return waypoints[edge.Index1];
		}
		if (waypoints[edge.Index1] == waypoint)
		{
			return waypoints[edge.Index0];
		}
		return null;
	}

	private void TestAllWaypoints()
	{
		for (int i = 0; i < waypoints.Length - 1; i++)
		{
			for (int j = i + 1; j < waypoints.Length; j++)
			{
				FindPath(waypoints[i].Position, waypoints[i], waypoints[j]);
			}
		}
	}

	private void FixNullPath(ref PolylinePath path, Waypoint from, Waypoint to)
	{
		if (path == null)
		{
			path = new PolylinePath();
			path.AddSegment(new LineSegment(from.PositionToVector3(0f), to.PositionToVector3(0f), Vector3.up));
		}
	}
}
