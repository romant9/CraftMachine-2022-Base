using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;

namespace TWDModel
{
	public class SurvivalManualModel : TWDModelObject
	{
		private static long CheckForNewNewSurvivalManuaDefaultTime = 60000L;

		private static long CheckForLimitedSurvivalManuasCombatInterval = 5000L;

		private static long CheckForLimitedSurvivalManuasNonCombatInterval = 500L;

		public const string LimitedEventAvailableEvent = "LimitedEventAvailableEvent";

		public const string LimitedEventExpiredEvent = "LimitedEventExpiredEvent";

		public bool SurvivalManualEmblesState;

		public bool InitSurvivalManual { get; set; }

		public long CheckForLimitedSurvivalManuasTimer { get; set; }

		public Dictionary<string, int> SurvivalManualActorLvs { get; set; }

		public ManualActorStoryLockTrait ManualActorStoryLockTraits { get; set; }

		[JsonIgnore]
		public SurvivalManualStorySkill SkillDefinition => base.gameEconomyData.GetSurvivalManualStorySkillLevel(ManualActorStoryLockTraits.Type, ManualActorStoryLockTraits.Level);

		public ModelList<SurvivalManualActorStoryNode> SurvivalManualActorStoryNodes { get; set; }

		[JsonIgnore]
		public SurvivalManualDefinition SurvivalManualDefinition => base.gameEconomyData.GetSurvivalManualDefinitionById(ID);

		public int ID { get; set; }

		public bool IsAvailable { get; set; }

		public long Timer { get; set; }

		public string StartTimestamp { get; set; }

		public string EndTimestamp { get; set; }

		public override void Initialize()
		{
			base.Initialize();
			SurvivalManualActorLvs = new Dictionary<string, int>();
			SurvivalManualActorStoryNodes = new ModelList<SurvivalManualActorStoryNode>();
			SurvivalManualActorStoryNodes.SetManager(base.manager);
			SurvivalManualActorStoryNodes.Initialize();
		}

		public override void Start()
		{
			base.Start();
			if (SurvivalManualActorLvs == null)
			{
				SurvivalManualActorLvs = new Dictionary<string, int>();
			}
			if (SurvivalManualActorStoryNodes == null)
			{
				SurvivalManualActorStoryNodes = new ModelList<SurvivalManualActorStoryNode>();
				SurvivalManualActorStoryNodes.SetManager(base.manager);
				SurvivalManualActorStoryNodes.Initialize();
			}
		}

		public void CheckSurvivalManualEmblesState()
		{
			SurvivalManualDefinition survivalManualDefinitionById = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID);
			if (GetTotalActorsAllLevel() >= survivalManualDefinitionById.SouvenirMedalLevel)
			{
				SurvivalManualEmblesState = true;
			}
		}

		public override void Tick(long deltaTime)
		{
			base.Tick(deltaTime);
			long num = CheckForLimitedSurvivalManuasNonCombatInterval;
			if (base.manager?.Player?.Combat != null)
			{
				num = CheckForLimitedSurvivalManuasCombatInterval;
			}
			CheckForLimitedSurvivalManuasTimer += deltaTime;
			if (CheckForLimitedSurvivalManuasTimer >= num)
			{
				TickRegisteredLimitedEvent(CheckForLimitedSurvivalManuasTimer);
				CheckForLimitedSurvivalManuasTimer = 0L;
			}
		}

		private void TickRegisteredLimitedEvent(long deltaTime)
		{
			SurvivalManualDefinition survivalManualDefinition = base.manager?.Player?.gameEconomyData.GetSurvivalManualDefinitionById(ID);
			if (survivalManualDefinition == null)
			{
				return;
			}
			long num = survivalManualDefinition.EndTimeMilliseconds - base.manager.Player.UtcTimeStamp;
			long timer = Timer;
			Timer -= deltaTime;
			if (num > 0)
			{
				Timer = Math.Min(Timer, num);
				if (!IsAvailable)
				{
					IsAvailable = !IsAvailable;
					StartTimestamp = survivalManualDefinition.ActiveOpenTime;
					EndTimestamp = survivalManualDefinition.ActiveEndTime;
				}
			}
			if (Timer > 0)
			{
				return;
			}
			IsAvailable = !IsAvailable;
			if (IsAvailable)
			{
				if (survivalManualDefinition.HasDateLimit)
				{
					long utcTimeStamp = base.manager.Player.UtcTimeStamp;
					long endTimeMilliseconds = survivalManualDefinition.EndTimeMilliseconds;
					Timer = Math.Max(0L, endTimeMilliseconds - utcTimeStamp);
				}
				else
				{
					Timer = 0L;
				}
			}
			else
			{
				long num2 = deltaTime - timer;
				Timer = Math.Max(0L, Math.Min(Timer, Timer - num2));
			}
			NotifyChange(IsAvailable ? "LimitedEventAvailableEvent" : "LimitedEventExpiredEvent", ID);
		}

		public int GetTotalActorsAllLevel()
		{
			if (SurvivalManualActorLvs == null)
			{
				return 0;
			}
			return SurvivalManualActorLvs?.Values.Sum() ?? 0;
		}

		public int GetActorLevel(string storyActorID)
		{
			if (SurvivalManualActorLvs == null)
			{
				return 0;
			}
			if (!SurvivalManualActorLvs.TryGetValue(storyActorID, out var value))
			{
				return 0;
			}
			return value;
		}

		private void SetActorLevel(string storyActorID, int level)
		{
			if (string.IsNullOrEmpty(storyActorID))
			{
				return;
			}
			if (SurvivalManualActorLvs == null)
			{
				SurvivalManualActorLvs = new Dictionary<string, int>();
			}
			if (SurvivalManualActorLvs.TryGetValue(storyActorID, out var value))
			{
				if (level > value)
				{
					SurvivalManualActorLvs[storyActorID] = level;
				}
			}
			else
			{
				SurvivalManualActorLvs.Add(storyActorID, level);
			}
		}

		public List<SurvivalManualActorLevel> GetAllActorPublicProperties()
		{
			List<SurvivalManualActorLevel> list = new List<SurvivalManualActorLevel>();
			if (SurvivalManualActorLvs == null || base.manager?.Player?.gameEconomyData == null)
			{
				return list;
			}
			List<string> actorList = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID).ActorList;
			for (int i = 0; i < actorList.Count; i++)
			{
				int value;
				int level = (SurvivalManualActorLvs.TryGetValue(actorList[i], out value) ? value : 0);
				int actorLevelAttrUpgrade = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID).ActorLevelAttrUpgrade;
				SurvivalManualActorLevel actorLeveDefinition = base.manager.Player.gameEconomyData.GetActorLeveDefinition(actorLevelAttrUpgrade, level);
				list.Add(actorLeveDefinition);
			}
			return list;
		}

		public SurvivalManualActorLevel GetActorPublicProperties(string storyActorID)
		{
			if (string.IsNullOrEmpty(storyActorID) || SurvivalManualActorLvs == null || base.manager?.Player?.gameEconomyData == null)
			{
				return null;
			}
			int value;
			int level = (SurvivalManualActorLvs.TryGetValue(storyActorID, out value) ? value : 0);
			int actorLevelAttrUpgrade = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID).ActorLevelAttrUpgrade;
			return base.manager.Player.gameEconomyData.GetActorLeveDefinition(actorLevelAttrUpgrade, level);
		}

		public FixedPoint GetSurvivalManualHp()
		{
			int num = 0;
			List<SurvivalManualActorLevel> allActorPublicProperties = GetAllActorPublicProperties();
			if (allActorPublicProperties != null)
			{
				for (int i = 0; i < allActorPublicProperties.Count; i++)
				{
					SurvivalManualActorLevel survivalManualActorLevel = allActorPublicProperties[i];
					if (survivalManualActorLevel != null)
					{
						num += survivalManualActorLevel.Attribute_hp_add;
					}
				}
			}
			return num;
		}

		public FixedPoint GetSurvivalManualAttack()
		{
			int num = 0;
			List<SurvivalManualActorLevel> allActorPublicProperties = GetAllActorPublicProperties();
			if (allActorPublicProperties == null || allActorPublicProperties.Count == 0)
			{
				return 0L;
			}
			foreach (SurvivalManualActorLevel item in allActorPublicProperties)
			{
				if (item != null)
				{
					num += item.Attribute_attack_add;
				}
			}
			return num;
		}

		public TWDModelResult UnlockSurvivalManualActorStory(string storyActorID, int memoryID)
		{
			if (string.IsNullOrEmpty(storyActorID) || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return TWDModelResult.Error;
			}
			if (GetSurvivalManualStoryUnlockStatus(storyActorID, memoryID) != StoryUnlockStatus.Unlockable)
			{
				return TWDModelResult.Error;
			}
			if (SurvivalManualActorStoryNodes == null)
			{
				SurvivalManualActorStoryNodes = new ModelList<SurvivalManualActorStoryNode>();
				SurvivalManualActorStoryNodes.SetManager(base.manager);
				SurvivalManualActorStoryNodes.Initialize();
			}
			SurvivalManualActorStoryNode survivalManualActorStoryNode = null;
			for (int i = 0; i < SurvivalManualActorStoryNodes.Count; i++)
			{
				SurvivalManualActorStoryNode survivalManualActorStoryNode2 = SurvivalManualActorStoryNodes[i];
				if (survivalManualActorStoryNode2 != null && survivalManualActorStoryNode2.StoryActorID == storyActorID && survivalManualActorStoryNode2.MemoryID == memoryID)
				{
					survivalManualActorStoryNode = survivalManualActorStoryNode2;
					break;
				}
			}
			if (survivalManualActorStoryNode == null)
			{
				string survivalManualActorId = base.manager.Player.gameEconomyData.GetSurvivalManualActorId(storyActorID);
				survivalManualActorStoryNode = new SurvivalManualActorStoryNode();
				survivalManualActorStoryNode.Initialize();
				survivalManualActorStoryNode.SetManager(base.manager);
				survivalManualActorStoryNode.Start();
				survivalManualActorStoryNode.StoryActorID = storyActorID;
				survivalManualActorStoryNode.LinkActorID = survivalManualActorId;
				survivalManualActorStoryNode.Status = StoryUnlockStatus.Unlocked;
				survivalManualActorStoryNode.MemoryID = memoryID;
				SurvivalManualActorStoryNodes.Add(survivalManualActorStoryNode);
				return TWDModelResult.OK;
			}
			base.manager.Debug.LogError("SurvivalManualActorStoryNodes 已存在该模型 storyActorID:" + storyActorID + " memoryID:" + memoryID);
			return TWDModelResult.Error;
		}

		public StoryUnlockStatus GetSurvivalManualStoryUnlockStatus(string storyActorId, int memoryID)
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return StoryUnlockStatus.Invalid;
			}
			SurvivalManualActorStory survivalManualActorStory = base.manager.Player.gameEconomyData.GetSurvivalManualActorStory(storyActorId, memoryID);
			if (survivalManualActorStory == null)
			{
				return StoryUnlockStatus.Invalid;
			}
			if (survivalManualActorStory.StartMemoryUnlockTime > base.manager.Player.UtcTimeStamp)
			{
				return StoryUnlockStatus.NotOpen;
			}
			if (GetActorLevel(storyActorId) < survivalManualActorStory.MemoryUnlockLevel)
			{
				return StoryUnlockStatus.Locked;
			}
			SurvivalManualActorStoryNode survivalManualActorStoryNode = SurvivalManualActorStoryNodes?.FirstOrDefault((SurvivalManualActorStoryNode s) => s?.StoryActorID == storyActorId && s.MemoryID == memoryID);
			if (survivalManualActorStoryNode != null && survivalManualActorStoryNode.Status == StoryUnlockStatus.Unlocked)
			{
				return StoryUnlockStatus.Unlocked;
			}
			return StoryUnlockStatus.Unlockable;
		}

		public int CalculateMaxLevelByFragments(string storyActorID)
		{
			if (string.IsNullOrEmpty(storyActorID) || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0;
			}
			SurvivalManualDefinition survivalManualDefinitionById = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID);
			if (survivalManualDefinitionById == null)
			{
				return 0;
			}
			SurvivorModel survivorByStoryActorId = GetSurvivorByStoryActorId(storyActorID);
			if (survivorByStoryActorId == null || survivorByStoryActorId.Definition == null)
			{
				return 0;
			}
			int num = survivorByStoryActorId.SurvivorRarityLevel + 1;
			CurrencyType traitUpgradeCurrency = survivorByStoryActorId.Definition.TraitUpgradeCurrency;
			int currencyAmount = base.manager.Player.GetCurrencyAmount(traitUpgradeCurrency);
			int actorLevel = GetActorLevel(storyActorID);
			List<SurvivalManualActorLevel> survivalManualActorLevels = base.manager.Player.gameEconomyData.GetSurvivalManualActorLevels(survivalManualDefinitionById.ActorLevelAttrUpgrade);
			if (survivalManualActorLevels == null || survivalManualActorLevels.Count == 0)
			{
				return actorLevel;
			}
			int num2 = 0;
			int num3 = actorLevel;
			for (int i = 0; i < survivalManualActorLevels.Count; i++)
			{
				SurvivalManualActorLevel survivalManualActorLevel = survivalManualActorLevels[i];
				if (survivalManualActorLevel.Level >= actorLevel)
				{
					if (!survivalManualDefinitionById.IsActiveEvent && i + 1 < survivalManualActorLevels.Count && num < survivalManualActorLevels[i + 1].UnlockActorStarLevel)
					{
						break;
					}
					num2 += survivalManualActorLevel.CostToken;
					if (num2 > currencyAmount)
					{
						break;
					}
					num3 = survivalManualActorLevel.Level + 1;
				}
			}
			int maxLevelByType = base.manager.Player.gameEconomyData.GetMaxLevelByType(survivalManualDefinitionById.ActorLevelAttrUpgrade);
			if (num3 > maxLevelByType)
			{
				num3 = maxLevelByType;
			}
			if (actorLevel > num3)
			{
				return 0;
			}
			return num3;
		}

		public int CalculateTotalFragmentsToMaxLevel(string storyActorID)
		{
			if (string.IsNullOrEmpty(storyActorID) || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return 0;
			}
			SurvivalManualDefinition survivalManualDefinitionById = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID);
			if (survivalManualDefinitionById == null)
			{
				return 0;
			}
			SurvivorModel survivorByStoryActorId = GetSurvivorByStoryActorId(storyActorID);
			if (survivorByStoryActorId == null || survivorByStoryActorId.Definition == null)
			{
				return 0;
			}
			int actorLevel = GetActorLevel(storyActorID);
			List<SurvivalManualActorLevel> survivalManualActorLevels = base.manager.Player.gameEconomyData.GetSurvivalManualActorLevels(survivalManualDefinitionById.ActorLevelAttrUpgrade);
			if (survivalManualActorLevels == null || survivalManualActorLevels.Count == 0)
			{
				return 0;
			}
			int num = CalculateMaxLevelByFragments(storyActorID);
			if (actorLevel > num)
			{
				return 0;
			}
			int num2 = 0;
			foreach (SurvivalManualActorLevel item in survivalManualActorLevels.OrderBy((SurvivalManualActorLevel x) => x.Level))
			{
				if (item.Level >= actorLevel && item.Level < num)
				{
					num2 += item.CostToken;
				}
			}
			return num2;
		}

		public virtual TWDModelResult OneClickUpgradeActors(List<string> storyActorIDs)
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return TWDModelResult.Error;
			}
			SurvivalManualDefinition survivalManualDefinitionById = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID);
			if (survivalManualDefinitionById == null)
			{
				return TWDModelResult.Error;
			}
			foreach (string storyActorID in storyActorIDs)
			{
				if (GetStoryActorCanUpgradeState(storyActorID) != StoryActorType.Upgradable)
				{
					return TWDModelResult.Error;
				}
				SurvivorModel survivorByStoryActorId = GetSurvivorByStoryActorId(storyActorID);
				if (survivorByStoryActorId == null || survivorByStoryActorId.Definition == null)
				{
					return TWDModelResult.Error;
				}
				CurrencyType traitUpgradeCurrency = survivorByStoryActorId.Definition.TraitUpgradeCurrency;
				int num = CalculateMaxLevelByFragments(storyActorID);
				int actorLevel = GetActorLevel(storyActorID);
				SurvivalManualActorLevel survivalManualActorLevel = base.manager.Player.gameEconomyData.GetSurvivalManualActorLevel(survivalManualDefinitionById.ActorLevelAttrUpgrade, num);
				if (survivalManualActorLevel == null)
				{
					return TWDModelResult.Error;
				}
				int num2 = survivorByStoryActorId.SurvivorRarityLevel + 1;
				if (survivalManualDefinitionById.IsActiveEvent || num2 >= survivalManualActorLevel.UnlockActorStarLevel)
				{
					int maxLevelByType = base.manager.Player.gameEconomyData.GetMaxLevelByType(survivalManualDefinitionById.ActorLevelAttrUpgrade);
					if (actorLevel >= maxLevelByType)
					{
						return TWDModelResult.AlreadyMaxLevel;
					}
					if (num <= 0 || num > maxLevelByType)
					{
						base.manager.Debug.LogError("计算升级方案中根据碎片数量计算出来的等级错误 SurvivalManualDefinitionID " + ID + "storyActorID :" + storyActorID);
						return TWDModelResult.Error;
					}
					if (actorLevel >= num)
					{
						return TWDModelResult.AlreadyMaxLevel;
					}
					int num3 = CalculateTotalFragmentsToMaxLevel(storyActorID);
					if (num3 <= 0)
					{
						return TWDModelResult.Error;
					}
					Cashier cashier = new Cashier(base.manager);
					CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeSurvivalManualActor);
					cashierItem.SetCost(traitUpgradeCurrency, num3);
					cashier.AddItem(cashierItem);
					if (cashier.Pay(survivalManualActorLevel) == TWDModelResult.OK)
					{
						SetActorLevel(storyActorID, num);
						CheckSurvivalManualEmblesState();
					}
					else
					{
						base.manager.Debug.LogError("upgrate actor error SurvivalManualDefinitionID " + ID + "storyActorID :" + storyActorID);
					}
				}
			}
			return TWDModelResult.OK;
		}

		public virtual TWDModelResult UpgradeActor(List<string> storyActorIDs)
		{
			if (storyActorIDs.Count == 0 || base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return TWDModelResult.Error;
			}
			SurvivalManualDefinition survivalManualDefinitionById = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID);
			if (survivalManualDefinitionById == null)
			{
				return TWDModelResult.Error;
			}
			foreach (string storyActorID in storyActorIDs)
			{
				if (GetStoryActorCanUpgradeState(storyActorID) != StoryActorType.Upgradable)
				{
					return TWDModelResult.Error;
				}
				int actorLevel = GetActorLevel(storyActorID);
				int num = CalculateMaxLevelByFragments(storyActorID);
				if (actorLevel >= num)
				{
					return TWDModelResult.AlreadyMaxLevel;
				}
				int maxLevelByType = base.manager.Player.gameEconomyData.GetMaxLevelByType(survivalManualDefinitionById.ActorLevelAttrUpgrade);
				if (actorLevel >= maxLevelByType)
				{
					return TWDModelResult.AlreadyMaxLevel;
				}
				if (survivalManualDefinitionById == null)
				{
					return TWDModelResult.Error;
				}
				SurvivalManualActorLevel survivalManualActorLevel = base.manager.Player.gameEconomyData.GetSurvivalManualActorLevel(survivalManualDefinitionById.ActorLevelAttrUpgrade, actorLevel);
				if (survivalManualActorLevel == null)
				{
					return TWDModelResult.Error;
				}
				SurvivorModel survivorByStoryActorId = GetSurvivorByStoryActorId(storyActorID);
				if (survivorByStoryActorId == null || survivorByStoryActorId.Definition == null)
				{
					return TWDModelResult.Error;
				}
				int num2 = survivorByStoryActorId.SurvivorRarityLevel + 1;
				if (!survivalManualDefinitionById.IsActiveEvent && num2 < survivalManualActorLevel.UnlockActorStarLevel)
				{
					return TWDModelResult.Error;
				}
				CurrencyType traitUpgradeCurrency = survivorByStoryActorId.Definition.TraitUpgradeCurrency;
				Cashier cashier = new Cashier(base.manager);
				CashierItem cashierItem = new CashierItem(PurchaseType.UpgradeSurvivalManualActor);
				cashierItem.SetCost(traitUpgradeCurrency, survivalManualActorLevel.CostToken);
				cashier.AddItem(cashierItem);
				if (cashier.Pay(survivalManualActorLevel) == TWDModelResult.OK)
				{
					SetActorLevel(storyActorID, actorLevel + 1);
					CheckSurvivalManualEmblesState();
				}
			}
			return TWDModelResult.OK;
		}

		public SurvivorModel GetSurvivorByStoryActorId(string storyActorId)
		{
			if (string.IsNullOrEmpty(storyActorId))
			{
				return null;
			}
			if (base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return null;
			}
			PlayerModel player = base.manager.Player;
			string survivalManualActorId = player.gameEconomyData.GetSurvivalManualActorId(storyActorId);
			if (string.IsNullOrEmpty(survivalManualActorId))
			{
				return null;
			}
			SurvivorContainerModel survivorContainer = player.SurvivorContainer;
			if (survivorContainer == null || survivorContainer.Survivors == null || survivorContainer.Survivors.Count == 0)
			{
				return null;
			}
			foreach (SurvivorModel survivor in survivorContainer.Survivors)
			{
				if (string.Equals(survivor.ActorDefinitionID, survivalManualActorId, StringComparison.Ordinal))
				{
					return survivor;
				}
			}
			return null;
		}

		public List<string> GetActorLinkIdList()
		{
			List<string> list = new List<string>();
			if (base.manager != null || base.manager.Player != null || base.manager.Player.gameEconomyData != null)
			{
				foreach (string actor in base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID).ActorList)
				{
					string survivalManualActorId = base.manager.Player.gameEconomyData.GetSurvivalManualActorId(actor);
					list.Add(survivalManualActorId);
				}
			}
			return list;
		}

		public FixedPoint GetActorMaxLevel()
		{
			if (base.manager != null || base.manager.Player != null || base.manager.Player.gameEconomyData != null)
			{
				SurvivalManualDefinition survivalManualDefinitionById = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID);
				if (survivalManualDefinitionById != null)
				{
					return base.manager.Player.gameEconomyData.GetMaxLevelByType(survivalManualDefinitionById.ActorLevelAttrUpgrade);
				}
			}
			return 0L;
		}

		public bool IsSurvivalInMaxLevel()
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return false;
			}
			SurvivalManualDefinition survivalManualDefinitionById = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID);
			if (survivalManualDefinitionById == null)
			{
				return false;
			}
			foreach (string actor in survivalManualDefinitionById.ActorList)
			{
				int actorLevel = GetActorLevel(actor);
				FixedPoint actorMaxLevel = GetActorMaxLevel();
				if (actorLevel < actorMaxLevel)
				{
					return false;
				}
			}
			return true;
		}

		public StoryActorType GetStoryActorCanUpgradeState(string storyActorId)
		{
			if (base.manager == null || base.manager.Player == null || base.manager.Player.gameEconomyData == null)
			{
				return StoryActorType.Invalid;
			}
			SurvivalManualDefinition survivalManualDefinitionById = base.manager.Player.gameEconomyData.GetSurvivalManualDefinitionById(ID);
			if (survivalManualDefinitionById == null)
			{
				return StoryActorType.Invalid;
			}
			SurvivorModel survivorByStoryActorId = GetSurvivorByStoryActorId(storyActorId);
			if (survivorByStoryActorId == null)
			{
				return StoryActorType.NotObtained;
			}
			int actorLevel = GetActorLevel(storyActorId);
			FixedPoint actorMaxLevel = GetActorMaxLevel();
			SurvivalManualActorLevel survivalManualActorLevel = base.manager.Player.gameEconomyData.GetSurvivalManualActorLevel(survivalManualDefinitionById.ActorLevelAttrUpgrade, actorLevel);
			if (survivalManualActorLevel == null)
			{
				return StoryActorType.Invalid;
			}
			if (survivorByStoryActorId.SurvivorRarityLevel + 1 < survivalManualActorLevel.UnlockActorStarLevel)
			{
				return StoryActorType.StarLevelTooLow;
			}
			CurrencyType traitUpgradeCurrency = survivorByStoryActorId.Definition.TraitUpgradeCurrency;
			if (base.manager.Player.GetCurrencyAmount(traitUpgradeCurrency) < survivalManualActorLevel.CostToken)
			{
				return StoryActorType.NotEnoughFragments;
			}
			if (actorLevel >= actorMaxLevel)
			{
				return StoryActorType.MaxLevelReached;
			}
			int num = CalculateMaxLevelByFragments(storyActorId);
			if (actorLevel >= num)
			{
				return StoryActorType.StarLevelTooLow;
			}
			return StoryActorType.Upgradable;
		}

		public override bool IsValid()
		{
			return true;
		}
	}
}
