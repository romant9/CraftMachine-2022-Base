using System;
using System.Collections;
using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class ProjectileVisualizationTask : FireWeaponVisualizationTask
{
	private static float throwSpeed = 12f;

	private float totalThrowDuration;

	private float throwTime = -1f;

	private Vector3 finalPosition;

	private Vector3 initialPosition;

	private GameObject projectile;

	private bool projectileDone;

	private bool shotAnimationTriggered;

	private AbilityDefinition Ability { get; set; }

	private GridCoordinate TargetCoordinate { get; set; }

	public ProjectileVisualizationTask(ProjectileAction action)
		: base(action)
	{
		base.Actor = action.SourceActor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		Ability = action.WeaponAbility.Definition;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
	}

	public override void Start()
	{
		InstantiateProjectile();
		WeaponEffectsSpawner component = base.ActorView.GetCurrentWeaponPrefab().GetComponent<WeaponEffectsSpawner>();
		initialPosition = component.transform.position;
		float magnitude = (finalPosition - initialPosition).magnitude;
		totalThrowDuration = magnitude / throwSpeed;
		base.Start();
	}

	private void InstantiateProjectile()
	{
		ProjectileAction projectileAction = (ProjectileAction)base.Action;
		FixedVec3 position = GridView.Instance.GetPosition(base.Actor.GridCoordinate);
		FixedVec3 position2 = GridView.Instance.GetPosition(projectileAction.TargetActor?.GridCoordinate ?? projectileAction.TargetGridCoordinate);
		Vector3 position3 = position.ToVector3();
		GameObject weaponVisualizationPrefab = base.ActorView.GetWeaponVisualizationPrefab();
		if (weaponVisualizationPrefab != null)
		{
			projectile = UnityEngine.Object.Instantiate(weaponVisualizationPrefab, position3, Quaternion.identity);
			projectile.transform.localScale = new Vector3(1f, 1f, 1f);
			projectile.transform.localRotation = default(Quaternion);
			finalPosition = position2.ToVector3();
			projectile.gameObject.SetActive(value: false);
			throwTime = 0f;
		}
	}

	protected override void SetupDamageDelay()
	{
		List<DamageVisualizationTask> tasksOfType = VisualizationQueue.Instance.GetTasksOfType<DamageVisualizationTask>();
		for (int i = 0; i < tasksOfType.Count; i++)
		{
			if (tasksOfType[i].DamagerActor == base.Actor)
			{
				AddActorDependency(tasksOfType[i].Actor);
			}
		}
		List<DeathVisualizationTask> tasksOfType2 = VisualizationQueue.Instance.GetTasksOfType<DeathVisualizationTask>();
		for (int j = 0; j < tasksOfType2.Count; j++)
		{
			if (tasksOfType2[j].Attacker == base.Actor)
			{
				AddActorDependency(tasksOfType2[j].Actor);
			}
		}
	}

	protected override bool FireWeaponAttack(float deltaTime)
	{
		if (!shotAnimationTriggered)
		{
			OnUseWeapon(preImpact: true);
			return true;
		}
		throwTime += deltaTime;
		bool flag = true;
		if (throwTime >= totalThrowDuration && !projectileDone)
		{
			if (projectile != null && projectile.gameObject != null)
			{
				projectile.transform.position = finalPosition;
			}
			SpawnOnHitEffect(new Vector3(finalPosition.x, finalPosition.y + 0.1f, finalPosition.z));
			UnityEngine.Object.Destroy(projectile);
			OnUseWeapon(preImpact: false);
			ClearListeners();
			base.ActorView.ResetTargetActorProperties();
			projectileDone = true;
			flag = true;
			ReleaseAllDependencies();
		}
		else if (throwTime >= totalThrowDuration && projectileDone)
		{
			flag = ((!base.CharacterAnimationController.IsIdle && !base.Actor.IsDead && !base.CharacterAnimationController.IsReloading) ? true : false);
		}
		else
		{
			float num = throwTime / totalThrowDuration;
			Vector3 vector = finalPosition - initialPosition;
			float magnitude = vector.magnitude;
			Vector3 position = initialPosition + vector.normalized * (magnitude * num);
			float num2 = Mathf.Lerp(0f, 180f, num);
			float newY = initialPosition.y + Mathf.Sin(num2 * (MathF.PI / 180f)) * (totalThrowDuration / 2f);
			position.Set(position.x, newY, position.z);
			projectile.transform.position = position;
			projectile.transform.Rotate(deltaTime * 100f, deltaTime * 100f, deltaTime * 100f, Space.Self);
			flag = true;
		}
		if (base.CharacterAnimationController.IsIdle && ActionCamera.Instance != null && ActionCamera.Instance.IsActive && ActionCamera.Instance.LastInstigatorId == base.TargetActor.ModelId)
		{
			ActionCamera.Instance.StopActionCamera();
		}
		return flag;
	}

	protected override IEnumerator SpawnEffects()
	{
		yield return null;
	}

	protected override void OnQuickHit(string direction)
	{
	}

	protected override void OnUseWeapon(bool preImpact)
	{
		if (!preImpact)
		{
			SurvivorAnimationController survivorAnimationController = base.ActorView.CharacterAnimationController as SurvivorAnimationController;
			if (survivorAnimationController != null && survivorAnimationController.CurrentWeaponPose != WeaponPose.Lowered && survivorAnimationController.CurrentWeaponPose != WeaponPose.BeingLowered)
			{
				survivorAnimationController.DesiredWeaponPose = WeaponPose.Lowered;
			}
		}
		else
		{
			shotAnimationTriggered = true;
			SpawnProjectile();
		}
	}

	private void SpawnProjectile()
	{
		WeaponEffectsSpawner component = base.ActorView.GetCurrentWeaponPrefab().GetComponent<WeaponEffectsSpawner>();
		projectile.transform.position = component.transform.position;
		projectile.SetActive(value: true);
		initialPosition = component.transform.position;
	}

	private void SpawnOnHitEffect(Vector3 target)
	{
		GameObject currentWeaponPrefab = base.ActorView.GetCurrentWeaponPrefab();
		WeaponEffectsSpawner weaponEffectsSpawner = (currentWeaponPrefab ? currentWeaponPrefab.GetComponent<WeaponEffectsSpawner>() : null);
		if (weaponEffectsSpawner != null && weaponEffectsSpawner.onHitEffectPrefab != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(weaponEffectsSpawner.onHitEffectPrefab);
			gameObject.transform.position = target;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!string.IsNullOrEmpty(weaponEffectsSpawner.onHitSoundEvent) && SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(weaponEffectsSpawner.onHitSoundEvent, gameObject);
			}
		}
	}
}
