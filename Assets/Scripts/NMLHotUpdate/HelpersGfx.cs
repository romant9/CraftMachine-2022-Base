using System;
using System.Collections.Generic;
using NextGames.Sdk.AssetBundleManager;
using TWDModel;
using UnityEngine;

public class HelpersGfx
{
	public static string BonusListElementPrefabPathString = "Bonus_List_Item";

	public static string ParticipantPlayerElementPrefabPathString = "Party_Member_List_Item";

	private static Dictionary<string, string> HeroArtAssetName = new Dictionary<string, string>
	{
		{ "Hero_Aaron", "S8E1" },
		{ "Hero_Abraham", "Abraham" },
		{ "Hero_Alpha", "Alpha" },
		{ "Hero_Beta", "Beta" },
		{ "Hero_Carl", "S3E4" },
		{ "Hero_Carol", "S8E9" },
		{ "Hero_Daryl", "S7E3" },
		{ "Hero_ScoutDaryl", "DarylScout" },
		{ "Hero_Dwight", "S7E11" },
		{ "Hero_Eugene", "S8E7" },
		{ "Hero_Ezekiel", "S7E16" },
		{ "Hero_Gabriel", "Gabriel" },
		{ "Hero_Glenn", "S3E1" },
		{ "Hero_BruiserGlenn", "GlennBruiser" },
		{ "Hero_Governor", "Governor" },
		{ "Hero_Jerry", "S8E4" },
		{ "Hero_Jesus", "S8E13" },
		{ "Hero_Maggie", "S7E5" },
		{ "Hero_ShooterMaggie", "MaggieShooter" },
		{ "Hero_Merle", "S3E6" },
		{ "Hero_Michonne", "S7E7" },
		{ "Hero_Morgan", "S7E13" },
		{ "Hero_HunterMorgan", "MorganHunter" },
		{ "Hero_Negan", "S7E8" },
		{ "Hero_Rick", "S7E10" },
		{ "Hero_ScoutRick", "RickScout" },
		{ "Hero_Rosita", "S7E4" },
		{ "Hero_Sasha", "S7E9" },
		{ "Hero_Tara", "S7E6" }
	};

	private static Color notAvailableColor = new Color(0.511f, 0.129f, 0.027f, 1f);

	public static string GetIconName(string type)
	{
		if (type == "Medal")
		{
			return "Ui_Icon_Resource_Trophy";
		}
		return "";
	}

	public static string GetOutpostBackgroundSpriteName(OutpostTemplateDefinition definition)
	{
		if (definition != null)
		{
			return "Ui_Outpost_" + definition.Id;
		}
		return "";
	}

	public static string GetTraitUpgradeCurrencyByActorDefinition(ActorDefinition actorDefinition)
	{
		return "Ui_Icon_" + actorDefinition.TraitUpgradeCurrency;
	}

	public static string GetSocialIconName(string type)
	{
		return type switch
		{
			"Fb" => "Ui_Icon_facebook",
			"Twitter" => "Ui_Icon_X",
			"Instagram" => "Ui_Icon_Instagram",
			"Forums" => "Ui_Icon_Forums",
			"Youtube" => "Ui_Icon_Youtube",
			"Discord" => "Ui_Icon_Discord",
			"Banana" => "Ui_Icon_Banana",
			_ => "",
		};
	}

	public static CurrencyType GetSPCurrencyType_N(CurrencyType tokenCurrencyType)
	{
		CurrencyType result = tokenCurrencyType;
		switch (tokenCurrencyType)
		{
		case CurrencyType.EquipmentTokenBP:
			result = CurrencyType.EquipmentTokenBP_N;
			break;
		case CurrencyType.BuildingTokenBP:
			result = CurrencyType.BuildingTokenBP_N;
			break;
		case CurrencyType.TrainingTokenBP:
			result = CurrencyType.TrainingTokenBP_N;
			break;
		case CurrencyType.HealingTokenBP:
			result = CurrencyType.HealingTokenBP_N;
			break;
		}
		return result;
	}

	public static string GetCurrencyIconName(CurrencyType currencyType, PlayerModel playerModel = null)
	{
		switch (currencyType)
		{
		case CurrencyType.ApocalypticSkipToken:
			return "Ui_Icon_Round_Pass_Apocalyptic";
		case CurrencyType.Supplies:
			return "Ui_Icon_Resource_Supplies";
		case CurrencyType.Phone:
			return "Ui_Icon_Resource_Radio";
		case CurrencyType.GoldRadio:
			return "Ui_Icon_Resource_GoldRadio";
		case CurrencyType.SurvivalPoints:
			return "Ui_Icon_Resource_Survival_Point";
		case CurrencyType.Inhabitants:
			return "Ui_Icon_Resource_Inhabitant";
		case CurrencyType.Diamonds:
			return "Ui_Icon_Resource_Gold";
		case CurrencyType.BounsItem:
			return "Ui_Icon_Resource_BounsItem";
		case CurrencyType.Survivor:
			return "Survivor_Icon";
		case CurrencyType.ReplayToken:
			return "Ui_Icon_Resource_Gas";
		case CurrencyType.ApocalypticEquipToken:
			return "Ui_Icon_Resource_ApocalypticEquipToken";
		case CurrencyType.Outpost:
			return "Ui_Icon_Resource_Outpost";
		case CurrencyType.GvGGas:
			return "Ui_Icon_Resource_Guild_Energy";
		case CurrencyType.GuildBattleRP:
			return "Ui_Icon_Resource_Guild_Currency";
		case CurrencyType.EquipmentUpgradeToken:
			return "Ui_Icon_Resource_UpgradeToken";
		case CurrencyType.TraitRerollToken:
			return "Ui_Icon_Resource_RerollTokenHero";
		case CurrencyType.GvGMissionKey:
			return "Ui_Icon_Resource_Guild_MissionKey";
		case CurrencyType.BlackMarketToken:
			return "Ui_Icon_Resource_BlackMarket";
		case CurrencyType.EndlessPassToken:
			return "Ui_Icon_EndlessPassToken";
		case CurrencyType.EndlessPassExpertToken:
			return "Ui_Icon_EndlessExpertPassToken";
		case CurrencyType.Fairmoney:
			return "Ui_Icon_Resource_Fairmoney";
		case CurrencyType.BulePrintToken:
			return "Ui_Icon_Resource_BluePrintMoney";
		case CurrencyType.HillTopCoin:
			return "Ui_Icon_Resource_HillCoin";
		case CurrencyType.PrimarySupportTalentToken:
			return "Ui_Icon_Resource_PrimarySupportTalentToken";
		case CurrencyType.AdvancedSupportTalentToken:
			return "Ui_Icon_Resource_AdvancedSupportTalentToken";
		case CurrencyType.ReturnMedal:
			return "Ui_Icon_Resource_RedemptionCoin";
		case CurrencyType.BuildingToken10min:
		case CurrencyType.BuildingToken1h:
		case CurrencyType.BuildingToken1min:
		case CurrencyType.BuildingToken5min:
		case CurrencyType.BuildingToken30min:
			return "Ui_Icon_SpeedUpToken_Building_Green";
		case CurrencyType.BuildingToken6h:
		case CurrencyType.BuildingToken12h:
		case CurrencyType.BuildingToken24h:
			return "Ui_Icon_SpeedUpToken_Building_Blue";
		case CurrencyType.BuildingTokenBP:
			return "Ui_Icon_SpeedUpToken_Building_Purple";
		case CurrencyType.BuildingTokenBP_N:
			return "Ui_Icon_SpeedUpToken_Building_Empty";
		case CurrencyType.SuperBuildingTokenBP:
			return "Ui_Icon_SpeedUpToken_Building_Gold";
		case CurrencyType.TrainingToken20min:
		case CurrencyType.TrainingToken1h:
		case CurrencyType.TrainingToken5min:
			return "Ui_Icon_SpeedUpToken_Training_Green";
		case CurrencyType.TrainingToken3h:
		case CurrencyType.TrainingToken8h:
		case CurrencyType.TrainingToken16h:
			return "Ui_Icon_SpeedUpToken_Training_Blue";
		case CurrencyType.TrainingTokenBP:
			return "Ui_Icon_SpeedUpToken_Training_Purple";
		case CurrencyType.TrainingTokenBP_N:
			return "Ui_Icon_SpeedUpToken_Training_Empty";
		case CurrencyType.SuperTrainingTokenBP:
			return "Ui_Icon_SpeedUpToken_Training_Gold";
		case CurrencyType.EquipmentToken20min:
		case CurrencyType.EquipmentToken1h:
		case CurrencyType.EquipmentToken1min:
		case CurrencyType.EquipmentToken10min:
			return "Ui_Icon_SpeedUpToken_Workshop_Green";
		case CurrencyType.EquipmentToken3h:
		case CurrencyType.EquipmentToken7h:
		case CurrencyType.EquipmentToken14h:
			return "Ui_Icon_SpeedUpToken_Workshop_Blue";
		case CurrencyType.EquipmentTokenBP:
			return "Ui_Icon_SpeedUpToken_Workshop_Purple";
		case CurrencyType.EquipmentTokenBP_N:
			return "Ui_Icon_SpeedUpToken_Workshop_Empty";
		case CurrencyType.SuperEquipmentTokenBP:
			return "Ui_Icon_SpeedUpToken_Workshop_Gold";
		case CurrencyType.HealingToken10min:
		case CurrencyType.HealingToken1h:
		case CurrencyType.HealingToken1min:
		case CurrencyType.HealingToken5min:
			return "Ui_Icon_SpeedUpToken_Healing_Green";
		case CurrencyType.HealingToken2h:
		case CurrencyType.HealingToken4h:
			return "Ui_Icon_SpeedUpToken_Healing_Blue";
		case CurrencyType.HealingTokenBP:
			return "Ui_Icon_SpeedUpToken_Healing_Purple";
		case CurrencyType.HealingTokenBP_N:
			return "Ui_Icon_SpeedUpToken_Healing_Empty";
		case CurrencyType.BattlePassPoints:
			if (GameManager.Instance.playerModel.BattlePass.PremiumActive)
			{
				return "Ui_Icon_BattlePass_Currency_Gold";
			}
			return "Ui_Icon_BattlePass_Currency_Silver";
		case CurrencyType.MTToken:
			return "Ui_Icon_Item_MTToken";
		case CurrencyType.EXToken:
			return "Ui_Icon_Item_ExToken";
		case CurrencyType.SPTraitsRemoldToken:
			return "UI_Icon_SPTraitsRemoldToken";
		case CurrencyType.SPTraitsUpgradeToken:
			return "UI_Icon_SPTraitsUpgradeToken";
		case CurrencyType.ShooterStar:
			return "UI_Icon_Resource_ShooterStar";
		case CurrencyType.HunterStar:
			return "UI_Icon_Resource_HunterStar";
		case CurrencyType.BruiserStar:
			return "UI_Icon_Resource_BrusierStar";
		case CurrencyType.WarriorStar:
			return "UI_Icon_Resource_WarriorStar";
		case CurrencyType.ScoutStar:
			return "UI_Icon_Resource_ScoutStar";
		case CurrencyType.AssaultStar:
			return "UI_Icon_Resource_AssaultStar";
		case CurrencyType.CBPWarrior:
			return "Ui_Icon_CommonBluePrint_Warrior";
		case CurrencyType.CBPScout:
			return "Ui_Icon_CommonBluePrint_Scout";
		case CurrencyType.CBPBruiser:
			return "Ui_Icon_CommonBluePrint_Bruiser";
		case CurrencyType.CBPShooter:
			return "Ui_Icon_CommonBluePrint_Shooter";
		case CurrencyType.CBPHunter:
			return "Ui_Icon_CommonBluePrint_Hunter";
		case CurrencyType.CBPAssault:
			return "Ui_Icon_CommonBluePrint_Assault";
		case CurrencyType.CampaignToken:
			playerModel = GameManager.Instance.playerModel;
			if (playerModel != null && playerModel.CampaignModel != null)
			{
				CampaignDefinition campaignDefinition = playerModel.gameEconomyData.GetCampaignDefinition(playerModel.CampaignModel.Id);
				if (campaignDefinition != null)
				{
					return campaignDefinition.TokenIcon;
				}
			}
			else
			{
				GetTokenCurrencyIconName(currencyType);
			}
			break;
		}
		if (IsSkillTonkenCurrencyType(currencyType))
		{
			return GetSkillKitTokenSetDefinition(currencyType).TopIcon;
		}
		return GetTokenCurrencyIconName(currencyType);
	}

	public static bool IsSkillTonkenCurrencyType(CurrencyType currencyType)
	{
		if (GetSkillKitTokenSetDefinition(currencyType) != null)
		{
			return true;
		}
		return false;
	}

	public static SPTraitsSkillKitTokenSet GetSkillKitTokenSetDefinition(CurrencyType currencyType)
	{
		return GameManager.Instance.playerModel.gameEconomyData.GetSPTraitsSkillKitTokenSetByID(currencyType.ToString());
	}

	public static string GetConsumableIconName(EquipmentModel.ConsumableType consumableType)
	{
		return consumableType switch
		{
			EquipmentModel.ConsumableType.Grenade => "Ui_Icon_Consumable_Grenade",
			EquipmentModel.ConsumableType.MedKit => "Ui_Icon_Consumable_Medkit",
			EquipmentModel.ConsumableType.Flare => "Ui_Icon_Consumable_Flare",
			EquipmentModel.ConsumableType.BlastGrenade => "Ui_Icon_Consumable_BlastGrenade",
			EquipmentModel.ConsumableType.Gore => "Ui_Icon_Consumable_Guts",
			_ => "",
		};
	}

	public static string GetTokenCurrencyIconName(CurrencyType currencyType)
	{
		return "Ui_Icon_" + currencyType;
	}

	public static string GetRewardTimedBonusIcon(RewardTimedBonus rewardTimedBonus)
	{
		if (rewardTimedBonus.TimedBonusType == TimedBonusType.UnlimitedGas)
		{
			return "Ui_Icon_Infinite_Gas";
		}
		if (rewardTimedBonus.TimedBonusType == TimedBonusType.DoubleXp)
		{
			return "Ui_Icon_Double_Xp";
		}
		return "";
	}

	public static CurrencyType GetSurvivorTraitUpgradeCurrencyType(ActorDefinition actorDefinition)
	{
		CurrencyType currencyType = CurrencyType.None;
		if (actorDefinition != null)
		{
			if (Enum.IsDefined(typeof(CurrencyType), actorDefinition.TraitUpgradeCurrency))
			{
				currencyType = actorDefinition.TraitUpgradeCurrency;
			}
			if (currencyType == CurrencyType.None)
			{
				if (actorDefinition.Class == "Assault")
				{
					currencyType = CurrencyType.AssaultToken;
				}
				else if (actorDefinition.Class == "Bruiser")
				{
					currencyType = CurrencyType.BruiserToken;
				}
				else if (actorDefinition.Class == "Hunter")
				{
					currencyType = CurrencyType.HunterToken;
				}
				else if (actorDefinition.Class == "Shooter")
				{
					currencyType = CurrencyType.ShooterToken;
				}
				else if (actorDefinition.Class == "Warrior")
				{
					currencyType = CurrencyType.WarriorToken;
				}
				else if (actorDefinition.Class == "Scout")
				{
					currencyType = CurrencyType.ScoutToken;
				}
			}
		}
		return currencyType;
	}

	public static string GetSpriteNameForLootType(DropEventDefinition.DropEventTag lootTag)
	{
		return lootTag switch
		{
			DropEventDefinition.DropEventTag.PreferEquipment => "Ui_Icon_Equipment",
			DropEventDefinition.DropEventTag.PreferSP => "Ui_Icon_Resource_Survival_Point",
			DropEventDefinition.DropEventTag.PreferSupplies => "Ui_Icon_Resource_Supplies",
			_ => "",
		};
	}

	public static string GetBuildingIconName(string buildingType)
	{
		return "Ui_Icon_" + buildingType;
	}

	public static string GetEquipmentPropertyIconName(EquipmentItemModel equipmentItemModel)
	{
		if (equipmentItemModel.Definition.Category == EquipmentCategory.RangeWeapon)
		{
			return "Shots_icon_small_white";
		}
		if (equipmentItemModel.Definition.Category == EquipmentCategory.MeleeWeapon)
		{
			return "melee_icon";
		}
		if (equipmentItemModel.Definition.Category == EquipmentCategory.Armor)
		{
			return "Nrj_icon";
		}
		return "";
	}

	public static string GetEquipmentCategoryIconName(EquipmentCategory equipmentCategory)
	{
		return equipmentCategory switch
		{
			EquipmentCategory.MeleeWeapon => "Ui_Icon_Stat_Damage",
			EquipmentCategory.RangeWeapon => "Ui_Icon_Stat_Damage",
			EquipmentCategory.Armor => "Ui_Icon_Stat_Health",
			_ => "",
		};
	}

	public static string GetEquipmentCategoryIconNameSmall(EquipmentCategory equipmentCategory)
	{
		return equipmentCategory switch
		{
			EquipmentCategory.MeleeWeapon => "Ui_Icon_Stat_Damage_Small",
			EquipmentCategory.RangeWeapon => "Ui_Icon_Stat_Damage_Small",
			EquipmentCategory.Armor => "UI_Icon_Stat_Health_Small",
			_ => "",
		};
	}

	public static Material GetTradeCrateMaterial(string tradeCrateId)
	{
		return tradeCrateId switch
		{
			"TradeCrateGolden" => AssetBundleManager.Instance.LoadAsset<Material>("ui_texture_gold_crate", "uimaterials"),
			"TradeCrateSilver" => AssetBundleManager.Instance.LoadAsset<Material>("ui_texture_silver_crate", "uimaterials"),
			"TradeCrateGearLow" => AssetBundleManager.Instance.LoadAsset<Material>("ui_texture_rare_crate", "uimaterials"),
			"TradeCrateGearMid" => AssetBundleManager.Instance.LoadAsset<Material>("ui_texture_epic_crate", "uimaterials"),
			"TradeCrateGearHigh" => AssetBundleManager.Instance.LoadAsset<Material>("ui_texture_legendary_crate", "uimaterials"),
			_ => null,
		};
	}

	public static Material GetSeasonHeroMaterial(string seasonId)
	{
		return AssetBundleManager.Instance.LoadAsset<Material>("ui_texture_season_hero_" + seasonId, "uimaterials");
	}

	public static Material GetSeasonBackgroundMaterial(string seasonId)
	{
		return AssetBundleManager.Instance.LoadAsset<Material>("ui_texture_season_" + seasonId, "uimaterials");
	}

	public static string GetEquipmentTypeIconName(EquipmentType equipmentType)
	{
		return "Ui_Icon_" + equipmentType;
	}

	public static string GetSurvivorClassIconName(SurvivorModel survivorModel)
	{
		return GetSurvivorClassIconName(survivorModel.Definition.Class.ToString(), survivorModel.SurvivorRarityLevel);
	}

	public static string GetSurvivorClassIconName(SurvivorClass survivorClass)
	{
		return GetSurvivorClassIconName(survivorClass.ToString());
	}

	public static string GetSurvivorClassIconName(string actorClass, int rarityLevel = 0)
	{
		return "Ui_Icon_Class_" + actorClass + "_" + HelpersUI.GetRarityName(rarityLevel);
	}

	public static string GetSurvivorEventIconName(string actorClass)
	{
		return "UI_EventIcon_Class_" + actorClass;
	}

	public static string GetSurvivorClassSmallIconName(SurvivorClass survivorClass)
	{
		return "Ui_Icon_Class_" + survivorClass;
	}

	public static string GetSurvivorClassSmallIconName(string survivorClass)
	{
		return "Ui_Icon_Class_" + survivorClass;
	}

	public static string GetSurvivorHealthbarClassIconName(SurvivorModel survivorModel)
	{
		return "Ui_Icon_Class_" + survivorModel.Definition.Class.ToString();
	}

	public static string GetHealthbarClassIconName(ActorModel actor)
	{
		if (actor.IsWalker)
		{
			return GetWalkerSpriteIconFromClass(actor);
		}
		return "Ui_Icon_Class_" + actor.Definition.Class.ToString();
	}

	public static string GetCoverIconName(CoverIconState state)
	{
		return state switch
		{
			CoverIconState.HalfCover => "Ui_Icon_Cover",
			CoverIconState.FullCover => "Ui_Icon_Cover",
			CoverIconState.Flanked => "Ui_Icon_Cover_Broken",
			_ => "",
		};
	}

	public static string GetCampIconName(CampType campType)
	{
		return "Ui_Icon_" + campType.Name;
	}

	public static Color GetColorForDamageDifference(float difference)
	{
		if (difference > 0f)
		{
			return new Color(0.196f, 0.333f, 0.118f, 1f);
		}
		if (difference < 0f)
		{
			return new Color(0.529f, 0.094f, 0.094f, 1f);
		}
		return Color.white;
	}

	public static Color GetBrightColorForDamageDifference(float difference)
	{
		if (difference > 0f)
		{
			return new Color(0.572f, 0.761f, 0.063f, 1f);
		}
		if (difference < 0f)
		{
			return new Color(0.898f, 0.545f, 0.259f, 1f);
		}
		return Color.white;
	}

	public static Color GetAvailabilityColor(bool available)
	{
		if (!available)
		{
			return notAvailableColor;
		}
		return Color.white;
	}

	public static string GetSurvivorRarityEdgeSpriteName(int rarityLevel)
	{
		return "Ui_Tier_" + HelpersUI.GetRarityName(rarityLevel);
	}

	public static string GetRarityBorderSpriteName(int rarityLevel)
	{
		return "Ui_Border_" + HelpersUI.GetRarityName(rarityLevel);
	}

	public static string GetEquipmentTraitIconName(UpgradeTraitsData traitsData)
	{
		if (traitsData == null)
		{
			return "Ui_Icon_Trait_Unknown";
		}
		string text = traitsData.Identifier.Replace("Equipment.", "");
		text = text.Replace("Armor.", "");
		text = UpgradeTraitsData.StripTraitLevelIdentifier(text);
		string text2 = "";
		if (traitsData.IsLocked)
		{
			text2 = "Low";
		}
		else
		{
			switch (traitsData.RarityLevel)
			{
			case 0:
				text2 = "Low";
				break;
			case 1:
				text2 = "Mid";
				break;
			case 2:
			case 3:
			case 4:
				text2 = "High";
				break;
			case 5:
				text2 = "Highest";
				break;
			default:
				text2 = "";
				break;
			}
		}
		if (string.IsNullOrEmpty(text2))
		{
			return "Ui_Icon_Trait_" + text;
		}
		return "Ui_Icon_Trait_" + text + "_" + text2;
	}

	public static string GetEquipmentTraitIconNameUsingTraitDefinition(TraitDefinition traitDefinition)
	{
		if (traitDefinition == null)
		{
			return "Ui_Icon_Trait_Unknown";
		}
		int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(traitDefinition.Identifier);
		string text = traitDefinition.Identifier.Replace("Equipment.", "");
		text = text.Replace("Armor.", "");
		text = UpgradeTraitsData.StripTraitLevelIdentifier(text);
		string text2 = "";
		switch (traitLevelIdentifier)
		{
		case 0:
			text2 = "Low";
			break;
		case 1:
			text2 = "Mid";
			break;
		case 2:
		case 3:
		case 4:
			text2 = "High";
			break;
		case 5:
			text2 = OfflineManager.IsLoadDataManager ? "Highest" : "";
			break;
		default:
			text2 = "";
			break;
		}
		if (string.IsNullOrEmpty(text2))
		{
			return "Ui_Icon_Trait_" + text;
		}
		return "Ui_Icon_Trait_" + text + "_" + text2;
	}

	public static string GetRandomComponentIconName(string componentName)
	{
		return "Ui_Icon_Shop_" + componentName;
	}

	public static string GetSurvivorTraitIconName(UpgradeTraitsData traitData)
	{
		return "Ui_Icon_Trait_" + UpgradeTraitsData.StripTraitLevelIdentifier(traitData.Identifier);
	}

	public static string GetSurvivorTraitIconName(TraitDefinition traitDefintion)
	{
		return "Ui_Icon_Trait_" + UpgradeTraitsData.StripTraitLevelIdentifier(traitDefintion.Identifier);
	}

	public static string GetGuildBattleBuffIconName(string traitIdentifier)
	{
		string gvGBuffIcon = GameManager.Instance.playerModel.gameEconomyData.GetGvGBuffIcon(traitIdentifier);
		if (!string.IsNullOrEmpty(gvGBuffIcon))
		{
			return gvGBuffIcon;
		}
		return null;
	}

	public static Color GetRarityColor(int rarityLevel)
	{
		if (rarityLevel < 5)
		{
			return rarityLevel switch
			{
				4 => new Color(0.8f, 0.5f, 0f),
				3 => new Color(0.6f, 0f, 0.6f),
				2 => new Color(0f, 0.6f, 0f),
				1 => new Color(0f, 0f, 0f),
				_ => new Color(0.5f, 0.5f, 0.5f),
			};
		}
		return new Color(0.8f, 0.5f, 0f);
	}

	public static void SetApocalypticEffectActive(GameObject apocalypticEffect, int rarityLevel)
	{
		if (apocalypticEffect != null)
		{
			Helpers.GameObjectSetActive(apocalypticEffect, IsApocalypticRarity(rarityLevel));
		}
	}

	public static void SetApocalypticEffectSprite(UITexture icon, List<string> activeTraits, List<string> passiveTraits, int rarityLevel)
	{
		Helpers.GameObjectSetActive(icon, value: false);
		string text = "";
		if (!IsApocalypticRarity(rarityLevel))
		{
			return;
		}
		List<string> list = new List<string>();
		if (activeTraits != null)
		{
			list.AddRange(activeTraits);
		}
		if (passiveTraits != null)
		{
			list.AddRange(passiveTraits);
		}
		for (int i = 0; i < list.Count; i++)
		{
			string text2 = list[i];
			if (text2.Contains("Equipment_Apocalyptic_DMG"))
			{
				text = "Equipment_Apocalyptic_DMG";
				break;
			}
			if (text2.Contains("Equipment_Apocalyptic_BS"))
			{
				text = "Equipment_Apocalyptic_BS";
				break;
			}
			if (text2.Contains("Equipment_Apocalyptic_DEF"))
			{
				text = "Equipment_Apocalyptic_DEF";
				break;
			}
		}
		if (icon != null && !string.IsNullOrEmpty(text))
		{
			UnityEngine.Object obj = UnityUtils.LoadFromAssetBundle(text, "itemgraphics");
			if (obj != null)
			{
				icon.mainTexture = (Texture)obj;
				Helpers.GameObjectSetActive(icon, value: true);
			}
		}
	}

	public static bool IsApocalypticRarity(int rarityLevel)
	{
		return rarityLevel >= 5;
	}

	public static string GetEquipmentRaritySprite(int rarityLevel)
	{
		string text = "Ui_Equipment_Rarity_";
		if (rarityLevel < 5)
		{
			return rarityLevel switch
			{
				0 => text + "Common",
				1 => text + "Uncommon",
				2 => text + "Rare",
				3 => text + "Epic",
				4 => text + "Legendary",
				_ => text + "Common",
			};
		}
		return text + "Legendary";
	}

	public static string GetEquipmentRaritySprite(int rarityLevel, bool switchRemoldMode)
	{
		if (switchRemoldMode)
		{
			return "Ui_Equipment_Rarity_Remold";
		}
		return GetEquipmentRaritySprite(rarityLevel);
	}

	public static string GetBadgeRaritySprite(int rarityLevel)
	{
		string text = "Ui_Bagde_Bg_";
		if (rarityLevel < 5)
		{
			return rarityLevel switch
			{
				0 => text + "Common",
				1 => text + "Uncommon",
				2 => text + "Rare",
				3 => text + "Epic",
				4 => text + "Legendary",
				_ => text + "Common",
			};
		}
		return text + "Legendary";
	}

	public static string GetBadgeTypeSprite(BadgeType badgeType)
	{
		return "Ui_Badge_Overlay_Type" + (int)(badgeType + 1);
	}

	public static string GetBadgeEffectSprite(string effect)
	{
		return "Ui_Badge_Icon_" + effect;
	}

	public static string GetSurvivorClassEpisodeIconName(SurvivorClass survivorClass, bool isLocked)
	{
		if (isLocked)
		{
			return "Ui_Episode_Icon_" + survivorClass.ToString() + "_Locked";
		}
		return "Ui_Episode_Icon_" + survivorClass.ToString() + "_Unlocked";
	}

	public static string GetChargeEquipmentIconName(EquipmentItemModel chargeEquipment)
	{
		EquipmentResourceEntry equipmentResourceEntry = GetEquipmentResourceEntry(chargeEquipment);
		if (equipmentResourceEntry != null)
		{
			return equipmentResourceEntry.IconSprite;
		}
		Debug.LogError("Could not find icon for charge ability " + chargeEquipment.EquipmentDefinitionIdentifier);
		return "Ui_Icon_Charge_Flare";
	}

	public static Texture GetEquipmentIconTexture(EquipmentItemModel equipment)
	{
		if (equipment == null)
		{
			return null;
		}
		return GetEquipmentIconTexture(equipment.Definition);
	}

	public static Texture GetEquipmentIconTexture(EquipmentDefinition equipmentDefinition)
	{
		if (!string.IsNullOrEmpty(equipmentDefinition.UseThisWeaponIcon))
		{
			return (Texture)UnityUtils.LoadFromAssetBundle(equipmentDefinition.UseThisWeaponIcon, "itemgraphics");
		}
		return (Texture)UnityUtils.LoadFromAssetBundle(equipmentDefinition.ID, "itemgraphics");
	}

	public static Texture GetEquipmentTokenIconTexture(EquipTokenDefinition equipTokenDefinition)
	{
		if (!string.IsNullOrEmpty(equipTokenDefinition.UseThisWeaponIcon))
		{
			return (Texture)UnityUtils.LoadFromAssetBundle(equipTokenDefinition.UseThisWeaponIcon, "itemgraphics");
		}
		return (Texture)UnityUtils.LoadFromAssetBundle(equipTokenDefinition.EquipTokenId, "itemgraphics");
	}

	public static Texture GetEquipmentIconTextureFromID(string equipmentID)
	{
		EquipmentDefinition equipmentDefinition = GameManager.Instance.modelManager.GameEconomyData.GetEquipmentDefinition(equipmentID);
		if (equipmentDefinition == null)
		{
			return null;
		}
		return GetEquipmentIconTexture(equipmentDefinition);
	}

	public static EquipmentResourceEntry GetEquipmentResourceEntry(EquipmentItemModel equipment)
	{
		return GetEquipmentResourceEntry(equipment.Definition);
	}

	public static EquipmentResourceEntry GetEquipmentResourceEntry(EquipmentDefinition equipmentDefinition)
	{
		if (!string.IsNullOrEmpty(equipmentDefinition.UseThisWeaponIcon))
		{
			return GameManager.Instance.GetResources<EquipmentResourceEntry>(equipmentDefinition.UseThisWeaponIcon);
		}
		return GameManager.Instance.GetResources<EquipmentResourceEntry>(equipmentDefinition.ID);
	}

	public static string GetLootIconName(DropType dropType)
	{
		if (dropType != DropType.Gold)
		{
			return "Ui_Icon_Loot_Silver_Small";
		}
		return "Ui_Icon_Loot_Gold_Small";
	}

	public static string GetTierEmblemIconName(string tierId)
	{
		if (!string.IsNullOrEmpty(tierId))
		{
			return "Ui_Emblem_League_" + tierId;
		}
		return "";
	}

	public static void SetSurvivorClassMaterial(UITexture texture, SurvivorClass survivorClass)
	{
		Material material = AssetBundleManager.Instance.LoadAsset<Material>("ui_texture_class_" + survivorClass.ToString().ToLower(), "uimaterials");
		if (material != null && texture != null)
		{
			texture.material = material;
		}
	}

	public static void SetSeasonHeroMaterial(UITexture texture, string season)
	{
		Material material = AssetBundleManager.Instance.LoadAsset<Material>("ui_texture_hero_" + season, "uimaterials");
		if (material != null && texture != null)
		{
			texture.material = material;
		}
	}

	public static void SetEpisodeTextureMaterial(UITexture texture, string textureName)
	{
		if (textureName != null)
		{
			Material material = Resources.Load("UI/Materials/ui_texture_detail_map_" + textureName.ToLower()) as Material;
			if (material != null)
			{
				texture.material = material;
			}
		}
	}

	public static void SetSurvivorRarityRating(UISprite[] starsArray, int survivorRarityLevel)
	{
		if (starsArray == null)
		{
			return;
		}
		for (int i = 0; i < starsArray.Length; i++)
		{
			if (starsArray[i] != null && (bool)starsArray[i].gameObject)
			{
				if (survivorRarityLevel >= i)
				{
					starsArray[i].gameObject.SetActive(value: true);
				}
				else
				{
					starsArray[i].gameObject.SetActive(value: false);
				}
			}
		}
	}

	public static void SetSurvivorFeaturedStars(UISprite[] featuredStars, SurvivorModel survivorModel)
	{
		if (featuredStars != null)
		{
			for (int i = 0; i < featuredStars.Length; i++)
			{
				Helpers.GameObjectSetActive(featuredStars[i], value: false);
			}
			int num = 0;
			FeaturedHeroDefinition featuredDefinition = survivorModel.FeaturedDefinition;
			if (featuredDefinition != null)
			{
				num = featuredDefinition.RarityModifier;
			}
			for (int j = 0; j < num; j++)
			{
				int num2 = (survivorModel.SurvivorRarityLevel + 1 + j) % featuredStars.Length;
				Helpers.GameObjectSetActive(featuredStars[num2], value: true);
			}
		}
	}

	public static void SetColorWithHex(UITexture texture, string hexColor)
	{
		if (texture != null && ColorUtility.TryParseHtmlString(hexColor, out var color))
		{
			texture.color = color;
		}
	}

	public static void SetColorWithHex(UISprite sprite, string hexColor)
	{
		if (sprite != null && ColorUtility.TryParseHtmlString(hexColor, out var color))
		{
			sprite.color = color;
		}
	}

	public static void SetWalkerPortaitTexture(UISprite sprite, string walkerId)
	{
		ActorResourceEntry resources = GameManager.Instance.GetResources<ActorResourceEntry>(walkerId);
		if (resources == null)
		{
			Debug.LogError("SetWalkerPortaitTexture: Could not find resources for actor prefab list " + walkerId + "!");
		}
		else
		{
			sprite.spriteName = resources.PortraitTexture;
		}
	}

	public static Texture GetSurvivorPortraitTextureBySurvivorMockData(SurvivorMockData mockData, PortraitManager.PortraitRenderedCallback portraitRenderedCallback)
	{
		SurvivorModel survivorModel = null;
		for (int i = 0; i < GameManager.Instance.playerModel.SurvivorContainer.Survivors.Count; i++)
		{
			if (GameManager.Instance.playerModel.SurvivorContainer.Survivors[i].IdForAnalytics == mockData.AnalyticsId)
			{
				survivorModel = GameManager.Instance.playerModel.SurvivorContainer.Survivors[i];
			}
		}
		if (survivorModel == null)
		{
			return null;
		}
		PortraitRenderSource info = PortraitRenderSource.fromActorModel(survivorModel);
		Texture portrait = PortraitManager.Instance.GetPortrait(info);
		if (portrait == null)
		{
			ModularCharacter prefabForActor = ActorView.GetPrefabForActor(survivorModel);
			if (prefabForActor != null)
			{
				PortraitManager.Instance.CreatePortrait(info, prefabForActor, portraitRenderedCallback);
			}
		}
		return portrait;
	}

	public static void UpdateSpriteAndKeepScale(UISprite sprite, string newSpriteName)
	{
		if (sprite != null && sprite.transform != null)
		{
			Vector3 localScale = sprite.transform.localScale;
			sprite.spriteName = newSpriteName;
			sprite.MakePixelPerfect();
			sprite.transform.localScale = localScale;
		}
	}

	public static string GetWalkerIconName(WalkerType walkerType)
	{
		return "Ui_Icon_Class_" + WalkerTypeToClassIconName(walkerType.ToString());
	}

	public static string GetOutpostTierIconName(string tierId)
	{
		return "Ui_Emblem_League_" + tierId;
	}

	public static int GetAmountForIReward(IReward reward)
	{
		if (reward != null)
		{
			if (reward is RewardCurrency)
			{
				return (reward as RewardCurrency).Amount;
			}
			if (reward is RewardEquipment)
			{
				return (reward as RewardEquipment).Amount;
			}
		}
		return 1;
	}

	public static UIAtlas GetIconNameForIReward(IReward reward, out string spriteName, UIAtlas uiCurrencyAtlas, UIAtlas shopAtlas, UIAtlas uiCampAtlas, PlayerModel playerModel = null)
	{
		spriteName = "";
		UIAtlas result = uiCurrencyAtlas;
		if (reward != null)
		{
			if (reward is RewardCurrency)
			{
				RewardCurrency rewardCurrency = reward as RewardCurrency;
				if (reward != null)
				{
					spriteName = GetCurrencyIconName(rewardCurrency.CurrencyType, playerModel);
				}
			}
			else if (reward is RewardTradeCrate)
			{
				if (reward is RewardTradeCrate rewardTradeCrate)
				{
					spriteName = "Ui_Icon_" + rewardTradeCrate.TradeCrateId;
				}
			}
			else if (reward is RewardRemoldSkill)
			{
				if (reward is RewardRemoldSkill rewardRemoldSkill)
				{
					SPTraitsRemoldDefinitions minRemoldDefinitionForGroup = Helpers.GetMinRemoldDefinitionForGroup(rewardRemoldSkill.SpRemoldSkillType);
					if (minRemoldDefinitionForGroup != null)
					{
						spriteName = minRemoldDefinitionForGroup.SPTraitsIcon;
					}
				}
			}
			else if (reward is RewardEquipment)
			{
				if (reward is RewardEquipment rewardEquipment)
				{
					spriteName = GetTextureNameForEquipmentReward(rewardEquipment);
				}
			}
			else if (reward.Type == RewardType.Outfit)
			{
				spriteName = "Ui_Icon_Outfits";
			}
			else if (reward.Type == RewardType.RewardSkipChallange)
			{
				spriteName = "Ui_Icon_Round_Pass";
			}
			else if (reward.Type == RewardType.HeroSkin)
			{
				spriteName = "Ui_Icon_Hero_Outfit";
				result = shopAtlas;
			}
			else if (reward.Type == RewardType.SurvivorSlot)
			{
				spriteName = "Ui_Icon_Survivor_Empty";
			}
			else if (reward.Type == RewardType.TimedBonus)
			{
				if (reward is RewardTimedBonus rewardTimedBonus)
				{
					result = shopAtlas;
					spriteName = GetRewardTimedBonusIcon(rewardTimedBonus);
				}
			}
			else if (reward.Type == RewardType.RandomEquipment)
			{
				if (reward is RewardRandomEquipment rewardRandomEquipment)
				{
					result = uiCampAtlas;
					if (rewardRandomEquipment.Category == EquipmentCategory.Armor)
					{
						spriteName = "Ui_Icon_BundleArmor";
					}
					else
					{
						spriteName = "Ui_Icon_BundleWeapon_" + rewardRandomEquipment.SurvivorClass;
					}
				}
			}
			else
			{
				Debug.LogWarning("Could not find icon for: Unsupported reward type: " + reward);
				result = null;
			}
		}
		return result;
	}

	public static string GetTextureNameForEquipmentReward(RewardEquipment rewardEquipment)
	{
		EquipmentDefinition equipmentDefinition = rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager);
		if (equipmentDefinition == null)
		{
			return "";
		}
		if (!string.IsNullOrEmpty(equipmentDefinition.UseThisWeaponIcon))
		{
			return equipmentDefinition.UseThisWeaponIcon;
		}
		return equipmentDefinition.ID;
	}

	public static Texture GetTextureForEquipmentReward(RewardEquipment rewardEquipment)
	{
		EquipmentDefinition equipmentDefinition = rewardEquipment.EquipmentDefinition(GameManager.Instance.modelManager);
		if (equipmentDefinition != null)
		{
			return GetEquipmentIconTexture(equipmentDefinition);
		}
		return null;
	}

	public static string GetIconNameForChallengeSegments(int cyclesTotalAmount)
	{
		return "Ui_Challenge_Difficulty_Rounds" + cyclesTotalAmount;
	}

	public static string GetSeasonArtForHero(string heroDefinition)
	{
		HeroArtAssetName.TryGetValue(heroDefinition ?? "", out var value);
		return value;
	}

	public static void SetShopAtlasToSprite(CurrencyType currencyType, UISprite sprite, UIAtlas shopAtlas, UIAtlas shopSurvivorTokenAtlas)
	{
		if (sprite == null || shopAtlas == null || shopSurvivorTokenAtlas == null)
		{
			return;
		}
		if (MissionIcon.HasTokens(currencyType))
		{
			if ((UIAtlas)sprite.atlas != shopSurvivorTokenAtlas)
			{
				sprite.atlas = shopSurvivorTokenAtlas;
			}
		}
		else if ((UIAtlas)sprite.atlas != shopAtlas)
		{
			sprite.atlas = shopAtlas;
		}
	}

	public static Texture LoadSupportIcon(string supportId)
	{
		return UnityUtils.LoadFromAssetBundle<Texture>("UI_Icon_Support_" + supportId, "itemgraphics");
	}

	public static string GetSupportSkillIconName(string supportId)
	{
		return "UI_Icon_SupportSkill_" + supportId;
	}

	public static string GetWalkerSpriteIconFromClass(ActorModel actor)
	{
		if (actor.Definition.Class.ToLower().StartsWith("walkerwhisperer"))
		{
			return "Ui_Icon_Class_" + WalkerType.WalkerNormal;
		}
		return "Ui_Icon_Class_" + WalkerTypeToClassIconName(actor.Definition.Class);
	}

	public static string WalkerTypeToClassIconName(string className)
	{
		return className.Split('_')[0];
	}

	public static string GetSupportRarityBorderSpriteName(int level)
	{
		return GetRarityBorderSpriteName(Mathf.Max(level - 1, 0));
	}



	#region mycode
	public static string GetEquipmentTraitIconNameUsingTraitDefinition(EquipTraitsDefinition traitDefinition)
	{
		if (traitDefinition == null)
		{
			return "Ui_Icon_Trait_Unknown";
		}
		int traitLevelIdentifier = UpgradeTraitsData.GetTraitLevelIdentifier(traitDefinition.TraitsGroup);
		string text = traitDefinition.TraitsGroup.Replace("Equipment.", "");
		text = text.Replace("Armor.", "");
		text = UpgradeTraitsData.StripTraitLevelIdentifier(text);
		string text2 = "";
		switch (traitLevelIdentifier)
		{
			case 0:
				text2 = "Low";
				break;
			case 1:
				text2 = "Mid";
				break;
			case 2:
				text2 = "High";
				break;
			case 3:
				text2 = "Highest";
				break;
			default:
				text2 = "";
				break;
		}
		if (string.IsNullOrEmpty(text2))
		{
			return "Ui_Icon_Trait_" + text;
		}
		return "Ui_Icon_Trait_" + text + "_" + text2;
	}

	public static Color GetTraitRarityColor(int rarityLevel)
	{
		if (rarityLevel <= 5)
		{
			return rarityLevel switch
			{
				0 => ColorConvert(169, 184, 190, 255),
				1 => ColorConvert(169, 184, 190, 255),
				//silver
				2 => ColorConvert(169, 184, 190, 255),
				//gold
				3 => ColorConvert(248, 222, 48, 255),
				//master
				4 => ColorConvert(226, 127, 212, 255),
				5 => ColorConvert(226, 127, 212, 255),
				_ => ColorConvert(248, 222, 48, 255),
			};
		}
		return ColorConvert(248, 222, 48, 255);
	}

	public static Color ColorConvert(float r, float g, float b, float a)
	{
		return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
	}

	public static string GetTraitRaritySprite(int rarityLevel)
	{
		string text = "Ui_Border_";
		if (rarityLevel < 5)
		{
			return rarityLevel switch
			{
				0 => text + "Common",
				1 => text + "Rare",
				2 => text + "Epic",
				3 => text + "Legendary",
				4 => text + "Master",
				_ => text + "Common",
			};
		}
		return text + "Legendary";
	}

	public static Texture GetEquipmentIconTextureFromFile(EquipmentItemModel equipment)
	{
		var iconName = equipment.Definition.UseThisWeaponIcon;
		if (string.IsNullOrEmpty(iconName))
		{
			iconName = equipment.Definition.ID;
		}
		string path = "itemgraphics/" + iconName;
		return (Texture)Resources.Load(path);
	}

	public static Texture GetEquipmentIconTextureFromFile(EquipmentDefinition equipmentDefinition)
	{
		var iconName = equipmentDefinition.UseThisWeaponIcon;
		if (string.IsNullOrEmpty(iconName))
		{
			iconName = equipmentDefinition.ID;
		}
		string path = "itemgraphics/" + iconName;
		return (Texture)Resources.Load(path);
	}

	public static Texture GetEquipmentIconTextureFromFile(EquipTokenDefinition equipTokenDefinition)
	{
		var iconName = equipTokenDefinition.UseThisWeaponIcon;
		if (string.IsNullOrEmpty(iconName))
		{
			iconName = equipTokenDefinition.EquipTokenId;
		}
		string path = "itemgraphics/" + iconName;
		return (Texture)Resources.Load(path);
	}
	#endregion
}
