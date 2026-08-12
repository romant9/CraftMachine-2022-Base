using UnityEngine;

public class Magnet : MonoBehaviour
{
	public GameObject magnet;

	public float radius = 1f;

	public float sharpness = 2f;

	public float amount = 0.5f;

	public float noiseAmount = 0.5f;

	private Mesh mesh;

	private Vector3 magnetPos;

	private Vector3[] origVertices;

	private void Start()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		origVertices = mesh.vertices;
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			origVertices[i] = vertices[i];
		}
	}

	private void Update()
	{
		magnetPos = base.transform.InverseTransformPoint(magnet.transform.position);
		Vector3[] vertices = mesh.vertices;
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < vertices.Length; i++)
		{
			Vector3 vector = origVertices[i] - magnetPos;
			Vector3 normalized = vector.normalized;
			num3 = Mathf.PerlinNoise(normalized.x, normalized.y) - Mathf.PerlinNoise(17.37f, normalized.z);
			num = vector.magnitude + noiseAmount * num3;
			num2 = Mathf.Pow(Mathf.Clamp01(1f - Mathf.Clamp01(num / radius)), sharpness);
			vertices[i] = origVertices[i] + num2 * amount * vector.normalized;
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
}
