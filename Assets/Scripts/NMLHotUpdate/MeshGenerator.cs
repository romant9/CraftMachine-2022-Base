using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator
{
	public enum ExtrudeMode
	{
		InOut = 0,
		Inwards = 1,
		Outwards = 2
	}

	public static void CreateRectangle(Vector3 position, Vector2 size, Vector3[] outVertices, Vector2[] outUVs, int[] outTriangles)
	{
		outVertices[0] = new Vector3(0f - size.x, 0f, 0f - size.y) + position;
		outVertices[1] = new Vector3(size.x, 0f, 0f - size.y) + position;
		outVertices[2] = new Vector3(size.x, 0f, size.y) + position;
		outVertices[3] = new Vector3(0f - size.x, 0f, size.y) + position;
		outUVs[0] = new Vector2(0f, 0f);
		outUVs[1] = new Vector2(1f, 0f);
		outUVs[2] = new Vector2(1f, 1f);
		outUVs[3] = new Vector2(0f, 1f);
		outTriangles[0] = 0;
		outTriangles[1] = 2;
		outTriangles[2] = 1;
		outTriangles[3] = 0;
		outTriangles[4] = 3;
		outTriangles[5] = 2;
	}

	public static void CreateMultiPartRectangle(Vector3 start, Vector3 end, float width, float[] fractions, float[] uvFractions, List<Vector3> outVertices, List<Vector2> outUVs, List<int> outTriangles)
	{
		int num = fractions.Length + 1;
		if (fractions.Length < 2)
		{
			return;
		}
		Vector3 vector = end - start;
		Vector3 lhs = vector;
		lhs.Normalize();
		float magnitude = (end - start).magnitude;
		Vector3 vector2 = -Vector3.Cross(lhs, new Vector3(0f, 1f, 0f)) * (width * 0.5f);
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			float x;
			float x2;
			Vector3 vector3;
			Vector3 vector4;
			if (i == 0)
			{
				x = 0f;
				x2 = uvFractions[i];
				vector3 = start;
				vector4 = start + vector * ((magnitude > 0f) ? Mathf.Clamp(width * fractions[i] / magnitude, 0f, 1f) : 0.5f);
			}
			else if (i == num - 1)
			{
				x = uvFractions[i - 1];
				x2 = 1f;
				vector3 = start + vector * ((magnitude > 0f) ? Mathf.Clamp(1f - width * fractions[i - 1] / magnitude, 0f, 1f) : 0.5f);
				vector4 = end;
			}
			else
			{
				x = uvFractions[i - 1];
				x2 = uvFractions[i];
				vector3 = ((i != 1) ? (start + vector * fractions[i - 1]) : (start + vector * ((magnitude > 0f) ? Mathf.Clamp(width * fractions[i - 1] / magnitude, 0f, 1f) : 0.5f)));
				vector4 = ((i != num - 2) ? (start + vector * fractions[i]) : (start + vector * ((magnitude > 0f) ? Mathf.Clamp(1f - width * fractions[i] / magnitude, 0f, 1f) : 0.5f)));
			}
			outVertices.Add(vector3 + vector2);
			outVertices.Add(vector4 + vector2);
			outVertices.Add(vector4 - vector2);
			outVertices.Add(vector3 - vector2);
			outUVs.Add(new Vector2(x, 0f));
			outUVs.Add(new Vector2(x2, 0f));
			outUVs.Add(new Vector2(x2, 1f));
			outUVs.Add(new Vector2(x, 1f));
			outTriangles.Add(num2);
			outTriangles.Add(num2 + 2);
			outTriangles.Add(num2 + 1);
			outTriangles.Add(num2);
			outTriangles.Add(num2 + 3);
			outTriangles.Add(num2 + 2);
			num2 += 4;
		}
	}

	public static void CreateThickline(List<Vector3> inPoints, List<Vector3> inPointNormals, List<Color> inColors, float thickness, List<Vector3> outVertices, List<Vector2> outUVs, List<int> outTriangles, List<Color> outColors, float textureScale, ExtrudeMode extrudeMode = ExtrudeMode.InOut)
	{
		int num = 0;
		int num2 = 0;
		float num3 = 0f;
		float num4 = 0f;
		Vector3 vector = inPoints[inPoints.Count - 1];
		for (int i = 0; i < inPoints.Count; i++)
		{
			Vector3 vector2 = inPoints[i];
			Vector3 vector3 = inPointNormals[i];
			num4 = num3 + (vector2 - vector).magnitude * textureScale;
			float num5;
			if (num4 > 1f)
			{
				num4 = 1f;
				num5 = 0f;
			}
			else
			{
				num5 = num4;
			}
			Vector3 vector4 = vector3 * thickness * 0.5f;
			Vector3 item = Vector3.zero;
			Vector3 item2 = Vector3.zero;
			switch (extrudeMode)
			{
			case ExtrudeMode.InOut:
				item = vector2 - vector4;
				item2 = vector2 + vector4;
				break;
			case ExtrudeMode.Inwards:
				item = vector2 - 2f * vector4;
				item2 = vector2;
				break;
			case ExtrudeMode.Outwards:
				item = vector2;
				item2 = vector2 + 2f * vector4;
				break;
			}
			outVertices.Add(item);
			outVertices.Add(item2);
			if (i == 0)
			{
				outUVs.Add(new Vector2(num3, 0f));
				outUVs.Add(new Vector2(num3, 1f));
			}
			else
			{
				outUVs.Add(new Vector2(num4, 0f));
				outUVs.Add(new Vector2(num4, 1f));
			}
			if (i > 0)
			{
				outTriangles.Add(num - 2);
				outTriangles.Add(num + 1);
				outTriangles.Add(num);
				outTriangles.Add(num - 2);
				outTriangles.Add(num - 1);
				outTriangles.Add(num + 1);
				num2 += 6;
			}
			if (inColors != null && outColors != null)
			{
				outColors.Add(inColors[i]);
				outColors.Add(inColors[i]);
			}
			num += 2;
			vector = vector2;
			num3 = num5;
		}
	}

	public static void CreateThickline(List<Vector3> inPoints, List<Vector3> inPointNormals, float thickness, List<Vector3> outVertices, List<Vector2> outUVs, List<int> outTriangles, float textureScale, ExtrudeMode extrudeMode = ExtrudeMode.InOut)
	{
		CreateThickline(inPoints, inPointNormals, null, thickness, outVertices, outUVs, outTriangles, null, textureScale, extrudeMode);
	}

	public static void Triangulate(List<Vector2> points, List<int> indices)
	{
		int count = points.Count;
		if (count < 3)
		{
			return;
		}
		List<int> list = new List<int>(points.Count);
		bool flag = Area(points) <= 0f;
		for (int i = 0; i < count; i++)
		{
			list.Add(flag ? (count - 1 - i) : i);
		}
		int num = count;
		int num2 = 2 * num - 1;
		int num3 = 0;
		while (num > 2 && num2 > 0)
		{
			int num4 = num3 % num;
			int num5 = (num3 + 1) % num;
			int num6 = (num3 + 2) % num;
			num3++;
			if (TriangleIsValid(points, num4, num5, num6, num, list))
			{
				indices.Add(list[num4]);
				indices.Add(list[num6]);
				indices.Add(list[num5]);
				for (int j = num5; j < num - 1; j++)
				{
					list[j] = list[j + 1];
				}
				num--;
				num2 = 2 * num - 1;
			}
			else
			{
				num2--;
			}
		}
	}

	public static float Area(List<Vector2> points)
	{
		int count = points.Count;
		float num = 0f;
		for (int i = 0; i < count; i++)
		{
			Vector2 vector = points[(i == 0) ? (count - 1) : (i - 1)];
			Vector2 vector2 = points[i];
			num += vector.x * vector2.y - vector2.x * vector.y;
		}
		return num * 0.5f;
	}

	private static bool TriangleIsValid(List<Vector2> points, int u, int v, int w, int n, List<int> pointIndices)
	{
		Vector2 p = points[pointIndices[u]];
		Vector2 p2 = points[pointIndices[v]];
		Vector2 p3 = points[pointIndices[w]];
		if ((p2.x - p.x) * (p3.y - p.y) - (p2.y - p.y) * (p3.x - p.x) < Mathf.Epsilon)
		{
			return false;
		}
		for (int i = 0; i < n; i++)
		{
			if (i != u && i != v && i != w)
			{
				Vector2 pointToTest = points[pointIndices[i]];
				if (InsideTriangle(p, p2, p3, pointToTest))
				{
					return false;
				}
			}
		}
		return true;
	}

	private static bool InsideTriangle(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 pointToTest)
	{
		float num = p2.x - p1.x;
		float num2 = p2.y - p1.y;
		float num3 = p0.x - p2.x;
		float num4 = p0.y - p2.y;
		float num5 = p1.x - p0.x;
		float num6 = p1.y - p0.y;
		float num7 = pointToTest.x - p0.x;
		float num8 = pointToTest.y - p0.y;
		float num9 = pointToTest.x - p1.x;
		float num10 = pointToTest.y - p1.y;
		float num11 = pointToTest.x - p2.x;
		float num12 = pointToTest.y - p2.y;
		float num13 = num * num10 - num2 * num9;
		float num14 = num5 * num8 - num6 * num7;
		float num15 = num3 * num12 - num4 * num11;
		if (num13 >= 0f && num15 >= 0f)
		{
			return num14 >= 0f;
		}
		return false;
	}
}
