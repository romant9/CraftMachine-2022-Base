using UnityEngine;

public class MapClouds : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Container object for clouds")]
	private GameObject containerObj;

	[Tooltip("Clouds will be spawned inside bounding box of this object")]
	public GameObject templateObj;

	public int CloudCount = 30;

	public Vector3 baseScale = new Vector3(1.4f, 0.3f, 1f);

	private void Start()
	{
		Random.InitState(4);
		UpdateCloudMesh();
	}

	private void OnEnable()
	{
		UpdateCloudMesh();
	}

	public void UpdateCloudMesh()
	{
		CreateCloudMesh();
	}

	private void CreateCloudMesh()
	{
		MeshFilter component = templateObj.GetComponent<MeshFilter>();
		MeshRenderer component2 = templateObj.GetComponent<MeshRenderer>();
		if (component == null)
		{
			Object.Destroy(base.gameObject);
		}
		Bounds bounds = component2.bounds;
		Mesh mesh = containerObj.GetComponent<MeshFilter>().mesh;
		Vector3[] array = new Vector3[4 * CloudCount];
		Vector2[] array2 = new Vector2[4 * CloudCount];
		int[] array3 = new int[6 * CloudCount];
		Vector4 uvRect = new Vector4(0f, 0f, 1f, 1f);
		float lean = 0.9f;
		Vector3 vector = baseScale;
		int num = 0;
		for (int i = 0; i < CloudCount; i++)
		{
			Random.InitState(i);
			Vector3 position = new Vector3(Random.value * bounds.size.x, Random.value * bounds.size.y, Random.value * bounds.size.z);
			position += bounds.min;
			createRectangle(num, position, vector, lean, uvRect, array, array2, array3);
			num += 2;
		}
		mesh.Clear();
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		Vector3 vector2 = new Vector3(0f, 0f, -2f);
		Color[] array4 = new Color[array.Length];
		Vector2[] array5 = new Vector2[array.Length];
		for (int j = 0; j < array.Length; j++)
		{
			int num2 = Mathf.FloorToInt(j / 4);
			Random.InitState(num2);
			float num3 = Mathf.PerlinNoise(0.8f * array[j].x, 0.8f * array[j].z);
			float num4 = 1f / (1.6f + 1f * Mathf.Abs(vector2.z - array[j].z));
			float a = 0.45f + 0.2f * num3;
			array4[j] = new Color(num4, num4, num4, a);
			array5[j] = new Vector2(0.15f + 0.2f * Random.value, num2);
		}
		mesh.colors = array4;
		mesh.uv2 = array5;
	}

	private static void createRectangle(int triIndex, Vector3 position, Vector2 size, float lean, Vector4 uvRect, Vector3[] outVertices, Vector2[] outUVs, int[] outTriangles)
	{
		int num = triIndex * 2;
		int num2 = triIndex * 3;
		outVertices[num] = new Vector3(0f - size.x, 0f - size.y, 0f) + position;
		outVertices[num + 1] = new Vector3(size.x, 0f - size.y, 0f) + position;
		outVertices[num + 2] = new Vector3(size.x, size.y, lean) + position;
		outVertices[num + 3] = new Vector3(0f - size.x, size.y, lean) + position;
		outUVs[num] = new Vector2(uvRect.x, uvRect.y);
		outUVs[num + 1] = new Vector2(uvRect.z, uvRect.y);
		outUVs[num + 2] = new Vector2(uvRect.z, uvRect.w);
		outUVs[num + 3] = new Vector2(uvRect.x, uvRect.w);
		outTriangles[num2] = num;
		outTriangles[num2 + 1] = num + 2;
		outTriangles[num2 + 2] = num + 1;
		outTriangles[num2 + 3] = num;
		outTriangles[num2 + 4] = num + 3;
		outTriangles[num2 + 5] = num + 2;
	}
}
