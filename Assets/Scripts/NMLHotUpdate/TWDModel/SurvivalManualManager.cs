using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SurvivalManualManager : TWDModelObject
	{
		private static long CheckForNewNewSurvivalManuaDefaultTime = 60000L;

		private static long CheckForLimitedSurvivalManuasCombatInterval = 5000L;

		private static long CheckForLimitedSurvivalManuasNonCombatInterval = 500L;

		private const long FiveMinutesInterval = 300000L;

		private long _fiveMinuteTickTimer;

		public const string LimitedEventAvailableEvent = "LimitedEventAvailableEvent";

		public long CheckForNewSurvivalManuaTimer { get; set; }

		public long CheckForInitSurvivalManualActorTraits { get; set; }

		public ModelList<SurvivalManualModel> SurvivalManualModels { get; set; }

		public Dictionary<string, List<SurvivalManualActorStoryLockTrait>> ActorSurvivalManualStorySkillList { get; set; }

		[JsonIgnore]
		public SurvivalManualSkill SkillDefinition => base.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel);

		public int SurvivalManualSkillLevel { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			CheckForNewSurvivalManuaTimer = 0L;
			CheckForInitSurvivalManualActorTraits = 0L;
			SurvivalManualSkillLevel = 1;
			_fiveMinuteTickTimer = 0L;
			SurvivalManualModels = new ModelList<SurvivalManualModel>();
			SurvivalManualModels.SetManager(base.manager);
			SurvivalManualModels.Initialize();
			ActorSurvivalManualStorySkillList = new Dictionary<string, List<SurvivalManualActorStoryLockTrait>>();
			ActivatedSurvivalManualTraits();
		}

		public override void Start()
		{
			base.Start();
			CheckForNewSurvivalManuaTimer = 0L;
			CheckForInitSurvivalManualActorTraits = 0L;
			_fiveMinuteTickTimer = 0L;
			if (SurvivalManualSkillLevel == 0)
			{
				SurvivalManualSkillLevel = 1;
			}
			if (ActorSurvivalManualStorySkillList == null)
			{
				ActorSurvivalManualStorySkillList = new Dictionary<string, List<SurvivalManualActorStoryLockTrait>>();
			}
			if (SurvivalManualModels == null)
			{
				SurvivalManualModels = new ModelList<SurvivalManualModel>();
				SurvivalManualModels.SetManager(base.manager);
				SurvivalManualModels.Initialize();
			}
			else
			{
				ActivatedSurvivalManualTraits();
			}
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			if (base.manager?.Player != null && base.manager.Player.gameEconomyData != null && IsSurvivalManualInit())
			{
				_fiveMinuteTickTimer += deltaTime;
				if (_fiveMinuteTickTimer >= 300000)
				{
					_fiveMinuteTickTimer = 0L;
					UpdateSurvivalManualLeaderboard();
				}
				CheckForNewSurvivalManuaTimer -= deltaTime;
				if (CheckForNewSurvivalManuaTimer <= 0)
				{
					CheckForNewLimitedSurvivalManuas();
					CheckForNewSurvivalManuaTimer = CheckForNewNewSurvivalManuaDefaultTime;
				}
				CheckForInitSurvivalManualActorTraits -= deltaTime;
				if (CheckForInitSurvivalManualActorTraits <= 0)
				{
					ActivatedSurvivalManualTraits();
					CheckForInitSurvivalManualActorTraits = CheckForNewNewSurvivalManuaDefaultTime;
				}
			}
		}

		public void UpdateSurvivalManualLeaderboard()
		{
			if (base.manager == null)
			{
				return;
			}
			PlayerModel player = base.manager.Player;
			if (player == null || player.SurvivalManualManager == null || player.gameEconomyData == null)
			{
				return;
			}
			IServerService serverService = base.manager.ServerService;
			if (serverService == null)
			{
				return;
			}
			LeaderboardEntry leaderboardEntry = Leaderboards.CreateSurvivalManualLeaderboardEntry(player);
			if (leaderboardEntry == null)
			{
				base.manager.Debug.LogWarning("[SurvivalManualLeaderboard] Entry 创建失败，跳过更新。");
				return;
			}
			try
			{
				string playerSurvivalManualLeaderboardName = Leaderboards.GetPlayerSurvivalManualLeaderboardName();
				serverService.SaveLeaderboardEntry(playerSurvivalManualLeaderboardName, leaderboardEntry);
			}
			catch (Exception ex)
			{
				base.manager.Debug.LogError("[SurvivalManualLeaderboard] 更新排行榜出错：" + ex);
			}
		}

		public void CheckForNewLimitedSurvivalManuas()
		{
			List<SurvivalManualDefinition> survivalManualOpenList = GetSurvivalManualOpenList();
			if (survivalManualOpenList == null)
			{
				return;
			}
			for (int i = 0; i < survivalManualOpenList.Count; i++)
			{
				SurvivalManualDefinition survivalManualDefinition = survivalManualOpenList[i];
				if (survivalManualDefinition != null)
				{
					if (survivalManualDefinition.HasDateLimit && GetInitiatedLimitedEventData(survivalManualDefinition.ID) == null)
					{
						SetupNewLimitedEventData(survivalManualDefinition);
					}
					else if (!survivalManualDefinition.HasDateLimit && base.manager.Player.UtcTimeStamp > survivalManualDefinition.StoryShowTimeMilliseconds && GetInitiatedLimitedEventData(survivalManualDefinition.ID) == null)
					{
						SetupNewLimitedEventData(survivalManualDefinition, skipValidation: true);
					}
				}
			}
		}

		public SurvivalManualModel GetInitiatedLimitedEventData(int id)
		{
			if (SurvivalManualModels == null)
			{
				return null;
			}
			for (int i = 0; i < SurvivalManualModels.Count; i++)
			{
				SurvivalManualModel survivalManualModel = SurvivalManualModels[i];
				if (survivalManualModel.ID == id)
				{
					return survivalManualModel;
				}
			}
			return null;
		}

		private void SetupNewLimitedEventData(SurvivalManualDefinition survivalManualDefinition, bool skipValidation = false)
		{
			if (skipValidation || (IsEventAvailable(survivalManualDefinition) && GetInitiatedLimitedEventData(survivalManualDefinition.ID) == null))
			{
				SurvivalManualModel survivalManualModel = new SurvivalManualModel();
				survivalManualModel.Initialize();
				survivalManualModel.SetManager(base.manager);
				survivalManualModel.Start();
				survivalManualModel.ID = survivalManualDefinition.ID;
				if (survivalManualDefinition.HasDateLimit)
				{
					long utcTimeStamp = base.manager.Player.UtcTimeStamp;
					long endTimeMilliseconds = survivalManualDefinition.EndTimeMilliseconds;
					survivalManualModel.Timer = Math.Max(0L, endTimeMilliseconds - utcTimeStamp);
				}
				else
				{
					survivalManualModel.Timer = 0L;
				}
				survivalManualModel.StartTimestamp = survivalManualDefinition.ActiveOpenTime;
				survivalManualModel.EndTimestamp = survivalManualDefinition.ActiveEndTime;
				if (survivalManualModel.ManualActorStoryLockTraits == null)
				{
					survivalManualModel.ManualActorStoryLockTraits = new ManualActorStoryLockTrait();
				}
				survivalManualModel.ManualActorStoryLockTraits.Type = survivalManualDefinition.StoryQueueSkill;
				survivalManualModel.ManualActorStoryLockTraits.Level = 0;
				survivalManualModel.ManualActorStoryLockTraits.State = ManualActorStoryTraitState.Activated;
				SurvivalManualModels.Add(survivalManualModel);
				NotifyChange("LimitedEventAvailableEvent");
			}
		}

		private bool IsEventAvailable(SurvivalManualDefinition survivalManualDefinition)
		{
			if (survivalManualDefinition == null)
			{
				return false;
			}
			PlayerModel player = base.manager.Player;
			GameEconomyData gameEconomyData = base.manager.Player.gameEconomyData;
			if (player != null && gameEconomyData != null)
			{
				if (survivalManualDefinition.HasDateLimit && (player.UtcTimeStamp > survivalManualDefinition.StartTimeMilliseconds || player.UtcTimeStamp > survivalManualDefinition.EndTimeMilliseconds))
				{
					return true;
				}
				if (!survivalManualDefinition.HasDateLimit && player.UtcTimeStamp > survivalManualDefinition.StoryShowTimeMilliseconds)
				{
					return true;
				}
			}
			return false;
		}

		public List<SurvivalManualDefinition> GetSurvivalManualOpenList()
		{
			if (base.manager != null && base.manager.Player != null)
			{
				PlayerModel player = base.manager.Player;
				return player.gameEconomyData.GetSurvivalManualOpenTimes(player.UtcTimeStamp);
			}
			return null;
		}

		public long GetSystemLV()
		{
			long num = 0L;
			if (SurvivalManualModels != null && SurvivalManualModels.Count > 0)
			{
				for (int i = 0; i < SurvivalManualModels.Count; i++)
				{
					SurvivalManualModel survivalManualModel = SurvivalManualModels[i];
					if (survivalManualModel != null)
					{
						num += survivalManualModel.GetTotalActorsAllLevel();
					}
				}
			}
			return num;
		}

		public SurvivalManualModel GetSurvivalManualModel(int survivalManualDefinitionId)
		{
			if (SurvivalManualModels == null)
			{
				return null;
			}
			for (int i = 0; i < SurvivalManualModels.Count; i++)
			{
				SurvivalManualModel survivalManualModel = SurvivalManualModels[i];
				if (survivalManualModel != null && survivalManualModel.ID == survivalManualDefinitionId)
				{
					return survivalManualModel;
				}
			}
			return null;
		}

		public virtual TWDModelResult UpgradeSurvivalManualAttributeLeve()
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return TWDModelResult.Error;
			}
			if (CanUpgradeSurvivalManualAttributeLeve() != SurvivalManualType.UpgradeCondition)
			{
				return TWDModelResult.Error;
			}
			SurvivalManualSkill survivalManualSkillByLevel = base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel);
			Cashier cashier = new Cashier(base.manager);
			foreach (KeyValuePair<CurrencyType, int> item in survivalManualSkillByLevel.GetUpgradCostInfo())
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeSurvivalManualSkill);
				CurrencyType key = item.Key;
				int value = item.Value;
				cashierItem.SetCost(key, value);
				cashier.AddItem(cashierItem);
			}
			if (cashier.Pay(survivalManualSkillByLevel) == TWDModelResult.OK)
			{
				SurvivalManualSkillLevel++;
			}
			return TWDModelResult.OK;
		}

		public SurvivalManualType CanUpgradeSurvivalManualAttributeLeve()
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return SurvivalManualType.UpgradeInvalid;
			}
			SurvivalManualSkill survivalManualSkillByLevel = base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel);
			if (survivalManualSkillByLevel == null)
			{
				return SurvivalManualType.UpgradeInvalid;
			}
			int survivalManualSkillMaxLevel = base.manager.Player.gameEconomyData.GetSurvivalManualSkillMaxLevel();
			if (SurvivalManualSkillLevel >= survivalManualSkillMaxLevel)
			{
				return SurvivalManualType.UpgradeMaxCondition;
			}
			if (GetSystemLV() < survivalManualSkillByLevel.UnlockLevel)
			{
				return SurvivalManualType.UpgradeNotLevelCondition;
			}
			return SurvivalManualType.UpgradeCondition;
		}

		public FixedPoint GetPrivateHp(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0L;
			}
			if (!IsSurvivalManualInit())
			{
				return 0L;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0L;
			}
			FixedPoint result = 0L;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							result += (FixedPoint)survivalManualActorStory.Attribute_hp_add;
						}
					}
				}
			}
			return result;
		}

		public int GetPrivateHpClient(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0;
			}
			if (!IsSurvivalManualInit())
			{
				return 0;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							num += survivalManualActorStory.Attribute_hp_add;
						}
					}
				}
			}
			return num;
		}

		public FixedPoint GetPrivateAttack(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0L;
			}
			if (!IsSurvivalManualInit())
			{
				return 0L;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0L;
			}
			FixedPoint result = 0L;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							result += (FixedPoint)survivalManualActorStory.Attribute_attack_add;
						}
					}
				}
			}
			return result;
		}

		public int GetAttackClinet(ActorModel actorModel)
		{
			return GetPrivateAttackClient(actorModel) + (int)GetSystemAttack();
		}

		public int GetHPClinet(ActorModel actorModel)
		{
			return GetPrivateHpClient(actorModel) + (int)GetSystemHP();
		}

		public int GetPrivateAttackClient(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0;
			}
			if (!IsSurvivalManualInit())
			{
				return 0;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							num += survivalManualActorStory.Attribute_attack_add;
						}
					}
				}
			}
			return num;
		}

		public FixedPoint GetPrivateAttackRatio(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0L;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0L;
			}
			if (!IsSurvivalManualInit())
			{
				return 0L;
			}
			FixedPoint result = 0L;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							result += (FixedPoint)survivalManualActorStory.Attribute_attack_ratio / (FixedPoint)100.0;
						}
					}
				}
			}
			return result;
		}

		public int GetPrivateAttackRatioClient(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0;
			}
			if (!IsSurvivalManualInit())
			{
				return 0;
			}
			int num = 0;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							num += survivalManualActorStory.Attribute_attack_ratio;
						}
					}
				}
			}
			return num;
		}

		public FixedPoint GetPrivateHpRatio(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0L;
			}
			if (!IsSurvivalManualInit())
			{
				return 0L;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0L;
			}
			FixedPoint result = 0L;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							result += (FixedPoint)survivalManualActorStory.Attribute_hp_ratio / (FixedPoint)100.0;
						}
					}
				}
			}
			return result;
		}

		public int GetPrivateHpRatioClient(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0;
			}
			if (!IsSurvivalManualInit())
			{
				return 0;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							num += survivalManualActorStory.Attribute_hp_ratio;
						}
					}
				}
			}
			return num;
		}

		public FixedPoint GetPrivateCritical(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0L;
			}
			if (!IsSurvivalManualInit())
			{
				return 0L;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0L;
			}
			FixedPoint result = 0L;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							result += (FixedPoint)survivalManualActorStory.Attribute_critical / (FixedPoint)100.0;
						}
					}
				}
			}
			return result;
		}

		public int GetPrivateCriticalClient(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0;
			}
			if (!IsSurvivalManualInit())
			{
				return 0;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							num += survivalManualActorStory.Attribute_critical;
						}
					}
				}
			}
			return num;
		}

		public FixedPoint GetPrivateDmgCriticalRatio(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0L;
			}
			if (!IsSurvivalManualInit())
			{
				return 0L;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0L;
			}
			FixedPoint result = 0L;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							result += (FixedPoint)survivalManualActorStory.Attribute_dmg_critical_ratio / (FixedPoint)100.0;
						}
					}
				}
			}
			return result;
		}

		public int GetPrivateDmgCriticalRatioClient(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0;
			}
			if (!IsSurvivalManualInit())
			{
				return 0;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							num += survivalManualActorStory.Attribute_dmg_critical_ratio;
						}
					}
				}
			}
			return num;
		}

		public FixedPoint GetPrivateDmgTotalRefRatio(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0L;
			}
			if (!IsSurvivalManualInit())
			{
				return 0L;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0L;
			}
			FixedPoint result = 0L;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							result += (FixedPoint)survivalManualActorStory.Attribute_dmg_total_ref_ratio / (FixedPoint)100.0;
						}
					}
				}
			}
			return result;
		}

		public int GetPrivateDmgTotalRefRatioClient(ActorModel actorModel)
		{
			if (actorModel == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0;
			}
			if (!IsSurvivalManualInit())
			{
				return 0;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (survivalManualModel == null || survivalManualModel.SurvivalManualActorStoryNodes == null || survivalManualModel.SurvivalManualActorStoryNodes == null)
				{
					continue;
				}
				foreach (SurvivalManualActorStoryNode survivalManualActorStoryNode in survivalManualModel.SurvivalManualActorStoryNodes)
				{
					if (survivalManualActorStoryNode != null && !(survivalManualActorStoryNode.LinkActorID != actorModel.ActorDefinitionID))
					{
						SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(survivalManualActorStoryNode.StoryActorID, survivalManualActorStoryNode.LinkActorID, survivalManualActorStoryNode.MemoryID);
						if (survivalManualActorStory != null)
						{
							num += survivalManualActorStory.Attribute_dmg_total_ref_ratio;
						}
					}
				}
			}
			return num;
		}

		public FixedPoint GetSystemHP()
		{
			int num = 0;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0L;
				}
				if (SurvivalManualModels != null && SurvivalManualModels.Count > 0)
				{
					for (int i = 0; i < SurvivalManualModels.Count; i++)
					{
						SurvivalManualModel survivalManualModel = SurvivalManualModels[i];
						if (survivalManualModel != null)
						{
							num += (int)survivalManualModel.GetSurvivalManualHp();
						}
					}
				}
			}
			return num;
		}

		public FixedPoint GetSystemAttack()
		{
			int num = 0;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0L;
				}
				if (SurvivalManualModels != null && SurvivalManualModels.Count > 0)
				{
					for (int i = 0; i < SurvivalManualModels.Count; i++)
					{
						SurvivalManualModel survivalManualModel = SurvivalManualModels[i];
						if (survivalManualModel != null)
						{
							num += (int)survivalManualModel.GetSurvivalManualAttack();
						}
					}
				}
			}
			return num;
		}

		public FixedPoint GetAttributeAttackRatio()
		{
			FixedPoint result = 0L;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0L;
				}
				result = (FixedPoint)base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_attack_ratio / (FixedPoint)100.0;
			}
			return result;
		}

		public int GetAttributeAttackRatioClient()
		{
			int result = 0;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0;
				}
				result = base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_attack_ratio;
			}
			return result;
		}

		public FixedPoint GetAttributeHpRatio()
		{
			FixedPoint result = 0L;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0L;
				}
				result = (FixedPoint)base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_hp_ratio / (FixedPoint)100.0;
			}
			return result;
		}

		public int GetAttributeHpRatioClient()
		{
			int result = 0;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0;
				}
				result = base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_hp_ratio;
			}
			return result;
		}

		public FixedPoint GetAttributeHitrateMelee()
		{
			FixedPoint result = 0L;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0L;
				}
				if (base.manager?.Player?.gameEconomyData != null)
				{
					result = (FixedPoint)base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_hitrate_melee / (FixedPoint)100.0;
				}
			}
			return result;
		}

		public int GetAttributeHitrateMeleeClient()
		{
			int result = 0;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0;
				}
				if (base.manager?.Player?.gameEconomyData != null)
				{
					result = base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_hitrate_melee;
				}
			}
			return result;
		}

		public FixedPoint GetAttributeHitrateRange()
		{
			FixedPoint result = 0L;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0L;
				}
				result = (FixedPoint)base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_hitrate_range / (FixedPoint)100.0;
			}
			return result;
		}

		public int GetAttributeHitrateRangeClient()
		{
			int result = 0;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0;
				}
				result = base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_hitrate_range;
			}
			return result;
		}

		public FixedPoint GetAttributeCriticalRef()
		{
			FixedPoint result = 0L;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0L;
				}
				result = (FixedPoint)base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_critical_ref / (FixedPoint)100.0;
			}
			return result;
		}

		public int GetAttributeCriticalRefClient()
		{
			int result = 0;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0;
				}
				result = base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_critical_ref;
			}
			return result;
		}

		public FixedPoint GetAttributeDmgCriticalRatioRef()
		{
			FixedPoint result = 0L;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0L;
				}
				result = (FixedPoint)base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_dmg_critical_ratio_ref / (FixedPoint)100.0;
			}
			return result;
		}

		public int GetAttributeDmgCriticalRatioRefClient()
		{
			int result = 0;
			if (base.manager?.Player?.gameEconomyData != null)
			{
				if (!IsSurvivalManualInit())
				{
					return 0;
				}
				result = base.manager.Player.gameEconomyData.GetSurvivalManualSkillByLevel(SurvivalManualSkillLevel).Attribute_dmg_critical_ratio_ref;
			}
			return result;
		}

		public bool IsSurvivalManualInit()
		{
			if (base.manager != null && base.manager.Player != null && base.manager.Player.gameEconomyData != null)
			{
				SystemOpen systemOpenById = base.manager.Player.gameEconomyData.GetSystemOpenById("SystemBase.Survival_Manual");
				if (base.manager.Player.CouncilLevel >= systemOpenById.OpenCampLv)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsSystemBaseSurvivalManualOpen()
		{
			if (base.manager != null && base.manager.Player != null && base.manager.Player.gameEconomyData != null)
			{
				SystemOpen systemOpenById = base.manager.Player.gameEconomyData.GetSystemOpenById("SystemBase.Survival_Manual");
				if (base.manager.Player.CouncilLevel >= systemOpenById.OpenCampLv && base.manager.Player.UtcTimeStamp >= systemOpenById.StartTimeMilliseconds && base.manager.Player.UtcTimeStamp <= systemOpenById.EndTimeMilliseconds)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsSystemBaseActorSheetSurvivalManualOpen()
		{
			if (base.manager != null && base.manager.Player != null && base.manager.Player.gameEconomyData != null)
			{
				SystemOpen systemOpenById = base.manager.Player.gameEconomyData.GetSystemOpenById("SystemBase.ActorSheet.Survival_Manual");
				if (base.manager.Player.CouncilLevel >= systemOpenById.OpenCampLv && base.manager.Player.UtcTimeStamp >= systemOpenById.StartTimeMilliseconds && base.manager.Player.UtcTimeStamp <= systemOpenById.EndTimeMilliseconds)
				{
					return true;
				}
			}
			return false;
		}

		public bool ActivatedSurvivalManualTraits()
		{
			if (base.manager?.Player?.gameEconomyData == null || !IsSurvivalManualInit() || SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return false;
			}
			if (ActorSurvivalManualStorySkillList == null)
			{
				ActorSurvivalManualStorySkillList = new Dictionary<string, List<SurvivalManualActorStoryLockTrait>>();
			}
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				SurvivalManualDefinition survivalManualDefinitionById = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(survivalManualModel.ID);
				if (survivalManualDefinitionById?.ActorList == null || survivalManualModel.InitSurvivalManual)
				{
					continue;
				}
				foreach (string actor in survivalManualDefinitionById.ActorList)
				{
					SurvivorModel survivorByStoryActorId = survivalManualModel.GetSurvivorByStoryActorId(actor);
					if (survivorByStoryActorId == null)
					{
						continue;
					}
					if (!ActorSurvivalManualStorySkillList.TryGetValue(survivorByStoryActorId.ActorDefinitionID, out var value))
					{
						value = new List<SurvivalManualActorStoryLockTrait>();
						ActorSurvivalManualStorySkillList[survivorByStoryActorId.ActorDefinitionID] = value;
					}
					SurvivalManualStorySkill newSkill = base.manager.Player.gameEconomyData.GetSurvivalManualStorySkillLevel(survivalManualDefinitionById.StoryQueueSkill, survivalManualModel.ManualActorStoryLockTraits?.Level ?? 0);
					if (value.Count <= 0 || !value.Any((SurvivalManualActorStoryLockTrait t) => t.TraitID == newSkill.ID))
					{
						SurvivalManualActorStoryLockTrait item = new SurvivalManualActorStoryLockTrait
						{
							TraitID = newSkill.ID,
							SurvivalManualID = survivalManualModel.ID
						};
						value.Add(item);
						SurvivalManualStorySkill survivalManualStorySkillLevel = base.manager.Player.gameEconomyData.GetSurvivalManualStorySkillLevel(survivalManualDefinitionById.StoryQueueSkill, survivalManualModel.ManualActorStoryLockTraits?.Level ?? 0);
						if (survivalManualStorySkillLevel != null && !survivorByStoryActorId.HasTrait(survivalManualStorySkillLevel.UpgradeTraits))
						{
							survivorByStoryActorId.AddTrait(survivalManualStorySkillLevel.UpgradeTraits);
						}
					}
				}
				survivalManualModel.InitSurvivalManual = true;
			}
			return true;
		}

		public List<ManualActorStoryLockTrait> GetActorTraitList(ActorModel actor)
		{
			List<ManualActorStoryLockTrait> list = new List<ManualActorStoryLockTrait>();
			if (actor == null || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return list;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return list;
			}
			string survivalManualStoryId = base.manager.Player.gameEconomyData.GetSurvivalManualStoryId(actor.ActorDefinitionID);
			new List<object>();
			foreach (SurvivalManualModel survivalManualModel in SurvivalManualModels)
			{
				if (IsActorInSurvivalManual(survivalManualStoryId, survivalManualModel.ID))
				{
					list.Add(survivalManualModel.ManualActorStoryLockTraits);
				}
			}
			return list;
		}

		public bool IsActorInSurvivalManual(string actorStoryId, int survivalManualDefinitionId)
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return false;
			}
			foreach (string actor in base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(survivalManualDefinitionId).ActorList)
			{
				if (actor == actorStoryId)
				{
					return true;
				}
			}
			return false;
		}

		public virtual TWDModelResult UpgradeSurvivalManualStorySkill(int definitionId)
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return TWDModelResult.Error;
			}
			if (SurvivalManualModels == null || SurvivalManualModels.Count == 0)
			{
				return TWDModelResult.Error;
			}
			if (GetSurvivalManualStorySkillCanUpgradeState(definitionId) != SurvivalManualType.UpgradeCondition)
			{
				return TWDModelResult.Error;
			}
			SurvivalManualModel survivalManualModel = null;
			foreach (SurvivalManualModel survivalManualModel2 in SurvivalManualModels)
			{
				if (survivalManualModel2.ID == definitionId)
				{
					survivalManualModel = survivalManualModel2;
					break;
				}
			}
			if (survivalManualModel == null)
			{
				return TWDModelResult.Error;
			}
			SurvivalManualDefinition survivalManualDefinitionById = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(survivalManualModel.ID);
			if (survivalManualDefinitionById == null)
			{
				return TWDModelResult.Error;
			}
			if (survivalManualModel.ManualActorStoryLockTraits == null)
			{
				survivalManualModel.ManualActorStoryLockTraits = new ManualActorStoryLockTrait
				{
					Level = 0,
					Id = 0,
					State = ManualActorStoryTraitState.Activated,
					Type = survivalManualDefinitionById.StoryQueueSkill
				};
			}
			ManualActorStoryLockTrait manualActorStoryLockTraits = survivalManualModel.ManualActorStoryLockTraits;
			int maxStorySkillLevelByType = base.manager.Player.gameEconomyData.GetMaxStorySkillLevelByType(manualActorStoryLockTraits.Type);
			if (manualActorStoryLockTraits.Level >= maxStorySkillLevelByType)
			{
				return TWDModelResult.AlreadyMaxLevel;
			}
			SurvivalManualStorySkill survivalManualStorySkillLevel = base.manager.Player.gameEconomyData.GetSurvivalManualStorySkillLevel(manualActorStoryLockTraits.Type, manualActorStoryLockTraits.Level);
			if (survivalManualStorySkillLevel == null)
			{
				return TWDModelResult.Error;
			}
			Cashier cashier = new Cashier(base.manager);
			foreach (KeyValuePair<CurrencyType, int> item in survivalManualStorySkillLevel.GetUpgradCostInfo())
			{
				CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeSurvivalManualStorySkill);
				cashierItem.SetCost(item.Key, item.Value);
				cashier.AddItem(cashierItem);
			}
			TWDModelResult tWDModelResult = cashier.Pay(survivalManualStorySkillLevel);
			if (tWDModelResult != TWDModelResult.OK)
			{
				return tWDModelResult;
			}
			manualActorStoryLockTraits.Level++;
			SurvivalManualStorySkill survivalManualStorySkillLevel2 = base.manager.Player.gameEconomyData.GetSurvivalManualStorySkillLevel(manualActorStoryLockTraits.Type, manualActorStoryLockTraits.Level);
			if (survivalManualStorySkillLevel2 != null)
			{
				manualActorStoryLockTraits.Id = survivalManualStorySkillLevel2.ID;
				SetActorStorySkillToActor(survivalManualModel, survivalManualDefinitionById, survivalManualStorySkillLevel2, survivalManualStorySkillLevel);
			}
			return TWDModelResult.OK;
		}

		public void SetActorStorySkillToActor(SurvivalManualModel smodel, SurvivalManualDefinition survivalManualDef, SurvivalManualStorySkill newSkill, SurvivalManualStorySkill oldSkill)
		{
			if (smodel == null || survivalManualDef == null || newSkill == null || oldSkill == null)
			{
				return;
			}
			foreach (string actor in survivalManualDef.ActorList)
			{
				string survivalManualActorId = base.manager.Player.gameEconomyData.GetSurvivalManualActorId(actor);
				SurvivorModel survivorByStoryActorId = smodel.GetSurvivorByStoryActorId(actor);
				if (survivorByStoryActorId != null && !(survivorByStoryActorId.ActorDefinitionID != survivalManualActorId))
				{
					survivorByStoryActorId.RemoveTrait(oldSkill.UpgradeTraits);
					survivorByStoryActorId.AddTrait(newSkill.UpgradeTraits);
					ReplaceOrAddActorTrait(survivorByStoryActorId.ActorDefinitionID, oldSkill.ID, newSkill.ID, smodel.ID);
				}
			}
		}

		public void ReplaceOrAddActorTrait(string key, int oldValue, int newValue, int survivalManualId)
		{
			if (!string.IsNullOrEmpty(key))
			{
				if (ActorSurvivalManualStorySkillList == null)
				{
					ActorSurvivalManualStorySkillList = new Dictionary<string, List<SurvivalManualActorStoryLockTrait>>();
				}
				if (!ActorSurvivalManualStorySkillList.TryGetValue(key, out var value))
				{
					value = new List<SurvivalManualActorStoryLockTrait>();
					ActorSurvivalManualStorySkillList[key] = value;
				}
				SurvivalManualActorStoryLockTrait survivalManualActorStoryLockTrait = value.FirstOrDefault((SurvivalManualActorStoryLockTrait t) => t.TraitID == oldValue);
				if (survivalManualActorStoryLockTrait != null)
				{
					value.Remove(survivalManualActorStoryLockTrait);
				}
				if (survivalManualId == 0)
				{
					base.manager.Debug.LogError($"没有可用的非 0 ID，不添加：key={key}, oldValue={oldValue}, newValue={newValue}");
				}
				else if (!value.Any((SurvivalManualActorStoryLockTrait t) => t.TraitID == newValue))
				{
					value.Add(new SurvivalManualActorStoryLockTrait
					{
						TraitID = newValue,
						SurvivalManualID = survivalManualId
					});
				}
			}
		}

		public SurvivalManualType GetSurvivalManualStorySkillCanUpgradeState(int definitionId)
		{
			if (base.manager != null || base.manager.Player != null || base.manager.Player.gameEconomyData != null)
			{
				SurvivalManualModel survivalManualModel = GetSurvivalManualModel(definitionId);
				if (survivalManualModel != null && survivalManualModel.ManualActorStoryLockTraits != null)
				{
					SurvivalManualStorySkill survivalManualStorySkillLevel = base.manager.Player.gameEconomyData.GetSurvivalManualStorySkillLevel(survivalManualModel.ManualActorStoryLockTraits.Type, survivalManualModel.ManualActorStoryLockTraits.Level);
					int maxStorySkillLevelByType = base.manager.Player.gameEconomyData.GetMaxStorySkillLevelByType(survivalManualModel.ManualActorStoryLockTraits.Type);
					if (survivalManualModel.ManualActorStoryLockTraits.Level >= maxStorySkillLevelByType)
					{
						return SurvivalManualType.UpgradeMaxCondition;
					}
					if (survivalManualModel.GetTotalActorsAllLevel() < survivalManualStorySkillLevel.UnlockLevel)
					{
						return SurvivalManualType.UpgradeNotLevelCondition;
					}
					if (survivalManualModel.GetTotalActorsAllLevel() >= survivalManualStorySkillLevel.UnlockLevel)
					{
						return SurvivalManualType.UpgradeCondition;
					}
				}
			}
			return SurvivalManualType.UpgradeNotLevelCondition;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
