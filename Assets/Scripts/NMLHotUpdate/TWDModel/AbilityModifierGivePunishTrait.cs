using System;
using System.Collections.Generic;

namespace TWDModel
{
	public class AbilityModifierGivePunishTrait : ParameterModifier
	{
		private string traitIdentifier;

		private FixedPoint chance;

		private FixedPoint count;

		public static string RollForPunishTrait = "RollForPunishTrait";

		public AbilityModifierGivePunishTrait()
		{
		}

		public AbilityModifierGivePunishTrait(string identifier, FixedPoint traitChance, FixedPoint traitCount)
		{
			traitIdentifier = identifier;
			chance = traitChance;
			count = traitCount;
		}

		public override bool VisitParameter(string paramName, ref FixedPoint value, ActorModel actor)
		{
			if (paramName == RollForPunishTrait)
			{
				try
				{
					int damage = (int)value;
					if (base.manager.GameEconomyData.GetTraitDefinition(traitIdentifier) != null)
					{
						if (base.manager.Player.RollDice(RollDiceType.GainTrait, chance, 0.0) != PlayerRandomChanceResult.Failed)
						{
							if (traitIdentifier == "Burning")
							{
								base.manager.ExecuteAction(new BurningOutAction(null, actor, onRedHealthBar: false, null, () => damage));
								List<ActorModel> neighbors = GetNeighbors(actor);
								if (neighbors != null)
								{
									for (int num = 0; num < neighbors.Count; num++)
									{
										base.manager.ExecuteAction(new BurningOutAction(null, neighbors[num], onRedHealthBar: false, null, () => damage));
									}
								}
							}
							else
							{
								actor.AddTrait(traitIdentifier);
							}
						}
						return true;
					}
					base.manager.Debug.LogWarning("AbilityModifierGivePunishTrait: Tried to give a trait '" + traitIdentifier + "', but could not find TraitDefinition for it!");
				}
				catch (Exception arg)
				{
					base.Debug.LogError($"AbilityModifierGivePunishTrait fail:{arg}");
				}
			}
			return false;
		}

		public override string[] GetParameterNames()
		{
			return new string[1] { RollForPunishTrait };
		}

		private List<ActorModel> GetNeighbors(ActorModel actor)
		{
			CombatModel combatModel = actor.manager.CombatModel;
			EnumerableNeighbors enumerableNeighbors = combatModel.Grid.Neighbors(actor.GridCoordinate);
			List<ActorModel> list = new List<ActorModel>();
			foreach (GridCoordinate item in enumerableNeighbors)
			{
				ActorModel occupier = combatModel.GetOccupier(item);
				if (occupier != null && IsEnemy(occupier) && !occupier.Definition.IsEnvironmental)
				{
					list.Add(occupier);
				}
			}
			FixedPoint fixedPoint = list.Count - count;
			if (fixedPoint <= 0L)
			{
				fixedPoint = 0L;
			}
			for (int i = 0; i < fixedPoint; i++)
			{
				combatModel.manager.Player.PlayerRandom.GetRandomElement(list, remove: true);
			}
			return list;
		}

		private bool IsEnemy(ActorModel actor)
		{
			if (actor.Faction != Faction.Walker && actor.Faction != Faction.Raider && actor.Faction != Faction.Dormant)
			{
				return actor.Faction == Faction.Environmental;
			}
			return true;
		}
	}
}
