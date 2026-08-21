using System.Collections.Generic;

namespace Client.Framework.Audio
{
	public static class AudioEvents
	{
		public static class Global
		{
			private const string global = "global/";

			public const string purchase = "global/purchase";

			public const string invalidAction = "global/invalid_action";

			public const string uiClick = "global/ui_click";

			public const string bundleIntro = "global/bundle_intro";

			public const string rewardClaim = "global/reward_claim";

			public const string collectToken = "global/collect_token";

			public const string viewChange = "global/view_change";

			public const string equipmentClick = "global/equipment_click";

			public const string matchFound = "global/match_found";

			public const string matchFail = "global/match_fail";

			public const string matchSearch = "global/match_search";

			public const string uiDrag = "global/ui_drag";

			public const string uiDrop = "global/ui_drop";

			public const string uiinvalidDrop = "global/ui_invalid_drop";

			public const string survivorCardClick = "global/survivor_card_click";

			public const string iapReward = "global/iap_reward";

			public const string rewardFlip = "global/reward_flip";

			public const string uiShimmer1 = "global/ui_shimmer_1";

			public const string survivorHeal = "global/survivor_heal";

			public const string uiSwoosh2 = "global/ui_swoosh_2";

			public const string found = "global/found_";

			public const string foundToken = "global/found_token";

			public const string uiDialogExit = "global/ui_dialog_exit";

			public const string memberAccept = "global/member_accept";

			public const string messageReceived = "global/message_received";

			public const string messageSend = "global/message_send";

			public const string memberPromote = "global/member_promote";

			public const string memberDemote = "global/member_demote";

			public const string memberKick = "global/member_kick";

			public const string memberRefuse = "global/member_refuse";

			public const string survivorEquip = "global/survivor_equip";

			public const string openShop = "global/open_shop";

			public const string survivorUpgradeStats = "global/survivor_upgrade_stats";

			public const string survivorAccept = "global/survivor_accept";

			public const string survivorUpgradeTrait = "global/survivor_upgrade_trait";

			public const string survivorUpgradeClose = "global/survivor_upgrade_close";

			public const string purchaseStart = "global/purchase_start";
		}

		public static class Camp
		{
			private const string camp = "camp/";

			public const string questAccept = "camp/quest_accept";

			public const string buildingBuildReady = "camp/building_build_ready";

			public const string buildingUpgradeReady = "camp/building_upgrade_ready";

			public const string buildingUpgradeStop = "camp/building_upgrade_stop";

			public const string forestCut = "camp/forest_cut";

			public const string buildingPick = "camp/building_pick";

			public const string buildingPlace = "camp/building_place";

			public const string buildingCanNotPlace = "camp/building_cannot_place";

			public const string phoneCall = "camp/phonecall";

			public const string uiTabChange = "camp/ui_tab_change";

			public const string achievementOpen = "camp/achievement_open";

			public const string achievementClose = "camp/achievement_close";

			public const string walkerKill = "camp/walker_kill";

			public const string storyTellerClick = "camp/storyteller_click";

			public const string uiOutpostCreate = "camp/ui_outpost_create";

			public const string uiChangeSlice = "camp/ui_change_slice";

			public const string uiSelectArea = "camp/ui_select_area";

			public const string uiAutofill = "camp/ui_autofill";

			public const string uiAddSurvivor = "camp/ui_add_survivor";

			public const string uiAddWalker = "camp/ui_add_walker";

			public const string uiAddItem = "camp/ui_add_item";

			public const string uiRemoveItem = "camp/ui_remove_item";

			public const string uiChangeMode = "camp/ui_change_mode";

			public const string survivorClick = "camp/survivor_click";

			public const string uiIncrement = "camp/ui_increment";

			public const string uiDecrement = "camp/ui_decrement";

			public const string uiOpenInfo = "camp/ui_open_info";

			public const string workshopEquipmentUpgrade = "camp/workshop_equipment_upgrade";

			public const string workshopEquipmentScrap = "camp/workshop_equipment_scrap";

			public const string openGraveyard = "camp/open_graveyard";

			public const string openMedicTent = "camp/open_medictent";

			public const string cardFlip = "camp/card_flip";

			public const string cardShake = "camp/card_shake";

			public const string cardSpin = "camp/card_spin";

			public const string cardClick = "camp/card_click";

			public const string cardMove = "camp/card_move";

			public const string cardZoom = "camp/card_zoom";

			public const string questComplete = "camp/quest_complete";

			public const string openOutpostManagement = "camp/open_outpostmanagement";

			public const string openTrainingGround = "camp/open_trainingground";

			public const string openResidence = "camp/open_residence";

			public const string getReward = "camp/get_reward";

			public const string openWorkshop = "camp/open_workshop";

			public const string openRadioTent = "camp/open_radiotent";

			public const string socialClose = "camp/social_close";

			public const string buildingUpgrade = "camp/building_upgrade";

			public const string moreWalkers = "camp/more_walkers";
		}

		public static class CombatUI
		{
			private const string combatUI = "combat_ui/";

			public const string saved = "combat_ui/saved";

			public const string combatStart = "combat_ui/combat_start";

			public const string turnSkip = "combat_ui/turn_skip";

			public const string exitReady = "combat_ui/exit_ready";

			public const string turnSurvivors = "combat_ui/turn_survivors";

			public const string turnWalkers = "combat_ui/turn_walkers";

			public const string selectSurvivor = "combat_ui/select_survivor";

			public const string timerWarning = "combat_ui/timer_warning";

			public const string timerTick = "combat_ui/timer_tick";

			public const string timerTickWarning = "combat_ui/timer_tick_warning";

			public const string starShow = "combat_ui/star_show";

			public const string combatVictory = "combat_ui/combat_victory";

			public const string chargeOn = "combat_ui/charge_on";

			public const string chargeOff = "combat_ui/charge_off";

			public const string waveIncoming = "combat_ui/wave_incoming";

			public const string turnWarning = "combat_ui/turn_warning";

			public const string coverSelect = "combat_ui/cover_select";

			public const string combatEndFadeDefeat = "combat_ui/combat_end_fade_defeat";

			public const string combatEndFadeVictory = "combat_ui/combat_end_fade_victory";

			public const string coverTarget = "combat_ui/cover_target";

			public const string permadeathStatus = "combat_ui/permadeath_status";
		}

		public static class CombatLevel
		{
			private const string combatLevel = "combat_level/";

			public const string barrelExplosion1 = "combat_level/barrel_explosion_1";

			public const string lootVehicleOpen = "combat_level/loot_vehicle_open";

			public const string lootPreciousOpen = "combat_level/loot_precious_open";

			public const string lootDumpsterOpen = "combat_level/loot_dumpster_open";

			public const string lootDumpsterLargeOpen = "combat_level/loot_dumpster_large_open";

			public const string lootSodamachineOpen = "combat_level/loot_sodamachine_open";

			public const string lootPrimaryOpen = "combat_level/loot_primary_open";

			public const string lootCoolerOpen = "combat_level/loot_cooler_open";

			public const string lootTentOpen = "combat_level/loot_tent_open";

			public const string doorMetalOpen = "combat_level/door_metal_open";

			public const string doorWoodOpen = "combat_level/door_wood_open";

			public const string bush = "combat_level/bush";

			public const string noiseAmpOff = "combat_level/noise_amp_off";

			public const string flagClaim = "combat_level/flag_claim";

			public const string doorMetalLargeOpen = "combat_level/door_metal_large_open";

			public const string doorWoodLargeOpen = "combat_level/door_wood_large_open";

			public const string tankShoot = "combat_level/tank_shoot";

			public const string tankEngine = "combat_level/tank_engine";

			public const string tankDamaged = "combat_level/tank_damaged";
		}

		public static class CombatWeapon
		{
			private const string combatWeapon = "combat_weapon/";

			public const string goreHit = "combat_weapon/gore_hit";
		}

		public static class RewardScreen
		{
			private const string rewardScreen = "reward_screen/";

			public const string buyMoreRewards = "reward_screen/buy_more_rewards";

			public const string returnToMap = "reward_screen/return_to_map";

			public const string foundOnMission = "reward_screen/found_on_mission";

			public const string containerOpenRarity1 = "reward_screen/container_open_rarity_1";

			public const string containerOpenRarity2 = "reward_screen/container_open_rarity_2";

			public const string containerOpenRarity3 = "reward_screen/container_open_rarity_3";
		}

		public static class Music
		{
			private const string music = "music/";

			public const string musicPhoneCall = "music/music_phonecall";

			public const string musicCamp = "music/music_camp";

			public const string musicCamp1 = "music/music_camp_1";

			public const string musicCamp2 = "music/music_camp_2";

			public const string musicCampDefense = "music/music_camp_defense";

			public const string musicCombat = "music/music_combat_";

			public const string musicMap = "music/music_map";

			public const string musicOutpostManagement = "music/music_outpost_management";

			public const string musicDefeat = "music/music_defeat";

			public const string musicVictory = "music/music_victory";

			public const string theme = "music/theme";

			public const string TWDGvGLobbyTheme = "music/TWD_GvG_Lobby_Theme";

			public const string musicEndlessMenus = "music/music_endless_menus";

			public const string musicCombatAll = "music/music_combat_all";
		}

		public static class Ambience
		{
			private const string ambience = "ambience/";

			public const string ambientCamp = "ambience/ambient_camp";

			public const string ambient = "ambience/ambient_";

			public const string ambientOutpostManagement = "ambience/ambient_outpost_management";

			public const string ambientRewardScreen = "ambience/ambient_reward_screen";

			public const string ambientMap = "ambience/ambient_map";
		}

		public static class Map
		{
			private const string map = "map/";

			public const string missionClick = "map/mission_click";

			public const string startmission = "map/start_mission";
		}

		public const string dialogPlayer = "dialog_player";

		private static readonly Dictionary<SoundType, string> sounds = new Dictionary<SoundType, string>
		{
			{
				SoundType.InteractiveObject_Loot_Vehicle,
				"combat_level/loot_vehicle_open"
			},
			{
				SoundType.InteractiveObject_Loot_Precious,
				"combat_level/loot_precious_open"
			},
			{
				SoundType.InteractiveObject_Loot_Dumpster,
				"combat_level/loot_dumpster_open"
			},
			{
				SoundType.InteractiveObject_Loot_Dumpster_Large,
				"combat_level/loot_dumpster_large_open"
			},
			{
				SoundType.InteractiveObject_Loot_Sodamachine,
				"combat_level/loot_sodamachine_open"
			},
			{
				SoundType.InteractiveObject_Loot_Primary,
				"combat_level/loot_primary_open"
			},
			{
				SoundType.InteractiveObject_Loot_Cooler,
				"combat_level/loot_cooler_open"
			},
			{
				SoundType.InteractiveObject_Loot_Tent,
				"combat_level/loot_tent_open"
			},
			{
				SoundType.InteractiveObject_Door_Metal,
				"combat_level/door_metal_open"
			},
			{
				SoundType.InteractiveObject_Door_Wood,
				"combat_level/door_wood_open"
			},
			{
				SoundType.InteractiveObject_Bush,
				"combat_level/bush"
			},
			{
				SoundType.InteractiveObject_Noise_Amp_Off,
				"combat_level/noise_amp_off"
			},
			{
				SoundType.InteractiveObject_Flag,
				"combat_level/flag_claim"
			},
			{
				SoundType.InteractiveObject_Door_Metal_Large,
				"combat_level/door_metal_large_open"
			},
			{
				SoundType.InteractiveObject_Door_Wood_Large,
				"combat_level/door_wood_large_open"
			}
		};

		private static readonly Dictionary<MusicState, List<string>> musicLoops = new Dictionary<MusicState, List<string>>
		{
			{
				MusicState.Camp,
				new List<string> { "ambience/ambient_camp", "music/music_camp" }
			},
			{
				MusicState.Map,
				new List<string> { "ambience/ambient_map", "music/music_map" }
			},
			{
				MusicState.Outpost,
				new List<string> { "ambience/ambient_outpost_management", "music/music_outpost_management" }
			},
			{
				MusicState.Defeat,
				new List<string> { "music/music_defeat" }
			},
			{
				MusicState.Victory,
				new List<string> { "music/music_victory" }
			},
			{
				MusicState.Rewards,
				new List<string> { "ambience/ambient_reward_screen" }
			},
			{
				MusicState.Theme,
				new List<string> { "music/theme" }
			},
			{
				MusicState.GvGTheme,
				new List<string> { "music/TWD_GvG_Lobby_Theme" }
			},
			{
				MusicState.EndlessMenus,
				new List<string> { "music/music_endless_menus" }
			}
		};

		public static string GetAudioEvent(SoundType soundType)
		{
			if (sounds.ContainsKey(soundType))
			{
				return sounds[soundType];
			}
			Debug.LogWarning(typeof(SoundType)?.ToString() + "not found. Returning default sound");
			return "";
		}

		public static List<string> GetMusicList(MusicState musicState)
		{
			if (musicLoops.ContainsKey(musicState))
			{
				return new List<string>(musicLoops[musicState]);
			}
			Debug.LogWarning(typeof(SoundType)?.ToString() + "not found. Returning default sound");
			return new List<string>();
		}
	}
}
