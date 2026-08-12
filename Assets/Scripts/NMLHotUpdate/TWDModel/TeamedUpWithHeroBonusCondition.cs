using System.Collections.Generic;

namespace TWDModel
{
	public class TeamedUpWithHeroBonusCondition : BaseBonusCondition
	{
		private string heroId;

		public TeamedUpWithHeroBonusCondition(FixedPoint bonus, string requiredHeroIdInTeam)
			: base(bonus)
		{
			heroId = requiredHeroIdInTeam;
		}

		public override bool Evaluate(ConditionContext context)
		{
			List<ActorModel> survivors = context.GetSurvivors();
			ActorModel badgeOwner = context.GetBadgeOwner();
			if (survivors != null && badgeOwner != null)
			{
				for (int i = 0; i < survivors.Count; i++)
				{
					if (survivors[i] is SurvivorModel survivorModel && badgeOwner != survivorModel && survivorModel.IsHero && survivorModel.Definition.GetNonAlternativeHeroDefinition() == heroId)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
