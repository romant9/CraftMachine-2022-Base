using Client.Utils;
using TWDModel;
using UnityEngine;

public class DeathVisualizationTask : ActorVisualizationTask
{
	private Animator animator;

	private float timeLeft;

	private bool fromChargeAttack;

	public float Delay { get; set; }

	protected bool IsExplosiveDeath => base.Actor.LastHitExplosive != null;

	private bool IsDeathRequested { get; set; }

	public ActorModel Attacker { get; set; }

	public DeathVisualizationTask(ActorModel actor, ActorModel attacker)
		: base(null, affectsCovers: true)
	{
		Attacker = attacker;
		if (actor.Faction == Faction.Survivor)
		{
			CombatView.Instance.UpdateVisibility = false;
		}
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
		if (attacker != null)
		{
			if (attacker.SelectedAbility.PushEffect != null)
			{
				AbilityEffectPush pushEffect = attacker.SelectedAbility.PushEffect;
				if (pushEffect == null || !pushEffect.IsDisablePushDirectionIndicators)
				{
					goto IL_00a6;
				}
			}
			AddDependency(attacker, reserve: false);
			fromChargeAttack = attacker.SelectedAbility.IsChargeAttack;
		}
		goto IL_00a6;
		IL_00a6:
		IsDeathRequested = false;
	}

	private void StartDeathAnimation()
	{
		CharacterAnimationController characterAnimationController = base.ActorView.CharacterAnimationController;
		ActorView actorView = ((Attacker != null) ? (GameManager.Instance.GetViewForModel(Attacker) as ActorView) : null);
		Vector3 vector = new Vector3(0f, 0f, 0f);
		bool flag = actorView != null && actorView.CurrentWeapon != null && actorView.CurrentWeapon.Definition != null;
		if (flag)
		{
			vector = actorView.transform.position;
		}
		else if (IsExplosiveDeath)
		{
			vector = GridView.Instance.GetPosition(base.Actor.LastHitExplosive.Location.Coordinate).ToVector3();
		}
		if (ImpactProfileManager.Instance != null && (flag || IsExplosiveDeath))
		{
			ImpactProfile impactProfile = (IsExplosiveDeath ? ImpactProfileManager.Instance.GetExplosionImpactProfile() : ImpactProfileManager.Instance.GetImpactProfile(actorView.CurrentWeapon.Definition.Type, actorView.CurrentWeapon.Definition.SubCategory));
			if (impactProfile != null)
			{
				Vector3 normalized = (base.ActorView.transform.position - vector).normalized;
				Vector3 impactDirection = (impactProfile.isExplosive ? normalized : (fromChargeAttack ? actorView.ChargeWeaponImpactDirection : actorView.WeaponImpactDirection));
				characterAnimationController.Die(impactProfile, impactDirection, normalized);
			}
			else
			{
				characterAnimationController.Die(characterAnimationController.IsStruggling);
			}
		}
		else
		{
			characterAnimationController.Die(characterAnimationController.IsStruggling);
		}
		SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("stop_group/struggle", base.ActorView.gameObject);
		SingularityMonoBehaviour<AudioManager>.Instance.StopEvent("stop_group/bleeding_out", base.ActorView.gameObject);
		string eventName = "combat_survivor/survivor_die";
		switch (base.Actor.Faction)
		{
		case Faction.Survivor:
		case Faction.Civilian:
		case Faction.Lure:
			eventName = ((base.Actor.Gender != ActorGender.Male) ? "combat_survivor/survivor_female_die" : "combat_survivor/survivor_male_die");
			break;
		case Faction.Walker:
			eventName = "combat_walker/walker_die";
			break;
		case Faction.Raider:
			eventName = ((base.Actor.Gender != ActorGender.Male) ? "combat_raider/raider_female_die" : "combat_raider/raider_male_die");
			break;
		}
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(eventName, base.ActorView.gameObject);
		base.ActorView.Die();
	}

	public override void Stop()
	{
	}

	public override bool Update(float deltaTime)
	{
		ReleaseDependency(Attacker);
		Delay -= deltaTime;
		if (Delay > 0f)
		{
			return true;
		}
		CharacterAnimationController characterAnimationController = base.ActorView.CharacterAnimationController;
		if (characterAnimationController != null && !IsDeathRequested)
		{
			IsDeathRequested = true;
			StartDeathAnimation();
		}
		if (characterAnimationController == null || characterAnimationController.IsInDeath)
		{
			CombatView.Instance.UpdateVisibility = true;
			if (base.ActorView.Model.Definition.ShouldDestroyViewOnDeath)
			{
				CombatView.Instance.RemoveActorView(base.ActorView);
			}
			if (base.Actor.Faction != Faction.Survivor)
			{
				CombatView.Instance.AddDeadbody(base.ActorView);
			}
			return false;
		}
		return true;
	}
}
