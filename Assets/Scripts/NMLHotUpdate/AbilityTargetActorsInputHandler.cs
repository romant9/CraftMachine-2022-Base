using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class AbilityTargetActorsInputHandler : AbilityInputHandler
{
	private bool highlightsAdded;

	private GridCoordinate targetPosition;

	protected override bool CanHandleAbility(AbilityModel ability)
	{
		return ability.Definition.TriggerType == AbilityTriggerType.Targetted;
	}

	protected override void OnAbilityEnabled()
	{
		highlightsAdded = false;
		targetPosition = GridCoordinate.Invalid;
		bool flag = currentSelectedAbility.Definition.TargetType == AbilityTargetType.Friendly;
		PlayerInputManager.Instance.PlayerSelectionEnabled = !flag;
	}

	protected override void OnAbilityDisabled()
	{
		PlayerInputManager.Instance.PlayerSelectionEnabled = true;
		if (base.abilityRangeVisualizer != null)
		{
			base.abilityRangeVisualizer.Clear();
		}
	}

	private void RefreshHighlights(bool forceRefresh = false)
	{
		if (!(!highlightsAdded || forceRefresh))
		{
			return;
		}
		List<GridCoordinate> availableTargetPositions = currentSelectedAbility.GetAvailableTargetPositions(base.Combat, selectedSurvivor, selectedSurvivor.GridCoordinate);
		Color color = Color.red;
		Color value = Color.red;
		Color value2 = new Color(1f, 0.33f, 0f);
		if (currentSelectedAbility.Definition.TargetType == AbilityTargetType.Friendly)
		{
			color = new Color(0f, 0.4f, 1f);
			value = color;
			value2 = Color.blue;
		}
		int item = 0;
		int value3 = 1;
		int value4 = 1;
		List<int> list = new List<int>();
		List<Color> list2 = new List<Color>();
		for (int i = 0; i < availableTargetPositions.Count; i++)
		{
			list2.Add(color);
			list.Add(item);
		}
		if (targetPosition.IsValid)
		{
			GridCoordinate sourceCell = selectedSurvivor.GridCoordinate;
			if (currentSelectedAbility.Definition.IsPerformedAfterPlayerMove)
			{
				GridPath gridPath = base.Combat.FindPath(selectedSurvivor, selectedSurvivor.GridCoordinate, targetPosition);
				if (gridPath.IsValid)
				{
					sourceCell = gridPath.End;
				}
			}
			List<ActorModel> list3 = base.Combat.AbilityManager.GetListOfActorsToBeTargetted(currentSelectedAbility, base.Combat.ActiveActor, sourceCell, targetPosition);
			FixedPoint value5 = 0.0;
			base.Combat.AbilityManager.VisitParameter(AbilityModifierIncreaseSecondaryHitsChance.SecondaryHitsChance, ref value5, selectedSurvivor);
			if (currentSelectedAbility.Definition.SecondaryTargetsHitChance * (1.0 + value5) < 1L)
			{
				list3 = list3.GetRange(0, 1);
			}
			for (int j = 0; j < availableTargetPositions.Count; j++)
			{
				if (availableTargetPositions[j] == targetPosition)
				{
					list2[j] = value;
					list[j] = value3;
					continue;
				}
				foreach (ActorModel item2 in list3)
				{
					if (availableTargetPositions[j] == item2.GridCoordinate)
					{
						list2[j] = value2;
						list[j] = value4;
					}
				}
			}
		}
		base.GridView.HighlightCoordinates(availableTargetPositions, list2, list);
		highlightsAdded = true;
	}

	public override void InteractionStopped()
	{
		base.InteractionStopped();
		GridCoordinate mouseGridCoordinate = base.PlayerInputManager.GetMouseGridCoordinate();
		if (base.PlayerInputManager.IsDragging)
		{
			return;
		}
		GridCoordinate sourceCell = selectedSurvivor.GridCoordinate;
		if (currentSelectedAbility.Definition.IsPerformedAfterPlayerMove)
		{
			GridPath gridPath = base.Combat.FindPath(selectedSurvivor, selectedSurvivor.GridCoordinate, mouseGridCoordinate);
			if (gridPath.IsValid)
			{
				sourceCell = gridPath.End;
			}
		}
		if (!targetPosition.IsValid || targetPosition != mouseGridCoordinate)
		{
			targetPosition = mouseGridCoordinate;
			VisualizationQueue.Instance.Add(new TurnToTargetVisualizationTask(selectedSurvivor, selectedSurvivorView.transform.position, base.GridView.GetPosition(targetPosition).ToVector3()));
			if (currentSelectedAbility != null && currentSelectedAbility.CanAbilityBePerformedOnGridCell(base.Combat, selectedSurvivor, sourceCell, targetPosition) == AbilityResult.Success)
			{
				RefreshHighlights(forceRefresh: true);
				if (!(base.abilityRangeVisualizer != null))
				{
					return;
				}
				Vector3 position = selectedSurvivorView.transform.position;
				Vector3 vector = base.GridView.GetPosition(targetPosition).ToVector3();
				if (currentSelectedAbility.Definition.MaxAffectedTargetsCount == 1)
				{
					base.abilityRangeVisualizer.SetPoint(vector);
					return;
				}
				Vector3 normalized = (vector - position).normalized;
				FixedPoint range = currentSelectedAbility.Definition.AbilityRange;
				CombatHelpers.CalculateRangeExtension(ref range, selectedSurvivor, base.Combat.AbilityManager);
				FixedPoint fixedPoint = range * base.GridView.Model.CellSize.X;
				Vector3 end = position + normalized * (float)fixedPoint;
				FixedPoint value = currentSelectedAbility.Definition.AbilityTargetAreaAngle;
				base.Combat.AbilityManager.VisitParameter("AbilityModifierIncreaseConeAngle", ref value, selectedSurvivor);
				base.Combat.AbilityManager.VisitParameter("AbilityModifierThreatArcUpgrade", ref value, selectedSurvivor);
				if (value <= 1L)
				{
					base.abilityRangeVisualizer.SetLine(position, end);
				}
				else
				{
					base.abilityRangeVisualizer.SetSector(position, end, (float)value);
				}
			}
			else
			{
				base.GridView.SelectedAbilityToDisplayTargetCells = null;
				if (base.abilityRangeVisualizer != null)
				{
					base.abilityRangeVisualizer.Clear();
				}
			}
			return;
		}
		if (currentSelectedAbility != null && currentSelectedAbility.CanAbilityBePerformedOnGridCell(base.Combat, selectedSurvivor, sourceCell, mouseGridCoordinate) == AbilityResult.Success)
		{
			if (currentSelectedAbility.Definition.IsPerformedAfterPlayerMove)
			{
				GridPath gridPath2 = base.Combat.FindPath(selectedSurvivor, selectedSurvivor.GridCoordinate, mouseGridCoordinate);
				if (gridPath2.IsValid)
				{
					Helpers.ExecuteCommand(new MoveCommand(selectedSurvivor, gridPath2));
				}
			}
			Helpers.ExecuteCommand(new AbilityCommand(selectedSurvivor, currentSelectedAbility, mouseGridCoordinate));
		}
		base.GridView.SelectedAbilityToDisplayTargetCells = null;
		if (base.abilityRangeVisualizer != null)
		{
			base.abilityRangeVisualizer.Clear();
		}
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		if (CanHandleInteraction())
		{
			RefreshHighlights();
		}
	}
}
