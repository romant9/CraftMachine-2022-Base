using System.Collections.Generic;

namespace TWDModel
{
	public class OpenList
	{
		private List<OpenListEntry> _heap = new List<OpenListEntry>();

		public int Count => _heap.Count;

		public void Clear()
		{
			_heap.Clear();
		}

		public void Enqueue(GridCoordinate inCoordinate, FixedPoint inDistance)
		{
			OpenListEntry item = new OpenListEntry
			{
				coordinate = inCoordinate,
				distance = inDistance
			};
			_heap.Add(item);
			HeapifyUp(_heap.Count - 1);
		}

		public OpenListEntry Dequeue()
		{
			OpenListEntry result = _heap[0];
			int index = _heap.Count - 1;
			_heap[0] = _heap[index];
			_heap.RemoveAt(index);
			if (_heap.Count > 0)
			{
				HeapifyDown(0);
			}
			return result;
		}

		private void HeapifyUp(int index)
		{
			while (index > 0)
			{
				int num = (index - 1) / 2;
				if (_heap[index].distance < _heap[num].distance)
				{
					Swap(index, num);
					index = num;
					continue;
				}
				break;
			}
		}

		private void HeapifyDown(int index)
		{
			int count = _heap.Count;
			while (true)
			{
				int num = index;
				int num2 = 2 * index + 1;
				int num3 = 2 * index + 2;
				if (num2 < count && _heap[num2].distance < _heap[num].distance)
				{
					num = num2;
				}
				if (num3 < count && _heap[num3].distance < _heap[num].distance)
				{
					num = num3;
				}
				if (num != index)
				{
					Swap(index, num);
					index = num;
					continue;
				}
				break;
			}
		}

		private void Swap(int i, int j)
		{
			OpenListEntry value = _heap[i];
			_heap[i] = _heap[j];
			_heap[j] = value;
		}
	}
}
