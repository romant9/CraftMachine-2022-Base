using TWDModel;
using UnityEngine;

public class SimpleGridAreaVisualization : MonoBehaviour
{
	public GameObject ShapeFill;

	[SerializeField]
	[Tooltip("Invert Z coordinate if grid")]
	private bool invertZ;

	private GridModel gridModel;

	private GridField<bool> gridCells;

	public virtual void Initialize(GridModel gridModel, GridField<bool> gridField)
	{
		gridCells = null;
		this.gridModel = gridModel;
		SetGridField(gridField);
	}

	public void SetGridField(GridField<bool> cellData)
	{
		gridCells = cellData;
		if (gridCells == null || gridCells.IsClear)
		{
			ClearAreaVisualization();
		}
		else
		{
			UpdateAreaVisualization();
		}
	}

	public virtual void OnDestroy()
	{
		gridCells = null;
	}

	protected void ClearAreaVisualization()
	{
		if (ShapeFill != null)
		{
			Mesh mesh = ShapeFill.GetComponent<MeshFilter>().mesh;
			if (mesh != null)
			{
				mesh.Clear();
				mesh.RecalculateBounds();
			}
		}
	}

	protected void UpdateAreaVisualization()
	{
		int width = gridModel.Width;
		int height = gridModel.Height;
		int num = 0;
		int num2 = 0;
		float num3 = 0.5f;
		Vector3[] array = new Vector3[width * height * 4];
		Vector2[] array2 = new Vector2[width * height * 4];
		int[] array3 = new int[width * height * 2 * 3];
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				GridCoordinate coordinate = new GridCoordinate(j, i);
				if (gridCells[coordinate])
				{
					Vector3 vector = new Vector3((float)j + 0.5f, 0f, (float)i + 0.5f);
					vector.z = (invertZ ? vector.z : (0f - vector.z));
					array[num] = vector + new Vector3(0f - num3, 0f, 0f - num3);
					array[num + 1] = vector + new Vector3(num3, 0f, 0f - num3);
					array[num + 2] = vector + new Vector3(num3, 0f, num3);
					array[num + 3] = vector + new Vector3(0f - num3, 0f, num3);
					array2[num] = new Vector2(0f, 0f);
					array2[num + 1] = new Vector2(1f, 0f);
					array2[num + 2] = new Vector2(1f, 1f);
					array2[num + 3] = new Vector2(0f, 1f);
					array3[num2] = num;
					array3[num2 + 1] = num + 2;
					array3[num2 + 2] = num + 1;
					array3[num2 + 3] = num;
					array3[num2 + 4] = num + 3;
					array3[num2 + 5] = num + 2;
					num += 4;
					num2 += 6;
				}
			}
		}
		if (ShapeFill != null)
		{
			Mesh mesh = ShapeFill.GetComponent<MeshFilter>().mesh;
			if (mesh != null)
			{
				mesh.Clear();
				mesh.vertices = array;
				mesh.normals = null;
				mesh.colors = null;
				mesh.uv = array2;
				mesh.triangles = array3;
				mesh.RecalculateBounds();
			}
		}
	}
}
