namespace TWDModel
{
	public class SupportResetCombatLoadQueueMessage : SupportLoadQueueMessage
	{
		public override bool Execute(TWDModelManager manager)
		{
			manager.Metrics.AddResetCombat(manager.Player.Combat != null).AddSupport(base.SupportGivenTimestamp, base.SupportEntityGUID).Send();
			if (manager.Player.Combat != null)
			{
				manager.Player.DeleteCombatModel(notify: false);
			}
			return true;
		}
	}
}
