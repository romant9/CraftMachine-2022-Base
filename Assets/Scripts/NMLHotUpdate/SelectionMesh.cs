using System;
using Client.Constants;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class SelectionMesh : MonoBehaviour
{
	[SerializeField]
	private Material NormalMaterial;

	[SerializeField]
	private Material SelectedMaterial;

	[SerializeField]
	private Color NormalColor = new Color(0.07f, 0.11f, 0.03f);

	[SerializeField]
	private Color SelectedColor = Color.green;

	[SerializeField]
	private Color InactiveColor = Color.green;

	[SerializeField]
	private Texture[] ActiveStateTextures = new Texture[Enum.GetValues(typeof(AIAlertness)).Length];

	[SerializeField]
	private Texture OverwatchStateTexture;

	private Material rendererMaterial;

	public bool IsSelected { get; set; }

	public bool IsInactive { get; set; }

	public void SetNormalColor(Color color)
	{
		NormalColor = color;
	}

	public void SetSelectedColor(Color color)
	{
		SelectedColor = color;
	}

	public void SetInactiveColor(Color color)
	{
		InactiveColor = color;
	}

	public void SetAlertnessState(AIAlertness state)
	{
		Texture texture = ActiveStateTextures[(int)state];
		if (texture != null)
		{
			rendererMaterial.mainTexture = texture;
		}
	}

	public void SetOverwatchStateTexture()
	{
		if (OverwatchStateTexture != null)
		{
			rendererMaterial.mainTexture = OverwatchStateTexture;
		}
	}

	private void Awake()
	{
		rendererMaterial = GetComponent<Renderer>().material;
		rendererMaterial.hideFlags = HideFlags.HideAndDontSave;
	}

	private void Start()
	{
		IsSelected = false;
		CreateMesh();
	}

	private void Update()
	{
		if (IsSelected && SelectedMaterial != null)
		{
			rendererMaterial.mainTexture = SelectedMaterial.mainTexture;
		}
		else if (!IsSelected && NormalMaterial != null)
		{
			rendererMaterial.mainTexture = NormalMaterial.mainTexture;
		}
		if (IsSelected && !IsInactive)
		{
			rendererMaterial.SetColor(MaterialParameters.TintColor, SelectedColor);
		}
		else if (IsInactive)
		{
			rendererMaterial.SetColor(MaterialParameters.TintColor, InactiveColor);
		}
		else
		{
			rendererMaterial.SetColor(MaterialParameters.TintColor, NormalColor);
		}
	}

	private void CreateMesh()
	{
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		if (mesh != null)
		{
			Vector2 size = GameManager.Instance.playerModel.Grid.CellSize.ToVector2() * 0.5f;
			mesh.Clear();
			Vector3[] array = new Vector3[4];
			Vector2[] array2 = new Vector2[4];
			int[] array3 = new int[6];
			MeshGenerator.CreateRectangle(new Vector3(0f, 0f, 0f), size, array, array2, array3);
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.triangles = array3;
			mesh.normals = null;
			mesh.colors = null;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
		}
	}
}
