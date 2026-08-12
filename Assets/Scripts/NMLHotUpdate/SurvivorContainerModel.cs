using System;
using System.Collections.Generic;
using System.Linq;
using BaseModel;
using Newtonsoft.Json;
using TWDModel;

public class SurvivorContainerModel : TWDModelObject
{
	public enum SurvivorType
	{
		Combat = 0,
		CombatOutpost = 1,
		Outpost = 2,
		CombatSurvival = 3,
		CombatGuildBattle = 4,
		GvGDefenders = 5
	}

	public const string AddSurvivorEvent = "addSurvivor";

	public const string CombatSurvivorChangedEvent = "combatSurvivorsChanged";

	public const string OutpostDefenseSurvivorChangedEvent = "outpostDefenseSurvivorsChanged";

	public const string SurvivorDiedEvent = "survivorDied";

	public const string SurvivorDemotedEvent = "survivorDemoted";

	private SurvivorModel nonMovingSurvivor;

	private SurvivorModel nonMovingOutpostSurvivor;

	[JsonIgnore]
	public int SurvivorSlotsCount
	{
		get
		{
			if (base.manager != null)
			{
				return base.manager.GameEconomyData.GetSurvivorSlotsData(SurvivorSlotsUpgradeLevel).AvailableSlotsCount + SurvivorGiftSlotsCount;
			}
			return 0;
		}
	}

	[JsonIgnore]
	public int MaximumBadgeCount
	{
		get
		{
			if (base.manager != null)
			{
				return base.manager.GameEconomyData.ConfigData.MaxBadgeInventorySize + SurvivorSlotsCount * 6;
			}
			return 0;
		}
	}

	public List<string> OutfitsOwned { get; set; }

	public List<string> HeroSkinsOwned { get; set; }

	public int SurvivorSlotsUpgradeLevel { get; set; }

	public int SurvivorGiftSlotsCount { get; set; }

	public ModelList<SurvivorModel> Survivors { get; private set; }

	public List<SurvivorModel> CombatSurvivors { get; private set; }

	public List<SurvivorModel> SavedCombatTeam { get; set; }

	public List<SurvivorModel> SavedSurvivalModeCombatTeam { get; set; }

	public List<SurvivorModel> OutpostDefendingSurvivors { get; private set; }

	public SurvivalCharacterContainerModel SurvivalCharacters { get; set; }

	public ModelList<DeadSurvivorModel> DeadSurvivors { get; private set; }

	public StoryTellerModel StoryTeller { get; private set; }

	public StoryTellerModel StoryTeller2 { get; private set; }

	[JsonIgnore]
	public bool HasInjuredSurvivorInCombatTeam
	{
		get
		{
			foreach (SurvivorModel combatSurvivor in CombatSurvivors)
			{
				if (combatSurvivor.InjuryType != InjuryType.None)
				{
					return true;
				}
			}
			return false;
		}
	}

	[JsonIgnore]
	public bool HasUpgradingSurvivorInCombatTeam
	{
		get
		{
			foreach (SurvivorModel combatSurvivor in CombatSurvivors)
			{
				if (combatSurvivor.IsUpgrading())
				{
					return true;
				}
			}
			return false;
		}
	}

	[JsonIgnore]
	public bool HasUpgradingSurvivor
	{
		get
		{
			int count = Survivors.Count;
			for (int i = 0; i < count; i++)
			{
				if (Survivors[i].IsUpgrading())
				{
					return true;
				}
			}
			return false;
		}
	}

	public string PendingCrossbowToGiveRewardsString { get; set; }

	public bool IsOutpostDefending(SurvivorModel survivorModel)
	{
		if (OutpostDefendingSurvivors != null)
		{
			return OutpostDefendingSurvivors.Contains(survivorModel);
		}
		return false;
	}

	public List<SurvivorModel> GetUpgradeableSurvivorsOfClass(SurvivorClass survivorClass)
	{
		List<SurvivorModel> list = new List<SurvivorModel>();
		int count = Survivors.Count;
		for (int i = 0; i < count; i++)
		{
			SurvivorModel survivorModel = Survivors[i];
			if (survivorModel.CanUpgrade && survivorModel.GetUpgradeCashier(instantUpgrade: false).CanAfford() && survivorModel.SurvivorClass == survivorClass)
			{
				list.Add(survivorModel);
			}
		}
		return list;
	}

	public List<SurvivorModel> GetUpgradeableSurvivors()
	{
		List<SurvivorModel> list = new List<SurvivorModel>();
		int count = Survivors.Count;
		for (int i = 0; i < count; i++)
		{
			SurvivorModel survivorModel = Survivors[i];
			if (survivorModel.CanUpgrade && survivorModel.GetUpgradeCashier(instantUpgrade: false).CanAfford())
			{
				list.Add(survivorModel);
			}
		}
		return list;
	}

	public List<SurvivorModel> GetPromotableSurvivors(bool herosOnly = false)
	{
		List<SurvivorModel> list = new List<SurvivorModel>();
		int count = Survivors.Count;
		for (int i = 0; i < count; i++)
		{
			SurvivorModel survivorModel = Survivors[i];
			if (!survivorModel.CanUpgradeSurvivorRarity() || !survivorModel.GetUpgradeTraitCashier().CanAfford())
			{
				continue;
			}
			if (herosOnly)
			{
				if (survivorModel.IsHero)
				{
					list.Add(survivorModel);
				}
			}
			else
			{
				list.Add(survivorModel);
			}
		}
		return list;
	}

	public List<SurvivorModel> GetUpgradeableCombatSurvivors()
	{
		List<SurvivorModel> list = new List<SurvivorModel>();
		int count = CombatSurvivors.Count;
		for (int i = 0; i < count; i++)
		{
			SurvivorModel survivorModel = CombatSurvivors[i];
			if (survivorModel.CanUpgrade && survivorModel.GetUpgradeCashier(instantUpgrade: false).CanAfford())
			{
				list.Add(survivorModel);
			}
		}
		return list;
	}

	public SurvivorModel GetOutpostDefendingSurvivor(int index)
	{
		if (index >= 0 && index < OutpostDefendingSurvivors.Count)
		{
			return OutpostDefendingSurvivors[index];
		}
		return null;
	}

	public bool CanPurchaseMoreSlots()
	{
		if (base.manager != null)
		{
			return base.manager.GameEconomyData.GetSurvivorSlotsData(SurvivorSlotsUpgradeLevel + 1) != null;
		}
		return false;
	}

	public Cashier GetPurchaseNextSlotsLevelCashier()
	{
		if (CanPurchaseMoreSlots())
		{
			SurvivorSlotsData survivorSlotsData = base.manager.GameEconomyData.GetSurvivorSlotsData(SurvivorSlotsUpgradeLevel + 1);
			Cashier cashier = new Cashier(base.manager);
			CashierItem cashierItem = new CashierItem(PurchaseType.AdditionalSurvivorSlots);
			cashierItem.SetCost(CurrencyType.Diamonds, survivorSlotsData.GemsCost);
			cashier.AddItem(cashierItem);
			return cashier;
		}
		return null;
	}

	public Cashier GetHeroUnlockCashier(CurrencyType type)
	{
		Cashier cashier = new Cashier(base.manager);
		ActorDefinition actorDefinition = base.gameEconomyData.GetActorDefinition(SurvivorToken.GetHeroId(type));
		if (actorDefinition != null)
		{
			CashierItem cashierItem = new CashierItem(PurchaseType.UnlockHero);
			cashierItem.SetCost(type, actorDefinition.TokensToUnlock);
			cashier.AddItem(cashierItem);
		}
		return cashier;
	}

	public SurvivorContainerModel()
	{
		Survivors = new ModelList<SurvivorModel>();
		CombatSurvivors = new List<SurvivorModel>();
		OutpostDefendingSurvivors = new List<SurvivorModel>();
		DeadSurvivors = new ModelList<DeadSurvivorModel>();
	}

	public override void Initialize()
	{
		Survivors.SetManager(base.Manager);
		Survivors.Initialize();
		SurvivalCharacters = new SurvivalCharacterContainerModel();
		SurvivalCharacters.SetManager(base.Manager);
		SurvivalCharacters.Initialize();
		DeadSurvivors.SetManager(base.Manager);
		DeadSurvivors.Initialize();
		StoryTeller = new StoryTellerModel(1, 1, 0);
		StoryTeller.ActorDefinitionID = "StoryTeller_1";
		StoryTeller.SetManager(base.Manager);
		StoryTeller.Initialize();
		SurvivorSlotsUpgradeLevel = 1;
		OutfitsOwned = new List<string>();
		HeroSkinsOwned = new List<string>();
		SavedCombatTeam = new List<SurvivorModel>();
		SavedSurvivalModeCombatTeam = new List<SurvivorModel>();
	}

	public override void Start()
	{
		base.Start();
		List<ActorModel> list = new List<ActorModel>();
		for (int i = 0; i < ((CombatSurvivors != null) ? CombatSurvivors.Count : 0); i++)
		{
			list.Add(CombatSurvivors[i]);
		}
		for (int j = 0; j < ((Survivors != null) ? Survivors.Count : 0); j++)
		{
			SurvivorModel survivorModel = Survivors[j];
			if (survivorModel != null)
			{
				bool flag = list.Contains(survivorModel);
				survivorModel.EvaluateBadges(new BadgeContext(survivorModel, flag ? list : null));
				survivorModel.ConfigureBaseAttributes();
			}
		}
		for (int k = 0; k < ((Survivors != null) ? Survivors.Count : 0); k++)
		{
			SurvivorModel survivor = Survivors[k];
			if (survivor == null)
			{
				continue;
			}
			foreach (HeroSkinDefinition item in base.manager.GameEconomyData.HeroSkinDefinitions.Where((HeroSkinDefinition x) => x.HeroID == survivor.ActorDefinitionID && x.AvailableOnHeroPurchased))
			{
				AddHeroSkin(item.ID);
			}
		}
	}

	public bool HasOutfit(string outfitDefinitionID)
	{
		if (OutfitsOwned != null)
		{
			for (int i = 0; i < OutfitsOwned.Count; i++)
			{
				if (OutfitsOwned[i] == outfitDefinitionID)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void AddOutfit(string outfitDefinitionID)
	{
		if (OutfitsOwned == null)
		{
			OutfitsOwned = new List<string>();
		}
		OutfitsOwned.Add(outfitDefinitionID);
	}

	public bool HasHeroSkin(string skinDefinitionID)
	{
		if (HeroSkinsOwned != null)
		{
			for (int i = 0; i < HeroSkinsOwned.Count; i++)
			{
				if (HeroSkinsOwned[i] == skinDefinitionID)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void AddHeroSkin(string skinDefinitionID)
	{
		if (HeroSkinsOwned == null)
		{
			HeroSkinsOwned = new List<string>();
		}
		if (!HeroSkinsOwned.Contains(skinDefinitionID))
		{
			HeroSkinsOwned.Add(skinDefinitionID);
		}
	}

	public override bool IsValid()
	{
		return true;
	}

	public bool CanAddSurvivor()
	{
		return Survivors.Count < SurvivorSlotsCount;
	}

	public bool ContainsSurvivor(SurvivorModel survivor)
	{
		return Survivors.Contains(survivor);
	}

	public bool HasHero(string heroId)
	{
		for (int i = 0; i < ((Survivors != null) ? Survivors.Count : 0); i++)
		{
			if (Survivors[i].ActorDefinitionID.Equals(heroId))
			{
				return true;
			}
		}
		return false;
	}

	public SurvivorModel GetHeroById(string heroId)
	{
		if (Survivors != null)
		{
			for (int i = 0; i < Survivors.Count; i++)
			{
				if (Survivors[i].IsHero && Survivors[i].ActorDefinitionID.Equals(heroId))
				{
					return Survivors[i];
				}
			}
		}
		return null;
	}

	public SurvivorModel GetSurvivorById(string heroId)
	{
		if (Survivors != null)
		{
			for (int i = 0; i < Survivors.Count; i++)
			{
				if (Survivors[i].ActorDefinitionID.Equals(heroId))
				{
					return Survivors[i];
				}
			}
		}
		return null;
	}

	public void UpdateSurvivalSurvivorsList()
	{
		SurvivalCharacters.RemoveAll();
		for (int i = 0; i < Survivors.Count; i++)
		{
			SurvivalCharacters.OnNewSurvivorReceived(Survivors[i]);
		}
	}

	public bool AddSurvivor(SurvivorModel survivor)
	{
		if (CanAddSurvivor())
		{
			if (survivor.MissionFailCondition != MissionFailCondition.None)
			{
				survivor.MissionFailCondition = MissionFailCondition.None;
			}
			Survivors.Add(survivor);
			SurvivalCharacters.OnNewSurvivorReceived(survivor);
			NotifyChange("addSurvivor", survivor);
			HeroSkinDefinition[] array = base.gameEconomyData.HeroSkinDefinitions.Where((HeroSkinDefinition x) => x.HeroID == survivor.ActorDefinitionID && x.AvailableOnHeroPurchased).ToArray();
			if (array != null)
			{
				for (int num = 0; num < array.Count(); num++)
				{
					AddHeroSkin(array[num].ID);
				}
			}
			if (base.manager?.Player?.SurvivalManualManager != null)
			{
				base.manager.Player.SurvivalManualManager.ActivatedSurvivalManualTraits();
			}
			return true;
		}
		return false;
	}

	public void RemoveSurvivor(SurvivorModel survivor)
	{
		survivor.UnequipAll();
		if (CombatSurvivors.Contains(survivor))
		{
			CombatSurvivors.Remove(survivor);
		}
		if (OutpostDefendingSurvivors.Contains(survivor))
		{
			OutpostDefendingSurvivors.Remove(survivor);
		}
		if (SavedCombatTeam.Contains(survivor))
		{
			SavedCombatTeam.Remove(survivor);
		}
		if (SavedSurvivalModeCombatTeam.Contains(survivor))
		{
			SavedSurvivalModeCombatTeam.Remove(survivor);
		}
		SurvivalCharacters.OnSurvivorRemoved(survivor);
		Survivors.Remove(survivor);
	}

	public TWDModelResult BuyNextSetOfSurvivorSlots()
	{
		if (CanPurchaseMoreSlots())
		{
			Cashier purchaseNextSlotsLevelCashier = GetPurchaseNextSlotsLevelCashier();
			purchaseNextSlotsLevelCashier.UsedReason = "BuySlots";
			TWDModelResult num = purchaseNextSlotsLevelCashier.Pay(this);
			if (num == TWDModelResult.OK)
			{
				SurvivorSlotsUpgradeLevel++;
			}
			return num;
		}
		return TWDModelResult.AlreadyMaxLevel;
	}

	public SurvivorModel GetEquipmentHolder(EquipmentItemModel equipment)
	{
		int count = Survivors.Count;
		for (int i = 0; i < count; i++)
		{
			SurvivorModel survivorModel = Survivors[i];
			if (survivorModel.IsEquipped(equipment))
			{
				return survivorModel;
			}
		}
		return null;
	}

	public SurvivorModel SetupInitialSurvivor(string survivorDefinition, ModelRandom random)
	{
		ActorDefinition actorDefinition = base.manager.GameEconomyData.GetActorDefinition(survivorDefinition);
		int rarityLevel = actorDefinition.RarityLevel;
		ActorGender gender = actorDefinition.Gender;
		SurvivorModel survivorModel = new SurvivorModel(1, rarityLevel);
		survivorModel.SetManager(base.manager);
		survivorModel.ActorDefinitionID = survivorDefinition;
		survivorModel.Age = ActorAge.Adult;
		survivorModel.TraitRandom = new ModelRandom(random.State);
		survivorModel.Gender = gender;
		survivorModel.CharacterPrefab = actorDefinition.VisualAsset;
		survivorModel.OutfitDefinitionID = actorDefinition.OutfitDefinitionID;
		if (string.IsNullOrEmpty(actorDefinition.Name))
		{
			GenerateName(survivorModel, random);
		}
		else
		{
			survivorModel.SurvivorName = actorDefinition.Name;
		}
		survivorModel.Initialize();
		survivorModel.InitUpgradeTraits();
		return survivorModel;
	}

	public TWDModelResult DemoteSurvivor(SurvivorModel survivorModel)
	{
		if (!Survivors.Contains(survivorModel))
		{
			return TWDModelResult.Error;
		}
		if (OutpostDefendingSurvivors != null && OutpostDefendingSurvivors.Contains(survivorModel))
		{
			return TWDModelResult.Error;
		}
		List<SurvivorMockData> gvGDefenders = base.manager.Player.GvGDefenders;
		if (base.manager.Player.IsGuildMember && gvGDefenders != null && gvGDefenders.Any((SurvivorMockData x) => x.AnalyticsId == survivorModel.IdForAnalytics))
		{
			return TWDModelResult.Error;
		}
		if (base.manager.Player.Camp.GetBuilding("TrainingGround") is TrainingGroundBuildingModel { UpgradingSurvivor: not null } trainingGroundBuildingModel && trainingGroundBuildingModel.UpgradingSurvivor == survivorModel)
		{
			trainingGroundBuildingModel.CancelUpgrade();
		}
		if (base.manager.Player.Camp.GetBuilding("MedicTent") is MedicTentModel medicTentModel && medicTentModel.TimedQueueModel.Exists(survivorModel))
		{
			TimedQueueItemModel queueItemFromItem = medicTentModel.TimedQueueModel.GetQueueItemFromItem(survivorModel);
			if (queueItemFromItem != null)
			{
				medicTentModel.TimedQueueModel.RemoveItemFromList(queueItemFromItem);
			}
		}
		Dictionary<CurrencyType, OverflowableAmount> dictionary = survivorModel.Demote();
		if (dictionary != null && dictionary.Count > 0)
		{
			base.manager.Metrics.AddFind().AddResources(dictionary).AddSurvivor(survivorModel)
				.AddScrap()
				.Send();
		}
		for (int num = 0; num < ((survivorModel.BadgeContainer != null) ? survivorModel.BadgeContainer.Badges.Count : 0); num++)
		{
			BadgeModel badge = survivorModel.BadgeContainer.Badges[num];
			base.manager.Player.Equipment.AddBadge(badge);
			base.manager.Metrics.AddUnequip().AddBadge(badge).AddSurvivor(survivorModel)
				.AddScrap()
				.Send();
		}
		survivorModel.BadgeContainer.Badges.Clear();
		RemoveSurvivor(survivorModel);
		NotifyChange("survivorDemoted", survivorModel);
		return TWDModelResult.OK;
	}

	public bool IsDead(SurvivorModel survivorModel)
	{
		int count = DeadSurvivors.Count;
		for (int i = 0; i < count; i++)
		{
			if (DeadSurvivors[i].SurvivorModel == survivorModel)
			{
				return true;
			}
		}
		return false;
	}

	public void SurvivorDied(SurvivorModel survivorModel)
	{
		DeadSurvivorModel deadSurvivorModel = new DeadSurvivorModel();
		deadSurvivorModel.SetManager(base.manager);
		deadSurvivorModel.Initialize();
		deadSurvivorModel.Start();
		deadSurvivorModel.SetDeadSurvivor(survivorModel);
		DeadSurvivors.Add(deadSurvivorModel);
		RemoveSurvivorFromCombat(survivorModel);
		RemoveSurvivor(survivorModel);
		NotifyChange("survivorDied", survivorModel);
	}

	public bool StoreCombatTeam(SurvivorType type)
	{
		switch (type)
		{
		case SurvivorType.Combat:
		case SurvivorType.CombatGuildBattle:
			SavedCombatTeam.Clear();
			SavedCombatTeam.AddRange(CombatSurvivors);
			break;
		case SurvivorType.CombatSurvival:
			SavedSurvivalModeCombatTeam.Clear();
			SavedSurvivalModeCombatTeam.AddRange(CombatSurvivors);
			break;
		default:
			return false;
		}
		return true;
	}

	public List<SurvivorModel> GetStoredCombatTeam(SurvivorType type)
	{
		switch (type)
		{
		case SurvivorType.Combat:
		case SurvivorType.CombatGuildBattle:
			return SavedCombatTeam;
		case SurvivorType.CombatSurvival:
			return SavedSurvivalModeCombatTeam;
		default:
			return null;
		}
	}

	public bool CanRestoreCombatTeam(SurvivorType type)
	{
		List<SurvivorModel> storedCombatTeam = GetStoredCombatTeam(type);
		if (storedCombatTeam == null || storedCombatTeam.Count == 0)
		{
			return false;
		}
		if (storedCombatTeam.SequenceEqual(CombatSurvivors))
		{
			return false;
		}
		return true;
	}

	public bool RestoreCombatTeam(SurvivorType type)
	{
		List<SurvivorModel> storedCombatTeam = GetStoredCombatTeam(type);
		if (storedCombatTeam == null || storedCombatTeam.Count == 0)
		{
			return false;
		}
		for (int num = CombatSurvivors.Count - 1; num >= 0; num--)
		{
			RemoveSurvivorFromCombat(CombatSurvivors[num]);
		}
		for (int i = 0; i < storedCombatTeam.Count; i++)
		{
			AddSurvivorToCombat(storedCombatTeam[i], null, type == SurvivorType.CombatSurvival);
		}
		return true;
	}

	public TWDModelResult AddSurvivorToCombat(SurvivorModel newSurvivor, SurvivorModel oldSurvivor = null, bool allowOutpostDefenders = false)
	{
		if (oldSurvivor == null && CombatSurvivors.Count >= 3)
		{
			return TWDModelResult.AlreadyMaxAmount;
		}
		if (CombatSurvivors.Contains(newSurvivor))
		{
			return TWDModelResult.SurvivorAlreadyInCollection;
		}
		if (newSurvivor == null)
		{
			return TWDModelResult.Error;
		}
		bool disableOutpostHeroLimits = base.manager.GameEconomyData.ConfigData.DisableOutpostHeroLimits;
		if (!Survivors.Contains(newSurvivor) || (IsOutpostDefending(newSurvivor) && !allowOutpostDefenders && !disableOutpostHeroLimits))
		{
			return TWDModelResult.Error;
		}
		int num = 0;
		if (oldSurvivor == null)
		{
			num = CombatSurvivors.Count;
			CombatSurvivors.Add(newSurvivor);
		}
		else
		{
			num = CombatSurvivors.FindIndex((SurvivorModel x) => x == oldSurvivor);
			if (num == -1)
			{
				return TWDModelResult.SurvivorNotInCollection;
			}
			CombatSurvivors[num] = newSurvivor;
		}
		if (oldSurvivor != null && oldSurvivor.IsHero)
		{
			oldSurvivor.UnregisterLeaderTraits();
		}
		if (num == 0 && newSurvivor.IsHero)
		{
			newSurvivor.RegisterLeaderTraits();
		}
		EvaluateCombatTeamBadges();
		if (oldSurvivor != null)
		{
			oldSurvivor.EvaluateBadges(new BadgeContext(oldSurvivor, null));
			oldSurvivor.ConfigureBaseAttributes();
		}
		NotifyChange("combatSurvivorsChanged");
		return TWDModelResult.OK;
	}

	public TWDModelResult AddSurvivorToCombatTeamSlot(SurvivorModel addSurvivor, int teamSlotIndex, int fromSlotIndex, bool allowOutpostDefenders = false)
	{
		if (CombatSurvivors.Count >= 3)
		{
			return TWDModelResult.AlreadyMaxAmount;
		}
		if (CombatSurvivors.Contains(addSurvivor))
		{
			return TWDModelResult.SurvivorAlreadyInCollection;
		}
		if (addSurvivor == null)
		{
			return TWDModelResult.Error;
		}
		bool disableOutpostHeroLimits = base.manager.GameEconomyData.ConfigData.DisableOutpostHeroLimits;
		if (!Survivors.Contains(addSurvivor) || (IsOutpostDefending(addSurvivor) && !allowOutpostDefenders && !disableOutpostHeroLimits))
		{
			return TWDModelResult.Error;
		}
		if (teamSlotIndex > 2)
		{
			return TWDModelResult.Error;
		}
		if (CombatSurvivors.Count == 1 && CombatSurvivors[0] != null)
		{
			nonMovingSurvivor = CombatSurvivors[0];
		}
		if (CombatSurvivors.Count <= teamSlotIndex)
		{
			CombatSurvivors.Add(addSurvivor);
		}
		else
		{
			CombatSurvivors.Insert(teamSlotIndex, addSurvivor);
		}
		if (CombatSurvivors.Count == 3 && nonMovingSurvivor != null && (CombatSurvivors.IndexOf(nonMovingSurvivor) == teamSlotIndex || CombatSurvivors.IndexOf(nonMovingSurvivor) == fromSlotIndex))
		{
			CombatSurvivors = ReorganizeTeam(CombatSurvivors, nonMovingSurvivor, teamSlotIndex, fromSlotIndex);
			nonMovingSurvivor = null;
		}
		if (teamSlotIndex == 0 && addSurvivor.IsHero)
		{
			addSurvivor.RegisterLeaderTraits();
		}
		EvaluateCombatTeamBadges();
		NotifyChange("combatSurvivorsChanged");
		return TWDModelResult.OK;
	}

	private void EvaluateCombatTeamBadges()
	{
		List<ActorModel> list = new List<ActorModel>();
		for (int i = 0; i < CombatSurvivors.Count; i++)
		{
			list.Add(CombatSurvivors[i]);
		}
		for (int j = 0; j < CombatSurvivors.Count; j++)
		{
			SurvivorModel survivorModel = CombatSurvivors[j];
			survivorModel.EvaluateBadges(new BadgeContext(survivorModel, list));
			survivorModel.ConfigureBaseAttributes();
		}
	}

	public TWDModelResult RemoveSurvivorFromCombat(SurvivorModel survivor)
	{
		if (!CombatSurvivors.Contains(survivor))
		{
			return TWDModelResult.Error;
		}
		survivor.UnregisterLeaderTraits();
		survivor.EvaluateBadges(new BadgeContext(survivor, null));
		survivor.ConfigureBaseAttributes();
		CombatSurvivors.Remove(survivor);
		List<ActorModel> list = new List<ActorModel>();
		for (int i = 0; i < CombatSurvivors.Count; i++)
		{
			list.Add(CombatSurvivors[i]);
		}
		for (int j = 0; j < CombatSurvivors.Count; j++)
		{
			SurvivorModel survivorModel = CombatSurvivors[j];
			survivorModel.EvaluateBadges(new BadgeContext(survivor, list));
			survivorModel.ConfigureBaseAttributes();
		}
		NotifyChange("combatSurvivorsChanged");
		return TWDModelResult.OK;
	}

	public TWDModelResult AddSurvivorToOutpostDefense(SurvivorModel newSurvivor, SurvivorModel oldSurvivor = null)
	{
		if (oldSurvivor == null && OutpostDefendingSurvivors.Count >= 3)
		{
			return TWDModelResult.AlreadyMaxAmount;
		}
		if (OutpostDefendingSurvivors.Contains(newSurvivor))
		{
			return TWDModelResult.SurvivorAlreadyInCollection;
		}
		if (oldSurvivor == null)
		{
			OutpostDefendingSurvivors.Add(newSurvivor);
		}
		else
		{
			int num = OutpostDefendingSurvivors.FindIndex((SurvivorModel x) => x == oldSurvivor);
			if (num == -1)
			{
				return TWDModelResult.SurvivorNotInCollection;
			}
			OutpostDefendingSurvivors[num] = newSurvivor;
		}
		List<ActorModel> list = new List<ActorModel>();
		for (int num2 = 0; num2 < OutpostDefendingSurvivors.Count; num2++)
		{
			list.Add(OutpostDefendingSurvivors[num2]);
		}
		for (int num3 = 0; num3 < OutpostDefendingSurvivors.Count; num3++)
		{
			SurvivorModel survivorModel = OutpostDefendingSurvivors[num3];
			survivorModel.EvaluateBadges(new BadgeContext(survivorModel, list));
			survivorModel.ConfigureBaseAttributes();
		}
		if (oldSurvivor != null)
		{
			oldSurvivor.EvaluateBadges(new BadgeContext(oldSurvivor, null));
			oldSurvivor.ConfigureBaseAttributes();
		}
		if (CombatSurvivors.Contains(newSurvivor))
		{
			RemoveSurvivorFromCombat(newSurvivor);
		}
		NotifyChange("outpostDefenseSurvivorsChanged");
		return TWDModelResult.OK;
	}

	public TWDModelResult AddSurvivorToOutpostDefenseSlot(SurvivorModel addSurvivor, int teamSlotIndex, int fromSlotIndex)
	{
		if (OutpostDefendingSurvivors.Count >= 3)
		{
			return TWDModelResult.AlreadyMaxAmount;
		}
		if (OutpostDefendingSurvivors.Contains(addSurvivor))
		{
			return TWDModelResult.SurvivorAlreadyInCollection;
		}
		if (teamSlotIndex > 2)
		{
			return TWDModelResult.Error;
		}
		if (OutpostDefendingSurvivors.Count == 1 && OutpostDefendingSurvivors[0] != null)
		{
			nonMovingOutpostSurvivor = OutpostDefendingSurvivors[0];
		}
		if (OutpostDefendingSurvivors.Count <= teamSlotIndex)
		{
			OutpostDefendingSurvivors.Add(addSurvivor);
		}
		else
		{
			OutpostDefendingSurvivors.Insert(teamSlotIndex, addSurvivor);
		}
		if (OutpostDefendingSurvivors.Count == 3 && nonMovingOutpostSurvivor != null && (OutpostDefendingSurvivors.IndexOf(nonMovingOutpostSurvivor) == teamSlotIndex || OutpostDefendingSurvivors.IndexOf(nonMovingOutpostSurvivor) == fromSlotIndex))
		{
			OutpostDefendingSurvivors = ReorganizeTeam(OutpostDefendingSurvivors, nonMovingOutpostSurvivor, teamSlotIndex, fromSlotIndex);
			nonMovingOutpostSurvivor = null;
		}
		List<ActorModel> list = new List<ActorModel>();
		for (int i = 0; i < OutpostDefendingSurvivors.Count; i++)
		{
			list.Add(OutpostDefendingSurvivors[i]);
		}
		for (int j = 0; j < OutpostDefendingSurvivors.Count; j++)
		{
			SurvivorModel survivorModel = OutpostDefendingSurvivors[j];
			survivorModel.EvaluateBadges(new BadgeContext(survivorModel, list));
			survivorModel.ConfigureBaseAttributes();
		}
		if (CombatSurvivors.Contains(addSurvivor))
		{
			RemoveSurvivorFromCombat(addSurvivor);
		}
		NotifyChange("outpostDefenseSurvivorsChanged");
		return TWDModelResult.OK;
	}

	private List<SurvivorModel> ReorganizeTeam(List<SurvivorModel> teamToReorganize, SurvivorModel survivorThatRemainedInTeam, int swappedToSlotIndex, int swappedFromSlotIndex)
	{
		List<SurvivorModel> list = new List<SurvivorModel>(teamToReorganize);
		int num = -1;
		for (int i = 0; i <= 2; i++)
		{
			if (i != swappedToSlotIndex && i != swappedFromSlotIndex)
			{
				num = i;
				break;
			}
		}
		if (survivorThatRemainedInTeam != null && num != -1)
		{
			list.Remove(survivorThatRemainedInTeam);
			if (num > list.Count)
			{
				list.Add(survivorThatRemainedInTeam);
			}
			else
			{
				list.Insert(num, survivorThatRemainedInTeam);
			}
		}
		return list;
	}

	public TWDModelResult RemoveSurvivorFromOutpostDefense(SurvivorModel survivor)
	{
		if (!OutpostDefendingSurvivors.Contains(survivor))
		{
			return TWDModelResult.Error;
		}
		OutpostDefendingSurvivors.Remove(survivor);
		survivor.EvaluateBadges(new BadgeContext(survivor, null));
		survivor.ConfigureBaseAttributes();
		List<ActorModel> list = new List<ActorModel>();
		for (int i = 0; i < OutpostDefendingSurvivors.Count; i++)
		{
			list.Add(OutpostDefendingSurvivors[i]);
		}
		for (int j = 0; j < OutpostDefendingSurvivors.Count; j++)
		{
			SurvivorModel survivorModel = OutpostDefendingSurvivors[j];
			survivorModel.EvaluateBadges(new BadgeContext(survivor, list));
			survivorModel.ConfigureBaseAttributes();
		}
		NotifyChange("outpostDefenseSurvivorsChanged");
		return TWDModelResult.OK;
	}

	public List<SurvivorModel> GetSurvivorsOfClass(SurvivorClass survivorClass)
	{
		List<SurvivorModel> list = new List<SurvivorModel>();
		foreach (SurvivorModel survivor in Survivors)
		{
			if (survivor.SurvivorClass == survivorClass)
			{
				list.Add(survivor);
			}
		}
		return list;
	}

	public string GetRandomSurvivorName(ActorGender gender, ModelRandom random)
	{
		_ = base.manager.GameEconomyData;
		string[] array = ((base.manager.Player.Language == null || !(base.manager.Player.Language.ToLower() == "ru")) ? ((gender == ActorGender.Female) ? SurvivorNames.FemaleNames : SurvivorNames.MaleNames) : ((gender == ActorGender.Female) ? SurvivorNames.FemaleNamesRussian : SurvivorNames.MaleNamesRussian));
		return random.GetRandomElement(array);
	}

	private void GenerateName(SurvivorModel survivor, ModelRandom random)
	{
		int num = 0;
		bool flag;
		do
		{
			survivor.SurvivorName = GetRandomSurvivorName(survivor.Gender, random);
			num++;
			flag = false;
			for (int i = 0; i < Survivors.Count; i++)
			{
				if (Survivors[i].SurvivorName == survivor.SurvivorName)
				{
					flag = true;
				}
			}
		}
		while (num < 10 && flag);
	}

	public SurvivorModel CreateSurvivorFromSurvivorMockData(SurvivorMockData survivorModel, int survivorLevel, bool preview = false)
	{
		ActorDefinition actorDefinition = base.manager.GameEconomyData.GetActorDefinition(survivorModel.ActorDefinitionId);
		SurvivorModel survivorModel2 = CreateSurvivorAndInitializeTraitsAndItemsFromMockData(survivorModel, actorDefinition, survivorLevel, preview);
		survivorModel2.CharacterPrefab = survivorModel.CharacterPrefabName;
		survivorModel2.SurvivorName = survivorModel.Name;
		return survivorModel2;
	}

	public SurvivorModel CreateSurvivorFromDefinition(string actorDefinitionId, int minLevel, int maxLevel, int rarityLevel, int equipmentLevel, int equipmentRarityLevel, ModelRandom random, string weaponId = null, string armorId = null, bool isMock = false)
	{
		GameEconomyData obj = base.manager.GameEconomyData;
		ActorDefinition actorDefinition = obj.GetActorDefinition(actorDefinitionId);
		int survivorsMaxUpgradeLevel = obj.GetSurvivorsMaxUpgradeLevel((SurvivorClass)Enum.Parse(typeof(SurvivorClass), actorDefinition.Class));
		if (string.IsNullOrEmpty(weaponId) || string.IsNullOrEmpty(armorId))
		{
			return CreateSurvivorAndInitializeTraitsAndRandomizeItems(minLevel, maxLevel, null, equipmentLevel, equipmentRarityLevel, actorDefinition, rarityLevel, survivorsMaxUpgradeLevel, random, isMock);
		}
		return CreateSurvivorAndInitializeTraitsAndItems(minLevel, maxLevel, null, equipmentLevel, actorDefinition, rarityLevel, survivorsMaxUpgradeLevel, random, weaponId, armorId, isMock);
	}

	public SurvivorModel CreateHero(string actorDefinitionId)
	{
		GameEconomyData obj = base.manager.GameEconomyData;
		ActorDefinition actorDefinition = obj.GetActorDefinition(actorDefinitionId);
		ModelRandom dedicatedRandom = base.manager.Player.LootManager.GetDedicatedRandom(actorDefinition.ID);
		int survivorsMaxUpgradeLevel = obj.GetSurvivorsMaxUpgradeLevel((SurvivorClass)Enum.Parse(typeof(SurvivorClass), actorDefinition.Class));
		int highestLevelSurvivor = GetHighestLevelSurvivor();
		int rarityLevel = actorDefinition.RarityLevel;
		int num = highestLevelSurvivor + actorDefinition.InitialLevelOffset;
		SurvivorModel survivorModel = CreateSurvivorAndInitializeTraits(num, num, null, actorDefinition, rarityLevel, survivorsMaxUpgradeLevel, dedicatedRandom);
		int startingLevel = Math.Min(num, survivorModel.Level);
		EquipmentModel equipment = base.manager.Player.Equipment;
		EquipmentItemModel equipmentItemModel = equipment.GenerateAndInitializeEquipmentFromDefinition(actorDefinition.InitialEquipmentsData[0].ID, actorDefinition.InitialEquipmentsData[0].RarityLevel, startingLevel, dedicatedRandom);
		base.manager.Player.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Survivor);
		survivorModel.Equip(equipmentItemModel);
		EquipmentItemModel equipmentItemModel2 = equipment.GenerateAndInitializeEquipmentFromDefinition(actorDefinition.InitialEquipmentsData[1].ID, actorDefinition.InitialEquipmentsData[1].RarityLevel, startingLevel, dedicatedRandom);
		base.manager.Player.Equipment.AddEquipment(equipmentItemModel2, EquipmentSource.Survivor);
		survivorModel.Equip(equipmentItemModel2);
		survivorModel.Start();
		return survivorModel;
	}

	public TWDModelResult UnlockHero(CurrencyType type)
	{
		ActorDefinition actorDefinition = base.manager.GameEconomyData.GetActorDefinition(SurvivorToken.GetHeroId(type));
		if (actorDefinition != null)
		{
			if (HasHero(actorDefinition.ID))
			{
				return TWDModelResult.SurvivorAlreadyInCollection;
			}
			if (!actorDefinition.IsAvailableToUnlock(base.manager.Player.UtcTimeStamp))
			{
				return TWDModelResult.SurvivorNotAvailableForUnlocking;
			}
			if (!(OfflineManager.IsLoadDataManager && OfflineManager.IsUnlockAll))
			{
				TWDModelResult tWDModelResult = GetHeroUnlockCashier(type).Pay(actorDefinition);
				if (tWDModelResult != TWDModelResult.OK)
				{
					return tWDModelResult;
				}
			}

			SurvivorGiftSlotsCount++;
			SurvivorModel survivorModel = CreateHero(actorDefinition.ID);
			if (AddSurvivor(survivorModel))
			{
				base.manager.Metrics.AddFind().AddSurvivor(survivorModel).AddHeroUnlock()
					.Send();
				string heroId = SurvivorToken.GetHeroId(CurrencyType.DarylToken);
				if (survivorModel != null && survivorModel.ActorDefinitionID.Equals(heroId) && base.manager.Player.Blackboard.IsToggleOn("Toggle.PendingCrossbowToBeGiven") && !string.IsNullOrEmpty(PendingCrossbowToGiveRewardsString))
				{
					Rewards rewards = new Rewards(PendingCrossbowToGiveRewardsString);
					if (rewards != null && rewards.RewardsList != null && rewards.RewardsList.Count > 0 && rewards.RewardsList[0] is RewardEquipment rewardEquipment)
					{
						rewardEquipment.StartingLevel = survivorModel.Level;
						if (rewardEquipment.Give(base.manager, new object[1] { base.manager.Player.PlayerRandom }) is EquipmentItemModel equipmentItemModel)
						{
							if (survivorModel.CanEquip(equipmentItemModel) && equipmentItemModel.Owner == null && equipmentItemModel.CanBeManipulated())
							{
								if (survivorModel.Equip(equipmentItemModel) != TWDModelResult.OK)
								{
									base.manager.Player.Equipment.AddEquipment(equipmentItemModel);
								}
							}
							else
							{
								base.manager.Player.Equipment.AddEquipment(equipmentItemModel);
							}
							PendingCrossbowToGiveRewardsString = null;
							base.manager.Player.Blackboard.ClearToggle("Toggle.PendingCrossbowToBeGiven");
						}
					}
				}
				base.manager.Player.RFMGiftManager.TriggerRFMEvent(RFMEvent.characterUnlock);
				return TWDModelResult.OK;
			}
			return TWDModelResult.Error;
		}
		return TWDModelResult.Error;
	}

	public SurvivorModel CreateRandomSurvivor(int rarityPreference = 0, int minLevel = 1, int maxLevel = 1, int targetRarity = -1, SurvivorClass requestedClass = SurvivorClass.None, string characterAssetName = null, int equipmentLevel = 1, int equipmentRarity = 0, List<SurvivorClass> excludeClasses = null, bool includeGachaOnly = true, ModelRandom random = null, SurvivorClass forceSurvivorProbabilityClass = SurvivorClass.None, int forceSurvivorProbabilityPercentageIncrease = 0, bool allowLockedClasses = false, bool isMock = false)
	{
		if (random == null)
		{
			random = base.manager.Player.PlayerRandom;
		}
		List<SurvivorClass> list = new List<SurvivorClass>();
		int trainingGroundLevel = base.manager.Player.Camp.GetTrainingGroundLevel();
		SurvivorClass survivorClass;
		for (int i = 0; i < 6; i++)
		{
			survivorClass = (SurvivorClass)i;
			int minimumTrainingGroundLevelForClass = base.manager.Player.gameEconomyData.GetMinimumTrainingGroundLevelForClass(survivorClass);
			if (allowLockedClasses || (trainingGroundLevel >= minimumTrainingGroundLevelForClass && IsSurvivorClassUnlocked(survivorClass)))
			{
				list.Add(survivorClass);
			}
		}
		if (excludeClasses != null)
		{
			for (int j = 0; j < excludeClasses.Count; j++)
			{
				if (list.Contains(excludeClasses[j]) && list.Count > 1)
				{
					list.Remove(excludeClasses[j]);
				}
			}
		}
		if (requestedClass != SurvivorClass.None)
		{
			survivorClass = requestedClass;
		}
		else if (forceSurvivorProbabilityClass == SurvivorClass.None || !list.Contains(forceSurvivorProbabilityClass))
		{
			survivorClass = random.GetRandomElement(list.ToArray());
		}
		else
		{
			int num = 6;
			int count = list.Count;
			FixedPoint fixedPoint = 1L + 1L * ((FixedPoint)forceSurvivorProbabilityPercentageIncrease / (FixedPoint)100L) * count / num;
			FixedPoint[] array = new FixedPoint[count];
			for (int k = 0; k < list.Count; k++)
			{
				array[k] = 1L;
			}
			array[list.IndexOf(forceSurvivorProbabilityClass)] = fixedPoint;
			int index = random.WeightedRandom(array);
			survivorClass = list[index];
		}
		List<ActorDefinition> survivorActorDefinition = GetSurvivorActorDefinition(survivorClass, includeGachaOnly);
		if (survivorActorDefinition.Count == 0)
		{
			base.manager.Debug.LogError("Actor definitions not found for " + survivorClass);
			return null;
		}
		ActorDefinition actorDefinition = null;
		actorDefinition = ((survivorActorDefinition.Count != 1) ? random.GetRandomElement(survivorActorDefinition.ToArray()) : survivorActorDefinition[0]);
		GameEconomyData obj = base.manager.GameEconomyData;
		int actualRarity = obj.GetRarityLevel(random, rarityPreference);
		if (targetRarity >= 0)
		{
			actualRarity = targetRarity;
		}
		int survivorsMaxUpgradeLevel = obj.GetSurvivorsMaxUpgradeLevel(survivorClass);
		return CreateSurvivorAndInitializeTraitsAndRandomizeItems(minLevel, maxLevel, characterAssetName, equipmentLevel, equipmentRarity, actorDefinition, actualRarity, survivorsMaxUpgradeLevel, random, isMock);
	}

	private SurvivorModel CreateSurvivorAndInitializeTraitsAndRandomizeItems(int minLevel, int maxLevel, string characterAssetName, int equipmentLevel, int equipmentRarity, ActorDefinition actorDefinition, int actualRarity, int levelCap, ModelRandom random, bool isMock = false)
	{
		SurvivorModel survivorModel = CreateSurvivorAndInitializeTraits(minLevel, maxLevel, characterAssetName, actorDefinition, actualRarity, levelCap, random);
		int startingLevel = Math.Min(equipmentLevel, survivorModel.Level);
		EquipmentModel equipment = base.manager.Player.Equipment;
		EquipmentItemModel equipmentItemModel = equipment.GenerateRandomEquipment(EquipmentCategory.Weapon, startingLevel, equipmentRarity, useSpecialization: false, Faction.Survivor, survivorModel.SurvivorClass, null, random, !isMock);
		if (!isMock)
		{
			base.manager.Player.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Survivor);
		}
		survivorModel.Equip(equipmentItemModel);
		EquipmentItemModel equipmentItemModel2 = equipment.GenerateRandomEquipment(EquipmentCategory.Armor, startingLevel, equipmentRarity, useSpecialization: false, Faction.Survivor, survivorModel.SurvivorClass, null, random, !isMock);
		if (!isMock)
		{
			base.manager.Player.Equipment.AddEquipment(equipmentItemModel2, EquipmentSource.Survivor);
		}
		survivorModel.Equip(equipmentItemModel2);
		if (!isMock)
		{
			survivorModel.Start();
		}
		return survivorModel;
	}

	private SurvivorModel CreateSurvivorAndInitializeTraitsAndItems(int minLevel, int maxLevel, string characterAssetName, int equipmentLevel, ActorDefinition actorDefinition, int actualRarity, int levelCap, ModelRandom random, string weaponId, string armorId, bool isMock = false)
	{
		SurvivorModel survivorModel = CreateSurvivorAndInitializeTraits(minLevel, maxLevel, characterAssetName, actorDefinition, actualRarity, levelCap, random);
		int startingLevel = Math.Min(equipmentLevel, survivorModel.Level);
		EquipmentModel equipment = base.manager.Player.Equipment;
		EquipmentItemModel equipmentItemModel = equipment.GenerateAndInitializeEquipmentFromDefinition(weaponId, (actorDefinition.InitialEquipmentsData != null) ? actorDefinition.InitialEquipmentsData[0].RarityLevel : actorDefinition.InitialEquipmentRarityLevel, startingLevel, random, !isMock);
		if (!isMock)
		{
			base.manager.Player.Equipment.AddEquipment(equipmentItemModel, EquipmentSource.Survivor);
		}
		survivorModel.Equip(equipmentItemModel);
		EquipmentItemModel equipmentItemModel2 = equipment.GenerateAndInitializeEquipmentFromDefinition(armorId, (actorDefinition.InitialEquipmentsData != null) ? actorDefinition.InitialEquipmentsData[1].RarityLevel : actorDefinition.InitialEquipmentRarityLevel, startingLevel, random, !isMock);
		if (!isMock)
		{
			base.manager.Player.Equipment.AddEquipment(equipmentItemModel2, EquipmentSource.Survivor);
		}
		survivorModel.Equip(equipmentItemModel2);
		if (!isMock)
		{
			survivorModel.Start();
		}
		return survivorModel;
	}

	private SurvivorModel CreateSurvivorAndInitializeTraitsAndItemsFromMockData(SurvivorMockData survivorMockData, ActorDefinition actorDefinition, int survivorLevel, bool preview = false)
	{
		SurvivorModel survivorModel = CreateSurvivorAndInitializeTraitsFromMockData(survivorMockData, survivorMockData.CharacterPrefabName, actorDefinition, survivorLevel, preview);
		EquipmentModel equipment = base.manager.Player.Equipment;
		if (survivorMockData.MockWeapon != null)
		{
			EquipmentItemModel equipmentItem = equipment.GenerateAndInitializeEquipmentFromMockData(survivorMockData.MockWeapon, survivorLevel, preview);
			survivorModel.Equip(equipmentItem, forceEquip: true, preview);
		}
		if (survivorMockData.MockArmor != null)
		{
			EquipmentItemModel equipmentItem2 = equipment.GenerateAndInitializeEquipmentFromMockData(survivorMockData.MockArmor, survivorLevel, preview);
			survivorModel.Equip(equipmentItem2, forceEquip: true, preview);
		}
		if (!preview)
		{
			survivorModel.Start();
		}
		return survivorModel;
	}

	private SurvivorModel CreateSurvivorAndInitializeTraitsFromMockData(SurvivorMockData survivorMockData, string characterAssetName, ActorDefinition actorDefinition, int survivorLevel, bool preview)
	{
		SurvivorModel survivorModel = new SurvivorModel(survivorLevel, survivorMockData.RarityLevel);
		survivorModel.SurvivorRarityLevel = survivorMockData.RarityLevel;
		survivorModel.ActorDefinitionID = actorDefinition.ID;
		survivorModel.Level = survivorLevel;
		survivorModel.Age = survivorMockData.Age;
		survivorModel.Gender = survivorMockData.Gender;
		survivorModel.SetManager(base.manager);
		if (characterAssetName != null)
		{
			survivorModel.CharacterPrefab = characterAssetName;
		}
		survivorModel.Initialize();
		survivorModel.InitUpgradeTraitsFromMockData(survivorMockData.UpgradeTraits);
		List<TraitEntry> list = new List<TraitEntry>();
		for (int i = 0; i < survivorModel.UpgradeTraits.Count; i++)
		{
			UpgradeTraitsData upgradeTraitsData = survivorModel.UpgradeTraits[i];
			TraitEntry item = new TraitEntry(upgradeTraitsData.Identifier, 1L, upgradeTraitsData.ConstructionMultiplier);
			list.Add(item);
		}
		survivorModel.ReplaceTraits(list);
		return survivorModel;
	}

	private SurvivorModel CreateSurvivorAndInitializeTraits(int minLevel, int maxLevel, string characterAssetName, ActorDefinition actorDefinition, int actualRarity, int levelCap, ModelRandom random)
	{
		SurvivorModel survivorModel = new SurvivorModel(Math.Max(1, Math.Min(levelCap, random.GetRandomInRange(minLevel, maxLevel))), actualRarity);
		survivorModel.ActorDefinitionID = actorDefinition.ID;
		survivorModel.SetManager(base.manager);
		random.Next();
		survivorModel.TraitRandom = new ModelRandom(random.State);
		survivorModel.Age = random.GetRandomEnum<ActorAge>();
		if (characterAssetName != null)
		{
			survivorModel.Gender = SurvivorModel.GetAssetGender(characterAssetName);
		}
		else
		{
			if (random.Next() >= 0.5f)
			{
				survivorModel.Gender = ActorGender.Female;
			}
			else
			{
				survivorModel.Gender = ActorGender.Male;
			}
			if (actorDefinition.ID.ToLower().Contains("unique") || actorDefinition.ID.ToLower().Contains("hero"))
			{
				survivorModel.Gender = actorDefinition.Gender;
			}
		}
		if (!actorDefinition.ID.ToLower().Contains("unique") && !actorDefinition.ID.ToLower().Contains("hero"))
		{
			GenerateName(survivorModel, random);
		}
		else
		{
			survivorModel.SurvivorName = actorDefinition.Name;
		}
		if (characterAssetName != null)
		{
			survivorModel.CharacterPrefab = characterAssetName;
		}
		survivorModel.Initialize();
		survivorModel.InitUpgradeTraits();
		return survivorModel;
	}

	public Dictionary<string, string> GetSurvivorAnalyticsProperties(SurvivorModel survivor)
	{
		PlayerModel player = base.manager.Player;
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		int level = player.Camp.GetBuilding("Council").Level;
		int num = 0;
		BuildingModel building = player.Camp.GetBuilding("RadioTent");
		if (building != null)
		{
			num = building.Level;
		}
		int level2 = player.Level;
		dictionary.Add("council_level", level.ToString());
		dictionary.Add("radio_tent_level", num.ToString());
		dictionary.Add("player_level", level2.ToString());
		if (survivor.IsHero)
		{
			dictionary.Add("survivor_name", survivor.FullName);
		}
		dictionary.Add("survivor_type", survivor.IsHero ? "Hero" : "Regular");
		dictionary.Add("survivor_level", survivor.Level.ToString());
		dictionary.Add("survivor_rarity", ModelHelpers.GetRarityNameForAnalytics(survivor.SurvivorRarityLevel));
		dictionary.Add("survivor_class", survivor.SurvivorClass.ToString());
		dictionary.Add("survivor_starting_level", survivor.StartingLevel.ToString());
		dictionary.Add("survivor_starting_rarity", ModelHelpers.GetRarityNameForAnalytics(survivor.StartingRarityLevel));
		dictionary.Add("survivor_traits_count", Math.Max(0, survivor.UpgradeTraits.Count - 1).ToString());
		dictionary.Add("survivor_totals_levels", survivor.GetUpgradeTraitRaritySum().ToString());
		EquipmentItemModel weaponEquipment = survivor.GetWeaponEquipment();
		if (weaponEquipment != null)
		{
			dictionary.Add("weapon_level", weaponEquipment.Level.ToString());
			dictionary.Add("weapon_rarity", ModelHelpers.GetRarityNameForAnalytics(weaponEquipment.RarityLevel));
			dictionary.Add("weapon_category", weaponEquipment.Definition.Category.ToString());
			dictionary.Add("weapon_id", (weaponEquipment.IdForAnalytics != null) ? weaponEquipment.IdForAnalytics : "0");
		}
		EquipmentItemModel equipmentOfCategory = survivor.GetEquipmentOfCategory(EquipmentCategory.Armor);
		if (equipmentOfCategory != null)
		{
			dictionary.Add("armor_level", equipmentOfCategory.Level.ToString());
			dictionary.Add("armor_rarity", ModelHelpers.GetRarityNameForAnalytics(equipmentOfCategory.RarityLevel));
			dictionary.Add("armor_id", (equipmentOfCategory.IdForAnalytics != null) ? equipmentOfCategory.IdForAnalytics : "0");
		}
		dictionary.Add("total_survivors", Survivors.Count.ToString());
		dictionary.Add("total_missions", survivor.Statistics.NumberMissionPlayed.ToString());
		int num2 = 5;
		List<TraitEntry> traits = survivor.GetTraits();
		for (int i = 0; i < num2; i++)
		{
			string key = "trait_" + (i + 1);
			if (traits != null && i < traits.Count)
			{
				dictionary.Add(key, traits[i].TraitIdentifier);
			}
			else
			{
				dictionary.Add(key, "");
			}
		}
		dictionary.Add("survivor_id", (survivor.IdForAnalytics != null) ? survivor.IdForAnalytics : "0");
		return dictionary;
	}

	public int GetHighestLevelSurvivor()
	{
		int num = 0;
		for (int i = 0; i < Survivors.Count; i++)
		{
			SurvivorModel survivorModel = Survivors[i];
			if (survivorModel.Level > num)
			{
				num = survivorModel.Level;
			}
		}
		return num;
	}

	public int GetHighestLevelOfSurvivorClass(SurvivorClass survivorClass)
	{
		int num = 0;
		for (int i = 0; i < Survivors.Count; i++)
		{
			SurvivorModel survivorModel = Survivors[i];
			if (survivorModel.SurvivorClass == survivorClass && survivorModel.Level > num)
			{
				num = survivorModel.Level;
			}
		}
		return num;
	}

	private List<ActorDefinition> GetSurvivorActorDefinition(SurvivorClass cls, bool includeGachaOnly)
	{
		List<ActorDefinition> actorDefinitions = base.gameEconomyData.ActorDefinitions;
		string text = cls.ToString();
		List<ActorDefinition> list = new List<ActorDefinition>();
		for (int i = 0; i < actorDefinitions.Count; i++)
		{
			Faction faction = actorDefinitions[i].Faction;
			if ((faction == Faction.Any || faction == Faction.Survivor) && actorDefinitions[i].Class == text)
			{
				if (includeGachaOnly && actorDefinitions[i].IncludedInGacha)
				{
					list.Add(actorDefinitions[i]);
				}
				else if (!includeGachaOnly)
				{
					list.Add(actorDefinitions[i]);
				}
			}
		}
		return list;
	}

	public int NumberCombatSurvivorsHaveRequiredLevelForMission(int levelRequired)
	{
		int num = 0;
		foreach (SurvivorModel combatSurvivor in CombatSurvivors)
		{
			if (combatSurvivor.Level >= levelRequired)
			{
				num++;
			}
		}
		return num;
	}

	public int NumberAnySurvivorsHaveRequiredLevelForMission(int levelRequired)
	{
		int num = 0;
		foreach (SurvivorModel survivor in Survivors)
		{
			if (survivor.Level >= levelRequired)
			{
				num++;
			}
		}
		return num;
	}

	public int GetAverageSurvivorLevelFromTop3()
	{
		int[] array = new int[3];
		for (int i = 0; i < Survivors.Count; i++)
		{
			if (Survivors[i].Level > array[2])
			{
				array[0] = array[1];
				array[1] = array[2];
				array[2] = Survivors[i].Level;
			}
			else if (Survivors[i].Level > array[1])
			{
				array[0] = array[1];
				array[1] = Survivors[i].Level;
			}
			else if (Survivors[i].Level > array[0])
			{
				array[0] = Survivors[i].Level;
			}
		}
		return (array[0] + array[1] + array[2]) / 3;
	}

	public int GetGvGBaseDifficultyFromSurvivors()
	{
		int averageSurvivorLevelFromTop = GetAverageSurvivorLevelFromTop3();
		if (averageSurvivorLevelFromTop <= base.manager.GameEconomyData.GuildWarConfig.MinBaseLevelOffset)
		{
			return base.manager.GameEconomyData.GuildWarConfig.MinBaseLevelOffset;
		}
		return averageSurvivorLevelFromTop;
	}

	public int GetEndlessBaseDifficultyFromSurvivors()
	{
		int averageSurvivorLevelFromTop = GetAverageSurvivorLevelFromTop3();
		if (averageSurvivorLevelFromTop <= base.manager.GameEconomyData.EndlessModeConfig.MinBaseLevelOffset)
		{
			return base.manager.GameEconomyData.EndlessModeConfig.MinBaseLevelOffset;
		}
		return averageSurvivorLevelFromTop;
	}

	public int GetHighestSurvivorLevel()
	{
		int num = 1;
		int count = Survivors.Count;
		for (int i = 0; i < count; i++)
		{
			SurvivorModel survivorModel = Survivors[i];
			if (survivorModel.Level > num)
			{
				num = survivorModel.Level;
			}
		}
		return num;
	}

	public int GetHighestSurvivorRarity()
	{
		int num = 0;
		for (int i = 0; i < ((Survivors != null) ? Survivors.Count : 0); i++)
		{
			SurvivorModel survivorModel = Survivors[i];
			if (survivorModel.SurvivorRarityLevel > num)
			{
				num = survivorModel.SurvivorRarityLevel;
			}
		}
		return num;
	}

	public Cashier GetSearchSurvivorCashier()
	{
		Cashier cashier = new Cashier(base.manager);
		CashierItem cashierItem = new CashierItem(PurchaseType.SearchSurvivor);
		cashierItem.SetCost(CurrencyType.Phone, 1);
		cashier.AddItem(cashierItem);
		return cashier;
	}

	public bool IsSurvivorClassUnlocked(SurvivorClass survivorClass)
	{
		return base.manager.Blackboard.IsUnlocked("Unlock.Survivor." + survivorClass);
	}

	public bool IsHeroClassUnlocked(CurrencyType currencyType)
	{
		return IsSurvivorClassUnlocked(GetHeroSurvivorClass(currencyType));
	}

	public bool IsHeroTypeUnlocked(SurvivorClass survivorClass)
	{
		for (int i = 0; i < ((Survivors != null) ? Survivors.Count : 0); i++)
		{
			SurvivorModel survivorModel = Survivors[i];
			if (survivorModel.IsHero && survivorModel.SurvivorClass == survivorClass)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsHeroUnlocked(CurrencyType currencyType)
	{
		string heroId = SurvivorToken.GetHeroId(currencyType);
		return base.manager.Player.SurvivorContainer.GetHeroById(heroId) != null;
	}

	public bool HasEnoughTokenToUnlock(CurrencyType currencyType)
	{
		Cashier heroUnlockCashier = base.manager.Player.SurvivorContainer.GetHeroUnlockCashier(currencyType);
		return base.manager.Player.GetCurrency(currencyType).Value >= heroUnlockCashier.GetTotalCost(currencyType);
	}

	public int GetHeroUnlockCost(CurrencyType currencyType)
	{
		return base.manager.Player.SurvivorContainer.GetHeroUnlockCashier(currencyType)?.GetTotalCost(currencyType) ?? int.MaxValue;
	}

	public SurvivorClass GetHeroSurvivorClass(CurrencyType currencyType)
	{
		string heroId = SurvivorToken.GetHeroId(currencyType);
		ActorDefinition actorDefinition = base.gameEconomyData.GetActorDefinition(heroId);
		if (actorDefinition == null)
		{
			return SurvivorClass.None;
		}
		return (SurvivorClass)Enum.Parse(typeof(SurvivorClass), actorDefinition.Class);
	}

	public void UnlockSurvivorClass(SurvivorClass survivorClass)
	{
		base.manager.Blackboard.Unlock("Unlock.Survivor." + survivorClass);
	}

	public List<SurvivorModel> GetSurvivorsForType(SurvivorType survivorType)
	{
		if (survivorType == SurvivorType.Outpost)
		{
			return OutpostDefendingSurvivors;
		}
		return CombatSurvivors;
	}

	public List<SurvivorClass> GetAvailableClasses(int currentTrainingGroundLevel)
	{
		List<SurvivorClass> list = new List<SurvivorClass>();
		for (int i = 0; i < 6; i++)
		{
			SurvivorClass survivorClass = (SurvivorClass)i;
			if (base.manager.Player != null)
			{
				int minimumTrainingGroundLevelForClass = base.manager.Player.gameEconomyData.GetMinimumTrainingGroundLevelForClass(survivorClass);
				if (currentTrainingGroundLevel >= minimumTrainingGroundLevelForClass && IsSurvivorClassUnlocked(survivorClass))
				{
					list.Add(survivorClass);
				}
			}
		}
		return list;
	}

	public List<CurrencyType> GetAvailableHeroes(int rarityLevel)
	{
		List<CurrencyType> list = new List<CurrencyType>();
		for (int i = 0; i < base.gameEconomyData.ActorDefinitions.Count; i++)
		{
			ActorDefinition actorDefinition = base.gameEconomyData.ActorDefinitions[i];
			if (actorDefinition != null && actorDefinition.ID != null && actorDefinition.ID.Contains("Hero_") && actorDefinition.IsAvailableToUnlock(base.manager.Player.UtcTimeStamp) && rarityLevel == actorDefinition.RarityLevel)
			{
				list.Add(actorDefinition.TraitUpgradeCurrency);
			}
		}
		return list;
	}

	public bool HasUnLockedHero(PhoneCallDefinition phoneCallDefinition)
	{
		int num = phoneCallDefinition.GetParsedCurrencyTypeValues().Length;
		int num2 = 0;
		CurrencyType[] parsedCurrencyTypeValues = phoneCallDefinition.GetParsedCurrencyTypeValues();
		foreach (CurrencyType type in parsedCurrencyTypeValues)
		{
			ActorDefinition actorDefinition = base.gameEconomyData.GetActorDefinition(SurvivorToken.GetHeroId(type));
			if (HasUnLockedHero(actorDefinition))
			{
				num2++;
			}
		}
		return num2 == num;
	}

	public bool HasUnLockedHero(ActorDefinition actorDefinition)
	{
		if (HasHero(actorDefinition.ID))
		{
			return true;
		}
		if (HasEnoughTokenToUnlock(actorDefinition.TraitUpgradeCurrency))
		{
			return true;
		}
		return false;
	}
}
