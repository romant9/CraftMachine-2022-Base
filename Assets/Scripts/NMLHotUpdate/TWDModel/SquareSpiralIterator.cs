namespace TWDModel
{
	public class SquareSpiralIterator
	{
		private enum Direction
		{
			Up = 0,
			Right = 1,
			Down = 2,
			Left = 3
		}

		private int x;

		private int y;

		private int step;

		private int numSteps;

		private int numStepsWidth;

		private int numStepsHeight;

		private Direction direction;

		private bool inwards;

		public int CirclesDone { get; private set; }

		public SquareSpiralIterator(int x, int y)
		{
			this.x = x;
			this.y = y;
			direction = Direction.Right;
			step = -1;
			numSteps = 1;
			inwards = false;
		}

		public SquareSpiralIterator(int x, int y, int width, int height)
		{
			this.x = x;
			this.y = y;
			direction = Direction.Right;
			step = 0;
			numStepsWidth = width;
			numStepsHeight = height;
			inwards = true;
		}

		public int GetX()
		{
			return x;
		}

		public int GetY()
		{
			return y;
		}

		public void MoveNext()
		{
			if (!inwards)
			{
				if (++step >= numSteps)
				{
					step = 0;
					direction = GetNextDirection();
					if (direction == Direction.Right)
					{
						CirclesDone++;
					}
					if (direction == Direction.Left || direction == Direction.Right)
					{
						numSteps++;
					}
				}
			}
			else if ((direction == Direction.Left || direction == Direction.Right) && ++step >= numStepsWidth)
			{
				step = 0;
				numStepsWidth--;
				numStepsHeight--;
				direction = GetNextDirection();
			}
			else if ((direction == Direction.Up || direction == Direction.Down) && ++step >= numStepsHeight)
			{
				step = 0;
				direction = GetNextDirection();
				if (direction == Direction.Right)
				{
					CirclesDone++;
				}
			}
			switch (direction)
			{
			case Direction.Up:
				y--;
				break;
			case Direction.Left:
				x--;
				break;
			case Direction.Down:
				y++;
				break;
			case Direction.Right:
				x++;
				break;
			}
		}

		private Direction GetNextDirection()
		{
			return direction switch
			{
				Direction.Up => Direction.Right, 
				Direction.Right => Direction.Down, 
				Direction.Down => Direction.Left, 
				Direction.Left => Direction.Up, 
				_ => Direction.Up, 
			};
		}
	}
}
