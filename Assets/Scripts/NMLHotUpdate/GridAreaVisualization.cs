using System;
using System.Collections.Generic;
using System.Linq;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class GridAreaVisualization : MonoBehaviour
{
	public GameObject ShapeOutline;

	public GameObject ShapeFill;

	public bool DoGradientEdge;

	protected GridModel gridModel;

	protected GridField<int> borderFlags;

	public GridAreaSettings gridAreaSettings = new GridAreaSettings();

	private List<PolylinePath> Polylines = new List<PolylinePath>();

	private static List<Vector3> fillVertices = new List<Vector3>(1);

	private static List<Vector3> fillNormals = new List<Vector3>(1);

	private static List<Vector2> fillUvs = new List<Vector2>(1);

	private static List<int> fillTriangles = new List<int>(1);

	private static List<Color> fillColors = new List<Color>(1);

	private static List<Vector3> shapeVertices = new List<Vector3>();

	private static List<Vector2> shapeUvs = new List<Vector2>();

	private static List<int> shapeTriangles = new List<int>();

	private static List<Vector3> pathPoints = new List<Vector3>();

	private static List<Vector3> pathNormals = new List<Vector3>();

	private static List<Vector3> vertices = new List<Vector3>();

	private static List<Vector2> uvs = new List<Vector2>();

	private static List<int> triangles = new List<int>();

	private static List<Vector3> upperPath = new List<Vector3>();

	private static List<Vector3> lowerPath = new List<Vector3>();

	private static List<Vector2> borderUvs = new List<Vector2>();

	private static List<Vector3> borderVertices = new List<Vector3>();

	private static List<Vector3> borderNormals = new List<Vector3>();

	private static List<int> borderTriangles = new List<int>();

	private static List<Color> borderColors = new List<Color>();

	private static List<Vector3> outerPolygon = new List<Vector3>();

	private static List<Vector3> createShapeFillNormals = new List<Vector3>();

	private static List<Vector2> createShapeFillUvs = new List<Vector2>();

	private static List<Vector2> createShapeFillPathPoints2d = new List<Vector2>();

	private static List<int> createShapeFillTriangles = new List<int>();

	private static List<int> createShapeFillDuplicates = new List<int>();

	public virtual void Initialize(GridModel gridModel, GridField<bool> gridField)
	{
		this.gridModel = gridModel;
		borderFlags = new GridField<int>(gridModel.Width, gridModel.Height, 255);
		SetGridField(gridField);
	}

	public void SetGridField(GridField<bool> cellData)
	{
		if (cellData == null || cellData.IsClear)
		{
			ClearAreaVisualization();
			return;
		}
		Mesh mesh = ShapeFill.GetComponent<MeshFilter>().mesh;
		Mesh mesh2 = ShapeOutline.GetComponent<MeshFilter>().mesh;
		Polylines = UpdateAreaVisualization(cellData, borderFlags, gridAreaSettings, gridModel, mesh, mesh2);
	}

	protected virtual void ClearAreaVisualization()
	{
		Mesh mesh = ShapeOutline.GetComponent<MeshFilter>().mesh;
		if (mesh != null)
		{
			mesh.Clear();
			mesh.RecalculateBounds();
		}
		mesh = ShapeFill.GetComponent<MeshFilter>().mesh;
		if (mesh != null)
		{
			mesh.Clear();
			mesh.RecalculateBounds();
		}
	}

	public List<PolylinePath> GetPolyPaths()
	{
		return Polylines;
	}

	protected static List<PolylinePath> UpdateAreaVisualization(GridField<bool> gridCells, GridField<int> borderFlags, GridAreaSettings settings, GridModel gridModel, Mesh fillMesh, Mesh shapeMesh)
	{
		List<Line> list = new List<Line>();
		borderFlags.Clear();
		for (int i = 0; i < gridModel.Height; i++)
		{
			for (int j = 0; j < gridModel.Width; j++)
			{
				GridCoordinate coordinate = new GridCoordinate(j, i);
				if (!gridCells[coordinate])
				{
					continue;
				}
				borderFlags[coordinate] = 0;
				for (int k = 0; k < 8; k += 2)
				{
					GridCoordinate coordinateNeighbor = gridModel.GetCoordinateNeighbor(coordinate, k);
					if (!gridCells[coordinateNeighbor] || !gridModel.IsCoordinateValid(coordinateNeighbor))
					{
						Vector3 vector = new Vector3(0f, 0f, 0f);
						Vector3 vector2 = new Vector3(0f, 0f, 0f);
						switch ((GridNeighborDirections)k)
						{
						case GridNeighborDirections.North:
							vector = new Vector3(-0.5f, 0f, -0.5f);
							vector2 = new Vector3(0.5f, 0f, -0.5f);
							break;
						case GridNeighborDirections.East:
							vector = new Vector3(0.5f, 0f, -0.5f);
							vector2 = new Vector3(0.5f, 0f, 0.5f);
							break;
						case GridNeighborDirections.South:
							vector = new Vector3(0.5f, 0f, 0.5f);
							vector2 = new Vector3(-0.5f, 0f, 0.5f);
							break;
						case GridNeighborDirections.West:
							vector = new Vector3(-0.5f, 0f, 0.5f);
							vector2 = new Vector3(-0.5f, 0f, -0.5f);
							break;
						}
						int num = 1 << k / 2;
						borderFlags[coordinate] |= num;
						Vector3 vector3 = gridModel.GetPosition(coordinate).ToVector3();
						vector.x *= (float)gridModel.CellSize.X;
						vector.z *= 0f - (float)gridModel.CellSize.Y;
						vector2.x *= (float)gridModel.CellSize.X;
						vector2.z *= 0f - (float)gridModel.CellSize.Y;
						list.Add(new Line(vector3 + vector, vector3 + vector2));
					}
				}
			}
		}
		Vector3 vector4 = new Vector3(0f, 1f, 0f);
		List<int> list2 = new List<int>();
		for (int l = 0; l < list.Count; l++)
		{
			int num2 = -1;
			Vector3 normal = list[l].GetNormal(vector4);
			for (int m = l + 2; m < list.Count; m++)
			{
				if (!list[l].Connected(list[m], checkEndOnly: true))
				{
					continue;
				}
				if (num2 != -1)
				{
					Vector3 normalized = (list[m].end - list[m].start).normalized;
					if (Vector3.Dot(normal, normalized) >= 0f)
					{
						num2 = m;
						break;
					}
				}
				else
				{
					num2 = m;
				}
			}
			if (num2 != -1)
			{
				Line value = list[l + 1];
				list[l + 1] = list[num2];
				list[num2] = value;
			}
		}
		for (int n = 0; n < list.Count; n++)
		{
			if (n > 0 && !list[n - 1].Connected(list[n], checkEndOnly: false))
			{
				list2.Add(n);
			}
		}
		list2.Add(list.Count);
		List<PolylinePath> list3 = new List<PolylinePath>();
		for (int num3 = 0; num3 < list2.Count; num3++)
		{
			PolylinePath polylinePath = new PolylinePath();
			int num4 = ((num3 > 0) ? (list2[num3] - list2[num3 - 1]) : list2[num3]);
			int num5 = ((num3 > 0) ? list2[num3 - 1] : 0);
			for (int num6 = 0; num6 < num4; num6++)
			{
				Line line = list[num5 + num6];
				Line line2 = list[num5 + (num6 + 1) % num4];
				if (Vector3.Dot(line2.end - line2.start, line.end - line.start) > 0.9f)
				{
					polylinePath.AddSegment(new LineSegment(line.center, line.end, vector4));
					polylinePath.AddSegment(new LineSegment(line.end, line2.center, vector4));
				}
				else
				{
					Vector3 startTangent = (line.end - line.center) * (1f - settings.Curvature * 0.75f);
					Vector3 endTangent = (line2.end - line2.center) * (1f - settings.Curvature * 0.75f);
					polylinePath.AddSegment(new CurveSegment(line.center, line2.center, startTangent, endTangent, vector4));
				}
			}
			list3.Add(polylinePath);
		}
		CreateMeshes(list3, settings, gridModel, fillMesh, shapeMesh);
		return list3;
	}

	public static void SmoothPath(List<Vector3> pathPoints, List<Vector3> pathNormals)
	{
		int num = pathPoints.Count - 1;
		int num2 = 0;
		int count = pathPoints.Count;
		int count2 = pathNormals.Count;
		for (int i = 0; i < count; i++)
		{
			num2 = ((i != count - 1) ? (i + 1) : 0);
			num = ((i != 0) ? (i - 1) : (count - 1));
			pathPoints.Add(0.333f * pathPoints[num] + 0.334f * pathPoints[i] + 0.333f * pathPoints[num2]);
			pathPoints.Add(0.333f * pathNormals[num] + 0.334f * pathNormals[i] + 0.333f * pathNormals[num2]);
		}
		pathPoints.RemoveRange(0, count);
		pathNormals.RemoveRange(0, count2);
	}

	private static void CreateMeshes(List<PolylinePath> polylines, GridAreaSettings settings, GridModel gridModel, Mesh fillMesh, Mesh shapeMesh)
	{
		fillVertices.Clear();
		fillNormals.Clear();
		fillUvs.Clear();
		fillTriangles.Clear();
		fillColors.Clear();
		shapeVertices.Clear();
		shapeUvs.Clear();
		shapeTriangles.Clear();
		pathPoints.Clear();
		pathNormals.Clear();
		GridAreaSettings.FillType fillType = ((fillMesh != null) ? settings.Fill : GridAreaSettings.FillType.None);
		for (int i = 0; i < polylines.Count; i++)
		{
			PolylinePath polylinePath = polylines[i];
			pathPoints.Clear();
			pathNormals.Clear();
			polylinePath.GetPathPoints(pathPoints, pathNormals, 8);
			if (pathPoints.Count < 3)
			{
				continue;
			}
			for (int j = 0; (float)j < settings.Smoothing; j++)
			{
				SmoothPath(pathPoints, pathNormals);
			}
			for (int k = 0; k < pathPoints.Count; k++)
			{
				Vector3 vector = pathNormals[k];
				pathPoints[k] += vector * settings.EdgeOffset;
			}
			if (shapeMesh != null)
			{
				vertices.Capacity = Math.Max(vertices.Capacity, pathPoints.Count * 2);
				vertices.Clear();
				uvs.Capacity = Math.Max(uvs.Capacity, pathPoints.Count * 2);
				uvs.Clear();
				triangles.Capacity = Math.Max(triangles.Capacity, pathPoints.Count * 6);
				triangles.Clear();
				MeshGenerator.CreateThickline(pathPoints, pathNormals, settings.Thickness, vertices, uvs, triangles, settings.TextureScale);
				if (shapeMesh != null)
				{
					if (shapeVertices.Count > 0)
					{
						for (int l = 0; l < triangles.Count; l++)
						{
							triangles[l] += shapeVertices.Count;
						}
					}
					shapeVertices.AddRange(vertices);
					shapeUvs.AddRange(uvs);
					shapeTriangles.AddRange(triangles);
				}
			}
			switch (fillType)
			{
			case GridAreaSettings.FillType.Inside:
				CreateShapeFill(pathPoints, fillVertices, fillNormals, fillUvs, fillTriangles);
				break;
			case GridAreaSettings.FillType.Outside:
			{
				Vector3 vector2 = gridModel.GetPosition(new GridCoordinate(0, 0)).ToVector3();
				Vector3 vector3 = gridModel.GetPosition(new GridCoordinate(gridModel.Width - 1, gridModel.Height - 1)).ToVector3();
				FindMinMaxIndices(pathPoints, out var outMinIndex, out var outMaxIndex);
				Vector3 vector4 = pathPoints[outMinIndex];
				Vector3 vector5 = pathPoints[outMaxIndex];
				upperPath.Clear();
				upperPath.Add(new Vector3(vector2.x - settings.AreaBorderWidth, vector2.y, vector2.z + settings.AreaBorderWidth));
				upperPath.Add(new Vector3(vector2.x - settings.AreaBorderWidth, vector2.y, vector4.z));
				AppendPath(upperPath, pathPoints, outMinIndex, outMaxIndex, reverse: false);
				upperPath.Add(new Vector3(vector3.x + settings.AreaBorderWidth, vector2.y, vector5.z));
				upperPath.Add(new Vector3(vector3.x + settings.AreaBorderWidth, vector2.y, vector2.z + settings.AreaBorderWidth));
				CreateShapeFill(upperPath, fillVertices, fillNormals, fillUvs, fillTriangles);
				lowerPath.Clear();
				lowerPath.Add(new Vector3(vector3.x + settings.AreaBorderWidth, vector2.y, vector3.z - settings.AreaBorderWidth));
				lowerPath.Add(new Vector3(vector3.x + settings.AreaBorderWidth, vector2.y, vector5.z));
				AppendPath(lowerPath, pathPoints, outMaxIndex, outMinIndex, reverse: false);
				lowerPath.Add(new Vector3(vector2.x - settings.AreaBorderWidth, vector2.y, vector4.z));
				lowerPath.Add(new Vector3(vector2.x - settings.AreaBorderWidth, vector2.y, vector3.z - settings.AreaBorderWidth));
				CreateShapeFill(lowerPath, fillVertices, fillNormals, fillUvs, fillTriangles);
				break;
			}
			}
			fillColors.Clear();
			for (int m = 0; m < fillNormals.Count(); m++)
			{
				fillColors.Add(Color.white);
			}
			if (shapeVertices != null && fillType == GridAreaSettings.FillType.Outside && polylines.Count > 1)
			{
				fillType = GridAreaSettings.FillType.Inside;
			}
		}
		if (fillMesh != null)
		{
			fillMesh.Clear();
			fillMesh.vertices = fillVertices.ToArray();
			fillMesh.normals = fillNormals.ToArray();
			fillMesh.colors = null;
			fillMesh.uv = fillUvs.ToArray();
			fillMesh.triangles = fillTriangles.ToArray();
			fillMesh.RecalculateBounds();
		}
		if (shapeMesh != null)
		{
			shapeMesh.Clear();
			shapeMesh.vertices = shapeVertices.ToArray();
			shapeMesh.normals = null;
			shapeMesh.uv = shapeUvs.ToArray();
			shapeMesh.colors = null;
			shapeMesh.triangles = shapeTriangles.ToArray();
			shapeMesh.RecalculateNormals();
			shapeMesh.RecalculateBounds();
		}
		borderUvs.Clear();
		borderVertices.Clear();
		borderNormals.Clear();
		borderTriangles.Clear();
		borderColors.Clear();
		Color item = new Color(1f, 1f, 1f, 0f);
		borderVertices.AddRange(fillVertices);
		borderTriangles.AddRange(fillTriangles);
		borderNormals.AddRange(fillNormals);
		borderColors.AddRange(fillColors);
		borderUvs.AddRange(fillUvs);
		for (int n = 0; n < polylines.Count; n++)
		{
			PolylinePath polylinePath2 = polylines[n];
			pathPoints.Clear();
			pathNormals.Clear();
			polylinePath2.GetPathPoints(pathPoints, pathNormals, 6);
			if (pathPoints.Count >= 3)
			{
				for (int num = 0; (float)num < settings.Smoothing; num++)
				{
					SmoothPath(pathPoints, pathNormals);
				}
				outerPolygon.Clear();
				for (int num2 = 0; num2 < pathPoints.Count; num2++)
				{
					Vector3 vector6 = pathNormals[num2];
					outerPolygon.Add(pathPoints[num2] + vector6 * settings.EdgeWidth + Mathf.PerlinNoise(pathPoints[num2].x * 1f + 0.1911f, 0.777f + pathPoints[num2].z * 1f) * settings.EdgeRandom * vector6);
					pathPoints[num2] += vector6 * settings.EdgeOffset;
				}
				int count = borderVertices.Count;
				for (int num3 = 0; num3 < pathPoints.Count; num3++)
				{
					borderVertices.Add(pathPoints[num3]);
					borderUvs.Add(new Vector2(pathPoints[num3].x, pathPoints[num3].z));
					borderColors.Add(Color.white);
					borderNormals.Add(Vector3.up);
				}
				int count2 = borderVertices.Count;
				for (int num4 = 0; num4 < pathPoints.Count; num4++)
				{
					borderVertices.Add(outerPolygon[num4]);
					borderUvs.Add(new Vector2(outerPolygon[num4].x, outerPolygon[num4].z));
					borderColors.Add(item);
					borderNormals.Add(Vector3.up);
				}
				int num5 = pathPoints.Count - 1;
				for (int num6 = 0; num6 < pathPoints.Count; num6++)
				{
					borderTriangles.Add(num5 + count);
					borderTriangles.Add(num6 + count);
					borderTriangles.Add(num5 + count2);
					borderTriangles.Add(num5 + count2);
					borderTriangles.Add(num6 + count);
					borderTriangles.Add(num6 + count2);
					num5 = num6;
				}
			}
		}
		if (fillMesh != null)
		{
			fillMesh.Clear();
			fillMesh.vertices = borderVertices.ToArray();
			fillMesh.normals = borderNormals.ToArray();
			fillMesh.colors = borderColors.ToArray();
			fillMesh.uv = borderUvs.ToArray();
			fillMesh.triangles = borderTriangles.ToArray();
			fillMesh.RecalculateBounds();
		}
	}

	private Vector2 GetNormal(Vector2 p)
	{
		return new Vector2(p.y, 0f - p.x);
	}

	private Vector3 ToVector3(Vector2 a)
	{
		return new Vector3(a.x, 0f, a.y);
	}

	private static void AppendPath(List<Vector3> first, List<Vector3> second, int fromIndex, int toIndex, bool reverse)
	{
		if (toIndex >= second.Count || toIndex < 0)
		{
			return;
		}
		int num = (reverse ? toIndex : fromIndex);
		int num2 = (reverse ? fromIndex : toIndex);
		int num3 = ((!reverse) ? 1 : (-1));
		int num4 = num;
		while (num4 != num2)
		{
			if (!first[first.Count - 1].Equals(second[num4]))
			{
				first.Add(second[num4]);
			}
			num4 += num3;
			if (num4 >= second.Count)
			{
				num4 = 0;
			}
			if (num4 < 0)
			{
				num4 = second.Count - 1;
			}
		}
		first.Add(second[num2]);
	}

	private static void FindMinMaxIndices(List<Vector3> pathPoints, out int outMinIndex, out int outMaxIndex)
	{
		float num = pathPoints[0].x;
		float num2 = pathPoints[0].x;
		int num3 = 0;
		int num4 = 0;
		for (int i = 1; i < pathPoints.Count; i++)
		{
			float x = pathPoints[i].x;
			if (x < num)
			{
				num = x;
				num3 = i;
			}
			if (x > num2)
			{
				num2 = x;
				num4 = i;
			}
		}
		outMinIndex = num3;
		outMaxIndex = num4;
	}

	private static void CreateShapeFill(List<Vector3> pathPoints, List<Vector3> fillVertices, List<Vector3> fillNormals, List<Vector2> fillUvs, List<int> fillTriangles)
	{
		createShapeFillDuplicates.Clear();
		for (int i = 0; i < pathPoints.Count; i++)
		{
			if (i > 0 && (double)(pathPoints[i] - pathPoints[i - 1]).sqrMagnitude < 0.001)
			{
				createShapeFillDuplicates.Add(i);
			}
		}
		if ((double)(pathPoints[0] - pathPoints[pathPoints.Count - 1]).sqrMagnitude < 0.001)
		{
			createShapeFillDuplicates.Add(pathPoints.Count - 1);
		}
		for (int num = createShapeFillDuplicates.Count - 1; num >= 0; num--)
		{
			pathPoints.RemoveAt(createShapeFillDuplicates[num]);
		}
		createShapeFillNormals.Clear();
		createShapeFillUvs.Clear();
		createShapeFillPathPoints2d.Clear();
		for (int j = 0; j < pathPoints.Count; j++)
		{
			createShapeFillPathPoints2d.Add(new Vector2(pathPoints[j].x, pathPoints[j].z));
			createShapeFillNormals.Add(Vector3.up);
			createShapeFillUvs.Add(createShapeFillPathPoints2d[j]);
		}
		createShapeFillTriangles.Clear();
		MeshGenerator.Triangulate(createShapeFillPathPoints2d, createShapeFillTriangles);
		if (fillVertices.Count > 0)
		{
			for (int k = 0; k < createShapeFillTriangles.Count; k++)
			{
				createShapeFillTriangles[k] += fillVertices.Count;
			}
		}
		fillVertices.AddRange(pathPoints);
		fillNormals.AddRange(createShapeFillNormals);
		fillUvs.AddRange(createShapeFillUvs);
		fillTriangles.AddRange(createShapeFillTriangles);
	}
}
