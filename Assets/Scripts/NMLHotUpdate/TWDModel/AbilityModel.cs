using System;
using System.Collections.Generic;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class AbilityModel : TWDModelObject, IModifiableModel
	{
		[NonSerialized]
		[JsonIgnore]
		private bool definitionInvalid = true;

		[NonSerialized]
		[JsonIgnore]
		private AbilityDefinition definition;

		[NonSerialized]
		[JsonIgnore]
		private string definitionID;

		private int maxUses = -1;

		[JsonIgnore]
		public AbilityEffectPush PushEffect;

		[JsonIgnore]
		public int UsesThisTurn;

		[JsonIgnore]
		public int MaxUsesPerTurn = 1;

		[JsonIgnore]
		public IModifierCollection Modifiers { get; private set; }

		[JsonIgnore]
		public AbilityDefinition Definition
		{
			get
			{
				if (definitionInvalid)
				{
					definition = base.manager.GameEconomyData.GetAbilityDefinition(DefinitionID);
					definitionInvalid = false;
				}
				return definition;
			}
			private set
			{
				definition = value;
			}
		}

		public string DefinitionID
		{
			get
			{
				return definitionID;
			}
			set
			{
				if (value != definitionID)
				{
					definitionInvalid = true;
					definitionID = value;
				}
			}
		}

		public int TotalUses { get; set; }

		public int MaxUses
		{
			get
			{
				return maxUses;
			}
			set
			{
				maxUses = value;
				if (LinkedAbility != null)
				{
					LinkedAbility.MaxUses = maxUses;
				}
			}
		}

		[JsonIgnore]
		public List<AbilityEffect> Effects { get; private set; }

		[JsonIgnore]
		public AbilityModel LinkedAbility { get; private set; }

		[JsonIgnore]
		public AbilityModel ParentAbility { get; set; }

		[JsonIgnore]
		public List<ModelAction> PostExecuteActions { get; private set; }

		[JsonIgnore]
		public bool IsChargeAttack
		{
			get
			{
				if (Definition != null)
				{
					return Definition.ChargePointCost > 0;
				}
				return false;
			}
		}

		[JsonIgnore]
		public bool IsConsumableAbility
		{
			get
			{
				if (!Definition.IsFreeAction && Definition.InitialCooldown <= 0)
				{
					return Definition.CooldownAfterUse > 1;
				}
				return true;
			}
		}

		[JsonIgnore]
		public bool IsAttackAbility => Definition.IsAttack;

		[JsonIgnore]
		protected virtual bool BypassTacticalCheck => false;

		public void SetDefinition(AbilityDefinition inDefinition)
		{
			definition = inDefinition;
			definitionInvalid = false;
		}

		public override bool IsValid()
		{
			return true;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, bool acceptInteractiveObjects = false)
		{
			if (sourceActor.DeathsDoor_IsPursuitAttack && !IsChargeAttack)
			{
				AbilityModel abilityModel = sourceActor.GetChargeEquipment()?.Ability;
				if (abilityModel != null)
				{
					FixedPoint range = abilityModel.Definition.AbilityRange;
					CombatHelpers.CalculateRangeExtension(ref range, sourceActor, combatModel.AbilityManager);
					return CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, targetCell, range, acceptInteractiveObjects);
				}
			}
			if (!BypassTacticalCheck && !sourceActor.DeathsDoor_IsPursuitAttack && sourceActor.AbilityCompleted && !Definition.IsFreeAction && sourceActor.AllowSecondMoveAfterAbility && !sourceActor.TurnComplete && sourceActor.AdditionalAttackCount <= 0 && !sourceActor.GetWeaponEquipment().HasTemporaryTrait("FiringSquadDamageActive") && !sourceActor.GetWeaponEquipment().HasTemporaryTrait("DeadlyFocusEXDamageActive"))
			{
				return AbilityResult.FailedOutOfUses;
			}
			if (Definition.RequiresLineOfSight && !combatModel.IsGridCellVisible(sourceCell, targetCell))
			{
				return AbilityResult.FailedVisibilityBlocked;
			}
			if (Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(sourceCell, targetCell))
			{
				return AbilityResult.FailedMovementBlocked;
			}
			AbilityResult abilityResult = AbilityResult.Success;
			for (int i = 0; i < Effects.Count; i++)
			{
				AbilityEffect abilityEffect = Effects[i];
				if (abilityEffect != null)
				{
					abilityResult = abilityEffect.CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, targetCell, acceptInteractiveObjects);
					if (abilityResult != AbilityResult.Success)
					{
						break;
					}
				}
			}
			return abilityResult;
		}

		public AbilityResult CanAbilityBePerformedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, FixedPoint preComputedRange, bool acceptInteractiveObjects = false)
		{
			if (!BypassTacticalCheck && !sourceActor.DeathsDoor_IsPursuitAttack && sourceActor.AbilityCompleted && !Definition.IsFreeAction && sourceActor.AllowSecondMoveAfterAbility && !sourceActor.TurnComplete && sourceActor.AdditionalAttackCount <= 0 && !sourceActor.GetWeaponEquipment().HasTemporaryTrait("FiringSquadDamageActive") && !sourceActor.GetWeaponEquipment().HasTemporaryTrait("DeadlyFocusEXDamageActive"))
			{
				return AbilityResult.FailedOutOfUses;
			}
			if (Definition.RequiresLineOfSight && !combatModel.IsGridCellVisible(sourceCell, targetCell))
			{
				return AbilityResult.FailedVisibilityBlocked;
			}
			if (Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(sourceCell, targetCell))
			{
				return AbilityResult.FailedMovementBlocked;
			}
			AbilityResult abilityResult = AbilityResult.Success;
			for (int i = 0; i < Effects.Count; i++)
			{
				AbilityEffect abilityEffect = Effects[i];
				if (abilityEffect != null)
				{
					abilityResult = abilityEffect.CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, targetCell, preComputedRange, acceptInteractiveObjects);
					if (abilityResult != AbilityResult.Success)
					{
						break;
					}
				}
			}
			return abilityResult;
		}

		public AbilityResult CanAbilityBePerformedOnGridCell_NoBypassTacticalCheck(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell, bool acceptInteractiveObjects = false)
		{
			if (Definition.RequiresLineOfSight && !combatModel.IsGridCellVisible(sourceCell, targetCell))
			{
				return AbilityResult.FailedVisibilityBlocked;
			}
			if (Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(sourceCell, targetCell))
			{
				return AbilityResult.FailedMovementBlocked;
			}
			AbilityResult abilityResult = AbilityResult.Success;
			for (int i = 0; i < Effects.Count; i++)
			{
				AbilityEffect abilityEffect = Effects[i];
				if (abilityEffect != null)
				{
					abilityResult = abilityEffect.CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, targetCell, acceptInteractiveObjects);
					if (abilityResult != AbilityResult.Success)
					{
						break;
					}
				}
			}
			return abilityResult;
		}

		public bool CanAbilityBeTargetedOnGridCell(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell, GridCoordinate targetCell)
		{
			GridModel grid = combatModel.Grid;
			bool flag = true;
			bool flag2 = true;
			bool flag3 = false;
			if (Definition.RequiresLineOfSight && !combatModel.IsGridCellVisible(sourceCell, targetCell))
			{
				flag = false;
			}
			else if (Definition.RequiresLineOfMovement && combatModel.IsGridLineMovementBlocked(sourceCell, targetCell))
			{
				flag2 = false;
			}
			FixedPoint range = Definition.AbilityRange;
			if (!Definition.IsFreeAction)
			{
				CombatHelpers.CalculateRangeExtension(ref range, sourceActor, combatModel.AbilityManager);
			}
			FixedPoint fixedPoint = (range + (Definition.AbilityTargetDiagonal ? 0.42f : 0f)) * grid.CellSize.X;
			FixedPoint fixedPoint2 = fixedPoint * fixedPoint;
			FixedVec3 position = grid.GetPosition(sourceCell);
			FixedVec3 position2 = grid.GetPosition(targetCell);
			if ((position - position2).SqrMagnitude < fixedPoint2)
			{
				flag3 = true;
			}
			return flag && flag2 && flag3;
		}

		public bool IsEquipmentAllowed(EquipmentType equipmentType)
		{
			if (Definition.AllowedEquipmentTypes != null)
			{
				return Definition.AllowedEquipmentTypes.Contains(equipmentType);
			}
			return false;
		}

		public List<GridCoordinate> GetAvailableTargetPositions(CombatModel combatModel, ActorModel sourceActor, GridCoordinate sourceCell)
		{
			List<GridCoordinate> list = new List<GridCoordinate>();
			bool isUsingAdditionalAttacks = sourceActor.GetIsUsingAdditionalAttacks();
			if (Definition.IsPerformedAfterPlayerMove && !isUsingAdditionalAttacks)
			{
				List<ActorModel> list2 = new List<ActorModel>();
				List<ActorModel> allActors = combatModel.GetAllActors();
				for (int i = 0; i < allActors.Count; i++)
				{
					ActorModel actorModel = allActors[i];
					if (IsTargetValid(sourceActor, actorModel) && combatModel.IsActorWithinMoveRangeForAbility(this, sourceActor, actorModel.GridCoordinate))
					{
						list2.Add(actorModel);
					}
				}
				for (int j = 0; j < list2.Count; j++)
				{
					ActorModel actorModel2 = list2[j];
					GridPath gridPath = combatModel.FindPath(sourceActor, sourceActor.GridCoordinate, actorModel2.GridCoordinate);
					GridCoordinate sourceCell2 = sourceActor.GridCoordinate;
					if (gridPath.IsValid)
					{
						sourceCell2 = gridPath.End;
					}
					List<GridCoordinate> abilityTargetsInRange = combatModel.GetAbilityTargetsInRange(this, sourceActor, sourceCell2);
					for (int k = 0; k < abilityTargetsInRange.Count; k++)
					{
						GridCoordinate item = abilityTargetsInRange[k];
						if (!list.Contains(item))
						{
							list.Add(item);
						}
					}
				}
			}
			else
			{
				List<GridCoordinate> abilityTargetsInRange2 = combatModel.GetAbilityTargetsInRange(this, sourceActor, sourceCell);
				for (int l = 0; l < abilityTargetsInRange2.Count; l++)
				{
					GridCoordinate gridCoordinate = abilityTargetsInRange2[l];
					if (CanAbilityBePerformedOnGridCell(combatModel, sourceActor, sourceCell, gridCoordinate) == AbilityResult.Success)
					{
						list.Add(gridCoordinate);
					}
				}
			}
			return list;
		}

		public bool IsAbilityAvailable()
		{
			if (MaxUses >= 0)
			{
				return TotalUses < MaxUses;
			}
			return true;
		}

		public bool IsTargetValid(ActorModel source, ActorModel target, bool allowFriendlyFire = false)
		{
			if (Definition.TargetType == AbilityTargetType.Friendly || allowFriendlyFire)
			{
				return !source.IsEnemy(target);
			}
			if (Definition.TargetType == AbilityTargetType.Enemy)
			{
				return source.IsEnemy(target);
			}
			return true;
		}

		public override void Start()
		{
			base.Start();
			Modifiers = new ModifierCollection();
			ModifierCollection obj = Modifiers as ModifierCollection;
			obj.SetManager(base.manager);
			obj.Initialize();
			Effects = new List<AbilityEffect>();
			if (Definition == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(Definition.LinkedAbilityIdentifier))
			{
				LinkedAbility = new AbilityModel();
				LinkedAbility.DefinitionID = Definition.LinkedAbilityIdentifier;
				LinkedAbility.SetManager(base.manager);
				LinkedAbility.MaxUses = MaxUses;
				LinkedAbility.Start();
				LinkedAbility.ParentAbility = this;
			}
			if (Definition.EffectDefinitions != null)
			{
				foreach (AbilityEffectDefinition effectDefinition in Definition.EffectDefinitions)
				{
					string type = effectDefinition.Type;
					if (string.IsNullOrEmpty(type))
					{
						continue;
					}
					if (!(ReflectionUtils.Instantiate(ReflectionUtils.FindDerivedType(typeof(AbilityEffect), type), effectDefinition.ConstructionParameters) is AbilityEffect abilityEffect))
					{
						base.Debug.LogError("Failed to instantiate " + type + " (" + effectDefinition.ConstructionParameters?.ToString() + ")");
					}
					else
					{
						abilityEffect.SetOwnerAbility(this);
						Effects.Add(abilityEffect);
						if (abilityEffect is AbilityEffectPush)
						{
							PushEffect = abilityEffect as AbilityEffectPush;
						}
					}
				}
			}
			if (Definition.Modifiers != null)
			{
				foreach (AbilityModifierDefinition modifier in Definition.Modifiers)
				{
					if (ReflectionUtils.Instantiate(ReflectionUtils.FindDerivedType(typeof(ModelModifier), modifier.Type), modifier.ConstructionParameters) is ModelModifier modelModifier)
					{
						modelModifier.SetManager(base.manager);
						Modifiers.RegisterModifier(modelModifier);
					}
				}
			}
			if (base.manager != null && base.manager.CombatModel != null)
			{
				base.manager.CombatModel.Changed += OnCombatModelChanged;
			}
			PostExecuteActions = new List<ModelAction>();
		}

		private void OnCombatModelChanged(ModelObject m, string changed, object args)
		{
			if (changed == "turnEnded")
			{
				ResetUsesPerTurn();
			}
		}

		public void OnApply()
		{
			if (PushEffect != null)
			{
				PushEffect.Reset();
			}
			PostExecuteActions.Clear();
		}

		public void ExecutePostActions()
		{
			foreach (ModelAction postExecuteAction in PostExecuteActions)
			{
				postExecuteAction.PostAbilityExecute(base.manager);
			}
		}

		public void OnApplied()
		{
			if (MaxUses > -1)
			{
				TotalUses++;
			}
		}

		public void SetupForCombat()
		{
			TotalUses = 0;
		}

		private void ResetUsesPerTurn()
		{
			UsesThisTurn = 0;
			MaxUsesPerTurn = 1;
		}

		public bool GetCanBeBlock(CombatModel combatModel, ActorModel actor)
		{
			if (!Definition.CanBeBlocked)
			{
				return false;
			}
			if (actor.HasAnyLevelTrait("Equipment_Active_Breakthrough"))
			{
				FixedPoint value = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierLBreakthroughMath", ref value, actor);
				FixedPoint successProbabilityExtension = 0.0;
				if (value != 0.0 && actor.manager.Player.RollDice(RollDiceType.ChanceToNotTriggerOverwatch, value, successProbabilityExtension) != PlayerRandomChanceResult.Failed)
				{
					return true;
				}
			}
			if (actor.HasTrait("Equipment.Breakthrough"))
			{
				FixedPoint value2 = 0.0;
				combatModel.AbilityManager.VisitParameter("AbilityModifierLEquipBreakthroughMath", ref value2, actor);
				FixedPoint successProbabilityExtension2 = 0.0;
				if (value2 != 0.0 && actor.manager.Player.RollDice(RollDiceType.ChanceToNotTriggerOverwatch, value2, successProbabilityExtension2) != PlayerRandomChanceResult.Failed)
				{
					return true;
				}
			}
			return false;
		}
	}
}
