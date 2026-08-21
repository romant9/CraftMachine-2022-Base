using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityLog
	{
		public ActorModel Actor;

		public AbilityModel Ability;

		public List<TargetActionLog> Dodges;

		public List<TargetActionLog> Jumpingshots;

		public List<TargetActionLog> StunAvoids;

		public List<TargetActionLog> HerdAvoids;

		public List<TargetActionLog> SecondChances;

		public List<TraitLog> TempTraits;

		public List<EffectLog> Effects;

		public List<DiceRollLog> DiceRolls;

		public List<DamageLog> Damages;

		public AbilityResult Result;

		public AbilityLog(ActorModel actor, AbilityModel ability)
		{
			Actor = actor;
			Ability = ability;
		}

		public void AddTempTrait(string trait, FixedPoint multiplier)
		{
			if (TempTraits == null)
			{
				TempTraits = new List<TraitLog>();
			}
			TempTraits.Add(new TraitLog
			{
				Trait = trait,
				Param = multiplier
			});
		}

		public void StartEffect(string effectType)
		{
			if (Effects == null)
			{
				Effects = new List<EffectLog>();
			}
			Effects.Add(new EffectLog
			{
				EffectType = effectType,
				Success = false
			});
		}

		public void EndEffect(bool success)
		{
			if (Effects != null && Effects.Count > 0)
			{
				Effects[Effects.Count - 1].Success = success;
			}
		}

		public void AddModifier(ModifierLog modifierLog)
		{
			if (Effects != null && Effects.Count > 0)
			{
				int index = Effects.Count - 1;
				Effects[index].CreateModifiers();
				Effects[index].ModifierLogs.Add(modifierLog);
			}
		}

		public void AddDiceRoll(DiceRollLog diceRollLog)
		{
			if (DiceRolls == null)
			{
				DiceRolls = new List<DiceRollLog>();
			}
			DiceRolls.Add(diceRollLog);
		}

		public void AddDamage(DamageLog damageLog)
		{
			if (Damages == null)
			{
				Damages = new List<DamageLog>();
			}
			Damages.Add(damageLog);
		}

		public void Dodge(ActorModel source, ActorModel target)
		{
			if (Dodges == null)
			{
				Dodges = new List<TargetActionLog>();
			}
			Dodges.Add(new TargetActionLog
			{
				Source = source,
				Target = target,
				Name = "Dodge"
			});
		}

		public void Jumpingshot(ActorModel source, ActorModel target)
		{
			if (Jumpingshots == null)
			{
				Jumpingshots = new List<TargetActionLog>();
			}
			Jumpingshots.Add(new TargetActionLog
			{
				Source = source,
				Target = target,
				Name = "Jumpingshot"
			});
		}

		public void SecondChance(ActorModel source, ActorModel target)
		{
			if (SecondChances == null)
			{
				SecondChances = new List<TargetActionLog>();
			}
			SecondChances.Add(new TargetActionLog
			{
				Source = source,
				Target = target,
				Name = "SecondChance"
			});
		}

		public void StunAvoided(ActorModel source, ActorModel target)
		{
			if (StunAvoids == null)
			{
				StunAvoids = new List<TargetActionLog>();
			}
			StunAvoids.Add(new TargetActionLog
			{
				Source = source,
				Target = target,
				Name = "StunAvoid"
			});
		}

		public void HerdAvoided(ActorModel source, ActorModel target)
		{
			if (HerdAvoids == null)
			{
				HerdAvoids = new List<TargetActionLog>();
			}
			HerdAvoids.Add(new TargetActionLog
			{
				Source = source,
				Target = target,
				Name = "HerdAvoid"
			});
		}

		public override string ToString()
		{
			string text = Ability.GetType().Name + " for '" + Actor.Name + "' Result = " + Result;
			if (TempTraits != null && TempTraits.Count > 0)
			{
				text += "\n\nTempTraits (";
				for (int i = 0; i < TempTraits.Count; i++)
				{
					text += TempTraits[i].ToString();
					if (i < TempTraits.Count - 1)
					{
						text += ", ";
					}
				}
				text += ")";
			}
			if (DiceRolls != null && DiceRolls.Count > 0)
			{
				text += "\n\nDiceRolls\n";
				for (int j = 0; j < DiceRolls.Count; j++)
				{
					text = text + "\t" + DiceRolls[j];
					if (j < DiceRolls.Count - 1)
					{
						text += "\n";
					}
				}
			}
			if (Effects != null && Effects.Count > 0)
			{
				text += "\n\nEffects\n";
				for (int k = 0; k < Effects.Count; k++)
				{
					text += Effects[k];
					if (k < Effects.Count - 1)
					{
						text += "\n";
					}
				}
			}
			if (Damages != null && Damages.Count > 0)
			{
				text += "\n\nDamages\n";
				for (int l = 0; l < Damages.Count; l++)
				{
					text = text + "\t" + Damages[l];
					if (l < Damages.Count - 1)
					{
						text += "\n\n";
					}
				}
			}
			if (Dodges != null && Dodges.Count > 0)
			{
				text += "\n\nDodges\n";
				for (int m = 0; m < Dodges.Count; m++)
				{
					text = text + "\t" + Dodges[m].ToString();
					if (m < Dodges.Count - 1)
					{
						text += "\n";
					}
				}
				text += ")";
			}
			if (StunAvoids != null && StunAvoids.Count > 0)
			{
				text += "\n\nStunAvoids\n";
				for (int n = 0; n < StunAvoids.Count; n++)
				{
					text = text + "\t" + StunAvoids[n].ToString();
					if (n < StunAvoids.Count - 1)
					{
						text += "\n";
					}
				}
				text += ")";
			}
			if (SecondChances != null && SecondChances.Count > 0)
			{
				text += "\n\nSecondChances\n";
				for (int num = 0; num < SecondChances.Count; num++)
				{
					text = text + "\t" + SecondChances[num].ToString();
					if (num < SecondChances.Count - 1)
					{
						text += "\n";
					}
				}
				text += ")";
			}
			return text;
		}
	}
}
