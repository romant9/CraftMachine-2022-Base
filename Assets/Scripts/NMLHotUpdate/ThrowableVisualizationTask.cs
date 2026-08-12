using System;
using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class ThrowableVisualizationTask : ActorVisualizationTask
{
	private static float throwSpeed = 5f;

	private static float throwMaxHeight = 2f;

	private float totalThrowDuration;

	private int finalRotation;

	private float throwTime = -1f;

	private Vector3 finalPosition;

	private Vector3 initialPosition;

	private GameObject projectile;

	private FireWeaponState State;

	private ActorView projectileView;

	private bool projectileThrown;

	private GridCoordinate TargetCoordinate { get; set; }

	private CharacterAnimationController CharacterAnimationController
	{
		get
		{
			if (!(base.ActorView != null))
			{
				return null;
			}
			return base.ActorView.CharacterAnimationController;
		}
	}

	public ThrowableVisualizationTask(ThrowableAction action)
		: base(action)
	{
		base.Actor = GameManager.Instance.modelManager.GetModel<ActorModel>(action.ModelId);
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		AddFactionDependency(base.Actor.Faction);
		AddActorDependency(base.Actor);
		projectileView = CombatView.Instance.GetActorViewFromModel(action.InstantiatedModel);
		TargetCoordinate = projectileView.Model.GridCoordinate;
		FixedVec3 position = GridView.Instance.GetPosition(TargetCoordinate);
		projectileView.UseModelForInitialPosition = false;
		projectile = projectileView.gameObject;
		initialPosition = base.ActorView.gameObject.transform.position;
		finalPosition = position.ToVector3();
		projectile.transform.position = initialPosition;
		float magnitude = (finalPosition - initialPosition).magnitude;
		totalThrowDuration = magnitude / throwSpeed;
		projectile.gameObject.SetActive(value: false);
		throwTime = 0f;
		State = FireWeaponState.Start;
	}

	public override List<VisualizationTask> TasksToQueue()
	{
		List<VisualizationTask> list = new List<VisualizationTask>();
		list.Add(new TurnToTargetVisualizationTask(base.Actor, initialPosition, finalPosition));
		SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
		if (survivorAnimationController != null && survivorAnimationController.CurrentWeaponPose != WeaponPose.Raised && survivorAnimationController.CurrentWeaponPose != WeaponPose.BeingRaised && !survivorAnimationController.IsInInteractionLoop)
		{
			list.Add(new ChangeWeaponPoseVisualizationTask(base.Actor, WeaponPose.Raised));
		}
		list.Add(this);
		return list;
	}

	public override bool Update(float deltaTime)
	{
		if (State == FireWeaponState.Start)
		{
			return RaiseWeapon();
		}
		if (State == FireWeaponState.WaitingForActionCamera)
		{
			CharacterAnimationController.OnUseWeapon += OnUseWeapon;
			CharacterAnimationController.UseWeapon(criticalDamage: false, useFenceAttack: false, useChargeAttack: false);
			State = FireWeaponState.Attack;
			return true;
		}
		if (CharacterAnimationController.IsInDeath)
		{
			return false;
		}
		if (!projectileThrown)
		{
			return true;
		}
		if (!projectile.activeSelf)
		{
			finalRotation = UnityEngine.Random.Range(0, 360);
			GameObject currentWeaponPrefab = base.ActorView.GetCurrentWeaponPrefab();
			currentWeaponPrefab.gameObject.SetActive(value: false);
			initialPosition = currentWeaponPrefab.transform.position;
			projectile.transform.position = initialPosition;
			projectile.transform.rotation = currentWeaponPrefab.transform.rotation;
			projectile.SetActive(value: true);
			projectileView.ShowHealthIndicator(visible: false);
		}
		throwTime += deltaTime;
		if (throwTime >= totalThrowDuration)
		{
			if (base.Combat != null)
			{
				foreach (ActorModel enemyFactionsActor in base.Combat.GetEnemyFactionsActors(base.Actor.Faction))
				{
					enemyFactionsActor.NotifyChange("actorTurnToTarget", TargetCoordinate);
				}
			}
			CharacterAnimationController.OnUseWeapon -= OnUseWeapon;
			projectile.transform.position = finalPosition;
			projectileView.HealthIndicator.UpdateFollowTarget();
			projectileView.ShowHealthIndicator(visible: true);
			return false;
		}
		float num = throwTime / totalThrowDuration;
		Vector3 vector = finalPosition - initialPosition;
		float magnitude = vector.magnitude;
		Vector3 position = initialPosition + vector.normalized * (magnitude * num);
		position.Set(position.x, initialPosition.y + throwMaxHeight * Mathf.Sin(MathF.PI * num), position.z);
		projectile.transform.position = position;
		if ((double)num > 0.8)
		{
			Vector3 localEulerAngles = Vector3.Lerp(projectile.transform.localEulerAngles, new Vector3(0f, finalRotation, 0f), num);
			projectile.transform.localEulerAngles = localEulerAngles;
		}
		else
		{
			projectile.transform.Rotate(deltaTime * 1000f, deltaTime * 1000f, deltaTime * 1000f, Space.Self);
		}
		return true;
	}

	private void OnUseWeapon(bool preImpact)
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
			projectileThrown = true;
		}
	}

	private bool RaiseWeapon()
	{
		SurvivorAnimationController survivorAnimationController = base.ActorView.CharacterAnimationController as SurvivorAnimationController;
		bool flag = survivorAnimationController != null && (survivorAnimationController.CurrentWeaponPose == WeaponPose.Raised || survivorAnimationController.CurrentWeaponPose == WeaponPose.BeingRaised);
		if (survivorAnimationController != null && !flag)
		{
			survivorAnimationController.ForceRaiseWeapon();
			flag = true;
		}
		if (CharacterAnimationController.IsIdle || flag)
		{
			State = FireWeaponState.WaitingForActionCamera;
		}
		return true;
	}
}
