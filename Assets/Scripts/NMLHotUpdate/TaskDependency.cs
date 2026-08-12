using System;

[Serializable]
public class TaskDependency
{
	public object DependencyObject;

	public bool Reserve;

	public bool CheckActiveTasks;

	public bool CheckQueuedTasks;

	public string DebugName { get; set; }

	public TaskDependency(object dependencyObject, bool reserve, bool checkActiveTasks, bool checkQueuedTasks)
	{
		DependencyObject = dependencyObject;
		Reserve = reserve;
		CheckActiveTasks = checkActiveTasks;
		CheckQueuedTasks = checkQueuedTasks;
		DebugName = "";
	}

	public bool DependsOn(TaskDependency otherDependency)
	{
		return DependencyObject == otherDependency.DependencyObject;
	}

	public override string ToString()
	{
		return DebugName + " Reserve: " + Reserve + " CheckActiveTasks: " + CheckActiveTasks + " CheckQueuedTasks: " + CheckQueuedTasks;
	}
}
