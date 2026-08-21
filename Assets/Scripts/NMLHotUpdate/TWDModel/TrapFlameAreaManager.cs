using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class TrapFlameAreaManager : CombatAreasManager
	{
		public const string TrapFlameInjuryNotification = "TrapFlameInjuryNotification";

		[JsonIgnore]
		public List<TrapFlameArea> ExistedTrapFlameAreas => base.manager.CombatModel.Models.OfType<TrapFlameArea>().ToList();

		protected override CombatAreaType CombatAreaType => CombatAreaType.TrapFlame;

		protected override Faction ExpirationCheckFactionTurn => Faction.Survivor;

		public override void SetManager(ModelManager mgr)
		{
			SetManagerForTrapFlameManager(mgr);
			base.manager.CombatModel.TurnManager.FactionChanged -= base.TurnManagerOnFactionChanged;
			base.manager.CombatModel.TurnManager.FactionChanged += base.TurnManagerOnFactionChanged;
			base.manager.CombatModel.Changed -= base.CombatModelOnChanged;
			base.manager.CombatModel.Changed += base.CombatModelOnChanged;
			base.manager.PreActionExecution -= base.ManagerOnActionExecuted;
			base.manager.PreActionExecution += base.ManagerOnActionExecuted;
		}

		public void UpdateWhenNewAreaGenerated(List<TrapFlameArea> newTrapFlameAreas)
		{
			if (ExistedTrapFlameAreas.Count > 0)
			{
				foreach (TrapFlameArea existedTrapFlameArea in ExistedTrapFlameAreas)
				{
					foreach (TrapFlameArea newTrapFlameArea in newTrapFlameAreas)
					{
						if (existedTrapFlameArea.EffectiveAreaGridCoordinate == newTrapFlameArea.EffectiveAreaGridCoordinate)
						{
							base.manager.CombatModel.RemoveModel(existedTrapFlameArea);
						}
					}
				}
			}
			foreach (TrapFlameArea newTrapFlameArea2 in newTrapFlameAreas)
			{
				AddArea(newTrapFlameArea2);
			}
		}

		public static List<TrapFlameArea> GetTrapFlameAreasFromGridCoordinates(ActorModel owenActor, GridCoordinate targetCell, int expiryTurn, List<GridCoordinate> gridCoordinates, FixedPoint inTrapFlameInjuryHPPercentage)
		{
			List<TrapFlameArea> list = new List<TrapFlameArea>();
			foreach (GridCoordinate gridCoordinate in gridCoordinates)
			{
				TrapFlameArea trapFlameArea = new TrapFlameArea(owenActor, targetCell, owenActor.Faction, expiryTurn, gridCoordinate, inTrapFlameInjuryHPPercentage);
				trapFlameArea.SetManager(owenActor.manager);
				list.Add(trapFlameArea);
			}
			return list;
		}

		protected override void Tick(IEnumerable<ActorModel> actorModels)
		{
			if (actorModels == null)
			{
				return;
			}
			foreach (ActorModel item in new List<ActorModel>(actorModels))
			{
				if (item.IsDead)
				{
					continue;
				}
				foreach (TrapFlameArea existedTrapFlameArea in ExistedTrapFlameAreas)
				{
					if (existedTrapFlameArea.Faction != item.Faction && (item.IsMultiCell ? item.GetOccupiedCells().Contains(existedTrapFlameArea.EffectiveAreaGridCoordinate) : (existedTrapFlameArea.EffectiveAreaGridCoordinate == item.GridCoordinate)))
					{
						ApplyDmg(existedTrapFlameArea.Owner, item, existedTrapFlameArea.InjuryHPPercent);
						break;
					}
				}
			}
		}

		protected void ApplyDmg(ActorModel source, ActorModel target, FixedPoint injuryHPPercent)
		{
			target.NotifyChange("AbilityVisited", new object[2] { "TrapFlame", false });
			FixedPoint fixedPoint = target.MaxHitPoints * injuryHPPercent;
			CombatHelpers.ExecuteDamage(base.manager.CombatModel, null, target, (int)fixedPoint, 0, DamageType.TrapFlame, PlayerRandomChanceResult.Failed, PlayerRandomChanceResult.Failed);
		}

		protected override void RefreshActorAreaStateInternal(ActorModel actor, GridCoordinate coord, ModelAction actorAction = null)
		{
			if (!actor.IsDead && actorAction is MoveAction moveAction && actor.Faction != Faction.Survivor)
			{
				GridCoordinate resultTrapFlameEndGridCoordinate = GridCoordinate.Invalid;
				GridPath originGridPath = GridPath.Create(moveAction.Path);
				if (RecalculatePathEndGridCoordinateBecauseOfTrapFlameArea(base.manager.CombatModel, actor, originGridPath, ref resultTrapFlameEndGridCoordinate, out var _))
				{
					moveAction.Path.ClipTo(resultTrapFlameEndGridCoordinate);
				}
			}
		}

		public bool RecalculatePathEndGridCoordinateBecauseOfTrapFlameArea(CombatModel combatModel, ActorModel actorModel, GridPath originGridPath, ref GridCoordinate resultTrapFlameEndGridCoordinate, out int reducePathCount)
		{
			reducePathCount = 0;
			if (ExistedTrapFlameAreas.Count == 0)
			{
				return false;
			}
			if (originGridPath.Path == null)
			{
				return false;
			}
			if (originGridPath.Path.Count <= 2)
			{
				return false;
			}
			int count = originGridPath.Path.Count;
			int num = 0;
			bool result = false;
			for (int num2 = originGridPath.Path.Count - 2; num2 > 0; num2--)
			{
				GridCoordinate currentGridCoordinate = originGridPath.Path[num2];
				if (ExistedTrapFlameAreas.Exists((TrapFlameArea x) => x.EffectiveAreaGridCoordinate == currentGridCoordinate && x.Faction != actorModel.Faction))
				{
					num++;
				}
			}
			if (num > 0 && ResistNegativeEffectsTrait.TryResist(actorModel, "TrapFlame"))
			{
				return false;
			}
			if (actorModel.Faction == Faction.Survivor)
			{
				int num3 = CombatHelpers.GetMoveRange(actorModel) - num;
				if (num > 0)
				{
					while ((originGridPath.MoveDistance > num3 || CombatHelpers.IsOccupiedOrBlocked(combatModel, originGridPath.End, actorModel)) && !(originGridPath.MoveDistance <= 0L))
					{
						originGridPath.RemoveLast();
						reducePathCount++;
						resultTrapFlameEndGridCoordinate = originGridPath.End;
						result = true;
					}
				}
			}
			else
			{
				if (num > 0)
				{
					for (int num4 = 0; num4 < num; num4++)
					{
						originGridPath.RemoveLast();
						reducePathCount++;
						resultTrapFlameEndGridCoordinate = originGridPath.End;
						result = true;
					}
				}
				int num5 = originGridPath.Path.Count - 1;
				while (num5 >= 0 && CombatHelpers.IsOccupiedOrBlocked(base.manager.CombatModel, originGridPath.Path[num5], actorModel))
				{
					originGridPath.RemoveLast();
					reducePathCount++;
					resultTrapFlameEndGridCoordinate = originGridPath.End;
					result = true;
					num5--;
				}
			}
			if (resultTrapFlameEndGridCoordinate == GridCoordinate.Invalid)
			{
				return false;
			}
			reducePathCount = UtilsMath.Clamp(reducePathCount, 0, count);
			return result;
		}

		protected override bool ShouldAdd(CombatArea newArea)
		{
			return true;
		}
	}
}
