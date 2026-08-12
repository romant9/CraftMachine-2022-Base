using UnityEngine;

public class bloodSplurt : MonoBehaviour
{
	public float SplurtHeight = 1.5f;

	public float SplurtScale = 4f;

	public float AirPolyDeleteTime = 1.5f;

	public float FadeOutTime = 1f;

	public float GameobjectDeleteTime = 5f;

	private float startTime;

	private float age;

	private float a = 0.2f;

	private int stage;

	private Mesh mesh;

	private void Start()
	{
		if (GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.IsGoreDisabled)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		startTime = Time.time;
		stage = 0;
		mesh = GetComponent<MeshFilter>().mesh;
		if (mesh != null)
		{
			mesh.Clear();
			Vector3[] array = new Vector3[12];
			Vector2[] array2 = new Vector2[12];
			int[] array3 = new int[18];
			createAllRectangles(SplurtHeight, new Vector3(0f, 0f, 0f), SplurtScale, array, array2, array3);
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.triangles = array3;
			mesh.normals = null;
			mesh.colors = null;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}
	}

	private void Update()
	{
		age = Time.time - startTime;
		if (age > AirPolyDeleteTime && stage == 0)
		{
			mesh.Clear();
			Vector3[] array = new Vector3[4];
			Vector2[] array2 = new Vector2[4];
			int[] array3 = new int[6];
			createGroundRectangle(SplurtHeight, new Vector3(0f, 0f, 0f), SplurtScale, array, array2, array3);
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.triangles = array3;
			mesh.normals = null;
			Color[] array4 = new Color[4];
			array4[0] = (array4[1] = (array4[2] = (array4[3] = Color.white)));
			mesh.colors = array4;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			stage = 1;
		}
		if (age > GameobjectDeleteTime - FadeOutTime && age < GameobjectDeleteTime && stage == 1)
		{
			float num = (Time.time - (startTime + (GameobjectDeleteTime - FadeOutTime))) / FadeOutTime;
			Color[] array5 = new Color[4];
			array5[0] = (array5[1] = (array5[2] = (array5[3] = new Color(1f, 1f, 1f, 1f - num))));
			mesh.colors = array5;
		}
		if (age > GameobjectDeleteTime && stage == 1)
		{
			Object.Destroy(base.gameObject);
			stage = 2;
		}
	}

	private void createAllRectangles(float height, Vector3 position, float size, Vector3[] outVertices, Vector2[] outUVs, int[] outTriangles)
	{
		outVertices[0] = new Vector3(a / 2f, height - size / 2f, 0f) + position;
		outVertices[1] = new Vector3(a / 2f, height - size / 2f, size) + position;
		outVertices[2] = new Vector3(0f - a, height, size) + position;
		outVertices[3] = new Vector3(0f - a, height, 0f) + position;
		outUVs[0] = new Vector2(0f, 0.5f);
		outUVs[1] = new Vector2(1f, 0.5f);
		outUVs[2] = new Vector2(1f, 1f);
		outUVs[3] = new Vector2(0f, 1f);
		outTriangles[0] = 0;
		outTriangles[1] = 2;
		outTriangles[2] = 1;
		outTriangles[3] = 0;
		outTriangles[4] = 3;
		outTriangles[5] = 2;
		outVertices[4] = new Vector3((0f - size) / 4f, 0f, 0f) + position;
		outVertices[5] = new Vector3(size / 4f, 0f, 0f) + position;
		outVertices[6] = new Vector3(size / 4f, 0f, size) + position;
		outVertices[7] = new Vector3((0f - size) / 4f, 0f, size) + position;
		outUVs[4] = new Vector2(0f, 0.5f);
		outUVs[5] = new Vector2(0f, 0f);
		outUVs[6] = new Vector2(1f, 0f);
		outUVs[7] = new Vector2(1f, 0.5f);
		outTriangles[6] = 4;
		outTriangles[7] = 6;
		outTriangles[8] = 5;
		outTriangles[9] = 4;
		outTriangles[10] = 7;
		outTriangles[11] = 6;
		outVertices[8] = new Vector3((0f - a) / 2f, height - size / 2f, 0f) + position;
		outVertices[9] = new Vector3((0f - a) / 2f, height - size / 2f, size) + position;
		outVertices[10] = new Vector3(a, height, size) + position;
		outVertices[11] = new Vector3(a, height, 0f) + position;
		outUVs[8] = new Vector2(0f, 0.5f);
		outUVs[9] = new Vector2(1f, 0.5f);
		outUVs[10] = new Vector2(1f, 1f);
		outUVs[11] = new Vector2(0f, 1f);
		outTriangles[12] = 8;
		outTriangles[13] = 9;
		outTriangles[14] = 10;
		outTriangles[15] = 8;
		outTriangles[16] = 10;
		outTriangles[17] = 11;
	}

	private void createGroundRectangle(float height, Vector3 position, float size, Vector3[] outVertices, Vector2[] outUVs, int[] outTriangles)
	{
		outVertices[0] = new Vector3((0f - size) / 4f, 0f, 0f) + position;
		outVertices[1] = new Vector3(size / 4f, 0f, 0f) + position;
		outVertices[2] = new Vector3(size / 4f, 0f, size) + position;
		outVertices[3] = new Vector3((0f - size) / 4f, 0f, size) + position;
		outUVs[0] = new Vector2(0f, 0.5f);
		outUVs[1] = new Vector2(0f, 0f);
		outUVs[2] = new Vector2(1f, 0f);
		outUVs[3] = new Vector2(1f, 0.5f);
		outTriangles[0] = 0;
		outTriangles[1] = 2;
		outTriangles[2] = 1;
		outTriangles[3] = 0;
		outTriangles[4] = 3;
		outTriangles[5] = 2;
	}
}
