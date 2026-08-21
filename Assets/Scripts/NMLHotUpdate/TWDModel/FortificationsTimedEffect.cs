using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public class FortificationsTimedEffect : CoexistTimedEffectAbstract
	{
		public List<string> GrantedTraitIds { get; private set; }

		public int SourceSkillID { get; private set; }

		public bool Interrupted { get; private set; }

		[JsonIgnore]
		public int CooldownOverride { get; private set; } = -1;

		[JsonIgnore]
		public ActorModel Holder => (base.Target as ActorModel) ?? base.Instigator;

		[JsonIgnore]
		public int LeftTurns => base.Duration - base.Counter;

		public FortificationsTimedEffect()
		{
			GrantedTraitIds = new List<string>();
		}

		public FortificationsTimedEffect(FortificationsTimedEffect other)
			: base(other)
		{
			GrantedTraitIds = ((other.GrantedTraitIds != null) ? new List<string>(other.GrantedTraitIds) : new List<string>());
			SourceSkillID = other.SourceSkillID;
			Interrupted = other.Interrupted;
		}

		public FortificationsTimedEffect(int duration, ActorModel holder, int sourceSkillID, List<string> grantedTraitIds)
			: base(CoexistTimedEffectType.Fortifications, duration, 0, holder, holder)
		{
			SourceSkillID = sourceSkillID;
			GrantedTraitIds = ((grantedTraitIds != null) ? new List<string>(grantedTraitIds) : new List<string>());
		}

		public void MarkInterrupted()
		{
			Interrupted = true;
		}

		public void SetCooldownOverride(int cooldownTurns)
		{
			CooldownOverride = ((cooldownTurns >= 0) ? cooldownTurns : (-1));
		}

		public override void PostNewTimedEffect()
		{
			NotifyHolder();
		}

		public override void UpdateTimedEffect(CoexistTimedEffectAbstract newTimedEffect)
		{
			if (!(newTimedEffect is FortificationsTimedEffect fortificationsTimedEffect))
			{
				return;
			}
			base.Duration = fortificationsTimedEffect.Duration;
			base.Counter = 0;
			Interrupted = false;
			CooldownOverride = -1;
			SourceSkillID = fortificationsTimedEffect.SourceSkillID;
			if (GrantedTraitIds == null)
			{
				GrantedTraitIds = new List<string>();
			}
			if (fortificationsTimedEffect.GrantedTraitIds != null)
			{
				foreach (string grantedTraitId in fortificationsTimedEffect.GrantedTraitIds)
				{
					if (!GrantedTraitIds.Contains(grantedTraitId))
					{
						GrantedTraitIds.Add(grantedTraitId);
					}
				}
			}
			NotifyHolder();
		}

		public override void PostFinishTimedEffect()
		{
			ActorModel holder = Holder;
			CommandSkillTraitsApply.Remove(holder, GrantedTraitIds);
			GrantedTraitIds?.Clear();
			CombatModel combatModel = ((base.manager != null) ? base.manager.CombatModel : null);
			if (combatModel != null)
			{
				FortificationsCoverModel.RemoveByOwner(combatModel, holder);
			}
			FortificationsSkill.FindSkill(holder, SourceSkillID)?.EnterCooldownOnStateEnd(!Interrupted, CooldownOverride);
			NotifyHolder();
		}

		private void NotifyHolder()
		{
			Holder?.NotifyChange("FortificationsStateChanged", this);
		}
	}
}
