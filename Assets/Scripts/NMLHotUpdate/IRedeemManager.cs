using TWDModel;

public interface IRedeemManager
{
	RedeemValidity RedeemCode(string code, out IRedeemDefinition redeemDefinition);
}
