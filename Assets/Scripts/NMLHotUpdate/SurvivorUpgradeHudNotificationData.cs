using System.Collections.Generic;
using TWDModel;

public class SurvivorUpgradeHudNotificationData
{
	public Dictionary<int, SurvivorModel> AnimationList = new Dictionary<int, SurvivorModel>();

	public Dictionary<int, SurvivorModel> HistoryList = new Dictionary<int, SurvivorModel>();

	private float delayTime = 1f;

	private float currrentDelayTime;

	public bool animationRunning;

	public SurvivorModel CurrentModel;

	private static SurvivorUpgradeHudNotificationData instance;

	public static SurvivorUpgradeHudNotificationData Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SurvivorUpgradeHudNotificationData();
			}
			return instance;
		}
	}

	public static float CurrentDelayTime
	{
		get
		{
			return Instance.currrentDelayTime;
		}
		set
		{
			Instance.currrentDelayTime = value;
		}
	}

	public static void ResetStartDelayIfNotAnimating()
	{
		if (!Instance.animationRunning)
		{
			Instance.currrentDelayTime = Instance.delayTime;
		}
	}

	public static void Animate(SurvivorModel model)
	{
		if (!Instance.AnimationList.ContainsKey(model.ModelId) && !Instance.HistoryList.ContainsKey(model.ModelId))
		{
			Instance.AnimationList.Add(model.ModelId, model);
		}
	}

	public static void Remove(SurvivorModel model)
	{
		if (Instance.AnimationList.ContainsKey(model.ModelId))
		{
			Instance.AnimationList.Remove(model.ModelId);
		}
		if (Instance.HistoryList.ContainsKey(model.ModelId))
		{
			Instance.HistoryList.Remove(model.ModelId);
		}
	}

	public static void SetCurrentModel(SurvivorModel current)
	{
		Instance.CurrentModel = current;
		Instance.AnimationList.Remove(current.ModelId);
	}

	public static void TweenDoneCallback()
	{
		Instance.animationRunning = false;
		if (Instance.CurrentModel != null && !Instance.HistoryList.ContainsKey(Instance.CurrentModel.ModelId))
		{
			Instance.CurrentModel = null;
		}
	}

	public static SurvivorModel GetNextModelToShow()
	{
		if (Instance.AnimationList != null && Instance.AnimationList.Count > 0)
		{
			using Dictionary<int, SurvivorModel>.Enumerator enumerator = Instance.AnimationList.GetEnumerator();
			if (enumerator.MoveNext())
			{
				KeyValuePair<int, SurvivorModel> current = enumerator.Current;
				Instance.HistoryList.Add(current.Key, current.Value);
				return current.Value;
			}
		}
		return null;
	}

	public static void ClearShownAnimationHistory()
	{
		Instance.HistoryList = new Dictionary<int, SurvivorModel>();
	}

	public static void ClearAll()
	{
		Instance.HistoryList = new Dictionary<int, SurvivorModel>();
		Instance.AnimationList = new Dictionary<int, SurvivorModel>();
		Instance.CurrentModel = null;
	}
}
