using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BaseModel;

namespace TWDModel
{
	public static class GvGModelHelper
	{
		public static GuildBattleParticipantInfo CreateEnemyPlayerData(PlayerModel playerModel, GameEconomyData gameEconomyData)
		{
			GuildBattleParticipantInfo guildBattleParticipantInfo = new GuildBattleParticipantInfo
			{
				HashedPlayerId = playerModel.HashedId,
				Name = playerModel.Name,
				PlayerEmblem = playerModel.PlayerEmblem,
				SelectedSurvivors = playerModel.GvGDefenders
			};
			List<Tuple<int, FixedPoint, SurvivorModel>> list = CalculateAndSortPlayerAdjustedLevelForSurvivors(playerModel, gameEconomyData);
			ModelList<SurvivorModel> survivors = playerModel.SurvivorContainer.Survivors;
			FixedPoint fixedPoint = new FixedPoint(0);
			FixedPoint fixedPoint2 = new FixedPoint(0);
			int num = gameEconomyData.ConfigData.ForceCouncilMaxLevel + 1;
			for (int i = 0; i < list.Count; i++)
			{
				Tuple<int, FixedPoint, SurvivorModel> tuple = list[i];
				SurvivorModel survivorModel = survivors[tuple.First];
				if (num == survivorModel.Level)
				{
					fixedPoint += tuple.Second * gameEconomyData.GuildWarConfig.MaxLevelWt;
					fixedPoint2 += (FixedPoint)gameEconomyData.GuildWarConfig.MaxLevelWt;
				}
				else if (num - 1 == survivorModel.Level)
				{
					fixedPoint += tuple.Second * gameEconomyData.GuildWarConfig.AlmostMaxLevelWt;
					fixedPoint2 += (FixedPoint)gameEconomyData.GuildWarConfig.AlmostMaxLevelWt;
				}
				else
				{
					fixedPoint += tuple.Second;
					fixedPoint2 += (FixedPoint)1L;
				}
			}
			guildBattleParticipantInfo.PlayerAdjustedLevel = (int)FixedPoint.Round(fixedPoint / fixedPoint2);
			fixedPoint = 0L;
			foreach (Tuple<int, FixedPoint, SurvivorModel> item in list)
			{
				fixedPoint += item.Second;
			}
			guildBattleParticipantInfo.PlayerActualLevel = (int)FixedPoint.Round(fixedPoint / list.Count);
			return guildBattleParticipantInfo;
		}

		public static List<Tuple<int, FixedPoint, SurvivorModel>> CalculateAndSortPlayerAdjustedLevelForSurvivors(PlayerModel playerModel, GameEconomyData gameEconomyData)
		{
			List<EquipmentItemModel> allEquipments = playerModel.Equipment.GetAllEquipments();
			List<EquipmentItemModel> resultList = new List<EquipmentItemModel>();
			ModelList<SurvivorModel> survivors = playerModel.SurvivorContainer.Survivors;
			List<Tuple<int, FixedPoint, SurvivorModel>> list = new List<Tuple<int, FixedPoint, SurvivorModel>>();
			for (int i = 0; i < survivors.Count; i++)
			{
				SurvivorModel survivorModel = survivors[i];
				if (survivorModel == null)
				{
					continue;
				}
				playerModel.Equipment.GetEquipmentsForActorNoAlloc(survivorModel, isWeapon: true, allEquipments, ref resultList);
				if (resultList.Count != 0)
				{
					playerModel.Equipment.GetEquipmentsForActorNoAlloc(survivorModel, isWeapon: false, allEquipments, ref resultList);
					if (resultList.Count != 0)
					{
						FixedPoint adjustedLevelForSurvivor = GetAdjustedLevelForSurvivor(survivorModel, gameEconomyData);
						list.Add(new Tuple<int, FixedPoint, SurvivorModel>(i, adjustedLevelForSurvivor, survivorModel));
					}
				}
			}
			list.StableSort(delegate(Tuple<int, FixedPoint, SurvivorModel> x, Tuple<int, FixedPoint, SurvivorModel> y)
			{
				int num = y.Second.Value.CompareTo(x.Second.Value);
				if (num == 0)
				{
					num = string.Compare(x.Third.ActorDefinitionID, y.Third.ActorDefinitionID, StringComparison.Ordinal);
				}
				if (num == 0)
				{
					num = x.Third.ModelId.CompareTo(y.Third.ModelId);
				}
				return num;
			});
			return list;
		}

		public static FixedPoint GetAdjustedLevelForSurvivor(SurvivorModel survivor, GameEconomyData gameEconomyData)
		{
			int num = (survivor.IsHero ? 1 : 0);
			return survivor.Level + num * gameEconomyData.GuildWarConfig.HeroLevelEq + UtilsMath.Max(0, survivor.SurvivorRarityLevel - 4) * gameEconomyData.GuildWarConfig.PinkLevelEq;
		}

		private static void SortEquipmentList(ref List<EquipmentItemModel> equipment)
		{
			equipment.StableSort(delegate(EquipmentItemModel x, EquipmentItemModel y)
			{
				int num = y.RarityLevel.CompareTo(x.RarityLevel);
				if (num == 0)
				{
					y.Level.CompareTo(x.Level);
				}
				if (num == 0)
				{
					num = y.GetTotalUpgrades.CompareTo(x.GetTotalUpgrades);
				}
				if (num == 0)
				{
					num = string.Compare(x.EquipmentDefinitionIdentifier, y.EquipmentDefinitionIdentifier, StringComparison.Ordinal);
				}
				if (num == 0)
				{
					num = x.ModelId.CompareTo(y.ModelId);
				}
				return num;
			});
		}

		public static Tuple<FixedPoint, FixedPoint> CalculateGuildLevel(Dictionary<string, GuildBattleParticipantInfo> players)
		{
			Tuple<FixedPoint, FixedPoint> tuple = new Tuple<FixedPoint, FixedPoint>();
			if (players == null || players.Count == 0)
			{
				return tuple;
			}
			List<KeyValuePair<string, GuildBattleParticipantInfo>> list = players.ToList();
			list.StableSort((KeyValuePair<string, GuildBattleParticipantInfo> x, KeyValuePair<string, GuildBattleParticipantInfo> y) => y.Value.PlayerAdjustedLevel.CompareTo(x.Value.PlayerAdjustedLevel));
			int num = 0;
			int num2 = 0;
			for (int num3 = 0; num3 < list.Count; num3++)
			{
				GuildBattleParticipantInfo value = list[num3].Value;
				num += value.PlayerAdjustedLevel;
				num2++;
			}
			tuple.First = num / num2;
			num = 0;
			foreach (KeyValuePair<string, GuildBattleParticipantInfo> player in players)
			{
				GuildBattleParticipantInfo value2 = player.Value;
				num += value2.PlayerAdjustedLevel;
			}
			tuple.Second = num / players.Count;
			return tuple;
		}

		public static void ObfuscateOpponentGuildData(GuildBattleMatchmakingInfo enemyMatchMakingInfo, FakeBattleDefinition fakeBattleDefinition, int targetTier, int RandomSeed)
		{
			enemyMatchMakingInfo.GroupId = "Fake_" + enemyMatchMakingInfo.GroupId;
			enemyMatchMakingInfo.GuildName = fakeBattleDefinition.OpponentName;
			enemyMatchMakingInfo.UpdateInfoOnEndBattle(targetTier, 0);
			ModelRandom modelRandom = new ModelRandom(RandomSeed);
			int num = 0;
			List<string> list = new List<string>(SurvivorNames.FemaleNames);
			List<string> list2 = new List<string>(SurvivorNames.MaleNames);
			foreach (GuildBattleParticipantInfo value2 in enemyMatchMakingInfo.PlayerInfoSnapshot.Values)
			{
				value2.HashedPlayerId = "Fake_Player_" + ++num;
				PlayerEmblem playerEmblem = new PlayerEmblem
				{
					BorderIndex = modelRandom.GetRandomInRange(0, 6),
					ColorIndex = modelRandom.GetRandomInRange(0, 10),
					IconIndex = modelRandom.GetRandomInRange(0, 12)
				};
				value2.PlayerEmblem = playerEmblem;
				foreach (SurvivorMockData selectedSurvivor in value2.SelectedSurvivors)
				{
					selectedSurvivor.OwnerHashedPlayerId = value2.HashedPlayerId;
					if (!selectedSurvivor.IsHero)
					{
						string name = ((selectedSurvivor.Gender == ActorGender.Female) ? modelRandom.GetRandomElement(list, remove: true) : modelRandom.GetRandomElement(list2, remove: true));
						selectedSurvivor.Name = name;
					}
				}
				value2.Name = value2.SelectedSurvivors[0].Name;
			}
			foreach (string item in new List<string>(enemyMatchMakingInfo.PlayerInfoSnapshot.Keys))
			{
				if (enemyMatchMakingInfo.PlayerInfoSnapshot.TryGetValue(item, out var value))
				{
					enemyMatchMakingInfo.PlayerInfoSnapshot.Remove(item);
					enemyMatchMakingInfo.PlayerInfoSnapshot.Add(value.HashedPlayerId, value);
				}
			}
		}

		public static int NotificationDelayInMilliseconds(string guildId, int delayInSeconds = 1)
		{
			try
			{
				return int.Parse(guildId?.Substring(0, 1), NumberStyles.HexNumber) * delayInSeconds * 1000;
			}
			catch
			{
				return 0;
			}
		}

		public static int GetPlayerSpecificDifficulty(PlayerModel player)
		{
			return Math.Max(player.SurvivorContainer.GetGvGBaseDifficultyFromSurvivors(), player.Equipment.GetHighestEquipableEquipmentLevel());
		}
	}
}
