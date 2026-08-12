using System.Collections.Generic;
using TWDModel;

public abstract class SupportInteractionBase : ISupportInteraction
{
	public abstract FixedPoint? MaxRange { get; }

	public virtual FixedPoint MinRange => 0L;

	public abstract bool Targeted { get; }

	public abstract FixedPoint? AreaRadius { get; }

	public int EquipIndex { get; }

	public SurvivorModel AttachedSurvivor { get; }

	public virtual SupportTargetsMessage NotExecutableMessage => SupportTargetsMessage.NoTargetsInRange;

	protected SupportInteractionBase(int equipIndex, SurvivorModel attachedSurvivor)
	{
		EquipIndex = equipIndex;
		AttachedSurvivor = attachedSurvivor;
	}

	public virtual IEnumerable<ActorModel> GetTargets(GridCoordinate target)
	{
		if (AttachedSurvivor.manager.CombatModel.SupportManager.TryGetSupport(EquipIndex, out var combatSupportModel))
		{
			return combatSupportModel.GetTargets(target);
		}
		return null;
	}
}
