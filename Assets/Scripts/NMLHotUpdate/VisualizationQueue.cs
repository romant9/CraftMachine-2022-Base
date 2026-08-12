using System;
using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class VisualizationQueue : MonoBehaviour
{
	public static bool VerboseDebugEnabled;

	private List<VisualizationTask> queuedTasks;

	private List<VisualizationTask> activeTasks;

	private Dictionary<Faction, object> factionDependencyObjects;

	private List<SpatialLock> spatialDependencyObjects;

	private bool paused;

	public static VisualizationQueue Instance { get; private set; }

	public object GlobalDependencyObject { get; private set; }

	public int TotalTaskCount => queuedTasks.Count + activeTasks.Count;

	public bool IsQueueEmpty => TotalTaskCount == 0;

	public event VisualizationTaskCompletedHandler VisualizationTaskCompleted;

	public VisualizationQueue()
	{
		queuedTasks = new List<VisualizationTask>();
		activeTasks = new List<VisualizationTask>();
		factionDependencyObjects = new Dictionary<Faction, object>();
		Faction[] array = Enum.GetValues(typeof(Faction)) as Faction[];
		for (int i = 0; i < array.Length; i++)
		{
			factionDependencyObjects.Add(array[i], new object());
		}
		GlobalDependencyObject = new object();
	}

	public List<VisualizationTask> GetQueuedTasks()
	{
		return queuedTasks;
	}

	public void AddTaskBlocker()
	{
		if (!OfflineManager.IsLoadDataManager) Add(new VisualizationTaskBlocker());
	}

	public SpatialLock GetSpatialDependencyObject(int x, int y)
	{
		if (GridView.Instance != null)
		{
			if (spatialDependencyObjects == null)
			{
				spatialDependencyObjects = new List<SpatialLock>();
				for (int i = 0; i < GridView.Instance.Grid.Height; i++)
				{
					for (int j = 0; j < GridView.Instance.Grid.Width; j++)
					{
						spatialDependencyObjects.Add(new SpatialLock(j, i));
					}
				}
			}
			int num = y * GridView.Instance.Grid.Width + x;
			if (spatialDependencyObjects == null || num < 0 || num >= spatialDependencyObjects.Count)
			{
				return null;
			}
			return spatialDependencyObjects[num];
		}
		return null;
	}

	public object GetFactionDependencyObject(Faction faction)
	{
		if (factionDependencyObjects.ContainsKey(faction))
		{
			return factionDependencyObjects[faction];
		}
		return null;
	}

	public List<object> GetFactionDependencyObjects()
	{
		List<object> list = new List<object>();
		foreach (object value in factionDependencyObjects.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public Faction GetFactionForFactionDependencyObject(object dependencyObject)
	{
		foreach (KeyValuePair<Faction, object> factionDependencyObject in factionDependencyObjects)
		{
			if (factionDependencyObject.Value == dependencyObject)
			{
				return factionDependencyObject.Key;
			}
		}
		return Faction.Any;
	}

	public int GetTaskCountForFactionsExcept(Faction faction)
	{
		int num = 0;
		foreach (Faction key in factionDependencyObjects.Keys)
		{
			if (key != faction)
			{
				num += GetTaskCountForFaction(key);
			}
		}
		return num;
	}

	public int GetTaskCountForFaction(Faction faction)
	{
		int num = 0;
		object factionDependencyObject = GetFactionDependencyObject(faction);
		if (factionDependencyObject == null)
		{
			return 0;
		}
		for (int i = 0; i < queuedTasks.Count; i++)
		{
			VisualizationTask visualizationTask = queuedTasks[i];
			if (visualizationTask.Dependencies == null)
			{
				continue;
			}
			for (int j = 0; j < visualizationTask.Dependencies.Count; j++)
			{
				if (visualizationTask.Dependencies[j].DependencyObject == factionDependencyObject)
				{
					num++;
					break;
				}
			}
		}
		for (int k = 0; k < activeTasks.Count; k++)
		{
			VisualizationTask visualizationTask2 = activeTasks[k];
			if (visualizationTask2.Dependencies == null)
			{
				continue;
			}
			for (int l = 0; l < visualizationTask2.Dependencies.Count; l++)
			{
				if (visualizationTask2.Dependencies[l].DependencyObject == factionDependencyObject)
				{
					num++;
					break;
				}
			}
		}
		return num;
	}

	public bool HasTaskOfType<T>() where T : VisualizationTask
	{
		for (int i = 0; i < queuedTasks.Count; i++)
		{
			if (queuedTasks[i] as T != null)
			{
				return true;
			}
		}
		for (int j = 0; j < activeTasks.Count; j++)
		{
			if (activeTasks[j] as T != null)
			{
				return true;
			}
		}
		return false;
	}

	private void Awake()
	{
		Instance = this;
	}

	public void OnDestroy()
	{
		StopAllTasks();
		Instance = null;
	}

	public void StopAllTasks()
	{
		for (int i = 0; i < activeTasks.Count; i++)
		{
			VisualizationTask visualizationTask = activeTasks[i];
			visualizationTask.Stop();
			visualizationTask.Finished();
		}
		queuedTasks.Clear();
		activeTasks.Clear();
	}

	public void StopDependentTasks(object dependencyObject)
	{
		List<VisualizationTask> list = new List<VisualizationTask>();
		for (int i = 0; i < queuedTasks.Count; i++)
		{
			VisualizationTask visualizationTask = queuedTasks[i];
			if (visualizationTask.Dependencies == null)
			{
				continue;
			}
			for (int j = 0; j < visualizationTask.Dependencies.Count; j++)
			{
				if (visualizationTask.Dependencies[j].DependencyObject == dependencyObject)
				{
					list.Add(visualizationTask);
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			VisualizationTask item = list[k];
			queuedTasks.Remove(item);
		}
		list.Clear();
		for (int l = 0; l < activeTasks.Count; l++)
		{
			VisualizationTask visualizationTask2 = activeTasks[l];
			if (visualizationTask2.Dependencies == null)
			{
				continue;
			}
			for (int m = 0; m < visualizationTask2.Dependencies.Count; m++)
			{
				if (visualizationTask2.Dependencies[m].DependencyObject == dependencyObject)
				{
					visualizationTask2.Stop();
					visualizationTask2.Finished();
					list.Add(visualizationTask2);
				}
			}
		}
		for (int n = 0; n < list.Count; n++)
		{
			VisualizationTask item2 = list[n];
			activeTasks.Remove(item2);
		}
	}

	public void Add(VisualizationTask task)
	{
		List<VisualizationTask> list = task.TasksToQueue();
		if (list == null || list.Count <= 0)
		{
			return;
		}
		if (VerboseDebugEnabled)
		{
			foreach (VisualizationTask item in list)
			{
				_ = item;
			}
		}
		queuedTasks.AddRange(list);
		task.Queued();
	}

	public void RemoveFromQueue(VisualizationTask task)
	{
		queuedTasks.Remove(task);
	}

	public void PauseVisualizations(bool pause)
	{
		paused = pause;
	}

	private void Activate(VisualizationTask task)
	{
		queuedTasks.Remove(task);
		task.Activate();
		activeTasks.Add(task);
		_ = VerboseDebugEnabled;
	}

	private void Complete(VisualizationTask task)
	{
		_ = VerboseDebugEnabled;
		task.Finished();
		activeTasks.Remove(task);
		NotifyVisualizationTaskCompleted(task);
	}

	private void NotifyVisualizationTaskCompleted(VisualizationTask task)
	{
		this.VisualizationTaskCompleted?.Invoke(task);
	}

	private bool CanActivate(VisualizationTask task)
	{
		for (int i = 0; i < activeTasks.Count; i++)
		{
			VisualizationTask task2 = activeTasks[i];
			if (task.DependsOn(task2))
			{
				return false;
			}
		}
		for (int j = 0; j < queuedTasks.Count; j++)
		{
			VisualizationTask visualizationTask = queuedTasks[j];
			if (visualizationTask == task)
			{
				break;
			}
			if (task.DependsOn(visualizationTask))
			{
				return false;
			}
		}
		return true;
	}

	public void Update()
	{
		if (paused)
		{
			return;
		}
		List<VisualizationTask> list = new List<VisualizationTask>();
		for (int i = 0; i < queuedTasks.Count; i++)
		{
			VisualizationTask visualizationTask = queuedTasks[i];
			if (CanActivate(visualizationTask))
			{
				list.Add(visualizationTask);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			Activate(list[j]);
		}
		List<VisualizationTask> list2 = new List<VisualizationTask>();
		for (int k = 0; k < activeTasks.Count; k++)
		{
			VisualizationTask visualizationTask2 = activeTasks[k];
			visualizationTask2.RunTime += Time.unscaledDeltaTime;
			if (visualizationTask2.RunTime > 10f)
			{
				_ = GameManager.Instance.playerModel.Combat;
			}
			if (!visualizationTask2.Update(Time.unscaledDeltaTime))
			{
				list2.Add(visualizationTask2);
			}
		}
		for (int l = 0; l < list2.Count; l++)
		{
			Complete(list2[l]);
		}
	}

	public bool HasDependencyObject(object dependencyObject)
	{
		for (int i = 0; i < queuedTasks.Count; i++)
		{
			VisualizationTask visualizationTask = queuedTasks[i];
			if (visualizationTask.Dependencies == null)
			{
				continue;
			}
			for (int j = 0; j < visualizationTask.Dependencies.Count; j++)
			{
				if (visualizationTask.Dependencies[j].DependencyObject == dependencyObject)
				{
					return true;
				}
			}
		}
		for (int k = 0; k < activeTasks.Count; k++)
		{
			VisualizationTask visualizationTask2 = activeTasks[k];
			if (visualizationTask2.Dependencies == null)
			{
				continue;
			}
			for (int l = 0; l < visualizationTask2.Dependencies.Count; l++)
			{
				if (visualizationTask2.Dependencies[l].DependencyObject == dependencyObject)
				{
					return true;
				}
			}
		}
		return false;
	}

	public T GetMostRecentlyAddedTask<T>(object dependencyObject) where T : VisualizationTask
	{
		for (int num = queuedTasks.Count - 1; num >= 0; num--)
		{
			VisualizationTask visualizationTask = queuedTasks[num];
			if (visualizationTask is T && visualizationTask.Dependencies != null)
			{
				for (int i = 0; i < visualizationTask.Dependencies.Count; i++)
				{
					if (visualizationTask.Dependencies[i].DependencyObject == dependencyObject)
					{
						return visualizationTask as T;
					}
				}
			}
		}
		for (int num2 = activeTasks.Count - 1; num2 >= 0; num2--)
		{
			VisualizationTask visualizationTask2 = activeTasks[num2];
			if (visualizationTask2 is T && visualizationTask2.Dependencies != null)
			{
				for (int j = 0; j < visualizationTask2.Dependencies.Count; j++)
				{
					if (visualizationTask2.Dependencies[j].DependencyObject == dependencyObject)
					{
						return visualizationTask2 as T;
					}
				}
			}
		}
		return null;
	}

	public T GetMostRecentlyAddedActorTask<T>(ActorModel primaryActor) where T : ActorVisualizationTask
	{
		for (int num = queuedTasks.Count - 1; num >= 0; num--)
		{
			ActorVisualizationTask actorVisualizationTask = queuedTasks[num] as ActorVisualizationTask;
			if (actorVisualizationTask is T && actorVisualizationTask.Actor == primaryActor)
			{
				return actorVisualizationTask as T;
			}
		}
		for (int num2 = activeTasks.Count - 1; num2 >= 0; num2--)
		{
			ActorVisualizationTask actorVisualizationTask2 = activeTasks[num2] as ActorVisualizationTask;
			if (actorVisualizationTask2 is T && actorVisualizationTask2.Actor == primaryActor)
			{
				return actorVisualizationTask2 as T;
			}
		}
		return null;
	}

	public T GetNextActorTask<T>(ActorModel primaryActor) where T : ActorVisualizationTask
	{
		for (int i = 0; i < queuedTasks.Count; i++)
		{
			ActorVisualizationTask actorVisualizationTask = queuedTasks[i] as ActorVisualizationTask;
			if (actorVisualizationTask is T && actorVisualizationTask.Actor == primaryActor)
			{
				return actorVisualizationTask as T;
			}
		}
		for (int j = 0; j < activeTasks.Count; j++)
		{
			ActorVisualizationTask actorVisualizationTask2 = activeTasks[j] as ActorVisualizationTask;
			if (actorVisualizationTask2 is T && actorVisualizationTask2.Actor == primaryActor)
			{
				return actorVisualizationTask2 as T;
			}
		}
		return null;
	}

	public List<T> GetTasksOfType<T>(bool includeActive = false) where T : VisualizationTask
	{
		List<T> list = new List<T>();
		for (int i = 0; i < queuedTasks.Count; i++)
		{
			VisualizationTask visualizationTask = queuedTasks[i];
			if (visualizationTask is T)
			{
				list.Add(visualizationTask as T);
			}
		}
		if (includeActive)
		{
			for (int j = 0; j < activeTasks.Count; j++)
			{
				VisualizationTask visualizationTask2 = activeTasks[j];
				if (visualizationTask2 is T)
				{
					list.Add(visualizationTask2 as T);
				}
			}
		}
		return list;
	}

	public void GameDisconnected()
	{
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.LostConnectionAlertPopup);
		if (noCreation != null)
		{
			noCreation.OnCloseCallback = (Callback)Delegate.Combine(noCreation.OnCloseCallback, new Callback(GameConnected));
			paused = true;
		}
	}

	private void GameConnected()
	{
		HUDElement noCreation = SingularityMonoBehaviour<HUDManager>.Instance.GetNoCreation(UIType.LostConnectionAlertPopup);
		if (noCreation != null)
		{
			noCreation.OnCloseCallback = (Callback)Delegate.Remove(noCreation.OnCloseCallback, new Callback(GameConnected));
		}
		paused = false;
	}
}
