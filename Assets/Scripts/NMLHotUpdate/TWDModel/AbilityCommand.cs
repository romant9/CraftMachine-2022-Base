using BaseModel;

namespace TWDModel
{
	public class AbilityCommand : ModelCommand
	{
		public int AbilityId { get; private set; }

		public GridCoordinate TargetCell { get; private set; }

		public AbilityCommand()
		{
		}

		public AbilityCommand(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell)
			: base(sourceActor)
		{
			AbilityId = ability.ModelId;
			TargetCell = targetCell;
		}

		public static bool PerformActions(TWDModelManager manager, ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, bool ignoreAPRestrictions = false)
		{
			CombatModel combatModel = manager.CombatModel;
			if (combatModel != null && sourceActor != null && ability != null && targetCell.IsValid)
			{
				ActorModel occupier = combatModel.GetOccupier(targetCell);
				manager.ExecuteAction(new PreAttackAction(occupier, sourceActor, effectiveAttack: true));
				if (manager.GameEconomyData.GetFeature("AbilityCompletedValidation").Enabled && sourceActor.AbilityCompleted && !ignoreAPRestrictions && sourceActor.AdditionalAttackCount <= 0)
				{
					return true;
				}
				int walkersKilled = combatModel.MissionStatistics.WalkersKilled;
				manager.Player.DailyQuestManager.BeginWindow(DailyQuestCompletionWindow.Ability);
				ChargeMeterModel chargeMeter = sourceActor.ChargeMeter;
				bool flag = manager.ExecuteAction(new AbilityAction(sourceActor, ability, targetCell, null, OOTType.None, skipActiveWeaponTraits: false, isAssistAttack: false, isTriggerExtraAttackDamage: false, isFromAbilityCommand: true));
				if (flag)
				{
					EquipmentItemModel selectedEquipment = sourceActor.SelectedEquipment;
					if (selectedEquipment.Ability.Definition.ChargePointCost > 0)
					{
						sourceActor.UsedChargeAttackThisTurn = true;
						chargeMeter.LastChargeConsume = 0;
					}
					if (ability.IsChargeAttack)
					{
						chargeMeter.LastChargeConsume = chargeMeter.ChargeLevel;
					}
					if (ability.IsChargeAttack && !ability.IsConsumableAbility)
					{
						AbilityRangeTridentSkill.NotifyFactionChargeAttack(combatModel, sourceActor);
					}
					if (CanChangeChargeLevel(combatModel, sourceActor, ability))
					{
						int abilityChargePointCost = GetAbilityChargePointCost(sourceActor, ability);
						chargeMeter.ChangeChargeLevel(-abilityChargePointCost);
					}
					if (ability.IsChargeAttack && sourceActor.HasAnyLevelTrait("Heirlooms_Daryl_Bracelets"))
					{
						FixedPoint value = 0.0;
						FixedPoint value2 = 0.0;
						combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, sourceActor);
						combatModel.AbilityManager.VisitParameter("BraceletsGainChargePointChanceForAll", ref value2, sourceActor);
						PlayerRandomChanceResult playerRandomChanceResult = combatModel.manager.Player.RollDice(RollDiceType.GainChargePoint, value2, value);
						if (playerRandomChanceResult != PlayerRandomChanceResult.Failed)
						{
							foreach (ActorModel factionActor in combatModel.GetFactionActors(Faction.Survivor))
							{
								factionActor.AddChargePoints(1);
								factionActor.NotifyChange("AbilityVisited", new object[2]
								{
									"Heirlooms_Daryl_Bracelets",
									playerRandomChanceResult == PlayerRandomChanceResult.SuccessDueToExtension
								});
							}
						}
					}
					int num = ability.Definition.NoiseRange;
					int num2 = ability.Definition.ThreatValue;
					if (selectedEquipment.Definition.Category != EquipmentCategory.Utility)
					{
						FixedPoint value3 = 0.0;
						combatModel.AbilityManager.VisitParameter("AbilitySilencedWeaponChance", ref value3, sourceActor);
						FixedPoint value4 = 0.0;
						if (value3 != 0.0)
						{
							combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value4, sourceActor);
						}
						EquipmentItemModel equipmentWithAbility = sourceActor.GetEquipmentWithAbility(ability);
						if (equipmentWithAbility != null && equipmentWithAbility.Definition != null && equipmentWithAbility.Definition.Category == EquipmentCategory.RangeWeapon)
						{
							combatModel.AbilityManager.VisitParameter("AbilityModifierIncreaseNoThreatChanceRanged", ref value3, sourceActor);
						}
						PlayerRandomChanceResult playerRandomChanceResult2 = manager.Player.RollDice(RollDiceType.Silenced, value3, value4);
						if (playerRandomChanceResult2 != PlayerRandomChanceResult.Failed)
						{
							num = 0;
							num2 = 0;
							FixedPoint value5 = 0.0;
							FixedPoint value6 = 0.0;
							FixedPoint value7 = 0L;
							combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value5, sourceActor);
							combatModel.AbilityManager.VisitParameter("LeaderBuffNoThreatRangedPercentageIncreaseChargePoint", ref value6, sourceActor);
							PlayerRandomChanceResult playerRandomChanceResult3 = combatModel.manager.Player.RollDice(RollDiceType.GainChargePoint, value6, value5);
							if (playerRandomChanceResult3 != PlayerRandomChanceResult.Failed)
							{
								combatModel.AbilityManager.VisitParameter("LeaderBuffNoThreatRangedIncreaseChargePoint", ref value7, sourceActor);
								sourceActor.AddChargePoints((int)value7);
								sourceActor.NotifyChange("AbilityVisited", new object[2]
								{
									"LeaderBuffNoThreatRanged",
									playerRandomChanceResult3 == PlayerRandomChanceResult.SuccessDueToExtension
								});
							}
							sourceActor.NotifyChange("AbilityVisited", new object[2]
							{
								"Silenced",
								playerRandomChanceResult2 == PlayerRandomChanceResult.SuccessDueToExtension
							});
						}
					}
					if (num > 0)
					{
						flag = manager.ExecuteAction(new NoiseAction(sourceActor, sourceActor.GridCoordinate, ability.Definition.NoiseRange, ability.Definition.ThreatValue));
					}
					if (flag && num2 != 0)
					{
						flag = manager.ExecuteAction(new ThreatAction(sourceActor, num2));
					}
					QuestVariables questVariables = manager.Player.DailyQuestManager.StartAction("UseAbility");
					questVariables.SurvivorClass.Clear();
					questVariables.Hero.Clear();
					if (sourceActor != null && sourceActor is SurvivorModel)
					{
						SurvivorModel survivorModel = sourceActor as SurvivorModel;
						questVariables.SurvivorClass.Add(survivorModel.SurvivorClass.ToString());
						if (survivorModel.IsHero)
						{
							questVariables.Hero.Add(survivorModel.Definition.GetNonAlternativeHeroDefinition());
						}
						if (sourceActor.SelectedEquipment.IsChargeEquipment)
						{
							questVariables.AbilityType = "Charge";
						}
						questVariables.AbilityId = ability.DefinitionID;
					}
					manager.Player.DailyQuestManager.CommitAction();
				}
				if (sourceActor.SelectedEquipment.IsChargeEquipment)
				{
					if (sourceActor.Faction == Faction.Survivor)
					{
						SurvivorModel survivorModel2 = sourceActor as SurvivorModel;
						survivorModel2?.Statistics.IncreaseChargeAbilitiesUse();
						FixedPoint value8 = 0.0;
						FixedPoint value9 = 0.0;
						PlayerRandomChanceResult playerRandomChanceResult4 = PlayerRandomChanceResult.Failed;
						if (manager.Player.AbilityManager.VisitParameter(AbilityModifierIncreaseHealingAtChargeUsage.FetchIncreaseHealingAtChargeUsageChance, ref value8, sourceActor) && value8 > 0.0 && manager.Player.AbilityManager.VisitParameter(AbilityModifierIncreaseHealingAtChargeUsage.FetchIncreaseHealingAtChargeUsageMultiplier, ref value9, sourceActor))
						{
							playerRandomChanceResult4 = manager.Player.RollDice(RollDiceType.ActivateChance, value8);
						}
						if (playerRandomChanceResult4 != PlayerRandomChanceResult.Failed)
						{
							int amountHealed = (int)(survivorModel2.MaxHitPoints * value9);
							flag &= manager.ExecuteAction(new HealAction(sourceActor, sourceActor, amountHealed));
							sourceActor.NotifyChange("AbilityVisited", new object[2] { "LeaderBuffHealingCharge", false });
						}
					}
					chargeMeter.ChargeEnabled = false;
					sourceActor.EquipWeaponEquipment();
				}
				else if (sourceActor.SelectedEquipment.Definition.Category == EquipmentCategory.Utility)
				{
					sourceActor.EquipWeaponEquipment();
					EquipmentItemModel consumableEquipment = sourceActor.GetConsumableEquipment();
					combatModel.MissionStatistics.AddConsumableUsed(consumableEquipment, combatModel.TurnManager.TurnCount);
					sourceActor.UsedToolThisTurn = true;
					EquipmentItemModel equipment = sourceActor.UnequipConsumableEquipment(consumableUsed: true);
					manager.Player.Equipment.RemoveEquipment(equipment);
				}
				manager.Player.DailyQuestManager.EndWindow(DailyQuestCompletionWindow.Ability);
				combatModel.AbilityManager.NotifyAbilityPerformed(sourceActor);
				int num3 = combatModel.MissionStatistics.WalkersKilled - walkersKilled;
				if (num3 > 0)
				{
					combatModel.MissionStatistics.AddMultiWalkerKill(num3);
				}
				manager.ExecuteAction(new PostAbilityExecuteAction(occupier, sourceActor));
				return flag;
			}
			return false;
		}

		public override IModelCommandRespond Execute(ModelManager manager)
		{
			ActorModel model = manager.GetModel<ActorModel>(base.ModelId);
			AbilityModel model2 = manager.GetModel<AbilityModel>(AbilityId);
			if (model == null || model2 == null)
			{
				return new NGModelCommandRespond(this, TWDModelResult.ModelObjectNotFound);
			}
			bool flag = PerformActions(manager as TWDModelManager, model, model2, TargetCell, model2.Definition.IsFreeAction);
			return new NGModelCommandRespond(this, (!flag) ? TWDModelResult.Error : TWDModelResult.OK);
		}

		private static bool CanChangeChargeLevel(CombatModel combatModel, ActorModel sourceActor, AbilityModel ability)
		{
			if (!ability.IsChargeAttack)
			{
				return true;
			}
			if (!sourceActor.HasTraitsThatContains("FreeChargePoint"))
			{
				return true;
			}
			bool result = true;
			FixedPoint value = 0.0;
			FixedPoint value2 = 0.0;
			combatModel.AbilityManager.VisitParameter("FreeChargePointNonConsumeChargePointPercentage", ref value2, sourceActor);
			if (value2 > 0.0)
			{
				combatModel.AbilityManager.VisitParameter("ExtendProbability", ref value, sourceActor);
				if (combatModel.manager.Player.RollDice(RollDiceType.FreeChargePoint, value2, value) != PlayerRandomChanceResult.Failed)
				{
					result = false;
				}
			}
			return result;
		}

		private static int GetAbilityChargePointCost(ActorModel sourceActor, AbilityModel ability)
		{
			EquipmentItemModel selectedEquipment = sourceActor.SelectedEquipment;
			ChargeMeterModel chargeMeter = sourceActor.ChargeMeter;
			int num = 0;
			if (ability.IsChargeAttack)
			{
				return chargeMeter.ChargeLevel;
			}
			return selectedEquipment.Ability.Definition.ChargePointCost;
		}
	}
}
