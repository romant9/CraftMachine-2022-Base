using System.Collections.Generic;

namespace TWDModel
{
	public class BeatEmUpTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (actor.dashTraitAttackFlag)
			{
				return ActionListClearFlag.Keep;
			}
			if (action is ChangeTurnAction)
			{
				CombatModel combatModel = actor.manager.CombatModel;
				if (actor != null && combatModel != null && !combatModel.MissionCompleted)
				{
					if (actor.Faction != combatModel.TurnManager.ActiveFaction)
					{
						return ActionListClearFlag.Keep;
					}
					if (actor.IsInvisible)
					{
						return ActionListClearFlag.Keep;
					}
					EnumerableNeighbors enumerableNeighbors = combatModel.Grid.Neighbors(actor.GridCoordinate);
					EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
					List<ActorModel> list = new List<ActorModel>();
					foreach (GridCoordinate item2 in enumerableNeighbors)
					{
						ActorModel occupier = combatModel.GetOccupier(item2);
						if (occupier != null && actor.IsEnemy(occupier) && !occupier.Definition.IsEnvironmental)
						{
							list.Add(occupier);
						}
					}
					ActorModel actorModel = ((list.Count > 0) ? combatModel.manager.Player.PlayerRandom.GetRandomElement(list, remove: false) : null);
					if (weaponEquipment != null && actorModel != null)
					{
						FixedPoint value = 0.0;
						combatModel.AbilityManager.VisitParameter("LeaderBuffBeatEmUpPunishMultiplier", ref value, actor);
						BeatEmUpAction item = new BeatEmUpAction(actor, weaponEquipment.Ability, actorModel.GridCoordinate, "ActorNotification.BeatEmUp", value, actorModel, OOTType.None, isTriggerExtraAttackDamage: true);
						addedActions.Add(item);
					}
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
