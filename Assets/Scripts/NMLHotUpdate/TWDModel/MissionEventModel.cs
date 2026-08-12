namespace TWDModel
{
	public class MissionEventModel : TWDModelObjectWithViewId, TriggerReceiver
	{
		public MissionEventType Type;

		public const string triggeredEvent = "triggerStateChanged";

		public MissionEventModel()
		{
		}

		public MissionEventModel(string viewId, MissionEventType eventType)
		{
			base.ViewId = viewId;
			Type = eventType;
		}

		public void OnTriggered(ActorModel instigator)
		{
			NotifyChange("triggerStateChanged");
			switch (Type)
			{
			case MissionEventType.MissionCompleted:
				base.manager.CombatModel.ForceEndMissionVictory();
				break;
			case MissionEventType.MissionFailed:
				base.manager.CombatModel.ForceEndMissionFailure();
				break;
			}
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
