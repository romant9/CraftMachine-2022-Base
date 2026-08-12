using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using TWDModel;

public static class UtilsMath
{
	public static float Min(float f1, float f2)
	{
		if (!(f1 < f2))
		{
			return f2;
		}
		return f1;
	}

	public static float Max(float f1, float f2)
	{
		if (!(f1 > f2))
		{
			return f2;
		}
		return f1;
	}

	public static float Clamp(float value, float min, float max)
	{
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	public static int Min(int i1, int i2)
	{
		if (i1 >= i2)
		{
			return i2;
		}
		return i1;
	}

	public static int Max(int i1, int i2)
	{
		if (i1 <= i2)
		{
			return i2;
		}
		return i1;
	}

	public static int Clamp(int value, int min, int max)
	{
		if (value < min)
		{
			return min;
		}
		if (value > max)
		{
			return max;
		}
		return value;
	}

	public static float Map(float value, float inA, float inB, float outA, float outB)
	{
		if (value < inA)
		{
			return outA;
		}
		if (value > inB)
		{
			return outB;
		}
		if (inB == inA)
		{
			return outA;
		}
		return (value - inA) / (inB - inA) * (outB - outA) + outA;
	}

	private static T[] ExtractEnumValues<T>()
	{
		Array values = Enum.GetValues(typeof(T));
		T[] array = new T[values.Length];
		values.CopyTo(array, 0);
		return array;
	}

	public static int WeightedRandom(this ModelRandom modelRandom, FixedPoint[] weights)
	{
		if (weights.Length == 0)
		{
			return -1;
		}
		FixedPoint fixedPoint = 0L;
		for (int i = 0; i < weights.Length; i++)
		{
			fixedPoint += weights[i];
		}
		if (fixedPoint == 0L)
		{
			return modelRandom.Next(weights.Length);
		}
		FixedPoint fixedPoint2 = modelRandom.Next();
		FixedPoint fixedPoint3 = 0L;
		for (int j = 0; j < weights.Length; j++)
		{
			fixedPoint3 += weights[j] / fixedPoint;
			if (fixedPoint2 < fixedPoint3)
			{
				return j;
			}
		}
		return weights.Length - 1;
	}

	public static float AverageOfList(List<int> list)
	{
		if (list == null)
		{
			return 0f;
		}
		if (list.Count > 0)
		{
			float num = 0f;
			for (int i = 0; i < list.Count; i++)
			{
				num += (float)list[i];
			}
			return num / (float)list.Count;
		}
		return 0f;
	}

	public static T GetRandomEnum<T>(this ModelRandom random)
	{
		T[] array = ExtractEnumValues<T>();
		return random.GetRandomElement(array);
	}

	public static T WeightedRandomEnum<T>(this ModelRandom modelRandom, FixedPoint[] weights)
	{
		T[] array = ExtractEnumValues<T>();
		int num = modelRandom.WeightedRandom(weights);
		return array[num];
	}

	public static List<T> WeightedRandomList<T>(this ModelRandom modelRandom, List<T> items, int count, Func<T, FixedPoint> action, bool isRepeat = true)
	{
		List<T> list = new List<T>();
		List<T> list2 = new List<T>();
		list2.AddRange(items);
		if (list2.Count <= 0)
		{
			return list;
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(list2[modelRandom.WeightedRandom(list2.Select((T x) => action(x)).ToArray())]);
			if (!isRepeat)
			{
				list2.Remove(list[i]);
				if (list2.Count <= 0)
				{
					return list;
				}
			}
		}
		return list;
	}

	public static void StableSort<T>(this List<T> input, Comparison<T> comparison = null)
	{
		if (input == null)
		{
			throw new ArgumentNullException("input");
		}
		if (comparison == null)
		{
			comparison = Comparer<T>.Default.Compare;
		}
		if (input.Count <= 16)
		{
			InsertionSort(input, comparison);
			return;
		}
		T[] temp = new T[input.Count];
		MergeSort(input, temp, 0, input.Count - 1, comparison);
	}

	private static void InsertionSort<T>(List<T> array, Comparison<T> comparison)
	{
		for (int i = 1; i < array.Count; i++)
		{
			T val = array[i];
			int num = i - 1;
			while (num >= 0 && comparison(array[num], val) > 0)
			{
				array[num + 1] = array[num];
				num--;
			}
			array[num + 1] = val;
		}
	}

	private static void MergeSort<T>(List<T> array, T[] temp, int left, int right, Comparison<T> comparison)
	{
		if (left < right)
		{
			int num = (left + right) / 2;
			MergeSort(array, temp, left, num, comparison);
			MergeSort(array, temp, num + 1, right, comparison);
			Merge(array, temp, left, num, right, comparison);
		}
	}

	private static void Merge<T>(List<T> array, T[] temp, int left, int mid, int right, Comparison<T> comparison)
	{
		int num = left;
		int num2 = mid + 1;
		int num3 = left;
		while (num <= mid && num2 <= right)
		{
			if (comparison(array[num], array[num2]) <= 0)
			{
				temp[num3++] = array[num++];
			}
			else
			{
				temp[num3++] = array[num2++];
			}
		}
		while (num <= mid)
		{
			temp[num3++] = array[num++];
		}
		while (num2 <= right)
		{
			temp[num3++] = array[num2++];
		}
		for (int i = left; i <= right; i++)
		{
			array[i] = temp[i];
		}
	}

	public static int SplitValueStep(ref FixedPoint floatSum, ref int integerSum, FixedPoint valueToAdd)
	{
		floatSum += valueToAdd;
		int num = (int)FixedPoint.Round(floatSum - integerSum);
		integerSum += num;
		return num;
	}

	public static void SplitValue(int baseValue, int firstPercentage, int secondPercentage, int thirdPercentage, out int firstValue, out int secondValue, out int thirdValue)
	{
		FixedPoint floatSum = 0.0;
		int integerSum = 0;
		firstValue = SplitValueStep(ref floatSum, ref integerSum, (FixedPoint)baseValue * (FixedPoint)firstPercentage / 100.0);
		secondValue = SplitValueStep(ref floatSum, ref integerSum, (FixedPoint)baseValue * (FixedPoint)secondPercentage / 100.0);
		thirdValue = SplitValueStep(ref floatSum, ref integerSum, (FixedPoint)baseValue * (FixedPoint)thirdPercentage / 100.0);
	}

	public static bool BitmaskContains(int searchValue, int bitmask)
	{
		return (searchValue & bitmask) != 0;
	}

	public static void BitmaskSet(int value, ref int bitmask)
	{
		bitmask = value | bitmask;
	}

	public static string GetIntAsBinaryString(int n)
	{
		char[] array = new char[32];
		int num = 31;
		for (int i = 0; i < 32; i++)
		{
			if ((n & (1 << i)) != 0)
			{
				array[num] = '1';
			}
			else
			{
				array[num] = '0';
			}
			num--;
		}
		return new string(array);
	}

	public static int RountToInt(float value)
	{
		return (int)(value + 0.5f);
	}
}
