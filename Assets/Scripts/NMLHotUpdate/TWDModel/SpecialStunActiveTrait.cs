using System.Collections.Generic;

namespace TWDModel
{
	public class SpecialStunActiveTrait : ActionModifier
	{
		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is SpecialStunExtraDamageAction specialStunExtraDamageAction && specialStunExtraDamageAction.Actor == actor)
			{
				CombatModel combatModel = base.manager.CombatModel;
				EquipmentItemModel weaponEquipment = specialStunExtraDamageAction.Actor.GetWeaponEquipment();
				List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(weaponEquipment.Ability, specialStunExtraDamageAction.Actor, specialStunExtraDamageAction.Actor.GridCoordinate, specialStunExtraDamageAction.TargetCell);
				if (listOfActorsToBeTargetted.Count <= 0)
				{
					return ActionListClearFlag.Keep;
				}
				foreach (ActorModel item in listOfActorsToBeTargetted)
				{
					item.GetWeaponEquipment().AddTemporaryTrait("SpecialStunTargetActiveFlag", TraitExpirationType.Activation, 1.0);
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
