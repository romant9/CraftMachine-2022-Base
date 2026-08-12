using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class ApocalypseWeeklyChallengeModel : TWDModelObject
	{
		public bool ApocalypticMode90RoundRewardsReissuance;

		public int NumberStars { get; private set; }

		public int LastSeenNumberStars { get; set; }

		public int LastSeenChallengeDifficulty { get; set; }

		public FixedPoint LastSeenChallengeDifficultyProgression { get; set; }

		public int LastSeenCycleCount { get; set; }

		[JsonIgnore]
		public bool HasShownCycleEndedOnClient { get; set; }

		public int PreviousChallengeSkipTokens { get; set; }

		public int DifficultyBeforeSkips
		{
			get
			{
				int result = 0;
				GameEconomyData obj = base.gameEconomyData;
				if (obj != null && obj.WeeklyChallengeApocalypseConfigs?.Count > 0)
				{
					result = base.gameEconomyData.WeeklyChallengeApocalypseConfigs[0].Difficulty;
				}
				return result;
			}
		}

		public bool SkipTokensAvailableSeen { get; set; }

		public int CurrentCycle { get; set; }

		public int ActiveSkipTokens { get; set; }

		public int PendingSkipTokens => base.manager.Player.GetCurrency(CurrencyType.ApocalypticSkipToken).Value;

		public List<IncrementalDifficultyEffectDefinition> AppendDifficultyEffect { get; set; }

		public int RerollApocalypseBuffCount { get; set; }

		public List<WeeklyChallengeApocalypseBuff> weeklyChallengeApocalypseBuffs { get; set; }

		public List<WeeklyChallengeApocalypseBuff> PendingSelectApocalypseBuffs { get; set; }

		public List<WeeklyChallengeApocalypseBuff> SkipPendingSelectApocalypseBuffs { get; set; }

		[JsonIgnore]
		public bool IsHaveApocalypseBuffs
		{
			get
			{
				if (PendingSelectApocalypseBuffs != null)
				{
					return PendingSelectApocalypseBuffs.Count >= 3;
				}
				return false;
			}
		}

		[JsonIgnore]
		[IgnoreModelProperty]
		public WeeklyChallengeModel WeeklyChallengeModel => base.manager.Player.WeeklyChallenge;

		[JsonIgnore]
		public int RerollRemainingCount => (int)GetApocalypseBuffTotalFirstParam(ChallengeApocalypseBuffType.Reroll) - RerollApocalypseBuffCount;

		[JsonIgnore]
		public WeeklyChallenge CurrentDefinition => WeeklyChallengeModel.CurrentDefinition;

		[JsonIgnore]
		public bool IsChallengeApocalypticMode90RoundRewards => CurrentCycle >= base.manager.GameEconomyData.ConfigData.ChallengeApocalypticMode90RoundRewards;

		[JsonIgnore]
		public bool IsShowApocalypticMode90RoundRewards => CurrentCycle == base.manager.GameEconomyData.ConfigData.ChallengeApocalypticMode90RoundRewards;

		[JsonIgnore]
		public int GetApocalypticRoundStars
		{
			get
			{
				FixedPoint fixedPoint = base.manager.GameEconomyData.GetWeeklyChallengeReward(WeeklyChallengeReward.ChallengeRewardType.ApocalypticRoundStars, CurrentCycle, controlExactMatch: false).BonusStarsMultiplier;
				if (fixedPoint == 0.0)
				{
					fixedPoint = 1.0;
				}
				return (int)(CurrentCircleDefinition.Difficulty * fixedPoint);
			}
		}

		[JsonIgnore]
		public WeeklyChallengeApocalypseConfig CurrentCircleDefinition => base.gameEconomyData.GetWeeklyChallengeCircle(CurrentCycle);

		public override void Initialize()
		{
			base.Initialize();
			CurrentCycle = 0;
			AppendDifficultyEffect = new List<IncrementalDifficultyEffectDefinition>();
			AppendDifficultyEffect.AddRange(base.manager.GameEconomyData.GetDifficultyEffects(IncrementalDifficultyMissionType.ThreatMission, 20));
			RerollApocalypseBuffCount = 0;
			PendingSelectApocalypseBuffs = new List<WeeklyChallengeApocalypseBuff>();
			SkipPendingSelectApocalypseBuffs = new List<WeeklyChallengeApocalypseBuff>();
			weeklyChallengeApocalypseBuffs = new List<WeeklyChallengeApocalypseBuff>();
		}

		public override bool IsValid()
		{
			return true;
		}

		public MapMissionGroupModel GetMapMissionGroupModel()
		{
			if (CurrentDefinition != null)
			{
				int weeklyConfigApocalypticMapId = GetWeeklyConfigApocalypticMapId();
				MapMissionGroupModel missionGroupModelForSpawnPointGroup = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(weeklyConfigApocalypticMapId);
				if (missionGroupModelForSpawnPointGroup == null)
				{
					base.manager.Debug.LogError("Could not find group model for detailmap id = " + CurrentDefinition.ApocalypticMapId);
				}
				return missionGroupModelForSpawnPointGroup;
			}
			return null;
		}

		public MapMissionGroupModel GetApocalypticMapId()
		{
			int weeklyConfigApocalypticMapId = GetWeeklyConfigApocalypticMapId();
			return base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(weeklyConfigApocalypticMapId);
		}

		public MissionSpawnPointGroup GetMissionSpawnPointGroup()
		{
			if (CurrentDefinition != null)
			{
				int weeklyConfigApocalypticMapId = GetWeeklyConfigApocalypticMapId();
				MissionSpawnPointGroup spawnPointGroup = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(weeklyConfigApocalypticMapId);
				if (spawnPointGroup == null)
				{
					base.manager.Debug.LogError("Could not find spawn point group for '" + weeklyConfigApocalypticMapId + "' cannot start challenge!");
				}
				return spawnPointGroup;
			}
			return null;
		}

		public MapMissionGroupModel GetCurrentOrNextMapMissionGroupModel()
		{
			MapMissionGroupModel mapMissionGroupModel = GetMapMissionGroupModel();
			if (mapMissionGroupModel == null)
			{
				WeeklyChallenge nextWeeklyChallenge = WeeklyChallengeModel.NextWeeklyChallenge;
				if (nextWeeklyChallenge != null)
				{
					MissionSpawnPointGroup spawnPointGroup = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(nextWeeklyChallenge.ApocalypticMapId);
					if (spawnPointGroup != null)
					{
						mapMissionGroupModel = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(spawnPointGroup);
					}
				}
			}
			return mapMissionGroupModel;
		}

		public bool HasSeenLatestPersonalStars()
		{
			return NumberStars == LastSeenNumberStars;
		}

		public void ReturnRewardsInBatches(int rewardTypeBitmask, int minStarCount, int maxStarCount, int rewardsPerBatch, out List<List<WeeklyChallengeReward>> returnList, out int batchCount, int firsBatchCountOffset = 0)
		{
			returnList = new List<List<WeeklyChallengeReward>>();
			List<WeeklyChallengeReward> list = new List<WeeklyChallengeReward>();
			returnList.Add(list);
			batchCount = 0;
			if (rewardsPerBatch <= 0)
			{
				return;
			}
			for (int i = 0; i < base.gameEconomyData.WeeklyChallengeRewards.Length; i++)
			{
				WeeklyChallengeReward weeklyChallengeReward = base.gameEconomyData.WeeklyChallengeRewards[i];
				if (weeklyChallengeReward == null || !UtilsMath.BitmaskContains(1 << (int)weeklyChallengeReward.RewardType, rewardTypeBitmask))
				{
					continue;
				}
				WeeklyChallengeReward weeklyChallengeReward2 = ((list.Count > 0) ? list[list.Count - 1] : null);
				int num = ((batchCount <= 0) ? (rewardsPerBatch + firsBatchCountOffset) : rewardsPerBatch);
				if (list.Count < num)
				{
					list.Add(weeklyChallengeReward);
					continue;
				}
				if (weeklyChallengeReward2 != null && weeklyChallengeReward2.Control < maxStarCount)
				{
					if (weeklyChallengeReward2.Control < minStarCount)
					{
						returnList.Remove(list);
					}
					list = new List<WeeklyChallengeReward>();
					returnList.Add(list);
					list.Add(weeklyChallengeReward2);
					list.Add(weeklyChallengeReward);
					batchCount++;
					continue;
				}
				break;
			}
		}

		public bool CanStartNextCycle()
		{
			if (WeeklyChallengeModel.IsNewCycleLockedByTimer())
			{
				return false;
			}
			if (HasCompleteMaxRound())
			{
				return false;
			}
			MapMissionGroupModel mapMissionGroupModel = GetMapMissionGroupModel();
			if (mapMissionGroupModel != null)
			{
				int count = mapMissionGroupModel.Missions.Count;
				for (int i = 0; i < count; i++)
				{
					MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[i];
					if (!mapMissionModel.IsMasterMission && (mapMissionModel.State != MapMissionState.Unlocked || mapMissionModel.Stars.NumberStars < 1))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		public void MarkSkipTokensAvailableSeen()
		{
			SkipTokensAvailableSeen = true;
		}

		public void Reset(int identifier)
		{
			NumberStars = 0;
			LastSeenNumberStars = 0;
			LastSeenChallengeDifficulty = 0;
			LastSeenChallengeDifficultyProgression = 0L;
			LastSeenCycleCount = 0;
			SkipTokensAvailableSeen = false;
			HasShownCycleEndedOnClient = false;
			ApocalypticMode90RoundRewardsReissuance = false;
			WeeklyChallengesMapConfig[] weeklyChallengesMapConfigs = base.manager.Player.gameEconomyData.WeeklyChallengesMapConfigs;
			if (weeklyChallengesMapConfigs != null && CurrentDefinition.Identifier >= base.manager.Player.gameEconomyData.ConfigData.WeeklyChallengesApocalypticMapIdSwitchToPackage)
			{
				MissionSpawnPointGroup spawnPointGroup = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(CurrentDefinition.ApocalypticMapId);
				if (spawnPointGroup != null)
				{
					WeeklyChallengesMapConfig[] array = weeklyChallengesMapConfigs;
					foreach (WeeklyChallengesMapConfig weeklyChallengesMapConfig in array)
					{
						MissionSpawnPointGroup spawnPointGroup2 = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(weeklyChallengesMapConfig.MapID);
						if (spawnPointGroup2 == null)
						{
							continue;
						}
						MapMissionGroupModel missionGroupModelForSpawnPointGroup = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(spawnPointGroup2);
						missionGroupModelForSpawnPointGroup.RemoveMissions();
						base.manager.Player.MapContainerModel.SpawnMissionsForGroup(spawnPointGroup2);
						foreach (MapMissionModel mission in missionGroupModelForSpawnPointGroup.Missions)
						{
							mission.ChallengeId = identifier;
						}
					}
					MapMissionGroupModel missionGroupModelForSpawnPointGroup2 = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(spawnPointGroup);
					missionGroupModelForSpawnPointGroup2.RemoveMissions();
					base.manager.Player.MapContainerModel.SpawnMissionsForGroup(spawnPointGroup);
					foreach (MapMissionModel mission2 in missionGroupModelForSpawnPointGroup2.Missions)
					{
						mission2.ChallengeId = identifier;
					}
				}
			}
			else
			{
				MissionSpawnPointGroup spawnPointGroup3 = base.gameEconomyData.MissionSpawnPointData.GetSpawnPointGroup(CurrentDefinition.ApocalypticMapId);
				if (spawnPointGroup3 != null)
				{
					MapMissionGroupModel missionGroupModelForSpawnPointGroup3 = base.manager.Player.MapContainerModel.GetMissionGroupModelForSpawnPointGroup(spawnPointGroup3);
					missionGroupModelForSpawnPointGroup3.RemoveMissions();
					base.manager.Player.MapContainerModel.SpawnMissionsForGroup(spawnPointGroup3);
					foreach (MapMissionModel mission3 in missionGroupModelForSpawnPointGroup3.Missions)
					{
						mission3.ChallengeId = identifier;
					}
				}
			}
			CurrentCycle = 0;
			ActiveSkipTokens = PendingSkipTokens;
			base.manager.Player.GetCurrency(CurrencyType.ApocalypticSkipToken).SetValue(0);
			AppendDifficultyEffect = new List<IncrementalDifficultyEffectDefinition>();
			AppendDifficultyEffect.AddRange(base.manager.GameEconomyData.GetDifficultyEffects(IncrementalDifficultyMissionType.ThreatMission, 20));
			RerollApocalypseBuffCount = 0;
			PendingSelectApocalypseBuffs = new List<WeeklyChallengeApocalypseBuff>();
			SkipPendingSelectApocalypseBuffs = new List<WeeklyChallengeApocalypseBuff>();
			weeklyChallengeApocalypseBuffs = new List<WeeklyChallengeApocalypseBuff>();
		}

		public bool HasCompleteMaxRound()
		{
			return CurrentCycle > base.manager.GameEconomyData.ConfigData.ChallengeApocalypticModeMaxRound;
		}

		public void StartNewCycle()
		{
			MapMissionGroupModel mapMissionGroupModel = null;
			for (int i = 0; i <= ActiveSkipTokens; i++)
			{
				CurrentCycle++;
				mapMissionGroupModel = GetMapMissionGroupModel();
				if (HasCompleteMaxRound())
				{
					break;
				}
				if (CurrentCircleDefinition.IncrementalDifficulty != IncrementalDifficultyEffect.None)
				{
					AppendDifficultyEffect.Add(new IncrementalDifficultyEffectDefinition
					{
						Effect = CurrentCircleDefinition.IncrementalDifficulty,
						Parameter = CurrentCircleDefinition.ConstructionParameters
					});
				}
				if (CurrentCircleDefinition.Buff != null)
				{
					List<WeeklyChallengeApocalypseBuff> canRandomChallengeApocalypseBuff = base.manager.GameEconomyData.GetCanRandomChallengeApocalypseBuff(CurrentCircleDefinition.Buff, weeklyChallengeApocalypseBuffs.Select((WeeklyChallengeApocalypseBuff x) => x.Identifier).ToList());
					PendingSelectApocalypseBuffs = base.manager.Player.PlayerRandom.WeightedRandomList(canRandomChallengeApocalypseBuff, 3, (WeeklyChallengeApocalypseBuff x) => x.Weight, isRepeat: false);
				}
				if (mapMissionGroupModel != null)
				{
					mapMissionGroupModel.RemoveMission(mapMissionGroupModel.Missions.Models.Find((MapMissionModel t) => t.IsMasterMission));
					MapMissionModel mapMissionModel = base.manager.Player.MapContainerModel.CreateMissionModel(mapMissionGroupModel.Missions[(CurrentCycle - 1) % mapMissionGroupModel.Missions.Count].MissionSpawnPoint);
					mapMissionModel.ChallengeId = WeeklyChallengeModel.Id;
					mapMissionModel.IsMasterMission = true;
					mapMissionGroupModel.AddMission(mapMissionModel);
				}
				if (i < ActiveSkipTokens)
				{
					SkipPendingSelectApocalypseBuffs.InsertRange(0, PendingSelectApocalypseBuffs);
					PendingSelectApocalypseBuffs.Clear();
					foreach (MapMissionModel mission in mapMissionGroupModel.Missions)
					{
						mission.Stars.Stars = new bool[3] { true, true, true };
						mission.Stars.TotalStars = 4;
						int amount = 4;
						if (!mission.IsMasterMission)
						{
							AddPersonalStars(amount);
						}
						mission.NotifyChange("StateChanged");
					}
					AddCycleCompleteRewards();
					WeeklyChallengeClassTeamActivityModel weeklyChallengeClassTeamActivity = base.manager.Player.WeeklyChallengeClassTeamActivity;
					if (weeklyChallengeClassTeamActivity != null && weeklyChallengeClassTeamActivity.IsActive)
					{
						ClassTeamDefinition currentDefinition = weeklyChallengeClassTeamActivity.CurrentDefinition;
						if (currentDefinition != null && currentDefinition.RewardsObj != null)
						{
							int num = 0;
							foreach (MapMissionModel mission2 in mapMissionGroupModel.Missions)
							{
								if (!mission2.IsMasterMission)
								{
									num++;
								}
							}
							weeklyChallengeClassTeamActivity.QueueSkipClaimRewards(currentDefinition.RewardsObj, num);
							for (int num2 = 0; num2 < num; num2++)
							{
								base.manager.TdMetrics.SetEventType("Class_Team_Challenge_Reward").AddProperty("reward_info", currentDefinition.Reward).Send();
								base.manager.Metrics.AddClassTeamReward(currentDefinition.Reward).Send();
							}
						}
					}
					base.manager.Debug.Log("SkipApocalypseNewCycle (" + CurrentCycle + ") ");
				}
				if (IsShowApocalypticMode90RoundRewards && !ApocalypticMode90RoundRewardsReissuance)
				{
					WeeklyChallengeModel.SkipToCircle((NumberStars >= base.manager.GameEconomyData.ConfigData.ChallengeApocalypticMode90RoundRewardsStar) ? 91 : 81);
					ApocalypticMode90RoundRewardsReissuance = true;
				}
				base.manager.Metrics.AddSpend().AddApocalypseSkipTokens(-ActiveSkipTokens, PendingSkipTokens).AddApocalyChallenge()
					.AddApocalypeSkipRounds(ActiveSkipTokens, DifficultyBeforeSkips)
					.Send();
			}
			PreviousChallengeSkipTokens = ActiveSkipTokens;
			SkipTokensAvailableSeen = ActiveSkipTokens == 0;
			ActiveSkipTokens = 0;
			HasShownCycleEndedOnClient = false;
			if (mapMissionGroupModel == null)
			{
				mapMissionGroupModel = GetMapMissionGroupModel();
			}
			if (mapMissionGroupModel != null && !HasCompleteMaxRound())
			{
				int count = mapMissionGroupModel.Missions.Count;
				for (int num3 = 0; num3 < count; num3++)
				{
					MapMissionModel mapMissionModel3 = mapMissionGroupModel.Missions[num3];
					if (mapMissionModel3 != null && mapMissionModel3.Stars != null)
					{
						mapMissionModel3.State = MapMissionState.Unlocked;
						mapMissionModel3.Stars.ResetStarsForNewChallengeCycle();
						mapMissionModel3.MissionLevel = CurrentCircleDefinition.MissionLevel;
						mapMissionModel3.CompletedFromMasterMission = false;
						mapMissionModel3.StarsFromMasterMission = 0;
						mapMissionModel3.ClassTeamRewardGiven = false;
					}
				}
			}
			base.manager.Debug.Log("StartApocalypseNewCycle (" + CurrentCycle + ") ");
		}

		public void CompleteMissionsInCycle()
		{
			MapMissionGroupModel mapMissionGroupModel = GetMapMissionGroupModel();
			if (mapMissionGroupModel == null)
			{
				return;
			}
			int count = mapMissionGroupModel.Missions.Count;
			for (int i = 0; i < count; i++)
			{
				MapMissionModel mapMissionModel = mapMissionGroupModel.Missions[i];
				if (mapMissionModel != null && !mapMissionModel.IsMasterMission)
				{
					mapMissionModel.GiveStars(giveStarsFromMasterMissionCompletion: true);
				}
			}
			UpdateChallengePlayerLeaderboards();
		}

		public TWDModelResult SelectApocalypse(int index)
		{
			if (SkipPendingSelectApocalypseBuffs.Count > 0)
			{
				int count = SkipPendingSelectApocalypseBuffs.Count;
				weeklyChallengeApocalypseBuffs.Add(SkipPendingSelectApocalypseBuffs[count - 3 + index]);
				SkipPendingSelectApocalypseBuffs = SkipPendingSelectApocalypseBuffs.Take(count - 3).ToList();
				return TWDModelResult.OK;
			}
			if (PendingSelectApocalypseBuffs.Count <= index)
			{
				return TWDModelResult.Error;
			}
			WeeklyChallengeApocalypseBuff weeklyChallengeApocalypseBuff = PendingSelectApocalypseBuffs[index];
			weeklyChallengeApocalypseBuffs.Add(weeklyChallengeApocalypseBuff);
			base.manager.TdMetrics.SetEventType("ApocalypticChallenge_BuffPick").AddProperty("BuffPick_Id", weeklyChallengeApocalypseBuff.Identifier).Send();
			PendingSelectApocalypseBuffs.Clear();
			return TWDModelResult.OK;
		}

		public TWDModelResult RerollApocalypse()
		{
			if (PendingSelectApocalypseBuffs.Count <= 0)
			{
				return TWDModelResult.Error;
			}
			if (RerollApocalypseBuffCount >= GetApocalypseBuffTotalFirstParam(ChallengeApocalypseBuffType.Reroll))
			{
				return TWDModelResult.Error;
			}
			RerollApocalypseBuffCount++;
			List<WeeklyChallengeApocalypseBuff> canRandomChallengeApocalypseBuff = base.manager.GameEconomyData.GetCanRandomChallengeApocalypseBuff(CurrentCircleDefinition.Buff, weeklyChallengeApocalypseBuffs.Select((WeeklyChallengeApocalypseBuff x) => x.Identifier).ToList());
			PendingSelectApocalypseBuffs = base.manager.Player.PlayerRandom.WeightedRandomList(canRandomChallengeApocalypseBuff, 3, (WeeklyChallengeApocalypseBuff x) => x.Weight, isRepeat: false);
			return TWDModelResult.OK;
		}

		public List<IncrementalDifficultyEffectDefinition> GetDifficultyEffects()
		{
			return AppendDifficultyEffect;
		}

		public FixedPoint GetApocalypseBuffClassDmgUp(SurvivorClass survivorClass)
		{
			FixedPoint result = 0.0;
			foreach (WeeklyChallengeApocalypseBuff weeklyChallengeApocalypseBuff in weeklyChallengeApocalypseBuffs)
			{
				if (weeklyChallengeApocalypseBuff.BuffType == ChallengeApocalypseBuffType.ClassDmgUp && (weeklyChallengeApocalypseBuff.ConstructionParameters[0] == (long)survivorClass || weeklyChallengeApocalypseBuff.ConstructionParameters[0] == 6L))
				{
					result += weeklyChallengeApocalypseBuff.ConstructionParameters[1];
				}
			}
			return result;
		}

		public FixedPoint GetApocalypseBuffTotalFirstParam(ChallengeApocalypseBuffType ApocalypseBuffType)
		{
			FixedPoint result = 0.0;
			foreach (WeeklyChallengeApocalypseBuff weeklyChallengeApocalypseBuff in weeklyChallengeApocalypseBuffs)
			{
				if (weeklyChallengeApocalypseBuff.BuffType == ApocalypseBuffType)
				{
					result += weeklyChallengeApocalypseBuff.ConstructionParameters[0];
				}
			}
			return result;
		}

		public List<DifficultyIncrementalDebuff> GetChallengeDebuffs()
		{
			List<DifficultyIncrementalDebuff> list = new List<DifficultyIncrementalDebuff>();
			list.AddRange(CurrentCircleDefinition.DebuffConfigs);
			list.AddRange(CurrentCircleDefinition.BaseDebuffConfigs);
			return list;
		}

		public bool IsApocalypseBuffEffect(ChallengeApocalypseBuffType ApocalypseBuffType, RollDiceType rollDiceType)
		{
			int chance = (int)GetApocalypseBuffTotalFirstParam(ApocalypseBuffType);
			return base.manager.Player.RollDice(rollDiceType, chance) != PlayerRandomChanceResult.Failed;
		}

		public void ApplyBuffAtCombatStart(CombatModel combat)
		{
			for (int i = 0; i < combat.AllActors.Count; i++)
			{
				ActorModel actorModel = combat.AllActors[i];
				if (actorModel.Faction == Faction.Survivor)
				{
					if (IsApocalypseBuffEffect(ChallengeApocalypseBuffType.StartCharge, RollDiceType.GainChargePointAtStart))
					{
						actorModel.AddChargePoints(1000);
					}
				}
				else if (IsApocalypseBuffEffect(ChallengeApocalypseBuffType.StartStun, RollDiceType.Stun))
				{
					actorModel.Stun(1, actorModel);
				}
				else if (IsApocalypseBuffEffect(ChallengeApocalypseBuffType.StartCrippling, RollDiceType.Cripple))
				{
					actorModel.Cripple(1, actorModel);
				}
			}
		}

		public void AddPersonalStars(int amount)
		{
			List<WeeklyChallengeReward> personalRewardsBetween = GetPersonalRewardsBetween(NumberStars, NumberStars + amount);
			NumberStars += amount;
			if (personalRewardsBetween != null)
			{
				for (int i = 0; i < personalRewardsBetween.Count; i++)
				{
					WeeklyChallengeReward challengeReward = personalRewardsBetween[i];
					WeeklyChallengeModel.AddReward(challengeReward);
				}
			}
		}

		public void AddCycleCompleteRewards()
		{
			if (CurrentCycle < 1 || base.manager == null || base.manager.GameEconomyData == null || base.manager.Player == null || !base.manager.GameEconomyData.GetFeature("ChallengeCycleEnabled").Enabled)
			{
				return;
			}
			WeeklyChallengeReward weeklyChallengeReward = base.manager.GameEconomyData.GetWeeklyChallengeReward(WeeklyChallengeReward.ChallengeRewardType.ApocalypticRoundStars, CurrentCycle, controlExactMatch: false);
			if (weeklyChallengeReward != null)
			{
				WeeklyChallengeModel.AddReward(weeklyChallengeReward);
			}
			int getApocalypticRoundStars = GetApocalypticRoundStars;
			if (getApocalypticRoundStars <= 0)
			{
				return;
			}
			base.manager.Metrics.AddFind().AddApocalypseBonusStars(getApocalypticRoundStars).AddApocalyChallenge()
				.AddApocalypseChallengeRoundReward()
				.Send();
			AddPersonalStars(getApocalypticRoundStars);
			if (base.manager.ServerService != null)
			{
				LeaderboardEntry entry = Leaderboards.CreateCurrentApocalypseChallengeLeaderboardEntry(base.manager.Player);
				string challengeId = WeeklyChallengeModel.Id.ToString();
				base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerApocalypseChallengeWeeklyLeaderboardName(challengeId), entry);
				if (base.manager.Player.Country != null)
				{
					base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerApocalypseChallengeWeeklyCountryLeaderboardName(base.manager.Player.Country, challengeId), entry);
				}
			}
		}

		public List<WeeklyChallengeReward> GetPersonalRewardsBetween(int fromStars, int toStars)
		{
			List<WeeklyChallengeReward> list = new List<WeeklyChallengeReward>();
			for (int i = 0; i < ((base.gameEconomyData.WeeklyChallengeRewards != null) ? base.gameEconomyData.WeeklyChallengeRewards.Length : 0); i++)
			{
				WeeklyChallengeReward weeklyChallengeReward = base.gameEconomyData.WeeklyChallengeRewards[i];
				if (weeklyChallengeReward.RewardType == WeeklyChallengeReward.ChallengeRewardType.ApocalypticStars && weeklyChallengeReward.Control > fromStars && weeklyChallengeReward.Control <= toStars)
				{
					list.Add(weeklyChallengeReward);
				}
			}
			return list;
		}

		public void UpdateChallengePlayerLeaderboards()
		{
			if (base.manager.ServerService != null)
			{
				LeaderboardEntry entry = Leaderboards.CreateCurrentApocalypseChallengeLeaderboardEntry(base.manager.Player);
				string challengeId = base.manager.Player.WeeklyChallenge.Id.ToString();
				base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerApocalypseChallengeWeeklyLeaderboardName(challengeId), entry);
				if (base.manager.Player.Country != null)
				{
					base.manager.ServerService.SaveLeaderboardEntry(Leaderboards.GetPlayerApocalypseChallengeWeeklyCountryLeaderboardName(base.manager.Player.Country, challengeId), entry);
				}
			}
		}

		public void SkipToCircle(int circle)
		{
			int num = circle - CurrentCycle - ActiveSkipTokens;
			for (int i = 0; i < num; i++)
			{
				MapMissionGroupModel currentOrNextMapMissionGroupModel = GetCurrentOrNextMapMissionGroupModel();
				if (currentOrNextMapMissionGroupModel == null)
				{
					break;
				}
				if (CurrentCycle == 0)
				{
					StartNewCycle();
					i++;
				}
				foreach (MapMissionModel mission in currentOrNextMapMissionGroupModel.Missions)
				{
					mission.Stars.Stars = new bool[3] { true, true, true };
					mission.Stars.TotalStars = 4;
					int amount = 4;
					if (!mission.IsMasterMission)
					{
						AddPersonalStars(amount);
					}
					mission.NotifyChange("StateChanged");
				}
				AddCycleCompleteRewards();
				if (HasCompleteMaxRound())
				{
					break;
				}
				StartNewCycle();
			}
		}

		public int GetWeeklyConfigApocalypticMapId()
		{
			if (CurrentDefinition.Identifier >= base.manager.Player.gameEconomyData.ConfigData.WeeklyChallengesApocalypticMapIdSwitchToPackage)
			{
				return base.manager.Player.gameEconomyData.GetMapIdByDifficulty(CurrentDefinition.ApocalypticMapId, CurrentCycle);
			}
			return CurrentDefinition.ApocalypticMapId;
		}
	}
}
