using System.Collections.Generic;

namespace TWDModel
{
	public static class ListPool<T>
	{
		private static readonly ObjectPool<List<T>> listPool = new ObjectPool<List<T>>(null, delegate(List<T> l)
		{
			l.Clear();
		});

		public static List<T> Get()
		{
			return listPool.Get();
		}

		public static void Release(List<T> toRelease)
		{
			listPool.Release(toRelease);
		}
	}
}
