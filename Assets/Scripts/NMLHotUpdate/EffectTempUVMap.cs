using UnityEngine;

public class EffectTempUVMap : MonoBehaviour
{
	private void Start()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector2[] array = new Vector2[mesh.vertexCount];
		Vector3[] array2 = new Vector3[mesh.vertexCount];
		Random.InitState(GetInstanceID());
		float value = Random.value;
		float value2 = Random.value;
		array2 = mesh.vertices;
		array = mesh.uv;
		for (int i = 0; i < mesh.vertexCount; i++)
		{
			float x = 5f * Mathf.Acos(array2[i].z) / 3.1415f + 0.4f * value;
			float y = 0.5f * Mathf.Atan(array2[i].y / array2[i].x) / 6.283f + 0.4f * value2;
			array[i] = new Vector2(x, y);
		}
		mesh.uv = array;
	}
}
