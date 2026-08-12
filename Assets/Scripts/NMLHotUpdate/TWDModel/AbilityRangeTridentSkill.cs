using System.Collections.Generic;

namespace TWDModel
{
	public sealed class AbilityRangeTridentSkill : BaseCommandSkill
	{
		public int AngleDegrees { get; private set; }

		public int MiddleExtraRange { get; private set; }

		public int CooldownTurns { get; private set; }

		public int InitialAndMaxCharge { get; private set; }

		public int TurnEndChargeDeduct { get; private set; }

		public int AllyChargeAttackGain { get; private set; }

		public int EndStateChargeThreshold { get; private set; }

		public bool IsActive { get; private set; }

		public int CurrentCharge { get; private set; }

		public FixedPoint ActiveMiddleExtraRange { get; private set; }

		public FixedPoint ActiveSideExtraRange { get; private set; }

		public bool ActiveExtraRangesCaptured { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillAbilityRangeTrident;

		public AbilityRangeTridentSkill()
		{
		}

		public AbilityRangeTridentSkill(AbilityRangeTridentSkill skill)
			: base(skill)
		{
			AngleDegrees = skill.AngleDegrees;
			MiddleExtraRange = skill.MiddleExtraRange;
			CooldownTurns = skill.CooldownTurns;
			InitialAndMaxCharge = skill.InitialAndMaxCharge;
			TurnEndChargeDeduct = skill.TurnEndChargeDeduct;
			AllyChargeAttackGain = skill.AllyChargeAttackGain;
			EndStateChargeThreshold = skill.EndStateChargeThreshold;
			IsActive = skill.IsActive;
			CurrentCharge = skill.CurrentCharge;
			ActiveMiddleExtraRange = skill.ActiveMiddleExtraRange;
			ActiveSideExtraRange = skill.ActiveSideExtraRange;
			ActiveExtraRangesCaptured = skill.ActiveExtraRangesCaptured;
		}

		public AbilityRangeTridentSkill(int angleDegrees, int middleExtraRange, int cooldownTurns, int initialAndMaxCharge, int turnEndChargeDeduct, int allyChargeAttackGain, int endStateChargeThreshold)
		{
			AngleDegrees = angleDegrees;
			MiddleExtraRange = middleExtraRange;
			CooldownTurns = cooldownTurns;
			InitialAndMaxCharge = initialAndMaxCharge;
			TurnEndChargeDeduct = turnEndChargeDeduct;
			AllyChargeAttackGain = allyChargeAttackGain;
			EndStateChargeThreshold = endStateChargeThreshold;
		}

		public override bool CanExecute(GridCoordinate targetCell)
		{
			return base.CanExecute(targetCell);
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			Activate();
		}

		public void Activate()
		{
			if (IsActive)
			{
				CurrentCharge = InitialAndMaxCharge;
				NotifyOwnerChanged("AbilityRangeTridentChargeChanged");
				return;
			}
			IsActive = true;
			CurrentCharge = InitialAndMaxCharge;
			CaptureActiveExtraRanges();
			NotifyOwnerChanged("AbilityRangeTridentStateChanged");
			NotifyOwnerChanged("AbilityRangeTridentChargeChanged");
		}

		public void Deactivate()
		{
			if (IsActive)
			{
				IsActive = false;
				ActiveMiddleExtraRange = 0L;
				ActiveSideExtraRange = 0L;
				ActiveExtraRangesCaptured = false;
				NotifyOwnerChanged("AbilityRangeTridentStateChanged");
				NotifyOwnerChanged("AbilityRangeTridentChargeChanged");
			}
		}

		public static void NotifyFactionChargeAttack(CombatModel combatModel, ActorModel chargeAttacker)
		{
			if (combatModel == null || chargeAttacker == null)
			{
				return;
			}
			List<ActorModel> factionActors = combatModel.GetFactionActors(chargeAttacker.Faction);
			if (factionActors == null || factionActors.Count == 0)
			{
				return;
			}
			for (int i = 0; i < factionActors.Count; i++)
			{
				CommandSkillModelManager commandSkillModelManager = factionActors[i].CommandSkillModelManager;
				if (commandSkillModelManager == null)
				{
					continue;
				}
				if (commandSkillModelManager.CommandSkills != null)
				{
					for (int j = 0; j < commandSkillModelManager.CommandSkills.Count; j++)
					{
						if (commandSkillModelManager.CommandSkills[j] is AbilityRangeTridentSkill abilityRangeTridentSkill)
						{
							abilityRangeTridentSkill.AddCharge(abilityRangeTridentSkill.AllyChargeAttackGain);
						}
					}
				}
				if (commandSkillModelManager.ActorCommandSkill is AbilityRangeTridentSkill abilityRangeTridentSkill2)
				{
					abilityRangeTridentSkill2.AddCharge(abilityRangeTridentSkill2.AllyChargeAttackGain);
				}
			}
		}

		public void AddCharge(int amount)
		{
			if (amount != 0)
			{
				int currentCharge = CurrentCharge;
				CurrentCharge += amount;
				if (CurrentCharge > InitialAndMaxCharge)
				{
					CurrentCharge = InitialAndMaxCharge;
				}
				if (CurrentCharge != currentCharge)
				{
					NotifyOwnerChanged("AbilityRangeTridentChargeChanged");
				}
				if (IsActive)
				{
					TryEndByChargeThreshold();
				}
			}
		}

		public void TickTurnEndCharge()
		{
			if (IsActive)
			{
				CurrentCharge -= TurnEndChargeDeduct;
				NotifyOwnerChanged("AbilityRangeTridentChargeChanged");
				TryEndByChargeThreshold();
			}
		}

		private void TryEndByChargeThreshold()
		{
			if (IsActive && CurrentCharge <= EndStateChargeThreshold)
			{
				Deactivate();
			}
		}

		public FixedPoint GetEffectiveMiddleExtraRange()
		{
			if (!IsActive)
			{
				return 0L;
			}
			EnsureActiveExtraRangesCaptured();
			return ActiveMiddleExtraRange;
		}

		public FixedPoint GetEffectiveSideExtraRange()
		{
			if (!IsActive)
			{
				return 0L;
			}
			EnsureActiveExtraRangesCaptured();
			return ActiveSideExtraRange;
		}

		public static FixedPoint GetActiveMiddleExtraRange(ActorModel actor)
		{
			return GetActiveSkill(actor)?.GetEffectiveMiddleExtraRange() ?? ((FixedPoint)0L);
		}

		private void EnsureActiveExtraRangesCaptured()
		{
			if (!ActiveExtraRangesCaptured)
			{
				CaptureActiveExtraRanges();
			}
		}

		private void CaptureActiveExtraRanges()
		{
			ActiveExtraRangesCaptured = true;
			FixedPoint value = MiddleExtraRange;
			FixedPoint value2 = 0L;
			AbilityManagerModel abilityManager = GetAbilityManager();
			if (abilityManager != null && base.OwnActorModel != null)
			{
				abilityManager.VisitParameter("AbilityModifierLineSeparatedMiddleRangePlus", ref value, base.OwnActorModel);
				abilityManager.VisitParameter("AbilityModifierLineSeparatedSideRangePlus", ref value2, base.OwnActorModel);
			}
			ActiveMiddleExtraRange = value;
			ActiveSideExtraRange = value2;
		}

		public static AbilityRangeTridentSkill FindSkill(ActorModel actor)
		{
			if (actor == null || actor.CommandSkillModelManager == null)
			{
				return null;
			}
			AbilityRangeTridentSkill abilityRangeTridentSkill = actor.CommandSkillModelManager.GetActorCommandSkill<AbilityRangeTridentSkill>(CommandSkillType.CommandSkillAbilityRangeTrident);
			if (abilityRangeTridentSkill == null)
			{
				abilityRangeTridentSkill = actor.CommandSkillModelManager.GetCommandSkill<AbilityRangeTridentSkill>(CommandSkillType.CommandSkillAbilityRangeTrident);
			}
			return abilityRangeTridentSkill;
		}

		public static AbilityRangeTridentSkill GetActiveSkill(ActorModel actor)
		{
			AbilityRangeTridentSkill abilityRangeTridentSkill = FindSkill(actor);
			if (abilityRangeTridentSkill == null || !abilityRangeTridentSkill.IsActive)
			{
				return null;
			}
			return abilityRangeTridentSkill;
		}

		public static bool ShouldApplySeparatedAttackLines(ActorModel source, AbilityModel ability)
		{
			if (ability == null || ability.Definition == null)
			{
				return false;
			}
			if (ability.Definition.AbilityTargetArea == AbilityTargetAreaType.LineSeparated)
			{
				return true;
			}
			if (GetActiveSkill(source) == null)
			{
				return false;
			}
			switch (ability.Definition.AbilityTargetArea)
			{
			case AbilityTargetAreaType.Line:
			case AbilityTargetAreaType.LineMax:
				return true;
			case AbilityTargetAreaType.Circle:
			case AbilityTargetAreaType.ConeLeft:
			case AbilityTargetAreaType.ConeRight:
				return false;
			default:
				return ability.Definition.AbilityTargetAreaAngle <= 1L;
			}
		}

		public void GetSeparatedLineEndCells(CombatModel combatModel, AbilityModel ability, ActorModel source, GridCoordinate sourceCell, GridCoordinate aimCell, out GridCoordinate middleEnd, out GridCoordinate leftEnd, out GridCoordinate rightEnd)
		{
			GetSeparatedLineEndCells(combatModel, ability, source, sourceCell, aimCell, AngleDegrees, GetEffectiveMiddleExtraRange(), GetEffectiveSideExtraRange(), out middleEnd, out leftEnd, out rightEnd);
		}

		public static void GetSeparatedLineWorldEnds(CombatModel combatModel, AbilityModel ability, ActorModel source, FixedVec3 sourcePos, FixedVec3 aimPos, FixedPoint angleDegrees, FixedPoint middleExtraRange, FixedPoint sideExtraRange, out FixedVec3 middleEnd, out FixedVec3 leftEnd, out FixedVec3 rightEnd, bool middleExtraAlreadyInRangeExtension = true)
		{
			FixedVec3 fixedVec = FixedVec3.Normalize(new FixedVec3(aimPos.X - sourcePos.X, 0L, aimPos.Z - sourcePos.Z));
			if (fixedVec.SqrMagnitude == 0L)
			{
				fixedVec = new FixedVec3(1L, 0L, 0L);
			}
			FixedPoint range = ability.Definition.AbilityRange;
			FixedPoint range2 = ability.Definition.AbilityRange;
			if (!ability.IsConsumableAbility)
			{
				CombatHelpers.CalculateRangeExtension(ref range, source, combatModel.AbilityManager);
				CombatHelpers.CalculateRangeExtension(ref range2, source, combatModel.AbilityManager);
				if (middleExtraAlreadyInRangeExtension)
				{
					range2 -= middleExtraRange;
					range2 += sideExtraRange;
				}
				else
				{
					range += middleExtraRange;
					range2 += sideExtraRange;
				}
				if (range2 < 0L)
				{
					range2 = 0L;
				}
			}
			else
			{
				range2 += sideExtraRange;
			}
			FixedPoint x = combatModel.Grid.CellSize.X;
			middleEnd = sourcePos + fixedVec * (range * x);
			leftEnd = sourcePos + RotateAroundY(fixedVec, angleDegrees) * (range2 * x);
			rightEnd = sourcePos + RotateAroundY(fixedVec, -angleDegrees) * (range2 * x);
		}

		public static void GetSeparatedLineEndCells(CombatModel combatModel, AbilityModel ability, ActorModel source, GridCoordinate sourceCell, GridCoordinate aimCell, FixedPoint angleDegrees, FixedPoint middleExtraRange, FixedPoint sideExtraRange, out GridCoordinate middleEnd, out GridCoordinate leftEnd, out GridCoordinate rightEnd, bool middleExtraAlreadyInRangeExtension = true)
		{
			FixedVec3 position = combatModel.Grid.GetPosition(sourceCell);
			FixedVec3 position2 = combatModel.Grid.GetPosition(aimCell);
			GetSeparatedLineWorldEnds(combatModel, ability, source, position, position2, angleDegrees, middleExtraRange, sideExtraRange, out var middleEnd2, out var leftEnd2, out var rightEnd2, middleExtraAlreadyInRangeExtension);
			middleEnd = combatModel.Grid.GetCoordinate(middleEnd2);
			leftEnd = combatModel.Grid.GetCoordinate(leftEnd2);
			rightEnd = combatModel.Grid.GetCoordinate(rightEnd2);
		}

		public static FixedPoint ResolveSeparatedAngleDegrees(ActorModel source, AbilityModel ability)
		{
			AbilityRangeTridentSkill abilityRangeTridentSkill = FindSkill(source);
			if (abilityRangeTridentSkill != null)
			{
				return abilityRangeTridentSkill.AngleDegrees;
			}
			if (ability != null && ability.Definition != null)
			{
				return ability.Definition.AbilityTargetAreaAngle;
			}
			return 0L;
		}

		public static void ResolveSeparatedLineEndCells(CombatModel combatModel, AbilityModel ability, ActorModel source, GridCoordinate sourceCell, GridCoordinate aimCell, out GridCoordinate middleEnd, out GridCoordinate leftEnd, out GridCoordinate rightEnd)
		{
			AbilityRangeTridentSkill activeSkill = GetActiveSkill(source);
			if (activeSkill != null)
			{
				activeSkill.GetSeparatedLineEndCells(combatModel, ability, source, sourceCell, aimCell, out middleEnd, out leftEnd, out rightEnd);
				return;
			}
			FixedPoint value = 0L;
			FixedPoint value2 = 0L;
			if (combatModel.AbilityManager != null && source != null)
			{
				combatModel.AbilityManager.VisitParameter("AbilityModifierLineSeparatedMiddleRangePlus", ref value, source);
				combatModel.AbilityManager.VisitParameter("AbilityModifierLineSeparatedSideRangePlus", ref value2, source);
			}
			GetSeparatedLineEndCells(combatModel, ability, source, sourceCell, aimCell, ability.Definition.AbilityTargetAreaAngle, value, value2, out middleEnd, out leftEnd, out rightEnd, middleExtraAlreadyInRangeExtension: false);
		}

		private static FixedVec3 RotateAroundY(FixedVec3 dir, FixedPoint degrees)
		{
			FixedPoint radians = degrees * FixedPoint.PI / 180L;
			FixedPoint fixedPoint = FixedPoint.Cos(radians);
			FixedPoint fixedPoint2 = FixedPoint.Sin(radians);
			return new FixedVec3(dir.X * fixedPoint - dir.Z * fixedPoint2, dir.Y, dir.X * fixedPoint2 + dir.Z * fixedPoint);
		}

		private AbilityManagerModel GetAbilityManager()
		{
			if (base.OwnActorModel == null || base.OwnActorModel.manager == null || base.OwnActorModel.manager.Player == null)
			{
				return null;
			}
			return base.OwnActorModel.manager.Player.AbilityManager;
		}

		private void NotifyOwnerChanged(string change)
		{
			if (base.OwnActorModel != null)
			{
				base.OwnActorModel.NotifyChange(change);
			}
		}
	}
}
