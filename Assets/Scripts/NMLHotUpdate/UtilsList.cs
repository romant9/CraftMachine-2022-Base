using System.Collections.Generic;

public class UtilsList
{
	public interface IDeepClonable<T> where T : class
	{
		T DeepClone();
	}

	public static List<T> DeepCloneList<T>(List<T> toClone) where T : class, IDeepClonable<T>
	{
		List<T> list = new List<T>();
		foreach (T item in toClone)
		{
			list.Add(item.DeepClone());
		}
		return list;
	}

	public static List<T> CreateDistinctList<T>(IList<T> list)
	{
		HashSet<T> hashSet = new HashSet<T>();
		for (int i = 0; i < list.Count; i++)
		{
			if (!hashSet.Contains(list[i]))
			{
				hashSet.Add(list[i]);
			}
		}
		return new List<T>(hashSet);
	}
}
