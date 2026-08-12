namespace TWDModel
{
	public class AbilityModifierArcUpgrade : ParameterModifier
	{
		private string ParameterName;

		private FixedPoint ArcUpgradeThreatLevel;

		public AbilityModifierArcUpgrade()
		{
		}

		public AbilityModifierArcUpgrade(string inParameterName, FixedPoint arcUpgradeThreatLevel)
		{
			ParameterName = inParameterName;
			ArcUpgradeThreatLevel = arcUpgradeThreatLevel;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == ParameterName)
			{
				if ((actor.manager.CombatModel?.ThreatMeter.ThreatLevel ?? 0) > ArcUpgradeThreatLevel)
				{
					value += (FixedPoint)90.0;
					return true;
				}
				return false;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { ParameterName };
		}
	}
}
