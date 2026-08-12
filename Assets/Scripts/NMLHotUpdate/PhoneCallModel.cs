using BaseModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TWDModel;

public class PhoneCallModel : TWDModelObject
{
	public enum LootClaimType
	{
		None = 0,
		Tokens = 1,
		AcceptedSurvivor = 2
	}

	public const string EventNewFreeCallAvailable = "EventNewFreeCallAvailable";

	public const string EventPendingSurvivorCleared = "EventPendingSurvivorCleared";

	public const string CallMade = "CallMade";

	public const int NumberRegularCalls = 3;

	public const int GoldCallSlot = 2;

	private int totalPhonesUsedBeforeaCall;

	public LootEntry Loot { get; set; }

	public ModelList<LootEntry> LootsList { get; set; }

	public int NumLootChoosable { get; set; }

	public int NumRerolls { get; set; }

	public DropType DropTypeForReroll { get; set; }

	public int CallSlotNumberForReroll { get; set; }

	public List<bool> LootsRerollLockingList { get; set; }

	public List<LootClaimType> LootsClaimedTypeList { get; set; }

	public string IdForAnalytics { get; set; }

	public long[] MillisecondsTillFreeCall { get; protected set; }

	public int[] FreeCallsStacked { get; protected set; }

	[JsonIgnore]
	public DropType CurrentCallDraDropType { get; private set; }

	[JsonIgnore]
	public int CurrentSlotNumber { get; private set; }

	[JsonIgnore]
	public PhoneCallDefinitionType CallType { get; private set; }

	[JsonIgnore]
	public Cashier CurrentCashier { get; private set; }

	[JsonIgnore]
	public bool HasPendingSurvivor
	{
		get
		{
			if (LootsList != null)
			{
				return LootsList.Count > 0;
			}
			return false;
		}
	}

	public override void Start()
	{
		DebugTWD.Log("Start Phone: " + base.manager.Player.Name);
		base.Start();
		InitFreeCall();
	}

	private void InitFreeCall()
	{
		if (FreeCallsStacked == null)
		{
			FreeCallsStacked = new int[3];
		}
		if (MillisecondsTillFreeCall == null)
		{
			MillisecondsTillFreeCall = new long[3];
		}
		for (int i = 0; i < 3; i++)
		{
			ResetFreeCallTimer(i, giveImmediately: true);
		}
		UpdateFreeCallTimers();
	}

	public override void Initialize()
	{
		DebugTWD.Log("Init Phone: " + base.manager.Player.Name);
		base.Initialize();
		LootsList = new ModelList<LootEntry>();
		LootsList.SetManager(base.manager);
		LootsList.Initialize();
		LootsRerollLockingList = null;
		LootsClaimedTypeList = null;
	}

	public override bool IsValid()
	{
		return true;
	}

	public override void Tick(long deltaTime)
	{
		base.Tick(deltaTime);
		for (int i = 0; i < 3; i++)
		{
			long num = deltaTime;
			while (num > 0 && MillisecondsTillFreeCall[i] > 0)
			{
				long num2 = Math.Min(MillisecondsTillFreeCall[i], num);
				MillisecondsTillFreeCall[i] -= num2;
				num -= num2;
				if (MillisecondsTillFreeCall[i] <= 0)
				{
					AddFreeCallWithUpgradedChance(i);
					NotifyChange("EventNewFreeCallAvailable");
				}
			}
		}
	}

	public void UpdateFreeCallTimers(bool onBuildingUpgrade = false)
	{
		for (int i = 0; i < 3; i++)
		{
			long num = GetFreeCallTimeMs(i);
			long freeCallTimeOnUpgradeMs = GetFreeCallTimeOnUpgradeMs(i);
			if (num != 0L)
			{
				if (onBuildingUpgrade && freeCallTimeOnUpgradeMs > 0)
				{
					num = freeCallTimeOnUpgradeMs;
				}
				if ((num > 0 && MillisecondsTillFreeCall[i] == 0L) || MillisecondsTillFreeCall[i] > num)
				{
					MillisecondsTillFreeCall[i] = num;
				}
			}
		}
	}

	private void AddFreeCallWithUpgradedChance(int slot)
	{
		if (slot < FreeCallsStacked.Length - 1 && FreeCallsStacked[slot + 1] < GetFreeCallStackable(slot + 1))
		{
			int upgradedCallChance = GetUpgradedCallChance(slot);
			if (base.manager.Player.PlayerRandom.GetRandomInRange(0, 100) < upgradedCallChance)
			{
				ResetFreeCallTimer(slot);
				slot++;
			}
		}
		FreeCallsStacked[slot]++;
	}

	public bool CanClaimEntireMultiLootsList()
	{
		if (NumLootChoosable == LootsList.Count)
		{
			return NumLootChoosable > 1;
		}
		return false;
	}

	private void OnLootsListEntryAdded()
	{
		if (LootsRerollLockingList == null)
		{
			LootsRerollLockingList = new List<bool>();
			for (int i = 0; i < LootsList.Count; i++)
			{
				LootsRerollLockingList.Add(item: false);
			}
		}
		else
		{
			LootsRerollLockingList.Add(item: false);
			if (LootsRerollLockingList.Count != LootsList.Count)
			{
				base.Debug.LogError("LootsRerollLockingList and LootsList count mismatch!");
				LootsRerollLockingList = null;
			}
		}
		if (LootsClaimedTypeList == null)
		{
			LootsClaimedTypeList = new List<LootClaimType>();
			for (int j = 0; j < LootsList.Count; j++)
			{
				LootsClaimedTypeList.Add(LootClaimType.None);
			}
			return;
		}
		LootsClaimedTypeList.Add(LootClaimType.None);
		if (LootsClaimedTypeList.Count != LootsList.Count)
		{
			base.Debug.LogError("LootsClaimedTypeList and LootsList count mismatch!");
			LootsClaimedTypeList = new List<LootClaimType>();
			for (int k = 0; k < LootsList.Count; k++)
			{
				LootsClaimedTypeList.Add(LootClaimType.Tokens);
			}
		}
	}

	public void AddLoot(LootEntry entry)
	{
		if (LootsList != null && entry != null && (IsLoadDataManager || !ContainsLootEntry(entry, out _)))
		{
			LootsList.Add(entry);
			OnLootsListEntryAdded();
		}
	}

	public void RemoveLoot(LootEntry entry)
	{
		int lootIndex = -1;
		if (LootsList == null || entry == null || !ContainsLootEntry(entry, out lootIndex))
		{
			return;
		}
		if (lootIndex != -1)
		{
			if (LootsRerollLockingList != null)
			{
				LootsRerollLockingList.RemoveAt(lootIndex);
			}
			if (LootsClaimedTypeList != null)
			{
				LootsClaimedTypeList.RemoveAt(lootIndex);
			}
		}
		LootsList.Remove(entry);
	}

	public bool SetLootLockedForReroll(int lootIndex, bool locked)
	{
		if (LootsList == null)
		{
			return false;
		}
		if (lootIndex < 0 || lootIndex >= LootsList.Count)
		{
			base.Debug.LogError("Attempt to set reroll locking state with out of bounds phone call loot index.");
			return false;
		}
		if (LootsRerollLockingList == null)
		{
			LootsRerollLockingList = new List<bool>();
			for (int i = 0; i < LootsList.Count; i++)
			{
				LootsRerollLockingList.Add(i == lootIndex && locked);
			}
			return locked;
		}
		if (LootsRerollLockingList[lootIndex] != locked)
		{
			LootsRerollLockingList[lootIndex] = locked;
			return true;
		}
		return false;
	}

	public bool IsAllLootLockedForReroll()
	{
		if (LootsList == null)
		{
			return false;
		}
		for (int i = 0; i < LootsList.Count; i++)
		{
			if (!IsLootLockedForReroll(i))
			{
				return false;
			}
		}
		return true;
	}

	public bool IsLootLockedForReroll(int lootIndex)
	{
		if (LootsList == null || LootsRerollLockingList == null)
		{
			return false;
		}
		if (lootIndex < 0 || lootIndex >= LootsRerollLockingList.Count)
		{
			base.Debug.LogError("Attempt to access reroll locking state with out of bounds phone call loot index.");
			return false;
		}
		return LootsRerollLockingList[lootIndex];
	}

	public void SetLootClaimed(int lootIndex, LootClaimType claimType)
	{
		if (LootsList == null)
		{
			return;
		}
		if (lootIndex < 0 || lootIndex >= LootsList.Count)
		{
			base.Debug.LogError("Attempt to set loot claimed state with out of bounds phone call loot index.");
		}
		else if (LootsClaimedTypeList == null)
		{
			LootsClaimedTypeList = new List<LootClaimType>();
			for (int i = 0; i < LootsList.Count; i++)
			{
				LootsClaimedTypeList.Add((i == lootIndex) ? claimType : LootClaimType.None);
			}
		}
		else
		{
			LootsClaimedTypeList[lootIndex] = claimType;
		}
	}

	public bool IsLootClaimedType(int lootIndex, LootClaimType claimType)
	{
		if (LootsList == null || LootsClaimedTypeList == null)
		{
			return claimType == LootClaimType.None;
		}
		if (lootIndex < 0 || lootIndex >= LootsClaimedTypeList.Count)
		{
			base.Debug.LogError("Attempt to access loot claiming state with out of bounds phone call loot index.");
			return claimType == LootClaimType.Tokens;
		}
		return LootsClaimedTypeList[lootIndex] == claimType;
	}

	public bool IsLootClaimed(int lootIndex)
	{
		if (LootsList == null || LootsClaimedTypeList == null)
		{
			return false;
		}
		if (lootIndex < 0 || lootIndex >= LootsClaimedTypeList.Count)
		{
			base.Debug.LogError("Attempt to access loot claiming state with out of bounds phone call loot index.");
			return true;
		}
		return LootsClaimedTypeList[lootIndex] != LootClaimType.None;
	}

	private bool IsAllLootClaimed()
	{
		if (LootsList == null || LootsClaimedTypeList == null)
		{
			return false;
		}
		for (int i = 0; i < LootsClaimedTypeList.Count; i++)
		{
			if (LootsClaimedTypeList[i] == LootClaimType.None)
			{
				return false;
			}
		}
		return true;
	}

	public int SolveLootIndexForSurvivor(SurvivorModel survivorInLootList)
	{
		for (int i = 0; i < LootsList.Count; i++)
		{
			if (LootsList[i] != null && LootsList[i].GeneratedSurvivor != null && survivorInLootList == LootsList[i].GeneratedSurvivor)
			{
				return i;
			}
		}
		return -1;
	}

	public bool ContainsLootEntry(LootEntry lootEntry, out int lootIndex)
	{
		if (LootsList != null && lootEntry != null)
		{
			for (int i = 0; i < LootsList.Count; i++)
			{
				if (LootsList[i] != null && LootsList[i].ModelId == lootEntry.ModelId)
				{
					lootIndex = i;
					return true;
				}
			}
		}
		lootIndex = -1;
		return false;
	}

	public bool IsUnlocked(DropType dropType)
	{
		if (dropType == DropType.Regular)
		{
			return true;
		}
		BuildingModel building = base.manager.CampModel.GetBuilding("RadioTent");
		if (building == null)
		{
			return false;
		}
		if (dropType == DropType.Silver && building.Level < base.gameEconomyData.ConfigData.PhoneSilverUnlockAtLevel)
		{
			return false;
		}
		if (dropType == DropType.Gold && building.Level < base.gameEconomyData.ConfigData.PhoneGoldUnlockAtLevel)
		{
			return false;
		}
		return true;
	}

	public bool IsUnlocked(int callSlotNumber)
	{
		PhoneCallDefinition phoneCallDefinition = GetPhoneCallDefinition(callSlotNumber);
		if (phoneCallDefinition == null)
		{
			return IsUnlocked((DropType)callSlotNumber);
		}
		return IsUnlocked(phoneCallDefinition.DropType);
	}

	public bool IsRegularPhoneCall(int callSlotNumber)
	{
		return callSlotNumber < 3;
	}

	public bool HasPhoneCallDefinition(int callSlotNumber)
	{
		return base.manager.GameEconomyData.GetPhoneCallDefinition(base.manager.Player.UtcTimeStamp, callSlotNumber) != null;
	}

	public PhoneCallDefinition GetPhoneCallDefinition(int callSlotNumber)
	{
		return base.manager.GameEconomyData.GetPhoneCallDefinition(base.manager.Player.UtcTimeStamp, callSlotNumber);
	}

	private void RemoveRerolledLootEquipment(int lootIndex)
	{
		if (LootsList[lootIndex] != null && LootsList[lootIndex].GeneratedSurvivor != null)
		{
			for (int i = 0; i < LootsList[lootIndex].GeneratedSurvivor.EquipmentItems.Count; i++)
			{
				base.manager.Player.Equipment.RemoveEquipment(LootsList[lootIndex].GeneratedSurvivor.EquipmentItems[i]);
			}
		}
	}

	public TWDModelResult RerollCall()
	{
		Debug.Log("RerollCall");
		if (NumRerolls <= 0)
		{
			return TWDModelResult.Error;
		}
		PhoneCallDefinition phoneCallDefinition = null;
		if (!IsRegularPhoneCall(CallSlotNumberForReroll))
		{
			phoneCallDefinition = GetPhoneCallDefinition(CallSlotNumberForReroll);
			int num = 2;
			while (phoneCallDefinition == null && num >= 0)
			{
				phoneCallDefinition = GetPhoneCallDefinition(num--);
			}
			if (phoneCallDefinition == null)
			{
				return TWDModelResult.PhoneCallDefinitionNotFound;
			}
		}
		NumRerolls--;
		bool flag = false;
		List<SurvivorClass> excludeSurvivorClasses = new List<SurvivorClass>();
		CurrencyType[] array = new CurrencyType[1];
		if (phoneCallDefinition != null)
		{
			array = phoneCallDefinition.GetParsedCurrencyTypeValues();
		}
		bool flag2 = false;
		for (int i = 0; i < LootsList.Count; i++)
		{
			if (!IsLootLockedForReroll(i))
			{
				flag = true;
				RemoveRerolledLootEquipment(i);
				int forceRarity = -1;
				int probabilityOverride = 0;
				LootsList[i] = base.manager.Player.LootManager.GetRadioPhoneLoot(DropTypeForReroll, phoneCallDefinition, excludeSurvivorClasses, forceRarity, probabilityOverride);
				//DebugTWD.Log("RadioPhone: " + GetRandom().CallCount + "|" + GetRandom().State + "|" + LootsList[i].RewardedCurrency);

				LootsList[i].Opened = false;
				flag2 |= Array.IndexOf(array, LootsList[i].RewardedCurrency) != -1;
			}
			else
			{
				if (IsLoadDataManager)
				{
					CallCraft.Instance.CurrentCall.AcceptIndexes[CallCraft.Instance._SelectSurvivorsPopup.RerollIndex][i].value = true;
				}
			}
		}
		if (base.manager.Player.LootManager.GetSpecialPhoneCallState(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc) != null && flag2)
		{
			base.manager.Player.LootManager.ResetSpecialPhoneCallProbability(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc);
		}
		if (!flag)
		{
			NumRerolls = 0;
		}
		return TWDModelResult.OK;
	}

	private int NextCallNumber()
	{
		base.manager.Blackboard.IncreaseCounter("Counter.NumberPhoneCallsMade");
		return base.manager.Blackboard.GetCounter("Counter.NumberPhoneCallsMade", 1);
	}

	public TWDModelResult Call(DropType dropType, int callSlotNumber)
	{
		Debug.Log("Call");
		CallType = PhoneCallDefinitionType.None;
		if (dropType != DropType.Regular)
		{
			if (base.manager.CampModel == null)
			{
				return TWDModelResult.Error;
			}
			if (!IsUnlocked(dropType))
			{
				return TWDModelResult.Error;
			}
		}
		PhoneCallDefinition phoneCallDefinition = null;
		if (!IsRegularPhoneCall(callSlotNumber))
		{
			phoneCallDefinition = GetPhoneCallDefinition(callSlotNumber);
			if (phoneCallDefinition == null)
			{
				return TWDModelResult.PhoneCallDefinitionNotFound;
			}
		}
		CurrentCallDraDropType = dropType;
		CurrentSlotNumber = callSlotNumber;
		if (phoneCallDefinition != null)
		{
			CallType = phoneCallDefinition.Type;
			NumRerolls = phoneCallDefinition.Rerolls;
		}
		else
		{
			NumRerolls = 0;
		}
		if (!IsLoadDataManager)
		{
			IdForAnalytics = ModelHelpers.MD5Sum(NextCallNumber().ToString() + base.manager.Player.UtcTimeStamp);
		}
		else
		{
			GameManager.Instance.playerModel.Blackboard.IncreaseCounter("Counter.NumberPhoneCallsMade");
		}
		DropTypeForReroll = dropType;
		CallSlotNumberForReroll = callSlotNumber;
		Cashier cashier = GetCashier(dropType, callSlotNumber);
		cashier.UsedReason = "PhoneCall";
		CurrentCashier = cashier;
		TWDModelResult tWDModelResult = cashier.Pay(this);
		if (tWDModelResult != TWDModelResult.OK)
		{
			return tWDModelResult;
		}
		if (base.manager.Player.Tutorial.CurrentPartId == "Phone" && base.manager.Player.SurvivorContainer.GetSurvivorsOfClass(SurvivorClass.Bruiser).Count == 0)
		{
			AddLoot(base.manager.Player.LootManager.GiveForcedSurvivor(SurvivorClass.Bruiser, 1));
			NumLootChoosable = 1;
		}
		else
		{
			int num = int.MaxValue;
			int num2 = 0;
			switch (dropType)
			{
			case DropType.Regular:
				num2 = 1;
				num = base.gameEconomyData.ConfigData.MaxCrappyCallsInARowRegular;
				break;
			case DropType.Silver:
				num2 = 2;
				num = base.gameEconomyData.ConfigData.MaxCrappyCallsInARowSilver;
				break;
			case DropType.Gold:
				num2 = 3;
				num = base.gameEconomyData.ConfigData.MaxCrappyCallsInARowGold;
				break;
			}
			if (phoneCallDefinition != null && phoneCallDefinition.HeroGuaranteed)
			{
				num2 = 3;
			}
			if (phoneCallDefinition != null && phoneCallDefinition.Rerolls > 0)
			{
				NumLootChoosable = num2;
				Debug.Log("NumLootChoosable: " + NumLootChoosable);
			}
			else
			{
				NumLootChoosable = 1;
			}
			totalPhonesUsedBeforeaCall = base.manager.Player.Blackboard.GetCounter("Counter.PhonesUsed");
			base.manager.Player.Blackboard.IncreaseCounter("Counter.PhonesUsed", cashier.GetTotalCost(CurrencyType.Phone));
			int forceRarity = -1;
			if (dropType == DropType.Silver && base.manager.Blackboard.GetCounter("Counter.PhoneCallSilver") == 0)
			{
				forceRarity = 2;
			}
			else if (dropType == DropType.Gold && base.manager.Blackboard.GetCounter("Counter.PhoneCallGold") == 0)
			{
				forceRarity = 3;
			}
			else if (!cashier.CanAfford() && base.manager.Player.Blackboard.GetCounter("Counter.PhonesFirstTimeGoldCall") == 0)
			{
				forceRarity = 2;
				base.manager.Player.Blackboard.IncreaseCounter("Counter.PhonesFirstTimeGoldCall");
			}
			else if (phoneCallDefinition == null && base.manager.Player.Blackboard.GetCounter("Counter.CrappyCallsInARow." + dropType) >= num)
			{
				switch (dropType)
				{
				case DropType.Regular:
					forceRarity = 1;
					break;
				case DropType.Silver:
					forceRarity = 2;
					break;
				case DropType.Gold:
					forceRarity = 3;//-1
					break;
				}
				base.manager.Player.Blackboard.SetCounter("Counter.CrappyCallsInARow." + dropType, 0);
			}
			List<SurvivorClass> list = new List<SurvivorClass>();
			LootEntry lootEntry = null;
			int num3 = 0;
			int num4 = 0;
			if (phoneCallDefinition != null && phoneCallDefinition.HeroGuaranteed)
			{
				num3 = base.manager.Player.LootManager.GetDedicatedRandom("RadioPhone" + dropType).GetRandomInRange(0, num2 - 1);//1
				//DebugTWD.Log("RadioPhone: " + random.CallCount + "|" + random.State);
			}
			CurrencyType[] array = new CurrencyType[1];
			if (phoneCallDefinition != null)
			{
				array = phoneCallDefinition.GetParsedCurrencyTypeValues();
			}
			bool flag = false;
			for (int i = 0; i < num2; i++)
			{
				if (dropType == DropType.Silver)
				{
					base.manager.Blackboard.IncreaseCounter("Counter.PhoneCallSilver");
				}
				if (dropType == DropType.Gold)
				{
					base.manager.Blackboard.IncreaseCounter("Counter.PhoneCallGold");//gold
				}
				num4 = ((phoneCallDefinition != null && phoneCallDefinition.HeroGuaranteed && i == num3) ? 100 : 0); //0
				lootEntry = base.manager.Player.LootManager.GetRadioPhoneLoot(dropType, phoneCallDefinition, list, forceRarity, num4);//rerolls 3
				//DebugTWD.Log("RadioPhone: " + GetRandom().CallCount + "|" + GetRandom().State + "|" + lootEntry.RewardedCurrency);

				flag |= Array.IndexOf(array, lootEntry.RewardedCurrency) != -1;
				AddLoot(lootEntry);
				if (i < 1 && list != null && lootEntry != null && lootEntry.GeneratedSurvivor != null)
				{
					if (!list.Contains(lootEntry.GeneratedSurvivor.SurvivorClass))
					{
						list.Add(lootEntry.GeneratedSurvivor.SurvivorClass);
					}
				}
				else if (list != null)
				{
					list = null;
				}
				forceRarity = -1;
			}
			MakeSureToGiveGlennAfterManyCalls();
			if (phoneCallDefinition != null)
			{
				if (base.manager.Player.LootManager.GetSpecialPhoneCallState(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc) != null)
				{
					if (flag)
					{
						base.manager.Player.LootManager.ResetSpecialPhoneCallProbability(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc);
					}
					else
					{
						base.manager.Player.LootManager.IncrementSpecialPhoneCallProbability(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc);
					}
				}
			}
			else
			{
				CheckIfCrappyCall(dropType);
			}
			list = null;
		}
		if (IsRegularPhoneCall(callSlotNumber) && FreeCallsStacked[callSlotNumber] > 0)
		{
			FreeCallsStacked[callSlotNumber]--;
		}
		ResetFreeCallTimer(callSlotNumber);
		if (!IsLoadDataManager && OfflineManager.IsUseServices)
		{
			base.manager.Player.DailyQuestManager.StartAction("RadioCall");
			base.manager.Player.DailyQuestManager.CommitAction();
		}

		NotifyChange("CallMade");
		Debug.Log("Call Made");
		return TWDModelResult.OK;
	}

	private void CheckIfCrappyCall(DropType dropType)
	{
		int num = 0;
		for (int i = 0; i < LootsList.Count; i++)
		{
			if (LootsList[i].DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
			{
				if (dropType == DropType.Regular && LootsList[i].RewardedRarityLevel == 0)
				{
					num++;
				}
				else if (dropType == DropType.Silver && LootsList[i].RewardedRarityLevel <= 1)
				{
					num++;
				}
				else if (dropType == DropType.Gold && LootsList[i].RewardedRarityLevel <= 2)
				{
					num++;
				}
			}
			else if (LootsList[i].DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken && dropType == DropType.Gold && LootsList[i].RewardedRarityLevel == 0)
			{
				num++;
			}
		}
		if (num == LootsList.Count)
		{
			base.manager.Player.Blackboard.IncreaseCounter("Counter.CrappyCallsInARow." + dropType);
		}
		else
		{
			base.manager.Player.Blackboard.SetCounter("Counter.CrappyCallsInARow." + dropType, 0);
		}
	}

	private void MakeSureToGiveGlennAfterManyCalls()
	{
		PlayerModel player = base.manager.Player;
		int maxPhonesUsedToGetGlenn = base.manager.GameEconomyData.ConfigData.MaxPhonesUsedToGetGlenn;
		int counter = player.Blackboard.GetCounter("Counter.PhonesUsed");
		if (maxPhonesUsedToGetGlenn == 0 || totalPhonesUsedBeforeaCall % maxPhonesUsedToGetGlenn < counter % maxPhonesUsedToGetGlenn || totalPhonesUsedBeforeaCall == counter || !player.SurvivorContainer.IsHeroUnlocked(CurrencyType.DarylToken) || player.SurvivorContainer.IsHeroUnlocked(CurrencyType.GlennToken) || player.SurvivorContainer.HasEnoughTokenToUnlock(CurrencyType.GlennToken))
		{
			return;
		}
		LootEntry lootEntry = LootsList[0];
		if (lootEntry != null)
		{
			SurvivorToken heroTokenForGatcha = base.manager.GameEconomyData.GetHeroTokenForGatcha(DropEventDefinition.DropEventType.RadioPhone, DropType.Gold, DropEventDefinition.DropEventTag.None, 0, CurrencyType.GlennToken, base.manager.Player.PlayerRandom);
			if (heroTokenForGatcha != null)
			{
				lootEntry.DropCurrencyType = DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken;
				lootEntry.DropType = DropType.Gold;
				lootEntry.RewardedCurrency = heroTokenForGatcha.Type;
				lootEntry.RewardedAmount = heroTokenForGatcha.Amount;
				lootEntry.RewardedRarityLevel = heroTokenForGatcha.AmountRarityLevel;
			}
		}
	}

	private void ResetFreeCallTimer(int slotNumber, bool giveImmediately = false)
	{
		bool condition = IsLoadDataManager ? (IsUnlocked(slotNumber) && slotNumber < FreeCallsStacked.Length && FreeCallsStacked[slotNumber] < GetFreeCallStackable(slotNumber) && GetFreeCallTimeMs(slotNumber) > 0 && slotNumber < MillisecondsTillFreeCall.Length && MillisecondsTillFreeCall[slotNumber] <= 0) :
					((base.manager == null || base.manager.Player == null || base.manager.Player.Tutorial == null || base.manager.Player.Tutorial.StaticTutorialComplete) && IsUnlocked(slotNumber) && slotNumber < FreeCallsStacked.Length && FreeCallsStacked[slotNumber] < GetFreeCallStackable(slotNumber) && GetFreeCallTimeMs(slotNumber) > 0 && slotNumber < MillisecondsTillFreeCall.Length && MillisecondsTillFreeCall[slotNumber] <= 0);
		if (condition)
		{
			if (giveImmediately)
			{
				MillisecondsTillFreeCall[slotNumber] = 1L;
			}
			else
			{
				MillisecondsTillFreeCall[slotNumber] = GetFreeCallTimeMs(slotNumber);
			}
		}
	}

	public bool HasFreeCall()
	{
		if (!HasFreeCall(0) && !HasFreeCall(1))
		{
			return HasFreeCall(2);
		}
		return true;
	}

	public bool HasFreeCall(int slotNumber)
	{
		return GetFreeCallStacked(slotNumber) > 0;
	}

	public int GetFreeCallStacked(int slotNumber)
	{
		if (!IsRegularPhoneCall(slotNumber))
		{
			return 0;
		}
		if (!IsLoadDataManager & (base.manager != null && base.manager.Player != null && base.manager.Player.Tutorial != null && !base.manager.Player.Tutorial.StaticTutorialComplete))
		{
			return 0;
		}
		if (slotNumber >= FreeCallsStacked.Length)
		{
			return 0;
		}
		return FreeCallsStacked[slotNumber];
	}

	public void ClearPendingPhoneCallLoot(SurvivorModel acceptedSurvivor, int lootIndex)
	{
		if (Loot != null && Loot.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor && Loot.GeneratedSurvivor != null)
		{
			if (acceptedSurvivor != Loot.GeneratedSurvivor)
			{
				for (int i = 0; i < Loot.GeneratedSurvivor.EquipmentItems.Count; i++)
				{
					base.manager.Player.Equipment.RemoveEquipment(Loot.GeneratedSurvivor.EquipmentItems[i]);
				}
			}
			NotifyChange("EventPendingSurvivorCleared");
			Loot = null;
		}
		if (LootsList.Count <= 0)
		{
			return;
		}
		bool flag;
		if (CanClaimEntireMultiLootsList())
		{
			if (lootIndex != -1)
			{
				if (!IsLootClaimed(lootIndex))
				{
					SetLootClaimed(lootIndex, (acceptedSurvivor == null) ? LootClaimType.Tokens : LootClaimType.AcceptedSurvivor);
				}
				else
				{
					base.Debug.LogError("Phone call loot already claimed!");
				}
				flag = IsAllLootClaimed();
				if (flag)
				{
					for (int j = 0; j < LootsList.Count; j++)
					{
						if (LootsList[j] != null && LootsList[j].GeneratedSurvivor != null && !IsLootClaimedType(j, LootClaimType.AcceptedSurvivor))
						{
							for (int k = 0; k < LootsList[j].GeneratedSurvivor.EquipmentItems.Count; k++)
							{
								base.manager.Player.Equipment.RemoveEquipment(LootsList[j].GeneratedSurvivor.EquipmentItems[k]);
							}
						}
					}
				}
			}
			else
			{
				base.Debug.LogError("ClearPendingPhoneCallLoot called with invalid loot index.");
				flag = true;
			}
		}
		else
		{
			flag = true;
			for (int l = 0; l < LootsList.Count; l++)
			{
				if (LootsList[l] != null && LootsList[l].GeneratedSurvivor != null && acceptedSurvivor != LootsList[l].GeneratedSurvivor)
				{
					for (int m = 0; m < LootsList[l].GeneratedSurvivor.EquipmentItems.Count; m++)
					{
						base.manager.Player.Equipment.RemoveEquipment(LootsList[l].GeneratedSurvivor.EquipmentItems[m]);
					}
				}
			}
		}
		if (flag)
		{
			LootsList = new ModelList<LootEntry>();
			LootsList.SetManager(base.manager);
			LootsRerollLockingList = null;
			LootsClaimedTypeList = null;
			NotifyChange("EventPendingSurvivorCleared");
		}
	}

	public Cashier GetCashier(DropType dropType, int callSlotNumber = 0)
	{
		Cashier cashier = new Cashier(base.manager);
		CashierItem cashierItem = new CashierItem(PurchaseType.PhoneCall);
		if (!IsRegularPhoneCall(callSlotNumber))
		{
			PhoneCallDefinition phoneCallDefinition = GetPhoneCallDefinition(callSlotNumber);
			if (phoneCallDefinition != null)
			{
				cashierItem.SetCost(CurrencyType.Phone, phoneCallDefinition.Price);
			}
		}
		else if (!HasFreeCall(callSlotNumber))
		{
			RadioTentLevelData radioTentLevelData = null;
			BuildingModel building = base.manager.CampModel.GetBuilding("RadioTent");
			if (building != null)
			{
				radioTentLevelData = base.gameEconomyData.GetRadioTentDataForLevel(building.Level, dropType);
			}
			if (radioTentLevelData == null)
			{
				radioTentLevelData = base.gameEconomyData.GetRadioTentDataForLevel(0, dropType);
			}
			cashierItem.SetCost(CurrencyType.Phone, radioTentLevelData.CostRadioPhone);
			cashierItem.SetCost(CurrencyType.SurvivalPoints, radioTentLevelData.CostXp);
			cashierItem.SetCost(CurrencyType.Supplies, radioTentLevelData.CostSupplies);
			if (dropType == DropType.Gold)
			{
				cashierItem.SetCost(CurrencyType.Diamonds, radioTentLevelData.CostDiamonds);
			}
		}
		cashier.AddItem(cashierItem);
		return cashier;
	}

	public bool AnyFreeCallAvailable()
	{
		for (int i = 0; i < 3; i++)
		{
			if (MillisecondsTillFreeCall[i] > 0)
			{
				return true;
			}
		}
		return false;
	}

	public long GetFreeCallTimeMs(int slotNumber)
	{
		BuildingModel building = base.manager.CampModel.GetBuilding("RadioTent");
		if (building == null)
		{
			if (slotNumber == 0)
			{
				return base.manager.GameEconomyData.ConfigData.FreeCallTimeMs[0];
			}
			return 0L;
		}
		List<long> freeCallTimers = building.GetFreeCallTimers();
		if (slotNumber >= freeCallTimers.Count)
		{
			return 0L;
		}
		return freeCallTimers[slotNumber] * 1000;
	}

	public long GetFreeCallTimeOnUpgradeMs(int slotNumber)
	{
		BuildingModel building = base.manager.CampModel.GetBuilding("RadioTent");
		if (building == null)
		{
			return 0L;
		}
		List<int> freeCallMaxAmounts = building.GetFreeCallMaxAmounts();
		if (slotNumber >= freeCallMaxAmounts.Count)
		{
			return 0L;
		}
		return building.GetFreeCallTimersOnBuildingUpgrade() * 1000;
	}

	public int GetFreeCallStackable(int slotNumber)
	{
		BuildingModel building = base.manager.CampModel.GetBuilding("RadioTent");
		if (building == null)
		{
			if (slotNumber == 0)
			{
				return base.manager.GameEconomyData.ConfigData.FreeCallMaxStackable[0];
			}
			return 0;
		}
		List<int> freeCallMaxAmounts = building.GetFreeCallMaxAmounts();
		if (slotNumber >= freeCallMaxAmounts.Count)
		{
			return 0;
		}
		return freeCallMaxAmounts[slotNumber];
	}

	public int GetUpgradedCallChance(int slotNumber)
	{
		BuildingModel building = base.manager.CampModel.GetBuilding("RadioTent");
		if (building == null)
		{
			return 0;
		}
		List<int> upgradedCallChances = building.GetUpgradedCallChances();
		if (slotNumber >= upgradedCallChances.Count)
		{
			return 0;
		}
		return upgradedCallChances[slotNumber];
	}



	#region myparams
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	private ModelRandom GetRandom(string type = "") => base.manager.Player.LootManager.GetDedicatedRandom("RadioPhone" + (!string.IsNullOrEmpty(type) ? type : "Gold"));

	#endregion
}
