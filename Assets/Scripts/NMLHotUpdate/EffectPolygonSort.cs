using System.Collections.Generic;
using UnityEngine;

public class EffectPolygonSort : MonoBehaviour
{
	public enum sortTypes
	{
		NoChange = 0,
		XAxis = 1,
		YAxis = 2,
		ZAxis = 3,
		AlongGivenVector = 4,
		AlongVectorFromObject = 5,
		RadialFromPosition = 6,
		RadialFromObject = 7,
		Random = 8
	}

	public bool sortRuntime;

	public sortTypes sortType;

	public bool sortReverse;

	public Vector3 sortVector = new Vector3(0f, 1f, 0f);

	public GameObject sortReferenceObject;

	private static int polySorter(KeyValuePair<int, float> a, KeyValuePair<int, float> b)
	{
		return a.Value.CompareTo(b.Value);
	}

	private void Start()
	{
		SortPolygons();
	}

	private void Update()
	{
		if (sortRuntime)
		{
			SortPolygons();
		}
	}

	public void SortPolygons()
	{
		MeshFilter component = GetComponent<MeshFilter>();
		if (component == null)
		{
			return;
		}
		Mesh mesh = component.mesh;
		List<KeyValuePair<int, float>> list = new List<KeyValuePair<int, float>>();
		if (!(mesh != null) || sortType == sortTypes.NoChange)
		{
			return;
		}
		Random.InitState(42);
		float num = 0f;
		int num2 = mesh.triangles.Length / 3;
		Vector3 rhs = new Vector3(0f, 0f, 0f);
		if (sortReferenceObject != null)
		{
			rhs = sortReferenceObject.transform.forward;
		}
		Vector3 vector = new Vector3(0f, 0f, 0f);
		if (sortReferenceObject != null)
		{
			vector = sortReferenceObject.transform.position;
		}
		Vector3[] vertices = mesh.vertices;
		int[] triangles = mesh.triangles;
		for (int i = 0; i < num2; i++)
		{
			int[] array = new int[3]
			{
				triangles[3 * i],
				triangles[3 * i + 1],
				triangles[3 * i + 2]
			};
			Vector3 vector2 = new Vector3(vertices[array[0]].x, vertices[array[0]].y, vertices[array[0]].z);
			vector2 += new Vector3(vertices[array[1]].x, vertices[array[0]].y, vertices[array[1]].z);
			vector2 += new Vector3(vertices[array[2]].x, vertices[array[0]].y, vertices[array[2]].z);
			vector2 *= 0.3333f;
			switch (sortType)
			{
			case sortTypes.XAxis:
				num = vertices[array[0]].x;
				break;
			case sortTypes.YAxis:
				num = vertices[array[0]].y;
				break;
			case sortTypes.ZAxis:
				num = vertices[array[0]].z;
				break;
			case sortTypes.AlongGivenVector:
				num = Vector3.Dot(vector2, sortVector);
				break;
			case sortTypes.AlongVectorFromObject:
				num = Vector3.Dot(vector2, rhs);
				break;
			case sortTypes.RadialFromPosition:
				num = (vector2 - sortVector).magnitude;
				break;
			case sortTypes.RadialFromObject:
				num = (vector2 - vector).magnitude;
				break;
			case sortTypes.Random:
				num = Random.value;
				break;
			}
			num = (sortReverse ? (0f - num) : num);
			list.Add(new KeyValuePair<int, float>(i, num));
		}
		list.Sort(polySorter);
		List<int> list2 = new List<int>();
		foreach (KeyValuePair<int, float> item in list)
		{
			list2.Add(triangles[item.Key * 3]);
			list2.Add(triangles[item.Key * 3 + 1]);
			list2.Add(triangles[item.Key * 3 + 2]);
		}
		mesh.triangles = list2.ToArray();
	}
}
