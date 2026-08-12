using BaseModel;

namespace TWDModel
{
	public class TriggerAction : ModelAction
	{
		public ActorModel Actor { get; private set; }

		public TriggerModel Trigger { get; private set; }

		public TriggerAction(ActorModel actor, TriggerModel trigger)
			: base(actor)
		{
			Actor = actor;
			Trigger = trigger;
		}

		public override bool Execute(ModelManager manager)
		{
			if (Trigger == null)
			{
				manager.Debug.LogWarning("TriggerAction::Execute() failed -> TriggerModel is null");
				return false;
			}
			if (Actor == null)
			{
				manager.Debug.LogWarning("TriggerAction::Execute() failed -> ActorModel is null");
				return false;
			}
			if (!Trigger.TriggerReserved(Actor))
			{
				manager.Debug.LogWarning("TriggerAction::Execute() failed -> Reserved trigger would not trigger!");
				return false;
			}
			return true;
		}
	}
}
