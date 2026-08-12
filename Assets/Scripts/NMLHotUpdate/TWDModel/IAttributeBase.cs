namespace TWDModel
{
	public interface IAttributeBase
	{
		[AttributeType(AttributeType.Hp)]
		FixedPoint GetHP();

		[AttributeType(AttributeType.Attack)]
		FixedPoint GetAttack();
	}
}
