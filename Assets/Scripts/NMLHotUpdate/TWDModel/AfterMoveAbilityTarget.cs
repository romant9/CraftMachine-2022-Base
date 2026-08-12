namespace TWDModel
{
	public struct AfterMoveAbilityTarget
	{
		private GridCoordinate abilityTarget;

		private GridCoordinate moveCoordinate;

		public GridCoordinate AbilityTarget => abilityTarget;

		public GridCoordinate MoveCoordinate => moveCoordinate;

		public AfterMoveAbilityTarget(GridCoordinate target, GridCoordinate coordinate)
		{
			abilityTarget = target;
			moveCoordinate = coordinate;
		}
	}
}
