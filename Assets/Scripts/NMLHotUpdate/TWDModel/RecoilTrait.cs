using System.Collections.Generic;

namespace TWDModel
{
	public class RecoilTrait : ActionModifier
	{
		private readonly FixedPoint damageReduce;

		public RecoilTrait(FixedPoint mult, FixedPoint normalStunChance, FixedPoint circleStunChance)
		{
			damageReduce = mult;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			return ActionListClearFlag.Keep;
		}
	}
}
