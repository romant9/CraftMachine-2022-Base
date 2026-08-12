using System.Collections;
using System.Collections.Generic;

namespace TWDModel
{
	public class Tuple<T1, T2>
	{
		public T1 First;

		public T2 Second;

		private static readonly IEqualityComparer Item1Comparer = EqualityComparer<T1>.Default;

		private static readonly IEqualityComparer Item2Comparer = EqualityComparer<T2>.Default;

		public Tuple()
		{
			First = default(T1);
			Second = default(T2);
		}

		public Tuple(T1 first, T2 second)
		{
			First = first;
			Second = second;
		}

		public override string ToString()
		{
			return $"<{First}, {Second}>";
		}

		public static bool operator ==(Tuple<T1, T2> a, Tuple<T1, T2> b)
		{
			if (IsNull(a) && !IsNull(b))
			{
				return false;
			}
			if (!IsNull(a) && IsNull(b))
			{
				return false;
			}
			if (IsNull(a) && IsNull(b))
			{
				return true;
			}
			ref T1 first = ref a.First;
			object obj = b.First;
			if (first.Equals(obj))
			{
				ref T2 second = ref a.Second;
				object obj2 = b.Second;
				return second.Equals(obj2);
			}
			return false;
		}

		public static bool operator !=(Tuple<T1, T2> a, Tuple<T1, T2> b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return (17 * 23 + First.GetHashCode()) * 23 + Second.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Tuple<T1, T2> tuple))
			{
				return false;
			}
			if (Item1Comparer.Equals(First, tuple.First))
			{
				return Item2Comparer.Equals(Second, tuple.Second);
			}
			return false;
		}

		private static bool IsNull(object obj)
		{
			return obj == null;
		}
	}
	public class Tuple<T1, T2, T3>
	{
		public T1 First;

		public T2 Second;

		public T3 Third;

		private static readonly IEqualityComparer Item1Comparer = EqualityComparer<T1>.Default;

		private static readonly IEqualityComparer Item2Comparer = EqualityComparer<T2>.Default;

		public Tuple()
		{
			First = default(T1);
			Second = default(T2);
			Third = default(T3);
		}

		public Tuple(T1 first, T2 second, T3 third)
		{
			First = first;
			Second = second;
			Third = third;
		}

		public override string ToString()
		{
			return $"<{First}, {Second}, {Third}>";
		}

		public static bool operator ==(Tuple<T1, T2, T3> a, Tuple<T1, T2, T3> b)
		{
			if (IsNull(a) && !IsNull(b))
			{
				return false;
			}
			if (!IsNull(a) && IsNull(b))
			{
				return false;
			}
			if (IsNull(a) && IsNull(b))
			{
				return true;
			}
			ref T1 first = ref a.First;
			object obj = b.First;
			if (first.Equals(obj))
			{
				ref T2 second = ref a.Second;
				object obj2 = b.Second;
				if (second.Equals(obj2))
				{
					ref T3 third = ref a.Third;
					object obj3 = b.Third;
					return third.Equals(obj3);
				}
			}
			return false;
		}

		public static bool operator !=(Tuple<T1, T2, T3> a, Tuple<T1, T2, T3> b)
		{
			return !(a == b);
		}

		public override int GetHashCode()
		{
			return ((17 * 23 + First.GetHashCode()) * 23 + Second.GetHashCode()) * 23 + Third.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Tuple<T1, T2, T3> tuple))
			{
				return false;
			}
			if (Item1Comparer.Equals(First, tuple.First) && Item2Comparer.Equals(Second, tuple.Second))
			{
				return Item2Comparer.Equals(Third, tuple.Third);
			}
			return false;
		}

		private static bool IsNull(object obj)
		{
			return obj == null;
		}
	}
}
