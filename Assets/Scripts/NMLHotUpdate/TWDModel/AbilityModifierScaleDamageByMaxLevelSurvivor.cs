namespace TWDModel
{
	public class AbilityModifierScaleDamageByMaxLevelSurvivor : ParameterModifier
	{
		private FixedPoint scalePercentage;

		public AbilityModifierScaleDamageByMaxLevelSurvivor()
		{
		}

		public AbilityModifierScaleDamageByMaxLevelSurvivor(FixedPoint scalePercentage)
		{
			this.scalePercentage = scalePercentage;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == "AbilityModifierScaleDamageByMaxSurvivorLevel")
			{
				value += ConsumableUtils.GetFlatDamage(base.manager, EquipmentModel.ConsumableType.Unknown, scalePercentage);
				return true;
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { "AbilityModifierScaleDamageByMaxSurvivorLevel" };
		}
	}
}
