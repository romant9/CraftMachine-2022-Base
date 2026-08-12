public class RewardGuildBattleWithMultiplier : RewardGuildBattleVP
{
	private int baseAmount;

	private float multiplier;

	public int BaseAmount
	{
		get
		{
			return baseAmount;
		}
		set
		{
			baseAmount = value;
			RecalculateAmount();
		}
	}

	public float Multiplier
	{
		get
		{
			return multiplier;
		}
		set
		{
			multiplier = value;
			RecalculateAmount();
		}
	}

	private void RecalculateAmount()
	{
		base.Amount = (int)((float)BaseAmount * Multiplier);
	}
}
