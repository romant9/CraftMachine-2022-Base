namespace TWDModel
{
	public class AbilityEffectThrowable : AbilityEffect
	{
		private string objectIdentifier;

		public AbilityEffectThrowable(string identifier)
		{
			objectIdentifier = identifier;
		}

		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null)
		{
			return true;
		}
	}
}
