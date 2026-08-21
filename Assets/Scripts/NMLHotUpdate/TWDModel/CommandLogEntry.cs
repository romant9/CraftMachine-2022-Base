using System.Collections.Generic;
using BaseModel;

namespace TWDModel
{
	public class CommandLogEntry
	{
		public ModelCommand Command;

		public int ServerResponseCode = -1;

		public bool Success;

		public IModelObject ModelObject;

		public List<ActionLog> Actions;

		public List<ActionLog> ActionLogStack;

		public ActionLog CurrentActionLog;

		public DamageLog CurrentDamage;

		public override string ToString()
		{
			string text = "";
			if (Command != null)
			{
				text = text + Command.SequenceId + "\t" + Command.GetType().Name;
				if (ModelObject != null)
				{
					text = text + "\n\t" + ModelObject.GetType().Name + " [ModelId = " + ModelObject.ModelId + "]";
					text = text + "\n\t" + ModelObject.ToString();
				}
			}
			return text;
		}

		public string GetDetails()
		{
			string text = "";
			if (Actions != null)
			{
				for (int i = 0; i < Actions.Count; i++)
				{
					text += Actions[i].ToString();
				}
			}
			return text;
		}

		public void StartAbilityLog(ActorModel actor, AbilityModel ability)
		{
			if (CurrentActionLog != null)
			{
				CurrentActionLog.StartAbilityLog(actor, ability);
			}
		}

		public void EndAbilityLog(AbilityResult result)
		{
			if (CurrentActionLog != null)
			{
				CurrentActionLog.EndAbilityLog(result);
			}
		}

		public void AddTempTrait(string trait, FixedPoint multiplier)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.AddTempTrait(trait, multiplier);
			}
		}

		public void StartEffect(string effectType)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.StartEffect(effectType);
			}
		}

		public void EndEffect(bool success)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.EndEffect(success);
			}
		}

		public void StartExecuteAction(ModelAction action)
		{
			CurrentActionLog = new ActionLog
			{
				Action = action
			};
			if (ActionLogStack != null && ActionLogStack.Count > 0)
			{
				if (ActionLogStack[ActionLogStack.Count - 1].NestedActions == null)
				{
					ActionLogStack[ActionLogStack.Count - 1].NestedActions = new List<ActionLog>();
				}
				ActionLogStack[ActionLogStack.Count - 1].NestedActions.Add(CurrentActionLog);
			}
			if (ActionLogStack == null)
			{
				ActionLogStack = new List<ActionLog>();
			}
			ActionLogStack.Add(CurrentActionLog);
		}

		public void EndExecuteAction(bool success)
		{
			ActionLog currentActionLog = CurrentActionLog;
			if (CurrentActionLog != null)
			{
				CurrentActionLog.Success = success;
			}
			if (ActionLogStack != null && ActionLogStack.Count > 0)
			{
				ActionLogStack.RemoveAt(ActionLogStack.Count - 1);
				CurrentActionLog = ((ActionLogStack.Count > 0) ? ActionLogStack[ActionLogStack.Count - 1] : null);
			}
			else
			{
				CurrentActionLog = null;
			}
			if ((ActionLogStack == null || ActionLogStack.Count == 0) && currentActionLog != null)
			{
				if (Actions == null)
				{
					Actions = new List<ActionLog>();
				}
				Actions.Add(currentActionLog);
			}
		}

		public void ActionModifier(ActorModel actor, ActionModifier modifier, ActionListClearFlag clearFlag, List<ModelAction> visitAddedActions)
		{
			if (CurrentActionLog != null)
			{
				CurrentActionLog.Modifier(actor, modifier, clearFlag, visitAddedActions);
			}
		}

		public void ParameterModifiedAbilityActive(string paramName, FixedPoint oldValue, FixedPoint newValue, ActorModel actor, AbilityModel ability)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				ModifierLog modifierLog = new ModifierLog
				{
					Ability = ability,
					Actor = actor,
					NewValue = newValue,
					OldValue = oldValue,
					ParamName = paramName,
					Passive = false
				};
				CurrentActionLog.CurrentAbilityLog.AddModifier(modifierLog);
			}
		}

		public void ParameterModifiedAbilityPassive(string paramName, FixedPoint oldValue, FixedPoint newValue, ActorModel actor, AbilityModel ability)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				ModifierLog modifierLog = new ModifierLog
				{
					Ability = ability,
					Actor = actor,
					NewValue = newValue,
					OldValue = oldValue,
					ParamName = paramName,
					Passive = true
				};
				CurrentActionLog.CurrentAbilityLog.AddModifier(modifierLog);
			}
		}

		public void ParameterModifiedActorPassive(string paramName, FixedPoint oldValue, FixedPoint newValue, ActorModel actor)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				ModifierLog modifierLog = new ModifierLog
				{
					Ability = null,
					Actor = actor,
					NewValue = newValue,
					OldValue = oldValue,
					ParamName = paramName,
					Passive = true
				};
				CurrentActionLog.CurrentAbilityLog.AddModifier(modifierLog);
			}
		}

		public void ParameterModifiedFactionPassive(string paramName, FixedPoint oldValue, FixedPoint newValue, ActorModel actor)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				ModifierLog modifierLog = new ModifierLog
				{
					Ability = null,
					Actor = actor,
					NewValue = newValue,
					OldValue = oldValue,
					ParamName = paramName,
					Passive = true
				};
				CurrentActionLog.CurrentAbilityLog.AddModifier(modifierLog);
			}
		}

		public void ParameterModifiedGlobalPassive(string paramName, FixedPoint oldValue, FixedPoint newValue, ActorModel actor)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				ModifierLog modifierLog = new ModifierLog
				{
					Ability = null,
					Actor = actor,
					NewValue = newValue,
					OldValue = oldValue,
					ParamName = paramName,
					Passive = true
				};
				CurrentActionLog.CurrentAbilityLog.AddModifier(modifierLog);
			}
		}

		public void RollDice(FixedPoint successProbability, FixedPoint successProbabilityExtension, FixedPoint rnd, FixedPoint extendedLimit, PlayerRandomChanceResult result, RollDiceType type)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.AddDiceRoll(new FloatDiceRollLog
				{
					SuccessProbability = successProbability,
					SuccessProbabilityExtension = successProbabilityExtension,
					Roll = rnd,
					Result = result,
					RollDiceType = type
				});
			}
		}

		public void RollDice(int chance, int chanceExtension, int d100, PlayerRandomChanceResult result, RollDiceType type)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.AddDiceRoll(new IntDiceRollLog
				{
					SuccessProbability = chance,
					SuccessProbabilityExtension = chanceExtension,
					Roll = d100,
					Result = result,
					RollDiceType = type
				});
			}
		}

		public void RollDice(int roll, int max, RollDiceType type)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.AddDiceRoll(new RangedDiceRoll
				{
					Min = 0,
					Max = max,
					Roll = roll,
					RollDiceType = type
				});
			}
		}

		public void RollDice(int roll, int min, int max, RollDiceType type)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.AddDiceRoll(new RangedDiceRoll
				{
					Min = min,
					Max = max,
					Roll = roll,
					RollDiceType = type
				});
			}
		}

		public void StunAvoided(ActorModel source, ActorModel target)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.StunAvoided(source, target);
			}
		}

		public void HerdAvoided(ActorModel source, ActorModel target)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.HerdAvoided(source, target);
			}
		}

		public void Dodge(ActorModel source, ActorModel target)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.Dodge(source, target);
			}
		}

		public void Jumpingshot(ActorModel source, ActorModel target)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.Jumpingshot(source, target);
			}
		}

		public void SecondChance(ActorModel source, ActorModel target)
		{
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.SecondChance(source, target);
			}
		}

		public void CalculateDamageStart(FixedPoint baseDamage)
		{
			CurrentDamage = new DamageLog();
			CurrentDamage.BaseDamage = baseDamage;
		}

		public void CalculateDamageVariation(FixedPoint modifiedDamage)
		{
			CurrentDamage.AfterDamageVariation = modifiedDamage;
		}

		public void CalculateDamageTypeModified(FixedPoint modifiedDamage, DamageType damageType, FixedPoint typeMultiplier, FixedPoint additionalTypeDamage)
		{
			CurrentDamage.DamageType = damageType;
			CurrentDamage.DamageTypeMultiplier = typeMultiplier;
			CurrentDamage.AdditionalTypeDamage = additionalTypeDamage;
			CurrentDamage.AfterTypeModification = modifiedDamage;
		}

		public void CalculateDamageBodyShot(FixedPoint modifiedDamage, FixedPoint bodyShotMultiplier, PlayerRandomChanceResult bodyShotResult)
		{
			CurrentDamage.BodyShotMultiplier = bodyShotMultiplier;
			CurrentDamage.BodyShotResult = bodyShotResult;
			CurrentDamage.AfterBodyShot = modifiedDamage;
		}

		public void CalculateDamageCritical(FixedPoint modifiedDamage, FixedPoint criticalMultiplier, PlayerRandomChanceResult criticalResult)
		{
			CurrentDamage.CriticalResult = criticalResult;
			CurrentDamage.CriticalMultiplier = criticalMultiplier;
			CurrentDamage.AfterCritical = modifiedDamage;
		}

		public void CalculateDamageFinal(FixedPoint modifiedDamage, FixedPoint finalDamageMultiplier)
		{
			CurrentDamage.FinalDamageMultiplier = finalDamageMultiplier;
			CurrentDamage.AfterFinalDamage = modifiedDamage;
		}

		public void CalculateDamageAfterReduction(FixedPoint modifiedDamage, FixedPoint amountDmgReduced, FixedPoint defenseWithCoverMultiplier)
		{
			CurrentDamage.AmountDamageReduced = amountDmgReduced;
			CurrentDamage.DefenseWithCoverMultiplier = defenseWithCoverMultiplier;
			CurrentDamage.AfterDamageReduction = modifiedDamage;
		}

		public void CalculateDamageEnd(int[] damage)
		{
			CurrentDamage.ResultDamage = damage;
			if (CurrentActionLog != null && CurrentActionLog.CurrentAbilityLog != null)
			{
				CurrentActionLog.CurrentAbilityLog.AddDamage(CurrentDamage);
			}
			CurrentDamage = null;
		}
	}
}
