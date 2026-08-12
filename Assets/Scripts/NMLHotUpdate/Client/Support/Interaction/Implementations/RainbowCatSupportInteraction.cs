using TWDModel;

namespace Client.Support.Interaction.Implementations
{
	public class RainbowCatSupportInteraction : SimpleSupportInteraction
	{
		public override SupportTargetsMessage NotExecutableMessage => SupportTargetsMessage.NoTargets;

		public RainbowCatSupportInteraction(int equipIndex, SurvivorModel attachedSurvivor)
			: base(equipIndex, attachedSurvivor)
		{
		}
	}
}
