using UnityEngine;

public class CurlNoisePrecalc2D : MonoBehaviour
{
	public Texture2D curlTexture;

	public float curlAmount;

	public float uvMul;

	private Mesh mesh;

	private void Start()
	{
		mesh = GetComponent<MeshFilter>().mesh;
	}

	private void Update()
	{
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			Color pixelBilinear = curlTexture.GetPixelBilinear(uvMul * vertices[i].x, uvMul * vertices[i].z);
			vertices[i].x += Time.deltaTime * curlAmount * (pixelBilinear.r * 2f - 1f);
			vertices[i].z += Time.deltaTime * curlAmount * (pixelBilinear.b * 2f - 1f);
		}
		mesh.vertices = vertices;
	}
}
