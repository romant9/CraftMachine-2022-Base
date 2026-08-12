using BaseModel;

namespace TWDModel
{
	public class ThreatAction : ModelActorAction
	{
		public int ChangeValue { get; set; }

		public ThreatAction(ActorModel actor, int value)
			: base(actor)
		{
			ChangeValue = value;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null)
			{
				ThreatInstigator instigator = ((!base.Actor.IsFriendlyHuman) ? ThreatInstigator.Enemy : ThreatInstigator.Survivor);
				return combatModel.ChangeThreatLevel(ChangeValue, instigator);
			}
			return false;
		}

		public override bool CanExecute()
		{
			bool num = base.Actor != null;
			bool flag = !base.Actor.IsDead || base.Actor.IsExploding;
			return num && flag;
		}
	}
}
