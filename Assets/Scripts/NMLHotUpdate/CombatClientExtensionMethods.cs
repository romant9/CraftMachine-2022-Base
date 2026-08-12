using TWDModel;

public static class CombatClientExtensionMethods
{
	public static GridCoordinate GetClosestFreeNeighbor(this GridField<FixedPoint> distanceField, GridCoordinate coordinate, ActorModel movingActor, FixedPoint maxDistance, InteractiveObjectModel interactiveObject, bool checkVisibility, bool edgeCheck = true)
	{
		return CombatHelpers.GetClosestFreeNeighbor(GameManager.Instance.playerModel.Combat, distanceField, coordinate, movingActor, maxDistance, interactiveObject, checkVisibility, edgeCheck);
	}
}
