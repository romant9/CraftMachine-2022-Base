using TWDModel;

public class WhisperersMaskSupportInteraction : SupportInteractionBase
{
	private readonly SupportModel support;

	public override FixedPoint? MaxRange => support.GetParameter(2);

	public override bool Targeted => true;

	public override FixedPoint? AreaRadius => support.GetParameter(1);

	public WhisperersMaskSupportInteraction(int equipIndex, SurvivorModel attachedSurvivor, SupportModel supportModel)
		: base(equipIndex, attachedSurvivor)
	{
		support = supportModel;
	}
}
