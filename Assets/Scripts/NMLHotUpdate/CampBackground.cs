using BaseModel;
using TWDModel;
using UnityEngine;

public class CampBackground : MonoBehaviour
{
	private GameObject background;

	public static string CameraBoundsObjectPrefix = "Camera_Bounds_";

	private GameObject[] cameraBoundObjects;

	public float[] cameraMaxZoomDistances = new float[6] { 170f, 170f, 200f, 200f, 200f, 200f };

	public void Awake()
	{
		GameManager.Instance.OnLoadCompleted += OnLoadCompleted;
		GameManager.Instance.playerModel.Camp.GetBuilding("Council").Changed += OnModelChange;
	}

	public void OnDestroy()
	{
		GameManager.Instance.OnLoadCompleted -= OnLoadCompleted;
		GameManager.Instance.playerModel.Camp.GetBuilding("Council").Changed -= OnModelChange;
	}

	private void OnLoadCompleted()
	{
		RemoveBackground();
		background = GetBackground();
		cameraBoundObjects = GameObject.FindGameObjectsWithTag("CameraBounds");
		SetEnabled(CampView.Instance.gameObject.activeSelf);
		BuildingModel building = GameManager.Instance.playerModel.Camp.GetBuilding("Cage");
		ShowWalkerPitFill(building == null);
	}

	public void ApplyCampBounds()
	{
		if (cameraBoundObjects == null)
		{
			return;
		}
		int level = GameManager.Instance.playerModel.Camp.GetBuilding("Council").Level;
		string text = CameraBoundsObjectPrefix + level;
		GameObject gameObject = cameraBoundObjects[cameraBoundObjects.Length - 1];
		for (int i = 0; i < cameraBoundObjects.Length; i++)
		{
			GameObject gameObject2 = cameraBoundObjects[i];
			gameObject2.SetActive(value: false);
			if (gameObject2.name == text)
			{
				gameObject = gameObject2;
			}
		}
		if (gameObject != null)
		{
			gameObject.SetActive(value: true);
			Bounds bounds = gameObject.GetComponent<BoxCollider>().bounds;
			int num = Mathf.Min(level - 1, cameraMaxZoomDistances.Length - 1);
			CampView.Instance.CameraController.SetMaxCameraMaxDistance(cameraMaxZoomDistances[num]);
			CampView.Instance.CameraController.FitToBounds(bounds);
			gameObject.gameObject.SetActive(value: false);
		}
	}

	protected virtual void OnModelChange(ModelObject model, string changed, object args)
	{
		if (changed == "level" && !TutorialView.Instance.Running)
		{
			ApplyCampBounds();
		}
	}

	private GameObject GetBackground()
	{
		return GameObject.FindGameObjectWithTag("Background");
	}

	public void RemoveBackground()
	{
		if (background != null)
		{
			Object.Destroy(background);
		}
	}

	public void SetEnabled(bool enabled)
	{
		if (background != null)
		{
			background.SetActive(enabled);
			if (enabled)
			{
				ApplyCampBounds();
			}
		}
	}

	public void ShowWalkerPitFill(bool show)
	{
		Transform transform = background.transform.Find("WalkerPitFill");
		if (transform != null)
		{
			transform.gameObject.SetActive(show);
		}
	}
}
