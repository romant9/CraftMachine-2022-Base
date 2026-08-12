using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffectCleave : AbilityEffect
	{
		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null)
		{
			bool flag = true;
			if (targetActor != null && targetActor.IsDead && source.KillsInTurn < 3)
			{
				List<ActorModel> list = new List<ActorModel>();
				FixedPoint range = ownerAbility.ParentAbility.Definition.AbilityRange;
				CombatHelpers.CalculateRangeExtension(ref range, source, combatModel.AbilityManager);
				foreach (ActorModel item in combatModel.GetActorsInRange(source.GridCoordinate, (int)range))
				{
					if (item != targetActor && !item.IsDead && ownerAbility.ParentAbility.CanAbilityBePerformedOnGridCell(combatModel, source, source.GridCoordinate, item.GridCoordinate) == AbilityResult.Success)
					{
						list.Add(item);
					}
				}
				ActorModel actorModel = null;
				if (list.Count > 0)
				{
					actorModel = combatModel.manager.Player.PlayerRandom.GetRandomElement(list, remove: false);
				}
				if (actorModel != null)
				{
					source.NotifyChange("actorCleaved");
					flag = combatModel.AbilityManager.PerformAbility(source, ownerAbility.ParentAbility, actorModel.GridCoordinate) == AbilityResult.Success;
					if (flag)
					{
						ownerAbility.ParentAbility.TotalUses--;
					}
				}
			}
			return flag;
		}
	}
}
