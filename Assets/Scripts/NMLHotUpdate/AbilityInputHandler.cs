using System;
using TWDModel;

public class AbilityInputHandler : PlayerInputHandler
{
	protected AbilityModel currentSelectedAbility;

	protected ActorModel selectedSurvivor;

	protected ActorView selectedSurvivorView;

	protected WeaponRangeVisualization abilityRangeVisualizer
	{
		get
		{
			if (selectedSurvivor == null)
			{
				return null;
			}
			return selectedSurvivorView.AbilityRangeVisualizer;
		}
	}

	public override int Priority => 200;

	public override bool TapOnly => true;

	public override bool CanHandleInteraction()
	{
		if (Helpers.IsCombatSkillSelectableStatus())
		{
			return false;
		}
		if (base.GridView.SelectedAbilityToDisplayTargetCells != null)
		{
			return CanHandleAbility(base.GridView.SelectedAbilityToDisplayTargetCells.Ability);
		}
		return false;
	}

	protected virtual bool CanHandleAbility(AbilityModel ability)
	{
		return false;
	}

	public override void Initialize()
	{
		GridView gridView = base.GridView;
		gridView.AbilityChanged = (AbilityChangeHandler)Delegate.Combine(gridView.AbilityChanged, new AbilityChangeHandler(AbilityChanged));
	}

	public virtual void AbilityChanged(AbilityModel ability, ActorModel sourceActor)
	{
		selectedSurvivor = sourceActor;
		if (currentSelectedAbility != ability)
		{
			currentSelectedAbility = ability;
			if (currentSelectedAbility != null && CanHandleAbility(ability))
			{
				OnAbilityEnabled();
			}
			else
			{
				OnAbilityDisabled();
			}
		}
	}

	protected virtual void OnAbilityDisabled()
	{
		base.GridView.ClearHighlights();
	}

	protected virtual void OnAbilityEnabled()
	{
	}
}
