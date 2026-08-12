using BaseModel;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;
using static CallCraft;
using static PhoneCallModel;

public partial class CallAutoManager : MonoBehaviour
{
	public static CallAutoManager Instance { get; private set; }
	private PlayerModel Player => DataManager.Instance?.Player ?? null;
	private GameEconomyData gameEconomyData => DataManager.Instance?.GameData ?? null;

	private Dictionary<string, ModelRandom> InitDedicatedRandomsZero;
	private Dictionary<string, ModelRandom> InitDedicatedRandoms;
	private Dictionary<string, ModelRandom> LastDedicatedRandoms;
	public Dictionary<string, ModelRandom> DedicatedRandoms => Player.LootManager.DedicatedRandoms;

	public string PriorityHeroToken => CallCraft.Instance.PriorityHeroToken;

	public PhoneCallDefinition phoneCallDefinition { get; protected set; }

	public PhoneCallDefinitionType CallType { get; private set; }
	public DropType CallDropType { get; private set; }
	public int NumRerolls { get; private set; }
	public int RerollIndex { get; private set; }
	public int CallSlotNumber { get; private set; }
	public List<bool> LootsRerollLockingList { get; set; }

	public List<LootItem> GetlootMaxList(List<LootItem> LootItemList)
	{
		return IsAlwaysPreferPriorityHero && PriorityHeroToken != "None" ? LootItemList.Where(x => x.CurrencyType.ToString().ToLower().Contains(PriorityHeroToken.ToLower())).ToList() : null;
	}

	public int btExpensiveNumber;
	public RadioCallButton btExpensive; // 21

	public int btCheapNumber;
	public RadioCallButton btCheap; // 6

	public int btRegularNumber;
	public RadioCallButton btRegular; // 3 (4)

	public List<RadioCallButton> CallButtonsList => NewPhonePopup.Instance.CallButtonsList;

	public RadioCallButton btCurrent { get; protected set; }

	[Header("Инкрементально число вызовов")]
	public int CallCount = 0;

	[Header("Заданное число вызовов")]
	public int CountOfCallCicles = 50;

	public bool IsMaxValueFond;
	public bool IsBackupRestoreRandom = true;
	public bool IsPreferHeroInRegular = true;
	public bool IsAlwaysPreferPriorityHero;

	string RewardHero;
	string RewardAmount;

	public bool IsSecondStage { get; protected set; }
	public int StageNumber = 0;

	public CallData CurrentCallData { get; protected set; } //!
	public List<CallData> CallDataList { get; protected set; } //!
	public List<CallScenarioInfo> HighRewardList { get; protected set; } //!
	public int NumLootChoosable { get; set; }
	public List<LootClaimType> LootsClaimedTypeList { get; set; }

	private bool allowLockedClassesOnRadio;
	private bool isShowNotes;

	private string savedCallDetails = "";
	private string savedHero = "";

	public enum BatchState
	{
		Running,
		Stop
	}

	public BatchState CurrentBatchState = BatchState.Stop;

	public void Awake()
	{
		Instance = this;
	}

	private void OnEnable()
	{
		DataManager.Instance.GetComponent<CallCraft>().OnVisualizeHandler += OnVisualize;
	}

	private void OnDisable()
	{
		if (DataManager.Instance) DataManager.Instance.GetComponent<CallCraft>().OnVisualizeHandler -= OnVisualize;
	}

	public void OnVisualize()
	{
		if (CallDataList != null && CallDataList.Count > 0) CallCraft.Instance.CallDataList = CallDataList;
	}

	[ContextMenu("Backup Model Random")]
	public void BackupModelRandom()
	{
		InitDedicatedRandoms = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);
		DebugTWD.Log("Backup randoms");
	}

	[ContextMenu("Restore Model Random")]
	public void RestoreModelRandom()
	{
		Player.LootManager.DedicatedRandoms = SetDedicatedRandom(InitDedicatedRandoms);
		DebugTWD.Log("Restore randoms");
	}

	[ContextMenu("Restore Model Random Scenario")]
	public void RestoreModelRandomScenario()
	{
		CurrentBatchState = BatchState.Stop;
		if (Player == null || InitDedicatedRandomsZero == null) return;

		StageNumber = 0;
		Player.LootManager.DedicatedRandoms = SetDedicatedRandom(InitDedicatedRandomsZero);
		DebugTWD.Log("Restore randoms Scenario");

		if (HighRewardList.Count > 0)
		{
			int index = -1;
			foreach (var item in HighRewardList)
			{
				index++;
				if (!string.IsNullOrEmpty(item.CallDataCountDetail))
				{
					var list = item.CallDataCountDetail.Split('+');
					if (int.Parse(list.Last()) > 1)
					{
						item.RewardHero = "[FF4343]" + item.RewardHero + "[-]"; // Red
					}
					else
					{
						item.RewardHero = "[19CF00]" + item.RewardHero + "[-]"; // Green
					}
				}
			}
			string text = "\nRecommend Heroes to Call is:\n" +
				string.Join("\n", HighRewardList.Select(x => x.RewardHero + "|" + x.CallDataCount +
				(!string.IsNullOrEmpty(x.CallDataCountDetail) ? "(" + x.CallDataCountDetail + ")" : "") + "|" + x.CallPriceSumm));
			UpdateUIPanel(text, isIncrement: true);
		}
		CallCraft.Instance.SetGenerated(true);
	}

	[ContextMenu("Restore Last Scenario State")]
	public void RestoreLastScenarioState()
	{
		Player.LootManager.DedicatedRandoms = SetDedicatedRandom(LastDedicatedRandoms);
		DebugTWD.Log("Restore Last Scenario State");
	}

	[ContextMenu("Find Nearest Hero")]
	public void FindNearestHero()
	{
		btExpensive = GetButtonBySlotNumber(btExpensiveNumber);
		if (btExpensive == null) return;
		StopAllCoroutines();

		allowLockedClassesOnRadio = Player.gameEconomyData.GetFeature("AllowLockedClassesOnRadio").Enabled;

		HighRewardList = new();
		CallDataList = new();

		StageNumber = 0;
		IsSecondStage = false;
		StartCoroutine(FindNearestHeroCor(btExpensive, CountOfCallCicles));
	}

	public RadioCallButton GetButtonBySlotNumber(int slotNumber)
	{
		if (CallButtonsList != null)
		{
			foreach (var bt in CallButtonsList)
			{
				if (bt.SlotNumber == slotNumber)
				{
					return bt;
				}
			}
		}
		return null;
	}
	BatchState oldCurrentBatchState;

	private void Update()
	{
		oldCurrentBatchState = CurrentBatchState;
	}

	public void OnValidate()
	{
		if (oldCurrentBatchState != CurrentBatchState && CurrentBatchState == BatchState.Stop)
		{
			DebugTWD.Log("Stop FindNearestHeroAdvanced");
			StopAllCoroutines();
			RestoreModelRandomScenario();
			HighRewardList = new();
		}
	}

	[ContextMenu("Find Nearest Hero Scenario")]
	public void FindNearestHeroAdvanced()
	{
		if (CurrentBatchState == BatchState.Running)
		{
			DebugTWD.Log("Stop FindNearestHeroAdvanced");
			StopAllCoroutines();
			RestoreModelRandomScenario();
			HighRewardList = new();
			return;
		}

		NewPhonePopup.Instance.MaxLevelPanel.ResetUICall();
		CallCraft.Instance.SetGenerated(false);
		CallCraft.Instance.SetViewed(false);

		btExpensive = GetButtonBySlotNumber(btExpensiveNumber);
		btCheap = GetButtonBySlotNumber(btCheapNumber);
		btRegular = GetButtonBySlotNumber(btRegularNumber);

		if (btCheap == null || btRegular == null || btRegular == null) return;
		StopAllCoroutines();

		allowLockedClassesOnRadio = Player.gameEconomyData.GetFeature("AllowLockedClassesOnRadio").Enabled;

		CallDataList = new();
		HighRewardList = new();

		isShowNotes = false;

		StageNumber = 0;
		InitDedicatedRandomsZero = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);
		IsSecondStage = false;
		StartCoroutine(FindNearestHeroAdvancedCor(CountOfCallCicles));
	}

	public bool IsSaveJSON;
	[ContextMenu("Save CallDataList")]
	public void SaveCallDataList()
	{
		if (CallDataList.Count > 0)
		{
			var CallDataListJson = JsonConvert.SerializeObject(CallDataList, Formatting.Indented);
			var file = @"g:\Unity Projects\TWD\EpicGames\HotUpdateData\json\callsManager.json";

			MyTools.SaveToFile(CallDataListJson, file);
		}
	}

	private IEnumerator FindNearestHeroAdvancedCor(int countOfCallCicles)
	{
		int currentStage = StageNumber;

		StartCoroutine(FindNearestHeroCor(btCheap, countOfCallCicles));
		yield return new WaitUntil(() => currentStage != StageNumber);
		if (IsSaveJSON) SaveCallDataList();
		//StartCoroutine(FindNearestHeroSecondCor());
	}

	private void ResetMaxValueFond()
	{
		if (IsMaxValueFond) IsMaxValueFond = false;
	}

	private IEnumerator FindNearestHeroSecondCor()
	{
		ResetMaxValueFond();
		if (CallDataList.Count > 150 || StageNumber > 200 || StageNumber == -1)
		{
			DebugTWD.Log("CallDataList is empty.");
			if (StageNumber == -1)
			{
				string text = "";
				if (DataManager.Instance.language == DataManager.Language.Ru)
					text = "В наградах вызова " + btExpensive.SlotNumber + " нет токенов " + PriorityHeroToken;
				else text = "There is no " + PriorityHeroToken + " in the rewards of call " + btExpensive.SlotNumber;
				UpdateUIPanel(text, isNoHero: false);
			}
			else UpdateUIPanel("", isNoHero: true);
			RestoreModelRandomScenario();
			yield break;
		}
		var callNumber = CallDataList.Last().CallNumber;
		int countSecond = 0;
		int lastCallCount = callNumber;
		IsSecondStage = true;
		while (true)
		{
			countSecond++;
			DoSimpleCall();

			LastDedicatedRandoms = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);

			StartCoroutine(FindNearestHeroCor(btExpensive, CountOfCallCicles));
			yield return CurrentBatchState == BatchState.Stop;
			if (CallCount > CountOfCallCicles)
			{
				DebugTWD.Log("CallDataList is empty.");
				UpdateUIPanel("", isNoHero: true);
				RestoreModelRandomScenario();
				yield break;
			}
			if (callNumber > 1 && (CallCount > callNumber || CallCount > lastCallCount))
			{
				int index = callNumber > 0 ? callNumber - 1 : 0;
				if (callNumber - 1 < 1)
				{
					DebugTWD.Log("CallDataList is empty.");
					UpdateUIPanel("", isNoHero: true);
					RestoreModelRandomScenario();
					yield break;
				}
				Player.LootManager.DedicatedRandoms = SetDedicatedRandom(CallDataList[index].DedicatedRandoms);
				CallDataList = CallDataList.GetRange(0, callNumber - 1);
				StartCoroutine(FindNearestHeroSecondCor());
				yield break;
			}
			else
			{
				lastCallCount = CallCount;
			}
			if (RewardHero == PriorityHeroToken || string.IsNullOrEmpty(PriorityHeroToken))
			{
				savedHero = RewardHero;
				string detail = (callNumber + 1).ToString() + "+" + countSecond + "+" + CallCount;
				DebugTWD.LogError("1.Pre. Нашли " + RewardHero + ", call second count: " + detail, DebugType.Call);

				var CallDataCount = CallDataList.Count;
				var CallPriceSumm = CallDataList.Select(x => x.CallPrice).Sum();

				CallScenarioInfo inListReward = HighRewardList.FirstOrDefault(x => x.RewardHero == RewardHero);
				if (inListReward == null)
				{
					inListReward = new()
					{
						RewardHero = RewardHero,
						CallDataCount = CallDataCount,
						CallPriceSumm = CallPriceSumm,
						CallDataCountDetail = detail
					};
					HighRewardList.Add(inListReward);
				}
				else
				{
					inListReward.CallDataCountDetail = detail;
				}

				IsSecondStage = false;
				if (CallCount > 0 && CallCount < 4)
				{
					if (CallCount > 1)
					{
						IsBackupRestoreRandom = false;
						StartCoroutine(FindNearestHeroCor(btCheap, CallCount - 1));
						yield return CurrentBatchState == BatchState.Stop;
						IsBackupRestoreRandom = true;
					}
					savedCallDetails = (callNumber + 1).ToString() + "+" + countSecond + "+" + CallCount;
					DebugTWD.LogError("2.Final. Нашли " + savedHero + ", call second count: " + savedCallDetails, DebugType.Call);
				}
				else
				{
					int index = callNumber > 0 ? callNumber - 1 : 0;
					if (callNumber - 1 < 1)
					{
						DebugTWD.Log("CallDataList is empty.", DebugType.Call);
						UpdateUIPanel("", isNoHero: true);
						RestoreModelRandomScenario();
						yield break;
					}
					Player.LootManager.DedicatedRandoms = SetDedicatedRandom(CallDataList[index].DedicatedRandoms);
					CallDataList = CallDataList.GetRange(0, callNumber - 1);
					StartCoroutine(FindNearestHeroSecondCor());
					yield break;
				}
				StartCoroutine(FindNearestHeroCor(btExpensive, CountOfCallCicles));
				yield return CurrentBatchState == BatchState.Stop;
				RestoreModelRandomScenario();
				yield break;
			}
			yield return null;
		}
	}

	private void UpdateUIPanel(string text, bool isNoHero = false, bool isIncrement = false)
	{
		if (isNoHero)
		{
			if (DataManager.Instance.language == DataManager.Language.Ru)
			{
				text = "Не удалось найти приоритетного героя";
			}
			else
			{
				text = "Failed to find priority hero";
			}
		}
		DebugTWD.Log(text, DebugType.Call);
		NewPhonePopup.Instance.MaxLevelPanel.UpdateUICall(text, isIncrement);
	}

	[ContextMenu("DoSimpleCall")]
	private void DoSimpleCall()
	{
		CurrentBatchState = BatchState.Running;
		ResetMaxValueFond();

		btCurrent = btRegular;
		List<string> currencuTypes = new List<string>();
		CallSlotNumber = btCurrent.SlotNumber;
		phoneCallDefinition = null;
		CallDropType = btCurrent.dropType;
		CallType = PhoneCallDefinitionType.None;

		if (CallSlotNumber >= 3)
		{
			phoneCallDefinition = gameEconomyData.GetPhoneCallDefinition(Player.UtcTimeStamp, CallSlotNumber);
			CallType = phoneCallDefinition.Type;
		}

		RerollIndex = 0;
		CallCount = 0;

		int num2 = 0;
		switch (CallDropType)
		{
			case DropType.Regular:
				num2 = 1;
				break;
			case DropType.Silver:
				num2 = 2;
				break;
			case DropType.Gold:
				num2 = 3;
				break;
		}
		if (phoneCallDefinition != null && phoneCallDefinition.HeroGuaranteed)
		{
			num2 = 3;
		}
		if (phoneCallDefinition != null)
		{
			NumRerolls = phoneCallDefinition.Rerolls;
		}
		else
		{
			NumRerolls = 0;
		}

		CurrentCallData = new CallData();
		CurrentCallData.CallNumber = CallDataList.Count;
		CurrentCallData.ButtonBySlotNumber = btCurrent;
		CurrentCallData.DedicatedRandoms = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);

		int forceRarity = GetForceRarity(CallDropType);
		List<LootEntry> LootsList = new List<LootEntry>();
		List<SurvivorClass> list = new List<SurvivorClass>();
		int num3 = 0;
		if (phoneCallDefinition != null && phoneCallDefinition.HeroGuaranteed)
		{
			num3 = Player.LootManager.GetDedicatedRandom("RadioPhone" + CallDropType).GetRandomInRange(0, num2 - 1); //"RadioPhoneGold"
		}
		CurrencyType[] array = new CurrencyType[1];
		if (phoneCallDefinition != null)
		{
			array = phoneCallDefinition.GetParsedCurrencyTypeValues();
		}
		bool flag = false;
		for (int i = 0; i < num2; i++)
		{
			int num4 = phoneCallDefinition != null && phoneCallDefinition.HeroGuaranteed && i == num3 ? 100 : 0;
			LootEntry lootEntry = GetRadioPhoneLoot(CallDropType, list, forceRarity, num4);
			flag |= Array.IndexOf(array, lootEntry.RewardedCurrency) != -1;
			LootsList.Add(lootEntry);
			OnLootsListEntryAdded(LootsList);
			if (i == 0 && lootEntry.GeneratedSurvivor != null && !list.Contains(lootEntry.GeneratedSurvivor.SurvivorClass))
			{
				list.Add(lootEntry.GeneratedSurvivor.SurvivorClass);
			}
			else
			{
				if (list != null) list = null;
			}
			forceRarity = -1;
		}
		if (phoneCallDefinition != null)
		{
			var specialPhoneCallState = Player.LootManager.GetSpecialPhoneCallState(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc);
			if (specialPhoneCallState != null)
			{
				if (flag)
				{
					specialPhoneCallState.CumulativeProbability = phoneCallDefinition.InitialProbabilityPercentage;
				}
				else
				{
					specialPhoneCallState.CumulativeProbability += phoneCallDefinition.ProbabilityPercentageIncrease;
				}
			}
		}
		DebugTWD.Log("Call " + CallCount + ", heroes: " + string.Join(", ", LootsList.Select(x => x.RewardedCurrency + "|" + x.RewardedAmount)), DebugType.Call);
		ExecuteRerolls(LootsList);

		CallDataList.Add(CurrentCallData);
	}

	private IEnumerator FindNearestHeroCor(RadioCallButton currentBtn, int cyclesCount)
	{
		CurrentBatchState = BatchState.Running;
		ResetMaxValueFond();

		btCurrent = currentBtn;
		List<string> currencuTypes = new List<string>();
		CallSlotNumber = btCurrent.SlotNumber;
		phoneCallDefinition = null;
		CallDropType = btCurrent.dropType;
		CallType = PhoneCallDefinitionType.None;
		RewardHero = "";
		RewardAmount = "";
		RerollIndex = 0;
		CallCount = 0;
		NumLootChoosable = 1;

		int num2 = 0;
		switch (CallDropType)
		{
			case DropType.Regular:
				num2 = 1;
				break;
			case DropType.Silver:
				num2 = 2;
				break;
			case DropType.Gold:
				num2 = 3;
				break;
		}

		if (CallSlotNumber >= 3)
		{
			phoneCallDefinition = gameEconomyData.GetPhoneCallDefinition(Player.UtcTimeStamp, CallSlotNumber);
			CallType = phoneCallDefinition.Type;

			if (phoneCallDefinition.HeroGuaranteed)
			{
				num2 = 3;
			}
			if (phoneCallDefinition.Rerolls > 0)
			{
				NumLootChoosable = num2;
			}
			var currencuTypesRaw = phoneCallDefinition.GetParsedCurrencyTypeValues().Select(x=>x.ToString()).ToList();
			foreach (var currencyType in currencuTypesRaw)
			{
				if (!currencuTypes.Contains(currencyType))
				{
					currencuTypes.Add(currencyType);
				}
			}
		}
		else
		{
			currencuTypes.Add(PriorityHeroToken);
		}
		if (PriorityHeroToken != "None" && !currencuTypes.Contains(PriorityHeroToken))
		{
			StageNumber = -1;
			string text = "В наградах вызова " + btCurrent.SlotNumber + " нет токенов " + PriorityHeroToken;
			UpdateUIPanel(text, isNoHero: false);
			yield break;
		}

		if (IsBackupRestoreRandom) BackupModelRandom();

		for (int j = 0; j < cyclesCount; j++)
		{
			CurrentCallData = new CallData()
			{
				CallNumber = CallDataList.Count,
				ButtonBySlotNumber = btCurrent,
				DedicatedRandoms = SetDedicatedRandom(Player.LootManager.DedicatedRandoms)
			};

			CallCount++;

			List<LootEntry> LootsList = new List<LootEntry>();
			List<SurvivorClass> list = new List<SurvivorClass>();
			CurrencyType[] array = new CurrencyType[1];

			NumRerolls = 0;

			int num3 = 0;
			if (phoneCallDefinition != null)
			{
				array = phoneCallDefinition.GetParsedCurrencyTypeValues();
				NumRerolls = phoneCallDefinition.Rerolls;
				if (phoneCallDefinition.HeroGuaranteed)
				{
					num3 = Player.LootManager.GetDedicatedRandom("RadioPhone" + CallDropType).GetRandomInRange(0, num2 - 1);
				}
			}

			int forceRarity = GetForceRarity(CallDropType);

			bool flag = false;
			for (int i = 0; i < num2; i++)
			{
				int num4 = phoneCallDefinition != null && phoneCallDefinition.HeroGuaranteed && i == num3 ? 100 : 0;
				LootEntry lootEntry = GetRadioPhoneLoot(CallDropType, list, forceRarity, num4);
				//QuinnToken, TaraToken, ProtectorDarylToken,,, BethToken, MerleToken, DwightToken
				flag |= Array.IndexOf(array, lootEntry.RewardedCurrency) != -1;
				LootsList.Add(lootEntry);
				OnLootsListEntryAdded(LootsList);
				if (i == 0 && lootEntry.GeneratedSurvivor != null && !list.Contains(lootEntry.GeneratedSurvivor.SurvivorClass))
				{
					list.Add(lootEntry.GeneratedSurvivor.SurvivorClass);
				}
				else
				{
					if (list != null) list = null;
				}
				forceRarity = -1;
			}
			if (phoneCallDefinition != null)
			{
				var specialPhoneCallState = Player.LootManager.GetSpecialPhoneCallState(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc);
				if (specialPhoneCallState != null)
				{
					if (flag)
					{
						specialPhoneCallState.CumulativeProbability = phoneCallDefinition.InitialProbabilityPercentage;
					}
					else
					{
						specialPhoneCallState.CumulativeProbability += phoneCallDefinition.ProbabilityPercentageIncrease;
					}
				}
			}
			DebugTWD.Log("Call " + CallCount + ", heroes: " + string.Join(", ", LootsList.Select(x => x.RewardedCurrency + "|" + x.RewardedAmount)), DebugType.Call);

			ExecuteRerolls(LootsList);

			if (!IsSecondStage)
			{
				CallDataList.Add(CurrentCallData);
			}

			if (IsMaxValueFond)
			{
				OnMaxValueFond();
				yield break;
			}
			yield return null;
		}

		if (IsBackupRestoreRandom) RestoreModelRandom();

		StageNumber = -1;
		int maxValue = btCurrent.parsedHeroTokensDropNumberValues != null ? btCurrent.parsedHeroTokensDropNumberValues.Last() : 0;

		string result = "В диапазоне 50 вызовов нет Джекпота " + (maxValue > 0 ? " (" + maxValue.ToString() + ")" : "");

		UpdateUIPanel(result, isIncrement: true);
		CurrentBatchState = BatchState.Stop;
		DebugTWD.Log("Cycle Call Finished");
	}

	private void OnMaxValueFond()
	{
		if (IsSaveJSON) SaveCallDataList();

		if (IsBackupRestoreRandom) RestoreModelRandom();

		int minCycle = CallCount < CountOfCallCicles ? CallCount : CountOfCallCicles;
		DebugTWD.Log("1. Finded MaxCallValue with token " + PriorityHeroToken + " for " + CallCount + " cycles", DebugType.Call);

		string result = "";
		StageNumber++;
		if (!isShowNotes)
		{
			result = string.IsNullOrEmpty(PriorityHeroToken) ? "Приоритетный герой не задан" : "Приоритетный герой: " + PriorityHeroToken;
			isShowNotes = true;
		}
		result += "\n" + "Джекпот для: " + RewardHero + " (" + RewardAmount + ")";
		result += "\n" + "через " + (minCycle - 1) + " вызовов";

		if (minCycle - 1 == 0 && btCurrent == btExpensive)
		{
			var CallDataCount = CallDataList.Count;
			var CallPriceSumm = CallDataList.Select(x => x.CallPrice).Sum();

			CallScenarioInfo inListReward = HighRewardList.FirstOrDefault(x => x.RewardHero == RewardHero);
			if (inListReward == null)
			{
				inListReward = new()
				{
					RewardHero = RewardHero,
					CallDataCount = CallDataCount,
					CallPriceSumm = CallPriceSumm
				};
				HighRewardList.Add(inListReward);
			}
			else if (inListReward.CallPriceSumm > CallPriceSumm || inListReward.CallDataCount > CallDataCount || !string.IsNullOrEmpty(inListReward.CallDataCountDetail))
			{
				inListReward.CallPriceSumm = CallPriceSumm;
				inListReward.CallDataCount = CallDataCount;
				if (!string.IsNullOrEmpty(inListReward.CallDataCountDetail) && !string.IsNullOrEmpty(savedCallDetails))
				{
					inListReward.CallDataCountDetail = savedCallDetails;
					savedCallDetails = "";
				}
			}
			if (!string.IsNullOrEmpty(savedCallDetails))
			{
				inListReward.CallDataCountDetail = savedCallDetails;
				savedCallDetails = "";
			}
		}
		UpdateUIPanel(result, isIncrement: false);
		CurrentBatchState = BatchState.Stop;
		DebugTWD.Log("Cycle Call Finished");
	}

	private void ExecuteRerolls(List<LootEntry> LootsList)
	{
		RerollIndex = 0;
		LootsRerollLockingList = null;
		int rewardAmountCurrent = 0;
		int rewardType = 0;

		int HeroRarityMin = btCurrent.parsedHeroTokensDropNumberValues != null ? btCurrent.parsedHeroTokensDropNumberValues.First() : int.Parse(btExpensive.HeroRarityAmounts.First().Amount);
		if (CallType == PhoneCallDefinitionType.GuaranteedHero || (CallType == PhoneCallDefinitionType.None && btCurrent.SlotNumber > 2))
		{
			while (NumRerolls > 0)
			{
				List<LootItem> lootMaxList = null;
				List<LootItem> LootItemList = new List<LootItem>();

				for (int j = 0; j < LootsList.Count; j++)
				{
					var lootEntry = LootsList[j];
					DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrencyType = lootEntry.DropCurrencyType;

					if (dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor || dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.ClassToken)
					{
						rewardType = 0;
						rewardAmountCurrent = lootEntry.GeneratedSurvivor.DemoteTokens;
					}
					else
					{
						rewardAmountCurrent = HelpersUI.GetActualRewardValue(btCurrent, lootEntry.RewardedAmount);
						rewardType = 1;
					}

					LootItemList.Add(new LootItem()
					{
						Index = j,
						Type = rewardType,
						RewardAmount = rewardAmountCurrent,
						CurrencyType = rewardType == 1 ? lootEntry.RewardedCurrency : SurvivorToken.GetClassAsCurrency(lootEntry.GeneratedSurvivor.SurvivorClass)
					});
				}

				int rewardAmountTokenMax;
				if (CallType == PhoneCallDefinitionType.None)
				{
					var survivors = LootItemList.Where(x => x.Type == 0);
					var heroTokens = LootItemList.Where(x => x.Type == 1);

					if (heroTokens != null && heroTokens.Count() > 0)
					{
						rewardAmountTokenMax = heroTokens.Max(x => x.RewardAmount);
						lootMaxList = heroTokens.Where(x => x.RewardAmount == rewardAmountTokenMax).ToList();
					}
					else
					{
						int rewardAmountSurvivorMax = survivors.Max(x => x.RewardAmount);
						lootMaxList = survivors.Where(x => x.RewardAmount == rewardAmountSurvivorMax).ToList();
					}
				}
				else
				{
					lootMaxList = GetlootMaxList(LootItemList);

					if (lootMaxList == null || lootMaxList.Count == 0)
					{
						rewardAmountTokenMax = LootItemList.Max(x => x.RewardAmount);
						if (rewardAmountTokenMax > HeroRarityMin)
						{
							lootMaxList = LootItemList.Where(x => x.RewardAmount <= rewardAmountTokenMax && x.RewardAmount > HeroRarityMin).ToList();
						}
					}
				}

				if (lootMaxList != null && lootMaxList.Count > 0)
				{
					foreach (var loot in lootMaxList)
					{
						if (SetLootLockedForReroll(LootsList, loot.Index, true))
						{
							DebugTWD.Log("Call " + CallCount + ", Reroll " + RerollIndex + ", Lock: " + loot.CurrencyType, DebugType.Call);
						}
					}
				}

				if (RerollCall(LootsList) == TWDModelResult.OK)
				{
					RerollIndex++;
				}

				var lockedList = LootsRerollLockingList ?? new() { false, false, false };

				List<string> rewardAmountList = LootItemList.Select(x => x.RewardAmount + "|" + x.CurrencyType).ToList();
				CurrentCallData.RewardAmountList.Add(rewardAmountList);
				CurrentCallData.LootsRerollLockingList.Add(lockedList);
			}

			List<string> rewardAmountList2 = LootsList.Select(x => x.RewardedAmount + "|" + x.RewardedCurrency).ToList();
			CurrentCallData.RewardAmountList.Add(rewardAmountList2);

			foreach (var loot in LootsList)
			{
				OnClickAcceptSelectedLoot(LootsList, loot);
			}
		}
		else
		{
			List<LootItem> lootMaxList = null;
			List<LootItem> LootItemList = new List<LootItem>();
			for (int j = 0; j < LootsList.Count; j++)
			{
				var lootEntry = LootsList[j];

				DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrencyType = lootEntry.DropCurrencyType;

				if (dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor || dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.ClassToken)
				{
					rewardAmountCurrent = lootEntry.GeneratedSurvivor.DemoteTokens;
					rewardType = 0;
				}
				else
				{
					rewardAmountCurrent = HelpersUI.GetActualRewardValue(btCurrent, lootEntry.RewardedAmount);
					rewardType = 1;
				}
				LootItemList.Add(new LootItem()
				{
					Index = j,
					Type = rewardType,
					RewardAmount = rewardAmountCurrent,
					CurrencyType = rewardType == 1 ? lootEntry.RewardedCurrency : SurvivorToken.GetClassAsCurrency(lootEntry.GeneratedSurvivor.SurvivorClass)
				});
			}

			var survivors = LootItemList.Where(x => x.Type == 0)?.ToList();
			var heroTokens = LootItemList.Where(x => x.Type == 1)?.ToList();

			if (heroTokens != null && heroTokens.Count > 0)
			{
				lootMaxList = GetlootMaxList(LootItemList);

				if (survivors.Count > 0)
				{
					if (IsPreferHeroInRegular)
					{
						var maxHeroAmount = heroTokens.Max(x => x.RewardAmount);
						lootMaxList = heroTokens.Where(x => x.RewardAmount == maxHeroAmount).ToList();
					}
					else
					{
						int rewardAmountSurvivorMax = survivors.Max(x => x.RewardAmount);
						lootMaxList = survivors.Where(x => x.RewardAmount >= rewardAmountSurvivorMax).ToList();
					}
				}

				if (lootMaxList == null || lootMaxList.Count == 0)
				{
					int rewardAmountTokenMax = LootItemList.Max(x => x.RewardAmount);
					if (rewardAmountTokenMax >= HeroRarityMin)
					{
						lootMaxList = LootItemList.Where(x => x.RewardAmount <= rewardAmountTokenMax && x.RewardAmount > HeroRarityMin).ToList();
					}
				}
			}
			else
			{
				int rewardAmountSurvivorMax = survivors.Max(x => x.RewardAmount);
				lootMaxList = survivors.Where(x => x.RewardAmount >= rewardAmountSurvivorMax).ToList();
			}

			LootItem lootMaxSingle = lootMaxList != null && lootMaxList.Count > 0 ? lootMaxList.First() : LootItemList.First();
			int index = lootMaxSingle.Index;

			List<bool> lockedList = new() { false, false, false };
			lockedList[index] = true;
			CurrentCallData.LootsRerollLockingList.Add(lockedList);
			List<string> rewardAmountList = LootItemList.Select(x => x.RewardAmount + "|" + x.CurrencyType).ToList();
			CurrentCallData.RewardAmountList.Add(rewardAmountList);
			OnClickAcceptSelectedLoot(LootsList, LootsList[index]);
		}
	}

	public void OnClickAcceptSelectedLoot(List<LootEntry> LootsList, LootEntry lootEntry)
	{
		if (lootEntry == null) return;

		if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
		{
			var survivorModel = lootEntry.GeneratedSurvivor;
			if (survivorModel != null)
			{
				ClearPendingPhoneCallLoot(LootsList, lootEntry, null);
			}
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
		{
			ClearPendingPhoneCallLoot(LootsList, lootEntry, null);
		}

		if (lootEntry.RewardedAmount == btCurrent.parsedHeroTokensDropNumberValues.Last())
		{
			RewardHero = lootEntry.RewardedCurrency.ToString();
			RewardAmount = lootEntry.RewardedAmount.ToString();

			DebugTWD.LogError("Reroll for: " + PriorityHeroToken + ", Value: " + RewardHero + "|" + RewardAmount, DebugType.Call);
			IsMaxValueFond = true;
		}
	}

	public void ClearPendingPhoneCallLoot(List<LootEntry> LootsList, LootEntry Loot, SurvivorModel acceptedSurvivor)
	{
		int lootIndex = LootsList.IndexOf(Loot);
		bool flag = true;
		if (CanClaimEntireMultiLootsList(LootsList))
		{
			if (lootIndex != -1)
			{
				if (!IsLootClaimed(LootsList, lootIndex))
				{
					SetLootClaimed(LootsList, lootIndex, (acceptedSurvivor == null) ? LootClaimType.Tokens : LootClaimType.AcceptedSurvivor);
				}
				flag = IsAllLootClaimed(LootsList);
				if (flag)
				{
					for (int j = 0; j < LootsList.Count; j++)
					{
						if (LootsList[j] != null && LootsList[j].GeneratedSurvivor != null && !IsLootClaimedType(LootsList, j, LootClaimType.AcceptedSurvivor))
						{
							for (int k = 0; k < LootsList[j].GeneratedSurvivor.EquipmentItems.Count; k++)
							{
								Player.Equipment.RemoveEquipment(LootsList[j].GeneratedSurvivor.EquipmentItems[k]);
							}
						}
					}
				}
			}
		}
		else
		{
			for (int l = 0; l < LootsList.Count; l++)
			{
				if (LootsList[l] != null && LootsList[l].GeneratedSurvivor != null && acceptedSurvivor != LootsList[l].GeneratedSurvivor)
				{
					for (int m = 0; m < LootsList[l].GeneratedSurvivor.EquipmentItems.Count; m++)
					{
						Player.Equipment.RemoveEquipment(LootsList[l].GeneratedSurvivor.EquipmentItems[m]);
					}
				}
			}
		}
		if (flag)
		{
			LootsRerollLockingList = null;
			LootsClaimedTypeList = null;
		}
	}

	public void SetLootClaimed(List<LootEntry> LootsList, int lootIndex, LootClaimType claimType)
	{
		if (LootsList == null)
		{
			return;
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

	private bool IsAllLootClaimed(List<LootEntry> LootsList)
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

	public bool IsLootClaimedType(List<LootEntry> LootsList, int lootIndex, LootClaimType claimType)
	{
		if (LootsList == null || LootsClaimedTypeList == null)
		{
			return claimType == LootClaimType.None;
		}
		if (lootIndex < 0 || lootIndex >= LootsClaimedTypeList.Count)
		{
			return claimType == LootClaimType.Tokens;
		}
		return LootsClaimedTypeList[lootIndex] == claimType;
	}

	private void OnLootsListEntryAdded(List<LootEntry> LootsList)
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
			LootsClaimedTypeList = new List<LootClaimType>();
			for (int k = 0; k < LootsList.Count; k++)
			{
				LootsClaimedTypeList.Add(LootClaimType.Tokens);
			}
		}
	}

	public bool IsLootClaimed(List<LootEntry> LootsList, int lootIndex)
	{
		if (LootsList == null || LootsClaimedTypeList == null)
		{
			return false;
		}
		if (lootIndex < 0 || lootIndex >= LootsClaimedTypeList.Count)
		{
			return true;
		}
		return LootsClaimedTypeList[lootIndex] != LootClaimType.None;
	}

	public bool CanClaimEntireMultiLootsList(List<LootEntry> LootsList)
	{
		if (NumLootChoosable == LootsList.Count)
		{
			return NumLootChoosable > 1;
		}
		return false;
	}

	public TWDModelResult RerollCall(List<LootEntry> LootsList)
	{
		if (NumRerolls <= 0)
		{
			return TWDModelResult.Error;
		}
		PhoneCallDefinition phoneCallDefinition = null;
		if (CallSlotNumber >= 3)
		{
			phoneCallDefinition = gameEconomyData.GetPhoneCallDefinition(Player.UtcTimeStamp, CallSlotNumber);
			int num = 2;
			while (phoneCallDefinition == null && num >= 0)
			{
				phoneCallDefinition = gameEconomyData.GetPhoneCallDefinition(Player.UtcTimeStamp, num--);
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
				int forceRarity = -1;
				int probabilityOverride = 0;
				LootsList[i] = GetRadioPhoneLoot(CallDropType, excludeSurvivorClasses, forceRarity, probabilityOverride);
				//DebugTWD.Log("RadioPhone: " + GetRandom().CallCount + "|" + GetRandom().State + "|" + LootsList[i].RewardedCurrency);
				LootsList[i].Opened = false;
				flag2 |= Array.IndexOf(array, LootsList[i].RewardedCurrency) != -1;
			}
		}
		DebugTWD.Log("Call " + CallCount + ", Reroll: " + RerollIndex + ", heroes: " + string.Join(", ", LootsList.Select(x => x.RewardedCurrency + "|" + x.RewardedAmount)));

		if (!flag)
		{
			NumRerolls = 0;
		}
		return TWDModelResult.OK;
	}

	public bool SetLootLockedForReroll(List<LootEntry> LootsList, int lootIndex, bool locked)
	{
		if (LootsList == null)
		{
			return false;
		}
		if (lootIndex < 0 || lootIndex >= LootsList.Count)
		{
			DebugTWD.LogError("Attempt to set reroll locking state with out of bounds phone call loot index.");
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

	public bool IsLootLockedForReroll(int lootIndex)
	{
		if (LootsRerollLockingList == null)
		{
			return false;
		}
		if (lootIndex < 0 || lootIndex >= LootsRerollLockingList.Count)
		{
			return false;
		}
		return LootsRerollLockingList[lootIndex];
	}

	private int GetForceRarity(DropType dropType)
	{
		int forceRarity = -1;
		if (dropType == DropType.Silver && Player.Blackboard.GetCounter("Counter.PhoneCallSilver") == 0)
		{
			forceRarity = 2;
		}
		else if (dropType == DropType.Gold && Player.Blackboard.GetCounter("Counter.PhoneCallGold") == 0)
		{
			forceRarity = 3;
		}
		else if (phoneCallDefinition == null)
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
					forceRarity = 3;
					break;
			}
		}
		return forceRarity;
	}

	private int GetRadioTentLevel()
	{
		int targetLevel = 0;
		if (Player.Camp != null)
		{
			BuildingModel building = Player.Camp.GetBuilding("RadioTent");
			if (building != null)
			{
				targetLevel = building.Level;
			}
		}
		return targetLevel;
	}

	public LootEntry GetRadioPhoneLoot(DropType dropType, List<SurvivorClass> ExcludeSurvivorClasses = null, int forceRarity = -1, int probabilityOverride = 0)
	{
		int targetLevel = GetRadioTentLevel();
		ModelRandom dedicatedRandom = Player.LootManager.GetDedicatedRandom("RadioPhone" + dropType);
		LootEntry lootEntry = null;
		SurvivorClass forceSurvivorClass = SurvivorClass.None;
		if (phoneCallDefinition != null && phoneCallDefinition.InitialProbabilityPercentage > 0)
		{
			int num = probabilityOverride > 0 ? probabilityOverride : phoneCallDefinition.InitialProbabilityPercentage;
			SpecialPhoneCallState specialPhoneCallState = Player.LootManager.GetSpecialPhoneCallState(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc);
			if (specialPhoneCallState != null)
			{
				num = probabilityOverride > 0 ? probabilityOverride : specialPhoneCallState.CumulativeProbability;
			}
			else
			{
				Player.LootManager.AddSpecialPhoneCallState(phoneCallDefinition.SlotNumber, phoneCallDefinition);
			}
			int getRandom = dedicatedRandom.GetRandomInRange(1, 100);
			if (getRandom <= num)
			{
				CurrencyType currencyType = CurrencyType.None;
				CurrencyType[] parsedCurrencyTypeValues = phoneCallDefinition.GetParsedCurrencyTypeValues();
				if (parsedCurrencyTypeValues.Length >= 1)
				{
					if (parsedCurrencyTypeValues.Length == 1)
					{
						currencyType = parsedCurrencyTypeValues[0];
					}
					else
					{
						int num2 = 0;
						int[] parsedCurrencyTypeDistributionValues = phoneCallDefinition.GetParsedCurrencyTypeDistributionValues();
						for (int i = 0; i < parsedCurrencyTypeValues.Length; i++)
						{
							num2 += parsedCurrencyTypeDistributionValues[i];
						}
						int randomInRange = dedicatedRandom.GetRandomInRange(1, num2);
						int num3 = 0;
						for (int j = 0; j < parsedCurrencyTypeValues.Length; j++)
						{
							num3 += parsedCurrencyTypeDistributionValues[j];
							if (num3 >= randomInRange)
							{
								currencyType = parsedCurrencyTypeValues[j];
								break;
							}
						}
					}
				}
				DropCurrenciesProbabilitiesDefinition.DropCurrency forcedCurrency = (currencyType != CurrencyType.None || phoneCallDefinition.HeroGuaranteed) ? DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken : DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor;
				var lootParams = new LootEntryGenParams
				{
					eventType = DropEventDefinition.DropEventType.RadioPhone,
					targetLevel = targetLevel,
					context = DropEventDefinition.DropEventContext.Normal,
					dropType = dropType,
					random = dedicatedRandom,
					forcedCurrency = forcedCurrency
				};

				lootEntry = Player.LootManager.ShuffleOneLoot(lootParams);
				lootEntry.DropType = phoneCallDefinition.DropType;
				if (currencyType != CurrencyType.None || phoneCallDefinition.HeroGuaranteed)
				{
					lootEntry.DropCurrencyType = DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken;
					if (currencyType != CurrencyType.None)
					{
						lootEntry.RewardedCurrency = currencyType;
					}
				}
				else
				{
					lootEntry.DropCurrencyType = DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor;
					forceSurvivorClass = phoneCallDefinition.SurvivorClass;
				}
			}
		}
		lootEntry ??= Player.LootManager.ShuffleOneLootWithoutTag(new LootEntryGenParams
		{
			eventType = DropEventDefinition.DropEventType.RadioPhone,
			targetLevel = targetLevel,
			context = DropEventDefinition.DropEventContext.Normal,
			dropType = dropType,
			random = dedicatedRandom
		});
		lootEntry.ExcludeSurvivorClasses = ExcludeSurvivorClasses;
		lootEntry.Random = dedicatedRandom;
		if (forceRarity != -1)
		{
			lootEntry.RewardedRarityLevel = forceRarity;
		}
		GiveLoot(lootEntry, forceSurvivorClass, allowLockedClassesOnRadio);
		return lootEntry;
	}

	public void GiveLoot(LootEntry lootEntry, SurvivorClass forceSurvivorClass = SurvivorClass.None, bool allowLockedClasses = false)
	{
		if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
		{
			RewardHeroToken(lootEntry);
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.ClassToken)
		{
			RewardClassToken(lootEntry);
		}
		else if (lootEntry.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor)
		{
			SurvivorModel generatedSurvivor = Player.SurvivorContainer.CreateRandomSurvivor(0, lootEntry.RewardedStartingLevel, lootEntry.RewardedStartingLevel, lootEntry.RewardedRarityLevel, forceSurvivorClass, null, 1, 0, lootEntry.ExcludeSurvivorClasses, includeGachaOnly: true, lootEntry.Random, SurvivorClass.None, 0, allowLockedClasses, true);
			lootEntry.GeneratedSurvivor = generatedSurvivor;
		}
	}

	public void RewardHeroToken(LootEntry lootEntry)
	{
		DropEventDefinition dropEventDefinition = lootEntry.DropEventDefinition;
		if (dropEventDefinition != null && dropEventDefinition.Tag == DropEventDefinition.DropEventTag.None)
		{
			int buildingLevel = Player.Camp.GetBuildingLevel("RadioTent");
			ModelRandom heroTokenRandom = Player.LootManager.GetDedicatedRandom("HeroToken");
			SurvivorToken heroTokenForGatcha = GetHeroTokenForGatcha(lootEntry.DropEventDefinition.EventType, lootEntry.DropType, DropEventDefinition.DropEventTag.None, buildingLevel, lootEntry.RewardedCurrency, heroTokenRandom, -1, phoneCallDefinition);
			if (heroTokenForGatcha != null && heroTokenForGatcha.Type != CurrencyType.None)
			{
				lootEntry.RewardedCurrency = heroTokenForGatcha.Type;
				lootEntry.RewardedAmount = heroTokenForGatcha.Amount;
				lootEntry.ActualAmountAdded = lootEntry.RewardedAmount;
				lootEntry.RewardedRarityLevel = heroTokenForGatcha.AmountRarityLevel;
			}
		}
	}

	public void RewardClassToken(LootEntry lootEntry)
	{
		int buildingLevel = Player.Camp.GetBuildingLevel("RadioTent");
		int trainingGroundLevel = Player.Camp.GetTrainingGroundLevel();
		ModelRandom challengeTokenRandom = Player.LootManager.GetDedicatedRandom("ChallengeToken");
		SurvivorToken survivorToken = GetClassTokenTypeAndAmount(availableClasses: Player.SurvivorContainer.GetAvailableClasses(trainingGroundLevel), eventType: lootEntry.DropEventDefinition.EventType, dropType: lootEntry.DropType, tag: lootEntry.DropEventDefinition.Tag, dropCurrency: lootEntry.DropCurrencyType, targetLevel: buildingLevel, random: challengeTokenRandom);
		if (survivorToken != null && survivorToken.Type != CurrencyType.None)
		{
			Player.GetCurrency(survivorToken.Type).Add(survivorToken.Amount);
			lootEntry.RewardedCurrency = survivorToken.Type;
			lootEntry.RewardedAmount = survivorToken.Amount;
			lootEntry.ActualAmountAdded = survivorToken.Amount;
			lootEntry.RewardedRarityLevel = survivorToken.AmountRarityLevel;
		}
	}

	public SurvivorToken GetClassTokenTypeAndAmount(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag tag, DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrency, int targetLevel, ModelRandom random, List<SurvivorClass> availableClasses)
	{
		TokenDropAmount[] TokenDropAmounts = gameEconomyData.TokenDropAmounts;
		int amount = 0;
		for (int i = 0; i < (TokenDropAmounts != null ? TokenDropAmounts.Length : 0); i++)
		{
			TokenDropAmount tokenDropAmount = TokenDropAmounts[i];
			if (tokenDropAmount != null && tokenDropAmount.EventType == eventType && tokenDropAmount.DropType == dropType && tokenDropAmount.Tag == tag && tokenDropAmount.DropCurrency == dropCurrency && targetLevel >= tokenDropAmount.ControlLevelMin && targetLevel <= tokenDropAmount.ControlLevelMax)
			{
				amount = random.GetRandomInRange(tokenDropAmount.Min, tokenDropAmount.Max);
				break;
			}
		}
		SurvivorClass randomElement = random.GetRandomElement(availableClasses.ToArray());
		return new SurvivorToken
		{
			Type = SurvivorToken.GetClassAsCurrency(randomElement),
			Amount = amount
		};
	}

	public SurvivorToken GetHeroTokenForGatcha(DropEventDefinition.DropEventType eventType, DropType dropType, DropEventDefinition.DropEventTag tag, int targetLevel, CurrencyType forceTokenType, ModelRandom random, int forceRarityLevel = -1, PhoneCallDefinition phoneCallDefinition = null)
	{
		HeroTokenDropDefinition heroTokenDropDefinition = gameEconomyData.GetHeroTokenDropDefinition(eventType, dropType, tag, targetLevel);
		if (heroTokenDropDefinition != null)
		{
			for (int i = 0; i < (gameEconomyData.HeroTokenDropDistributionDefinitions != null ? gameEconomyData.HeroTokenDropDistributionDefinitions.Length : 0); i++)
			{
				HeroTokenDropDistributionDefinition heroTokenDropDistributionDefinition = gameEconomyData.HeroTokenDropDistributionDefinitions[i];
				if (!(heroTokenDropDistributionDefinition.BucketId == heroTokenDropDefinition.BucketId) || !(heroTokenDropDistributionDefinition.BucketId != "HeroGrouping"))
				{
					continue;
				}
				DropEquipmentsAndSurvivorsRaritiesDefinition dropRarityDefinition = gameEconomyData.GetDropRarityDefinition(dropType, DropRewardType.HeroToken, targetLevel, tag);
				int num = forceRarityLevel != -1 ? forceRarityLevel : dropRarityDefinition.GetDropRarityForRandomNumber(random.Next() * 100f);
				SurvivorToken survivorToken = new SurvivorToken();
				if (forceTokenType != CurrencyType.None)
				{
					survivorToken.Type = forceTokenType;
				}
				else
				{
					survivorToken.Type = heroTokenDropDistributionDefinition.GetTokenTypeForRandomNumber(random.Next() * 100f);
				}
				survivorToken.Amount = gameEconomyData.GetHeroTokenAmountForRarity(survivorToken.Type, num);
				var hashList = new List<CurrencyType>() { CurrencyType.PerlieToken, CurrencyType.GauntletAaronToken, CurrencyType.SimonToken, CurrencyType.ProtectorDarylToken, CurrencyType.LydiaToken };
				if (survivorToken.Amount == 0 && hashList.Contains(survivorToken.Type))
				{
					survivorToken.Amount = gameEconomyData.GetHeroTokenAmountForRarity(CurrencyType.TaraToken, num);
				}
				survivorToken.AmountRarityLevel = num;
				if (phoneCallDefinition != null && phoneCallDefinition.HeroTokensDropNumber != null)
				{
					List<int> hreoKensDropNumberValues = phoneCallDefinition.getHreoKensDropNumberValues(out _);
					CurrencyType[] source = phoneCallDefinition.ParseCurrencyTypeValues(out bool parseError2);
					if (!parseError2 && source.Contains(survivorToken.Type))
					{
						switch (num)
						{
							case 2:
								survivorToken.Amount = hreoKensDropNumberValues[0];
								break;
							case 3:
								survivorToken.Amount = hreoKensDropNumberValues[1];
								break;
							case 4:
							case 5:
							case 6:
							case 7:
							case 8:
							case 9:
								survivorToken.Amount = hreoKensDropNumberValues[2];
								break;
						}
					}
				}
				if (num > 3)
				{
					RewardHero = survivorToken.Type.ToString();
					RewardAmount = survivorToken.Amount.ToString();
					DebugTWD.LogError("Max Reward Call for PriorityToken: " + PriorityHeroToken + ", Value: " + RewardHero + "|" + RewardAmount);
					IsMaxValueFond = true;
				}
				if (survivorToken.Type == CurrencyType.None && heroTokenDropDistributionDefinition.GlennToken > 0L)
				{
					survivorToken.Type = CurrencyType.GlennToken;
					survivorToken.Amount = 8;
					survivorToken.AmountRarityLevel = 0;
				}
				return survivorToken;
			}
		}
		return null;
	}
}
