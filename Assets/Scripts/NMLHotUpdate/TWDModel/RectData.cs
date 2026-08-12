using System;

namespace TWDModel
{
	[Serializable]
	public class RectData
	{
		public int X;

		public int Y;

		public int Width;

		public int Height;

		public RectData()
		{
		}

		public RectData(int x, int y, int width, int height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}

		public void Encapsulate(int x, int y)
		{
			int num = Math.Max(X + Width - 1, x);
			int num2 = Math.Max(Y + Height - 1, y);
			X = Math.Min(x, X);
			Y = Math.Min(y, Y);
			Width = num - X + 1;
			Height = num2 - Y + 1;
		}

		public bool IsInside(int x, int y)
		{
			if (x >= X && x < X + Width && y >= Y)
			{
				return y < Y + Height;
			}
			return false;
		}

		public void Grow(int width, int height)
		{
			X -= width;
			Width += width * 2;
			Y -= height;
			Height += height * 2;
		}

		public override string ToString()
		{
			return "(x,y): (" + X + "," + Y + ") - (w,h): (" + Width + "," + Height + ")";
		}
	}
}
