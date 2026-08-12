using System.Collections.Generic;
using UnityEngine;

public class EffectGridAreaFence : MonoBehaviour
{
	public GameObject GridSource;

	public float FenceHeight;

	public float StartDuration = 0.6f;

	private List<PolylinePath> polylines;

	private float startTime;

	private static List<Vector3> fencePathPoints = new List<Vector3>();

	private static List<Vector3> fencePathNormals = new List<Vector3>();

	private static List<Vector3> fenceVertices = new List<Vector3>();

	private static List<Vector2> fenceUVs = new List<Vector2>();

	private static List<int> fenceTriangles = new List<int>();

	private static List<Vector3> fenceTotalVertices = new List<Vector3>();

	private static List<Vector2> fenceTotalUVs = new List<Vector2>();

	private static List<int> fenceTotalTriangles = new List<int>();

	private void Initialize()
	{
		if (GridSource == null)
		{
			Debug.LogWarning("EffectGridEraFence: Grid Source not specified");
			return;
		}
		GridAreaVisualization component = GridSource.GetComponent<GridAreaVisualization>();
		if (component != null)
		{
			polylines = component.GetPolyPaths();
			MeshFilter component2 = base.gameObject.GetComponent<MeshFilter>();
			if (component2 != null)
			{
				Mesh mesh = new Mesh();
				CreateFenceMesh(polylines, component.gridAreaSettings, mesh);
				component2.sharedMesh = mesh;
			}
			else
			{
				Debug.LogWarning("EffectGridAreFence: no MeshFilter on gameobject!");
			}
			base.transform.localScale = new Vector3(1f, 0f, 1f);
		}
		else
		{
			Debug.LogWarning("EffectGridEraFence: Grid Source has no *GridAreaVisualization component");
		}
	}

	private void Start()
	{
		startTime = Time.time;
		base.transform.localScale = new Vector3(1f, 0f, 1f);
	}

	private void Update()
	{
		float num = Time.time - startTime;
		base.transform.localScale = new Vector3(1f, Mathf.SmoothStep(0f, 1f, num / StartDuration), 1f);
	}

	private void OnEnable()
	{
		Initialize();
		startTime = Time.time;
	}

	private void CreateFenceMesh(List<PolylinePath> polylines, GridAreaSettings settings, Mesh fenceMesh)
	{
		fenceTotalVertices.Clear();
		fenceTotalUVs.Clear();
		fenceTotalTriangles.Clear();
		for (int i = 0; i < polylines.Count; i++)
		{
			PolylinePath polylinePath = polylines[i];
			fencePathPoints.Clear();
			fencePathNormals.Clear();
			fenceVertices.Clear();
			fenceUVs.Clear();
			fenceTriangles.Clear();
			polylinePath.GetPathPoints(fencePathPoints, fencePathNormals, 8);
			if (fencePathPoints.Count < 3)
			{
				continue;
			}
			for (int j = 0; (float)j < settings.Smoothing; j++)
			{
				GridAreaVisualization.SmoothPath(fencePathPoints, fencePathNormals);
			}
			for (int k = 0; k < fencePathPoints.Count; k++)
			{
				fencePathNormals[k] = Vector3.up;
				fencePathPoints[k] += fencePathNormals[k] * FenceHeight * 0.5f;
			}
			if (!(fenceMesh != null))
			{
				continue;
			}
			MeshGenerator.CreateThickline(fencePathPoints, fencePathNormals, FenceHeight, fenceVertices, fenceUVs, fenceTriangles, settings.TextureScale);
			if (fenceTotalTriangles.Count > 0)
			{
				for (int l = 0; l < fenceTriangles.Count; l++)
				{
					fenceTriangles[l] += fenceTotalVertices.Count;
				}
			}
			fenceTotalVertices.AddRange(fenceVertices);
			fenceTotalUVs.AddRange(fenceUVs);
			fenceTotalTriangles.AddRange(fenceTriangles);
		}
		fenceMesh.Clear();
		fenceMesh.vertices = fenceTotalVertices.ToArray();
		fenceMesh.triangles = fenceTotalTriangles.ToArray();
		fenceMesh.uv = fenceTotalUVs.ToArray();
		fenceMesh.RecalculateBounds();
	}
}
