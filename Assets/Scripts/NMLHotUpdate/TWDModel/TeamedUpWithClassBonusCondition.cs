using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class TeamedUpWithClassBonusCondition : BaseBonusCondition
	{
		private SurvivorClass ClassRequired;

		public TeamedUpWithClassBonusCondition(FixedPoint bonus, string requiredClassInTeam)
			: base(bonus)
		{
			ClassRequired = (SurvivorClass)Enum.Parse(typeof(SurvivorClass), requiredClassInTeam);
		}

		public override bool Evaluate(ConditionContext context)
		{
			List<ActorModel> survivors = context.GetSurvivors();
			ActorModel badgeOwner = context.GetBadgeOwner();
			if (survivors != null && badgeOwner != null)
			{
				for (int i = 0; i < survivors.Count; i++)
				{
					if (survivors[i] is SurvivorModel survivorModel && badgeOwner != survivorModel && survivorModel.SurvivorClass == ClassRequired)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
