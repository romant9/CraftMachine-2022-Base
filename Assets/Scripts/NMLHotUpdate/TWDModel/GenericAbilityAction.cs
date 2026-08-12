namespace TWDModel
{
	public class GenericAbilityAction : AbilityAction
	{
		public string NotificationKey { get; protected set; }

		public GenericAbilityAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, string notificationKey, ActorModel targetActor = null, OOTType ootType = OOTType.None, bool skipActiveWeaponTraits = false, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
			: base(sourceActor, ability, targetCell, targetActor, ootType, skipActiveWeaponTraits, isAssistAttack, isTriggerExtraAttackDamage)
		{
			NotificationKey = notificationKey;
		}
	}
}
