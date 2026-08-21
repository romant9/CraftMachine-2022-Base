using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WeaponRangeVisualization : MonoBehaviour
{
	private struct Blocker
	{
		public Vector3 Center;

		public Vector3 Direction;

		public float Radius;
	}

	public GameObject borderMesh;

	public GameObject fillMesh;

	public WeaponRangeVisualizationShape shape;

	public const float DefaultLineWidth = 0.4f;

	public Vector3 StartPoint;

	public Vector3 EndPoint;

	public float SectorAngle;

	public float LineWidth = 0.4f;

	public float BorderWidth;

	public float BorderTextureScale;

	public int MaxVertexCount = 32;

	public float SingleTargetRadius = 0.4f;

	private Color normalBorderColor;

	private Color normalFillColor;

	private PointBlankShotRangeVisualConfig pointBlankShotRangeVisualConfigInternal;

	private List<Blocker> blockers;

	private Vector3 blocker;

	private PointBlankShotRangeVisualConfig pointBlankShotRangeVisualConfig
	{
		get
		{
			if (pointBlankShotRangeVisualConfigInternal == null)
			{
				pointBlankShotRangeVisualConfigInternal = UnityUtils.LoadFromAssetBundle<PointBlankShotRangeVisualConfig>("PointBlankShotRangeVisualConfig", "scriptableobjects");
			}
			return pointBlankShotRangeVisualConfigInternal;
		}
	}

	private void Awake()
	{
		normalBorderColor = borderMesh.GetComponent<MeshRenderer>().material.GetColor("_TintColor");
		normalFillColor = fillMesh.GetComponent<MeshRenderer>().material.GetColor("_TintColor");
	}

	public void Clear()
	{
		fillMesh.GetComponent<MeshFilter>().mesh = null;
		borderMesh.GetComponent<MeshFilter>().mesh = null;
		LineWidth = 0.4f;
	}

	public void SetSector(Vector3 start, Vector3 end, float sectorAngle)
	{
		shape = WeaponRangeVisualizationShape.Sector;
		StartPoint = start;
		EndPoint = end;
		SectorAngle = sectorAngle;
		UpdateMesh();
	}

	public void SetBrokenSector(Vector3 start, Vector3 end, float sectorAngle, List<Vector3> blockedCellCenters, float blockerRadius)
	{
		shape = WeaponRangeVisualizationShape.BrokenSector;
		StartPoint = start;
		EndPoint = end;
		SectorAngle = sectorAngle;
		if (blockers == null)
		{
			blockers = new List<Blocker>();
		}
		blockers.Clear();
		float sqrMagnitude = (end - start).sqrMagnitude;
		Vector3 vector = base.transform.worldToLocalMatrix.MultiplyPoint(start);
		foreach (Vector3 blockedCellCenter in blockedCellCenters)
		{
			if ((blockedCellCenter - start).sqrMagnitude < sqrMagnitude)
			{
				Vector3 vector2 = base.transform.worldToLocalMatrix.MultiplyPoint(blockedCellCenter);
				Vector3 normalized = Vector3.Cross(vector2 - vector, Vector3.up).normalized;
				blockers.Add(new Blocker
				{
					Center = vector2,
					Direction = normalized,
					Radius = blockerRadius
				});
			}
		}
		UpdateMesh();
	}

	public void SetLine(Vector3 start, Vector3 end, float width = 0.4f)
	{
		shape = WeaponRangeVisualizationShape.Line;
		StartPoint = start;
		EndPoint = end;
		LineWidth = width;
		UpdateMesh();
	}

	public void SetBrokenLine(Vector3 start, Vector3 end, Vector3 blocker, float width = 0.4f)
	{
		shape = WeaponRangeVisualizationShape.BrokenLine;
		StartPoint = start;
		EndPoint = end;
		LineWidth = width;
		this.blocker = blocker;
		UpdateMesh();
	}

	public void SetDiamond(Vector3 center, float radius)
	{
		shape = WeaponRangeVisualizationShape.Diamond;
		StartPoint = center;
		EndPoint = center + new Vector3(radius, 0f, 0f);
		UpdateMesh();
	}

	public void SetCircle(Vector3 start, float radius)
	{
		shape = WeaponRangeVisualizationShape.Circle;
		StartPoint = start;
		EndPoint = start + new Vector3(radius, 0f, 0f);
		UpdateMesh();
	}

	public void SetPoint(Vector3 position)
	{
		SetCircle(position, SingleTargetRadius);
	}

	private void UpdateMesh()
	{
		List<Vector3> list = new List<Vector3>();
		Vector3 vector = base.transform.worldToLocalMatrix.MultiplyPoint(StartPoint);
		Vector3 vector2 = base.transform.worldToLocalMatrix.MultiplyPoint(EndPoint);
		Vector3 vector3 = vector2 - vector;
		if (shape == WeaponRangeVisualizationShape.Line)
		{
			Vector3 vector4 = Vector3.Cross(vector3, Vector3.up).normalized * LineWidth / 2f;
			list.Add(vector + vector4);
			list.Add(vector2 + vector4);
			list.Add(vector2 - vector4);
			list.Add(vector - vector4);
		}
		else if (shape == WeaponRangeVisualizationShape.BrokenLine)
		{
			Vector3 vector5 = Vector3.Cross(vector3, Vector3.up).normalized * LineWidth / 2f;
			Vector3 vector6 = base.transform.worldToLocalMatrix.MultiplyPoint(blocker);
			Vector3 vector7 = vector + Vector3.Dot(vector6 - vector, vector3) * vector3 / vector3.sqrMagnitude;
			list.Add(vector + vector5);
			list.Add(vector7 + vector5);
			list.Add(vector7 - vector5);
			list.Add(vector - vector5);
		}
		else if (shape == WeaponRangeVisualizationShape.Sector)
		{
			if (SectorAngle == 360f)
			{
				SectorAngle -= 0.1f;
			}
			list.Add(vector);
			for (int i = 0; i < MaxVertexCount; i++)
			{
				float angle = ((float)i / (float)(MaxVertexCount - 1) - 0.5f) * SectorAngle;
				Vector3 item = vector + Quaternion.AngleAxis(angle, Vector3.up) * vector3;
				list.Add(item);
			}
		}
		else if (shape == WeaponRangeVisualizationShape.BrokenSector)
		{
			list.Add(vector);
			for (int j = 0; j < MaxVertexCount; j++)
			{
				float angle2 = ((float)j / (float)(MaxVertexCount - 1) - 0.5f) * SectorAngle;
				Vector3 vector8 = vector + Quaternion.AngleAxis(angle2, Vector3.up) * vector3;
				Vector3 lhs = Vector3.Cross(vector8 - vector, Vector3.up);
				foreach (Blocker blocker in blockers)
				{
					float num = Vector3.Dot(lhs, blocker.Direction);
					if (!(Mathf.Abs(num) > Mathf.Epsilon))
					{
						continue;
					}
					float num2 = Vector3.Dot(lhs, vector - blocker.Center) / num;
					if (num2 > 0f - blocker.Radius && num2 < blocker.Radius)
					{
						Vector3 vector9 = blocker.Center + blocker.Direction * num2;
						if ((vector9 - vector).sqrMagnitude < (vector8 - vector).sqrMagnitude)
						{
							vector8 = vector9;
						}
					}
				}
				list.Add(vector8);
			}
		}
		else if (shape == WeaponRangeVisualizationShape.Circle)
		{
			float magnitude = vector3.magnitude;
			for (int k = 0; k < MaxVertexCount; k++)
			{
				float f = (float)k / (float)MaxVertexCount * MathF.PI * 2f;
				Vector3 item2 = vector + new Vector3(Mathf.Cos(f) * magnitude, 0f, Mathf.Sin(f) * magnitude);
				list.Add(item2);
			}
		}
		else if (shape == WeaponRangeVisualizationShape.Diamond)
		{
			float magnitude2 = vector3.magnitude;
			list.Add(vector + new Vector3(0f, 0f, magnitude2));
			list.Add(vector + new Vector3(magnitude2, 0f, 0f));
			list.Add(vector + new Vector3(0f, 0f, 0f - magnitude2));
			list.Add(vector + new Vector3(0f - magnitude2, 0f, 0f));
		}
		List<Vector2> list2 = new List<Vector2>(list.Count);
		for (int l = 0; l < list.Count; l++)
		{
			list2.Add(new Vector2(list[l].x, list[l].z));
		}
		Mesh mesh = new Mesh();
		mesh.vertices = list.ToArray();
		List<int> list3 = new List<int>();
		MeshGenerator.Triangulate(list2, list3);
		mesh.triangles = list3.ToArray();
		mesh.uv = list2.ToArray();
		fillMesh.GetComponent<MeshFilter>().sharedMesh = mesh;
		if (shape == WeaponRangeVisualizationShape.BrokenSector)
		{
			list.Clear();
			list.Add(vector);
			for (int m = 0; m < MaxVertexCount; m++)
			{
				float angle3 = ((float)m / (float)(MaxVertexCount - 1) - 0.5f) * SectorAngle;
				Vector3 item3 = vector + Quaternion.AngleAxis(angle3, Vector3.up) * vector3;
				list.Add(item3);
			}
		}
		else if (shape == WeaponRangeVisualizationShape.BrokenLine)
		{
			list.Clear();
			Vector3 vector10 = Vector3.Cross(vector3, Vector3.up).normalized * LineWidth / 2f;
			list.Add(vector + vector10);
			list.Add(vector2 + vector10);
			list.Add(vector2 - vector10);
			list.Add(vector - vector10);
		}
		list.Add(list[0]);
		List<Vector3> list4 = new List<Vector3>(list.Count);
		for (int n = 0; n < list.Count; n++)
		{
			int index = ((n == 0) ? (list.Count - 1) : (n - 1));
			int index2 = ((n != list.Count - 1) ? (n + 1) : 0);
			Vector3 normalized = (list[index2] - list[n]).normalized;
			Vector3 normalized2 = (list[n] - list[index]).normalized;
			list4.Add((new Vector3(0f - normalized.z, 0f, normalized.x) + new Vector3(0f - normalized2.z, 0f, normalized2.x)).normalized);
		}
		List<Vector3> list5 = new List<Vector3>(list.Count * 2);
		List<Vector2> list6 = new List<Vector2>(list.Count * 2);
		List<int> list7 = new List<int>(list.Count * 6);
		MeshGenerator.CreateThickline(list, list4, BorderWidth, list5, list6, list7, BorderTextureScale);
		Mesh mesh2 = new Mesh();
		mesh2.vertices = list5.ToArray();
		mesh2.triangles = list7.ToArray();
		mesh2.uv = list6.ToArray();
		borderMesh.GetComponent<MeshFilter>().sharedMesh = mesh2;
	}

	private void Update()
	{
		if (!Application.isPlaying)
		{
			UpdateMesh();
		}
		base.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
	}

	public void SetDangerIndicator()
	{
		fillMesh.GetComponent<MeshRenderer>().material.SetColor("_TintColor", Color.red);
		borderMesh.GetComponent<MeshRenderer>().material.SetColor("_TintColor", Color.red);
	}

	public void ClearDangerIndicator()
	{
		fillMesh.GetComponent<MeshRenderer>().material.SetColor("_TintColor", normalFillColor);
		borderMesh.GetComponent<MeshRenderer>().material.SetColor("_TintColor", normalBorderColor);
	}

	public Color GetFillColor()
	{
		return normalFillColor;
	}

	public void SetHerdIndicator()
	{
		Color value = new Color(1f, 0.15f, 0f);
		fillMesh.GetComponent<MeshRenderer>().material.SetColor("_TintColor", value);
		borderMesh.GetComponent<MeshRenderer>().material.SetColor("_TintColor", Color.red);
	}

	public void SetPointBlankIndicator()
	{
		fillMesh.GetComponent<MeshRenderer>().material.SetColor("_TintColor", pointBlankShotRangeVisualConfig.GetColorForShape(shape));
	}

	public void SetSuppressIndicator()
	{
		fillMesh.GetComponent<MeshRenderer>().material.SetColor("_TintColor", pointBlankShotRangeVisualConfig.GetColorForShape(shape));
	}
}
