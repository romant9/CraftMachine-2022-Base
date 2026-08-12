using BaseModel;

namespace TWDModel
{
	public class MovableModel : TWDModelObjectWithViewId, InteractionReceiver
	{
		public const string ChangeIsMoved = "IsMoved";

		public Direction Direction;

		public int Distance;

		public bool IsMoved;

		public bool CheckCollidersInRegion;

		[IgnoreModelProperty]
		public RegionModel RegionToBeClear { get; set; }

		public MovableModel()
		{
		}

		public MovableModel(string viewId, Direction direction, int distance, RegionModel regionToBeClear, bool checkCollidersInRegion)
		{
			base.ViewId = viewId;
			Direction = direction;
			Distance = distance;
			RegionToBeClear = regionToBeClear;
			CheckCollidersInRegion = checkCollidersInRegion;
		}

		public override void Initialize()
		{
			base.Initialize();
			IsMoved = false;
		}

		public void OnInteractionStep(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnInteractionCanceled(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnAttacked(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnDestroyed(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public void OnInteractionCompleted(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			Move(instigator);
		}

		public GridCoordinate GetTargetCoordinate(GridCoordinate pushStart)
		{
			GridCoordinate result = pushStart;
			switch (Direction)
			{
			case Direction.Up:
				result = new GridCoordinate(result.X, result.Y - Distance);
				break;
			case Direction.Right:
				result = new GridCoordinate(result.X + Distance, result.Y);
				break;
			case Direction.Down:
				result = new GridCoordinate(result.X, result.Y + Distance);
				break;
			case Direction.Left:
				result = new GridCoordinate(result.X - Distance, result.Y);
				break;
			}
			return result;
		}

		public void Reset()
		{
			if (IsMoved)
			{
				IsMoved = false;
			}
		}

		public void Move(ActorModel actorMoving)
		{
			if (!IsMoved)
			{
				IsMoved = true;
				NotifyChange("IsMoved", actorMoving);
				actorMoving.GridCoordinate = GetTargetCoordinate(actorMoving.GridCoordinate);
				base.manager.CombatModel.UpdateAllActorsVisibility();
				base.manager.CombatModel.UpdateObjectsVisibility();
				base.manager.CombatModel.UpdateOccupiers();
			}
		}

		public bool CheckClearance()
		{
			if (RegionToBeClear == null)
			{
				return true;
			}
			for (int i = 0; i < RegionToBeClear.Location.Coordinates.Count; i++)
			{
				GridCoordinate gridCoordinate = RegionToBeClear.Location.Coordinates[i];
				if (base.manager.CombatModel.GetOccupier(gridCoordinate) != null)
				{
					return false;
				}
				if (CheckCollidersInRegion && base.manager.CombatModel.IsBlocked(gridCoordinate))
				{
					return false;
				}
			}
			return true;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
