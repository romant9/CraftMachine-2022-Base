namespace TWDModel
{
	public interface IRedeemDefinition
	{
		Rewards Rewards { get; }

		RedeemValidity CheckValidity(PlayerModel playerModel);
	}
}
