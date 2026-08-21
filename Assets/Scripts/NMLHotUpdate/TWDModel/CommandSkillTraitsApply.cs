using System.Collections.Generic;

namespace TWDModel
{
	public static class CommandSkillTraitsApply
	{
		public static List<TraitApplyEntry> Parse(List<string> rawEntries)
		{
			List<TraitApplyEntry> list = new List<TraitApplyEntry>();
			if (rawEntries == null)
			{
				return list;
			}
			foreach (string rawEntry in rawEntries)
			{
				if (string.IsNullOrEmpty(rawEntry))
				{
					continue;
				}
				string[] array = rawEntry.Split(':');
				string text = array[0].Trim();
				if (!string.IsNullOrEmpty(text))
				{
					int result = 0;
					if (array.Length > 1)
					{
						int.TryParse(array[1].Trim(), out result);
					}
					int result2 = 0;
					if (array.Length > 2)
					{
						int.TryParse(array[2].Trim(), out result2);
					}
					list.Add(new TraitApplyEntry
					{
						TraitId = text,
						Chance = result,
						Turns = result2
					});
				}
			}
			return list;
		}

		public static List<string> ApplyToSelf(ActorModel caster, List<string> rawEntries, long duration = 0L)
		{
			List<string> list = new List<string>();
			if (caster == null || caster.IsDead)
			{
				return list;
			}
			foreach (TraitApplyEntry item in Parse(rawEntries))
			{
				if (!caster.HasAnyLevelTrait(UpgradeTraitsData.StripTraitLevelIdentifier(item.TraitId)))
				{
					caster.AddTemporaryTrait(item.TraitId, default(FixedPoint), item.HasChanceOverride ? new FixedPoint?(item.Chance) : ((FixedPoint?)null), duration);
					if (caster.HasTrait(item.TraitId))
					{
						list.Add(item.TraitId);
					}
				}
			}
			return list;
		}

		public static List<string> ApplyToTarget(TWDModelManager manager, ActorModel target, List<string> rawEntries)
		{
			List<string> list = new List<string>();
			if (target == null || target.IsDead)
			{
				return list;
			}
			foreach (TraitApplyEntry item in Parse(rawEntries))
			{
				if ((manager?.Player == null || manager.Player.RollDice(RollDiceType.GainTrait, (int)item.Chance) != PlayerRandomChanceResult.Failed) && !target.HasAnyLevelTrait(UpgradeTraitsData.StripTraitLevelIdentifier(item.TraitId)))
				{
					target.AddTemporaryTrait(item.TraitId, default(FixedPoint), null, item.HasTurns ? item.Turns : 0);
					if (target.HasTrait(item.TraitId))
					{
						list.Add(item.TraitId);
					}
				}
			}
			return list;
		}

		public static void Remove(ActorModel actor, List<string> grantedTraitIds)
		{
			if (actor != null && grantedTraitIds != null)
			{
				for (int i = 0; i < grantedTraitIds.Count; i++)
				{
					actor.RemoveTrait(grantedTraitIds[i]);
				}
			}
		}
	}
}
