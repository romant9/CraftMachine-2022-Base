using TWDModel;

public class SimpleSupportInteraction : SupportInteractionBase
{
	public override FixedPoint? MaxRange => null;

	public override bool Targeted => false;

	public override FixedPoint? AreaRadius => null;

	public SimpleSupportInteraction(int equipIndex, SurvivorModel attachedSurvivor)
		: base(equipIndex, attachedSurvivor)
	{
	}
}
