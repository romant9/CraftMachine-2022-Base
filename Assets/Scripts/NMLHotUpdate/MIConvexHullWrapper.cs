using System.Collections.Generic;
using MIConvexHull;
using UnityEngine;

public class MIConvexHullWrapper
{
	private class MIVector2 : IVertex
	{
		public double[] Position { get; set; }

		public double x => Position[0];

		public double y => Position[1];

		public MIVector2(double x, double y)
		{
			Position = new double[2] { x, y };
		}

		public Vector2 ToVector2()
		{
			return new Vector2((float)Position[0], (float)Position[1]);
		}

		public Vector3 ToVector3()
		{
			return new Vector3((float)Position[0], 0f, (float)Position[1]);
		}
	}

	private class Indexer
	{
		private List<MIVector2> vertices;

		public Indexer()
		{
			vertices = new List<MIVector2>();
		}

		public int GetIndexForVertex(MIVector2 v)
		{
			int num = vertices.IndexOf(v);
			if (num == -1)
			{
				num = vertices.Count;
				vertices.Add(v);
			}
			return num;
		}

		public Vector2[] ToVector2Array()
		{
			Vector2[] array = new Vector2[vertices.Count];
			for (int i = 0; i < vertices.Count; i++)
			{
				array[i] = vertices[i].ToVector2();
			}
			return array;
		}

		public Vector3[] ToVector3Array()
		{
			Vector3[] array = new Vector3[vertices.Count];
			for (int i = 0; i < vertices.Count; i++)
			{
				array[i] = vertices[i].ToVector3();
			}
			return array;
		}
	}

	public static int[] Triangulate(ref Vector2[] pointsInOut)
	{
		MIVector2[] array = new MIVector2[pointsInOut.Length];
		for (int i = 0; i < pointsInOut.Length; i++)
		{
			array[i] = new MIVector2(pointsInOut[i].x, pointsInOut[i].y);
		}
		IEnumerable<DefaultTriangulationCell<MIVector2>> cells = DelaunayTriangulation<MIVector2, DefaultTriangulationCell<MIVector2>>.Create(array).Cells;
		List<int> list = new List<int>();
		Indexer indexer = new Indexer();
		foreach (DefaultTriangulationCell<MIVector2> item in cells)
		{
			for (int j = 0; j < 3; j++)
			{
				list.Add(indexer.GetIndexForVertex(item.Vertices[j]));
			}
		}
		int[] result = list.ToArray();
		pointsInOut = indexer.ToVector2Array();
		return result;
	}
}
