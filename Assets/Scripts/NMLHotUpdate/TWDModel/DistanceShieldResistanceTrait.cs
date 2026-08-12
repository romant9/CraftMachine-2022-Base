using System.Collections.Generic;

namespace TWDModel
{
	public class DistanceShieldResistanceTrait : ActionModifier
	{
		private FixedPoint resistanceHitPointsPercent;

		private FixedPoint resistanceRange;

		private FixedPoint preAttackHitPointsPercent = 1L;

		public DistanceShieldResistanceTrait(FixedPoint resistanceHitPointsPercent, FixedPoint resistanceRange)
		{
			this.resistanceHitPointsPercent = resistanceHitPointsPercent;
			this.resistanceRange = resistanceRange;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (action is PreAttackAction)
			{
				preAttackHitPointsPercent = FixedPoint.Min((FixedPoint)actor.Hitpoints / (FixedPoint)actor.MaxHitPoints, 1.0);
				return ActionListClearFlag.Keep;
			}
			if (preAttackHitPointsPercent >= resistanceHitPointsPercent)
			{
				if (!(action is StunAction stunAction))
				{
					if (!(action is RootAction rootAction))
					{
						if (action is CrippleAction crippleAction && crippleAction.TargetActor == actor && crippleAction.SourceActor.GridCoordinate.ChebyshevDistance(crippleAction.TargetActor.GridCoordinate) > resistanceRange)
						{
							crippleAction.Avoided = true;
						}
					}
					else if (rootAction.TargetActor == actor && rootAction.SourceActor.GridCoordinate.ChebyshevDistance(rootAction.TargetActor.GridCoordinate) > resistanceRange)
					{
						rootAction.Avoided = true;
					}
				}
				else if (stunAction.TargetActor == actor && stunAction.SourceActor.GridCoordinate.ChebyshevDistance(stunAction.TargetActor.GridCoordinate) > resistanceRange)
				{
					stunAction.Avoided = true;
				}
			}
			return ActionListClearFlag.Keep;
		}
	}
}
