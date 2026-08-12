using System;

namespace TWDModel
{
	[Serializable]
	public class PlayerEmblem
	{
		public int IconIndex;

		public int BorderIndex;

		public int ColorIndex;

		public PlayerEmblem()
		{
		}

		public PlayerEmblem(PlayerEmblem other)
		{
			IconIndex = other.IconIndex;
			BorderIndex = other.BorderIndex;
			ColorIndex = other.ColorIndex;
		}

		public override bool Equals(object other)
		{
			if (!(other is PlayerEmblem playerEmblem))
			{
				return false;
			}
			if (IconIndex == playerEmblem.IconIndex && BorderIndex == playerEmblem.BorderIndex)
			{
				return ColorIndex == playerEmblem.ColorIndex;
			}
			return false;
		}
	}
}
