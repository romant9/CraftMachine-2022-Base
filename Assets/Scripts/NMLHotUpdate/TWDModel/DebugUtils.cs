using System;
using System.Diagnostics;

namespace TWDModel
{
	public class DebugUtils
	{
		[Conditional("UNITY_EDITOR")]
		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				throw new Exception("Assertion failed: " + message);
			}
		}

		[Conditional("UNITY_EDITOR")]
		public static void Assert(bool condition)
		{
			if (!condition)
			{
				throw new Exception("Assertion failed!");
			}
		}
	}
}
