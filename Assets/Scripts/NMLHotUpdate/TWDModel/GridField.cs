namespace TWDModel
{
	public class GridField<T>
	{
		private T[] contents;

		private int width;

		public T DefaultValue { get; set; }

		public int Length
		{
			get
			{
				if (contents == null)
				{
					return 0;
				}
				return contents.Length;
			}
		}

		public T this[GridCoordinate coordinate]
		{
			get
			{
				int num = coordinate.Y * width + coordinate.X;
				if (num < 0 || num >= contents.Length)
				{
					return default(T);
				}
				return contents[num];
			}
			set
			{
				int num = coordinate.Y * width + coordinate.X;
				if (num >= 0 && num < contents.Length)
				{
					contents[num] = value;
					IsClear = false;
				}
			}
		}

		public T this[int index]
		{
			get
			{
				if (index >= 0 && index < contents.Length)
				{
					return contents[index];
				}
				return default(T);
			}
			set
			{
				if (index >= 0 && index < contents.Length)
				{
					contents[index] = value;
					IsClear = false;
				}
			}
		}

		public bool IsClear { get; set; }

		public GridField(int width, int height, T defaultValue)
		{
			this.width = width;
			int num = width * height;
			contents = new T[num];
			DefaultValue = defaultValue;
			Clear();
		}

		public void Clear()
		{
			UtilsArray.Fill(contents, DefaultValue);
			IsClear = true;
		}

		public int GetWidth()
		{
			return width;
		}
	}
}
