using System.Collections;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class BuildingPhotoManager : MonoBehaviour
{
	public GameObject buildingContainer;

	public Camera smallBuildingCamera;

	public Camera largeBuildingCamera;

	private Queue<BuildingVisualizationData> queue = new Queue<BuildingVisualizationData>();

	private Dictionary<string, Texture> portraits = new Dictionary<string, Texture>();

	private static BuildingPhotoManager instance;

	public bool IsRendering { get; private set; }

	public static BuildingPhotoManager Instance => instance;

	private void Awake()
	{
		if (instance != null)
		{
			Debug.LogError("Multiple portrait managers!");
			return;
		}
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
		largeBuildingCamera.gameObject.SetActive(value: false);
		smallBuildingCamera.gameObject.SetActive(value: false);
		base.gameObject.SetActive(value: false);
		SetRenderingActive(shouldBeActive: false);
	}

	private string GetBuildingKey(string buildingType, int level)
	{
		if (!string.IsNullOrEmpty(buildingType) && level >= 0)
		{
			return buildingType + "_" + level;
		}
		return null;
	}

	public Texture GetBuildingPhoto(string buildingTypeName, int level)
	{
		string buildingKey = GetBuildingKey(buildingTypeName, level);
		if (portraits.ContainsKey(buildingKey))
		{
			return portraits[buildingKey];
		}
		GameEconomyData gameEconomyData = GameManager.Instance.playerModel.gameEconomyData;
		BuildingType buildingType = gameEconomyData.GetBuildingType(buildingTypeName);
		GridSize buildingSize = new GridSize((int)Mathf.Ceil((float)gameEconomyData.ScaleToGrid(buildingType.Size.X) * 0.5f) * 2, (int)Mathf.Ceil((float)gameEconomyData.ScaleToGrid(buildingType.Size.Y) * 0.5f) * 2);
		BuildingVisualizationData buildingVisualizationData = new BuildingVisualizationData();
		buildingVisualizationData.BuildingType = buildingTypeName;
		buildingVisualizationData.BuildingLevel = level;
		buildingVisualizationData.BuildingSize = buildingSize;
		queue.Enqueue(buildingVisualizationData);
		int width = 512;
		int height = 512;
		RenderTextureFormat format = RenderTextureFormat.ARGB32;
		if (PlatformInfo.HasFlag(PlatformFlag.SDResolution) && !Application.isEditor)
		{
			width = 256;
			height = 256;
			format = RenderTextureFormat.ARGB4444;
		}
		RenderTexture value = new RenderTexture(width, height, 16, format);
		portraits.Add(buildingKey, value);
		SetRenderingActive(shouldBeActive: true);
		return portraits[buildingKey];
	}

	private void SetRenderingActive(bool shouldBeActive)
	{
		if (shouldBeActive && !base.gameObject.activeSelf)
		{
			IsRendering = true;
			base.gameObject.SetActive(value: true);
			StartCoroutine(RenderBuildings());
		}
		else if (base.gameObject.activeSelf && !shouldBeActive)
		{
			IsRendering = false;
			base.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator RenderBuildings()
	{
		while (queue.Count > 0)
		{
			BuildingVisualizationData visualizationData = queue.Dequeue();
			bool isLargeBuilding = visualizationData.BuildingSize.X > 4 || visualizationData.BuildingSize.Y > 4;
			BuildingResource buildingResourceFromStats = BuildingResource.GetBuildingResourceFromStats(visualizationData.BuildingType, visualizationData.BuildingLevel);
			RenderTexture targetTexture = portraits[GetBuildingKey(visualizationData.BuildingType, visualizationData.BuildingLevel)] as RenderTexture;
			GameObject prefab = buildingResourceFromStats.GetPrefab();
			buildingContainer.RemoveAllChildren();
			GameObject prefabInstance = Helpers.InstantiateToParent(prefab, buildingContainer);
			if (prefabInstance == null)
			{
				Debug.LogWarning("Instantiating portrait prefab failed");
				yield break;
			}
			prefabInstance.transform.Reset();
			prefabInstance.layer = LayerMask.NameToLayer("TextureRendering");
			SetLayerInChildren(prefabInstance.transform, LayerMask.NameToLayer("TextureRendering"));
			yield return null;
			string text = "Setup_" + visualizationData.BuildingType;
			Transform transform = base.transform.Find(text);
			Camera camera;
			if (transform == null)
			{
				Debug.LogWarning("Could not find setup '" + text + "' - using default setup");
				transform = base.transform.Find("Setup_Default");
				camera = (isLargeBuilding ? largeBuildingCamera : smallBuildingCamera);
			}
			else
			{
				transform.gameObject.SetActive(value: true);
				NGUITools.SetActiveChildren(transform.gameObject, state: true);
				camera = transform.GetComponentInChildren<Camera>();
			}
			transform.gameObject.SetActive(value: true);
			camera.targetTexture = targetTexture;
			camera.gameObject.SetActive(value: true);
			camera.Render();
			camera.gameObject.SetActive(value: false);
			Object.Destroy(prefabInstance);
			transform.gameObject.SetActive(value: false);
		}
		SetRenderingActive(shouldBeActive: false);
		EventManager.NotifyEvent(EventManager.EventType.BuildingPhotoRendered);
	}

	private void SetLayerInChildren(Transform transform, int layer)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject gameObject = transform.GetChild(i).gameObject;
			gameObject.layer = layer;
			SetLayerInChildren(gameObject.transform, layer);
		}
	}

	public void RemoveAll()
	{
		foreach (KeyValuePair<string, Texture> portrait in portraits)
		{
			RenderTexture renderTexture = portrait.Value as RenderTexture;
			if (renderTexture != null)
			{
				renderTexture.Release();
			}
		}
		portraits.Clear();
	}
}
