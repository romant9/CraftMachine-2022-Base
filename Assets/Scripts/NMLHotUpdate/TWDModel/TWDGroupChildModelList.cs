using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class TWDGroupChildModelList<T> : TWDGroupModelChild, IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable where T : TWDGroupModelChild
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
				Models[index] = value;
			}
		}

		public bool IsReadOnly => false;

		public TWDGroupChildModelList()
		{
			Models = new List<T>();
		}

		public IEnumerator GetEnumerator()
		{
			return Models.GetEnumerator();
		}

		public void Add(T model)
		{
			Models.Add(model);
		}

		public void Remove(T model)
		{
			Models.Remove(model);
		}

		public override void SetPlayerOwnerAndGameEconomyData(GameEconomyData ged, TWDGroupModelChild root, PlayerModel playerModel)
		{
			base.SetPlayerOwnerAndGameEconomyData(ged, root, playerModel);
			for (int i = 0; i < Models.Count; i++)
			{
				Models[i].SetPlayerOwnerAndGameEconomyData(ged, root, playerModel);
			}
		}

		public override void Start()
		{
			base.Start();
			for (int i = 0; i < Models.Count; i++)
			{
				Models[i].Start();
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
	}
}
