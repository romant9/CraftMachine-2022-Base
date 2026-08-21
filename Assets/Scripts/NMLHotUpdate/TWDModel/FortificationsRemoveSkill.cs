using System;
using Newtonsoft.Json;

namespace TWDModel
{
	public sealed class FortificationsRemoveSkill : BaseCommandSkill
	{
		public int FortificationsCooldownTurns { get; private set; }

		public FixedPoint HealMaxHealthRatio { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillFortificationsRemove;

		[JsonIgnore]
		protected override bool EnterCooldownOnCast => false;

		public FortificationsRemoveSkill()
		{
		}

		public FortificationsRemoveSkill(FortificationsRemoveSkill skill)
			: base(skill)
		{
			FortificationsCooldownTurns = skill.FortificationsCooldownTurns;
			HealMaxHealthRatio = skill.HealMaxHealthRatio;
		}

		public FortificationsRemoveSkill(int fortificationsCooldownTurns, FixedPoint healMaxHealthRatio)
		{
			FortificationsCooldownTurns = Math.Max(0, fortificationsCooldownTurns);
			HealMaxHealthRatio = healMaxHealthRatio;
		}

		public override bool CanExecute(GridCoordinate targetCell)
		{
			if (base.manager == null || base.manager.GameEconomyData == null || base.OwnActorModel == null || targetCell != base.OwnActorModel.GridCoordinate || !base.OwnActorModel.IsInFortifications)
			{
				return false;
			}
			CommandSkillDefinition definition = base.Definition;
			if (definition == null || definition.TargetType == null)
			{
				return false;
			}
			return base.CanExecute(targetCell);
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			ActorModel ownActorModel = base.OwnActorModel;
			if (ownActorModel == null || ownActorModel.FortificationsTimedEffect == null)
			{
				return;
			}
			ownActorModel.EndFortifications(interrupted: true, FortificationsCooldownTurns);
			if (!ownActorModel.IsInFortifications)
			{
				ownActorModel.NotifyChange("FortificationsRemove");
				int val = (int)(ownActorModel.MaxHitPoints * HealMaxHealthRatio);
				int val2 = Math.Max(0, ownActorModel.MaxHitPoints - ownActorModel.Hitpoints);
				int num = Math.Min(val, val2);
				if (ownActorModel.DebuffReduceRecoveryTimedEffect != null && ownActorModel.DebuffReduceRecoveryTimedEffect.HealReduceAmount >= 100)
				{
					num = 0;
				}
				if (num > 0)
				{
					base.manager?.ExecuteAction(new HealAction(ownActorModel, ownActorModel, num));
				}
			}
		}
	}
}
