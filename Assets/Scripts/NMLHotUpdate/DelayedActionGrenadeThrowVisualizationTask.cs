using System;
using System.Collections.Generic;
using System.Linq;
using Client.Utils;
using NextGames.Sdk.AssetBundleManager;
using TWDModel;
using UnityEngine;

public class DelayedActionGrenadeThrowVisualizationTask : ActorVisualizationTask
{
	private sealed class PendingThrowInfo
	{
		public readonly object Lock = new object();

		public ActorModel Thrower;
	}

	private const string GrenadePrefabName = "Consumable_Grenade";

	private const float ThrowSpeed = 12f;

	private static readonly Dictionary<GridCoordinate, PendingThrowInfo> PendingThrows = new Dictionary<GridCoordinate, PendingThrowInfo>();

	private static readonly Dictionary<GridCoordinate, List<DelayedActionGrenadeAreaView>> DeferredExplosionViews = new Dictionary<GridCoordinate, List<DelayedActionGrenadeAreaView>>();

	private static readonly Dictionary<GridCoordinate, int> PendingDetonations = new Dictionary<GridCoordinate, int>();

	private static readonly Dictionary<GridCoordinate, List<TrapFlameAreaView>> DeferredFlameTrapViewsByDetonation = new Dictionary<GridCoordinate, List<TrapFlameAreaView>>();

	private readonly GridCoordinate targetCell;

	private readonly Vector3 finalPosition;

	private readonly Vector3 actorStartPosition;

	private Vector3 initialPosition;

	private GameObject projectile;

	private float totalThrowDuration;

	private float throwTime;

	private bool projectileThrown;

	private bool deferredExplosionHandled;

	private FireWeaponState state;

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

	public static bool IsThrowPendingForCell(GridCoordinate cell)
	{
		if (cell.IsValid)
		{
			return PendingThrows.ContainsKey(cell);
		}
		return false;
	}

	public static void AddDependenciesForPendingThrowAttacker(VisualizationTask task, ActorModel attacker)
	{
		if (task == null || attacker == null)
		{
			return;
		}
		foreach (PendingThrowInfo value in PendingThrows.Values)
		{
			if (value != null && value.Thrower == attacker)
			{
				task.AddDependency(value.Lock, reserve: false);
			}
		}
	}

	public static void RegisterPendingDetonation(GridCoordinate center, int explosionRadius)
	{
		if (center.IsValid)
		{
			PendingDetonations[center] = explosionRadius;
		}
	}

	public static bool TryDeferFlameTrapUntilDetonation(TrapFlameAreaView view, GridCoordinate cell)
	{
		if (view == null || !cell.IsValid || PendingDetonations.Count == 0)
		{
			return false;
		}
		bool result = false;
		foreach (KeyValuePair<GridCoordinate, int> pendingDetonation in PendingDetonations)
		{
			if (cell.ChebyshevDistance(pendingDetonation.Key) <= pendingDetonation.Value)
			{
				if (!DeferredFlameTrapViewsByDetonation.TryGetValue(pendingDetonation.Key, out var value))
				{
					value = new List<TrapFlameAreaView>();
					DeferredFlameTrapViewsByDetonation[pendingDetonation.Key] = value;
				}
				if (!value.Contains(view))
				{
					value.Add(view);
					result = true;
				}
			}
		}
		return result;
	}

	public static void ShowDeferredFlameTrapsForDetonation(GridCoordinate center)
	{
		if (!center.IsValid)
		{
			return;
		}
		PendingDetonations.Remove(center);
		if (DeferredFlameTrapViewsByDetonation.TryGetValue(center, out var value))
		{
			DeferredFlameTrapViewsByDetonation.Remove(center);
			for (int i = 0; i < value.Count; i++)
			{
				value[i]?.SetFlameTrapVisible(visible: true);
			}
		}
	}

	public static void CancelPendingDetonation(GridCoordinate center)
	{
		if (!center.IsValid)
		{
			return;
		}
		PendingDetonations.Remove(center);
		if (DeferredFlameTrapViewsByDetonation.TryGetValue(center, out var value))
		{
			DeferredFlameTrapViewsByDetonation.Remove(center);
			for (int i = 0; i < value.Count; i++)
			{
				value[i]?.SetFlameTrapVisible(visible: true);
			}
		}
	}

	public static void RegisterDeferredExplosion(GridCoordinate cell, DelayedActionGrenadeAreaView view)
	{
		if (cell.IsValid && !(view == null))
		{
			if (!DeferredExplosionViews.TryGetValue(cell, out var value))
			{
				value = new List<DelayedActionGrenadeAreaView>();
				DeferredExplosionViews[cell] = value;
			}
			if (!value.Contains(view))
			{
				value.Add(view);
			}
		}
	}

	private static void TryPlayDeferredExplosionAndDestroy(GridCoordinate cell, Vector3 position)
	{
		if (!cell.IsValid)
		{
			return;
		}
		if (!DeferredExplosionViews.TryGetValue(cell, out var value) || value.Count == 0)
		{
			ShowDeferredFlameTrapsForDetonation(cell);
			return;
		}
		DeferredExplosionViews.Remove(cell);
		value[0]?.PlayDeferredExplosionAt(position);
		for (int i = 0; i < value.Count; i++)
		{
			value[i]?.DestroyViewImmediate();
		}
	}

	private static void CancelDeferredExplosion(GridCoordinate cell)
	{
		if (!cell.IsValid || !DeferredExplosionViews.TryGetValue(cell, out var value))
		{
			CancelPendingDetonation(cell);
			return;
		}
		DeferredExplosionViews.Remove(cell);
		for (int i = 0; i < value.Count; i++)
		{
			value[i]?.DestroyViewImmediate();
		}
		CancelPendingDetonation(cell);
	}

	public DelayedActionGrenadeThrowVisualizationTask(ActorModel actor, GridCoordinate targetCell)
		: base(null)
	{
		base.Actor = actor;
		base.ActorView = GameManager.Instance.GetViewForModel(actor) as ActorView;
		this.targetCell = targetCell;
		AddFactionDependency(actor.Faction);
		AddActorDependency(actor);
		actorStartPosition = ((base.ActorView != null) ? base.ActorView.transform.position : Vector3.zero);
		finalPosition = GridView.Instance.GetPosition(targetCell).ToVector3();
		throwTime = 0f;
		state = FireWeaponState.Start;
		if (!PendingThrows.TryGetValue(targetCell, out var value))
		{
			value = new PendingThrowInfo();
			PendingThrows[targetCell] = value;
		}
		value.Thrower = actor;
		AddDependency(value.Lock);
	}

	public override List<VisualizationTask> TasksToQueue()
	{
		List<VisualizationTask> list = new List<VisualizationTask>
		{
			new TurnToTargetVisualizationTask(base.Actor, actorStartPosition, finalPosition)
		};
		SurvivorAnimationController survivorAnimationController = CharacterAnimationController as SurvivorAnimationController;
		if (survivorAnimationController != null && survivorAnimationController.CurrentWeaponPose != WeaponPose.Raised && survivorAnimationController.CurrentWeaponPose != WeaponPose.BeingRaised && !survivorAnimationController.IsInInteractionLoop)
		{
			list.Add(new ChangeWeaponPoseVisualizationTask(base.Actor, WeaponPose.Raised));
		}
		list.Add(this);
		return list;
	}

	public override void Start()
	{
		base.Start();
		SetupDamageAndDeathDelay();
		HideGroundBombViewAtTarget();
		if (CharacterAnimationController == null)
		{
			SpawnProjectileImmediately();
		}
	}

	public override bool Update(float deltaTime)
	{
		if (state == FireWeaponState.Start)
		{
			return RaiseWeapon();
		}
		if (state == FireWeaponState.WaitingForActionCamera)
		{
			CharacterAnimationController.OnUseWeapon += OnUseWeapon;
			CharacterAnimationController.UseWeapon(criticalDamage: false, useFenceAttack: false, useChargeAttack: false);
			state = FireWeaponState.Attack;
			return true;
		}
		if (CharacterAnimationController == null || CharacterAnimationController.IsInDeath)
		{
			CleanupProjectile();
			return false;
		}
		if (!projectileThrown)
		{
			return true;
		}
		if (projectile != null && !projectile.activeSelf)
		{
			ActivateProjectileAtWeapon();
		}
		throwTime += deltaTime;
		if (throwTime >= totalThrowDuration)
		{
			FinishThrow();
			return false;
		}
		UpdateProjectilePosition(deltaTime);
		return true;
	}

	public override void Finished()
	{
		CleanupProjectile();
		base.ActorView?.SetWeaponActive(active: true);
		PendingThrows.Remove(targetCell);
		if (!deferredExplosionHandled)
		{
			CancelDeferredExplosion(targetCell);
		}
		base.Finished();
	}

	private void SpawnProjectileImmediately()
	{
		GameObject gameObject = LoadGrenadePrefab();
		if (gameObject == null)
		{
			FinishThrow();
			return;
		}
		projectile = UnityEngine.Object.Instantiate(gameObject);
		projectile.SetActive(value: false);
		initialPosition = actorStartPosition;
		RecalculateThrowDuration();
		projectileThrown = true;
		state = FireWeaponState.Attack;
	}

	private void ActivateProjectileAtWeapon()
	{
		GameObject currentWeaponPrefab = base.ActorView.GetCurrentWeaponPrefab();
		if (currentWeaponPrefab != null)
		{
			currentWeaponPrefab.gameObject.SetActive(value: false);
			initialPosition = currentWeaponPrefab.transform.position;
		}
		else
		{
			initialPosition = actorStartPosition;
		}
		RecalculateThrowDuration();
		throwTime = 0f;
		projectile.transform.position = initialPosition;
		projectile.SetActive(value: true);
	}

	private void RecalculateThrowDuration()
	{
		float magnitude = (finalPosition - initialPosition).magnitude;
		totalThrowDuration = Mathf.Max(magnitude / 12f, 0.01f);
	}

	private void UpdateProjectilePosition(float deltaTime)
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
	}

	private void HideGroundBombViewAtTarget()
	{
		GetGroundBombViewAtTarget()?.HideGroundModel();
	}

	private void FinishThrow()
	{
		if (CharacterAnimationController != null)
		{
			CharacterAnimationController.OnUseWeapon -= OnUseWeapon;
		}
		if (projectile != null)
		{
			projectile.transform.position = finalPosition;
		}
		CleanupProjectile();
		DelayedActionGrenadeArea delayedActionGrenadeArea = FindBombAtCell(targetCell);
		if (delayedActionGrenadeArea != null)
		{
			GetBombView(delayedActionGrenadeArea)?.ShowGroundModel();
		}
		else
		{
			TryPlayDeferredExplosionAndDestroy(targetCell, finalPosition);
			deferredExplosionHandled = true;
		}
		ReleaseAllDependencies();
	}

	private void SetupDamageAndDeathDelay()
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
		else if (projectile == null)
		{
			GameObject gameObject = LoadGrenadePrefab();
			if (gameObject != null)
			{
				projectile = UnityEngine.Object.Instantiate(gameObject);
				projectile.SetActive(value: false);
			}
			projectileThrown = gameObject != null;
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
		if (CharacterAnimationController == null || CharacterAnimationController.IsIdle || flag)
		{
			state = FireWeaponState.WaitingForActionCamera;
		}
		return true;
	}

	private void CleanupProjectile()
	{
		if (projectile != null)
		{
			UnityEngine.Object.Destroy(projectile);
			projectile = null;
		}
	}

	private DelayedActionGrenadeAreaView GetGroundBombViewAtTarget()
	{
		return GetBombView(FindBombAtCell(targetCell));
	}

	private static DelayedActionGrenadeAreaView GetBombView(DelayedActionGrenadeArea bomb)
	{
		if (bomb == null)
		{
			return null;
		}
		return GameManager.Instance.GetViewForModel((TWDModelObject)bomb) as DelayedActionGrenadeAreaView;
	}

	private static DelayedActionGrenadeArea FindBombAtCell(GridCoordinate cell)
	{
		return GameManager.Instance.playerModel.Combat?.Models.OfType<DelayedActionGrenadeArea>().FirstOrDefault((DelayedActionGrenadeArea bomb) => bomb.EffectiveAreaGridCoordinate == cell);
	}

	private static GameObject LoadGrenadePrefab()
	{
		return AssetBundleManager.Instance.LoadAsset<GameObject>("Consumable_Grenade", "weapons");
	}
}
