using TWDModel;

namespace Client.Support.Interaction.Implementations
{
	public class HwachaSupportInteraction : SupportInteractionBase
	{
		private readonly SupportModel support;

		public override FixedPoint? MaxRange => 1000L;

		public override bool Targeted => true;

		public override FixedPoint? AreaRadius => support.GetParameter(4);

		public override FixedPoint MinRange => support.GetParameter(3);

		public HwachaSupportInteraction(int equipIndex, SurvivorModel attachedSurvivor, SupportModel supportModel)
			: base(equipIndex, attachedSurvivor)
		{
			support = supportModel;
		}

		public override FixedPoint? GetPreviewAreaRadius(GridCoordinate target)
		{
			FixedPoint? areaRadius = AreaRadius;
			if (!areaRadius.HasValue)
			{
				return null;
			}
			AbilityManagerModel abilityManagerModel = base.AttachedSurvivor?.manager?.CombatModel?.AbilityManager;
			if (abilityManagerModel == null)
			{
				return areaRadius;
			}
			return abilityManagerModel.GetDamageAreaBlockEffectiveSupportRadius(target, (int)areaRadius.Value);
		}
	}
}
