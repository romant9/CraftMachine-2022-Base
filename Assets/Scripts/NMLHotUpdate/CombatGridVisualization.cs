using TWDModel;
using UnityEngine;

public class CombatGridVisualization : MonoBehaviour
{
	public float GridThickness = 0.02f;

	private int height;

	private int width;

	private Vector2 cellSize;

	private MeshFilter MF;

	private Mesh mesh;

	private void Awake()
	{
		UpdateVisibility();
		GameManager.Instance.Blackboard.BlackboardChanged += BlackboardChangedHandler;
	}

	private void BlackboardChangedHandler(BlackboardEntryType changedType, string keyChanged)
	{
		if (changedType == BlackboardEntryType.Toggle && !(keyChanged != "Toggle.ToggleCombatGridEnabled"))
		{
			UpdateVisibility();
		}
	}

	private void OnDestroy()
	{
		GameManager.Instance.Blackboard.BlackboardChanged -= BlackboardChangedHandler;
	}

	private void Start()
	{
		cellSize = new Vector2(GridView.Instance.ConfiguredCellSize.X, GridView.Instance.ConfiguredCellSize.Y);
		height = GridView.Instance.ConfiguredHeight;
		width = GridView.Instance.ConfiguredWidth;
		MF = base.gameObject.GetComponent<MeshFilter>();
		if (MF != null)
		{
			MF.mesh.Clear();
			mesh = MF.sharedMesh;
		}
		CreateGrid();
	}

	private void CreateGrid()
	{
		Vector3 vector = new Vector3(0f, 0f, 0f);
		_ = Vector3.one;
		Vector2 one = Vector2.one;
		Vector3[] array = new Vector3[4];
		Vector2[] array2 = new Vector2[4];
		int[] array3 = new int[6];
		int num = height + 1 + (width + 1);
		Vector3[] array4 = new Vector3[4 * num];
		Vector2[] array5 = new Vector2[4 * num];
		int[] array6 = new int[6 * num];
		for (int i = 0; i < height + 1; i++)
		{
			Vector3 position = vector + new Vector3((float)width * cellSize.x * 0.5f, 0f, (float)(-i) * cellSize.y);
			one = new Vector2((float)width * cellSize.x * 0.5f, GridThickness);
			MeshGenerator.CreateRectangle(position, one, array, array2, array3);
			array.CopyTo(array4, i * 4);
			array2.CopyTo(array5, i * 4);
			for (int j = 0; j < 6; j++)
			{
				array6[i * 6 + j] = array3[j] + i * 4;
			}
		}
		int num2 = height + 1;
		for (int k = 0; k < width + 1; k++)
		{
			Vector3 position2 = vector + new Vector3((float)k * cellSize.x, 0f, (float)(-height) * cellSize.y * 0.5f);
			one = new Vector2(GridThickness, (float)height * cellSize.y * 0.5f);
			MeshGenerator.CreateRectangle(position2, one, array, array2, array3);
			array.CopyTo(array4, (num2 + k) * 4);
			array2.CopyTo(array5, (num2 + k) * 4);
			for (int l = 0; l < 6; l++)
			{
				array6[(num2 + k) * 6 + l] = array3[l] + (num2 + k) * 4;
			}
		}
		mesh.vertices = array4;
		mesh.uv = array5;
		mesh.triangles = array6;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

	public void UpdateVisibility()
	{
		if (this != null && gameObject != null && GameManager.Instance != null)
		{
			bool IsActive = !OfflineManager.IsLoadDataManager ? GameManager.Instance.playerModel.Blackboard.IsToggleOn("Toggle.ToggleCombatGridEnabled") : OfflineManager.IsCombatGridEnabled;
			DebugTWD.Log((IsActive ? "Включили " : "Выключили ") + "сетку");
			base.gameObject.SetActive(IsActive);
		}
	}
}
