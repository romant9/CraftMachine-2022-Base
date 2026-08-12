namespace TWDModel
{
	public class ModifyMissionVariableModel : TWDModelObject, TriggerReceiver, InteractionReceiver
	{
		public ModifyMissionVariableOperation VariableOperation { get; set; }

		public int VariableHash { get; set; }

		public int Value { get; set; }

		private void PerformModification()
		{
			int orCreateVariable = base.manager.Player.Combat.GetOrCreateVariable(VariableHash);
			int num = orCreateVariable;
			switch (VariableOperation)
			{
			case ModifyMissionVariableOperation.Set:
				num = Value;
				break;
			case ModifyMissionVariableOperation.Add:
				num += Value;
				break;
			case ModifyMissionVariableOperation.Sub:
				num -= Value;
				break;
			}
			if (num != orCreateVariable)
			{
				base.manager.Player.Combat.SetVariable(VariableHash, num);
			}
		}

		void TriggerReceiver.OnTriggered(ActorModel instigator)
		{
			PerformModification();
		}

		void InteractionReceiver.OnInteractionCompleted(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
			PerformModification();
		}

		void InteractionReceiver.OnInteractionStep(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		void InteractionReceiver.OnInteractionCanceled(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		void InteractionReceiver.OnAttacked(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		void InteractionReceiver.OnDestroyed(InteractiveObjectModel interactiveObject, ActorModel instigator)
		{
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
