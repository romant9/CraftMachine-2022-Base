using UnityEngine;

public class SquashStretch : MonoBehaviour
{
	public float Ground = 0.1f;

	public float SquashHeight = 0.3f;

	public float SquashAmount = 1f;

	public float Penetration = 0.1f;

	private Mesh mesh;

	private Vector3[] vertices;

	private Vector3[] origVertices;

	private Vector3[] origNormals;

	private Color[] origColors;

	private float startTime;

	private float age;

	private void Start()
	{
		startTime = Time.time;
		mesh = GetComponent<MeshFilter>().mesh;
		vertices = new Vector3[mesh.vertexCount];
		origVertices = new Vector3[mesh.vertexCount];
		origNormals = new Vector3[mesh.vertexCount];
		origColors = new Color[mesh.vertexCount];
		origVertices = mesh.vertices;
		origNormals = mesh.normals;
		if (mesh.colors.Length != 0)
		{
			origColors = mesh.colors;
		}
	}

	private void Update()
	{
		age = Time.time - startTime;
		if (!(age < 4f))
		{
			return;
		}
		Transform transform = base.transform;
		int vertexCount = mesh.vertexCount;
		for (int i = 0; i < vertexCount; i++)
		{
			Vector3 position = transform.TransformPoint(origVertices[i]);
			Vector3 vector = transform.TransformDirection(origNormals[i]);
			float num = Mathf.Pow((Mathf.Clamp(Ground + SquashHeight - position.y, 0f, Ground + SquashHeight + Penetration) + 0f) / (Ground + SquashHeight + Penetration), 1f);
			if (position.y < Ground + SquashHeight)
			{
				position.y = Ground + SquashHeight - num * SquashHeight;
			}
			float f = num;
			float num2 = (Mathf.Sqrt(Mathf.Min(1f, origColors[i].grayscale)) + 0.2f) * SquashAmount * (Mathf.Pow(f, 5f) - Mathf.Pow(f, 16f));
			position.x += num2 * vector.x;
			position.z += num2 * vector.z;
			position.y -= Ground;
			vertices[i] = transform.InverseTransformPoint(position);
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}
}
