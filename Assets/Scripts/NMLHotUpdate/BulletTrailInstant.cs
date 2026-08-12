using System.Collections.Generic;
using UnityEngine;

public class BulletTrailInstant : MonoBehaviour
{
	public Vector3 start;

	public Vector3 end;

	public float flightSpeed;

	public float trailWidth;

	public float smokeAge;

	private Vector3 line;

	private float distance;

	private float startTime;

	private Mesh trailmesh;

	private Color32[] trailMeshColors;

	private GameObject projectile;

	private Renderer trailRenderer;

	private Color color;

	private List<Vector3> lineVertices = new List<Vector3>();

	private List<Vector3> lineNormals = new List<Vector3>();

	private List<Vector3> vertices = new List<Vector3>();

	private List<Vector2> uvs = new List<Vector2>();

	private List<int> triangles = new List<int>();

	public void SetTrailCoordinates(Vector3 start, Vector3 end)
	{
		this.start = start;
		this.end = end;
		line = end - start;
		distance = line.magnitude;
	}

	private void Start()
	{
		line = end - start;
		distance = line.magnitude;
		startTime = Time.time;
		trailmesh = GetComponent<MeshFilter>().mesh;
		CreateTrailMesh();
		trailMeshColors = new Color32[trailmesh.vertexCount];
	}

	private void UpdateTrailAlpha(float flightStage)
	{
		if (trailmesh != null)
		{
			for (int i = 0; i < trailmesh.vertexCount; i++)
			{
				float x = trailmesh.uv[i].x;
				byte a = (byte)(255f * Mathf.Clamp01(flightStage - x));
				trailMeshColors[i] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, a);
			}
			trailmesh.colors32 = trailMeshColors;
		}
	}

	private void CreateTrailMesh()
	{
		if (trailmesh != null)
		{
			int num = Mathf.CeilToInt(distance / (trailWidth * 1.5f));
			float num2 = distance / (float)num;
			int num3 = num + 1;
			lineVertices.Clear();
			lineNormals.Clear();
			for (int i = 0; i < num3; i++)
			{
				lineVertices.Add(start + num2 * (float)i * line.normalized);
				lineNormals.Add(Vector3.Cross(line.normalized, Vector3.up));
			}
			trailmesh.Clear();
			vertices.Clear();
			uvs.Clear();
			triangles.Clear();
			float textureScale = 1f / distance;
			MeshGenerator.CreateThickline(lineVertices, lineNormals, trailWidth, vertices, uvs, triangles, textureScale);
			trailmesh.vertices = vertices.ToArray();
			trailmesh.uv = uvs.ToArray();
			trailmesh.triangles = triangles.ToArray();
			trailmesh.RecalculateNormals();
			trailmesh.RecalculateBounds();
			trailRenderer = GetComponent<Renderer>();
			color = trailRenderer.material.color;
		}
	}

	private void Update()
	{
		float num = Time.time - startTime;
		float a = 1f - Mathf.SmoothStep(1f, 0f, (smokeAge - num) / smokeAge);
		trailRenderer.material.color = new Color(color.r, color.g, color.b, a);
		if (num > smokeAge)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
