using System;

namespace MIConvexHull
{
	internal sealed class VertexBuffer
	{
		private VertexWrap[] items;

		private int count;

		private int capacity;

		public int Count => count;

		public VertexWrap this[int i] => items[i];

		private void EnsureCapacity()
		{
			if (count + 1 > capacity)
			{
				if (capacity == 0)
				{
					capacity = 4;
				}
				else
				{
					capacity = 2 * capacity;
				}
				Array.Resize(ref items, capacity);
			}
		}

		public void Add(VertexWrap item)
		{
			EnsureCapacity();
			items[count++] = item;
		}

		public void Clear()
		{
			count = 0;
		}
	}
}
