using System;
using System.Collections.Generic;
using BaseModel;

public class UtilsArray
{
	public static void Fill<T>(T[] array, T value)
	{
		if (array.Length != 0)
		{
			array[0] = value;
			int num;
			for (num = 1; num <= array.Length / 2; num *= 2)
			{
				Array.Copy(array, 0, array, num, num);
			}
			Array.Copy(array, 0, array, num, array.Length - num);
		}
	}

	public static void ShuffleList<T>(IList<T> list, ModelRandom random)
	{
		int num = list.Count;
		while (num > 1)
		{
			num--;
			int index = (int)((float)num * random.Next() + 0.5f);
			T value = list[index];
			list[index] = list[num];
			list[num] = value;
		}
	}
}
