namespace TWDModel
{
	public class EmitAreasManager : CombatAreasManager
	{
		public FixedPoint MaxRadius;

		protected override CombatAreaType CombatAreaType => CombatAreaType.Emitter;

		protected override Faction ExpirationCheckFactionTurn => Faction.Walker;

		public EmitAreasManager()
		{
		}

		public EmitAreasManager(FixedPoint maxRadius)
		{
			MaxRadius = maxRadius;
		}

		protected override void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			if (!actor.IsDead)
			{
				foreach (object model in base.manager.CombatModel.Models)
				{
					if (model is EmitArea emitArea && emitArea.Faction == actor.Faction && emitArea.IsInArea(coord))
					{
						actor.AddTemporaryTrait("EmitterDamageActive", emitArea.Multiplier - 100.0, null, 0L);
						return;
					}
				}
			}
			actor.RemoveTrait("EmitterDamageActive");
		}

		protected override bool ShouldAdd(CombatArea newArea)
		{
			CombatModel combatModel = base.manager.CombatModel;
			foreach (object model in combatModel.Models)
			{
				if (!(model is EmitArea emitArea) || emitArea.Faction != newArea.Faction)
				{
					continue;
				}
				if (emitArea.Coordinate == newArea.Coordinate && emitArea.Radius == newArea.Radius)
				{
					emitArea.ExpiryTurn = newArea.ExpiryTurn;
					return false;
				}
				if (emitArea.Coordinate.SquaredDistanceTo(newArea.Coordinate) < (emitArea.Radius + newArea.Radius) * (emitArea.Radius + newArea.Radius))
				{
					FixedPoint fixedPoint = emitArea.Coordinate.DistanceTo(newArea.Coordinate);
					FixedPoint fixedPoint2 = fixedPoint * 0.5 + FixedPoint.Max(emitArea.Radius, newArea.Radius);
					if (fixedPoint2 <= MaxRadius)
					{
						GridCoordinate coordinate = emitArea.Coordinate + newArea.Coordinate;
						coordinate.X /= 2;
						coordinate.Y /= 2;
						newArea.Coordinate = coordinate;
						newArea.Radius = fixedPoint2;
						combatModel.RemoveModel(emitArea);
						return ShouldAdd(newArea);
					}
					if (fixedPoint < emitArea.Radius - newArea.Radius)
					{
						emitArea.ExpiryTurn = newArea.ExpiryTurn;
						return false;
					}
				}
			}
			return true;
		}
	}
}
