using System.Collections.Generic;
using ClipperLib;
using UnityEngine;

public class PolyClipping
{
	private static int ScaleMultiplier = 65536;

	public static void IntersectAndClip(List<List<List<Vector2>>> polygonSets, List<Vector2> clipBounds, List<List<Vector2>> outPolygons)
	{
		Clipper clipper = new Clipper();
		int count = polygonSets.Count;
		Queue<List<List<IntPoint>>> queue = new Queue<List<List<IntPoint>>>();
		for (int i = 0; i < count; i++)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>();
			for (int j = 0; j < polygonSets[i].Count; j++)
			{
				List<IntPoint> item = ToIntPoints(polygonSets[i][j]);
				list.Add(item);
			}
			queue.Enqueue(list);
		}
		if (queue.Count > 1)
		{
			do
			{
				List<List<IntPoint>> ppg = queue.Dequeue();
				List<List<IntPoint>> ppg2 = queue.Dequeue();
				clipper.Clear();
				clipper.AddPaths(ppg, PolyType.ptSubject, closed: true);
				clipper.AddPaths(ppg2, PolyType.ptClip, closed: true);
				List<List<IntPoint>> list2 = new List<List<IntPoint>>();
				clipper.Execute(ClipType.ctIntersection, list2, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
				queue.Enqueue(list2);
			}
			while (queue.Count > 1);
		}
		List<List<IntPoint>> ppg3 = queue.Dequeue();
		clipper.Clear();
		clipper.AddPaths(ppg3, PolyType.ptSubject, closed: true);
		clipper.AddPath(ToIntPoints(clipBounds), PolyType.ptClip, Closed: true);
		List<List<IntPoint>> list3 = new List<List<IntPoint>>();
		clipper.Execute(ClipType.ctIntersection, list3, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
		for (int k = 0; k < list3.Count; k++)
		{
			outPolygons.Add(ToVector2s(list3[k]));
		}
	}

	private static IntPoint ToIntPoint(Vector2 v)
	{
		return new IntPoint((int)(v.x * (float)ScaleMultiplier), (int)(v.y * (float)ScaleMultiplier));
	}

	private static List<IntPoint> ToIntPoints(List<Vector2> v)
	{
		List<IntPoint> list = new List<IntPoint>(v.Count);
		for (int i = 0; i < v.Count; i++)
		{
			list.Add(ToIntPoint(v[i]));
		}
		return list;
	}

	private static Vector2 ToVector2(IntPoint p)
	{
		return new Vector2((float)p.X / (float)ScaleMultiplier, (float)p.Y / (float)ScaleMultiplier);
	}

	private static List<Vector2> ToVector2s(List<IntPoint> p)
	{
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < p.Count; i++)
		{
			list.Add(ToVector2(p[i]));
		}
		return list;
	}
}
