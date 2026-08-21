using BaseModel;

namespace TWDModel
{
	public class NoiseAction : ModelActorAction
	{
		public GridCoordinate Source { get; set; }

		public int NoiseRange { get; set; }

		public int ThreatValue { get; set; }

		public NoiseAction(ActorModel actor, GridCoordinate source, int range, int threatValue = 0)
			: base(actor)
		{
			Source = source;
			NoiseRange = range;
			ThreatValue = threatValue;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null)
			{
				if (!combatModel.IsEndlessBattleMission && !(base.Actor is TankActorModel))
				{
					base.Actor.NotifyChange("actorCreateThreat", ThreatValue);
				}
				return combatModel.CreateNoise(Source, NoiseRange);
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
