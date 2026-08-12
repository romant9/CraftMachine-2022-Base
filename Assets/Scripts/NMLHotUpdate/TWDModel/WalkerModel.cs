using System;
using Newtonsoft.Json;
using TWDModel.ContentTypes;

namespace TWDModel
{
	public class WalkerModel : ActorModel
	{
		private AbilityModifierIncrementer meleeDamageModifier;

		public DormantType DormantType { get; set; }

		public WalkerVisualization VisualVariation { get; set; }

		public ActivationType ActivationType { get; set; }

		[JsonIgnore]
		public WalkerType WalkerType => GameEconomyData.GetTypeEnum<WalkerType>(base.Definition.ID);

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
		}

		public override void Start()
		{
			foreach (EquipmentItemModel model in base.EquipmentItems.Models)
			{
				model.SetManager(base.manager);
				model.Start();
			}
			meleeDamageModifier = null;
			base.Start();
		}

		public override void SetupForCombat(CombatModel combatModel)
		{
			base.SetupForCombat(combatModel);
			if (ActivationType == ActivationType.Threat && combatModel != null && combatModel.IsRedacting)
			{
				RedactTimedEffect redactTimedEffect = combatModel.RedactTimedEffect;
				if (base.manager.Player.RollDice(RollDiceType.Redact, redactTimedEffect.ReduceHpChance) != PlayerRandomChanceResult.Failed)
				{
					int currentHitPoints = (int)((1f - (float)redactTimedEffect.ReducedHpRatio * 1f / 100f) * (float)base.Hitpoints);
					SetHitPoints(currentHitPoints, base.MaxHitPoints);
				}
			}
		}
	}
}
