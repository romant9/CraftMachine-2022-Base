using System.Collections.Generic;

namespace TWDModel
{
	public class PlayerInputMetricsData
	{
		public int MovesToNonInstructedCells;

		public int MovesToEmptyCells;

		public int MovesNextToWalker;

		public int MovesNextToExit;

		public int SwipesOnSpentSurvivors;

		public int SwipesNextToSurvivor;

		public int SwipesOutOfMoveRange;

		public int TapsOnSurvivor;

		public int TapsOnWalker;

		public int TapsOnEmptyCell;

		public int TapsOutsideGrid;

		public Dictionary<string, string> ToDictionary()
		{
			return new Dictionary<string, string>
			{
				{
					"MovesToNonInstructedCells",
					MovesToNonInstructedCells.ToString()
				},
				{
					"MovesToEmptyCells",
					MovesToEmptyCells.ToString()
				},
				{
					"MovesNextToWalker",
					MovesNextToWalker.ToString()
				},
				{
					"MovesNextToExit",
					MovesNextToExit.ToString()
				},
				{
					"SwipesOnSpentSurvivors",
					SwipesOnSpentSurvivors.ToString()
				},
				{
					"SwipesNextToSurvivor",
					SwipesNextToSurvivor.ToString()
				},
				{
					"SwipesOutOfMoveRange",
					SwipesOutOfMoveRange.ToString()
				},
				{
					"TapsOnSurvivor",
					TapsOnSurvivor.ToString()
				},
				{
					"TapsOnWalker",
					TapsOnWalker.ToString()
				},
				{
					"TapsOnEmptyCell",
					TapsOnEmptyCell.ToString()
				},
				{
					"TapsOutsideGrid",
					TapsOutsideGrid.ToString()
				}
			};
		}
	}
}
