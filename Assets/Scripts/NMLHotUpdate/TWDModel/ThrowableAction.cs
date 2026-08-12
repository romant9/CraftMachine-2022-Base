using BaseModel;

namespace TWDModel
{
	public class ThrowableAction : ModelAction
	{
		public ActorModel Actor { get; private set; }

		public GridCoordinate TargetCoordinate { get; private set; }

		public string ObjectIdentifier { get; private set; }

		private int lureLifetime { get; set; }

		private int lureEffectDuration { get; set; }

		public ActorModel InstantiatedModel { get; private set; }

		public ThrowableAction(ActorModel actor, GridCoordinate targetCoordinate, string objectIdentifier, int lifeTime, int duration)
			: base(actor)
		{
			Actor = actor;
			TargetCoordinate = targetCoordinate;
			ObjectIdentifier = objectIdentifier;
			lureLifetime = lifeTime;
			lureEffectDuration = duration;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null && Actor != null)
			{
				InstantiatedModel = combatModel.CreateActor(TargetCoordinate, Faction.Lure, lureLifetime, 0, null, null, -1, "Props", ObjectIdentifier);
				combatModel.CollectWalkersToEatLure(InstantiatedModel, lureEffectDuration);
				combatModel.Perceptors.Add(InstantiatedModel);
				combatModel.UpdateAllActorsVisibility();
				combatModel.UpdateObjectsVisibility();
				return true;
			}
			return false;
		}
	}
}
