namespace TWDModel
{
	public class LootModel : TWDModelObjectWithViewId, InteractionReceiver, TriggerReceiver
	{
		public const string ChangeIsOpened = "IsOpened";

		public bool ContainsKey;

		public bool IsOpened { get; set; }

		public LootModel()
		{
		}

		public LootModel(string viewId)
		{
			base.ViewId = viewId;
		}

		public override void Initialize()
		{
			base.Initialize();
			IsOpened = false;
		}

		public void OnInteractionStep(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnInteractionCanceled(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnAttacked(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnDestroyed(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
		}

		public void OnInteractionCompleted(InteractiveObjectModel interactiveOject, ActorModel instigator)
		{
			Loot(instigator);
		}

		public void OnTriggered(ActorModel instigator)
		{
			Loot(instigator);
		}

		public void Loot(ActorModel instigator)
		{
			if (!IsOpened)
			{
				if (ContainsKey)
				{
					base.manager.Player.LootManager.AddCombatFoundKey(1);
					base.manager.CombatModel.MissionStatistics.AddCollectedLoot();
				}
				IsOpened = true;
				NotifyChange("IsOpened", instigator);
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
