using System.Collections.Generic;

namespace TWDModel
{
	public class ResistNegativeEffectsTrait : ActionModifier
	{
		public const string EffectTagTrapFlame = "TrapFlame";

		private List<string> EffectIndex;

		public ResistNegativeEffectsTrait()
		{
		}

		public ResistNegativeEffectsTrait(List<string> effectIndex)
		{
			EffectIndex = effectIndex;
		}

		public override ActionListClearFlag VisitActions(ModelAction action, ActorModel actor, List<ModelAction> addedActions)
		{
			if (EffectIndex == null || EffectIndex.Count == 0)
			{
				return ActionListClearFlag.Keep;
			}
			for (int i = 0; i < EffectIndex.Count; i++)
			{
				if (EffectIndex[i] == "Stun" && action is StunAction stunAction && ShouldAvoid(actor, stunAction.TargetActor, RollDiceType.AvoidStun))
				{
					stunAction.Avoided = true;
					NotifyResist(stunAction.TargetActor);
				}
				if (EffectIndex[i] == "ABTesterAed" && action is ABTesterAction aBTesterAction && ShouldAvoid(actor, aBTesterAction.TargetActor, RollDiceType.AvoidBurn))
				{
					aBTesterAction.Avoided = true;
					NotifyResist(aBTesterAction.TargetActor);
				}
				if (EffectIndex[i] == "Pitfall" && action is PitfallAction pitfallAction && ShouldAvoid(actor, pitfallAction.TargetActor, RollDiceType.Unknown))
				{
					pitfallAction.Avoided = true;
					NotifyResist(pitfallAction.TargetActor);
				}
				if (EffectIndex[i] == "Burning" && action is BurningOutAction burningOutAction && ShouldAvoid(actor, burningOutAction.TargetActor, RollDiceType.AvoidBurn))
				{
					burningOutAction.Avoided = true;
					NotifyResist(burningOutAction.TargetActor);
				}
				if (EffectIndex[i] == "Disoriented" && action is DisorientAction disorientAction && ShouldAvoid(actor, disorientAction.TargetActor, RollDiceType.Disorient))
				{
					disorientAction.Avoided = true;
					NotifyResist(disorientAction.TargetActor);
				}
				if (EffectIndex[i] == "Cripple" && action is CrippleAction crippleAction && ShouldAvoid(actor, crippleAction.TargetActor, RollDiceType.AvoidCripple))
				{
					crippleAction.Avoided = true;
					NotifyResist(crippleAction.TargetActor);
				}
				if (EffectIndex[i] == "Root" && action is RootAction rootAction && ShouldAvoid(actor, rootAction.TargetActor, RollDiceType.AvoidRoot))
				{
					rootAction.Avoided = true;
					NotifyResist(rootAction.TargetActor);
				}
				if (EffectIndex[i] == "StaggerActive" && action is StaggerAction staggerAction && ShouldAvoid(actor, staggerAction.TargetActor, RollDiceType.AvoidStagger))
				{
					staggerAction.Avoided = true;
					NotifyResist(staggerAction.TargetActor);
				}
				if (EffectIndex[i] == "Skinned" && action is SkinnedAction skinnedAction && ShouldAvoid(actor, skinnedAction.TargetActor, RollDiceType.Skinned))
				{
					skinnedAction.Avoided = true;
					NotifyResist(skinnedAction.TargetActor);
				}
				if (EffectIndex[i] == "Bleeding" && action is BleedingOutAction bleedingOutAction && ShouldAvoid(actor, bleedingOutAction.Target, RollDiceType.AvoidBleed))
				{
					bleedingOutAction.Avoided = true;
					NotifyResist(bleedingOutAction.Target);
				}
				if (EffectIndex[i] == "Taunted" && action is TauntAction tauntAction && ShouldAvoid(actor, tauntAction.TargetActor, RollDiceType.Taunt))
				{
					tauntAction.Avoided = true;
					NotifyResist(tauntAction.TargetActor);
				}
				if (EffectIndex[i] == "Herd" && action is HerdAction herdAction && ShouldAvoid(actor, herdAction.TargetActor, RollDiceType.AvoidHerd))
				{
					herdAction.Avoided = true;
					NotifyResist(herdAction.TargetActor);
				}
				if (EffectIndex[i] == "RemoteWeakened" && action is RemoteWeakenAction remoteWeakenAction && ShouldAvoid(actor, remoteWeakenAction.TargetActor, RollDiceType.Unknown))
				{
					remoteWeakenAction.Avoided = true;
					NotifyResist(remoteWeakenAction.TargetActor);
				}
				if (EffectIndex[i] == "ElectricShock" && action is ElectricShockAction electricShockAction && ShouldAvoid(actor, electricShockAction.TargetActor, RollDiceType.AvoidElectricShock))
				{
					electricShockAction.Avoided = true;
					NotifyResist(electricShockAction.TargetActor);
				}
				if (EffectIndex[i] == "Quantun" && action is QuantunAction quantunAction && ShouldAvoid(actor, quantunAction.TargetActor, RollDiceType.AvoidQuantun))
				{
					quantunAction.Avoided = true;
					NotifyResist(quantunAction.TargetActor);
				}
			}
			return ActionListClearFlag.Keep;
		}

		public static bool TryResist(ActorModel target, string effectTag, RollDiceType diceType = RollDiceType.Disorient)
		{
			if (target == null || string.IsNullOrEmpty(effectTag))
			{
				return false;
			}
			if (!target.HasAnyLevelTrait("ResistNegativeEffects"))
			{
				return false;
			}
			if (!HasConfiguredEffectTag(target, effectTag))
			{
				return false;
			}
			FixedPoint value = 0.0;
			FixedPoint successProbabilityExtension = 0.0;
			AbilityManagerModel abilityManagerModel = target.manager?.CombatModel?.AbilityManager;
			if (abilityManagerModel == null || target.manager.Player == null)
			{
				return false;
			}
			abilityManagerModel.VisitParameter("ResistNegativeEffectsParm1", ref value, target);
			if (target.manager.Player.RollDice(diceType, value, successProbabilityExtension) == PlayerRandomChanceResult.Failed)
			{
				return false;
			}
			NotifyResist(target);
			return true;
		}

		public static bool HasConfiguredEffectTag(ActorModel target, string effectTag)
		{
			if (target?.TraitContainer == null || target.manager?.GameEconomyData == null)
			{
				return false;
			}
			TraitEntry traitAnyLevel = target.TraitContainer.GetTraitAnyLevel("ResistNegativeEffects");
			if (traitAnyLevel == null)
			{
				return false;
			}
			TraitDefinition traitDefinition = target.manager.GameEconomyData.GetTraitDefinition(traitAnyLevel.TraitIdentifier);
			if (traitDefinition?.EffectIndex == null || traitDefinition.EffectIndex.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < traitDefinition.EffectIndex.Count; i++)
			{
				if (traitDefinition.EffectIndex[i] == effectTag)
				{
					return true;
				}
			}
			return false;
		}

		private static bool ShouldAvoid(ActorModel traitHolder, ActorModel target, RollDiceType diceType)
		{
			if (traitHolder != target)
			{
				return false;
			}
			TWDModelManager tWDModelManager = traitHolder.manager;
			AbilityManagerModel abilityManager = tWDModelManager.CombatModel.AbilityManager;
			FixedPoint value = 0.0;
			FixedPoint successProbabilityExtension = 0.0;
			abilityManager.VisitParameter("ResistNegativeEffectsParm1", ref value, traitHolder);
			return tWDModelManager.Player.RollDice(diceType, value, successProbabilityExtension) != PlayerRandomChanceResult.Failed;
		}

		private static void NotifyResist(ActorModel target)
		{
			target?.NotifyChange("AbilityVisited", new object[2] { "ResistNegativeEffects", false });
		}
	}
}
