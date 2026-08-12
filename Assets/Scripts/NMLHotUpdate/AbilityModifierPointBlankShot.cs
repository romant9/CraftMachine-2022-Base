using TWDModel;

public class AbilityModifierPointBlankShot : ParameterModifier
{
	private FixedPoint multiplier;

	public static string FetchIncreasePercentageRanagedDamageWithPointBlankShot = "FetchIncreasePercentageRanagedDamageWithPointBlankShot";

	public AbilityModifierPointBlankShot(FixedPoint damagePercentageMultiplier)
	{
		multiplier = damagePercentageMultiplier;
	}

	public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
	{
		if (paramName == FetchIncreasePercentageRanagedDamageWithPointBlankShot)
		{
			value += multiplier;
			return true;
		}
		return false;
	}

	public override string[] GetParameterNames()
	{
		return new string[1] { FetchIncreasePercentageRanagedDamageWithPointBlankShot };
	}
}
