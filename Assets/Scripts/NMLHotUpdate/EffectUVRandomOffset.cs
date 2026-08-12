using UnityEngine;

public class EffectUVRandomOffset : MonoBehaviour
{
	public float UVPreScale = 1f;

	public float OffsetScale = 1f;

	private void Start()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector2[] array = new Vector2[mesh.vertexCount];
		Random.InitState(GetInstanceID());
		float value = Random.value;
		float value2 = Random.value;
		array = mesh.uv;
		for (int i = 0; i < mesh.vertexCount; i++)
		{
			float x = UVPreScale * array[i].x + OffsetScale * value;
			float y = UVPreScale * array[i].y + OffsetScale * value2;
			array[i] = new Vector2(x, y);
		}
		mesh.uv = array;
	}
}
