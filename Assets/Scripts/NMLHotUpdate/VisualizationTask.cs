using System;
using System.Collections.Generic;
using TWDModel;

[Serializable]
public class VisualizationTask
{
	public float RunTime;

	private List<TaskDependency> dependencies;

	public virtual bool IsGlobalBlocker => false;

	public List<TaskDependency> Dependencies
	{
		get
		{
			return dependencies;
		}
		private set
		{
			if (dependencies != value)
			{
				dependencies = value;
			}
		}
	}

	public bool IsActive { get; private set; }

	public ModelAction Action { get; private set; }

	public VisualizationTask(ModelAction inAction)
	{
		Action = inAction;
		IsActive = false;
	}

	public void AddDependencyToAllActors(bool reserve = false, ActorModel excludeActor = null)
	{
		foreach (ActorModel allActor in GameManager.Instance.modelManager.CombatModel.GetAllActors())
		{
			if (allActor != excludeActor)
			{
				AddDependency(allActor, reserve);
			}
		}
	}

	public void AddActorDependency(ActorModel actor)
	{
		TaskDependency taskDependency = AddDependency(actor);
		if (actor != null)
		{
			taskDependency.DebugName = Enum.GetName(typeof(Faction), actor.Faction) + "_" + actor.Definition.Name;
		}
		else
		{
			taskDependency.DebugName = "empty";
		}
	}

	public void AddFactionDependency(Faction faction, bool reserve = false)
	{
		AddDependency(VisualizationQueue.Instance.GetFactionDependencyObject(faction), reserve).DebugName = Enum.GetName(typeof(Faction), faction) + "_FACTION";
	}

	public void AddSpatialDependency(int x, int y, bool reserve = true)
	{
		SpatialLock spatialDependencyObject = VisualizationQueue.Instance.GetSpatialDependencyObject(x, y);
		if (spatialDependencyObject != null)
		{
			AddDependency(spatialDependencyObject, reserve).DebugName = "SpatialDependency_" + x + "_" + y;
		}
	}

	public void AddAllOtherFactionDependencies(Faction faction)
	{
		object factionDependencyObject = VisualizationQueue.Instance.GetFactionDependencyObject(faction);
		foreach (object factionDependencyObject2 in VisualizationQueue.Instance.GetFactionDependencyObjects())
		{
			if (factionDependencyObject2 != factionDependencyObject)
			{
				AddDependency(factionDependencyObject2, reserve: false).DebugName = Enum.GetName(typeof(Faction), VisualizationQueue.Instance.GetFactionForFactionDependencyObject(factionDependencyObject2)) + "_FACTION";
			}
		}
	}

	public void AddGlobalDependency()
	{
		AddDependency(VisualizationQueue.Instance.GlobalDependencyObject, reserve: false).DebugName = "GLOBAL";
	}

	public TaskDependency AddDependency(object dependencyObject, bool reserve = true, bool checkActiveTasks = true, bool checkQueuedTasks = true)
	{
		TaskDependency taskDependency = new TaskDependency(dependencyObject, reserve, checkActiveTasks, checkQueuedTasks);
		if (Dependencies == null)
		{
			Dependencies = new List<TaskDependency> { taskDependency };
		}
		else
		{
			Dependencies.Add(taskDependency);
		}
		return taskDependency;
	}

	public void AddDependencies<T>(List<T> inDependencies, bool reserve = true, bool checkActiveTasks = true, bool checkQueuedTasks = true)
	{
		if (Dependencies == null)
		{
			Dependencies = new List<TaskDependency>();
		}
		foreach (T inDependency in inDependencies)
		{
			TaskDependency item = new TaskDependency(inDependency, reserve, checkActiveTasks, checkQueuedTasks);
			Dependencies.Add(item);
		}
	}

	protected void ReleaseDependency(object dependencyObject, bool reservationOnly = true)
	{
		if (Dependencies == null)
		{
			return;
		}
		foreach (TaskDependency dependency in Dependencies)
		{
			if (dependency.DependencyObject == dependencyObject)
			{
				if (reservationOnly)
				{
					dependency.Reserve = false;
				}
				else
				{
					Dependencies.Remove(dependency);
				}
				break;
			}
		}
	}

	protected void ReleaseAllDependencies()
	{
		Dependencies = null;
	}

	public bool DependsOn(VisualizationTask task)
	{
		if (IsGlobalBlocker || task.IsGlobalBlocker)
		{
			return true;
		}
		if (task == null || Dependencies == null || task.Dependencies == null)
		{
			return false;
		}
		foreach (TaskDependency dependency in Dependencies)
		{
			if ((!dependency.CheckActiveTasks && task.IsActive) || (!dependency.CheckQueuedTasks && !task.IsActive))
			{
				continue;
			}
			foreach (TaskDependency dependency2 in task.Dependencies)
			{
				if (dependency2.Reserve && dependency.DependsOn(dependency2))
				{
					return true;
				}
			}
		}
		return false;
	}

	public virtual List<VisualizationTask> TasksToQueue()
	{
		return new List<VisualizationTask> { this };
	}

	public void Activate()
	{
		IsActive = true;
		Start();
	}

	public virtual void Queued()
	{
	}

	public virtual void Finished()
	{
	}

	public virtual void Start()
	{
	}

	public virtual void Stop()
	{
	}

	public virtual bool Update(float deltaTime)
	{
		return false;
	}

	public override string ToString()
	{
		string text = GetType().Name + " (IsActive = " + IsActive + ") Dependencies (";
		if (Dependencies != null)
		{
			for (int i = 0; i < Dependencies.Count; i++)
			{
				TaskDependency taskDependency = Dependencies[i];
				text = text + ((i > 0) ? ", " : "") + taskDependency.ToString();
			}
			return text + ")";
		}
		return text + "none)";
	}
}
