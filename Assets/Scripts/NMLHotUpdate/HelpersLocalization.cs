using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using BaseModel;
using TWDModel;
using UnityEngine;

public class HelpersLocalization : MonoBehaviour
{
	private static string[] OverrideActorIds = new string[960]
	{
		"S7_01A", "Hero_Negan", "S7_01B", "Hero_Negan", "S7_01C", "Hero_Negan", "S7_02A", "Hero_Morgan", "S7_02B", "Hero_Morgan",
		"S7_02C", "Hero_Morgan", "S7_03A", "Hero_Daryl", "S7_03B", "Hero_Daryl", "S7_03C", "Hero_Daryl", "S7_04A", "Hero_Negan",
		"S7_04B", "Hero_Negan", "S7_04C", "Hero_Negan", "S7_05A", "Hero_Negan", "S7_05B", "Hero_Negan", "S7_05C", "Hero_Negan",
		"S7_06A", "Hero_Tara", "S7_06B", "Hero_Tara", "S7_06C", "Hero_Tara", "S7_07A", "Hero_Michonne", "S7_07B", "Hero_Michonne",
		"S7_07C", "Hero_Michonne", "S7_08A", "Hero_Jesus", "S7_08B", "Hero_Jesus", "S7_08C", "Hero_Jesus", "S7_09A", "Hero_Sasha",
		"S7_09B", "Hero_Sasha", "S7_09C", "Hero_Sasha", "S7_10A", "Hero_Rick", "S7_10B", "Hero_Rick", "S7_10C", "Hero_Rick",
		"S7_11A", "Hero_Dwight", "S7_11B", "Hero_Dwight", "S7_11C", "Hero_Dwight", "S7_12A", "Hero_Michonne", "S7_12B", "Hero_Michonne",
		"S7_12C", "Hero_Michonne", "S7_13A", "Hero_Rosita", "S7_13B", "Hero_Rosita", "S7_13C", "Hero_Rosita", "S7_14A", "Hero_Morgan",
		"S7_14B", "Hero_Morgan", "S7_14C", "Hero_Morgan", "S7_15A", "Hero_Daryl", "S7_15B", "Hero_Daryl", "S7_15C", "Hero_Daryl",
		"S7_16A", "Hero_Ezekiel", "S7_16B", "Hero_Ezekiel", "S7_16C", "Hero_Ezekiel", "S3_01A", "Hero_Glenn", "S3_01B", "Hero_Glenn",
		"S3_01C", "Hero_Glenn", "S3_02A", "Hero_Rick", "S3_02B", "Hero_Rick", "S3_02C", "Hero_Rick", "S3_03A", "Hero_Daryl",
		"S3_03B", "Hero_Daryl", "S3_03C", "Hero_Daryl", "S3_04A", "Hero_Carl", "S3_04B", "Hero_Carl", "S3_04C", "Hero_Carl",
		"S3_05A", "Hero_Rick", "S3_05B", "Hero_Rick", "S3_05C", "Hero_Rick", "S3_06A", "Hero_Merle", "S3_06B", "Hero_Merle",
		"S3_06C", "Hero_Merle", "S3_07A", "Hero_Rick", "S3_07B", "Hero_Rick", "S3_07C", "Hero_Rick", "S3_08A", "Hero_Morgan",
		"S3_08B", "Hero_Morgan", "S3_08C", "Hero_Morgan", "S3_09A", "Hero_Michonne", "S3_09B", "Hero_Michonne", "S3_09C", "Hero_Michonne",
		"S8_01A", "Hero_Rick", "S8_01B", "Hero_Rick", "S8_01C", "Hero_Rick", "S8_02A", "Hero_Rick", "S8_02B", "Hero_Rick",
		"S8_02C", "Hero_Rick", "S8_03A", "Hero_Jesus", "S8_03B", "Hero_Jesus", "S8_03C", "Hero_Jesus", "S8_04A", "Hero_Ezekiel",
		"S8_04B", "Hero_Ezekiel", "S8_04C", "Hero_Ezekiel", "S8_05A", "Hero_Negan", "S8_05B", "Hero_Negan", "S8_05C", "Hero_Negan",
		"S8_06A", "Hero_Rosita", "S8_06B", "Hero_Rosita", "S8_06C", "Hero_Rosita", "S8_07A", "Hero_Eugene", "S8_07B", "Hero_Eugene",
		"S8_07C", "Hero_Eugene", "S8_08A", "Hero_Tara", "S8_08B", "Hero_Tara", "S8_08C", "Hero_Tara", "S8_09A", "Hero_Carol",
		"S8_09B", "Hero_Carol", "S8_09C", "Hero_Carol", "S8_10A", "Hero_Michonne", "S8_10B", "Hero_Michonne", "S8_10C", "Hero_Michonne",
		"S8_11A", "Hero_Daryl", "S8_11B", "Hero_Daryl", "S8_11C", "Hero_Daryl", "S8_12A", "Hero_Maggie", "S8_12B", "Hero_Maggie",
		"S8_12C", "Hero_Maggie", "S8_13A", "Hero_Maggie", "S8_13B", "Hero_Maggie", "S8_13C", "Hero_Maggie", "S8_14A", "Hero_Rick",
		"S8_14B", "Hero_Rick", "S8_14C", "Hero_Rick", "S8_15A", "Hero_Rosita", "S8_15B", "Hero_Rosita", "S8_15C", "Hero_Rosita",
		"S8_16A", "Hero_Rick", "S8_16B", "Hero_Rick", "S8_16C", "Hero_Rick", "S9_01A", "Hero_Rick", "S9_01B", "Hero_Rick",
		"S9_01C", "Hero_Rick", "S9_02A", "Hero_Aaron", "S9_02B", "Hero_Aaron", "S9_02C", "Hero_Aaron", "S9_03A", "Hero_Maggie",
		"S9_03B", "Hero_Maggie", "S9_03C", "Hero_Maggie", "S9_04A", "Hero_Michonne", "S9_04B", "Hero_Michonne", "S9_04C", "Hero_Michonne",
		"S9_05A", "Hero_Rick", "S9_05B", "Hero_Rick", "S9_05C", "Hero_Rick", "S9_06A", "Hero_Eugene", "S9_06B", "Hero_Eugene",
		"S9_06C", "Hero_Eugene", "S9_07A", "Hero_Daryl", "S9_07B", "Hero_Daryl", "S9_07C", "Hero_Daryl", "S9_08A", "Hero_Jesus",
		"S9_08B", "Hero_Jesus", "S9_08C", "Hero_Jesus", "S9_09A", "Hero_Daryl", "S9_09B", "Hero_Daryl", "S9_09C", "Hero_Daryl",
		"S9_10A", "Hero_Tara", "S9_10B", "Hero_Tara", "S9_10C", "Hero_Tara", "S9_11A", "Hero_Jerry", "S9_11B", "Hero_Jerry",
		"S9_11C", "Hero_Jerry", "S9_12A", "Hero_Eugene", "S9_12B", "Hero_Eugene", "S9_12C", "Hero_Eugene", "S9_13A", "Hero_Ezekiel",
		"S9_13B", "Hero_Ezekiel", "S9_13C", "Hero_Ezekiel", "S9_14A", "Hero_Michonne", "S9_14B", "Hero_Michonne", "S9_14C", "Hero_Michonne",
		"S9_15A", "Hero_Carol", "S9_15B", "Hero_Carol", "S9_15C", "Hero_Carol", "S9_16A", "Hero_Aaron", "S9_16B", "Hero_Aaron",
		"S9_16C", "Hero_Aaron", "S10_01A", "Hero_Michonne", "S10_01B", "Hero_Michonne", "S10_01C", "Hero_Michonne", "S10_02A", "Hero_Alpha",
		"S10_02B", "Hero_Alpha", "S10_02C", "Hero_Alpha", "S10_03A", "Hero_Rosita", "S10_03B", "Hero_Rosita", "S10_03C", "Hero_Rosita",
		"S10_04A", "Hero_Ezekiel", "S10_04B", "Hero_Ezekiel", "S10_04C", "Hero_Ezekiel", "S10_05A", "Hero_Negan", "S10_05B", "Hero_Negan",
		"S10_05C", "Hero_Negan", "S10_06A", "Hero_Daryl", "S10_06B", "Hero_Daryl", "S10_06C", "Hero_Daryl", "S10_07A", "Hero_Aaron",
		"S10_07B", "Hero_Aaron", "S10_07C", "Hero_Aaron", "S10_08A", "Hero_Daryl", "S10_08B", "Hero_Daryl", "S10_08C", "Hero_Daryl",
		"S10_09A", "Hero_Daryl", "S10_09B", "Hero_Daryl", "S10_09C", "Hero_Daryl", "S10_10A", "Hero_Rosita", "S10_10B", "Hero_Rosita",
		"S10_10C", "Hero_Rosita", "S10_11A", "Hero_Ezekiel", "S10_11B", "Hero_Ezekiel", "S10_11C", "Hero_Ezekiel", "S10_12A", "Hero_Aaron",
		"S10_12B", "Hero_Aaron", "S10_12C", "Hero_Aaron", "S10_13A", "Hero_Michonne", "S10_13B", "Hero_Michonne", "S10_13C", "Hero_Michonne",
		"S10_14A", "Hero_Negan", "S10_14B", "Hero_Negan", "S10_14C", "Hero_Negan", "S10_15A", "Hero_Ezekiel", "S10_15B", "Hero_Ezekiel",
		"S10_15C", "Hero_Ezekiel", "S10_16A", "Hero_Jerry", "S10_16B", "Hero_Jerry", "S10_16C", "Hero_Jerry", "S1_02A", "Unique_Glenn_S1",
		"S1_02B", "Unique_Glenn_S1", "S1_02C", "Unique_Glenn_S1", "S1_03A", "Unique_Rick_S1", "S1_03B", "Unique_Rick_S1", "S1_03C", "Unique_Rick_S1",
		"S1_04A", "Unique_Rick_S1", "S1_04B", "Unique_Rick_S1", "S1_04C", "Unique_Rick_S1", "S1_05A", "Unique_Rick_S1", "S1_05B", "Unique_Rick_S1",
		"S1_05C", "Unique_Rick_S1", "S1_06A", "Hero_Maggie", "S1_06B", "Hero_Maggie", "S1_06C", "Hero_Maggie", "S1_07A", "Hero_Shane",
		"S1_07B", "Hero_Shane", "S1_07C", "Hero_Shane", "S1_08_1A", "Hero_Maggie", "S1_08_1B", "Hero_Maggie", "S1_08_1C", "Hero_Maggie",
		"S1_08_2A", "Hero_ScoutDaryl", "S1_08_2B", "Hero_ScoutDaryl", "S1_08_2C", "Hero_ScoutDaryl", "S10C_01A", "Hero_Daryl", "S10C_01B", "Hero_Daryl",
		"S10C_01C", "Hero_Daryl", "S10C_02A", "Hero_Carol", "S10C_02B", "Hero_Carol", "S10C_02C", "Hero_Carol", "S10C_03A", "Hero_Princess",
		"S10C_03B", "Hero_Princess", "S10C_03C", "Hero_Princess", "S10C_04A", "Hero_Gabriel", "S10C_04B", "Hero_Gabriel", "S10C_04C", "Hero_Gabriel",
		"S10C_05A", "Hero_ScoutDaryl", "S10C_05B", "Hero_ScoutDaryl", "S10C_05C", "Hero_ScoutDaryl", "S10C_06A", "Hero_Negan", "S10C_06B", "Hero_Negan",
		"S10C_06C", "Hero_Negan", "S11_01A", "Unique_Carol_S10", "S11_01B", "Unique_Carol_S10", "S11_01C", "Unique_Carol_S10", "S11_02A", "Hero_ShooterMaggie",
		"S11_02B", "Hero_ShooterMaggie", "S11_02C", "Hero_ShooterMaggie", "S11_03A", "Hero_Negan", "S11_03B", "Hero_Negan", "S11_03C", "Hero_Negan",
		"S11_04A", "Hero_Daryl", "S11_04B", "Hero_Daryl", "S11_04C", "Hero_Daryl", "S11_05A", "Hero_Jerry", "S11_05B", "Hero_Jerry",
		"S11_05C", "Hero_Jerry", "S11_06A", "Unique_Carol_S10", "S11_06B", "Unique_Carol_S10", "S11_06C", "Unique_Carol_S10", "S11_07A", "Hero_Eugene",
		"S11_07B", "Hero_Eugene", "S11_07C", "Hero_Eugene", "S11_08A", "Hero_Negan", "S11_08B", "Hero_Negan", "S11_08C", "Hero_Negan",
		"S11_09A", "Hero_ShooterMaggie", "S11_09B", "Hero_ShooterMaggie", "S11_09C", "Hero_ShooterMaggie", "S11_10A", "Hero_ScoutDaryl", "S11_10B", "Hero_ScoutDaryl",
		"S11_10C", "Hero_ScoutDaryl", "S11_11A", "Hero_Mercer", "S11_11B", "Hero_Mercer", "S11_11C", "Hero_Mercer", "S11_12A", "Hero_ShooterMaggie",
		"S11_12B", "Hero_ShooterMaggie", "S11_12C", "Hero_ShooterMaggie", "S11_13A", "Hero_Gabriel", "S11_13B", "Hero_Gabriel", "S11_13C", "Hero_Gabriel",
		"S11_14A", "Hero_Daryl", "S11_14B", "Hero_Daryl", "S11_14C", "Hero_Daryl", "S11_15A", "Hero_Aaron", "S11_15B", "Hero_Aaron",
		"S11_15C", "Hero_Aaron", "S11_16A", "Hero_Daryl", "S11_16B", "Hero_Daryl", "S11_16C", "Hero_Daryl", "S3_01A1", "Hero_HunterHershel",
		"S3_01B1", "Hero_HunterHershel", "S3_01C1", "Hero_HunterHershel", "S4_2A", "Hero_Tyreese", "S4_2B", "Hero_Tyreese", "S4_2C", "Hero_Tyreese",
		"S4_4A", "Hero_Rick", "S4_4B", "Hero_Rick", "S4_4C", "Hero_Rick", "S4_11A", "Hero_Michonne", "S4_11B", "Hero_Michonne",
		"S4_11C", "Hero_Michonne", "S4_12A", "Hero_Daryl", "S4_12B", "Hero_Daryl", "S4_12C", "Hero_Daryl", "S4_14A", "Hero_Tyreese",
		"S4_14B", "Hero_Tyreese", "S4_14C", "Hero_Tyreese", "S4_15A", "Hero_BruiserGlenn", "S4_15B", "Hero_BruiserGlenn", "S4_15C", "Hero_BruiserGlenn",
		"S5_1A", "Hero_Carol", "S5_1B", "Hero_Carol", "S5_1C", "Hero_Carol", "S5_2A", "Hero_Carl", "S5_2B", "Hero_Carl",
		"S5_2C", "Hero_Carl", "S11_17A", "Hero_Daryl", "S11_17B", "Hero_Daryl", "S11_17C", "Hero_Daryl", "S11_18A", "Hero_Ezekiel",
		"S11_18B", "Hero_Ezekiel", "S11_18C", "Hero_Ezekiel", "S11_19A", "Hero_Aaron", "S11_19B", "Hero_Aaron", "S11_19C", "Hero_Aaron",
		"S11_20A", "Hero_AssassinCarol", "S11_20B", "Hero_AssassinCarol", "S11_20C", "Hero_AssassinCarol", "S11_21A", "Hero_BruiserRosita", "S11_21B", "Hero_BruiserRosita",
		"S11_21C", "Hero_BruiserRosita", "S11_22A", "Hero_Maggie", "S11_22B", "Hero_Maggie", "S11_22C", "Hero_Maggie", "S11_23A", "Hero_Negan",
		"S11_23B", "Hero_Negan", "S11_23C", "Hero_Negan", "S11_24A", "Hero_Mercer", "S11_24B", "Hero_Mercer", "S11_24C", "Hero_Mercer",
		"S20_01A", "Hero_CowboyNegan", "S20_01B", "Hero_CowboyNegan", "S20_01C", "Hero_CowboyNegan", "S20_02A", "Hero_CowboyNegan", "S20_02B", "Hero_CowboyNegan",
		"S20_02C", "Hero_CowboyNegan", "S20_03A", "Hero_CowboyNegan", "S20_03B", "Hero_CowboyNegan", "S20_03C", "Hero_CowboyNegan", "S20_04A", "Hero_ShooterMaggie",
		"S20_04B", "Hero_ShooterMaggie", "S20_04C", "Hero_ShooterMaggie", "S20_05A", "Hero_ShooterMaggie", "S20_05B", "Hero_ShooterMaggie", "S20_05C", "Hero_ShooterMaggie",
		"S20_06A", "Hero_ShooterMaggie", "S20_06B", "Hero_ShooterMaggie", "S20_06C", "Hero_ShooterMaggie", "S21_01A", "Hero_Daryl", "S21_01B", "Hero_Daryl",
		"S21_01C", "Hero_Daryl", "S21_02A", "Hero_Daryl", "S21_02B", "Hero_Daryl", "S21_02C", "Hero_Daryl", "S21_03A", "Hero_Daryl",
		"S21_03B", "Hero_Daryl", "S21_03C", "Hero_Daryl", "S21_04A", "Hero_Daryl", "S21_04B", "Hero_Daryl", "S21_04C", "Hero_Daryl",
		"S21_05A", "Hero_Daryl", "S21_05B", "Hero_Daryl", "S21_05C", "Hero_Daryl", "S21_06A", "Hero_Daryl", "S21_06B", "Hero_Daryl",
		"S21_06C", "Hero_Daryl", "S22_01A", "Hero_ScoutRick", "S22_01B", "Hero_ScoutRick", "S22_01C", "Hero_ScoutRick", "S22_02A", "Hero_Michonne",
		"S22_02B", "Hero_Michonne", "S22_02C", "Hero_Michonne", "S22_03A", "Hero_ScoutRick", "S22_03B", "Hero_ScoutRick", "S22_03C", "Hero_ScoutRick",
		"S22_04A", "Hero_Michonne", "S22_04B", "Hero_Michonne", "S22_04C", "Hero_Michonne", "S22_05A", "Hero_Jadis", "S22_05B", "Hero_Jadis",
		"S22_05C", "Hero_Jadis", "S22_06A", "Hero_ScoutRick", "S22_06B", "Hero_ScoutRick", "S22_06C", "Hero_ScoutRick", "S23_01A", "Hero_Carol",
		"S23_01B", "Hero_Carol", "S23_01C", "Hero_Carol", "S23_02A", "Hero_Daryl", "S23_02B", "Hero_Daryl", "S23_02C", "Hero_Daryl",
		"S23_03A", "Hero_ProtectorDaryl", "S23_03B", "Hero_ProtectorDaryl", "S23_03C", "Hero_ProtectorDaryl", "S23_04A", "Hero_Daryl", "S23_04B", "Hero_Daryl",
		"S23_04C", "Hero_Daryl", "S23_05A", "Hero_Carol", "S23_05B", "Hero_Carol", "S23_05C", "Hero_Carol", "S23_06A", "Hero_Daryl",
		"S23_06B", "Hero_Daryl", "S23_06C", "Hero_Daryl", "S24_01A", "Unique_MoonlightMaggie", "S24_01B", "Unique_MoonlightMaggie", "S24_01C", "Unique_MoonlightMaggie",
		"S24_02A", "Unique_MoonlightMaggie", "S24_02B", "Unique_MoonlightMaggie", "S24_02C", "Unique_MoonlightMaggie", "S24_03A", "Unique_MoonlightMaggie", "S24_03B", "Unique_MoonlightMaggie",
		"S24_03C", "Unique_MoonlightMaggie", "S24_04A", "Hero_Negan", "S24_04B", "Hero_Negan", "S24_04C", "Hero_Negan", "S24_05A", "Hero_Perlie",
		"S24_05B", "Hero_Perlie", "S24_05C", "Hero_Perlie", "S24_06A", "Unique_MoonlightMaggie", "S24_06B", "Unique_MoonlightMaggie", "S24_06C", "Unique_MoonlightMaggie",
		"S24_07A", "Hero_Negan", "S24_07B", "Hero_Negan", "S24_07C", "Hero_Negan", "S24_08A", "Hero_Perlie", "S24_08B", "Hero_Perlie",
		"S24_08C", "Hero_Perlie", "S26_01A", "Hero_ScoutMaggie", "S26_01B", "Hero_Negan", "S26_01C", "Hero_Croat", "S26_02A", "Hero_ShooterMaggie",
		"S26_02B", "Hero_Negan", "S26_02C", "Hero_Perlie", "S26_03A", "Hero_ShooterMaggie", "S26_03B", "Unique_Henchman_A", "S26_03C", "Unique_Henchman_D",
		"S26_04A", "Hero_Negan", "S26_04B", "Hero_ScoutMaggie", "S26_04C", "Unique_Henchman_D", "S26_05A", "Hero_Negan", "S26_05B", "Hero_Maggie",
		"S26_05C", "Unique_Curtis", "S26_06A", "Hero_Maggie", "S26_06B", "Hero_Negan", "S26_06C", "Unique_Henchman_B", "S26_07A", "Hero_Negan",
		"S26_07B", "Hero_ShooterMaggie", "S26_07C", "Hero_ShooterMaggie", "S26_08A", "Hero_ScoutMaggie", "S26_08B", "Hero_Negan", "S26_08C", "Unique_Bud"
	};

	public static string ReplaceTripleSpaceWithNewline(string text)
	{
		return Regex.Replace(text, "(?<=(   )(?!$))", "$&\n$&\n");
	}

	public static string GetTradeItemName(TradeDefinition item)
	{
		return LocalizationManager.GetText("TradeItems.Name." + item.UniqueId);
	}

	public static string GetTradeCrateName(string itemId)
	{
		return LocalizationManager.GetText("TradeItems.Name." + itemId);
	}

	public static string GetSeasonTitle(string seasonId)
	{
		return LocalizationManager.GetText("Season." + seasonId + ".Title");
	}

	public static string GetSeasonSubtitle(string seasonId)
	{
		return LocalizationManager.GetText("Season." + seasonId + ".Subtitle");
	}

	public static string GetSeasonRewardDescription(string seasonId)
	{
		return LocalizationManager.GetText("Season." + seasonId + ".RewardDescription");
	}

	public static string GetSeasonEpisodeName(string mapId)
	{
		return LocalizationManager.GetText("Map.Episode.Season." + mapId + ".Name");
	}

	public static string GetDefensiveModeDescription(AIMode mode)
	{
		return LocalizationManager.GetText("Popup.Outpost.Edit.SelectDefender." + mode);
	}

	public static string GetCurrencyDescription(CurrencyType currencyType)
	{
		return LocalizationManager.GetText("Currency.Description." + currencyType);
	}

	public static string GetCurrencyName(CurrencyType currencyType)
	{
		if (currencyType == CurrencyType.CampaignToken)
		{
			CampaignDefinition currentCampaignDefinition = GameManager.Instance.playerModel.CampaignModel.GetCurrentCampaignDefinition();
			if (currentCampaignDefinition != null)
			{
				return LocalizationManager.GetText(currentCampaignDefinition.CampaignTokenLocKey);
			}
		}
		if (currencyType == CurrencyType.Fairmoney)
		{
			return LocalizationManager.GetText("Currency.FairmoneyReal");
		}
		return LocalizationManager.GetText("Currency." + currencyType);
	}

	public static string GetSpeedCurrencyName(CurrencyType currencyType)
	{
		return LocalizationManager.GetText("Currency.Name.SpeedUp.Display." + currencyType);
	}

	public static string GetSpeedCurrencyShortName(CurrencyType currencyType)
	{
		return LocalizationManager.GetText("Currency.Name.SpeedUp.Use." + currencyType);
	}

	public static string GetDropCurrencyName(DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency)
	{
		return LocalizationManager.GetText("DropCurrency." + dropCurrency);
	}

	public static string GetCurrencyContext(CurrencyType currencyType)
	{
		return LocalizationManager.GetText("Currency.Context." + currencyType);
	}

	public static string GetBattlePassTokenName(CurrencyType currencyType)
	{
		string text = "";
		string text2 = currencyType.ToString();
		if (text2.Substring(0, 5).ToUpper() == "SUPER")
		{
			text = text2.Substring(5, text2.Length - 5);
			if (currencyType == CurrencyType.SuperEquipmentTokenBP)
			{
				return LocalizationManager.GetText("BattlePass.SpeedupToken.Type.WorkshopToken+");
			}
			return LocalizationManager.GetText("BattlePass.SpeedupToken.Type." + text + "+");
		}
		if (currencyType == CurrencyType.EquipmentTokenBP)
		{
			return LocalizationManager.GetText("BattlePass.SpeedupToken.Type.WorkshopToken");
		}
		text = text2.Substring(0, text2.Length);
		return LocalizationManager.GetText("BattlePass.SpeedupToken.Type." + text);
	}

	public static string GetHeroName(string actorDefinitionId)
	{
		ActorDefinition actorDefinition = GameManager.Instance.gameEconomyData.GetActorDefinition(actorDefinitionId);
		if (actorDefinition != null)
		{
			return GetCurrencyContext(actorDefinition.TraitUpgradeCurrency);
		}
		return "";
	}

	public static string GetBuildingName(BuildingModel buildingModel)
	{
		return LocalizationManager.GetText("Building.Name." + buildingModel.TypeName);
	}

	public static string GetBuildingName(string typeName)
	{
		return LocalizationManager.GetText("Building.Name." + typeName);
	}

	public static string GetBuildingDescription(string typeName)
	{
		return LocalizationManager.GetText("Building.Description." + typeName);
	}

	public static string GetEquipmentName(EquipmentItemModel equipmentItemModel)
	{
		string useThisLocalizationName = equipmentItemModel.Definition.UseThisLocalizationName;
		if (!string.IsNullOrEmpty(useThisLocalizationName))
		{
			return LocalizationManager.GetText(useThisLocalizationName);
		}
		return LocalizationManager.GetText("Equipment.Name." + equipmentItemModel.Definition.ID);
	}

	public static string GetEquipmentTokenName(EquipTokenItemModel equipTokenItemModel)
	{
		string useThisLocalizationName = equipTokenItemModel.Definition.UseThisLocalizationName;
		if (!string.IsNullOrEmpty(useThisLocalizationName))
		{
			return LocalizationManager.GetText(useThisLocalizationName);
		}
		string equipmentName = GetEquipmentName(equipTokenItemModel.Definition.RelateEquipId);
		return LocalizationManager.GetText("EquipToken.Name.NoAmount", equipmentName) + "noamoutddl";
	}

	public static string GetEquipmentName(string equipmentId)
	{
		EquipmentDefinition equipmentDefinition = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(equipmentId);
		if (equipmentDefinition == null)
		{
			return LocalizationManager.GetText("Equipment.Name." + equipmentId);
		}
		string useThisLocalizationName = equipmentDefinition.UseThisLocalizationName;
		if (!string.IsNullOrEmpty(useThisLocalizationName))
		{
			return LocalizationManager.GetText(useThisLocalizationName);
		}
		return LocalizationManager.GetText("Equipment.Name." + equipmentId);
	}

	public static string GetEquipmentNameMultiple(string equipmentId, int amount)
	{
		string useThisLocalizationName = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(equipmentId).UseThisLocalizationName;
		if (!string.IsNullOrEmpty(useThisLocalizationName))
		{
			return LocalizationManager.GetText(useThisLocalizationName + "{Amount}", amount);
		}
		return LocalizationManager.GetText("Equipment.Name." + equipmentId + "{Amount}", amount);
	}

	public static string GetEquipmentTokenNameMultiple(string equipmentTokenId, int amount)
	{
		string useThisLocalizationName = GameManager.Instance.gameEconomyData.GetEquipTokenDefinition(equipmentTokenId).UseThisLocalizationName;
		if (!string.IsNullOrEmpty(useThisLocalizationName))
		{
			return LocalizationManager.GetText(useThisLocalizationName + "{Amount}", amount);
		}
		string equipmentName = GetEquipmentName(GameManager.Instance.gameEconomyData.GetEquipTokenDefinition(equipmentTokenId).RelateEquipId);
		return LocalizationManager.GetText("EquipToken.Name.Amount", amount, equipmentName);
	}

	public static string GetEquipmentDescription(string equipmentId)
	{
		string useThisLocalizationDescription = GameManager.Instance.gameEconomyData.GetEquipmentDefinition(equipmentId).UseThisLocalizationDescription;
		if (!string.IsNullOrEmpty(useThisLocalizationDescription))
		{
			return LocalizationManager.GetText(useThisLocalizationDescription);
		}
		return LocalizationManager.GetText("Equipment.Description." + equipmentId);
	}

	public static string GetEquipmentSpecialDescription(EquipmentDefinition equipmentDefinition)
	{
		AbilityDefinition abilityDefinition = GameManager.Instance.gameEconomyData.GetAbilityDefinition(equipmentDefinition.AbilityIdentifier);
		if (!string.IsNullOrEmpty(abilityDefinition.SpecialDescriptionKey))
		{
			return LocalizationManager.GetText(abilityDefinition.SpecialDescriptionKey);
		}
		return "";
	}

	public static string GetGvGInfusedWeaponSpecialDescription(EquipmentDefinition equipmentDefinition)
	{
		TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(equipmentDefinition.InfusedTrait);
		string text = SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Popup.EquipmentInfoPopup.InfusedTrait") + ":\n";
		if (traitDefinition == null)
		{
			Debug.LogError("Could not find trait definition for EquipmentItemModel:" + equipmentDefinition.ID + " - " + equipmentDefinition.ActiveTraits[0] + "," + equipmentDefinition.Type.ToString() + "," + equipmentDefinition.Category);
		}
		else
		{
			UpgradeTraitsData upgradeTraitsData = new UpgradeTraitsData();
			upgradeTraitsData.Identifier = traitDefinition.Identifier;
			text += GetInstantiatedTraitDescription(upgradeTraitsData);
		}
		return text;
	}

	public static string GetEquipmentStatName(EquipmentItemModel equipmentItemModel)
	{
		if (equipmentItemModel.Definition.Category == EquipmentCategory.Armor)
		{
			return LocalizationManager.GetText("Equipment.Stat.Defense");
		}
		return LocalizationManager.GetText("Equipment.Stat.Damage");
	}

	public static string GetEquipmentModifierName(EquipmentModifierDefinition equipmentModifierDefinition)
	{
		return LocalizationManager.GetText("Equipment.Modifier.Name." + equipmentModifierDefinition.DisplayName);
	}

	public static string GetEquipmentModifierDescription(EquipmentModifierDefinition equipmentModifierDefinition)
	{
		return LocalizationManager.GetText("Equipment.Modifier.Description." + equipmentModifierDefinition.DisplayName);
	}

	public static string GetSurvivorClassName(SurvivorClass survivorClass)
	{
		return LocalizationManager.GetText("Survivor.Class." + survivorClass);
	}

	public static string GetSurvivorClassName(string survivorClass)
	{
		return LocalizationManager.GetText("Survivor.Class." + survivorClass);
	}

	public static string GetSurvivorClassDescription(SurvivorClass survivorClass)
	{
		return LocalizationManager.GetText("Survivor.Class.Description." + survivorClass);
	}

	public static string GetTraitName(TraitDefinition traitDefinition)
	{
		return GetTraitName(traitDefinition.DisplayName);
	}

	public static string GetTraitName(string traitDisplayName)
	{
		return LocalizationManager.GetText(traitDisplayName);
	}

	public static string GetTraitDescription(TraitDefinition traitDefinition)
	{
		string textId = traitDefinition.DisplayName + ".Description{Parameter}";
		object[] arguments = traitDefinition.ConstructionParameters.ToArray();
		return LocalizationManager.GetText(textId, arguments);
	}

	public static string GetTraitDescription(string traitId)
	{
		string result = "";
		TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(traitId);
		if (traitDefinition != null)
		{
			result = GetTraitDescription(traitDefinition);
		}
		return result;
	}

	public static string GetApocalypticDescription(WeeklyChallengeApocalypseBuff apocalypseBuff)
	{
		string description = apocalypseBuff.Description;
		object[] getConstructionParameters = apocalypseBuff.GetConstructionParameters;
		return LocalizationManager.GetText(description, getConstructionParameters);
	}

	public static string GetLeaderTraitTeamDescription(TraitDefinition traitDefinition)
	{
		string textId = traitDefinition.DisplayName + ".Description.Team{Parameter}";
		object[] arguments = traitDefinition.ConstructionParameters.ToArray();
		return LocalizationManager.GetText(textId, arguments);
	}

	public static string GetInstantiatedTraitDescription(UpgradeTraitsData traitsData)
	{
		TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(traitsData.Identifier);
		if (traitDefinition == null)
		{
			Debug.LogError("Trait definition not found for " + traitsData.Identifier);
			return "";
		}
		int num = ((traitDefinition.ConstructionParameters != null) ? traitDefinition.ConstructionParameters.Count : 0);
		object[] array = new object[num];
		for (int i = 0; i < num; i++)
		{
			string s = traitDefinition.ConstructionParameters[i];
			if ((traitsData.RemodelIng || traitsData.RemodelEd) && traitsData.ThisRemodeValues.TryGetValue(traitsData.Identifier, out var value) && traitsData.ThisRemodeParamIndex.TryGetValue(traitsData.Identifier, out var value2))
			{
				for (int j = 0; j < value.Count && j < value2.Count; j++)
				{
					if (value2[j] == i)
					{
						s = value[j].ToString();
					}
				}
			}
			float result2;
			if (int.TryParse(s, out var result))
			{
				float num2 = 1f + (float)traitsData.ConstructionMultiplier / 100f;
				array[i] = (int)Math.Round((float)result * num2);
			}
			else if (float.TryParse(s, out result2))
			{
				float num3 = 1f + (float)traitsData.ConstructionMultiplier / 100f;
				array[i] = (float)Math.Round(result2 * num3, 1);
			}
		}
		if (num > 0)
		{
			return LocalizationManager.GetText(traitDefinition.DisplayName + ".Description{Parameter}", array);
		}
		return LocalizationManager.GetText(traitDefinition.DisplayName + ".Description{Parameter}", 0);
	}

	public static string GetLastInstantiatedTraitDescription(UpgradeTraitsData traitsData)
	{
		TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(traitsData.Identifier);
		if (traitDefinition == null)
		{
			Debug.LogError("Trait definition not found for " + traitsData.Identifier);
			return "";
		}
		int num = ((traitDefinition.ConstructionParameters != null) ? traitDefinition.ConstructionParameters.Count : 0);
		object[] array = new object[num];
		for (int i = 0; i < num; i++)
		{
			string s = traitDefinition.ConstructionParameters[i];
			if (traitsData.RemodeValues != null && traitsData.ThisRemodeParamIndex.TryGetValue(traitsData.Identifier, out var value))
			{
				for (int j = 0; j < traitsData.RemodeValues.Count && j < value.Count; j++)
				{
					if (value[j] == i)
					{
						s = traitsData.RemodeValues[j].ToString();
					}
				}
			}
			float result2;
			if (int.TryParse(s, out var result))
			{
				float num2 = 1f + (float)traitsData.ConstructionMultiplier / 100f;
				array[i] = (int)Math.Round((float)result * num2);
			}
			else if (float.TryParse(s, out result2))
			{
				float num3 = 1f + (float)traitsData.ConstructionMultiplier / 100f;
				array[i] = (float)Math.Round(result2 * num3, 1);
			}
		}
		if (num > 0)
		{
			return LocalizationManager.GetText(traitDefinition.DisplayName + ".Description{Parameter}", array);
		}
		return LocalizationManager.GetText(traitDefinition.DisplayName + ".Description{Parameter}", 0);
	}

	public static string GetChallengeDebuffDescription(DifficultyIncrementalDebuff traitDefinition)
	{
		if (traitDefinition == null)
		{
			Debug.LogError("Challenge Debuff definition not found");
			return "";
		}
		int num = ((traitDefinition.ConstructionParameters != null) ? traitDefinition.ConstructionParameters.Count : 0);
		object[] array = new object[num];
		for (int i = 0; i < num; i++)
		{
			int result;
			float result2;
			if (traitDefinition.DebuffType == ChallengeDebuffType.Supportcooldown)
			{
				if (traitDefinition.ConstructionParameters != null && int.TryParse(traitDefinition.ConstructionParameters[i].ToString(), out result))
				{
					SupportDefinitionRaw supportDefinitions = GameManager.Instance.gameEconomyData.GetSupportDefinitions(result);
					array[i] = supportDefinitions.Identifier;
				}
			}
			else if (traitDefinition.ConstructionParameters != null && int.TryParse(traitDefinition.ConstructionParameters[i].ToString(), out result))
			{
				float num2 = 1f;
				array[i] = (int)Math.Round((float)result * num2);
			}
			else if (traitDefinition.ConstructionParameters != null && float.TryParse(traitDefinition.ConstructionParameters[i].ToString(), out result2))
			{
				float num3 = 1f;
				array[i] = (float)Math.Round(result2 * num3, 1);
			}
		}
		if (num > 0)
		{
			return LocalizationManager.GetText(traitDefinition.Description, array);
		}
		return LocalizationManager.GetText(traitDefinition.Description, 0);
	}

	public static string GetChargeEquipmentTraitDescription(EquipmentItemModel equipment)
	{
		return LocalizationManager.GetText("Traits." + equipment.Definition.ChargeEquipmentIdentifier + ".Description");
	}

	public static string GetMissionTypeName(MissionType missionType)
	{
		return LocalizationManager.GetText("Mission.Type." + missionType);
	}

	public static string GetMissionName(string missionTextID)
	{
		return LocalizationManager.GetText("Mission." + missionTextID + ".Title");
	}

	public static string GetMissionBody(string missionTextID)
	{
		return LocalizationManager.GetText("Mission." + missionTextID + ".Body");
	}

	public static string GetMissionBriefing(string missionTextID)
	{
		string text = LocalizationManager.GetText("Mission." + missionTextID + ".Briefing");
		Match match = new Regex("<<(.*)>>|>>(.*)<<").Match(text);
		if (match.Success && match.Groups[0] != null)
		{
			text = text.Replace(match.Groups[0].ToString(), "");
		}
		return text;
	}

	public static string GetSurvivalMissionName(SurvivalMissionConfig survivalConfig)
	{
		if (survivalConfig == null || string.IsNullOrEmpty(survivalConfig.TitleDisplayLocale))
		{
			return "";
		}
		return LocalizationManager.GetText("Map.Episode.Title.Survival." + survivalConfig.TitleDisplayLocale);
	}

	public static string GetSurvivalMissionBriefing(SurvivalMissionConfig survivalConfig, SurvivalSavedMissionModel saveData)
	{
		if (survivalConfig == null || survivalConfig.BriefingDisplayLocale == "")
		{
			return "";
		}
		if (SingularityMonoBehaviour<LocalizationManager>.Instance == null)
		{
			return "";
		}
		string text = "Map.Episode.Briefing.Survival.";
		string text2 = "Map.Episode.Briefing.Survival.Single.";
		string text3 = "Map.Episode.Briefing.Survival.Zero.";
		switch (survivalConfig.ObjectiveType)
		{
		case SurvivalMissionConfig.SurvivalObjectiveType.KillAmountAndExit:
		{
			int num2 = survivalConfig.KillsRequired;
			if (saveData != null && saveData.DoesSavedMissionDataExist)
			{
				string presetVariableName = PersistentMissionVariable.GetPresetVariableName(PersistentVariablePresetName.SurvivalKillCountLeft);
				if (saveData.DoesPersistentIntVariableValueExist(presetVariableName))
				{
					num2 = saveData.GetPersistentIntVariableValue(presetVariableName);
				}
			}
			switch (num2)
			{
			case 0:
				if (SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text3 + survivalConfig.BriefingDisplayLocale))
				{
					return LocalizationManager.GetText(text3 + survivalConfig.BriefingDisplayLocale, num2);
				}
				return LocalizationManager.GetText(text + "GoToExit", num2);
			case 1:
				if (SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text2 + survivalConfig.BriefingDisplayLocale))
				{
					return LocalizationManager.GetText(text2 + survivalConfig.BriefingDisplayLocale, num2);
				}
				break;
			}
			return LocalizationManager.GetText(text + survivalConfig.BriefingDisplayLocale, num2);
		}
		case SurvivalMissionConfig.SurvivalObjectiveType.KillBossAndExit:
		{
			int num = 0;
			for (int i = 0; i < SurvivalMissionConfig.SupportedWalkerTypes.Length; i++)
			{
				WalkerType walkerType = SurvivalMissionConfig.SupportedWalkerTypes[i];
				if (!survivalConfig.IsWalkerTypeBoss(walkerType))
				{
					continue;
				}
				int numWalkersOfType = survivalConfig.GetNumWalkersOfType(walkerType);
				if (saveData != null && saveData.DoesSavedMissionDataExist)
				{
					for (int j = 0; j < SurvivalMissionConfig.CountedTags.Length; j++)
					{
						int actorTag = SurvivalMissionConfig.CountedTags[j];
						if (saveData.IsCountSavedForWalker(walkerType, actorTag))
						{
							num += saveData.ClampSpawnCountForWalker(numWalkersOfType, walkerType, actorTag);
						}
					}
				}
				else
				{
					num += numWalkersOfType;
				}
			}
			switch (num)
			{
			case 0:
				if (SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text3 + survivalConfig.BriefingDisplayLocale))
				{
					return LocalizationManager.GetText(text3 + survivalConfig.BriefingDisplayLocale, num);
				}
				return LocalizationManager.GetText(text + "GoToExit", num);
			case 1:
				if (SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(text2 + survivalConfig.BriefingDisplayLocale))
				{
					return LocalizationManager.GetText(text2 + survivalConfig.BriefingDisplayLocale, num);
				}
				break;
			}
			return LocalizationManager.GetText(text + survivalConfig.BriefingDisplayLocale, num);
		}
		default:
			return LocalizationManager.GetText(text + survivalConfig.BriefingDisplayLocale);
		}
	}

	public static string GetMissionDifficulty(MissionDifficulty difficulty)
	{
		return LocalizationManager.GetText("Mission.Difficulty." + difficulty);
	}

	public static string GetRarityLevel(int rarityLevel)
	{
		string text = "";
		switch (rarityLevel)
		{
		case 0:
			return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Rarity.Common");
		case 1:
			return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Rarity.Uncommon");
		case 2:
			return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Rarity.Rare");
		case 3:
			return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Rarity.Epic");
		case 4:
			return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Rarity.Legendary");
		case 5:
		case 6:
			return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Rarity.Apocalyptic");
		default:
			return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Rarity.Elite");
		}
	}

	public static string GetComponentRewardName(CurrencyType currencyType, int amount, int rarity = -1, bool colorRarity = false)
	{
		string componentTypeName = ComponentHelper.GetComponentTypeName(currencyType);
		string text = LocalizationManager.GetText("Component." + componentTypeName + ".Name" + ((amount != 1) ? ".Plural" : ""));
		string text2 = "";
		if (rarity > -1)
		{
			text2 = GetRarityLevel(ComponentHelper.GetComponentRarityLevel(currencyType));
			if (colorRarity)
			{
				ColorEntry rarityColorData = GameManager.Instance.GetRarityColorData(rarity);
				if (rarityColorData != null)
				{
					text2 = "[" + ColorUtility.ToHtmlStringRGB(rarityColorData.GradientColorBottom) + "]" + text2 + "[-] ";
				}
			}
			return amount + " x " + text2 + " " + text;
		}
		return amount + " " + text;
	}

	public static string GetComponentName(CurrencyType currencyType)
	{
		string componentTypeName = ComponentHelper.GetComponentTypeName(currencyType);
		return LocalizationManager.GetText("Component." + componentTypeName + ".Name");
	}

	public static string GetGuildMemberRole(GuildMemberInfo guildMemberInfo)
	{
		return LocalizationManager.GetText("Generic.Guild.Member.Role." + guildMemberInfo.Role);
	}

	public static string GetGuildPurpose(string purpose)
	{
		if (purpose != null && purpose.Length > 0)
		{
			return LocalizationManager.GetText("Generic.Guild.PurposeType." + char.ToUpperInvariant(purpose[0]) + purpose.Substring(1).ToLowerInvariant());
		}
		return LocalizationManager.GetText("Generic.Guild.PurposeType." + purpose);
	}

	public static string GetEpisodeName(MissionSpawnPointGroup group)
	{
		if (group != null)
		{
			return LocalizationManager.GetText("Map.Episode.Name." + group.DisplayName);
		}
		return "";
	}

	public static string GetEpisodeTitle(MapMissionGroupModel mapMissionGroupModel)
	{
		if (mapMissionGroupModel.IsWeeklyChallenge || mapMissionGroupModel.IsInApocalyptiWeeklyChallenge)
		{
			return GetWeeklyChallengeTitle();
		}
		return LocalizationManager.GetText("Map.Episode.Title." + mapMissionGroupModel.MissionSpawnPointGroup.DisplayName);
	}

	public static string GetEpisodeTitleComplete(MapMissionGroupModel mapMissionGroupModel)
	{
		return LocalizationManager.GetText("Map.Episode.Title.Complete." + mapMissionGroupModel.MissionSpawnPointGroup.DisplayName);
	}

	public static string GetEpisodeUnlocks(MapMissionGroupModel mapMissionGroupModel)
	{
		return LocalizationManager.GetText("Map.Episode.Unlocks." + mapMissionGroupModel.MissionSpawnPointGroup.DisplayName);
	}

	public static string GetEpisodeVideoDescription(string episodeId)
	{
		return LocalizationManager.GetText("Map.Episode.VideoDescription." + episodeId);
	}

	public static string GetSpriteAndText(string localKey, out string localizedString)
	{
		string text = "";
		if (localKey != "")
		{
			text = LocalizationManager.GetText(localKey);
			Match match = new Regex("<<(.*)>>").Match(text);
			if (match.Success)
			{
				localizedString = text.Replace(match.Groups[0].ToString(), "");
				return "Portrait_" + match.Groups[1];
			}
		}
		localizedString = text;
		return "Portrait_Daryl";
	}

	public static string GetEpisodeTitle(int spawnGroupId)
	{
		if (GameManager.Instance.gameEconomyData.MissionSpawnPointData != null)
		{
			MissionSpawnPointGroup spawnPointGroup = GameManager.Instance.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(spawnGroupId);
			if (spawnPointGroup != null)
			{
				return LocalizationManager.GetText("Map.Episode.Title." + spawnPointGroup.DisplayName);
			}
		}
		return "";
	}

	public static string GetWeeklyChallengeTitle()
	{
		return LocalizationManager.GetText("Map.WeeklyChallenge.Title");
	}

	public static string GetActorClassName(string factionName, string className)
	{
		return LocalizationManager.GetText(factionName + ".Class." + className);
	}

	public static string GetWalkerCageDescription(string className)
	{
		return LocalizationManager.GetText("Walker.Class." + className + ".CageDescription");
	}

	public static string GetActorClassDescrption(string factionName, string className)
	{
		return LocalizationManager.GetText(factionName + ".Class." + className + ".Description");
	}

	public static string GetTimedBonusTitle(TimedBonusType timedBonusType, FixedPoint timedRewardDuration)
	{
		string text = "";
		float num = 0f;
		if (timedRewardDuration >= UtilsDateTime.DayInMilliseconds)
		{
			num = (float)timedRewardDuration / (float)UtilsDateTime.DayInMilliseconds;
			text = "{Days}";
		}
		else if (timedRewardDuration >= UtilsDateTime.HourInMilliseconds)
		{
			num = (float)timedRewardDuration / (float)UtilsDateTime.HourInMilliseconds;
			text = "{Hours}";
		}
		else if (timedRewardDuration >= UtilsDateTime.MinuteInMilliseconds)
		{
			num = (float)timedRewardDuration / (float)UtilsDateTime.MinuteInMilliseconds;
			text = "{Minutes}";
		}
		else
		{
			num = (int)timedRewardDuration / 1000;
			text = "{Seconds}";
		}
		if (num == 1f)
		{
			return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.Reward" + timedBonusType.ToString() + "TitleSingular" + text);
		}
		return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.Reward" + timedBonusType.ToString() + "TitlePlural" + text, num);
	}

	public static string GetTimedBonusConfirmation(FixedPoint days)
	{
		if (days == 1.0)
		{
			return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardTimedBonusConfirmationSingular{Days}", days);
		}
		return SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("Bundle.RewardTimedBonusConfirmationPlural{Days}", days);
	}

	public static string GetRewardsCurrencyDescription(Rewards rewards)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < rewards.Count; i++)
		{
			if (rewards.RewardsList[i] is RewardCurrency rewardCurrency)
			{
				stringBuilder.Append(rewardCurrency.Amount + " " + GetCurrencyName(rewardCurrency.CurrencyType));
				if (i < rewards.Count - 1)
				{
					stringBuilder.Append(", ");
				}
			}
		}
		return stringBuilder.ToString();
	}

	public static string GetHeroQuote(ActorDefinition actorDefinition)
	{
		if (actorDefinition != null)
		{
			return LocalizationManager.GetText("SurvivorInfoPopup.HeroUnlockQuote." + actorDefinition.ID);
		}
		return "";
	}

	public static string GetHeroDescription(ActorDefinition actorDefinition)
	{
		if (actorDefinition != null)
		{
			return LocalizationManager.GetText("SurvivorInfoPopup.HeroDescription." + actorDefinition.ID);
		}
		return "";
	}

	public static string GetOverrideActor(string missionId)
	{
		for (int i = 0; i < OverrideActorIds.Length; i += 2)
		{
			if (OverrideActorIds[i] == missionId && i + 1 < OverrideActorIds.Length)
			{
				return OverrideActorIds[i + 1];
			}
		}
		return null;
	}

	public static string GetBundleTitleForIReward(IReward reward)
	{
		if (reward != null)
		{
			if (reward.Type == RewardType.Currency)
			{
				if (reward is RewardCurrency rewardCurrency)
				{
					if (rewardCurrency.CurrencyType == CurrencyType.CampaignToken)
					{
						CampaignDefinition currentCampaignDefinition = GameManager.Instance.playerModel.CampaignModel.GetCurrentCampaignDefinition();
						if (currentCampaignDefinition != null)
						{
							return LocalizationManager.GetText(currentCampaignDefinition.CampaignTokenBundleLocKey, rewardCurrency.Amount);
						}
					}
					if (rewardCurrency.Amount == -1)
					{
						return LocalizationManager.GetText("Bundle.RewardCurrencyTitle." + rewardCurrency.CurrencyType.ToString() + ".Full");
					}
					return LocalizationManager.GetText("Bundle.RewardCurrencyTitle." + rewardCurrency.CurrencyType.ToString() + "{Parameter}", rewardCurrency.Amount);
				}
			}
			else if (reward.Type == RewardType.Equipment)
			{
				if (reward is RewardEquipment rewardEquipment)
				{
					string text = ((rewardEquipment.Amount <= 1) ? GetEquipmentName(rewardEquipment.EquipmentId) : GetEquipmentNameMultiple(rewardEquipment.EquipmentId, rewardEquipment.Amount));
					return LocalizationManager.GetText("Bundle.RewardEquipmentTitle{Parameter}", text);
				}
			}
			else if (reward.Type == RewardType.EquipToken)
			{
				if (reward is RewardEquipToken rewardEquipToken)
				{
					string equipmentTokenNameMultiple = GetEquipmentTokenNameMultiple(rewardEquipToken.EquipTokenId, rewardEquipToken.RewardAmount);
					return LocalizationManager.GetText("Bundle.RewardEquipmentTitle{Parameter}", equipmentTokenNameMultiple);
				}
			}
			else if (reward.Type == RewardType.RandomEquipment)
			{
				if (reward is RewardRandomEquipment rewardRandomEquipment)
				{
					if (rewardRandomEquipment.SurvivorClass != SurvivorClass.None)
					{
						return LocalizationManager.GetText("Bundle.RewardRandomEquipmentTitle." + HelpersUI.GetRarityName(rewardRandomEquipment.RarityLevel) + "{SurvivorClass}", GetSurvivorClassName(rewardRandomEquipment.SurvivorClass));
					}
					return LocalizationManager.GetText("Bundle.RewardRandomEquipmentTitle." + HelpersUI.GetRarityName(rewardRandomEquipment.RarityLevel));
				}
			}
			else if (reward.Type == RewardType.Outfit)
			{
				if (reward is RewardOutfit rewardOutfit)
				{
					string text2 = LocalizationManager.GetText(GameManager.Instance.gameEconomyData.GetOutfitDefinition(rewardOutfit.PreferredOrder[0]).TitleLocalizationKey);
					return LocalizationManager.GetText("Bundle.RewardOutfitTitle{Parameter}", text2);
				}
			}
			else if (reward.Type == RewardType.HeroSkin)
			{
				if (reward is RewardHeroSkin rewardHeroSkin)
				{
					string text3 = LocalizationManager.GetText(GameManager.Instance.gameEconomyData.GetSkinDefinition(rewardHeroSkin.PreferredOrder[0]).LocalizationKey);
					return LocalizationManager.GetText("Bundle.RewardHeroSkinTitle{Parameter}", text3);
				}
			}
			else if (reward.Type == RewardType.SurvivorSlot)
			{
				if (reward is RewardSurvivorSlot rewardSurvivorSlot)
				{
					return LocalizationManager.GetText("Bundle.RewardSurvivorSlotTitle{Parameter}", rewardSurvivorSlot.Amount.ToString());
				}
			}
			else if (reward.Type == RewardType.Loot)
			{
				if (reward is RewardLootEntry rewardLootEntry)
				{
					return LocalizationManager.GetText("Bundle.RewardLootTitle." + rewardLootEntry.DropType);
				}
			}
			else if (reward.Type == RewardType.SurvivorClass)
			{
				if (reward is RewardSurvivorClass rewardSurvivorClass)
				{
					string survivorClassName = GetSurvivorClassName(rewardSurvivorClass.SurvivorClass);
					return LocalizationManager.GetText("Bundle.RewardSurvivorClassTitle{Parameter}", survivorClassName);
				}
			}
			else if (reward.Type == RewardType.UnlockBuilding)
			{
				if (reward is RewardUnlockBuilding rewardUnlockBuilding)
				{
					string text4 = LocalizationManager.GetText("Building.Name." + rewardUnlockBuilding.BuildingTypeName);
					return LocalizationManager.GetText("Bundle.RewardUnlockBuildingTitle{Parameter}", text4);
				}
			}
			else if (reward.Type == RewardType.TimedBonus)
			{
				if (reward is RewardTimedBonus rewardTimedBonus)
				{
					return GetTimedBonusTitle(rewardTimedBonus.TimedBonusType, rewardTimedBonus.Duration);
				}
			}
			else if (reward.Type == RewardType.Avatars)
			{
				if (reward is RewardAvatars rewardAvatars)
				{
					if (rewardAvatars.Avatar >= 0)
					{
						return LocalizationManager.GetText("Bundle.RewardTitle.RewardAvatars.Avatar");
					}
					if (rewardAvatars.Border >= 0)
					{
						return LocalizationManager.GetText("Bundle.RewardTitle.RewardAvatars.Border");
					}
				}
			}
			else if (reward.Type == RewardType.RemoldSkill)
			{
				RewardRemoldSkill rewardRemoldSkill = reward as RewardRemoldSkill;
				return LocalizationManager.GetText("Bundle.RewardCurrency." + rewardRemoldSkill.SpRemoldSkillType);
			}
		}
		return LocalizationManager.GetText("Bundle.RewardTitle." + reward.Type);
	}

	public static string GetBadgeEffectTitle(BadgeModel data)
	{
		return LocalizationManager.GetText("BadgeEffects." + data.EffectId + ".Title");
	}

	public static string GetBadgeEffectDescription(BadgeModel data, FixedPoint increment)
	{
		return LocalizationManager.GetText("BadgeEffects." + data.EffectId + ".Description{Parameter}", (int)increment);
	}

	public static string GetBadgeBonusDescription(BadgeModel data)
	{
		string textId = "BadgeBonus." + data.BonusId + ".Description";
		BadgeEffectDefinition badgeEffectDefinition = GameManager.Instance.gameEconomyData.GetBadgeEffectDefinition(data.EffectId, data.Level);
		if (badgeEffectDefinition == null)
		{
			return "";
		}
		float num = float.Parse(data.BonusParameters[0]);
		string text = ((float)Math.Max(Math.Round((float)data.Increment * num / 100f), 1.0)).ToString("0");
		//Debug.Log($"GetBadgeBonusDescription text: ((float)Math.Max(Math.Round((float){data.Increment} * {num} / 100f), 1.0)).ToString(\"0\")");

		if (badgeEffectDefinition.IsRelative)
		{
			text += " %";
		}
		if (data.BonusCondition is TeamedUpWithHeroBonusCondition && data.BonusParameters.Count > 1)
		{
			return LocalizationManager.GetText(textId, text, GetHeroName(data.BonusParameters[1]));
		}
		if (data.BonusCondition is TeamedUpWithClassBonusCondition && data.BonusParameters.Count > 1)
		{
			SurvivorClass survivorClass = (SurvivorClass)Enum.Parse(typeof(SurvivorClass), data.BonusParameters[1]);
			return LocalizationManager.GetText(textId, text, GetSurvivorClassName(survivorClass));
		}
		if (data.BonusCondition is CharacterTraitBonusCondition && data.BonusParameters.Count > 1)
		{
			return LocalizationManager.GetText(textId, text, LocalizationManager.GetText("Traits." + data.BonusParameters[1]));
		}
		return LocalizationManager.GetText(textId, text);
	}

	public static string GetShopTooltipForIReward(IReward iReward)
	{
		if (iReward == null)
		{
			return "";
		}
		if (!(iReward is RewardCurrency rewardCurrency))
		{
			if (!(iReward is RewardTradeCrate rewardTradeCrate))
			{
				if (!(iReward is RewardSurvivorSlot))
				{
					if (!(iReward is RewardOutfit))
					{
						if (!(iReward is RewardTimedBonus rewardTimedBonus))
						{
							if (!(iReward is RewardEquipment rewardEquipment))
							{
								if (iReward is RewardMissingTokens rewardMissingTokens)
								{
									string currencyContext = GetCurrencyContext(rewardMissingTokens.RewardCurrencyType);
									int value = GameManager.Instance.playerModel.GetCurrency(rewardMissingTokens.RewardCurrencyType).Value;
									int upgradeCost = GetUpgradeCost(rewardMissingTokens.RewardCurrencyType);
									return LocalizationManager.GetText("Popup.Shop.Currency.HeroToken.Tooltip{CurrencyName}{OwnedAmount}{NextUpgradeCost}", currencyContext, value, upgradeCost);
								}
								return "";
							}
							return LocalizationManager.GetText("Popup.Shop.Consumable." + rewardEquipment.EquipmentId + ".Tooltip");
						}
						return LocalizationManager.GetText("Popup.Shop.TimedBonus." + rewardTimedBonus.TimedBonusType.ToString() + ".Tooltip");
					}
					return LocalizationManager.GetText("Popup.Shop.Outfit.Tooltip");
				}
				return LocalizationManager.GetText("Popup.Shop.SurvivorSlot.Tooltip");
			}
			return LocalizationManager.GetText("Popup.Shop.TradeCrate." + rewardTradeCrate.TradeCrateId + ".Tooltip");
		}
		string currencyName = GetCurrencyName(rewardCurrency.CurrencyType);
		int value2 = GameManager.Instance.playerModel.GetCurrency(rewardCurrency.CurrencyType).Value;
		if (rewardCurrency.CurrencyType == CurrencyType.CBPWarrior)
		{
			return LocalizationManager.GetText("Bundle.RewardCurrencyDescription.CBPWarrior{Parameter}", value2);
		}
		if (rewardCurrency.CurrencyType == CurrencyType.CBPScout)
		{
			return LocalizationManager.GetText("Bundle.RewardCurrencyDescription.CBPScout{Parameter}", value2);
		}
		if (rewardCurrency.CurrencyType == CurrencyType.CBPBruiser)
		{
			return LocalizationManager.GetText("Bundle.RewardCurrencyDescription.CBPBruiser{Parameter}", value2);
		}
		if (rewardCurrency.CurrencyType == CurrencyType.CBPShooter)
		{
			return LocalizationManager.GetText("Bundle.RewardCurrencyDescription.CBPShooter{Parameter}", value2);
		}
		if (rewardCurrency.CurrencyType == CurrencyType.CBPHunter)
		{
			return LocalizationManager.GetText("Bundle.RewardCurrencyDescription.CBPHunter{Parameter}", value2);
		}
		if (rewardCurrency.CurrencyType == CurrencyType.CBPAssault)
		{
			return LocalizationManager.GetText("Bundle.RewardCurrencyDescription.CBPAssault{Parameter}", value2);
		}
		if (GameManager.Instance.gameEconomyData.IsClassToken(rewardCurrency.CurrencyType))
		{
			currencyName = GetCurrencyContext(rewardCurrency.CurrencyType);
			return LocalizationManager.GetText("Popup.Shop.Currency.ClassToken.Tooltip{CurrencyName}{OwnedAmount}", currencyName, value2);
		}
		if (GameManager.Instance.gameEconomyData.IsHeroToken(rewardCurrency.CurrencyType))
		{
			int upgradeCost2 = GetUpgradeCost(rewardCurrency.CurrencyType);
			currencyName = GetCurrencyContext(rewardCurrency.CurrencyType);
			return LocalizationManager.GetText("Popup.Shop.Currency.HeroToken.Tooltip{CurrencyName}{OwnedAmount}{NextUpgradeCost}", currencyName, value2, upgradeCost2);
		}
		string textId = "Popup.Shop.Currency." + rewardCurrency.CurrencyType.ToString() + ".Tooltip{CurrencyName}{OwnedAmount}";
		if (rewardCurrency.CurrencyType == CurrencyType.BattlePassPoints)
		{
			return LocalizationManager.GetText(textId, currencyName, value2, GameManager.Instance.playerModel.BattlePass.NextTierBCPrice);
		}
		return LocalizationManager.GetText(textId, currencyName, value2);
	}

	private static int GetUpgradeCost(CurrencyType currencyType)
	{
		Cashier cashier = null;
		string heroId = SurvivorToken.GetHeroId(currencyType);
		if (GameManager.Instance.playerModel.SurvivorContainer.HasHero(heroId))
		{
			SurvivorModel heroById = GameManager.Instance.playerModel.SurvivorContainer.GetHeroById(heroId);
			if (heroById != null)
			{
				cashier = heroById.GetUpgradeTraitCashier();
			}
		}
		else
		{
			cashier = GameManager.Instance.playerModel.SurvivorContainer.GetHeroUnlockCashier(currencyType);
		}
		return cashier?.GetTotalCost(currencyType) ?? 0;
	}

	public static string GetShopTooltipForComponentCrateItem(ComponentCrateItem componentCrateItem)
	{
		if (componentCrateItem == null)
		{
			return "";
		}
		bool num = componentCrateItem.IsFixedRarity();
		bool flag = componentCrateItem.IsFixedType();
		string text = LocalizationManager.GetText("Component.Name" + ((componentCrateItem.Count == 1) ? "" : ".Plural"));
		string text2 = "";
		string text3 = "";
		if (num)
		{
			text2 = GetRarityLevel(componentCrateItem.Rarity);
		}
		text3 = ((!flag) ? text : LocalizationManager.GetText("Component." + componentCrateItem.Type + ".Name"));
		if (!num && !flag)
		{
			return LocalizationManager.GetText("ComponentShop.RandomItem.Tooltip{Amount}{ComponentName}", componentCrateItem.Count, text);
		}
		return LocalizationManager.GetText("ComponentShop.Item.Tooltip{Amount}{Rarity}{Type}", componentCrateItem.Count, text2, text3);
	}

	public static string GetGuildBattleSectorName(GuildBattleMapSectorModel sectorModel)
	{
		return LocalizationManager.GetText("GuildBattle.Sector." + sectorModel.SectorId + ".Name");
	}

	public static string GetGuildBonusTooltip(TraitDefinition traitDefinition)
	{
		string[] array = traitDefinition.ConstructionParameters.ToArray();
		object[] arguments;
		if (array.Length == 2)
		{
			string textId = traitDefinition.DisplayName + "{Parameter}{SurvivorClass}";
			arguments = array;
			return LocalizationManager.GetText(textId, arguments);
		}
		string textId2 = traitDefinition.DisplayName + "{Parameter}";
		arguments = array;
		return LocalizationManager.GetText(textId2, arguments);
	}

	public static string GetLeaderboardName(string leaderboardName)
	{
		return LocalizationManager.GetText("Leaderboard_" + leaderboardName);
	}

	public static string GetParticipantsTooltipText(GuildBattleMapMissionModel missionModel)
	{
		string empty = string.Empty;
		if (missionModel == null)
		{
			return empty;
		}
		List<string> pvpParticipants = missionModel.PvpParticipants;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(SingularityMonoBehaviour<LocalizationManager>.Instance.GetLocalizedText("GvG.PvpMissionParticipants"));
		GuildMemberInfo memberInfo = GameManager.Instance.guildModel.GetMemberInfo(missionModel.PvpPlayerHashedId);
		if (memberInfo != null)
		{
			stringBuilder.Append("\n");
			stringBuilder.Append(GameManager.Instance.GetFilteredText(memberInfo.Name));
		}
		foreach (string item in pvpParticipants)
		{
			memberInfo = GameManager.Instance.guildModel.GetMemberInfo(item);
			if (memberInfo != null)
			{
				stringBuilder.Append("\n");
				stringBuilder.Append(GameManager.Instance.GetFilteredText(GameManager.Instance.guildModel.GetMemberInfo(item)?.Name));
			}
		}
		return stringBuilder.ToString();
	}

	public static string GetRewardLocalizedName(IReward reward, int seed)
	{
		if (!(reward is RewardEquipment rewardEquipment))
		{
			if (!(reward is RewardCurrency rewardCurrency))
			{
				if (!(reward is RewardMissingTokens rewardMissingTokens))
				{
					if (!(reward is RewardRandomEquipment rewardRandomEquipment))
					{
						if (!(reward is RewardTradeCrate rewardTradeCrate))
						{
							if (!(reward is RewardOutfit rewardOutfit))
							{
								if (!(reward is RewardTimedBonus reward2))
								{
									if (reward is RewardEquipToken rewardEquipToken)
									{
										return GetEquipmentTokenName(rewardEquipToken.FakeRewardEquipTokenItemModel(GameManager.Instance.modelManager));
									}
									return "Reward Type: " + reward.Type;
								}
								return GetBundleTitleForIReward(reward2);
							}
							OutfitDefinition outfitDefinition = GameManager.Instance.gameEconomyData.GetOutfitDefinition(rewardOutfit.PreferredOrder[0]);
							return LocalizationManager.GetText("Bundle.Outfit.Description{Parameter}", LocalizationManager.GetText(outfitDefinition.TitleLocalizationKey));
						}
						return GetTradeCrateName(rewardTradeCrate.TradeCrateId);
					}
					int levelOut;
					return GetEquipmentName(rewardRandomEquipment.GetRandomEquipmentDefinition(GameManager.Instance.modelManager, new ModelRandom(seed), out levelOut).ID);
				}
				return GetCurrencyName(rewardMissingTokens.RewardCurrencyType);
			}
			return GetCurrencyName(rewardCurrency.CurrencyType);
		}
		return GetEquipmentName(rewardEquipment.EquipmentId);
	}

	public static string GetSupportName(string supportId)
	{
		return LocalizationManager.GetText("Support.Entry." + supportId + ".Name");
	}

	public static string GetSupportSkillDescription(SupportModel supportModel)
	{
		object[] array = new object[supportModel.ParameterCount];
		for (int i = 0; i < supportModel.ParameterCount; i++)
		{
			array[i] = supportModel.GetParameter(i);
		}
		return LocalizationManager.GetText("Support.Entry." + supportModel.SupportId + ".Skill.Description{Parameters}", array);
	}

	public static string GetSupportCooldownText(int cooldown)
	{
		return LocalizationManager.GetText("Support.Cooldown{Parameter}", cooldown);
	}

	public static string GetSupportLevelRankLabel(int level)
	{
		string textId = level switch
		{
			1 => "Support.Rank.Common",
			2 => "Support.Rank.Uncommon",
			3 => "Support.Rank.Rare",
			4 => "Support.Rank.Epic",
			5 => "Support.Rank.Legendary",
			_ => "Support.Rank.Default",
		};
		if (level > 5)
		{
			textId = "Support.Rank.Apocalyptic";
		}
		return LocalizationManager.GetText(textId);
	}

	public static string GetSupportStatName(string supportId, int index)
	{
		string textId = $"Support.Entry.{supportId}.Skill.Stat{index + 1}.Name";
		if (SingularityMonoBehaviour<LocalizationManager>.Instance.HasLocalizedText(textId))
		{
			return LocalizationManager.GetText(textId);
		}
		return string.Empty;
	}

	public static string GetWeaponAreaDesc(AbilityDefinition definition)
	{
		if (definition.AbilityTargetArea == AbilityTargetAreaType.Circle)
		{
			if (definition.AbilityTargetAreaRadius > 0L)
			{
				return LocalizationManager.GetText("AbilityRange.Circle.Desc", definition.AbilityTargetAreaRadius);
			}
			if (definition.AbilityTargetAreaRadius == 0L)
			{
				return LocalizationManager.GetText("AbilityRange.Circle.Single.Desc");
			}
		}
		if (definition.AbilityTargetArea == AbilityTargetAreaType.LineMax || definition.AbilityTargetArea == AbilityTargetAreaType.Line || definition.AbilityTargetArea == AbilityTargetAreaType.LineSeparated)
		{
			return LocalizationManager.GetText("AbilityRange.Line.Desc");
		}
		if (definition.AbilityTargetArea == AbilityTargetAreaType.Cone || definition.AbilityTargetArea == AbilityTargetAreaType.ConeLeft || definition.AbilityTargetArea == AbilityTargetAreaType.ConeRight)
		{
			return LocalizationManager.GetText("AbilityRange.Cone.Desc", definition.AbilityTargetAreaAngle);
		}
		return null;
	}

	public static string GetWeaponAreaDescNoArea(AbilityDefinition definition)
	{
		if (definition.AbilityTargetArea == AbilityTargetAreaType.Circle)
		{
			if (definition.AbilityTargetAreaRadius > 0L)
			{
				return LocalizationManager.GetText("BasicInfo.Ability.Circle.Desc", definition.AbilityTargetAreaRadius);
			}
			if (definition.AbilityTargetAreaRadius == 0L)
			{
				return LocalizationManager.GetText("BasicInfo.Ability.Circle.Single.Desc");
			}
		}
		if (definition.AbilityTargetArea == AbilityTargetAreaType.LineMax || definition.AbilityTargetArea == AbilityTargetAreaType.Line || definition.AbilityTargetArea == AbilityTargetAreaType.LineSeparated)
		{
			return LocalizationManager.GetText("BasicInfo.Ability.Line.Desc");
		}
		if (definition.AbilityTargetArea == AbilityTargetAreaType.Cone || definition.AbilityTargetArea == AbilityTargetAreaType.ConeLeft || definition.AbilityTargetArea == AbilityTargetAreaType.ConeRight)
		{
			return LocalizationManager.GetText("BasicInfo.Ability.Cone.Desc", definition.AbilityTargetAreaAngle);
		}
		return null;
	}



	#region mycode
	public static string GetInstantiatedTraitDescription(UpgradeTraitsData traitsData, out string constructParams)
	{
		constructParams = null;
		TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(traitsData.Identifier);
		if (traitDefinition == null)
		{
			DebugTWD.LogError("Trait definition not found for " + traitsData.Identifier);
			return "";
		}
		int num = ((traitDefinition.ConstructionParameters != null) ? traitDefinition.ConstructionParameters.Count : 0);
		object[] array = new object[num];
		for (int i = 0; i < num; i++)
		{
			string s = traitDefinition.ConstructionParameters[i];
			if ((traitsData.RemodelIng || traitsData.RemodelEd) && traitsData.ThisRemodeValues.TryGetValue(traitsData.Identifier, out var value) && traitsData.ThisRemodeParamIndex.TryGetValue(traitsData.Identifier, out var value2))
			{
				for (int j = 0; j < value.Count && j < value2.Count; j++)
				{
					if (value2[j] == i)
					{
						s = value[j].ToString();
					}
				}
			}
			float result2;
			if (int.TryParse(s, out var result))
			{
				float num2 = 1f + (float)traitsData.ConstructionMultiplier / 100f;
				array[i] = (int)Math.Round((float)result * num2);
			}
			else if (float.TryParse(s, out result2))
			{
				float num3 = 1f + (float)traitsData.ConstructionMultiplier / 100f;
				array[i] = (float)Math.Round(result2 * num3, 1);
			}
		}
		if (num > 0)
		{
			constructParams = num == 1 ? array[0].ToString() : string.Join("/", array);
			return LocalizationManager.GetText(traitDefinition.DisplayName + ".Description{Parameter}", array);
		}
		constructParams = "0";
		return LocalizationManager.GetText(traitDefinition.DisplayName + ".Description{Parameter}", 0);
	}

	public static string GetLastInstantiatedTraitDescription(UpgradeTraitsData traitsData, out string constructParams)
	{
		constructParams = null;
		TraitDefinition traitDefinition = GameManager.Instance.gameEconomyData.GetTraitDefinition(traitsData.Identifier);
		if (traitDefinition == null)
		{
			DebugTWD.LogError("Trait definition not found for " + traitsData.Identifier);
			return "";
		}
		int num = ((traitDefinition.ConstructionParameters != null) ? traitDefinition.ConstructionParameters.Count : 0);
		object[] array = new object[num];
		for (int i = 0; i < num; i++)
		{
			string s = traitDefinition.ConstructionParameters[i];
			if (traitsData.RemodeValues != null && traitsData.ThisRemodeParamIndex.TryGetValue(traitsData.Identifier, out var value))
			{
				for (int j = 0; j < traitsData.RemodeValues.Count && j < value.Count; j++)
				{
					if (value[j] == i)
					{
						s = traitsData.RemodeValues[j].ToString();
					}
				}
			}
			float result2;
			if (int.TryParse(s, out var result))
			{
				float num2 = 1f + (float)traitsData.ConstructionMultiplier / 100f;
				array[i] = (int)Math.Round((float)result * num2);
			}
			else if (float.TryParse(s, out result2))
			{
				float num3 = 1f + (float)traitsData.ConstructionMultiplier / 100f;
				array[i] = (float)Math.Round(result2 * num3, 1);
			}
		}
		if (num > 0)
		{
			constructParams = num == 1 ? array[0].ToString() : string.Join("/", array);
			return LocalizationManager.GetText(traitDefinition.DisplayName + ".Description{Parameter}", array);
		}
		constructParams = "0";
		return LocalizationManager.GetText(traitDefinition.DisplayName + ".Description{Parameter}", 0);
	}
	#endregion
}
