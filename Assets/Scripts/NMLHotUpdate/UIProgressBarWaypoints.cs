using System.Collections.Generic;
using UnityEngine;

public class UIProgressBarWaypoints : UIProgressBarExtended
{
	[SerializeField]
	private UIWidget waypointsParent;

	[SerializeField]
	private GameObject waypointsPrefab;

	private Dictionary<float, GameObject> waypointGameObjects = new Dictionary<float, GameObject>();

	private bool animatingWaypoints;

	private int currentWaypointIndex = -1;

	private List<ProgressBarWaypoint> waypointsList = new List<ProgressBarWaypoint>();

	public void StartWaypoints()
	{
		if (!animatingWaypoints && IsNotNull(waypointsList, "UIProgressBarWaypoints::AnimateWaypoints") && waypointsList.Count > 0)
		{
			DebugLog("Start waypoints count: " + waypointsList.Count);
			AnimateNextWaypoint();
		}
	}

	public void StopWaypoints()
	{
		if (animatingWaypoints && CurrentWaypoint() != null)
		{
			DebugLog("Waypoints complete. Last waypoint id: " + CurrentWaypoint().id);
		}
		CancelInvoke();
		animatingWaypoints = false;
	}

	public virtual void Reset()
	{
		CancelInvoke();
		foreach (KeyValuePair<float, GameObject> waypointGameObject in waypointGameObjects)
		{
			if (waypointGameObject.Value != null)
			{
				Object.Destroy(waypointGameObject.Value);
			}
		}
		waypointGameObjects = new Dictionary<float, GameObject>();
		waypointsList = new List<ProgressBarWaypoint>();
		currentWaypointIndex = -1;
		animatingWaypoints = false;
		SetProgress(0f);
	}

	public GameObject CreateWaypointIconAt(float progress, GameObject prefabOverride = null, bool setActivate = true, bool positionNow = false)
	{
		progress = Mathf.Clamp01(progress);
		GameObject value = null;
		if (waypointsParent != null && waypointsParent.gameObject != null && !waypointGameObjects.TryGetValue(progress, out value))
		{
			if (prefabOverride != null)
			{
				value = Helpers.InstantiateToParent(prefabOverride, waypointsParent.gameObject);
			}
			else if (waypointsPrefab != null)
			{
				value = Helpers.InstantiateToParent(waypointsPrefab, waypointsParent.gameObject);
			}
			if (IsNotNull(value, "UIProgressBarWaypoints->InstantiateToParent"))
			{
				Helpers.GameObjectSetActive(value, setActivate);
				waypointGameObjects[progress] = value;
			}
		}
		if (positionNow)
		{
			PositionWaypoint(value.transform, progress);
		}
		return value;
	}

	public void AddAnimationWaypoint(ProgressBarWaypoint waypoint)
	{
		if (IsNotNull(waypointsList, "UIProgressBarWaypoints::AddAnimationWaypoint") && IsNotNull(waypoint, "UIProgressBarWaypoints::AddAnimationWaypoint"))
		{
			waypointsList.Add(waypoint);
		}
	}

	public void PositionWaypoints()
	{
		if (!(waypointsParent != null))
		{
			return;
		}
		foreach (KeyValuePair<float, GameObject> waypointGameObject in waypointGameObjects)
		{
			if (waypointGameObject.Value != null && waypointGameObject.Value.transform != null)
			{
				PositionWaypoint(waypointGameObject.Value.transform, waypointGameObject.Key);
			}
		}
	}

	public override void Clear()
	{
		base.Clear();
		Reset();
	}

	public ProgressBarWaypoint GetWaypointAtIndex(int index)
	{
		if (waypointsList != null && waypointsList.Count > 0)
		{
			if (index == -1)
			{
				return waypointsList[waypointsList.Count - 1];
			}
			if (waypointsList.Count > index)
			{
				return waypointsList[index];
			}
		}
		return null;
	}

	public float GetWaypointsTotalDuration()
	{
		float num = 0f;
		for (int i = 0; i < waypointsList.Count; i++)
		{
			if (waypointsList[i] != null)
			{
				num += waypointsList[i].TotalDuration;
			}
		}
		return num;
	}

	private void PositionWaypoint(Transform trans, float progress)
	{
		progress = Mathf.Clamp01(progress);
		Vector3 vector = default(Vector3);
		vector = trans.localPosition;
		vector.x = waypointsParent.localSize.x * progress;
		trans.localPosition = vector;
	}

	private void AnimateNextWaypoint(int indexOffset = 0)
	{
		if (TryIncrementCurrentIndex(indexOffset + 1) && CurrentWaypoint() != null)
		{
			DebugLog("Staring animating waypoint id: " + CurrentWaypoint().id + " to progress: " + CurrentWaypoint().to);
			animatingWaypoints = true;
			TweenToProgress(CurrentWaypoint().to, CurrentWaypoint().from, CurrentWaypoint().duration, CurrentWaypoint().Easing);
			Invoke("WaypointComplete", CurrentWaypoint().duration);
		}
		else
		{
			StopWaypoints();
		}
	}

	private void WaypointComplete()
	{
		Invoke("WaypointDelayComplete", CurrentWaypoint().completionDelay);
		if (CurrentWaypoint().ActivateObject != null)
		{
			CurrentWaypoint().Complete();
		}
	}

	private void WaypointDelayComplete()
	{
		DebugLog("Completed animating waypoint with id: " + currentWaypointIndex);
		AnimateNextWaypoint();
	}

	protected override void OnEasingComplete()
	{
		base.OnEasingComplete();
	}

	private ProgressBarWaypoint CurrentWaypoint()
	{
		if (currentWaypointIndex > -1 && currentWaypointIndex < waypointsList.Count)
		{
			return waypointsList[currentWaypointIndex];
		}
		return null;
	}

	private bool TryIncrementCurrentIndex(int value)
	{
		int num = currentWaypointIndex + value;
		if (num > -1 && num < waypointsList.Count)
		{
			currentWaypointIndex = num;
			return true;
		}
		return false;
	}
}
