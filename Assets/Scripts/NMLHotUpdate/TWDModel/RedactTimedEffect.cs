namespace TWDModel
{
	public class RedactTimedEffect : TWDModelObject
	{
		public int Layers { get; set; }

		public int IncreaseDamageRatio { get; set; }

		public FixedPoint ReduceHpChance { get; set; }

		public int ReducedHpRatio { get; set; }

		public TraitDefinition GetTrait(CombatModel combatModel)
		{
			if (combatModel?.Survivors == null || combatModel?.gameEconomyData == null)
			{
				return null;
			}
			for (int i = 0; i < combatModel.Survivors.Count; i++)
			{
				SurvivorModel survivorModel = (SurvivorModel)combatModel.Survivors[i];
				if (survivorModel == null)
				{
					continue;
				}
				for (int j = 0; j < survivorModel.UpgradeTraits.Count; j++)
				{
					UpgradeTraitsData upgradeTraitsData = survivorModel.UpgradeTraits[j];
					if (upgradeTraitsData != null && upgradeTraitsData.Identifier.Contains("LeaderBuffRedact"))
					{
						return combatModel.gameEconomyData.GetTraitDefinition(upgradeTraitsData.Identifier);
					}
				}
			}
			return null;
		}

		public RedactTimedEffect(int increaseDamageRatio, FixedPoint reduceHpChance, int reducedHpRatio)
		{
			Layers = 1;
			IncreaseDamageRatio = increaseDamageRatio;
			ReduceHpChance = reduceHpChance;
			ReducedHpRatio = reducedHpRatio;
		}

		public override bool IsValid()
		{
			return Layers > 0;
		}
	}
}
