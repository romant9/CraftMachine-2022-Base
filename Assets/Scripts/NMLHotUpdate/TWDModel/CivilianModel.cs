using System;
using Newtonsoft.Json;
using TWDModel.ContentTypes;

namespace TWDModel
{
	public class CivilianModel : ActorModel
	{
		private AbilityModifierIncrementer meleeDamageModifier;

		private AbilityModifierIncrementer rangedDamageModifier;

		public string DisplayName { get; set; }

		[JsonIgnore]
		public override string Name
		{
			get
			{
				if (!string.IsNullOrEmpty(DisplayName))
				{
					return DisplayName;
				}
				return base.Name;
			}
		}

		public override void ConfigureBaseAttributes()
		{
			ActorLevelDefinition actorLevelDefinition = base.manager.GameEconomyData.GetActorLevelDefinition(base.ActorDefinitionID, base.Level);
			if (actorLevelDefinition == null)
			{
				return;
			}
			FixedPoint fixedPoint = (FixedPoint)base.Definition.DamageMultiplier / (FixedPoint)100.0;
			FixedPoint fixedPoint2 = (FixedPoint)base.Definition.HealthMultiplier / (FixedPoint)100.0;
			int num = actorLevelDefinition.Health;
			int num2 = actorLevelDefinition.Damage;
			if (base.manager.Player.GetAttackTargetMissionModel() is MapMissionModel mapMissionModel)
			{
				if (mapMissionModel.IsInApocalyptiWeeklyChallenge)
				{
					num = actorLevelDefinition.ApocalypticChallengeHealth;
					num2 = actorLevelDefinition.ApocalypticChallengeDamage;
				}
				if (mapMissionModel.IsEndlessMission && base.manager.Player.EndlessModeManager != null)
				{
					if (base.manager.Player.EndlessModeManager.EndlessModeGameModeType == EndlessModeGameModeType.Expert)
					{
						num = actorLevelDefinition.EndlessExpertHealth;
						num2 = actorLevelDefinition.EndlessExpertDamage;
					}
					else
					{
						num = actorLevelDefinition.EndlessNormalHealth;
						num2 = actorLevelDefinition.EndlessNormalDamage;
					}
				}
			}
			int num3 = (int)(num * (1.0 + fixedPoint2));
			int num4 = Math.Max(base.MaxHitPoints - base.Hitpoints, 0);
			int currentHitPoints = Math.Max(num3 - num4, 0);
			SetHitPoints(currentHitPoints, num3);
			int num5 = (int)(num2 * (1.0 + fixedPoint));
			base.Modifiers.RemoveModifier(meleeDamageModifier);
			meleeDamageModifier = new AbilityModifierIncrementer("AddMeleeDamage", num5);
			base.Modifiers.RegisterModifier(meleeDamageModifier);
			base.Modifiers.RemoveModifier(rangedDamageModifier);
			rangedDamageModifier = new AbilityModifierIncrementer("AddMeleeDamage", num5);
			base.Modifiers.RegisterModifier(rangedDamageModifier);
		}

		protected override void SetupTraits()
		{
			base.SetupTraits();
			if (!HasTrait("Overwatch"))
			{
				AddTrait("Overwatch");
			}
		}

		public override void Start()
		{
			foreach (EquipmentItemModel model in base.EquipmentItems.Models)
			{
				model.SetManager(base.manager);
				model.Start();
			}
			meleeDamageModifier = null;
			rangedDamageModifier = null;
			base.Start();
		}
	}
}
