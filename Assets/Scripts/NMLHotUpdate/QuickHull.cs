using System.Collections.Generic;
using UnityEngine;

public class QuickHull
{
	public static List<Vector2> GetHullEdgeVertices(List<Vector2> points)
	{
		List<Edge> hull = GetHull(points);
		List<Vector2> list = new List<Vector2>();
		Edge edge = hull[hull.Count - 1];
		hull.RemoveAt(hull.Count - 1);
		list.Add(points[edge.A]);
		list.Add(points[edge.B]);
		int num = edge.B;
		while (true)
		{
			int index = FindEdgeContainingPoint(hull, num);
			edge = hull[index];
			int num2 = ((edge.A == num) ? edge.B : edge.A);
			hull.RemoveAt(index);
			if (hull.Count == 0)
			{
				break;
			}
			list.Add(points[num2]);
			num = num2;
		}
		if (MeshGenerator.Area(list) < 0f)
		{
			list.Reverse();
		}
		return list;
	}

	public static List<Edge> GetHull(List<Vector2> points)
	{
		if (points.Count < 2)
		{
			return null;
		}
		List<Edge> list = new List<Edge>();
		List<int> list2 = new List<int>();
		int num = -1;
		int num2 = -1;
		double num3 = double.MinValue;
		double num4 = double.MaxValue;
		for (int i = 0; i < points.Count; i++)
		{
			bool flag = false;
			for (int j = 0; j < i; j++)
			{
				if (points[j] == points[i])
				{
					flag = true;
				}
			}
			if (!flag)
			{
				list2.Add(i);
			}
			if ((double)points[i].x > num3)
			{
				num3 = points[i].x;
				num2 = i;
			}
			if ((double)points[i].x < num4)
			{
				num4 = points[i].x;
				num = i;
			}
		}
		List<Edge> list3 = new List<Edge>();
		list3.Add(new Edge(num, num2));
		list3.Add(new Edge(num2, num));
		list2.Remove(num);
		list2.Remove(num2);
		while (list3.Count > 0)
		{
			Edge edge = list3[0];
			list3.RemoveAt(0);
			double num5 = -1.0;
			int num6 = -1;
			int num7 = 0;
			for (int k = 0; k < list2.Count; k++)
			{
				int num8 = list2[k];
				Vector2 point = points[num8];
				if (!edge.Contains(num8) && edge.PointInFront(points, point))
				{
					double num9 = edge.DistanceToPoint(points, point);
					if (num9 > num5)
					{
						num5 = num9;
						num6 = num8;
					}
					num7++;
				}
			}
			if (num6 >= 0)
			{
				if (!edge.PointInLine(points, points[num6]))
				{
					for (int l = 0; l < list2.Count; l++)
					{
						int index = list2[l];
						if (PointInTriangle(points[index], points[edge.B], points[edge.A], points[num6]))
						{
							list2.RemoveAt(l);
							l--;
							num7--;
						}
					}
				}
				if (num7 == 0)
				{
					list.Add(new Edge(edge.A, num6));
					list.Add(new Edge(num6, edge.B));
				}
				else
				{
					list3.Add(new Edge(edge.A, num6));
					list3.Add(new Edge(num6, edge.B));
				}
			}
			else
			{
				list.Add(new Edge(edge.A, edge.B));
			}
		}
		return list;
	}

	protected static int FindEdgeContainingPoint(List<Edge> edges, int pointIndex)
	{
		for (int i = 0; i < edges.Count; i++)
		{
			if (edges[i].A == pointIndex || edges[i].B == pointIndex)
			{
				return i;
			}
		}
		return -1;
	}

	protected static bool PointInTriangle(Vector2 pt, Vector2 a, Vector2 b, Vector2 c)
	{
		Vector2 rhs = pt - a;
		Vector2 rhs2 = pt - b;
		Vector2 rhs3 = pt - c;
		Vector2 lhs = new Vector2(0f - (b.y - a.y), b.x - a.x);
		Vector2 lhs2 = new Vector2(0f - (c.y - b.y), c.x - b.x);
		Vector2 lhs3 = new Vector2(0f - (a.y - c.y), a.x - c.x);
		if (Vector2.Dot(lhs, rhs) >= 0f && Vector2.Dot(lhs2, rhs2) >= 0f)
		{
			return Vector2.Dot(lhs3, rhs3) >= 0f;
		}
		return false;
	}
}
