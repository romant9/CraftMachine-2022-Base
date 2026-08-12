using Client.Constants;
using UnityEngine;

public class EffectZipperBag : MonoBehaviour
{
	public enum effectModes
	{
		Upper = 0,
		Lower = 1
	}

	public GameObject magnet;

	public float radius = 1f;

	public float sharpness = 2f;

	public float amount = 0.5f;

	public effectModes effectMode;

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
		Vector3 vector = new Vector3(0f, 1f, 0f);
		switch (effectMode)
		{
		case effectModes.Upper:
			vector = new Vector3(0f, 1f, 0f);
			break;
		case effectModes.Lower:
			vector = new Vector3(0f, -1f, 0f);
			break;
		}
		Vector3[] vertices = mesh.vertices;
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < vertices.Length; i++)
		{
			num = magnetPos.x - origVertices[i].x;
			if (num < 0f)
			{
				num = 0f;
			}
			num2 = Mathf.Pow(num, sharpness);
			vertices[i] = origVertices[i] + num2 * amount * vector;
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		GetComponent<Renderer>().material.SetFloat(MaterialParameters.ClipOffset, base.transform.position.y);
	}
}
