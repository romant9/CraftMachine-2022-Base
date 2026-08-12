using System;
using System.Linq;
using TWDModel;

public static class ConsumableUtils
{
	public static float GetMedKitRecoveredHealthDefinition(TWDModelManager manager)
	{
		AbilityModifierDefinition abilityModifierDefinition = manager.GameEconomyData.GetAbilityDefinition("WeaponAbilityMedkitConsumable")?.Modifiers.FirstOrDefault((AbilityModifierDefinition x) => x.Type == "AbilityModifierScaleHealth");
		if (abilityModifierDefinition == null)
		{
			return 0f;
		}
		string value = abilityModifierDefinition.ConstructionParameters.FirstOrDefault();
		if (string.IsNullOrEmpty(value))
		{
			return 0f;
		}
		return (float)Convert.ToDouble(value);
	}

	public static float GetPercentageDamageDefinition(TWDModelManager manager, EquipmentModel.ConsumableType consumableType)
	{
		AbilityModifierDefinition abilityModifierDefinition = manager.GameEconomyData.GetAbilityDefinition(ConsumableTypeToAbilityDefinition(consumableType))?.Modifiers.FirstOrDefault((AbilityModifierDefinition x) => x.Type == "AbilityModifierScaleHealth");
		if (abilityModifierDefinition == null)
		{
			return 0f;
		}
		string value = abilityModifierDefinition.ConstructionParameters.FirstOrDefault();
		if (string.IsNullOrEmpty(value))
		{
			return 0f;
		}
		return (float)Convert.ToDouble(value);
	}

	private static FixedPoint GetFlatDamageScale(TWDModelManager manager, EquipmentModel.ConsumableType consumableType)
	{
		AbilityModifierDefinition abilityModifierDefinition = manager.GameEconomyData.GetAbilityDefinition(ConsumableTypeToAbilityDefinition(consumableType))?.Modifiers.FirstOrDefault((AbilityModifierDefinition x) => x.Type == "AbilityModifierScaleDamageByMaxLevelSurvivor");
		if (abilityModifierDefinition == null)
		{
			return 0L;
		}
		string text = abilityModifierDefinition.ConstructionParameters.FirstOrDefault();
		if (string.IsNullOrEmpty(text))
		{
			return 0L;
		}
		return new FixedPoint(text);
	}

	private static FixedPoint GetGrenadeFlatDamageScale(TWDModelManager manager)
	{
		AbilityModifierDefinition abilityModifierDefinition = manager.GameEconomyData.GetAbilityDefinition("WeaponAbilityMolotovConsumable")?.Modifiers.FirstOrDefault((AbilityModifierDefinition x) => x.Type == "AbilityModifierScaleDamageByMaxLevelSurvivor");
		if (abilityModifierDefinition == null)
		{
			return 0L;
		}
		string text = abilityModifierDefinition.ConstructionParameters.FirstOrDefault();
		if (string.IsNullOrEmpty(text))
		{
			return 0L;
		}
		return new FixedPoint(text);
	}

	public static FixedPoint GetGrenadeFlatDamageDefinition(TWDModelManager manager)
	{
		GameEconomyData gameEconomyData = manager.GameEconomyData;
		ActorDefinition actorDefinition = gameEconomyData.GetActorDefinition(Enum.GetName(typeof(WalkerType), WalkerType.ExplosiveBarrel));
		WalkerExplosionDefinition walkerExplosionDefinition = gameEconomyData.GetWalkerExplosionDefinition(actorDefinition.InitialTraits[0]);
		int highestLevelSurvivor = manager.Player.SurvivorContainer.GetHighestLevelSurvivor();
		EquipmentDefinition equipmentDefinition = gameEconomyData.GetEquipmentDefinition(actorDefinition.InitialEquipmentsData[0].ID);
		RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition = gameEconomyData.GetRarityBasedUpgradeDefinition(actorDefinition.InitialEquipmentsData[0].RarityLevel, TWDModel.UpgradeType.EquipmentUpgrade);
		FixedPoint parameter = walkerExplosionDefinition.GetParameter<FixedPoint>(0);
		FixedPoint fixedPoint = manager.GameEconomyData.GetEquipmentLevelDefinition(highestLevelSurvivor).DamageBase;
		FixedPoint fixedPoint2 = (FixedPoint)equipmentDefinition.DamageMultiplier / (FixedPoint)100.0;
		FixedPoint fixedPoint3 = (FixedPoint)rarityBasedUpgradeDefinition.DamageMultiplier / (FixedPoint)100.0;
		FixedPoint fixedPoint4 = fixedPoint * (fixedPoint2 + fixedPoint3);
		FixedPoint grenadeFlatDamageScale = GetGrenadeFlatDamageScale(manager);
		return FixedPoint.Round(parameter * (int)fixedPoint4 / 100.0) * grenadeFlatDamageScale;
	}

	public static FixedPoint GetFlatDamage(TWDModelManager manager, EquipmentModel.ConsumableType consumableType, FixedPoint percentage = default(FixedPoint))
	{
		GameEconomyData gameEconomyData = manager.GameEconomyData;
		ActorDefinition actorDefinition = gameEconomyData.GetActorDefinition(Enum.GetName(typeof(WalkerType), WalkerType.ExplosiveBarrel));
		WalkerExplosionDefinition walkerExplosionDefinition = gameEconomyData.GetWalkerExplosionDefinition(actorDefinition.InitialTraits[0]);
		int highestLevelSurvivor = manager.Player.SurvivorContainer.GetHighestLevelSurvivor();
		EquipmentDefinition equipmentDefinition = gameEconomyData.GetEquipmentDefinition(actorDefinition.InitialEquipmentsData[0].ID);
		RarityBasedUpgradeDefinition rarityBasedUpgradeDefinition = gameEconomyData.GetRarityBasedUpgradeDefinition(actorDefinition.InitialEquipmentsData[0].RarityLevel, TWDModel.UpgradeType.EquipmentUpgrade);
		FixedPoint parameter = walkerExplosionDefinition.GetParameter<FixedPoint>(0);
		FixedPoint fixedPoint = manager.GameEconomyData.GetEquipmentLevelDefinition(highestLevelSurvivor).DamageBase;
		FixedPoint fixedPoint2 = (FixedPoint)equipmentDefinition.DamageMultiplier / (FixedPoint)100.0;
		FixedPoint fixedPoint3 = (FixedPoint)rarityBasedUpgradeDefinition.DamageMultiplier / (FixedPoint)100.0;
		FixedPoint fixedPoint4 = fixedPoint * (fixedPoint2 + fixedPoint3);
		FixedPoint fixedPoint5 = ((consumableType == EquipmentModel.ConsumableType.Unknown) ? percentage : GetFlatDamageScale(manager, consumableType));
		return FixedPoint.Round(parameter * (int)fixedPoint4 / 100.0) * fixedPoint5;
	}

	public static int GetFlareDuration(TWDModelManager manager)
	{
		AbilityEffectDefinition abilityEffectDefinition = manager.GameEconomyData.GetAbilityDefinition("WeaponAbilityFlareConsumable")?.EffectDefinitions.FirstOrDefault((AbilityEffectDefinition x) => x.Type == "AbilityEffectFlareConsumable");
		if (abilityEffectDefinition == null)
		{
			return 0;
		}
		int.TryParse(abilityEffectDefinition.ConstructionParameters[2], out var result);
		return result;
	}

	public static int GetGoreDuration(TWDModelManager manager)
	{
		AbilityEffectDefinition abilityEffectDefinition = manager.GameEconomyData.GetAbilityDefinition("WeaponAbilityGoreConsumable")?.EffectDefinitions.FirstOrDefault((AbilityEffectDefinition x) => x.Type == "AbilityEffectApplyGore");
		if (abilityEffectDefinition == null)
		{
			return 0;
		}
		int.TryParse(abilityEffectDefinition.ConstructionParameters[0], out var result);
		return result;
	}

	public static int GetBlastGrenadePushDistance(TWDModelManager manager)
	{
		int.TryParse((manager.GameEconomyData.GetAbilityDefinition("WeaponAbilityPushGrenadeConsumable")?.EffectDefinitions.FirstOrDefault((AbilityEffectDefinition x) => x.Type == "AbilityEffectPush")).ConstructionParameters[0], out var result);
		return result;
	}

	public static int GetCooldownDefinition(TWDModelManager manager, EquipmentModel.ConsumableType consumableType)
	{
		return manager.GameEconomyData.GetAbilityDefinition(ConsumableTypeToAbilityDefinition(consumableType)).CooldownAfterUse;
	}

	public static int GetThreatDefinition(TWDModelManager manager, EquipmentModel.ConsumableType consumableType)
	{
		return manager.GameEconomyData.GetAbilityDefinition(ConsumableTypeToAbilityDefinition(consumableType)).ThreatValue;
	}

	public static string ConsumableTypeToId(EquipmentModel.ConsumableType consumableType)
	{
		return consumableType switch
		{
			EquipmentModel.ConsumableType.Grenade => "Weapon_Throwable_Grenade_Consumable", 
			EquipmentModel.ConsumableType.MedKit => "Medkit_Consumable", 
			EquipmentModel.ConsumableType.Flare => "Weapon_Throwable_Flare_Consumable", 
			EquipmentModel.ConsumableType.BlastGrenade => "Weapon_Throwable_Blast_Grenade_Consumable", 
			EquipmentModel.ConsumableType.Gore => "Gore_Consumable", 
			_ => throw new ArgumentOutOfRangeException("consumableType", consumableType, null), 
		};
	}

	public static EquipmentModel.ConsumableType IdToConsumableType(string id)
	{
		return id switch
		{
			"Weapon_Throwable_Grenade_Consumable" => EquipmentModel.ConsumableType.Grenade, 
			"Medkit_Consumable" => EquipmentModel.ConsumableType.MedKit, 
			"Weapon_Throwable_Flare_Consumable" => EquipmentModel.ConsumableType.Flare, 
			"Weapon_Throwable_Blast_Grenade_Consumable" => EquipmentModel.ConsumableType.BlastGrenade, 
			"Gore_Consumable" => EquipmentModel.ConsumableType.Gore, 
			_ => EquipmentModel.ConsumableType.Unknown, 
		};
	}

	public static string ConsumableTypeToAbilityDefinition(EquipmentModel.ConsumableType consumableType)
	{
		return consumableType switch
		{
			EquipmentModel.ConsumableType.Grenade => "WeaponAbilityMolotovConsumable", 
			EquipmentModel.ConsumableType.MedKit => "WeaponAbilityMedkitConsumable", 
			EquipmentModel.ConsumableType.Flare => "WeaponAbilityFlareConsumable", 
			EquipmentModel.ConsumableType.BlastGrenade => "WeaponAbilityPushGrenadeConsumable", 
			EquipmentModel.ConsumableType.Gore => "WeaponAbilityGoreConsumable", 
			_ => throw new ArgumentOutOfRangeException("consumableType", consumableType, null), 
		};
	}
}
