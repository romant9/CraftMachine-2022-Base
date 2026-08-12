using System.Collections.Generic;

namespace TWDModel
{
	public class AIBehaviorHelpers
	{
		public static ActorModel GetLureTarget(ActorModel actor, CombatModel combatModel)
		{
			foreach (ActorModel factionActor in combatModel.GetFactionActors(Faction.Lure))
			{
				if (IsTargetInActivationRange(actor, combatModel, factionActor) && CanSeeTarget(actor, combatModel, factionActor))
				{
					return factionActor;
				}
			}
			return null;
		}

		public static ActorModel GetPvPAttackTarget(ActorModel actor, CombatModel combatModel, ActorModel previousTarget, bool closest, Faction targetFaction = Faction.Any)
		{
			ActorModel actorModel = null;
			List<ActorModel> list = new List<ActorModel>();
			list = ((!actor.MoveCompleted) ? GetAttackTargetsInActivationRange(actor, combatModel, targetFaction) : GetAttackTargetsInAttackRange(actor, combatModel, targetFaction));
			List<ActorModel> list2 = new List<ActorModel>();
			for (int i = 0; i < list.Count; i++)
			{
				ActorModel actorModel2 = list[i];
				if (CanSeeTarget(actor, combatModel, actorModel2) && !actorModel2.IsDead && (!actor.IsWalker || (actorModel2.Faction != Faction.Raider && actorModel2.Faction != Faction.Environmental)) && (!actor.IsWalker || (!actorModel2.IsStruggling && !actorModel2.IsBleedingOut && !actorModel2.IsInvisible && !actorModel2.IsCamouflaged)))
				{
					list2.Add(actorModel2);
				}
			}
			if (list2.Count > 0)
			{
				if (actor.SelectedAbility != null && actor.SelectedAbility.Definition.MaxAffectedTargetsCount > 1)
				{
					actorModel = PickTargetWithMostTargetsHit(actor, combatModel, list2);
					if (actorModel == null)
					{
						actorModel = PickClosestTarget(actor, combatModel, list2);
					}
				}
				else
				{
					actorModel = (closest ? PickClosestTarget(actor, combatModel, list2) : PickClosestTargetPreferOld(actor, combatModel, previousTarget, list2));
				}
			}
			return actorModel;
		}

		public static ActorModel GetAttackTarget(ActorModel actor, CombatModel combatModel, ActorModel previousTarget, bool closest, Faction targetFaction = Faction.Any)
		{
			if (actor.IsDisoriented)
			{
				return GetDisorientAttackTarget(actor, combatModel);
			}
			if (actor.IsABTesterA2ed)
			{
				List<ActorModel> oneGridWalkRaiderModels = combatModel.GetOneGridWalkRaiderModels(actor);
				if (oneGridWalkRaiderModels != null && oneGridWalkRaiderModels.Count > 0)
				{
					for (int i = 0; i < oneGridWalkRaiderModels.Count; i++)
					{
						if (oneGridWalkRaiderModels[i].IsABTesterAed && !oneGridWalkRaiderModels[i].IsDead)
						{
							return oneGridWalkRaiderModels[i];
						}
					}
				}
				actor.EndAction();
				return null;
			}
			if (actor.IsTaunted && actor.TauntTimedEffect.Instigator != null)
			{
				return actor.TauntTimedEffect.Instigator;
			}
			ActorModel actorModel = null;
			List<ActorModel> list = new List<ActorModel>();
			list = ((!actor.MoveCompleted) ? GetAttackTargetsInActivationRange(actor, combatModel, targetFaction) : GetAttackTargetsInAttackRange(actor, combatModel, targetFaction));
			List<ActorModel> list2 = new List<ActorModel>();
			for (int j = 0; j < list.Count; j++)
			{
				ActorModel actorModel2 = list[j];
				if (CanSeeTarget(actor, combatModel, actorModel2) && !actorModel2.IsStruggling && !actorModel2.IsBleedingOut && !actorModel2.IsDead && (!actorModel2.IsInvisible || actor.Faction == Faction.Raider) && (!actorModel2.IsCamouflaged || actor.Faction == Faction.Raider) && (!actorModel2.IsSneak || actor.Faction != Faction.Raider))
				{
					list2.Add(actorModel2);
				}
			}
			if (list2.Count > 0)
			{
				if (actor.SelectedAbility.Definition.MaxAffectedTargetsCount > 1)
				{
					actorModel = PickTargetWithMostTargetsHit(actor, combatModel, list2);
					if (actorModel == null)
					{
						actorModel = PickClosestTarget(actor, combatModel, list2);
					}
				}
				else
				{
					actorModel = (closest ? PickClosestTarget(actor, combatModel, list2) : PickClosestTargetPreferOld(actor, combatModel, previousTarget, list2));
				}
			}
			return actorModel;
		}

		public static List<ActorModel> GetAttackTargetsInAttackRange(ActorModel actor, CombatModel combatModel, Faction targetFaction = Faction.Any)
		{
			List<ActorModel> list = new List<ActorModel>();
			List<ActorModel> list2 = null;
			list2 = ((targetFaction != Faction.Any) ? combatModel.GetFactionActors(targetFaction) : combatModel.GetEnemyFactionsActors(actor.Faction));
			for (int i = 0; i < list2.Count; i++)
			{
				ActorModel actorModel = list2[i];
				if (actor.SelectedAbility.CanAbilityBeTargetedOnGridCell(combatModel, actor, actor.GridCoordinate, actorModel.GridCoordinate))
				{
					list.Add(actorModel);
				}
			}
			return list;
		}

		public static List<ActorModel> GetAttackTargetsInActivationRange(ActorModel actor, CombatModel combatModel, Faction targetFaction = Faction.Any)
		{
			List<ActorModel> list = new List<ActorModel>();
			List<ActorModel> list2 = null;
			list2 = ((targetFaction != Faction.Any) ? combatModel.GetFactionActors(targetFaction) : combatModel.GetEnemyFactionsActors(actor.Faction));
			for (int i = 0; i < list2.Count; i++)
			{
				ActorModel actorModel = list2[i];
				if (IsTargetInActivationRange(actor, combatModel, actorModel))
				{
					list.Add(actorModel);
				}
			}
			return list;
		}

		public static GridCoordinate GetRetreatTarget(ActorModel actor, CombatModel combat)
		{
			return GridCoordinate.Invalid;
		}

		public static ActorModel PickRandomTarget(CombatModel combatModel, List<ActorModel> targetList)
		{
			return combatModel.manager.Player.PlayerRandom.GetRandomElement(targetList, remove: false);
		}

		public static ActorModel PickClosestTarget(ActorModel actor, CombatModel combatModel, List<ActorModel> targetList)
		{
			ActorModel result = null;
			int num = int.MaxValue;
			foreach (ActorModel target in targetList)
			{
				if ((combatModel.HasPvPRules && actor.Faction != Faction.Walker) || (!target.IsStruggling && !target.IsBleedingOut && (!target.IsInvisible || actor.Faction != Faction.Walker) && (!target.IsCamouflaged || actor.Faction != Faction.Walker)))
				{
					int num2 = actor.GridCoordinate.SquaredDistanceTo(target.GridCoordinate);
					if (num2 < num)
					{
						num = num2;
						result = target;
					}
				}
			}
			return result;
		}

		public static ActorModel PickFarthestTarget(ActorModel actor, CombatModel combatModel, List<ActorModel> targetList)
		{
			ActorModel result = null;
			int num = int.MinValue;
			foreach (ActorModel target in targetList)
			{
				if ((combatModel.HasPvPRules && actor.Faction != Faction.Walker) || (!target.IsStruggling && !target.IsBleedingOut && (!target.IsInvisible || actor.Faction != Faction.Walker) && (!target.IsCamouflaged || actor.Faction != Faction.Walker)))
				{
					int num2 = actor.GridCoordinate.SquaredDistanceTo(target.GridCoordinate);
					if (num2 > num)
					{
						num = num2;
						result = target;
					}
				}
			}
			return result;
		}

		public static ActorModel PickClosestTargetPreferOld(ActorModel actor, CombatModel combatModel, ActorModel previousTarget, List<ActorModel> targetList)
		{
			ActorModel actorModel = null;
			int num = int.MaxValue;
			foreach (ActorModel target in targetList)
			{
				if ((combatModel.HasPvPRules && actor.Faction != Faction.Walker) || (!target.IsStruggling && !target.IsBleedingOut))
				{
					int num2 = actor.GridCoordinate.SquaredDistanceTo(target.GridCoordinate);
					if (num2 < num)
					{
						num = num2;
						actorModel = target;
					}
				}
			}
			if (previousTarget != null && previousTarget != actorModel && !previousTarget.IsStruggling && !previousTarget.IsBleedingOut)
			{
				int num3 = 1;
				int num4 = actor.GridCoordinate.SquaredDistanceTo(previousTarget.GridCoordinate);
				int num5 = actor.GridCoordinate.SquaredDistanceTo(actorModel.GridCoordinate);
				if (num4 - num3 * num3 <= num5)
				{
					actorModel = previousTarget;
				}
			}
			return actorModel;
		}

		public static ActorModel PickMinHealthTarget(CombatModel combatModel, List<ActorModel> targetList)
		{
			ActorModel result = null;
			int num = int.MaxValue;
			foreach (ActorModel target in targetList)
			{
				if (!target.IsStruggling && !target.IsBleedingOut)
				{
					int hitpoints = target.Hitpoints;
					if (hitpoints < num)
					{
						num = hitpoints;
						result = target;
					}
				}
			}
			return result;
		}

		public static ActorModel PickMaxHealthTarget(CombatModel combatModel, List<ActorModel> targetList)
		{
			ActorModel result = null;
			int num = int.MinValue;
			foreach (ActorModel target in targetList)
			{
				if (!target.IsStruggling && !target.IsBleedingOut)
				{
					int hitpoints = target.Hitpoints;
					if (hitpoints > num)
					{
						num = hitpoints;
						result = target;
					}
				}
			}
			return result;
		}

		public static ActorModel PickTargetWithMostTargetsHit(ActorModel actor, CombatModel combatModel, List<ActorModel> targetList)
		{
			ActorModel result = null;
			int num = 0;
			int num2 = int.MaxValue;
			for (int i = 0; i < targetList.Count; i++)
			{
				ActorModel actorModel = targetList[i];
				if (actor.SelectedAbility.CanAbilityBeTargetedOnGridCell(combatModel, actor, actor.GridCoordinate, actorModel.GridCoordinate))
				{
					List<ActorModel> listOfActorsToBeTargetted = combatModel.AbilityManager.GetListOfActorsToBeTargetted(actor.SelectedAbility, actor, actor.GridCoordinate, actorModel.GridCoordinate);
					int num3 = actor.GridCoordinate.SquaredDistanceTo(actorModel.GridCoordinate);
					if (listOfActorsToBeTargetted.Count > num || (listOfActorsToBeTargetted.Count == num && listOfActorsToBeTargetted.Count > 0 && num3 < num2))
					{
						num = listOfActorsToBeTargetted.Count;
						num2 = num3;
						result = actorModel;
					}
				}
			}
			return result;
		}

		public static ActorModel GetBuddyAidTarget(ActorModel actor, CombatModel combat)
		{
			GridField<FixedPoint> gridField = DistanceField.CreateDistanceField(combat, actor.GridCoordinate, new DistanceFieldOptions(1.5f, actor, actor));
			ActorModel result = null;
			List<ActorModel> list = new List<ActorModel>();
			List<ActorModel> factionActors = combat.GetFactionActors(actor.Faction);
			for (int i = 0; i < factionActors.Count; i++)
			{
				ActorModel actorModel = factionActors[i];
				if (actorModel != actor && (actorModel.IsStruggling || actorModel.IsBleedingOut) && gridField[actorModel.GridCoordinate] <= actor.MoveRange * (actor.MoveCompleted ? 1 : 2))
				{
					list.Add(actorModel);
				}
			}
			FixedPoint fixedPoint = FixedPoint.MaxValue;
			for (int j = 0; j < list.Count; j++)
			{
				ActorModel actorModel2 = list[j];
				FixedPoint fixedPoint2 = gridField[actorModel2.GridCoordinate];
				if (fixedPoint2 < fixedPoint)
				{
					fixedPoint = fixedPoint2;
					result = actorModel2;
				}
			}
			return result;
		}

		public static bool CanSeeTarget(ActorModel actor, CombatModel combatModel, ActorModel target)
		{
			if (combatModel.Grid.IsCoordinateValid(actor.GridCoordinate) && target != null && target.IsValid())
			{
				return combatModel.IsGridCellVisible(actor.GridCoordinate, target.GridCoordinate);
			}
			return false;
		}

		public static bool IsTargetInActivationRange(ActorModel actor, CombatModel combatModel, ActorModel target)
		{
			if (combatModel.Grid.IsCoordinateValid(actor.GridCoordinate) && target != null && target.IsValid())
			{
				int num = actor.GridCoordinate.SquaredDistanceTo(target.GridCoordinate);
				int num2 = actor.ActivationRange * actor.ActivationRange;
				return num <= num2;
			}
			return false;
		}

		public static bool IsTargetInAttackRange(ActorModel actor, CombatModel combatModel, ActorModel target)
		{
			if (target != null && target.IsValid())
			{
				return IsTargetInAttackRange(actor, combatModel, target.GridCoordinate);
			}
			return false;
		}

		public static bool IsTargetInAttackRange(ActorModel actor, CombatModel combatModel, GridCoordinate c)
		{
			if (combatModel.Grid.IsCoordinateValid(actor.GridCoordinate))
			{
				EquipmentItemModel weaponEquipment = actor.GetWeaponEquipment();
				if (weaponEquipment != null)
				{
					FixedPoint range = weaponEquipment.Ability.Definition.AbilityRange;
					if (!weaponEquipment.Ability.IsConsumableAbility)
					{
						CombatHelpers.CalculateRangeExtension(ref range, actor, combatModel.AbilityManager);
					}
					int num = (weaponEquipment.Ability.Definition.AbilityTargetDiagonal ? ((int)range) : 0);
					GridCoordinate other = new GridCoordinate(actor.GridCoordinate.X + (int)range, actor.GridCoordinate.Y + num);
					int num2 = actor.GridCoordinate.SquaredDistanceTo(c);
					int num3 = actor.GridCoordinate.SquaredDistanceTo(other);
					return num2 <= num3;
				}
				return false;
			}
			return false;
		}

		public static InteractiveObjectModel FindNearestDestroyableObject(ActorModel actor)
		{
			List<TWDModelObject> models = actor.manager.GetModels<InteractiveObjectModel>();
			InteractiveObjectModel result = null;
			int num = int.MaxValue;
			foreach (InteractiveObjectModel item in models)
			{
				if (item.NPCAttacksToDestroy > 0 && !item.Disabled && !item.Completed)
				{
					int num2 = actor.GridCoordinate.SquaredDistanceTo(item.Location.Coordinate);
					if (num2 < num)
					{
						num = num2;
						result = item;
					}
				}
			}
			return result;
		}

		public static GridCoordinate GetFlankingCoordinate(ActorModel actor, CombatModel combatModel, GridCoordinate startingCoordinate)
		{
			GridModel grid = combatModel.Grid;
			GridCoordinate gridCoordinate = startingCoordinate;
			List<GridCoordinate> list = new List<GridCoordinate>();
			foreach (ActorModel factionActor in combatModel.GetFactionActors(actor.Faction))
			{
				if (factionActor != actor)
				{
					list.Add(factionActor.GridCoordinate);
				}
			}
			GridCoordinate gridCoordinate2 = GridCoordinate.Invalid;
			GridField<FixedPoint> gridField = DistanceField.CreateDistanceField(combatModel, list, new DistanceFieldOptions(1f, actor, actor));
			FixedPoint fixedPoint = gridField[actor.GridCoordinate];
			if (fixedPoint < 2L)
			{
				foreach (GridCoordinate item in grid.Neighbors(gridCoordinate))
				{
					if (actor.GridCoordinate != item && grid.AreNeighbors(item, actor.GridCoordinate))
					{
						FixedPoint fixedPoint2 = gridField[item];
						if (fixedPoint2 >= fixedPoint && !combatModel.IsBlocked(item))
						{
							fixedPoint = fixedPoint2;
							gridCoordinate2 = item;
						}
					}
				}
			}
			if (grid.IsCoordinateValid(gridCoordinate2))
			{
				int num = 0;
				int num2 = 0;
				foreach (GridCoordinate item2 in grid.Neighbors(actor.GridCoordinate))
				{
					ActorModel occupier = combatModel.GetOccupier(item2);
					if (occupier != null && occupier.Faction == actor.Faction && occupier.AIController.IsStuck())
					{
						num++;
					}
				}
				foreach (GridCoordinate item3 in grid.Neighbors(gridCoordinate2))
				{
					ActorModel occupier2 = combatModel.GetOccupier(item3);
					if (occupier2 != null && occupier2.Faction == actor.Faction && occupier2 != actor)
					{
						num2++;
					}
				}
				if (num > num2)
				{
					gridCoordinate = gridCoordinate2;
				}
			}
			return gridCoordinate;
		}

		public static GridCoordinate GetRandomMoveCoordinate(ActorModel actor, CombatModel combatModel)
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			foreach (GridCoordinate coordinate in combatModel.Grid.Coordinates)
			{
				if (coordinate.DistanceTo(actor.GridCoordinate) > 2.0 && coordinate.DistanceTo(actor.GridCoordinate) < combatModel.Grid.Height && !combatModel.IsBlocked(coordinate))
				{
					list.Add(coordinate);
				}
			}
			return combatModel.manager.Player.PlayerRandom.GetRandomElement(list, remove: false);
		}

		public static ActorModel GetDisorientAttackTarget(ActorModel actor, CombatModel combatModel)
		{
			if (actor.DisorientLockActor != null)
			{
				if (actor.DisorientLockActor.IsDead)
				{
					return null;
				}
				if (!actor.DisorientLockActor.IsDisoriented)
				{
					return actor.DisorientLockActor;
				}
			}
			List<ActorModel> list = new List<ActorModel>();
			list.Clear();
			list.AddRange(combatModel.Raiders.Models);
			list.AddRange(combatModel.Walkers.Models);
			list.Remove(actor);
			list.RemoveAll((ActorModel t) => combatModel.IsDisorientedModel(t));
			List<ActorModel> list2 = new List<ActorModel>();
			for (int num = 0; num < list.Count; num++)
			{
				ActorModel actorModel = list[num];
				if (IsTargetInActivationRange(actor, combatModel, actorModel) && !list2.Contains(actorModel))
				{
					list2.Add(actorModel);
				}
			}
			for (int num2 = 0; num2 < list.Count; num2++)
			{
				ActorModel actorModel2 = list[num2];
				if (actor.SelectedAbility.CanAbilityBeTargetedOnGridCell(combatModel, actor, actor.GridCoordinate, actorModel2.GridCoordinate) && !list2.Contains(actorModel2))
				{
					list2.Add(actorModel2);
				}
			}
			FixedPoint value = 0.0;
			combatModel.AbilityManager.VisitParameter("DisorientLeastSpaces", ref value);
			for (int num3 = 0; num3 < list.Count; num3++)
			{
				ActorModel actorModel3 = list[num3];
				if (actor.GridCoordinate.ChebyshevDistance(actorModel3.GridCoordinate) <= value && !list2.Contains(actorModel3))
				{
					list2.Add(actorModel3);
				}
			}
			ActorModel actorModel4 = null;
			List<ActorModel> list3 = new List<ActorModel>();
			for (int num4 = 0; num4 < list2.Count; num4++)
			{
				ActorModel actorModel5 = list2[num4];
				if (CanSeeTarget(actor, combatModel, actorModel5) && !actorModel5.IsStruggling && !actorModel5.IsBleedingOut && !actorModel5.IsDead && !actorModel5.IsInvisible)
				{
					list3.Add(actorModel5);
				}
			}
			if (list3.Count > 0)
			{
				if (actor.SelectedAbility.Definition.MaxAffectedTargetsCount > 1)
				{
					actorModel4 = PickTargetWithMostTargetsHit(actor, combatModel, list3);
					if (actorModel4 == null)
					{
						actorModel4 = PickFarthestTarget(actor, combatModel, list3);
					}
				}
				else
				{
					actorModel4 = PickFarthestTarget(actor, combatModel, list3);
				}
			}
			return actorModel4;
		}
	}
}
