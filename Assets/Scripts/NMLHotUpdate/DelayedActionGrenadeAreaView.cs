using BaseModel;
using Client.Utils;
using TWDModel;
using UnityEngine;

public class DelayedActionGrenadeAreaView : CombatModelView
{
	private const float ExplosionLifetimeSeconds = 3f;

	private const float CountdownAnchorHeight = 1.2f;

	[SerializeField]
	private GameObject groundModel;

	[SerializeField]
	private GameObject explosionPrefab;

	private GameObject activationRangeIndicator;

	private WeaponRangeVisualization explosionRangeVisualizer;

	private GameObject countdownAnchor;

	private TurnCountIndicator countdownIndicator;

	private bool isSubscribedToTurnChanges;

	public GridCoordinate GridCoordinate
	{
		get
		{
			if (!(base.Model is DelayedActionGrenadeArea delayedActionGrenadeArea))
			{
				return GridCoordinate.Invalid;
			}
			return delayedActionGrenadeArea.EffectiveAreaGridCoordinate;
		}
	}

	private WeaponRangeVisualization ExplosionRangeVisualizer
	{
		get
		{
			if (activationRangeIndicator != null && explosionRangeVisualizer == null)
			{
				explosionRangeVisualizer = activationRangeIndicator.GetComponent<WeaponRangeVisualization>();
			}
			return explosionRangeVisualizer;
		}
	}

	public override void Initialize(ModelObject model)
	{
		base.Initialize(model);
		DelayedActionGrenadeArea delayedActionGrenadeArea = (DelayedActionGrenadeArea)base.Model;
		base.transform.position = GridView.Instance.GetPosition(delayedActionGrenadeArea.EffectiveAreaGridCoordinate).ToVector3();
		HideGroundModel();
	}

	public void HideGroundModel()
	{
		Helpers.GameObjectSetActive(GetGroundModel(), value: false);
		HideCountdownIndicator();
	}

	public void ShowGroundModel()
	{
		if (base.Model != null)
		{
			Helpers.GameObjectSetActive(GetGroundModel(), value: true);
			ShowCountdownIndicator();
		}
	}

	public void ShowExplosionRange()
	{
		DelayedActionGrenadeArea delayedActionGrenadeArea = base.Model as DelayedActionGrenadeArea;
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		if (delayedActionGrenadeArea != null && combat != null)
		{
			EnsureActivationRangeIndicator();
			WeaponRangeVisualization weaponRangeVisualization = ExplosionRangeVisualizer;
			if (!(weaponRangeVisualization == null))
			{
				float radius = (float)delayedActionGrenadeArea.ExplosionRadius * (float)combat.Grid.CellSize.X + (float)combat.Grid.CellSize.X / 2f;
				weaponRangeVisualization.SetCircle(base.transform.position, radius);
			}
		}
	}

	public void ClearExplosionRange()
	{
		ExplosionRangeVisualizer?.Clear();
	}

	public override void Kill()
	{
		ClearExplosionRange();
		DestroyCountdownIndicator();
		DelayedActionGrenadeAreaInputHandler.ClearIfSelected(this);
		if (base.Model is DelayedActionGrenadeArea delayedActionGrenadeArea)
		{
			DelayedActionGrenadeThrowVisualizationTask.RegisterPendingDetonation(delayedActionGrenadeArea.EffectiveAreaGridCoordinate, delayedActionGrenadeArea.ExplosionRadius);
		}
		if (DelayedActionGrenadeThrowVisualizationTask.IsThrowPendingForCell(GridCoordinate))
		{
			DelayedActionGrenadeThrowVisualizationTask.RegisterDeferredExplosion(GridCoordinate, this);
			return;
		}
		VisualizationQueue.Instance.Add(new CustomVisualizationTask(delegate
		{
			SpawnExplosionEffect();
			base.Kill();
		}));
	}

	public void PlayDeferredExplosionAndDestroy(Vector3 position)
	{
		PlayDeferredExplosionAt(position);
		DestroyViewImmediate();
	}

	public void PlayDeferredExplosionAt(Vector3 position)
	{
		SpawnExplosionEffectAt(position);
	}

	public void DestroyViewImmediate()
	{
		DelayedActionGrenadeAreaInputHandler.ClearIfSelected(this);
		Object.Destroy(base.gameObject);
	}

	protected override void OnDestroy()
	{
		DestroyCountdownIndicator();
		DelayedActionGrenadeAreaInputHandler.ClearIfSelected(this);
		base.OnDestroy();
	}

	private GameObject GetGroundModel()
	{
		if (groundModel != null)
		{
			return groundModel;
		}
		Transform transform = base.transform.Find("Weapon_Throwable_DynamiteBundle");
		if (!(transform != null))
		{
			return null;
		}
		return transform.gameObject;
	}

	private void EnsureActivationRangeIndicator()
	{
		if (!(activationRangeIndicator != null))
		{
			PrefabResource prefabResource = UnityUtils.LoadAsset("Combat/ActivationRangeIndicator") as PrefabResource;
			if (prefabResource == null)
			{
				Debug.LogError("Could not find resource: Combat/ActivationRangeIndicator");
				return;
			}
			activationRangeIndicator = Object.Instantiate(prefabResource.GetPrefab());
			activationRangeIndicator.transform.SetParent(base.transform, worldPositionStays: false);
			activationRangeIndicator.transform.localPosition = Vector3.zero;
		}
	}

	private void EnsureCountdownIndicator()
	{
		if (countdownIndicator != null)
		{
			return;
		}
		CombatHUD combatHUD = CombatView.Instance?.CombatHUD;
		if (!(combatHUD == null))
		{
			countdownAnchor = new GameObject("DelayedActionGrenadeCountdownAnchor");
			countdownAnchor.transform.SetParent(base.transform, worldPositionStays: false);
			countdownAnchor.transform.localPosition = new Vector3(0f, 1.2f, 0f);
			countdownIndicator = combatHUD.CreateGrenadeTurnCountIndicator();
			if (countdownIndicator == null)
			{
				Object.Destroy(countdownAnchor);
				countdownAnchor = null;
			}
			else
			{
				countdownIndicator.FollowTarget(countdownAnchor);
			}
		}
	}

	private void ShowCountdownIndicator()
	{
		EnsureCountdownIndicator();
		SubscribeTurnChanges();
		RefreshCountdownIndicator();
	}

	private void HideCountdownIndicator()
	{
		UnsubscribeTurnChanges();
		if (countdownIndicator != null)
		{
			countdownIndicator.SetTurnCount(-1);
		}
	}

	private void DestroyCountdownIndicator()
	{
		UnsubscribeTurnChanges();
		if (countdownIndicator != null)
		{
			Object.Destroy(countdownIndicator.gameObject);
			countdownIndicator = null;
		}
		if (countdownAnchor != null)
		{
			Object.Destroy(countdownAnchor);
			countdownAnchor = null;
		}
	}

	private void RefreshCountdownIndicator()
	{
		DelayedActionGrenadeArea delayedActionGrenadeArea = base.Model as DelayedActionGrenadeArea;
		CombatModel combatModel = GameManager.Instance?.playerModel?.Combat;
		if (delayedActionGrenadeArea != null && combatModel != null && !(countdownIndicator == null))
		{
			int num = delayedActionGrenadeArea.DetonateTurn - combatModel.TurnManager.TurnCount;
			countdownIndicator.SetTurnCount((num <= 0) ? (-1) : num);
		}
	}

	private void SubscribeTurnChanges()
	{
		if (!isSubscribedToTurnChanges)
		{
			CombatModel combatModel = GameManager.Instance?.playerModel?.Combat;
			if (combatModel?.TurnManager != null)
			{
				combatModel.TurnManager.Changed += OnTurnManagerChanged;
				isSubscribedToTurnChanges = true;
			}
		}
	}

	private void UnsubscribeTurnChanges()
	{
		if (isSubscribedToTurnChanges)
		{
			CombatModel combatModel = GameManager.Instance?.playerModel?.Combat;
			if (combatModel?.TurnManager != null)
			{
				combatModel.TurnManager.Changed -= OnTurnManagerChanged;
			}
			isSubscribedToTurnChanges = false;
		}
	}

	private void OnTurnManagerChanged(ModelObject model, string changed, object args)
	{
		if (!(changed != "TurnCountChanged"))
		{
			RefreshCountdownIndicator();
		}
	}

	private void SpawnExplosionEffect()
	{
		SpawnExplosionEffectAt(base.transform.position);
	}

	private void SpawnExplosionEffectAt(Vector3 position)
	{
		ShowDetonationNotificationAt(position);
		PlayExplosionSound();
		if (explosionPrefab != null)
		{
			Object.Destroy(Object.Instantiate(explosionPrefab, position, Quaternion.identity), 3f);
		}
		DelayedActionGrenadeThrowVisualizationTask.ShowDeferredFlameTrapsForDetonation(GridCoordinate);
	}

	private void PlayExplosionSound()
	{
		SingularityMonoBehaviour<AudioManager>.Instance?.PlayEvent("combat_level/barrel_explosion_1", base.gameObject);
	}

	private static void ShowDetonationNotificationAt(Vector3 worldPosition)
	{
		GameObject obj = new GameObject("DelayedActionGrenadeDetonationNotification");
		obj.transform.position = worldPosition + new Vector3(0f, 1.2f, 0f);
		obj.AddComponent<DelayedActionGrenadeDetonationNotification>().Play(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Traits.DelayedActionGrenade"));
	}
}
