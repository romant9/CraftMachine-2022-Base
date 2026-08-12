using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffectDash : AbilityEffect
	{
		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null)
		{
			GridCoordinate gridCoordinate = source.GridCoordinate;
			bool num = (combatModel.Manager as TWDModelManager).ExecuteAction(new DashAction(source, targetCell, ownerAbility));
			if (num)
			{
				List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(ownerAbility, source, gridCoordinate, targetCell);
				CombatHelpers.AttackTargets(combatModel, source, listOfActorsToBeTargetted, ownerAbility, DamageType.Melee);
				if (ownerAbility.IsChargeAttack)
				{
					CombatHelpers.CheckForLeaderBuffLeadByExample(combatModel, source);
				}
				source.ClearPerAttackFlags();
			}
			return num;
		}
	}
}
