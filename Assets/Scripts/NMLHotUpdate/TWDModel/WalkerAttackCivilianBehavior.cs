namespace TWDModel
{
	public class WalkerAttackCivilianBehavior : ScriptedBehavior
	{
		public WalkerAttackCivilianBehavior(AIController controller)
			: base(controller)
		{
		}

		public override int GetPriority()
		{
			if (base.Controller.CombatModel.GetFactionActors(Faction.Civilian).Count > 0 && !base.AIDataModel.HasEvent(AIDataModel.ForceCivilianTargets) && !base.AIDataModel.HasEvent(AIDataModel.DamageReceived))
			{
				return 200;
			}
			return 0;
		}

		public override void ExecuteAction()
		{
			base.AIDataModel.SetEvent(AIDataModel.ForceCivilianTargets, 1);
			if (base.AIDataModel.Alertness < AIAlertness.Homing)
			{
				base.AIDataModel.Alertness = AIAlertness.Homing;
			}
		}
	}
}
