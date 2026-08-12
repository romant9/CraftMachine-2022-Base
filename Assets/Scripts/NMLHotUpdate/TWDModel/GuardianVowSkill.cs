using Newtonsoft.Json;

namespace TWDModel
{
	public sealed class GuardianVowSkill : BaseCommandSkill
	{
		[JsonIgnore]
		private GuardianVowPursuitTrait pursuitTrait;

		[JsonIgnore]
		private GuardianVowTransferTrait transferTrait;

		[JsonIgnore]
		private GuardianVowChargeRefreshTrait chargeRefreshTrait;

		public FixedPoint ChargeGain { get; private set; }

		public int ChargeAttackMaxTimes { get; private set; }

		public int DurationTurns { get; private set; }

		public FixedPoint PursuitChance { get; private set; }

		public int PursuitMaxTimes { get; private set; }

		public int GuardRange { get; private set; }

		public FixedPoint TransferRatio { get; private set; }

		public FixedPoint TransferReduction { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillGuardianVow;

		public GuardianVowSkill()
		{
		}

		public GuardianVowSkill(GuardianVowSkill skill)
			: base(skill)
		{
			ChargeGain = skill.ChargeGain;
			ChargeAttackMaxTimes = skill.ChargeAttackMaxTimes;
			DurationTurns = skill.DurationTurns;
			PursuitChance = skill.PursuitChance;
			PursuitMaxTimes = skill.PursuitMaxTimes;
			GuardRange = skill.GuardRange;
			TransferRatio = skill.TransferRatio;
			TransferReduction = skill.TransferReduction;
		}

		public GuardianVowSkill(FixedPoint chargeGain, int chargeAttackMaxTimes, int durationTurns, FixedPoint pursuitChance, int pursuitMaxTimes, int guardRange, FixedPoint transferRatio, FixedPoint transferReduction)
		{
			ChargeGain = chargeGain;
			ChargeAttackMaxTimes = chargeAttackMaxTimes;
			DurationTurns = durationTurns;
			PursuitChance = pursuitChance;
			PursuitMaxTimes = pursuitMaxTimes;
			GuardRange = guardRange;
			TransferRatio = transferRatio;
			TransferReduction = transferReduction;
		}

		public override bool CanExecute(GridCoordinate targetCell)
		{
			bool num = base.CanExecute(targetCell);
			CombatModel combatModel = base.manager.CombatModel;
			ActorModel actorModel = combatModel?.GetOccupier(targetCell);
			bool flag = base.OwnActorModel != null && base.OwnActorModel.IsStunned;
			bool flag2 = base.OwnActorModel != null && base.OwnActorModel.IsStruggling;
			if (num && combatModel != null && actorModel != null && !actorModel.IsDead && !flag)
			{
				return !flag2;
			}
			return false;
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			if (base.OwnActorModel == null || base.manager == null)
			{
				return;
			}
			CombatModel combatModel = base.manager.CombatModel;
			if (combatModel == null)
			{
				return;
			}
			ActorModel occupier = combatModel.GetOccupier(targetCell);
			if (occupier != null && !base.OwnActorModel.IsDead && !occupier.IsDead)
			{
				combatModel.BindGuardianVow(base.OwnActorModel, occupier, DurationTurns, ChargeAttackMaxTimes, ChargeGain);
				if (ChargeGain > 0L)
				{
					occupier.AddChargePoints((int)ChargeGain);
				}
				base.OwnActorModel.NotifyChange("GuardianVowSkill");
				occupier.NotifyChange("GuardianVowSkill");
			}
		}

		public override void PostExecute(GridCoordinate targetCell)
		{
			base.PostExecute(targetCell);
		}

		public override void Start()
		{
			base.Start();
			RegisterPursuitTrait();
			RegisterTransferTrait();
			RegisterChargeRefreshTrait();
		}

		private void RegisterPursuitTrait()
		{
			if (base.OwnActorModel == null || base.OwnActorModel.Modifiers == null)
			{
				return;
			}
			int count = base.OwnActorModel.Modifiers.GetCount();
			for (int i = 0; i < count; i++)
			{
				if (base.OwnActorModel.Modifiers.GetModifier(i) is GuardianVowPursuitTrait guardianVowPursuitTrait)
				{
					guardianVowPursuitTrait.RebindSkill(this);
					pursuitTrait = guardianVowPursuitTrait;
					return;
				}
			}
			pursuitTrait = new GuardianVowPursuitTrait(this);
			base.OwnActorModel.Modifiers.RegisterModifier(pursuitTrait);
		}

		private void RegisterTransferTrait()
		{
			if (base.OwnActorModel == null || base.OwnActorModel.Modifiers == null)
			{
				return;
			}
			int count = base.OwnActorModel.Modifiers.GetCount();
			for (int i = 0; i < count; i++)
			{
				if (base.OwnActorModel.Modifiers.GetModifier(i) is GuardianVowTransferTrait guardianVowTransferTrait)
				{
					guardianVowTransferTrait.RebindSkill(this);
					transferTrait = guardianVowTransferTrait;
					return;
				}
			}
			transferTrait = new GuardianVowTransferTrait(this);
			base.OwnActorModel.Modifiers.RegisterModifier(transferTrait);
		}

		private void RegisterChargeRefreshTrait()
		{
			if (base.OwnActorModel == null || base.OwnActorModel.Modifiers == null)
			{
				return;
			}
			int count = base.OwnActorModel.Modifiers.GetCount();
			for (int i = 0; i < count; i++)
			{
				if (base.OwnActorModel.Modifiers.GetModifier(i) is GuardianVowChargeRefreshTrait guardianVowChargeRefreshTrait)
				{
					guardianVowChargeRefreshTrait.RebindSkill(this);
					chargeRefreshTrait = guardianVowChargeRefreshTrait;
					return;
				}
			}
			chargeRefreshTrait = new GuardianVowChargeRefreshTrait(this);
			base.OwnActorModel.Modifiers.RegisterModifier(chargeRefreshTrait);
		}
	}
}
