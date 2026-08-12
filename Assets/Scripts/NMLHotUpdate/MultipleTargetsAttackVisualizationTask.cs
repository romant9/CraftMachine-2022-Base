using TWDModel;

public class MultipleTargetsAttackVisualizationTask : VisualizationTask
{
	private FireWeaponState State;

	private ActorView ActorView { get; set; }

	public MultipleTargetsAttackVisualizationTask(MultipleTargetsAttackAction action)
		: base(action)
	{
		ActorView = GameManager.Instance.GetViewForModel(action.Actor) as ActorView;
		AbilityResourceEntry resources = GameManager.Instance.GetResources<AbilityResourceEntry>(action.Ability.DefinitionID);
		if (resources != null && !string.IsNullOrEmpty(resources.CharacterAnimation))
		{
			(ActorView.CharacterAnimationController as SurvivorAnimationController).SetController(resources.CharacterAnimation);
			State = FireWeaponState.Start;
		}
	}

	public override bool Update(float deltaTime)
	{
		CharacterAnimationController characterAnimationController = ActorView.CharacterAnimationController;
		switch (State)
		{
		case FireWeaponState.Start:
			if (characterAnimationController.IsIdle)
			{
				characterAnimationController.UseWeapon(criticalDamage: false, useFenceAttack: false, useChargeAttack: false);
				State = FireWeaponState.Attack;
			}
			break;
		case FireWeaponState.Attack:
			if (characterAnimationController.IsIdle)
			{
				return false;
			}
			break;
		}
		return true;
	}
}
