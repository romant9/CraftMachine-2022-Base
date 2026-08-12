namespace TWDModel
{
	public struct DistanceFieldOptions
	{
		public FixedPoint OccupancyCostMultiplier;

		public ActorModel IgnoreActorOccupancy;

		public ActorModel IgnoreFactionOccupancy;

		public FixedPoint MaxDistance;

		public DistanceFieldOptions(float occupancyCostMultiplier = 1f, ActorModel ignoreActorOccupancy = null, ActorModel ignoreFactionOccupancy = null, float maxDistance = 1000f)
		{
			OccupancyCostMultiplier = occupancyCostMultiplier;
			IgnoreActorOccupancy = ignoreActorOccupancy;
			IgnoreFactionOccupancy = ignoreFactionOccupancy;
			MaxDistance = maxDistance;
		}
	}
}
