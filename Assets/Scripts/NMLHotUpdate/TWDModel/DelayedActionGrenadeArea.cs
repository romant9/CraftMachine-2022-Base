using System.Collections.Generic;

namespace TWDModel
{
	public class DelayedActionGrenadeArea : CombatAreaSingleGrid
	{
		public ActorModel Owner;

		public int DetonateTurn;

		public int ExplosionRadius;

		public FixedPoint PanelDamagePercent;

		public FixedPoint MaxHpDamagePercent;

		public FixedPoint OnFlameTrapExtraPercent;

		public FixedPoint FlameTrapChancePercent;

		public int FlameTrapTurns;

		public FixedPoint FlameTrapInjuryPercent;

		public List<string> SelfTraitsApply;

		public List<string> TargetTraitsApply;

		public override CombatAreaType Type => CombatAreaType.DelayedActionGrenade;

		public DelayedActionGrenadeArea()
		{
		}

		public DelayedActionGrenadeArea(ActorModel owner, GridCoordinate targetGridCoordinate, Faction faction, int expiryTurn, int detonateTurn, int explosionRadius, FixedPoint panelDamagePercent, FixedPoint maxHpDamagePercent, FixedPoint onFlameTrapExtraPercent, FixedPoint flameTrapChancePercent, int flameTrapTurns, FixedPoint flameTrapInjuryPercent, List<string> selfTraitsApply, List<string> targetTraitsApply)
			: base(targetGridCoordinate, faction, expiryTurn, targetGridCoordinate)
		{
			Owner = owner;
			DetonateTurn = detonateTurn;
			ExplosionRadius = explosionRadius;
			PanelDamagePercent = panelDamagePercent;
			MaxHpDamagePercent = maxHpDamagePercent;
			OnFlameTrapExtraPercent = onFlameTrapExtraPercent;
			FlameTrapChancePercent = flameTrapChancePercent;
			FlameTrapTurns = flameTrapTurns;
			FlameTrapInjuryPercent = flameTrapInjuryPercent;
			SelfTraitsApply = ((selfTraitsApply != null) ? new List<string>(selfTraitsApply) : new List<string>());
			TargetTraitsApply = ((targetTraitsApply != null) ? new List<string>(targetTraitsApply) : new List<string>());
		}

		public DelayedActionGrenadeArea(DelayedActionGrenadeArea area)
			: base(area)
		{
			Owner = area.Owner;
			DetonateTurn = area.DetonateTurn;
			ExplosionRadius = area.ExplosionRadius;
			PanelDamagePercent = area.PanelDamagePercent;
			MaxHpDamagePercent = area.MaxHpDamagePercent;
			OnFlameTrapExtraPercent = area.OnFlameTrapExtraPercent;
			FlameTrapChancePercent = area.FlameTrapChancePercent;
			FlameTrapTurns = area.FlameTrapTurns;
			FlameTrapInjuryPercent = area.FlameTrapInjuryPercent;
			SelfTraitsApply = ((area.SelfTraitsApply != null) ? new List<string>(area.SelfTraitsApply) : new List<string>());
			TargetTraitsApply = ((area.TargetTraitsApply != null) ? new List<string>(area.TargetTraitsApply) : new List<string>());
		}

		public static List<TraitApplyEntry> ParseTraitsApply(List<string> rawEntries)
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
	}
}
