using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class MagazineAreasManager : CombatAreasManager
	{
		[JsonIgnore]
		public List<MagazineArea> ExistedMagazineAreas => base.manager.CombatModel.Models.OfType<MagazineArea>().ToList();

		protected override CombatAreaType CombatAreaType => CombatAreaType.Magazine;

		protected override Faction ExpirationCheckFactionTurn => Faction.Survivor;

		public override void SetManager(ModelManager mgr)
		{
			base.SetManager(mgr);
			if (mgr is TWDModelManager tWDModelManager)
			{
				tWDModelManager.ActionExecuted += MagazinePickupAfterMoveExecuted;
			}
		}

		public override void Destroy()
		{
			TWDModelManager tWDModelManager = base.manager;
			if (tWDModelManager != null)
			{
				tWDModelManager.ActionExecuted -= MagazinePickupAfterMoveExecuted;
			}
			base.Destroy();
		}

		private void MagazinePickupAfterMoveExecuted(ModelAction action)
		{
			if (action is PushActorAction pushActorAction)
			{
				TryPickupMagazine(pushActorAction.Actor, pushActorAction.Path.End);
			}
		}

		protected override void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			_ = actor.IsDead;
		}

		public bool TryPickupMagazine(ActorModel actor, GridCoordinate coord)
		{
			foreach (MagazineArea model in base.manager.CombatModel.GetModels<MagazineArea>())
			{
				if (!(model.Coordinate != coord) && model.Faction == actor.Faction && !string.IsNullOrEmpty(model.RequiredTraitIdentifier) && actor.HasTraitsThatContains(model.RequiredTraitIdentifier))
				{
					base.manager.ExecuteAction(new PickupMagazineAction(actor, model));
					return true;
				}
			}
			return false;
		}

		protected override bool ShouldAdd(CombatArea newArea)
		{
			foreach (object model in base.manager.CombatModel.Models)
			{
				if (model is MagazineArea magazineArea && magazineArea.Coordinate == newArea.Coordinate)
				{
					return false;
				}
			}
			return true;
		}

		public static int GetMagazineCountByFaction(CombatModel combatModel, Faction faction)
		{
			int num = 0;
			foreach (MagazineArea model in combatModel.GetModels<MagazineArea>())
			{
				if (model.Faction == faction)
				{
					num++;
				}
			}
			return num;
		}

		public static bool IsGridEmpty(CombatModel combatModel, GridCoordinate coord, ActorModel ignoreActor = null)
		{
			ActorModel occupier = combatModel.GetOccupier(coord);
			if (occupier != null && occupier != ignoreActor)
			{
				return false;
			}
			if (combatModel.IsBlocked(coord))
			{
				return false;
			}
			foreach (MagazineArea model in combatModel.GetModels<MagazineArea>())
			{
				if (model.Coordinate == coord)
				{
					return false;
				}
			}
			return true;
		}

		public static List<GridCoordinate> GetEmptyGridsAround(CombatModel combatModel, GridCoordinate center, int radius, ActorModel ignoreActor = null, bool requirePathReachableFromIgnoreActor = false)
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			GridModel grid = combatModel.Grid;
			GridField<FixedPoint> gridField = null;
			if (requirePathReachableFromIgnoreActor && ignoreActor != null)
			{
				gridField = DistanceField.CreateDistanceField(combatModel, center, new DistanceFieldOptions(1.5f, ignoreActor, ignoreActor));
			}
			for (int i = center.X - radius; i <= center.X + radius; i++)
			{
				for (int j = center.Y - radius; j <= center.Y + radius; j++)
				{
					GridCoordinate gridCoordinate = new GridCoordinate(i, j);
					if (!(gridCoordinate == center) && grid.IsCoordinateValid(gridCoordinate) && IsGridEmpty(combatModel, gridCoordinate, ignoreActor) && (gridField == null || !(gridField[gridCoordinate] >= DistanceField.DistanceNotSet)))
					{
						list.Add(gridCoordinate);
					}
				}
			}
			return list;
		}

		protected override void PostOnFactionChangedRemoved(CombatModel combatModel)
		{
			base.PostOnFactionChangedRemoved(combatModel);
			combatModel.NotifyChange("MagazineAreasUpdate");
		}
	}
}
