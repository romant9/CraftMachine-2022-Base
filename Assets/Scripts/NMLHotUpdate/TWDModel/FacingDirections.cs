using System;

namespace TWDModel
{
	public static class FacingDirections
	{
		public static FacingDirection FromRotationY(float rotationY)
		{
			int num = (int)Math.Round(rotationY / 90f) % 4;
			if (num < 0)
			{
				num += 4;
			}
			return (FacingDirection)num;
		}

		public static float ToRotationY(FacingDirection facing)
		{
			return (float)facing * 90f;
		}
	}
}
