namespace TWDModel
{
	public class ProjectileAction : FireWeaponAction
	{
		public ProjectileAction(ActorModel actor, ActorModel targetActor, GridCoordinate gridCoordinate, AbilityModel ability)
			: base(actor, targetActor, gridCoordinate, ability)
		{
		}
	}
}
