using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityEffectPush : AbilityEffect
	{
		private List<PushEffect> pushEffects;

		private bool applyingEffects;

		private FixedPoint distance;

		private FixedPoint collisionDamageModifier;

		private FixedPoint forceAngle;

		private bool forceSourceFromProjectile;

		private bool isDisablePushDirectionIndicators;

		public FixedPoint ForceAngle => forceAngle;

		public FixedPoint Distance => distance;

		public bool ForceSourceFromProjectile => forceSourceFromProjectile;

		public bool IsDisablePushDirectionIndicators => isDisablePushDirectionIndicators;

		public AbilityEffectPush(string inDistance, string inCollisionDamageModifier, string inForceAngle)
		{
			pushEffects = new List<PushEffect>();
			distance = new FixedPoint(inDistance);
			collisionDamageModifier = new FixedPoint(inCollisionDamageModifier);
			forceAngle = new FixedPoint(inForceAngle);
		}

		public AbilityEffectPush(string inDistance, string inCollisionDamageModifier, string inForceAngle, bool forceSourceFromProjectile)
		{
			pushEffects = new List<PushEffect>();
			distance = new FixedPoint(inDistance);
			collisionDamageModifier = new FixedPoint(inCollisionDamageModifier);
			forceAngle = new FixedPoint(inForceAngle);
			this.forceSourceFromProjectile = forceSourceFromProjectile;
		}

		public AbilityEffectPush(string inDistance, string inCollisionDamageModifier, string inForceAngle, bool forceSourceFromProjectile, bool isDisablePushDirectionIndicators)
		{
			pushEffects = new List<PushEffect>();
			distance = new FixedPoint(inDistance);
			collisionDamageModifier = new FixedPoint(inCollisionDamageModifier);
			forceAngle = new FixedPoint(inForceAngle);
			this.forceSourceFromProjectile = forceSourceFromProjectile;
			this.isDisablePushDirectionIndicators = isDisablePushDirectionIndicators;
		}

		public bool Add(DamageAction action)
		{
			if (applyingEffects || action.DamagerActor == null)
			{
				return false;
			}
			pushEffects.Add(new PushEffect
			{
				DamageAction = action
			});
			return true;
		}

		public void Reset()
		{
			pushEffects.Clear();
			applyingEffects = false;
		}

		public override bool ApplyEffect(CombatModel combatModel, ActorModel source, GridCoordinate targetCell, ActorModel targetActor = null, Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls = null, OOTType ootType = OOTType.None, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false)
		{
			applyingEffects = true;
			pushEffects.RemoveAll((PushEffect t) => t.DamageAction.Dodged && !t.DamageAction.Critical);
			foreach (PushEffect pushEffect2 in pushEffects)
			{
				pushEffect2.OriginalCoordinate = pushEffect2.DamageAction.TargetActor.GridCoordinate;
				GridCoordinate sourceCoordinate = ((forceSourceFromProjectile && targetCell != pushEffect2.DamageAction.TargetActor.GridCoordinate) ? targetCell : pushEffect2.DamageAction.DamagerActor.GridCoordinate);
				pushEffect2.PushCoordinate = FindFurthestPushCoordinate(combatModel, pushEffect2, sourceCoordinate);
			}
			List<PushEffect> list = new List<PushEffect>();
			foreach (PushEffect pushEffect3 in pushEffects)
			{
				if (pushEffect3.DependsOn == null)
				{
					list.Add(pushEffect3);
				}
			}
			foreach (PushEffect item in list)
			{
				PushEffect pushEffect = item;
				while (pushEffect != null && !pushEffect.Handled)
				{
					ActorModel occupier = combatModel.GetOccupier(pushEffect.PushCoordinate);
					ActorModel targetActor2 = pushEffect.DamageAction.TargetActor;
					if (targetActor2.IsDisoriented || CombatHelpers.CheckWeeklyChallengePushAvoid(targetActor2) || targetActor2.IsABTesterAed || targetActor2.IsABTesterA2ed)
					{
						pushEffect = pushEffect.Dependant;
						continue;
					}
					if (CombatHelpers.CheckPreventPush(targetActor2))
					{
						targetActor2.NotifyChange("AbilityVisited", new object[2] { "PreventPush", false });
						pushEffect = pushEffect.Dependant;
						continue;
					}
					GridPath gridPath = combatModel.FindPath(targetActor2, targetActor2.GridCoordinate, pushEffect.PushCoordinate);
					if (combatModel.IsBlocked(pushEffect.PushCoordinate) || gridPath.Length - 1 > distance || (occupier == null && !gridPath.IsValid))
					{
						CollideTarget(combatModel, pushEffect, targetActor2);
					}
					else if (occupier != null)
					{
						CollideTarget(combatModel, pushEffect, targetActor2);
						if (occupier.IsEnemy(pushEffect.DamageAction.DamagerActor))
						{
							CollideTarget(combatModel, pushEffect, occupier);
						}
					}
					else
					{
						combatModel.manager.ExecuteAction(new PushActorAction(pushEffect));
						if (pushEffect.DamageAction.DamagerActor.Definition.Class == SurvivorClass.Bruiser.ToString() && !targetActor2.IsDead && !targetActor2.IsEnvironmental)
						{
							int num = 0;
							if (pushEffect.DamageAction.DamagerActor.HasAnyLevelTrait("Equipment_Active_RiotShield_Stun"))
							{
								TraitEntry traitAnyLevel = pushEffect.DamageAction.DamagerActor.TraitContainer.GetTraitAnyLevel("Equipment_Active_RiotShield_Stun");
								num = (int)pushEffect.DamageAction.DamagerActor.manager.GameEconomyData.GetTraitDefinition(traitAnyLevel.TraitIdentifier).GetParameter<FixedPoint>(0);
							}
							else
							{
								num = (int)pushEffect.DamageAction.DamagerActor.manager.GameEconomyData.GetTraitDefinition("Equipment_Active_Stun").GetParameter<FixedPoint>(0);
							}
							DamageAction damageAction = pushEffect.DamageAction;
							combatModel.manager.ExecuteAction(new StunAction(pushEffect.DamageAction.DamagerActor, targetActor2, num, ignoreSourceBeingDead: false, null, () => damageAction.FinalDamage));
						}
					}
					pushEffect.Handled = true;
					pushEffect = pushEffect.Dependant;
				}
			}
			applyingEffects = false;
			return true;
		}

		private void CollideTarget(CombatModel combatModel, PushEffect effect, ActorModel target)
		{
			ActorModel damagerActor = effect.DamageAction.DamagerActor;
			bool isConsumableAbility = damagerActor.SelectedAbility.IsConsumableAbility;
			damagerActor.AddTemporaryTrait("PushCollisionDamage", default(FixedPoint), null, 0L);
			Dictionary<ActorModel, List<DamageNotificationData>> damageNotifications = new Dictionary<ActorModel, List<DamageNotificationData>>();
			PlayerRandomChanceResult criticalResult;
			PlayerRandomChanceResult bodyShotResult;
			int[] array = ((!isConsumableAbility) ? CombatHelpers.CalculateDamage(combatModel, damagerActor, target, effect.DamageAction.DamageType, out criticalResult, out bodyShotResult, null, isSingleTarget: false, isChargeAttack: false, ref damageNotifications) : CombatHelpers.CalculateDamageConsumable(combatModel, damagerActor, target, effect.DamageAction.DamageType, out criticalResult, out bodyShotResult, null, isSingleTarget: false, isChargeAttack: false, ref damageNotifications));
			array[0] = (int)(array[0] * collisionDamageModifier);
			if (isConsumableAbility)
			{
				int strugglesLeft = target.StrugglesLeft;
				CombatHelpers.ExecuteDamageConsumable(combatModel, damagerActor, target, array[0], array[1], effect.DamageAction.DamageType, criticalResult, bodyShotResult, damageNotifications);
				if (target.IsHuman)
				{
					if (strugglesLeft > target.StrugglesLeft)
					{
						foreach (ModelAction item in combatModel.manager.Player.AbilityManager.AbilityUnderApplication.PostExecuteActions.FindAll((ModelAction action) => action is DamageConsumableAction damageConsumableAction2 && damageConsumableAction2.TargetActor == target))
						{
							((DamageConsumableAction)item).DamageIgnored = true;
							combatModel.manager.Player.AbilityManager.AbilityUnderApplication.PostExecuteActions.Remove(item);
						}
					}
					else if (strugglesLeft == 1)
					{
						foreach (ModelAction postExecuteAction in combatModel.manager.Player.AbilityManager.AbilityUnderApplication.PostExecuteActions)
						{
							if (postExecuteAction is DamageConsumableAction damageConsumableAction && damageConsumableAction.TargetActor == target)
							{
								damageConsumableAction.CalculateFinalDamage();
							}
						}
					}
				}
			}
			else
			{
				CombatHelpers.ExecuteDamage(combatModel, damagerActor, target, array[0], array[1], effect.DamageAction.DamageType, criticalResult, bodyShotResult, damageNotifications);
			}
			damagerActor.RemoveTrait("PushCollisionDamage");
		}

		private GridCoordinate FindFurthestPushCoordinate(CombatModel combatModel, PushEffect effect, GridCoordinate sourceCoordinate)
		{
			GridModel grid = combatModel.Grid;
			GridCoordinate gridCoordinate = effect.DamageAction.TargetActor.GridCoordinate;
			FixedVec3 position = grid.GetPosition(sourceCoordinate);
			FixedVec3 position2 = grid.GetPosition(gridCoordinate);
			FixedVec3 fixedVec = FixedVec3.Normalize(position2 - position);
			FixedPoint radians = -FixedPoint.DegToRad(forceAngle);
			fixedVec = new FixedVec3(FixedPoint.Cos(radians) * fixedVec.X - FixedPoint.Sin(radians) * fixedVec.Z, fixedVec.Y, FixedPoint.Cos(radians) * fixedVec.Z + FixedPoint.Sin(radians) * fixedVec.X);
			GridCoordinate coordinate = grid.GetCoordinate(position2 + fixedVec * (distance * grid.CellSize.X));
			if (coordinate == gridCoordinate)
			{
				return gridCoordinate;
			}
			List<GridCoordinate> lineCoordinates = GridModel.GetLineCoordinates(gridCoordinate, coordinate);
			for (int i = 1; i < lineCoordinates.Count; i++)
			{
				GridCoordinate fromCoordinate = lineCoordinates[i - 1];
				GridCoordinate gridCoordinate2 = lineCoordinates[i];
				if (!combatModel.CanTraverse(null, fromCoordinate, gridCoordinate2) || combatModel.IsBlocked(gridCoordinate2))
				{
					return gridCoordinate2;
				}
				ActorModel occupier = combatModel.GetOccupier(gridCoordinate2);
				if (occupier != null)
				{
					PushEffect pushEffect = FindPushEffectWithOccupier(occupier);
					if (pushEffect != null && pushEffect.Dependant == null)
					{
						pushEffect.Dependant = effect;
						effect.DependsOn = pushEffect;
					}
					return gridCoordinate2;
				}
				if (FindPushEffectWithPushCoordinate(gridCoordinate2) != null)
				{
					return gridCoordinate2;
				}
			}
			return coordinate;
		}

		public GridCoordinate FindFurthestPushCoordinateByCoordinates(CombatModel combatModel, GridCoordinate sourceCoordinate, GridCoordinate targetCoordinate)
		{
			GridModel grid = combatModel.Grid;
			FixedVec3 position = grid.GetPosition(sourceCoordinate);
			FixedVec3 position2 = grid.GetPosition(targetCoordinate);
			FixedVec3 fixedVec = FixedVec3.Normalize(position2 - position);
			FixedPoint radians = -FixedPoint.DegToRad(forceAngle);
			fixedVec = new FixedVec3(FixedPoint.Cos(radians) * fixedVec.X - FixedPoint.Sin(radians) * fixedVec.Z, fixedVec.Y, FixedPoint.Cos(radians) * fixedVec.Z + FixedPoint.Sin(radians) * fixedVec.X);
			GridCoordinate coordinate = grid.GetCoordinate(position2 + fixedVec * (distance * grid.CellSize.X));
			if (coordinate == targetCoordinate)
			{
				return targetCoordinate;
			}
			List<GridCoordinate> lineCoordinates = GridModel.GetLineCoordinates(targetCoordinate, coordinate);
			for (int i = 1; i < lineCoordinates.Count; i++)
			{
				GridCoordinate fromCoordinate = lineCoordinates[i - 1];
				GridCoordinate gridCoordinate = lineCoordinates[i];
				if (!combatModel.CanTraverse(null, fromCoordinate, gridCoordinate) || combatModel.IsBlocked(gridCoordinate))
				{
					return gridCoordinate;
				}
				if (combatModel.GetOccupier(gridCoordinate) != null || FindPushEffectWithPushCoordinate(gridCoordinate) != null)
				{
					return gridCoordinate;
				}
			}
			return coordinate;
		}

		private PushEffect FindPushEffectWithOccupier(ActorModel occupier)
		{
			foreach (PushEffect pushEffect in pushEffects)
			{
				if (pushEffect.DamageAction.TargetActor == occupier)
				{
					return pushEffect;
				}
			}
			return null;
		}

		private PushEffect FindPushEffectWithPushCoordinate(GridCoordinate coordinate)
		{
			foreach (PushEffect pushEffect in pushEffects)
			{
				if (pushEffect.PushCoordinate == coordinate)
				{
					return pushEffect;
				}
			}
			return null;
		}
	}
}
