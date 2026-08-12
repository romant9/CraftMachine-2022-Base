using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class ObjectPool<T> where T : new()
	{
		private readonly Stack<T> stack = new Stack<T>();

		private readonly Action<T> onGetAction;

		private readonly Action<T> onReleaseAction;

		public int CountAll { get; private set; }

		public int CountActive => CountAll - CountInactive;

		public int CountInactive => stack.Count;

		public ObjectPool(Action<T> onGetAction, Action<T> onReleaseAction)
		{
			this.onGetAction = onGetAction;
			this.onReleaseAction = onReleaseAction;
		}

		public T Get()
		{
			T val;
			if (stack.Count == 0)
			{
				val = new T();
				CountAll++;
			}
			else
			{
				val = stack.Pop();
			}
			if (onGetAction != null)
			{
				onGetAction(val);
			}
			return val;
		}

		public void Release(T element)
		{
			if (stack.Count > 0)
			{
				_ = (object)stack.Peek();
				_ = (object)element;
			}
			if (onReleaseAction != null)
			{
				onReleaseAction(element);
			}
			stack.Push(element);
		}
	}
}
