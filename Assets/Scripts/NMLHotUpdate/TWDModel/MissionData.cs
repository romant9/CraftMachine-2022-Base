using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TWDModel
{
	[Serializable]
	public class MissionData
	{
		public const int DefaultTeamSize = 3;

		public string Id;

		public MissionType MissionType;

		public string RunLocationName;

		public string MissionName;

		public List<int> MissionTags;

		public string DisplayTextID;

		public string Stars;

		public int RewardedSurvivorRarityLevel;

		public int WalkerTypes;

		public int RaiderTypes;

		public MissionExtraData ExtraData;

		public string[] FixedSupports;

		[JsonIgnore]
		public MissionStarConditions MissionStarConditions => MissionStarConditions.ConvertFromString(Stars);

		[JsonIgnore]
		public int MaxTeamSize
		{
			get
			{
				if (ExtraData == null || !ExtraData.InUse)
				{
					return 3;
				}
				return ExtraData.MaxTeamSize;
			}
		}

		public MissionData()
		{
		}

		public MissionData(MissionModel missionModel)
		{
			MissionName = missionModel.MissionName;
			MissionType = missionModel.TypeOfMission;
			Id = missionModel.Id;
			Stars = new MissionStarConditions(missionModel.MissionStarConditions).ConvertToString();
			if (missionModel.MaxTeamSize != 3)
			{
				EnsureExtraDataCreated();
				ExtraData.MaxTeamSize = missionModel.MaxTeamSize;
			}
			DisplayTextID = missionModel.DisplayTextID;
			if (missionModel.MissionTags != null && missionModel.MissionTags.Count > 0)
			{
				MissionTags = new List<int>();
				MissionTags.AddRange(missionModel.MissionTags);
			}
			AddActorInfo(missionModel);
			FixedSupports = new string[3];
			for (int i = 0; i < FixedSupports.Length; i++)
			{
				FixedSupports[i] = "";
			}
			foreach (TWDModelObject model in missionModel.Models)
			{
				if (model is ScenarioSupportModel scenarioSupportModel)
				{
					FixedSupports[scenarioSupportModel.EquippedIndex] = $"{scenarioSupportModel.SupportId}-{scenarioSupportModel.Level}";
				}
			}
		}

		public bool HasWalker(WalkerType type)
		{
			int num = 1 << (int)type;
			return (WalkerTypes & num) != 0;
		}

		public bool HasRaider(SurvivorClass cls)
		{
			int num = 1 << (int)cls;
			return (RaiderTypes & num) != 0;
		}

		public bool HasEnemyTrait(string trait)
		{
			if (ExtraData != null && ExtraData.InUse && ExtraData.EnemyAdditionalTraits != null)
			{
				for (int i = 0; i < ExtraData.EnemyAdditionalTraits.Count; i++)
				{
					if (ExtraData.EnemyAdditionalTraits[i].CompareTo(trait) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasCivilianActorId(string id)
		{
			if (ExtraData != null && ExtraData.InUse && ExtraData.CivilianActorIds != null)
			{
				for (int i = 0; i < ExtraData.CivilianActorIds.Count; i++)
				{
					if (ExtraData.CivilianActorIds[i].CompareTo(id) == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasCivilianActorIdContaining(string str)
		{
			if (ExtraData != null && ExtraData.InUse && ExtraData.CivilianActorIds != null)
			{
				for (int i = 0; i < ExtraData.CivilianActorIds.Count; i++)
				{
					if (ExtraData.CivilianActorIds[i].Contains(str))
					{
						return true;
					}
				}
			}
			return false;
		}

		public SurvivorClass GetRewardedSurvivorType()
		{
			if (ExtraData != null && ExtraData.InUse)
			{
				return ExtraData.RewardedSurvivorType;
			}
			return SurvivorClass.None;
		}

		public int GetRewardedSurvivorRarityLevel()
		{
			if (ExtraData != null && ExtraData.InUse)
			{
				return ExtraData.RewardedSurvivorRarityLevel;
			}
			return -1;
		}

		private void AddWalker(WalkerType type)
		{
			int num = 1 << (int)type;
			WalkerTypes |= num;
		}

		private void AddRaider(SurvivorClass cls)
		{
			int num = 1 << (int)cls;
			RaiderTypes |= num;
		}

		private void EnsureExtraDataCreated()
		{
			if (ExtraData == null || !ExtraData.InUse)
			{
				ExtraData = new MissionExtraData();
				ExtraData.MaxTeamSize = 3;
				ExtraData.InUse = true;
			}
		}

		private void AddActorInfo(MissionModel missionModel)
		{
			foreach (ActorSpawnPointModel actorSpawnPoint in missionModel.GetActorSpawnPoints())
			{
				if (actorSpawnPoint is SurvivorSpawnPointModel)
				{
					SurvivorSpawnPointModel survivorSpawnPointModel = actorSpawnPoint as SurvivorSpawnPointModel;
					if (!survivorSpawnPointModel.IsNotGivenToPlayer)
					{
						EnsureExtraDataCreated();
						ExtraData.RewardedSurvivorType = survivorSpawnPointModel.SurvivorClass;
						ExtraData.RewardedSurvivorRarityLevel = survivorSpawnPointModel.RarityLevel;
					}
					else if (!string.IsNullOrEmpty(survivorSpawnPointModel.ActorID))
					{
						EnsureExtraDataCreated();
						if (ExtraData.PlayableSurvivors == null || ExtraData.PlayableSurvivors.Count == 0)
						{
							ExtraData.PlayableSurvivors = new List<PlayableSurvivor>();
						}
						PlayableSurvivor playableSurvivor = new PlayableSurvivor();
						playableSurvivor.ActorID = survivorSpawnPointModel.ActorID;
						playableSurvivor.MinLevel = survivorSpawnPointModel.MinLevelOffset;
						playableSurvivor.MaxLevel = survivorSpawnPointModel.MaxLevelOffset;
						playableSurvivor.Rarity = survivorSpawnPointModel.RarityLevel;
						playableSurvivor.WeaponID = survivorSpawnPointModel.WeaponOverrideId;
						playableSurvivor.ArmorID = survivorSpawnPointModel.ArmorOverrideId;
						playableSurvivor.EqLevel = survivorSpawnPointModel.EquipmentLevel;
						playableSurvivor.EqRarity = survivorSpawnPointModel.EquipmentRarityLevel;
						playableSurvivor.RosterIndex = survivorSpawnPointModel.RosterIndex;
						ExtraData.PlayableSurvivors.Add(playableSurvivor);
					}
				}
				else if (actorSpawnPoint is WalkerSpawnPointModel)
				{
					WalkerSpawnPointModel walkerSpawnPointModel = actorSpawnPoint as WalkerSpawnPointModel;
					if (walkerSpawnPointModel.UseOverrideWalkerType)
					{
						AddWalker(walkerSpawnPointModel.OverrideWalkerType);
					}
					else
					{
						AddWalker(WalkerType.WalkerNormal);
					}
				}
				else if (actorSpawnPoint is RaiderSpawnPointModel)
				{
					RaiderSpawnPointModel raiderSpawnPointModel = actorSpawnPoint as RaiderSpawnPointModel;
					AddRaider(raiderSpawnPointModel.Class);
				}
				else if (actorSpawnPoint is CivilianSpawnPointModel)
				{
					EnsureExtraDataCreated();
					if (ExtraData.CivilianActorIds == null)
					{
						ExtraData.CivilianActorIds = new List<string>();
					}
					CivilianSpawnPointModel civilianSpawnPointModel = actorSpawnPoint as CivilianSpawnPointModel;
					ExtraData.CivilianActorIds.Add(civilianSpawnPointModel.ActorClassID.ToLowerInvariant());
				}
				if (actorSpawnPoint.AdditionalTraits == null || actorSpawnPoint.AdditionalTraits.Count <= 0)
				{
					continue;
				}
				EnsureExtraDataCreated();
				if (ExtraData.EnemyAdditionalTraits == null)
				{
					ExtraData.EnemyAdditionalTraits = new List<string>();
				}
				foreach (string additionalTrait in actorSpawnPoint.AdditionalTraits)
				{
					if (!ExtraData.EnemyAdditionalTraits.Contains(additionalTrait))
					{
						ExtraData.EnemyAdditionalTraits.Add(additionalTrait);
					}
				}
			}
		}

		public bool AcceptTags(List<int> includeTags, List<int> excludeTags)
		{
			bool flag = includeTags == null || includeTags.Count == 0;
			if (includeTags != null && MissionTags != null && MissionTags.Count > 0)
			{
				foreach (int includeTag in includeTags)
				{
					if (MissionTags.Contains(includeTag))
					{
						flag = true;
						break;
					}
				}
			}
			bool flag2 = true;
			if (excludeTags != null && excludeTags.Count > 0 && MissionTags != null && MissionTags.Count > 0)
			{
				foreach (int excludeTag in excludeTags)
				{
					if (MissionTags.Contains(excludeTag))
					{
						flag2 = false;
						break;
					}
				}
			}
			return flag && flag2;
		}
	}
}
