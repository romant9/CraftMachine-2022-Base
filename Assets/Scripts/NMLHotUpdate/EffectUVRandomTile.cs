using UnityEngine;

public class EffectUVRandomTile : MonoBehaviour
{
	public int HorizontalTiles = 1;

	public int VerticalTiles = 1;

	private void Start()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector2[] array = new Vector2[mesh.vertexCount];
		int num = Mathf.FloorToInt(Random.value * (float)HorizontalTiles * (float)VerticalTiles);
		float num2 = 1f / (float)HorizontalTiles;
		float num3 = 1f / (float)VerticalTiles;
		int num4 = num % HorizontalTiles;
		int num5 = Mathf.CeilToInt((float)num / (float)VerticalTiles);
		array = mesh.uv;
		for (int i = 0; i < mesh.vertexCount; i++)
		{
			float x = num2 * array[i].x + (float)num4 * num2;
			float y = num3 * array[i].y + (float)num5 * num3;
			array[i] = new Vector2(x, y);
		}
		mesh.uv = array;
	}
}
