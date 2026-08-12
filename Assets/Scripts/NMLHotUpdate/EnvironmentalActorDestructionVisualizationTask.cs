using System.Collections;
using TWDModel;
using UnityEngine;

public class EnvironmentalActorDestructionVisualizationTask : DeathVisualizationTask
{
	private bool isDying;

	private bool hasDeathAnimationPlayed;

	private bool isExplodable;

	private IEnumerator DyingCoroutineContainer;

	public EnvironmentalActorDestructionVisualizationTask(ActorModel actor, ActorModel attacker)
		: base(actor, attacker)
	{
		ActorView actorView = ((base.Attacker != null) ? (GameManager.Instance.GetViewForModel(base.Attacker) as ActorView) : null);
		if (actorView != null && actorView.CurrentWeapon != null && actorView.CurrentWeapon.Definition.Category == EquipmentCategory.RangeWeapon && actor.GetTraitWithTag("Explosive") != null)
		{
			isExplodable = true;
		}
		if (actor == attacker)
		{
			AddDependencyToAllActors(reserve: true, actor);
		}
	}

	private void DeathAnimation()
	{
		isDying = true;
		hasDeathAnimationPlayed = true;
		ActorView actorView = ((base.Attacker != null) ? (GameManager.Instance.GetViewForModel(base.Attacker) as ActorView) : null);
		Vector3 vector = new Vector3(0f, 0f, 0f);
		Rigidbody component = base.ActorView.GetComponent<Rigidbody>();
		if (component != null)
		{
			bool flag = actorView != null && actorView.CurrentWeapon != null && actorView.CurrentWeapon.Definition != null;
			if (flag)
			{
				vector = actorView.transform.position;
			}
			if (ImpactProfileManager.Instance != null && flag)
			{
				ImpactProfile impactProfile = ImpactProfileManager.Instance.GetImpactProfile(EquipmentType.None, base.Actor.ActorDefinitionID);
				if (impactProfile != null)
				{
					Vector3 normalized = (base.ActorView.transform.position - vector).normalized;
					Vector3 weaponImpactDirection = actorView.WeaponImpactDirection;
					float num = 0.3f;
					Vector3 vector2 = new Vector3(num * (Random.value - 0.5f), 0f, num * (Random.value - 0.5f));
					Vector3 vector3 = weaponImpactDirection;
					if (impactProfile.ImpactConfigurations[0].forceDirectionType == ForceDirectionType.AttackDirection)
					{
						vector3 = normalized;
					}
					vector3 += vector2;
					component.isKinematic = false;
					component.AddForceAtPosition(vector3 * impactProfile.ImpactConfigurations[0].forceMagnitude, component.transform.position, ForceMode.Impulse);
					Helpers.StartCoroutine(base.ActorView, EndDying(), ref DyingCoroutineContainer);
				}
				else
				{
					isDying = false;
				}
			}
			else
			{
				isDying = false;
			}
		}
		else
		{
			isDying = false;
		}
	}

	private IEnumerator EndDying()
	{
		yield return new WaitForSeconds(1.5f);
		base.ActorView.GetComponent<Rigidbody>().isKinematic = true;
		isDying = false;
	}

	public override bool Update(float deltaTime)
	{
		if (base.Actor != base.Attacker)
		{
			ReleaseDependency(base.Attacker);
		}
		base.Delay -= deltaTime;
		if (base.Delay > 0f)
		{
			return true;
		}
		if (!base.IsExplosiveDeath && !isExplodable && !isDying && !hasDeathAnimationPlayed)
		{
			DeathAnimation();
		}
		if (!isDying)
		{
			if (base.ActorView.Model.Definition.ShouldDestroyViewOnDeath)
			{
				CombatView.Instance.RemoveActorViewWithDelay(base.ActorView, 2f);
			}
			base.ActorView.Die();
			return false;
		}
		return true;
	}
}
