using System.Collections.Generic;
using Client.Utils;
using TWDModel;

public class AttackInteractiveObjectVisualizationTask : ActorVisualizationTask
{
	private InteractiveObjectView TargetObjectView;

	private bool WeaponUsed;

	public AttackInteractiveObjectVisualizationTask(AttackInteractiveObjectAction action)
		: base(action, affectsCovers: true)
	{
		AddFactionDependency(action.Attacker.Faction);
		AddActorDependency(action.Attacker);
		AddDependency(action.Target);
		base.Actor = action.Attacker;
		TargetObjectView = GameManager.Instance.GetViewForModel(action.Target) as InteractiveObjectView;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
	}

	public override List<VisualizationTask> TasksToQueue()
	{
		return new List<VisualizationTask>
		{
			new TurnToTargetVisualizationTask(sourcePosition: GridView.Instance.GetPosition(base.Actor.GridCoordinate).ToVector3(), targetPosition: GridView.Instance.GetPosition(TargetObjectView.Model.Location.Coordinate).ToVector3(), actor: base.Actor),
			this
		};
	}

	public override void Start()
	{
		base.Start();
		CharacterAnimationController characterAnimationController = base.ActorView.CharacterAnimationController;
		characterAnimationController.OnUseWeapon += OnUseWeapon;
		characterAnimationController.UseWeapon(criticalDamage: false, useFenceAttack: false, useChargeAttack: false);
	}

	private void OnUseWeapon(bool preImpact)
	{
		base.ActorView.CharacterAnimationController.OnUseWeapon -= OnUseWeapon;
		ObjectHitEffects component = TargetObjectView.GetComponent<ObjectHitEffects>();
		if (component != null && TargetObjectView.Model.NPCAttackCount != TargetObjectView.Model.NPCAttacksToDestroy)
		{
			float num = (float)(TargetObjectView.Model.NPCAttackCount + 1) / (float)TargetObjectView.Model.NPCAttacksToDestroy;
			component.ShakeObject(num * num);
		}
		WeaponUsed = true;
	}

	public override bool Update(float deltaTime)
	{
		if (base.ActorView.CharacterAnimationController.IsIdle)
		{
			return !WeaponUsed;
		}
		return true;
	}
}
