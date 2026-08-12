namespace TWDModel
{
	public interface IAttackTargetModel
	{
		bool IsDisabledOnGED { get; }

		int AttackTargetId { get; }
	}
}
