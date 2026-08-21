using System.Collections;
using System.Collections.Generic;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class FireWeaponVisualizationTask : ActorVisualizationTask
{
	private FireWeaponState State;

	protected Vector3 StartPosition;

	protected Quaternion StartRotation;

	protected List<DamageVisualizationTask> damageTasks;

	private List<QuickHitInfo> requestedQuickHits;

	private QuickHitProfile QuickHitProfile;

	private bool EffectsSpawned;

	private bool waitingForCamera;

	private GridCoordinate actorGridCoordinate = GridCoordinate.Invalid;

	private GridCoordinate targetGridCoordinate = GridCoordinate.Invalid;

	protected bool isStruggling;

	protected bool isBleedingOut;

	private EquipmentItemModel consumableEquipment;

	protected bool TargetEndsUpDead
	{
		get
		{
			if (TargetActor != null)
			{
				if (TargetActor.Faction != Faction.Lure)
				{
					return TargetActor.Hitpoints <= 0;
				}
				return true;
			}
			return false;
		}
	}

	protected CharacterAnimationController CharacterAnimationController
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

	protected ActorView TargetView { get; set; }

	public ActorModel TargetActor { get; protected set; }

	public AbilityModel WeaponAbility { get; private set; }

	public bool IsCritical { get; protected set; }

	public bool IsFenceAttack { get; protected set; }

	private bool IsDiagonal
	{
		get
		{
			if (base.ActorView.IsMeleeWeaponEquipped)
			{
				GridCoordinate gridCoordinate = base.ActorView.Model.GridCoordinate;
				GridCoordinate gridCoordinate2 = TargetView?.Model.GridCoordinate ?? targetGridCoordinate;
				bool result = true;
				int num = gridCoordinate2.X - gridCoordinate.X;
				int num2 = gridCoordinate2.Y - gridCoordinate.Y;
				if (num == 0 || num2 == 0)
				{
					result = false;
				}
				return result;
			}
			return false;
		}
	}

	public FireWeaponVisualizationTask(FireWeaponAction action)
		: base(action)
	{
		AddFactionDependency(action.SourceActor.Faction);
		AddActorDependency(action.SourceActor);
		AddActorDependency(action.TargetActor);
		if (action.TargetActor != null)
		{
			AddSpatialDependency(action.TargetActor.GridCoordinate.X, action.TargetActor.GridCoordinate.Y);
		}
		base.Actor = action.SourceActor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		actorGridCoordinate = action.SourceActorGridCoordinate;
		TargetActor = action.TargetActor;
		TargetView = ((TargetActor != null) ? (GameManager.Instance.GetViewForModel(TargetActor) as ActorView) : null);
		targetGridCoordinate = action.TargetGridCoordinate;
		WeaponAbility = action.WeaponAbility;
		isStruggling = base.Actor.IsStruggling;
		isBleedingOut = base.Actor.IsBleedingOut;
		State = FireWeaponState.Start;
	}

	public FireWeaponVisualizationTask(AbilityModel weaponAbility, ActorModel sourceActor, ActorModel targetActor)
		: base(null)
	{
		AddFactionDependency(sourceActor.Faction);
		AddActorDependency(sourceActor);
		AddActorDependency(targetActor);
		if (targetActor != null)
		{
			AddSpatialDependency(targetActor.GridCoordinate.X, targetActor.GridCoordinate.Y);
		}
		base.Actor = sourceActor;
		base.ActorView = GameManager.Instance.GetViewForModel(base.Actor) as ActorView;
		TargetActor = targetActor;
		TargetView = ((TargetActor != null) ? (GameManager.Instance.GetViewForModel(TargetActor) as ActorView) : null);
		WeaponAbility = weaponAbility;
		State = FireWeaponState.Start;
	}

	public override List<VisualizationTask> TasksToQueue()
	{
		List<VisualizationTask> list = new List<VisualizationTask>();
		if (base.ActorView == null)
		{
			Debug.LogWarning("ActorView is NULL at FireWeaponVisualizationTask");
			return list;
		}
		FixedVec3 position = GridView.Instance.GetPosition(base.Actor.GridCoordinate);
		FixedVec3 position2 = GridView.Instance.GetPosition(TargetActor?.GridCoordinate ?? targetGridCoordinate);
		if (VisualizationQueue.Instance.GetMostRecentlyAddedActorTask<FireWeaponVisualizationTask>(base.Actor) != null)
		{
			VisualizationQueue.Instance.AddTaskBlocker();
		}
		list.Add(new ActionCameraVisualizationTask(base.Actor, TargetActor));
		if (base.Actor is TankActorModel)
		{
			list.Add(this);
			return list;
		}
		list.Add(new TurnToTargetVisualizationTask(base.Actor, position.ToVector3(), position2.ToVector3()));
		SurvivorAnimationController survivorAnimationController = base.ActorView.CharacterAnimationController as SurvivorAnimationController;
		EquipmentItemModel selectedEquipment = base.Actor.SelectedEquipment;
		bool flag = selectedEquipment.Definition.Category == EquipmentCategory.MeleeWeapon || (selectedEquipment.IsConsumable && !WeaponAbility.IsConsumableAbility);
		if (base.ActorView.IsMeleeWeaponEquipped != flag && !base.ActorView.SwitchingWeapon && !selectedEquipment.IsChargeEquipment)
		{
			if (selectedEquipment.IsConsumable)
			{
				consumableEquipment = selectedEquipment;
			}
			list.Add(new SwitchWeaponVisualizationTask(base.Actor, flag));
			list.Add(new ChangeWeaponPoseVisualizationTask(base.Actor, WeaponPose.Raised));
		}
		else if (survivorAnimationController != null && survivorAnimationController.CurrentWeaponPose != WeaponPose.Raised && survivorAnimationController.CurrentWeaponPose != WeaponPose.BeingRaised && !survivorAnimationController.IsInInteractionLoop)
		{
			list.Add(new ChangeWeaponPoseVisualizationTask(base.Actor, WeaponPose.Raised));
		}
		list.Add(this);
		return list;
	}

	public override void Start()
	{
		base.Start();
		IsCritical = VisualizationQueue.Instance.GetMostRecentlyAddedActorTask<DamageVisualizationTask>(TargetActor)?.IsCritical ?? false;
		StartPosition = base.Combat.Grid.GetPosition(actorGridCoordinate).ToVector3();
		StartRotation = base.ActorView.transform.rotation;
		IsFenceAttack = false;
		if (base.Actor != null && TargetActor != null)
		{
			GridCoordinate gridCoordinate = base.Actor.GridCoordinate;
			GridCoordinate gridCoordinate2 = TargetActor?.GridCoordinate ?? targetGridCoordinate;
			bool flag = WeaponAbility.IsChargeAttack && base.Actor.GetWeaponEquipment().Definition.Category == EquipmentCategory.RangeWeapon;
			FixedPoint fixedPoint = WeaponAbility.Definition.AbilityRange + (WeaponAbility.Definition.AbilityTargetDiagonal ? 0.42f : 0f);
			if (base.ActorView.IsMeleeWeaponEquipped && !flag)
			{
				PushActorVisualizationTask mostRecentlyAddedActorTask = VisualizationQueue.Instance.GetMostRecentlyAddedActorTask<PushActorVisualizationTask>(TargetActor);
				if (mostRecentlyAddedActorTask != null)
				{
					IsFenceAttack = !base.Actor.manager.CombatModel.CanTraverse(null, gridCoordinate, (mostRecentlyAddedActorTask.Action as PushActorAction).PushEffect.OriginalCoordinate);
				}
				else if (base.Actor.manager.CombatModel.Grid.AreNeighbors(gridCoordinate, gridCoordinate2))
				{
					IsFenceAttack = !base.Actor.manager.CombatModel.CanTraverse(null, gridCoordinate, gridCoordinate2, (float)fixedPoint);
				}
			}
			if (base.ActorView.IsRangedWeaponEquipped && TargetActor != null && TargetActor.Faction != Faction.Walker && TargetActor.Faction != Faction.Environmental)
			{
				base.ActorView.IsTargetInCover = base.Actor.manager.CombatModel.IsInCover(gridCoordinate2, gridCoordinate);
				base.ActorView.IsTargetHuman = true;
			}
			else
			{
				base.ActorView.IsTargetEnvironmentalActor = TargetActor?.Definition.IsEnvironmental ?? false;
			}
		}
		if (base.ActorView.CurrentWeapon != null && base.ActorView.CurrentWeapon.Definition != null)
		{
			QuickHitProfile = ImpactProfileManager.Instance.GetQuickHitProfile(base.ActorView.CurrentWeapon.Definition.Type, base.ActorView.CurrentWeapon.Definition.SubCategory);
		}
		SetupDamageDelay();
	}

	protected virtual void SetupDamageDelay()
	{
		bool flag = false;
		damageTasks = new List<DamageVisualizationTask>();
		List<VisualizationTask> queuedTasks = VisualizationQueue.Instance.GetQueuedTasks();
		for (int i = 0; i < queuedTasks.Count; i++)
		{
			if (queuedTasks[i] is DamageVisualizationTask damageVisualizationTask)
			{
				if (damageVisualizationTask.DamagerActor == base.Actor && ((!damageVisualizationTask.IsFollowThrough && !damageVisualizationTask.IsPushDamage) || !flag))
				{
					damageTasks.Add(damageVisualizationTask);
					if (flag)
					{
						damageVisualizationTask.Delay = Random.value * 0.2f;
					}
					flag = true;
				}
			}
			else if (queuedTasks[i] is FireWeaponVisualizationTask fireWeaponVisualizationTask && fireWeaponVisualizationTask.Actor == base.Actor)
			{
				ChangeWeaponPoseVisualizationTask item = new ChangeWeaponPoseVisualizationTask(base.Actor, WeaponPose.Lowered);
				queuedTasks.Insert(i, item);
				break;
			}
		}
		flag = false;
		List<DeathVisualizationTask> tasksOfType = VisualizationQueue.Instance.GetTasksOfType<DeathVisualizationTask>();
		for (int j = 0; j < tasksOfType.Count; j++)
		{
			if (tasksOfType[j].Attacker == base.Actor)
			{
				if (flag)
				{
					tasksOfType[j].Delay = Random.value * 0.2f;
				}
				flag = true;
			}
		}
	}

	public override void Finished()
	{
		base.Finished();
		SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
		if (survivorAnimationController != null)
		{
			survivorAnimationController.SetWeaponPoseUpdate(enabled: true);
			survivorAnimationController.NotifyWeaponRaised(raised: false);
			if (consumableEquipment != null)
			{
				base.ActorView.RequestSwitchEquipment(consumableEquipment);
				survivorAnimationController.NotifyWeaponSwitch();
			}
		}
		ActionCamera.Instance.OnCameraReady -= OnActionCameraReady;
	}

	private void MoveActor()
	{
		if (base.ActorView.IsMeleeWeaponEquipped)
		{
			Vector3 lastDeltaMovement = CharacterAnimationController.LastDeltaMovement;
			float num = (IsDiagonal ? 1.414f : 1f);
			base.ActorView.transform.position = base.ActorView.transform.position + lastDeltaMovement * num;
			base.ActorView.transform.rotation = base.ActorView.transform.rotation * CharacterAnimationController.LastDeltaRotation;
		}
	}

	public override bool Update(float deltaTime)
	{
		bool result = true;
		if (isStruggling || isBleedingOut)
		{
			base.ActorView.ResetTargetActorProperties();
			return false;
		}
		MoveActor();
		switch (State)
		{
		case FireWeaponState.Start:
			result = FireWeaponStart(deltaTime);
			break;
		case FireWeaponState.WaitingForActionCamera:
			result = FireWeaponWaitForCamera(deltaTime);
			break;
		case FireWeaponState.Attack:
			result = FireWeaponAttack(deltaTime);
			break;
		}
		return result;
	}

	protected virtual bool FireWeaponStart(float deltaTime)
	{
		SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
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

	protected virtual bool FireWeaponWaitForCamera(float deltaTime)
	{
		bool result = true;
		bool flag = ActionCamera.Instance != null && ActionCamera.Instance.IsActive && !base.Actor.IsDead && TargetActor != null && (ActionCamera.Instance.LastInstigatorId != TargetActor.ModelId || (!ActionCamera.Instance.IsAtTarget && ActionCamera.Instance.LastInstigatorId == TargetActor.ModelId));
		if (flag && !waitingForCamera)
		{
			SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
			if (survivorAnimationController != null)
			{
				survivorAnimationController.SetWeaponPoseUpdate(enabled: false);
				ActionCamera.Instance.OnCameraReady += OnActionCameraReady;
				waitingForCamera = true;
			}
		}
		if (base.Actor is TankActorModel && TargetActor != null)
		{
			StationaryBossTurretAim component = base.ActorView.GetComponent<StationaryBossTurretAim>();
			if (component != null && !component.IsAimedAt(TargetActor))
			{
				component.AimToward(TargetActor, deltaTime);
				return result;
			}
		}
		if (!flag)
		{
			CharacterAnimationController.OnUseWeapon += OnUseWeapon;
			CharacterAnimationController.OnTakeQuickHit += OnQuickHit;
			bool isChargeAttack = WeaponAbility.IsChargeAttack;
			CharacterAnimationController.UseWeapon(IsCritical, IsFenceAttack, isChargeAttack);
			State = FireWeaponState.Attack;
		}
		return result;
	}

	private void OnActionCameraReady()
	{
		SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
		if (survivorAnimationController != null)
		{
			survivorAnimationController.SetWeaponPoseUpdate(enabled: true);
		}
		ActionCamera.Instance.OnCameraReady -= OnActionCameraReady;
		waitingForCamera = false;
	}

	protected virtual bool FireWeaponAttack(float deltaTime)
	{
		bool result = true;
		UpdateQuickHits();
		if (CharacterAnimationController.IsIdle || CharacterAnimationController.IsReloading)
		{
			if (ActionCamera.Instance != null && ActionCamera.Instance.IsActive && TargetActor != null && ActionCamera.Instance.LastInstigatorId == TargetActor.ModelId)
			{
				ActionCamera.Instance.StopActionCamera();
			}
			if (base.ActorView.IsMeleeWeaponEquipped)
			{
				base.ActorView.transform.position = StartPosition;
				base.ActorView.transform.rotation = StartRotation;
			}
			CharacterAnimationController.OnUseWeapon -= OnUseWeapon;
			CharacterAnimationController.OnTakeQuickHit -= OnQuickHit;
			base.ActorView.ResetTargetActorProperties();
			if (base.Actor is TankActorModel)
			{
				(CharacterAnimationController as TankAnimationController)?.OnFireAnimationComplete();
			}
			result = false;
		}
		else if (base.Actor.IsDead || CharacterAnimationController.IsInDeath || CharacterAnimationController.IsDeathRequested)
		{
			base.ActorView.ResetTargetActorProperties();
			if (base.Actor is TankActorModel)
			{
				(CharacterAnimationController as TankAnimationController)?.OnFireAnimationComplete();
			}
			result = false;
		}
		return result;
	}

	protected void ClearListeners()
	{
		CharacterAnimationController.OnUseWeapon -= OnUseWeapon;
		CharacterAnimationController.OnTakeQuickHit -= OnQuickHit;
	}

	private void SpawnWeaponEffects(QuickHitProfile quickHitProfile, bool spawnTrail)
	{
		if (WeaponAbility == null || WeaponAbility.Definition == null || !(base.ActorView != null))
		{
			return;
		}
		bool isChargeAttack = WeaponAbility.IsChargeAttack;
		GameObject weaponVisualizationPrefab = base.ActorView.GetWeaponVisualizationPrefab();
		WeaponEffectsSpawner weaponEffectsSpawner = (weaponVisualizationPrefab ? weaponVisualizationPrefab.GetComponent<WeaponEffectsSpawner>() : null);
		if ((bool)weaponEffectsSpawner && (TargetView != null || targetGridCoordinate.IsValid))
		{
			Vector3 vector = ((TargetView == null) ? base.Combat.Grid.GetPosition(targetGridCoordinate).ToVector3() : new Vector3(TargetView.transform.position.x, weaponVisualizationPrefab.transform.position.y, TargetView.transform.position.z));
			if (quickHitProfile != null && quickHitProfile.spawnEffectsOnlyOnce && !EffectsSpawned)
			{
				EffectsSpawned = true;
				weaponEffectsSpawner.SpawnFireEffects(vector, base.Actor.GetWeaponEquipment(), spawnTrail);
			}
			else if (quickHitProfile == null || (!quickHitProfile.spawnEffectsOnlyOnce && (!quickHitProfile.spawnEffectsOnlyOnCharge || isChargeAttack)))
			{
				weaponEffectsSpawner.SpawnFireEffects(vector, base.Actor.GetWeaponEquipment(), spawnTrail);
			}
			if (weaponEffectsSpawner.onHitEffectPrefab != null)
			{
				SpawnOnHitEffect(vector);
			}
		}
		ActorHitEffects component = base.ActorView.gameObject.GetComponent<ActorHitEffects>();
		if ((bool)component && isChargeAttack && (TargetView != null || targetGridCoordinate.IsValid))
		{
			Vector3 targetPos = ((TargetView == null) ? base.Combat.Grid.GetPosition(targetGridCoordinate).ToVector3() : new Vector3(TargetView.transform.position.x, 0f, TargetView.transform.position.z));
			component.SpawnGenericChargeAbilityEffect(targetPos);
		}
	}

	protected virtual IEnumerator SpawnEffects()
	{
		yield return new WaitForSeconds(0.2f);
		if ((bool)base.ActorView && ((bool)TargetView || WeaponAbility.Definition.TriggerType == AbilityTriggerType.GridOrTarget))
		{
			SpawnWeaponEffects(QuickHitProfile, spawnTrail: true);
		}
	}

	protected virtual void OnQuickHit(string direction)
	{
		if (requestedQuickHits == null)
		{
			requestedQuickHits = new List<QuickHitInfo>();
		}
		for (int i = 0; i < damageTasks.Count; i++)
		{
			ReleasePushEffectDependencies(damageTasks[i].Actor);
			CharacterAnimationController characterAnimationController = damageTasks[i].ActorView.CharacterAnimationController;
			if (characterAnimationController != null)
			{
				requestedQuickHits.Add(new QuickHitInfo
				{
					AnimationController = characterAnimationController,
					Delay = Random.value * 0.2f,
					Direction = direction,
					DamageAction = (damageTasks[i].Action as DamageAction)
				});
			}
		}
		SpawnWeaponEffects(QuickHitProfile, spawnTrail: false);
	}

	private void ReleasePushEffectDependencies(ActorModel target)
	{
		VisualizationQueue.Instance.GetMostRecentlyAddedActorTask<PushActorVisualizationTask>(target)?.ReleaseDependenciesToAttacker();
	}

	private void UpdateQuickHits()
	{
		if (requestedQuickHits == null)
		{
			return;
		}
		for (int num = requestedQuickHits.Count - 1; num >= 0; num--)
		{
			requestedQuickHits[num].Delay -= Time.deltaTime;
			if (requestedQuickHits[num].Delay <= 0f)
			{
				CharacterAnimationController animationController = requestedQuickHits[num].AnimationController;
				if (animationController != null && base.ActorView != null && base.ActorView.CurrentWeapon != null && base.ActorView.CurrentWeapon.Definition != null)
				{
					animationController.QuickHit(requestedQuickHits[num].Direction, base.ActorView.CurrentWeapon.Definition.Type, base.ActorView.transform.position, requestedQuickHits[num].DamageAction, base.ActorView.CurrentWeapon.Definition.SubCategory);
					requestedQuickHits.RemoveAt(num);
				}
			}
		}
	}

	protected virtual void OnUseWeapon(bool preImpact)
	{
		if (preImpact)
		{
			GameManager.Instance.StartCoroutine(SpawnEffects());
		}
		if (!TargetEndsUpDead || !preImpact)
		{
			ReleaseAllDependencies();
			SurvivorAnimationController survivorAnimationController = base.ActorView.CharacterAnimationController as SurvivorAnimationController;
			if (survivorAnimationController != null)
			{
				survivorAnimationController.DesiredWeaponPose = WeaponPose.Lowered;
			}
		}
	}

	private void SpawnOnHitEffect(Vector3 target)
	{
		GameObject currentWeaponPrefab = base.ActorView.GetCurrentWeaponPrefab();
		WeaponEffectsSpawner weaponEffectsSpawner = (currentWeaponPrefab ? currentWeaponPrefab.GetComponent<WeaponEffectsSpawner>() : null);
		if (weaponEffectsSpawner != null && weaponEffectsSpawner.onHitEffectPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(weaponEffectsSpawner.onHitEffectPrefab);
			gameObject.transform.position = target;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			if (!string.IsNullOrEmpty(weaponEffectsSpawner.onHitSoundEvent) && SingularityMonoBehaviour<AudioManager>.Instance != null)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(weaponEffectsSpawner.onHitSoundEvent, gameObject);
			}
		}
	}
}
