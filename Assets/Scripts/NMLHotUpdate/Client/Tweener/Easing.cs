using System.Runtime.InteropServices;

namespace Client.Tweener
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct Easing
	{
		public enum All
		{
			BackEaseOutIn = 0,
			BackEaseInOut = 1,
			BackEaseIn = 2,
			BackEaseOut = 3,
			CubicEaseOut = 4,
			ExpoEaseOut = 5,
			Linear = 6,
			CubicEaseOutIn = 7,
			CubicEaseInOut = 8,
			BounceEaseOutIn = 9,
			BounceEaseInOut = 10,
			BounceEaseIn = 11,
			BounceEaseOut = 12,
			ElasticEaseOutIn = 13,
			ElasticEaseInOut = 14,
			ElasticEaseIn = 15,
			ElasticEaseOut = 16,
			ExpoEaseOutIn = 17,
			ExpoEaseInOut = 18,
			ExpoEaseIn = 19
		}
	}
}
