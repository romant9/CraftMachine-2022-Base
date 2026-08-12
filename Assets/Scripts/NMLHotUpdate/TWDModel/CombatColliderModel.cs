namespace TWDModel
{
	public class CombatColliderModel : TWDModelObjectWithViewId, InteractionReceiver, TriggerReceiver
	{
		public const string IsEnabledChanged = "IsEnabled";

		public bool BlockMovement { get; private set; }

		public bool BlockVision { get; private set; }

		public bool IsDynamic { get; private set; }

		public bool IsEnabled { get; set; }

		public CombatColliderModel()
		{
			IsEnabled = true;
		}

		public CombatColliderModel(string viewId)
		{
			IsEnabled = true;
			base.ViewId = viewId;
		}

		public CombatColliderModel(string viewId, bool blockMovement, bool blockVision, bool isDynamic, bool isEnabled)
		{
			base.ViewId = viewId;
			BlockMovement = blockMovement;
			BlockVision = blockVision;
			IsDynamic = isDynamic;
			IsEnabled = isEnabled;
		}

		public override bool IsValid()
		{
			return true;
		}

		public void OnInteractionStep(InteractiveObjectModel instigator, ActorModel interactingActor)
		{
		}

		public void OnInteractionCanceled(InteractiveObjectModel instigator, ActorModel interactingActor)
		{
		}

		public void OnAttacked(InteractiveObjectModel instigator, ActorModel attackingActor)
		{
		}

		public void OnDestroyed(InteractiveObjectModel instigator, ActorModel attackingActor)
		{
			if (IsEnabled)
			{
				IsEnabled = false;
				base.manager.CombatModel.UpdateDynamicColliders();
				NotifyChange("IsEnabled", this);
			}
		}

		public void OnInteractionCompleted(InteractiveObjectModel instigator, ActorModel interactingActor)
		{
			FlipCollision();
		}

		public void OnTriggered(ActorModel instigator)
		{
			FlipCollision();
		}

		public void SetEnabled(bool enabled)
		{
			if (IsEnabled != enabled)
			{
				IsEnabled = enabled;
				base.manager.CombatModel.UpdateDynamicColliders();
				NotifyChange("IsEnabled", this);
			}
		}

		private void FlipCollision()
		{
			IsEnabled = !IsEnabled;
			base.manager.CombatModel.UpdateDynamicColliders();
			NotifyChange("IsEnabled", this);
		}
	}
}
