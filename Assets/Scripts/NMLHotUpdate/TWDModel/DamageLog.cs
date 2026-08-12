namespace TWDModel
{
	public class DamageLog
	{
		public FixedPoint BaseDamage;

		public FixedPoint AfterDamageVariation;

		public FixedPoint AfterTypeModification;

		public FixedPoint AfterBodyShot;

		public FixedPoint AfterCritical;

		public FixedPoint AfterFinalDamage;

		public FixedPoint AfterDamageReduction;

		public int[] ResultDamage;

		public DamageType DamageType;

		public FixedPoint DamageTypeMultiplier;

		public FixedPoint AdditionalTypeDamage;

		public PlayerRandomChanceResult BodyShotResult;

		public FixedPoint BodyShotMultiplier;

		public PlayerRandomChanceResult CriticalResult;

		public FixedPoint CriticalMultiplier;

		public FixedPoint FinalDamageMultiplier;

		public FixedPoint AmountDamageReduced;

		public FixedPoint DefenseWithCoverMultiplier;

		public override string ToString()
		{
			FixedPoint baseDamage = BaseDamage;
			string text = "Base() " + baseDamage.ToString() + "\n";
			string text2 = text;
			baseDamage = AfterDamageVariation;
			text = text2 + "\t -> Variation() " + baseDamage.ToString() + "\n";
			string[] obj = new string[10]
			{
				text,
				"\t -> Type(",
				DamageType.ToString(),
				", ",
				null,
				null,
				null,
				null,
				null,
				null
			};
			baseDamage = DamageTypeMultiplier;
			obj[4] = baseDamage.ToString();
			obj[5] = ", ";
			baseDamage = AdditionalTypeDamage;
			obj[6] = baseDamage.ToString();
			obj[7] = ") ";
			baseDamage = AfterTypeModification;
			obj[8] = baseDamage.ToString();
			obj[9] = "\n";
			text = string.Concat(obj);
			string[] obj2 = new string[8]
			{
				text,
				"\t -> BodyShot(",
				BodyShotResult.ToString(),
				", ",
				null,
				null,
				null,
				null
			};
			baseDamage = BodyShotMultiplier;
			obj2[4] = baseDamage.ToString();
			obj2[5] = ") ";
			baseDamage = AfterBodyShot;
			obj2[6] = baseDamage.ToString();
			obj2[7] = "\n";
			text = string.Concat(obj2);
			string[] obj3 = new string[8]
			{
				text,
				"\t -> Critical(",
				CriticalResult.ToString(),
				", ",
				null,
				null,
				null,
				null
			};
			baseDamage = CriticalMultiplier;
			obj3[4] = baseDamage.ToString();
			obj3[5] = ") ";
			baseDamage = AfterCritical;
			obj3[6] = baseDamage.ToString();
			obj3[7] = "\n";
			text = string.Concat(obj3);
			string[] obj4 = new string[6] { text, "\t -> Final(", null, null, null, null };
			baseDamage = FinalDamageMultiplier;
			obj4[2] = baseDamage.ToString();
			obj4[3] = ") ";
			baseDamage = AfterFinalDamage;
			obj4[4] = baseDamage.ToString();
			obj4[5] = "\n";
			text = string.Concat(obj4);
			string text3 = text;
			baseDamage = AmountDamageReduced;
			text = text3 + "\t -> AmountDamageReduced(" + baseDamage.ToString() + ")\n";
			string[] obj5 = new string[6] { text, "\t -> DefenseWithCoverReduction(", null, null, null, null };
			baseDamage = DefenseWithCoverMultiplier;
			obj5[2] = baseDamage.ToString();
			obj5[3] = ") ";
			baseDamage = AfterDamageReduction;
			obj5[4] = baseDamage.ToString();
			obj5[5] = "\n";
			text = string.Concat(obj5);
			if (ResultDamage != null && ResultDamage.Length == 2)
			{
				text = text + "\t -> BaseDamage " + ResultDamage[0] + " AdditionalCriticalDamage " + ResultDamage[1];
			}
			return text;
		}
	}
}
