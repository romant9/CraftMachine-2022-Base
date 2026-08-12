using System.Collections.Generic;

namespace BaseModel
{
	public class ModelRandom
	{
		public int State { get; set; }

		public int CallCount { get; set; }

		public int InitialSeed { get; set; }

		public ModelRandom()
		{
		}

		public ModelRandom(int seed)
		{
			InitialSeed = seed;
			State = seed;
			Next();
		}

		public float Next()
		{
			CallCount++;
			State = (State * 1103515245 + 12345) & 0x7FFFFFFF;
			return (float)((State >> 4) & 0xFFFFF) / 1048576f;
		}

		public int Next(int n)
		{
			CallCount++;
			State = (State * 1103515245 + 12345) & 0x7FFFFFFF;
			return (State >> 4) % n;
		}

		public T GetRandomElement<T>(T[] array)
		{
			int num = Next(array.Length);
			return array[num];
		}

		public T GetRandomElement<T>(List<T> list, bool remove)
		{
			int index = Next(list.Count);
			T result = list[index];
			if (remove)
			{
				list.RemoveAt(index);
			}
			return result;
		}

		public List<T> GetRandomRange<T>(List<T> list, int count)
		{
			List<T> list2 = new List<T>();
			if (list.Count < count)
			{
				list2 = new List<T>(list);
			}
			else
			{
				for (int i = 0; i < count; i++)
				{
					list2.Add(GetRandomElement(list, remove: false));
				}
			}
			return list2;
		}

		public void ShuffleArray<T>(T[] arr)
		{
			for (int i = 0; i < arr.Length - 1; i++)
			{
				int num = i + GetRandomInRange(0, arr.Length - i - 1);
				T val = arr[i];
				arr[i] = arr[num];
				arr[num] = val;
			}
		}

		public int GetRandomInRange(int min, int max)
		{
			return Next(max - min + 1) + min;
		}



		#region mycode
		public ModelRandom(ModelRandom random)
		{
			State = random.State;
			CallCount = random.CallCount;
			InitialSeed = random.InitialSeed;
		}

		public void SetNewState(int newState)
		{
			var randomOld = new ModelRandom(this);
			int count = 0;
			while (State != newState)
			{
				Next();
				count++;
				if (count > 500)
				{
					this.State = randomOld.State;
					this.CallCount = randomOld.CallCount;
					this.InitialSeed = randomOld.InitialSeed;
					string msg = "Не удалось дойти до значения за 500 итераций";
					DebugTWD.Log(msg);
					AlertPopup.ShowPopupGetText("", msg, "Button.Ok", null);
					break;
				}
			}
		}
		#endregion
	}
}
