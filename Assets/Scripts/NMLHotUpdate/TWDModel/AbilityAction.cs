using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class AbilityAction : ModelActorAction
	{
		private const string ActiveTraitTag = "EquipmentActive";

		private Dictionary<RollDiceType, PlayerRandomChanceResult> resolvedRolls;

		public AbilityModel Ability { get; private set; }

		public GridCoordinate TargetCell { get; private set; }

		public ActorModel TargetActor { get; private set; }

		public OOTType OOTType { get; private set; }

		public bool IsFromAbilityCommand { get; private set; }

		public bool SkipActiveWeaponTraits { get; }

		public bool IsAssistAttack { get; }

		public bool IsTriggerExtraAttackDamage { get; }

		public AbilityAction(ActorModel sourceActor, AbilityModel ability, GridCoordinate targetCell, ActorModel targetActor = null, OOTType ootType = OOTType.None, bool skipActiveWeaponTraits = false, bool isAssistAttack = false, bool isTriggerExtraAttackDamage = false, bool isFromAbilityCommand = false)
			: base(sourceActor)
		{
			Ability = ability;
			TargetCell = targetCell;
			TargetActor = targetActor;
			OOTType = ootType;
			SkipActiveWeaponTraits = skipActiveWeaponTraits;
			resolvedRolls = new Dictionary<RollDiceType, PlayerRandomChanceResult>();
			IsAssistAttack = isAssistAttack;
			IsTriggerExtraAttackDamage = isTriggerExtraAttackDamage;
			IsFromAbilityCommand = isFromAbilityCommand;
		}

		public void AddResolvedRoll(RollDiceType type, PlayerRandomChanceResult result)
		{
			resolvedRolls[type] = result;
		}

		public override bool CanExecute()
		{
			if (base.CanExecute() && !base.Actor.IsStruggling && !base.Actor.IsBleedingOut && !base.Actor.IsStunned && !base.Actor.IsElectricShocked && !base.Actor.IsEatingLure)
			{
				return !base.Actor.IsQuantunCanNotMove;
			}
			return false;
		}

		public override bool Execute(ModelManager manager)
		{
			CombatModel combatModel = (manager as TWDModelManager).CombatModel;
			if (combatModel != null)
			{
				if (OOTType != OOTType.None || base.Actor.OverwatchedOnTurn)
				{
					if (base.Actor.IsDead || (TargetActor != null && (TargetActor.IsDead || TargetActor.Faction == Faction.Lure)))
					{
						base.Actor.SelectedEquipment.RemoveTemporaryTraitsByExpirationType(TraitExpirationType.Activation);
						base.Actor.OverwatchedOnTurn = false;
						return true;
					}
					if (TargetCell != TargetActor.GridCoordinate)
					{
						if (GetOOTValidationAbility().CanAbilityBePerformedOnGridCell(combatModel, base.Actor, base.Actor.GridCoordinate, TargetActor.GridCoordinate) != AbilityResult.Success)
						{
							base.Actor.SelectedEquipment.RemoveTemporaryTraitsByExpirationType(TraitExpirationType.Activation);
							base.Actor.OverwatchedOnTurn = false;
							return true;
						}
						TargetCell = TargetActor.GridCoordinate;
					}
				}
				if (combatModel.manager.CurrentCommandLogEntry != null)
				{
					combatModel.manager.CurrentCommandLogEntry.StartAbilityLog(base.Actor, Ability);
				}
				EquipmentItemModel equipmentItemModel = ((OOTType != OOTType.PassByAttack) ? base.Actor.SelectedEquipment : base.Actor.GetWeaponEquipment());
				Dictionary<string, FixedPoint> dictionary = new Dictionary<string, FixedPoint>();
				List<UpgradeTraitsData> availableTraits = equipmentItemModel.GetAvailableTraits();
				List<TemporaryTraitsData> temporaryTraitsByExpirationType = equipmentItemModel.GetTemporaryTraitsByExpirationType(TraitExpirationType.Activation);
				GameEconomyData gameEconomyData = combatModel.manager.GameEconomyData;
				for (int i = 0; i < temporaryTraitsByExpirationType.Count; i++)
				{
					TemporaryTraitsData temporaryTraitsData = temporaryTraitsByExpirationType[i];
					if (!base.Actor.HasTrait(temporaryTraitsData.Identifier))
					{
						dictionary.Add(temporaryTraitsData.Identifier, temporaryTraitsData.ConstructionMultiplier);
						continue;
					}
					(manager as TWDModelManager).Debug.LogWarning("Actor [" + base.Actor.ToString() + "] already has a trait with the same Identifier when trying to add a temporary activation trait : " + temporaryTraitsData.Identifier + " - The temporary trait was ignored!");
				}
				if (equipmentItemModel != null && equipmentItemModel.GetEquipmentActiveTraits() != null)
				{
					foreach (string equipmentActiveTrait in equipmentItemModel.GetEquipmentActiveTraits())
					{
						dictionary.Add(equipmentActiveTrait, 0L);
					}
				}
				EquipmentItemModel weaponEquipment = base.Actor.GetWeaponEquipment();
				if (weaponEquipment != null)
				{
					if (equipmentItemModel.IsChargeEquipment)
					{
						List<TraitDefinition> chargeActiveTraits = weaponEquipment.GetChargeActiveTraits();
						if (chargeActiveTraits != null && chargeActiveTraits.Count > 0)
						{
							foreach (TraitDefinition item in chargeActiveTraits)
							{
								dictionary.Add(item.Identifier, 0L);
							}
						}
					}
					else
					{
						List<TraitDefinition> activeTraits = equipmentItemModel.GetActiveTraits();
						if (activeTraits != null && activeTraits.Count > 0)
						{
							foreach (TraitDefinition item2 in activeTraits)
							{
								dictionary.Add(item2.Identifier, 0L);
							}
						}
					}
				}
				if (!SkipActiveWeaponTraits)
				{
					foreach (KeyValuePair<string, FixedPoint> item3 in dictionary)
					{
						if (combatModel.manager.CurrentCommandLogEntry != null)
						{
							combatModel.manager.CurrentCommandLogEntry.AddTempTrait(item3.Key, item3.Value);
						}
						base.Actor.AddTrait(item3.Key, item3.Value);
					}
					for (int j = 0; j < availableTraits.Count; j++)
					{
						UpgradeTraitsData upgradeTraitsData = availableTraits[j];
						TraitDefinition traitDefinition = gameEconomyData.GetTraitDefinition(upgradeTraitsData.Identifier);
						if (traitDefinition != null && traitDefinition.Tags.Contains("EquipmentActive"))
						{
							if (upgradeTraitsData.RemodeValues != null && upgradeTraitsData.ThisRemodeParamIndex.TryGetValue(upgradeTraitsData.Identifier, out var value))
							{
								ActorModel actor = base.Actor;
								string identifier = upgradeTraitsData.Identifier;
								FixedPoint constructionMultiplier = upgradeTraitsData.ConstructionMultiplier;
								List<int> remodeValues = upgradeTraitsData.RemodeValues;
								List<int> remodeIndex = value;
								actor.AddTrait(identifier, constructionMultiplier, doNotInstantiateTrait: false, null, "", remodeIndex, remodeValues);
							}
							else
							{
								base.Actor.AddTrait(upgradeTraitsData.Identifier, upgradeTraitsData.ConstructionMultiplier);
							}
							dictionary.Add(upgradeTraitsData.Identifier, upgradeTraitsData.ConstructionMultiplier);
						}
					}
				}
				if (base.Actor != null && !base.Actor.HasGainedExtraAP)
				{
					if (!base.Actor.MoveCompleted)
					{
						if (base.Actor.HasAnyLevelTrait("SupportTalent_NoMoveHitrate"))
						{
							base.Actor.SupportTalent_NoMoveHitrateFlag = true;
						}
						else
						{
							base.Actor.SupportTalent_NoMoveHitrateFlag = false;
						}
						if (base.Actor.HasAnyLevelTrait("SupportTalent_NoMoveCritRate"))
						{
							base.Actor.SupportTalent_NoMoveCritRateFlag = true;
						}
						else
						{
							base.Actor.SupportTalent_NoMoveCritRateFlag = false;
						}
					}
					else
					{
						base.Actor.SupportTalent_NoMoveHitrateFlag = false;
						base.Actor.SupportTalent_NoMoveCritRateFlag = false;
					}
				}
				else
				{
					base.Actor.SupportTalent_NoMoveHitrateFlag = false;
					base.Actor.SupportTalent_NoMoveCritRateFlag = false;
				}
				(manager as TWDModelManager).ExecuteAction(new AbilityAfterAddActiveTraitAction(base.Actor, TargetCell));
				base.Actor?.UnityOutputCurrentTraits("After add ActiveTraits");
				AbilityResult abilityResult = combatModel.manager.Player.AbilityManager.PerformAbility(base.Actor, Ability, TargetCell, null, resolvedRolls, OOTType, IsAssistAttack, IsTriggerExtraAttackDamage);
				(manager as TWDModelManager).ExecuteAction(new AbilityBeforeRemoveActiveTraitAction(base.Actor, TargetCell, this));
				if (!SkipActiveWeaponTraits)
				{
					foreach (KeyValuePair<string, FixedPoint> item4 in dictionary)
					{
						base.Actor.RemoveTrait(item4.Key);
					}
					List<TraitDefinition> activeTraits2 = equipmentItemModel.GetActiveTraits();
					if (activeTraits2 != null && activeTraits2.Count > 0)
					{
						foreach (TraitDefinition item5 in activeTraits2)
						{
							base.Actor.RemoveTrait(item5.Identifier);
						}
					}
				}
				equipmentItemModel.RemoveTemporaryTraitsByExpirationType(TraitExpirationType.Activation);
				if (TargetActor != null && TargetActor.AIController != null && (TargetActor.IsDead || TargetActor.AIController.IsActorIncapacitated))
				{
					TargetActor.EndAction();
				}
				if (abilityResult == AbilityResult.Success && OOTType != OOTType.None && OOTType != OOTType.PassByAttack && OOTType != OOTType.Revenge && OOTType != OOTType.PreEmptiveStrike && OOTType != OOTType.ParryRiposteRetaliation && OOTType != OOTType.FreeShooting)
				{
					base.Actor.SetOOTPerformed(OOTType);
					if (OOTType == OOTType.Retaliation)
					{
						base.Actor.NotifyChange("AbilityVisited", new object[2] { "Retaliate", false });
					}
				}
				if (combatModel.manager.CurrentCommandLogEntry != null)
				{
					combatModel.manager.CurrentCommandLogEntry.EndAbilityLog(abilityResult);
				}
				return abilityResult == AbilityResult.Success;
			}
			manager.Debug.LogError("AbilityAction::Execute() failed -> CombatModel is null");
			return false;
		}

		protected virtual AbilityModel GetOOTValidationAbility()
		{
			return base.Actor.SelectedAbility;
		}

		public override string ToString()
		{
			return "SourceActor = " + ((base.Actor != null) ? base.Actor.DebugInfo : "null") + ", TargetActor = " + ((TargetActor != null) ? TargetActor.DebugInfo : "null") + ", AbilityID = " + Ability.DefinitionID + " TargetCell = " + TargetCell.ToString() + " OOTType = " + OOTType;
		}
	}
}
