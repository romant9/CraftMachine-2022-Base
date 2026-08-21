using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class AbilityTargetGridInputHandler : AbilityInputHandler
{
	public DrawnPathChangedHandler DrawnPathChanged;

	private bool isRepositioningTarget;

	private GridCoordinate lastMouseGridCoordinate;

	private GridCoordinate targetPosition;

	private GridCoordinate previousGridCoordinate;

	private GridPath path;

	protected override bool CanHandleAbility(AbilityModel ability)
	{
		if (ability != null)
		{
			return ability.Definition.TriggerType == AbilityTriggerType.Grid;
		}
		return false;
	}

	public override void InteractionStarted()
	{
		if (!targetPosition.IsValid)
		{
			lastMouseGridCoordinate = PlayerInputManager.Instance.GetMouseGridCoordinate();
			RefreshCurrentTargetPosition();
			isRepositioningTarget = true;
		}
		else
		{
			GridCoordinate gridCoordinate = (lastMouseGridCoordinate = PlayerInputManager.Instance.GetMouseGridCoordinate());
			isRepositioningTarget = targetPosition == gridCoordinate;
			PlayerInputManager.Instance.GetHandler<CameraInputHandler>().DraggingEnabled = !isRepositioningTarget;
			if (!isRepositioningTarget)
			{
				base.InteractionStarted();
			}
		}
		path = GridPath.Create();
	}

	public override bool UpdateInteraction(float deltaTime)
	{
		if (targetPosition.IsValid)
		{
			if (isRepositioningTarget)
			{
				GridCoordinate mouseGridCoordinate = PlayerInputManager.Instance.GetMouseGridCoordinate();
				if (mouseGridCoordinate != lastMouseGridCoordinate)
				{
					lastMouseGridCoordinate = mouseGridCoordinate;
					RefreshCurrentTargetPosition();
				}
				lastMouseGridCoordinate = mouseGridCoordinate;
			}
			else
			{
				base.UpdateInteraction(deltaTime);
			}
		}
		return true;
	}

	public override void InteractionStopped()
	{
		base.InteractionStopped();
		GridCoordinate mouseGridCoordinate = base.PlayerInputManager.GetMouseGridCoordinate();
		if (PlayerInputManager.Instance.IsDragging)
		{
			return;
		}
		if (!previousGridCoordinate.IsValid || previousGridCoordinate != mouseGridCoordinate)
		{
			previousGridCoordinate = mouseGridCoordinate;
			RefreshCurrentTargetPosition();
		}
		else
		{
			if (!PlayerInputManager.Instance.HasBeenDragged)
			{
				if (mouseGridCoordinate == targetPosition)
				{
					Helpers.ExecuteCommand(new AbilityCommand(selectedSurvivor, currentSelectedAbility, mouseGridCoordinate));
					base.GridView.SelectedAbilityToDisplayTargetCells = null;
				}
				else
				{
					RefreshCurrentTargetPosition();
				}
			}
			PlayerInputManager.Instance.GetHandler<CameraInputHandler>().DraggingEnabled = true;
		}
		if (currentSelectedAbility != null && currentSelectedAbility.Definition.AbilityTargetArea == AbilityTargetAreaType.Line)
		{
			path.Clear();
			path.AddNode(selectedSurvivor.GridCoordinate);
			path.AddNode(targetPosition);
			NotifyDrawnPathChanged();
		}
	}

	protected override void OnAbilityDisabled()
	{
		base.OnAbilityDisabled();
		targetPosition = GridCoordinate.Invalid;
		base.GridView.ClearHighlights();
		ClearPath();
	}

	protected override void OnAbilityEnabled()
	{
		targetPosition = GridCoordinate.Invalid;
		base.GridView.ClearHighlights();
	}

	private void RefreshCurrentTargetPosition()
	{
		GridCoordinate gridCoordinate = selectedSurvivor.GridCoordinate;
		GridCoordinate mouseGridCoordinate = base.PlayerInputManager.GetMouseGridCoordinate();
		FixedPoint range = currentSelectedAbility.Definition.AbilityRange;
		CombatHelpers.CalculateRangeExtension(ref range, selectedSurvivor, base.Combat.AbilityManager);
		GridCoordinate closestEmptyGridToThrowLocation = base.Combat.GetClosestEmptyGridToThrowLocation(gridCoordinate, mouseGridCoordinate, currentSelectedAbility.Definition.RequiresLineOfSight, (float)range);
		targetPosition = ((!closestEmptyGridToThrowLocation.IsValid) ? targetPosition : closestEmptyGridToThrowLocation);
		base.GridView.ClearHighlights();
		if (!targetPosition.IsValid)
		{
			return;
		}
		List<GridCoordinate> list = new List<GridCoordinate>();
		List<Color> list2 = new List<Color>();
		List<int> list3 = new List<int>();
		list.Add(targetPosition);
		list2.Add(Color.green);
		list3.Add(0);
		if (currentSelectedAbility.Definition.AbilityTargetArea == AbilityTargetAreaType.Line)
		{
			List<ActorModel> actorsInLine = base.Combat.GetActorsInLine(gridCoordinate, targetPosition);
			int i = 0;
			int num = 1;
			for (; i < actorsInLine.Count; i++)
			{
				ActorModel actorModel = actorsInLine[i];
				if (actorModel.IsEnemy(selectedSurvivor))
				{
					list.Add(actorModel.GridCoordinate);
					list2.Add(Color.red);
					list3.Add(num);
					num++;
				}
			}
		}
		base.GridView.HighlightCoordinates(list, list2, list3);
		if (currentSelectedAbility.Definition.AbilityTargetArea == AbilityTargetAreaType.Diamond)
		{
			int damageAreaBlockEffectiveAreaRadius = base.Combat.AbilityManager.GetDamageAreaBlockEffectiveAreaRadius(currentSelectedAbility, targetPosition, (int)currentSelectedAbility.Definition.AbilityTargetAreaRadius);
			List<GridCoordinate> diamondCoordinates = base.Combat.GetDiamondCoordinates(targetPosition, damageAreaBlockEffectiveAreaRadius);
			Color fillColor = ((base.abilityRangeVisualizer != null) ? base.abilityRangeVisualizer.GetFillColor() : Color.green);
			base.GridView.HighlightCoordinatesWithFill(diamondCoordinates, fillColor);
		}
	}

	private void ClearPath()
	{
		path = null;
		previousGridCoordinate = GridCoordinate.Invalid;
		NotifyDrawnPathChanged();
	}

	private void NotifyDrawnPathChanged()
	{
		DrawnPathChanged?.Invoke(path, doubleMove: false);
	}
}
