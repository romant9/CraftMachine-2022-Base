using System.Collections.Generic;
using TWDModel;

public static class CommandSkillGridHelpers
{
	public static GridField<FixedPoint> CreatePlayableMapDistanceField(CombatModel combat)
	{
		if (combat == null)
		{
			return null;
		}
		List<GridCoordinate> list = new List<GridCoordinate>();
		List<ActorModel> allActors = combat.GetAllActors();
		for (int i = 0; i < allActors.Count; i++)
		{
			ActorModel actorModel = allActors[i];
			if (!actorModel.IsDead && !actorModel.IsEnvironmental)
			{
				GridCoordinate gridCoordinate = actorModel.GridCoordinate;
				if (combat.Grid.IsCoordinateValid(gridCoordinate) && !combat.IsBlocked(gridCoordinate))
				{
					list.Add(gridCoordinate);
				}
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		return DistanceField.CreateDistanceField(combat, list, new DistanceFieldOptions(1.5f));
	}

	public static bool IsGridCellOnPlayableMap(CombatModel combat, GridCoordinate target, GridField<FixedPoint> playableField = null)
	{
		if (combat == null || !combat.Grid.IsCoordinateValid(target) || combat.IsBlocked(target))
		{
			return false;
		}
		if (combat.GetOccupier(target) != null)
		{
			return true;
		}
		if (playableField == null)
		{
			playableField = CreatePlayableMapDistanceField(combat);
		}
		if (playableField == null)
		{
			return false;
		}
		return playableField[target] < DistanceField.DistanceNotSet;
	}

	public static GridCoordinate GetSourceCell(BaseCommandSkill skill, ActorModel activeActor)
	{
		return (skill?.OwnActorModel ?? activeActor)?.GridCoordinate ?? GridCoordinate.Invalid;
	}

	public static bool IsGridCellVisibleFrom(CombatModel combat, GridCoordinate from, GridCoordinate to)
	{
		if (combat == null || !from.IsValid || !to.IsValid)
		{
			return false;
		}
		return combat.IsGridCellVisible(from, to);
	}
}
