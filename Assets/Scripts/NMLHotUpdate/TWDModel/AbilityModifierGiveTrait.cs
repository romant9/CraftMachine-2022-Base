namespace TWDModel
{
	public class AbilityModifierGiveTrait : ParameterModifier
	{
		private string traitIdentifier;

		private FixedPoint chance;

		public static string RollForTrait = "RollForTrait";

		public AbilityModifierGiveTrait()
		{
		}

		public AbilityModifierGiveTrait(string identifier, FixedPoint traitChance)
		{
			traitIdentifier = identifier;
			chance = traitChance;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == RollForTrait)
			{
				int damage = (int)value;
				if (base.manager.GameEconomyData.GetTraitDefinition(traitIdentifier) != null)
				{
					if (actor.AttributeModel?.GetAttributeModelValue("burn_be_ratio") != 0L)
					{
						FixedPoint value2 = chance;
						FixedPoint value3 = 1L;
						FixedPoint? obj = actor.AttributeModel?.GetAttributeModelValue("burn_be_ratio");
						FixedPoint? fixedPoint = value3 + obj;
						chance = (value2 * fixedPoint).Value;
					}
					if (base.manager.Player.RollDice(RollDiceType.GainTrait, chance, 0.0) != PlayerRandomChanceResult.Failed)
					{
						if (traitIdentifier == "Burning")
						{
							base.manager.ExecuteAction(new BurningOutAction(null, actor, onRedHealthBar: false, null, () => damage));
						}
						else
						{
							actor.AddTrait(traitIdentifier);
						}
					}
					return true;
				}
				base.manager.Debug.LogWarning("AbilityModifierGiveTrait: Tried to give a trait '" + traitIdentifier + "', but could not find TraitDefinition for it!");
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { RollForTrait };
		}
	}
}
