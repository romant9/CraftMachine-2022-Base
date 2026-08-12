using UnityEngine;

public class EffectZipperBagUv : MonoBehaviour
{
	public enum effectModes
	{
		Upper = 0,
		Lower = 1
	}

	public GameObject magnet;

	public float Radius = 1f;

	public float Sharpness = 2f;

	public float Amount = 0.5f;

	public effectModes effectMode;

	private Mesh mesh;

	private Vector3 magnetPos;

	private Vector2[] origUvs;

	private void Start()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		mesh.uv2 = mesh.uv;
	}

	private void Update()
	{
		magnetPos = base.transform.InverseTransformPoint(magnet.transform.position);
		float num = 1f;
		switch (effectMode)
		{
		case effectModes.Upper:
			num = -1f;
			break;
		case effectModes.Lower:
			num = 1f;
			break;
		}
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector2[] uv2 = mesh.uv2;
		Vector3 vector = new Vector3(0f, 0f, 0f);
		float num2 = 0f;
		float num3 = 0f;
		for (int i = 0; i < mesh.vertices.Length; i++)
		{
			vector = magnetPos - vertices[i];
			if (vector.x < 0f)
			{
				vector.x = 0f;
			}
			num2 = Mathf.Pow(vector.x, Sharpness);
			num3 = Mathf.Pow(Mathf.Abs(vector.y), Radius);
			uv[i].y = uv2[i].y + num2 * (0.9f - num3) * Amount * num;
		}
		mesh.uv = uv;
	}
}
