using System.Collections.Generic;

namespace TWDModel
{
	public class ActorFootprint
	{
		public static readonly ActorFootprint SingleCell = new ActorFootprint(new List<GridCoordinate>
		{
			new GridCoordinate(0, 0)
		}, new GridCoordinate(0, 0));

		public List<GridCoordinate> BaseOffsets { get; private set; }

		public GridCoordinate AnchorOffset { get; private set; }

		public ActorFootprint(List<GridCoordinate> baseOffsets, GridCoordinate anchorOffset)
		{
			BaseOffsets = baseOffsets;
			AnchorOffset = anchorOffset;
		}

		public List<GridCoordinate> GetOccupiedCells(GridCoordinate anchor, FacingDirection facing)
		{
			List<GridCoordinate> list = new List<GridCoordinate>(BaseOffsets.Count);
			for (int i = 0; i < BaseOffsets.Count; i++)
			{
				GridCoordinate gridCoordinate = RotateOffset(BaseOffsets[i] - AnchorOffset, facing);
				list.Add(anchor + gridCoordinate);
			}
			return list;
		}

		public static GridCoordinate RotateOffset(GridCoordinate offset, FacingDirection facing)
		{
			return facing switch
			{
				FacingDirection.North => offset, 
				FacingDirection.East => new GridCoordinate(-offset.Y, offset.X), 
				FacingDirection.South => new GridCoordinate(-offset.X, -offset.Y), 
				FacingDirection.West => new GridCoordinate(offset.Y, -offset.X), 
				_ => offset, 
			};
		}

		public static GridCoordinate UnrotateOffset(GridCoordinate offset, FacingDirection facing)
		{
			return facing switch
			{
				FacingDirection.North => offset, 
				FacingDirection.East => new GridCoordinate(offset.Y, -offset.X), 
				FacingDirection.South => new GridCoordinate(-offset.X, -offset.Y), 
				FacingDirection.West => new GridCoordinate(-offset.Y, offset.X), 
				_ => offset, 
			};
		}

		public static ActorFootprint CreateFromDimensions(int width, int height, GridCoordinate anchorOffset)
		{
			List<GridCoordinate> list = new List<GridCoordinate>(width * height);
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					list.Add(new GridCoordinate(i, j));
				}
			}
			return new ActorFootprint(list, anchorOffset);
		}

		public static ActorFootprint CreateTankFootprint()
		{
			return CreateFromDimensions(3, 5, new GridCoordinate(1, 2));
		}

		public static ActorFootprint CreateFromActorDefinition(ActorDefinition definition)
		{
			if (definition == null || definition.FootprintWidth <= 0 || definition.FootprintHeight <= 0)
			{
				return CreateTankFootprint();
			}
			return CreateFromDimensions(definition.FootprintWidth, definition.FootprintHeight, new GridCoordinate(definition.FootprintAnchorX, definition.FootprintAnchorY));
		}
	}
}
