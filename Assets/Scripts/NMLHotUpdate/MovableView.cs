using BaseModel;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class MovableView : ModelView<MovableModel>, IRunLocationItem
{
	[Tooltip("Direction to push object to.")]
	public Direction Direction;

	[Tooltip("Distance to move the object.")]
	public int Distance = 1;

	[Tooltip("Region that is used to check for clearance.")]
	public RegionView RegionToBeClear;

	[Tooltip("Should check for colliders in the region, if this is false then only actors are checked.")]
	public bool CheckCollidersInRegion;

	private Vector3 startPosition;

	public override bool AutoGenerateViewID => true;

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		model.Changed += OnModelChanged;
		startPosition = base.transform.position;
		RefreshPosition();
	}

	private Direction GetOpposite(Direction direction)
	{
		if (Direction == Direction.Down)
		{
			return Direction.Up;
		}
		if (Direction == Direction.Up)
		{
			return Direction.Down;
		}
		if (Direction == Direction.Left)
		{
			return Direction.Right;
		}
		if (Direction == Direction.Right)
		{
			return Direction.Left;
		}
		return Direction.Any;
	}

	public TWDModelObject Apply(IRunLocationItemContainer runLocation, IRunLocationErrorContext errors)
	{
		MovableModel movableModel = new MovableModel(ViewId, Direction, Distance, null, CheckCollidersInRegion);
		runLocation.AddModelObject(movableModel);
		return movableModel;
	}

	public void RefreshPosition()
	{
		if (base.Model != null && base.Model.IsMoved)
		{
			GridCoordinate gridCoordinate = new GridCoordinate(0, 0);
			Vector3 vector = GridView.Instance.GetPosition(gridCoordinate).ToVector3();
			Vector3 vector2 = GridView.Instance.GetPosition(base.Model.GetTargetCoordinate(gridCoordinate)).ToVector3() - vector;
			base.gameObject.transform.position = startPosition + vector2;
		}
	}

	public void OnModelChanged(ModelObject model, string changed, object args)
	{
		if (args is ActorModel pushingActor && this != null)
		{
			VisualizationQueue.Instance.Add(new PushVisualizationTask(pushingActor, this));
		}
	}
}
