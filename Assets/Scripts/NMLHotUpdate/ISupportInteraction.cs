using System.Collections.Generic;
using TWDModel;

public interface ISupportInteraction
{
	FixedPoint MinRange { get; }

	FixedPoint? MaxRange { get; }

	bool Targeted { get; }

	FixedPoint? AreaRadius { get; }

	int EquipIndex { get; }

	SurvivorModel AttachedSurvivor { get; }

	SupportTargetsMessage NotExecutableMessage { get; }

	IEnumerable<ActorModel> GetTargets(GridCoordinate target);

	FixedPoint? GetPreviewAreaRadius(GridCoordinate target);
}
