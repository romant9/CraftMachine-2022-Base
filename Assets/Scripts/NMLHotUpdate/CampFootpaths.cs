using System.Collections.Generic;
using UnityEngine;

public class CampFootpaths : MonoBehaviour
{
	private int PathCount = 64;

	public float PathRecycleTime = 2f;

	public float PathAlpha = 0.4f;

	public GameObject PathTemplate;

	public GameObject PathParent;

	private List<GameObject> PathObjects = new List<GameObject>();

	private List<MeshFilter> PathMeshFilters = new List<MeshFilter>();

	private bool inited;

	private int nextPathIndex;

	private int previousPathIndex;

	private float previousPathUpdateTime;

	private static int pathSubdivision = 50;

	private static int meshPointcount = 2 * pathSubdivision;

	private static int meshTriVertcount = 6 * pathSubdivision;

	private Vector3[] meshPoints = new Vector3[meshPointcount];

	private Vector2[] uvs = new Vector2[meshPointcount];

	private int[] triangleVerts = new int[meshTriVertcount];

	private Color[] meshColors = new Color[meshPointcount];

	private Mesh tempMesh;

	private Vector3[] pathVertices = new Vector3[pathSubdivision];

	private Vector3[] pathNormals = new Vector3[pathSubdivision];

	private void Start()
	{
		if (!inited && !PlatformInfo.HasFlag(PlatformFlag.SlowCPU))
		{
			int level = GameManager.Instance.playerModel.Camp.GetBuilding("Council").Level;
			PathCount = 4 + 3 * level;
			InitPathMeshes();
		}
	}

	private void Update()
	{
		if (inited)
		{
			float num = Mathf.Clamp01((Time.time - previousPathUpdateTime) / PathRecycleTime);
			for (int i = 2; i < meshPointcount - 2; i++)
			{
				meshColors[i] = new Color(0.8f, 0.8f, 0.8f, num * PathAlpha);
			}
			PathMeshFilters[previousPathIndex].mesh.colors = meshColors;
			for (int j = 2; j < meshPointcount - 2; j++)
			{
				meshColors[j] = new Color(0.8f, 0.8f, 0.8f, (1f - num) * PathAlpha);
			}
			PathMeshFilters[nextPathIndex].mesh.colors = meshColors;
		}
	}

	private void InitPathMeshes()
	{
		tempMesh = new Mesh();
		PolylinePath polylinePath = new PolylinePath();
		polylinePath.AddSegment(new LineSegment(new Vector3(0f, -1f, 0f), new Vector3(10f, -1f, 0f), new Vector3(0f, 1f, 0f)));
		for (int i = 0; i < PathCount; i++)
		{
			GameObject gameObject = Object.Instantiate(PathTemplate, PathParent.transform.localPosition, PathParent.transform.localRotation, PathParent.transform);
			gameObject.transform.localPosition = new Vector3(-0.3f + 0.6f * Random.value, 0.0077f, -0.3f + 0.6f * Random.value);
			gameObject.name = "Footpath_" + i;
			BuildPath(polylinePath);
			MeshFilter component = gameObject.GetComponent<MeshFilter>();
			component.sharedMesh = tempMesh;
			PathObjects.Add(gameObject);
			PathMeshFilters.Add(component);
		}
		inited = true;
	}

	public void NewPathRequest(PolylinePath newPath)
	{
		if (Time.time - previousPathUpdateTime > PathRecycleTime)
		{
			UpdatePath(nextPathIndex, newPath);
			previousPathUpdateTime = Time.time;
			previousPathIndex = nextPathIndex;
			nextPathIndex++;
			nextPathIndex = ((nextPathIndex != PathCount) ? nextPathIndex : 0);
		}
	}

	private void UpdatePath(int index, PolylinePath path)
	{
		meshPoints = BuildPath(path, buildFullMesh: false);
		PathMeshFilters[index].mesh.vertices = meshPoints;
		PathMeshFilters[index].mesh.RecalculateBounds();
	}

	private Vector3[] BuildPath(PolylinePath newPath, bool buildFullMesh = true)
	{
		float thickness = 0.75f;
		float textureScale = 0.1f;
		PolylinePathIterator polylinePathIterator = new PolylinePathIterator(newPath);
		float distance = newPath.Length / (float)pathSubdivision;
		for (int i = 0; i < pathSubdivision; i++)
		{
			Vector3 position = polylinePathIterator.Position;
			if (!polylinePathIterator.AtEnd)
			{
				polylinePathIterator.Advance(distance);
			}
			pathVertices[i] = position;
		}
		for (int j = 0; j < pathVertices.Length; j++)
		{
			int num = ((j == 0) ? 1 : j);
			Vector3 vector = pathVertices[num] - pathVertices[num - 1];
			pathNormals[j] = Vector3.Cross(vector.normalized, Vector3.up);
		}
		BuildPathMesh(pathVertices, pathNormals, thickness, textureScale, buildFullMesh);
		if (!buildFullMesh)
		{
			return meshPoints;
		}
		tempMesh.vertices = meshPoints;
		tempMesh.uv = uvs;
		for (int k = 0; k < meshPointcount; k++)
		{
			meshColors[k] = new Color(0.8f, 0.8f, 0.8f, 0f);
		}
		tempMesh.colors = meshColors;
		tempMesh.triangles = triangleVerts;
		tempMesh.RecalculateNormals();
		tempMesh.RecalculateBounds();
		return meshPoints;
	}

	private void BuildPathMesh(Vector3[] inPoints, Vector3[] inPointNormals, float thickness, float textureScale, bool buildFullMesh = true)
	{
		int num = 0;
		float num2 = 0f;
		float num3 = 0f;
		Vector3 vector = inPoints[^1];
		for (int i = 0; i < inPoints.Length; i++)
		{
			Vector3 vector2 = inPoints[i];
			Vector3 vector3 = inPointNormals[i] * thickness * 0.5f;
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			zero = vector2 - vector3;
			zero2 = vector2 + vector3;
			meshPoints[2 * i] = zero;
			meshPoints[2 * i + 1] = zero2;
			if (buildFullMesh)
			{
				num3 = num2 + (vector2 - vector).magnitude * textureScale;
				float num4 = ((!(num3 > 1f)) ? num3 : 0f);
				if (i == 0)
				{
					uvs[2 * i] = new Vector2(num2, 0f);
					uvs[2 * i + 1] = new Vector2(num2, 1f);
				}
				else
				{
					uvs[2 * i] = new Vector2(num3, 0f);
					uvs[2 * i + 1] = new Vector2(num3, 1f);
				}
				if (i > 0)
				{
					triangleVerts[6 * i] = num - 2;
					triangleVerts[6 * i + 1] = num + 1;
					triangleVerts[6 * i + 2] = num;
					triangleVerts[6 * i + 3] = num - 2;
					triangleVerts[6 * i + 4] = num - 1;
					triangleVerts[6 * i + 5] = num + 1;
				}
				num += 2;
				num2 = num4;
			}
			vector = vector2;
		}
	}
}
