using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	public sealed class FortificationsSkill : BaseCommandSkill
	{
		public int DurationTurns { get; private set; }

		public bool SkipNextCooldownReduce { get; private set; }

		public override CommandSkillType Type => CommandSkillType.CommandSkillFortifications;

		[JsonIgnore]
		protected override bool EnterCooldownOnCast => false;

		public FortificationsSkill()
		{
		}

		public FortificationsSkill(FortificationsSkill skill)
			: base(skill)
		{
			DurationTurns = skill.DurationTurns;
			SkipNextCooldownReduce = skill.SkipNextCooldownReduce;
		}

		public FortificationsSkill(int durationTurns)
		{
			DurationTurns = durationTurns;
		}

		public override bool CanExecute(GridCoordinate targetCell)
		{
			if (base.OwnActorModel == null || DurationTurns <= 0 || base.manager?.CombatModel == null || base.OwnActorModel.IsInFortifications)
			{
				return false;
			}
			return base.CanExecute(targetCell);
		}

		public override void OnExecute(GridCoordinate targetCell)
		{
			if (base.OwnActorModel != null && DurationTurns > 0)
			{
				CombatModel combatModel = ((base.manager != null) ? base.manager.CombatModel : null);
				if (combatModel != null)
				{
					FortificationsCoverModel.RemoveByOwner(combatModel, base.OwnActorModel);
					FortificationsCoverModel fortificationsCoverModel = new FortificationsCoverModel(base.OwnActorModel, base.OwnActorModel.GridCoordinate, GetOwnerFacing());
					fortificationsCoverModel.Initialize();
					fortificationsCoverModel.SetManager(base.manager);
					fortificationsCoverModel.Start();
					combatModel.AddModel(fortificationsCoverModel);
					List<string> grantedTraitIds = ApplySelfTraitsApply(0L);
					base.OwnActorModel.StartFortifications(DurationTurns, base.SkillID, grantedTraitIds);
				}
			}
		}

		public void EnterCooldownOnStateEnd(bool skipNextCooldownReduce, int cooldownOverride = -1)
		{
			if (base.manager != null && base.Definition != null)
			{
				base.LeftCooldownTurns = ((cooldownOverride >= 0) ? cooldownOverride : base.Definition.Cooldown);
				SkipNextCooldownReduce = skipNextCooldownReduce;
				if (base.OwnActorModel != null)
				{
					base.OwnActorModel.NotifyChange("CooldownLeftTurnUpdate");
				}
			}
		}

		public override void OnFactionChangeReduceCooldownLeftTurns()
		{
			if (SkipNextCooldownReduce)
			{
				SkipNextCooldownReduce = false;
			}
			else
			{
				base.OnFactionChangeReduceCooldownLeftTurns();
			}
		}

		public static FortificationsSkill FindSkill(ActorModel actor, int skillID = 0)
		{
			CommandSkillModelManager commandSkillModelManager = actor?.CommandSkillModelManager;
			if (commandSkillModelManager == null)
			{
				return null;
			}
			if (skillID != 0)
			{
				if (commandSkillModelManager.CommandSkills != null)
				{
					for (int i = 0; i < commandSkillModelManager.CommandSkills.Count; i++)
					{
						if (commandSkillModelManager.CommandSkills[i] is FortificationsSkill fortificationsSkill && fortificationsSkill.SkillID == skillID)
						{
							return fortificationsSkill;
						}
					}
				}
				if (commandSkillModelManager.ActorCommandSkill is FortificationsSkill fortificationsSkill2 && fortificationsSkill2.SkillID == skillID)
				{
					return fortificationsSkill2;
				}
			}
			FortificationsSkill fortificationsSkill3 = commandSkillModelManager.GetCommandSkill<FortificationsSkill>(CommandSkillType.CommandSkillFortifications);
			if (fortificationsSkill3 == null)
			{
				fortificationsSkill3 = commandSkillModelManager.GetActorCommandSkill<FortificationsSkill>(CommandSkillType.CommandSkillFortifications);
			}
			return fortificationsSkill3;
		}

		private FacingDirection GetOwnerFacing()
		{
			if (!(base.OwnActorModel is TankActorModel tankActorModel))
			{
				return FacingDirection.Any;
			}
			return tankActorModel.Facing;
		}
	}
}
