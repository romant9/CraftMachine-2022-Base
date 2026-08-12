using System.Collections.Generic;
using UnityEngine;

public class ThickLineRenderer : MonoBehaviour
{
	public Material Material;

	public float LineThickness = 1f;

	public float StartFadeOutDistance = 1f;

	public float EndFadeOutDistance = 1f;

	public float TextureScale = 1f;

	public MeshGenerator.ExtrudeMode extrudeMode;

	private PolylinePath polylinePath;

	private List<Color> updateMeshColors = new List<Color>();

	private List<Vector3> updateMeshVertices = new List<Vector3>();

	private List<Vector2> updateMeshUvs = new List<Vector2>();

	private List<int> updateMeshTriangles = new List<int>();

	public void SetPoints(List<Vector3> points, Vector3 up)
	{
		polylinePath = new PolylinePath();
		List<Line> list = new List<Line>();
		for (int i = 0; i < points.Count - 1; i++)
		{
			list.Add(new Line(points[i], points[i + 1]));
		}
		for (int j = 0; j < list.Count; j++)
		{
			Line line = list[j];
			Line line2 = ((j + 1 < list.Count) ? list[j + 1] : null);
			if (line2 == null || Vector3.Dot(Vector3.Normalize(line2.end - line2.start), Vector3.Normalize(line.end - line.start)) > 0.95f)
			{
				if (!polylinePath.EndsAtCurve)
				{
					polylinePath.AddSegment(new LineSegment(line.start, line.end, up));
				}
				else
				{
					polylinePath.AddSegment(new LineSegment(line.center, line.end, up));
				}
				continue;
			}
			Vector3 startTangent = (line.end - line.center) * 0.5f;
			Vector3 endTangent = (line2.end - line2.center) * 0.5f;
			if (!polylinePath.EndsAtCurve)
			{
				polylinePath.AddSegment(new LineSegment(line.start, line.center, up));
			}
			polylinePath.AddSegment(new CurveSegment(line.center, line2.center, startTangent, endTangent, up));
		}
		UpdateMesh();
	}

	private void UpdateMesh()
	{
		if (GetComponent<MeshFilter>() == null)
		{
			base.gameObject.AddComponent<MeshFilter>();
		}
		Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
		if (mesh == null)
		{
			mesh = new Mesh();
			GetComponent<MeshFilter>().sharedMesh = mesh;
		}
		Color color = new Color(0f, 0f, 0f, 0f);
		if (!(mesh != null))
		{
			return;
		}
		mesh.Clear();
		if (polylinePath.Segments.Count > 0)
		{
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			polylinePath.GetPathPoints(list, list2, 8);
			updateMeshColors.Clear();
			updateMeshVertices.Clear();
			updateMeshUvs.Clear();
			updateMeshTriangles.Clear();
			float num = 0f;
			for (int i = 0; i < list.Count - 1; i++)
			{
				num += Vector3.Distance(list[i], list[i + 1]);
			}
			float num2 = 0f;
			for (int j = 0; j < list.Count; j++)
			{
				Color item = Color.white;
				if (j > 0)
				{
					num2 += Vector3.Distance(list[j - 1], list[j]);
				}
				float num3 = ((num > 0f) ? (num2 / num) : 1f);
				if (num3 > EndFadeOutDistance)
				{
					item = color;
				}
				else if (num3 > StartFadeOutDistance && EndFadeOutDistance - StartFadeOutDistance > 0f)
				{
					float a = num3 - StartFadeOutDistance / (EndFadeOutDistance - StartFadeOutDistance);
					item = new Color(1f, 1f, 1f, a);
				}
				updateMeshColors.Add(item);
				updateMeshColors.Add(item);
			}
			MeshGenerator.CreateThickline(list, list2, LineThickness, updateMeshVertices, updateMeshUvs, updateMeshTriangles, TextureScale, extrudeMode);
			mesh.vertices = updateMeshVertices.ToArray();
			mesh.normals = null;
			mesh.uv = updateMeshUvs.ToArray();
			mesh.colors = updateMeshColors.ToArray();
			mesh.triangles = updateMeshTriangles.ToArray();
		}
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
}
