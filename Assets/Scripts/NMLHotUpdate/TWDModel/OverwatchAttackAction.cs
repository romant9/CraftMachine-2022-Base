namespace TWDModel
{
	public class OverwatchAttackAction : AbilityAction
	{
		private GridCoordinate targetMoveStartCoordinate;

		public bool Interrupted { get; set; }

		public bool BodyShot { get; set; }

		public OverwatchAttackAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, GridCoordinate targetMoveStartCoordinate, ActorModel targetActor = null, OOTType ootType = OOTType.None, bool isTriggerExtraAttackDamage = false)
			: base(sourceActor, ability, targetCell, targetActor, ootType, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage)
		{
			hasOrderWhenGrouped = true;
			this.targetMoveStartCoordinate = targetMoveStartCoordinate;
			base.Actor.NotifyChange("AbilityVisited", new object[2] { "Overwatch", false });
		}

		public override int SortOrder()
		{
			if (base.Actor != null && base.Actor.manager.CombatModel != null)
			{
				GridModel grid = base.Actor.manager.CombatModel.Grid;
				FixedVec3 position = grid.GetPosition(base.Actor.GridCoordinate);
				FixedVec3 position2 = grid.GetPosition(targetMoveStartCoordinate);
				return (int)(position - position2).SqrMagnitude;
			}
			return int.MaxValue;
		}
	}
}
