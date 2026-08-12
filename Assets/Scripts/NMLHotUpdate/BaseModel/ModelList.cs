using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BaseModel
{
	public class ModelList<T> : ModelObject, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : ModelObject
	{
		public List<T> Models { get; set; }

		[JsonIgnore]
		public int Count => Models.Count;

		[JsonIgnore]
		public T this[int index]
		{
			get
			{
				return Models[index];
			}
			set
			{
				if (value == null)
				{
					Models.RemoveAt(index);
				}
				else
				{
					Models[index] = value;
				}
			}
		}

		public bool IsReadOnly => false;

		public ModelList()
		{
			Models = new List<T>();
		}

		public IEnumerator GetEnumerator()
		{
			return Models.GetEnumerator();
		}

		public void Add(T model)
		{
			if (model != null)
			{
				Models.Add(model);
			}
		}

		public void Remove(T model)
		{
			Models.Remove(model);
		}

		public void RemoveAll(Predicate<T> match)
		{
			Models.RemoveAll(match);
		}

		public T Get(int modelId)
		{
			for (int i = 0; i < Models.Count; i++)
			{
				T val = Models[i];
				if (val.ModelId == modelId)
				{
					return val;
				}
			}
			return null;
		}

		public override bool IsValid()
		{
			for (int i = 0; i < Models.Count; i++)
			{
				if (!Models[i].Validate())
				{
					return false;
				}
			}
			return true;
		}

		public override void SetManager(ModelManager manager)
		{
			base.Manager = manager;
			for (int i = 0; i < Models.Count; i++)
			{
				Models[i].SetManager(manager);
			}
		}

		public override void Start()
		{
			base.ModelId = base.Manager.RegisterModel(this);
			started = true;
			for (int i = 0; i < Models.Count; i++)
			{
				Models[i].Start();
			}
		}

		public override void Tick(long deltaTime)
		{
			for (int i = 0; i < Models.Count; i++)
			{
				Models[i].Tick(deltaTime);
			}
		}

		public int IndexOf(T item)
		{
			return Models.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			Models.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			Models.RemoveAt(index);
		}

		public void Clear()
		{
			Models.Clear();
		}

		public bool Contains(T item)
		{
			return Models.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Models.CopyTo(array, arrayIndex);
		}

		bool ICollection<T>.Remove(T item)
		{
			return Models.Remove(item);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return Models.GetEnumerator();
		}

		public T Find(Predicate<T> match)
		{
			return Models.Find(match);
		}

		public bool RemoveAfter(int index)
		{
			Models = Models.Take(index + 1).ToList();
			return true;
		}
	}
}
