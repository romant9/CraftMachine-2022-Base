using System;
using System.Collections.Generic;

public static class ObjectCacheKeyValue<T, V> where T : class, new()
{
	private static readonly Dictionary<string, T> pool = new Dictionary<string, T>();

	public static T Get(string key, Func<V, T> createNewInstance, V param)
	{
		T value = null;
		if (!pool.TryGetValue(key, out value))
		{
			value = createNewInstance(param);
			pool.Add(key, value);
		}
		return value;
	}

	public static void Release(string key)
	{
		pool.Remove(key);
	}

	public static void Clean()
	{
		pool.Clear();
	}
}
