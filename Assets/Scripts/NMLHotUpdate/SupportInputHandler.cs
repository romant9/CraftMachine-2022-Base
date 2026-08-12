using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class SupportInputHandler : PlayerInputHandler
{
	private WeaponRangeVisualization weaponRadiusVisualization;

	private GridCoordinate? lastInputCoordinate;

	private GridPath runPath;

	public SupportInteractionManager SupportInteractionManager { get; private set; }

	public override int Priority => 2000;

	public event DrawnPathChangedHandler DrawnPathChanged;

	public override bool CanHandleInteraction()
	{
		return SupportInteractionManager.ActiveSupportInteraction != null;
	}

	public override void Initialize()
	{
		base.Initialize();
		SupportInteractionManager = new SupportInteractionManager(base.Combat);
		SupportInteractionManager.SupportActivated += OnSupportActivationChange;
		SupportInteractionManager.SupportDeactivated += OnSupportActivationChange;
		SupportInteractionManager.SupportExecuted += OnSupportActivationChange;
		SupportInteractionManager.SupportExecutionFailed += delegate(int equipIndex, SupportTargetsMessage message)
		{
			OnSupportActivationChange(equipIndex);
		};
	}

	private void OnSupportActivationChange(int equipIndex)
	{
		ISupportInteraction activeSupportInteraction = SupportInteractionManager.ActiveSupportInteraction;
		if (activeSupportInteraction != null)
		{
			Helpers.ExecuteCommand(new SetActiveActorCommand(activeSupportInteraction.AttachedSurvivor));
			if (!activeSupportInteraction.Targeted)
			{
				HighlightTargets(activeSupportInteraction.GetTargets(activeSupportInteraction.AttachedSurvivor.GridCoordinate));
			}
		}
		else
		{
			base.GridView.ClearHighlights();
		}
	}

	public override void InteractionStarted()
	{
		ISupportInteraction activeSupportInteraction = SupportInteractionManager.ActiveSupportInteraction;
		if (activeSupportInteraction.Targeted)
		{
			weaponRadiusVisualization = GetActorView(activeSupportInteraction.AttachedSurvivor).ActivationRangeVisualizer;
		}
	}

	private static ActorView GetActorView(ActorModel actorModel)
	{
		return GameManager.Instance.GetViews<CombatView>()[0].GetActorViewFromModel(actorModel);
	}

	public override bool UpdateInteraction(float deltaTime)
	{
		GridCoordinate mouseGridCoordinate = base.PlayerInputManager.GetMouseGridCoordinate();
		if (!lastInputCoordinate.HasValue || mouseGridCoordinate != lastInputCoordinate.Value)
		{
			Refresh(mouseGridCoordinate);
		}
		return true;
	}

	private void Refresh(GridCoordinate inputCoord)
	{
		if (!weaponRadiusVisualization)
		{
			return;
		}
		ISupportInteraction activeSupportInteraction = SupportInteractionManager.ActiveSupportInteraction;
		SurvivorModel attachedSurvivor = activeSupportInteraction.AttachedSurvivor;
		GridCoordinate gridCoordinate = attachedSurvivor.GridCoordinate;
		FixedPoint fixedPoint = activeSupportInteraction.MinRange * activeSupportInteraction.MinRange;
		if (inputCoord == gridCoordinate || inputCoord.SquaredDistanceTo(gridCoordinate) < fixedPoint)
		{
			runPath = GridPath.Create();
			this.DrawnPathChanged?.Invoke(runPath, doubleMove: false);
			weaponRadiusVisualization.Clear();
			base.GridView.ClearHighlights();
			lastInputCoordinate = inputCoord;
		}
		else
		{
			if (!lastInputCoordinate.HasValue)
			{
				return;
			}
			GridPath gridPath = base.Combat.FindPath(activeSupportInteraction.AttachedSurvivor, gridCoordinate, inputCoord);
			if (!gridPath.IsValid)
			{
				return;
			}
			FixedPoint? fixedPoint2 = activeSupportInteraction.MaxRange * activeSupportInteraction.MaxRange;
			GridCoordinate? gridCoordinate2 = null;
			foreach (GridCoordinate item in gridPath.Path)
			{
				FixedPoint fixedPoint3 = item.SquaredDistanceTo(inputCoord);
				FixedPoint? fixedPoint4 = fixedPoint2;
				if (fixedPoint4.HasValue && fixedPoint3 <= fixedPoint4.GetValueOrDefault() && !CombatHelpers.IsOccupiedOrBlocked(base.Combat, item, activeSupportInteraction.AttachedSurvivor))
				{
					gridCoordinate2 = item;
					break;
				}
			}
			if (gridCoordinate2.HasValue)
			{
				if (gridCoordinate2 != gridCoordinate)
				{
					gridPath.ClipTo(gridCoordinate2.Value);
				}
				else
				{
					gridPath = GridPath.Create();
				}
				if (gridPath.Length <= CombatHelpers.GetMoveRange(attachedSurvivor))
				{
					runPath = gridPath;
					this.DrawnPathChanged?.Invoke(runPath, doubleMove: false);
					lastInputCoordinate = inputCoord;
					Vector3 start = GameManager.Instance.modelManager.CombatModel.Grid.GetPosition(lastInputCoordinate.Value).ToVector3();
					FixedPoint fixedPoint5 = activeSupportInteraction.AreaRadius ?? ((FixedPoint)0.0);
					weaponRadiusVisualization.SetCircle(start, GetWorldRadius((float)fixedPoint5));
					base.GridView.ClearHighlights();
					HighlightTargets(activeSupportInteraction.GetTargets(inputCoord), inputCoord);
				}
			}
		}
	}

	private float GetWorldRadius(float radius)
	{
		float num = (float)base.Combat.Grid.CellSize.X;
		return radius * num;
	}

	public override void InteractionStopped()
	{
		ISupportInteraction activeSupportInteraction = SupportInteractionManager.ActiveSupportInteraction;
		if (activeSupportInteraction != null && activeSupportInteraction.Targeted)
		{
			if (lastInputCoordinate.HasValue && !base.Combat.MissionCompleted)
			{
				FixedPoint minRange = activeSupportInteraction.MinRange;
				FixedPoint fixedPoint = minRange * minRange;
				if (activeSupportInteraction.AttachedSurvivor.GridCoordinate.SquaredDistanceTo(lastInputCoordinate.Value) >= fixedPoint)
				{
					SupportInteractionManager.Execute(lastInputCoordinate, runPath);
					ActorMoveInputHandler.EndTurnRoutine(activeSupportInteraction.AttachedSurvivor, base.Combat);
				}
				else
				{
					SupportInteractionManager.Deactivate();
				}
			}
		}
		else
		{
			SupportInteractionManager.Deactivate();
		}
		if ((bool)weaponRadiusVisualization)
		{
			weaponRadiusVisualization.Clear();
		}
		base.GridView.ClearHighlights();
		weaponRadiusVisualization = null;
		lastInputCoordinate = null;
		runPath = null;
		this.DrawnPathChanged?.Invoke(GridPath.Create(), doubleMove: false);
	}

	private void HighlightTargets(IEnumerable<ActorModel> targets, GridCoordinate? targetPoint = null)
	{
		if (targets == null)
		{
			return;
		}
		List<GridCoordinate> list = new List<GridCoordinate>();
		List<Color> list2 = new List<Color>();
		List<int> list3 = new List<int>();
		int num = 1;
		foreach (ActorModel target in targets)
		{
			list.Add(target.GridCoordinate);
			list2.Add(Color.red);
			list3.Add(num++);
		}
		if (targetPoint.HasValue && !list.Contains(targetPoint.Value))
		{
			list.Add(targetPoint.Value);
			list2.Add(Color.red);
			list3.Add(0);
		}
		base.GridView.HighlightCoordinates(list, list2, list3);
	}
}
