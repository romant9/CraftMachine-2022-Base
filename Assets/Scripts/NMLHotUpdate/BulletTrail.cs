using System.Collections.Generic;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
	public Vector3 start;

	public Vector3 end;

	public float flightSpeed;

	public float trailWidth;

	public float projectileWidth;

	public float projectileLength;

	public float smokeAge;

	public Material projectileMaterial;

	public float FlightTime;

	private Vector3 line;

	private float distance;

	private float startTime;

	private Mesh trailmesh;

	private Color[] trailMeshColors;

	private GameObject projectile;

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
		FlightTime = distance / flightSpeed;
	}

	private void Start()
	{
		line = end - start;
		distance = line.magnitude;
		projectile = GameObject.CreatePrimitive(PrimitiveType.Quad);
		projectile.name = "projectile";
		projectile.GetComponent<Renderer>().material = projectileMaterial;
		projectile.transform.localPosition = start;
		projectile.transform.localScale = new Vector3(projectileLength, projectileWidth, projectileWidth);
		projectile.transform.LookAt(end);
		projectile.transform.Rotate(90f, 90f, 0f);
		startTime = Time.time;
		FlightTime = distance / flightSpeed;
		trailmesh = GetComponent<MeshFilter>().mesh;
		CreateTrailMesh();
		trailMeshColors = new Color[trailmesh.vertexCount];
	}

	private void UpdateTrailAlpha(float flightStage)
	{
		if (trailmesh != null)
		{
			for (int i = 0; i < trailmesh.vertexCount; i++)
			{
				float x = trailmesh.uv[i].x;
				trailMeshColors[i] = new Color(1f, 1f, 1f, Mathf.Clamp01(flightStage - x));
			}
			trailmesh.colors = trailMeshColors;
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
		}
	}

	private void Update()
	{
		float num = Time.time - startTime;
		float num2 = Mathf.Clamp01(num / FlightTime);
		if (projectile != null)
		{
			projectile.transform.localPosition = num2 * end + (1f - num2) * start;
		}
		float a = 1f - Mathf.SmoothStep(1f, 0f, (smokeAge - num) / smokeAge);
		UpdateTrailAlpha(num2);
		GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, a);
		if (num > FlightTime)
		{
			Object.Destroy(projectile);
		}
		if (num > smokeAge)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
