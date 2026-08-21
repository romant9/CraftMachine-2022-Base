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

public class CallAuto : MonoBehaviour
{
	private PlayerModel Player => DataManager.Instance.Player;
	private GameEconomyData gameEconomyData => DataManager.Instance.GameData;
	private Dictionary<string, ModelRandom> InitDedicatedRandomsZero;
	private Dictionary<string, ModelRandom> InitDedicatedRandoms;
	private Dictionary<string, ModelRandom> LastDedicatedRandoms;

	private string PriorityHeroTokenOrigin => CallCraft.Instance.PriorityHeroToken;

	//true - использовать при подготовке и джекпоте
	//false - только для джекпота
	public bool UseRerollRandom;
	private ModelRandom rerollRandom;
	private PhoneCallDefinition phoneCallDefinition;

	public PhoneCallDefinitionType CallType { get; private set; }
	public DropType CallDropType { get; private set; }
	public int NumRerolls { get; private set; }
	public int RerollIndex { get; private set; }
	public int CallSlotNumber { get; private set; }
	public List<bool> LootsRerollLockingList { get; set; }

	//private ModelRandom GetRandom(string type = "") => Player.LootManager.GetDedicatedRandom("RadioPhone" + (!string.IsNullOrEmpty(type) ? type : "Gold"));
	public TweenColor tweenColorBatch;
	public UILabel labelStateCount;

	public List<LootItem> GetlootMaxList(List<LootItem> LootItemList)
	{
		List<LootItem> lootMaxList = new List<LootItem>();
		foreach (var item in LootItemList)
		{
			if (item.CurrencyType.ToString().ToLower().Contains(PriorityHeroTokenOrigin.ToLower()) && !lootMaxList.Contains(item))
			{
				lootMaxList.Add(item);
			}
		}
		return lootMaxList;
	}

	public UIInput CallButtonExpensiveInput;
	public RadioCallButton buttonBySlotNumberExpensive; // 21
	public UIInput CallButtonCheapInput;
	public RadioCallButton buttonBySlotNumberСheap; // 6
	public UIInput CallButtonSimpleInput;
	public RadioCallButton buttonBySlotNumberSimple; // 3 (4)

	public RadioCallButton buttonBySlotNumberCurrent { get; protected set; }

	[Header("Инкрементально число вызовов")]
	public int CallCount = 0;
	//protected int CallCountStage1 = 0;

	[Header("Заданное число вызовов")]
	public int CountOfCallCicles = 50;

	public bool IsMaxValueFond;
	public bool IsBackupRestoreRandom = true;
	public bool UseRegularCalls = true;
	string RewardHero;
	string RewardAmount;

	public bool IsSecondStage { get; private set; }
	//public int buttonNumberExpensive = 21;
	//public int buttonNumberSimple = 3;
	//public int buttonNumberCheap = 6;

	public int StageNumber = 0;

	public List<CallData> CallDataList { get; protected set; }
	public CallData CurrentCallData { get; protected set; }
	public List<CallScenarioInfo> HighRewardList { get; protected set; }
	public int NumLootChoosable { get; set; }
	public List<LootClaimType> LootsClaimedTypeList { get; set; }

	private bool allowLockedClassesOnRadio;

	private string savedCallDetails = "";
	private string savedHero = "";

	public bool IsSaveJSON;

	void Start()
	{
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
		tweenColorBatch.SetCurrentValueToStart();
		tweenColorBatch.enabled = false;
		labelStateCount.gameObject.SetActive(false);

		OfflineManager.IsBatch = false;
		StageNumber = 0;
		//CallCraft.Instance.MaxRewardDedicatedRandoms = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);
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
		buttonBySlotNumberExpensive = GetCallButtonFromInput(CallButtonExpensiveInput);
		if (buttonBySlotNumberExpensive == null) return;
		StopAllCoroutines();

		allowLockedClassesOnRadio = Player.gameEconomyData.GetFeature("AllowLockedClassesOnRadio").Enabled;

		HighRewardList = new();
		CallDataList = new();

		StageNumber = 0;
		IsSecondStage = false;
		StartCoroutine(FindNearestHeroCor(buttonBySlotNumberExpensive, CountOfCallCicles));
	}

	private void Update()
	{
		if (labelStateCount.gameObject.activeSelf)
		{
			labelStateCount.text = StageNumber.ToString();
		}
	}

	[ContextMenu("Find Nearest Hero Scenario")]
	public void FindNearestHeroAdvanced()
	{
		if (OfflineManager.IsBatch)
		{
			DebugTWD.Log("Stop FindNearestHeroAdvanced");
			StopAllCoroutines();
			RestoreModelRandomScenario();
			HighRewardList = new();
			return;
		}

		labelStateCount.gameObject.SetActive(true);
		//StartCoroutine(StateCountCor());
		tweenColorBatch.enabled = true;
		tweenColorBatch.PlayForward();
		CallCraft.Instance._NewPhonePopup.MaxLevelPanel.ResetUICall();
		CallCraft.Instance.SetGenerated(false);
		CallCraft.Instance.SetViewed(false);

		buttonBySlotNumberExpensive = GetCallButtonFromInput(CallButtonExpensiveInput);
		buttonBySlotNumberСheap = GetCallButtonFromInput(CallButtonCheapInput);
		buttonBySlotNumberSimple = GetCallButtonFromInput(CallButtonSimpleInput);
		if (buttonBySlotNumberСheap == null || buttonBySlotNumberSimple == null || buttonBySlotNumberSimple == null) return;
		StopAllCoroutines();

		allowLockedClassesOnRadio = Player.gameEconomyData.GetFeature("AllowLockedClassesOnRadio").Enabled;

		CallDataList = new();
		HighRewardList = new();

		isShow1 = false;

		StageNumber = 0;
		InitDedicatedRandomsZero = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);

		var randomValue = UnityEngine.Random.Range(1000000, 9999999);
		rerollRandom = new ModelRandom(randomValue);

		IsSecondStage = false;
		StartCoroutine(FindNearestHeroAdvancedCor(CountOfCallCicles));
	}

	[ContextMenu("Save CallDataList")]
	public void SaveCallDataList()
	{
		if (CallDataList.Count > 0)
		{
			//var CallDataListJson = OfflineManager.JsonSerializer.Serialize(CallDataList);
			var CallDataListJson = JsonConvert.SerializeObject(CallDataList, Formatting.Indented);
			var file = @"g:\Unity Projects\TWD\EpicGames\HotUpdateData\json\calls.json";

			MyTools.SaveToFile(CallDataListJson, file);
		}
	}

	private IEnumerator FindNearestHeroAdvancedCor(int countOfCallCicles)
	{
		int currentStage = StageNumber;

		StartCoroutine(FindNearestHeroCor(buttonBySlotNumberСheap, countOfCallCicles));
		yield return new WaitUntil(() => currentStage != StageNumber);
		if (IsSaveJSON) SaveCallDataList();
		if (UseRegularCalls) StartCoroutine(FindNearestHeroSecondCor());
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
					text = "В наградах вызова " + buttonBySlotNumberExpensive.SlotNumber + " нет токенов " + PriorityHeroTokenOrigin;
				else text = "There is no " + PriorityHeroTokenOrigin + " in the rewards of call " + buttonBySlotNumberExpensive.SlotNumber;
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

			StartCoroutine(FindNearestHeroCor(buttonBySlotNumberExpensive, CountOfCallCicles));
			yield return new WaitUntil(() => OfflineManager.IsBatch == false);
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
			if (RewardHero == PriorityHeroTokenOrigin || PriorityHeroTokenOrigin == "None")
			{
				savedHero = RewardHero;
				string detail = (callNumber + 1).ToString() + "+" + countSecond + "+" + CallCount;
				DebugTWD.LogError("1.Pre. Нашли " + RewardHero + ", call second count: " + detail, DebugType.Call);

				var CallDataCount = CallDataList.Count;
				var CallPriceSumm = CallDataList.Select(x => x.CallPrice).Sum();

				//CallScenarioInfo inListReward = new()
				//{
				//    RewardHero = RewardHero,
				//    CallDataCount = CallDataCount,
				//    CallPriceSumm = CallPriceSumm,
				//    CallDataCountDetail = detail
				//};
				//HighRewardList.Add(inListReward);

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
					if (CallCount > 2)
					{
						IsBackupRestoreRandom = false;
						StartCoroutine(FindNearestHeroCor(buttonBySlotNumberСheap, CallCount - 1));
						yield return new WaitUntil(() => OfflineManager.IsBatch == false);
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
				StartCoroutine(FindNearestHeroCor(buttonBySlotNumberExpensive, CountOfCallCicles));
				yield return new WaitUntil(() => OfflineManager.IsBatch == false);
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
		CallCraft.Instance._NewPhonePopup.MaxLevelPanel.UpdateUICall(text, isIncrement);
	}

	[ContextMenu("DoSimpleCall")]
	private void DoSimpleCall()
	{
		OfflineManager.IsBatch = true;
		ResetMaxValueFond();

		buttonBySlotNumberCurrent = buttonBySlotNumberSimple;
		List<string> currencuTypes = new List<string>();
		CallSlotNumber = buttonBySlotNumberCurrent.SlotNumber;
		phoneCallDefinition = null;
		CallDropType = buttonBySlotNumberCurrent.dropType;
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
		CurrentCallData.ButtonBySlotNumber = buttonBySlotNumberCurrent;
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
			//Simple Call
			int num4 = phoneCallDefinition != null && phoneCallDefinition.HeroGuaranteed && i == num3 ? 100 : 0;
			LootEntry lootEntry = GetRadioPhoneLoot(CallDropType, list, forceRarity, num4);
			flag |= Array.IndexOf(array, lootEntry.RewardedCurrency) != -1;
			AddLoot(LootsList, lootEntry);
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
		if (phoneCallDefinition != null)
		{
			if (Player.LootManager.GetSpecialPhoneCallState(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc) != null)
			{
				if (flag)
				{
					Player.LootManager.ResetSpecialPhoneCallProbability(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc);
				}
				else
				{
					Player.LootManager.IncrementSpecialPhoneCallProbability(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc);
				}
			}
		}
		DebugTWD.Log("Call " + CallCount + ", heroes: " + string.Join(", ", LootsList.Select(x => x.RewardedCurrency + "|" + x.RewardedAmount)), DebugType.Call);
		ExecuteRerolls(LootsList);

		CurrentCallData.LootEntryList.AddRange(LootsList);
		CallDataList.Add(CurrentCallData);
	}

	public void AddLoot(List<LootEntry> LootsList, LootEntry entry)
	{
		if (LootsList != null && entry != null)
		{
			LootsList.Add(entry);
			OnLootsListEntryAdded(LootsList);
		}
	}

	private IEnumerator FindNearestHeroCor(RadioCallButton currentBtn, int cyclesCount)
	{
		OfflineManager.IsBatch = true;
		ResetMaxValueFond();

		buttonBySlotNumberCurrent = currentBtn;
		List<string> currencuTypes = new List<string>();
		CallSlotNumber = buttonBySlotNumberCurrent.SlotNumber;
		phoneCallDefinition = null;
		CallDropType = buttonBySlotNumberCurrent.dropType;
		CallType = PhoneCallDefinitionType.None;
		RewardHero = "";
		RewardAmount = "";

		if (buttonBySlotNumberCurrent.parsedHeroTokensDropNumberValues != null)
		{
			DebugTWD.Log("Reward values: " + string.Join(',', buttonBySlotNumberCurrent.parsedHeroTokensDropNumberValues), DebugType.Call);
		}
		if (CallSlotNumber >= 3)
		{
			phoneCallDefinition = gameEconomyData.GetPhoneCallDefinition(Player.UtcTimeStamp, CallSlotNumber);
			CallType = phoneCallDefinition.Type;

			List<string> currencuTypesRaw = phoneCallDefinition.CurrencyTypes.Split(';').ToList();
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
			currencuTypes.Add(PriorityHeroTokenOrigin);
		}
		if (PriorityHeroTokenOrigin != "None" && !currencuTypes.Contains(PriorityHeroTokenOrigin) && buttonBySlotNumberCurrent == buttonBySlotNumberExpensive)
		{
			StageNumber = -1;
			string text = "";
			if (DataManager.Instance.language == DataManager.Language.Ru)
				text = "В наградах вызова " + buttonBySlotNumberCurrent.SlotNumber + " нет токенов " + PriorityHeroTokenOrigin;
			else text = "There is no " + PriorityHeroTokenOrigin + " in the rewards of call " + buttonBySlotNumberCurrent.SlotNumber;
			UpdateUIPanel(text, isNoHero: false);
			yield break;
		}

		int minCycle = CountOfCallCicles;

		if (IsBackupRestoreRandom) BackupModelRandom();

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
		if (phoneCallDefinition != null && phoneCallDefinition.Rerolls > 0)
		{
			NumLootChoosable = num2;
		}
		else
		{
			NumLootChoosable = 1;
		}
		for (int j = 0; j < cyclesCount; j++)
		{
			CurrentCallData = new CallData();
			CurrentCallData.CallNumber = CallDataList.Count;
			CurrentCallData.ButtonBySlotNumber = buttonBySlotNumberCurrent;
			CurrentCallData.DedicatedRandoms = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);

			CallCount++;

			if (phoneCallDefinition != null)
			{
				NumRerolls = phoneCallDefinition.Rerolls;
			}
			else
			{
				NumRerolls = 0;
			}
			int forceRarity = GetForceRarity(CallDropType);
			List<LootEntry> LootsList = new List<LootEntry>();
			List<SurvivorClass> list = new List<SurvivorClass>();
			int num3 = 0;
			if (phoneCallDefinition != null && phoneCallDefinition.HeroGuaranteed)
			{
				num3 = Player.LootManager.GetDedicatedRandom("RadioPhone" + CallDropType).GetRandomInRange(0, num2 - 1);
				//"RadioPhoneGold" //212971/1741074398, 213004/99483551
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
				int num4 = phoneCallDefinition != null && phoneCallDefinition.HeroGuaranteed && i == num3 ? 100 : 0;
				LootEntry lootEntry = GetRadioPhoneLoot(CallDropType, list, forceRarity, num4); //QuinnToken, TaraToken, ProtectorDarylToken
				//DebugTWD.Log("RadioPhone: " + GetRandom().CallCount + "|" + GetRandom().State + "|" + lootEntry.RewardedCurrency);

				flag |= Array.IndexOf(array, lootEntry.RewardedCurrency) != -1; //BethToken, MerleToken, DwightToken
				if (LootsList != null && lootEntry != null)
				{
					LootsList.Add(lootEntry);
				}
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
			if (phoneCallDefinition != null)
			{
				if (Player.LootManager.GetSpecialPhoneCallState(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc) != null)
				{
					if (flag)
					{
						Player.LootManager.ResetSpecialPhoneCallProbability(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc);
					}
					else
					{
						Player.LootManager.IncrementSpecialPhoneCallProbability(phoneCallDefinition.SlotNumber, phoneCallDefinition.EndTimeUtc);
					}
				}
			}
			DebugTWD.Log("Call " + CallCount + ", heroes: " + string.Join(", ", LootsList.Select(x => x.RewardedCurrency + "|" + x.RewardedAmount)), DebugType.Call);

			ExecuteRerolls(LootsList);

			if (!IsSecondStage)
			{
				CurrentCallData.LootEntryList.AddRange(LootsList);
				CallDataList.Add(CurrentCallData);
			}

			if (IsMaxValueFond)
			{
				OnMaxValueFond(minCycle);				
				yield break;
			}
			yield return null;
		}

		if (IsBackupRestoreRandom) RestoreModelRandom();

		StageNumber = -1;
		int maxValue = 0;
		if (buttonBySlotNumberCurrent.parsedHeroTokensDropNumberValues != null)
		{
			maxValue = buttonBySlotNumberCurrent.parsedHeroTokensDropNumberValues.Last();
		}
		else
		{
			maxValue = 0;
		}

		string result = "";
		if (DataManager.Instance.language == DataManager.Language.Ru)
		{
			result = "В диапазоне 50 вызовов нет Джекпота " + (maxValue > 0 ? " (" + maxValue.ToString() + ")" : "");
		}
		else
		{
			result = "There is no JackPot for 50 calls range" + (maxValue > 0 ? " (" + maxValue.ToString() + ")" : "");
		}

		UpdateUIPanel(result, isIncrement: true);
		OfflineManager.IsBatch = false;
		DebugTWD.Log("Cycle Call Finished");
	}

	bool isShow1 = false;

	private void OnMaxValueFond(int minCycle)
	{
		if (IsBackupRestoreRandom) RestoreModelRandom();

		DebugTWD.Log("1. Finded MaxCallValue with token " + PriorityHeroTokenOrigin + " for " + CallCount + " cycles", DebugType.Call);
		if (CallCount < minCycle)
		{
			minCycle = CallCount;
		}

		string result = "";
		StageNumber++;
		if (DataManager.Instance.language == DataManager.Language.Ru)
		{
			if (!isShow1)
			{
				result = PriorityHeroTokenOrigin == "None" ? "Приоритетный герой не задан" : "Приоритетный герой: " + PriorityHeroTokenOrigin;
				isShow1 = true;
			}
			result += "\n" + "Джекпот для: " + RewardHero + " (" + RewardAmount + ")";
			result += "\n" + "через " + (minCycle - 1) + " вызовов";
		}
		else
		{
			if (!isShow1)
			{
				result = PriorityHeroTokenOrigin == "None" ? "Priority hero not specified" : "Priority hero: " + PriorityHeroTokenOrigin;
				isShow1 = true;
			}
			result += "\n" + "Jackpot for: " + RewardHero + " (" + RewardAmount + ")";
			result += "\n" + "after " + (minCycle - 1) + " calls";
		}

		//if (minCycle - 1 == 0 && buttonBySlotNumberCurrent == buttonBySlotNumberExpensive)
		if (minCycle - 1 < 3 && buttonBySlotNumberCurrent == buttonBySlotNumberExpensive)
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

			CallCraft.Instance.MaxRewardDedicatedRandoms = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);
		}
		UpdateUIPanel(result, isIncrement: false);
		OfflineManager.IsBatch = false;
		DebugTWD.Log("Cycle Call Finished");
	}

	private void ExecuteRerolls(List<LootEntry> LootsList)
	{
		RerollIndex = 0;
		LootsRerollLockingList = null;
		int rewardAmountCurrent = 0;
		int rewardType = 0;

		int HeroRarityMin = buttonBySlotNumberCurrent.parsedHeroTokensDropNumberValues != null ? buttonBySlotNumberCurrent.parsedHeroTokensDropNumberValues.First() : int.Parse(buttonBySlotNumberExpensive.HeroRarityAmounts.First().Amount);
		int HeroRarityMax = buttonBySlotNumberCurrent.parsedHeroTokensDropNumberValues != null ? buttonBySlotNumberCurrent.parsedHeroTokensDropNumberValues.Last() : int.Parse(buttonBySlotNumberExpensive.HeroRarityAmounts.Last().Amount);

		LootItem MaxLoot = null;
		List<LootItem> lootMaxList = null;
		List<LootItem> LootItemList = new List<LootItem>();
		var callRarityTypeCurrent = CallCraft.Instance.CallRarityTypeCurrent;
		if (CallType == PhoneCallDefinitionType.GuaranteedHero || (CallType == PhoneCallDefinitionType.None && buttonBySlotNumberCurrent.SlotNumber > 2))
		{
			//NumRerolls++;
			while (NumRerolls > 0)
			{
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
						rewardAmountCurrent = HelpersUI.GetActualRewardValue(buttonBySlotNumberCurrent, lootEntry.RewardedAmount);
						rewardType = 1;
					}

					LootItemList.Add(new LootItem()
					{
						Index = j,
						Type = rewardType,
						RewardAmount = rewardAmountCurrent,
						CurrencyType = rewardType == 1 ? lootEntry.RewardedCurrency : SurvivorToken.GetClassAsCurrency(lootEntry.GeneratedSurvivor.SurvivorClass)
					});

					if (rewardAmountCurrent == HeroRarityMax)
					{
						MaxLoot = LootItemList.Last();
						DebugTWD.Log("Find MAX Loot: " + MaxLoot.CurrencyType + " " + MaxLoot.RewardAmount);
					}
				}

				List<bool> lockedList = new();
				int rewardAmountTokenMax;

				if (UseRerollRandom && !IsSecondStage)
				{
					lootMaxList = new List<LootItem>();
					if (LootsRerollLockingList == null)
					{
						LootsRerollLockingList = new List<bool>() { false, false, false };

						for (int i = 0; i < LootsRerollLockingList.Count; i++)
						{
							if (LootItemList[i].RewardAmount == HeroRarityMax)
							{
								lootMaxList.Add(LootItemList[i]);
								LootsRerollLockingList[i] = true;
							}
							else
							{
								if (LootItemList[i].CurrencyType.ToString() == PriorityHeroTokenOrigin)
								{
									LootsRerollLockingList[i] = false;
								}
								else
								{
									var randomValue = rerollRandom.GetRandomInRange(0, 1);
									if (randomValue == 1)
									{
										lootMaxList.Add(LootItemList[i]);
										LootsRerollLockingList[i] = true;
									}
								}
							}
						}
					}
					else
					{
						for (int i = 0; i < LootsRerollLockingList.Count; i++)
						{
							if (LootsRerollLockingList[i])
							{
								lootMaxList.Add(LootItemList[i]);
							}
							else
							{
								if (LootItemList[i].RewardAmount == HeroRarityMax)
								{
									lootMaxList.Add(LootItemList[i]);
									LootsRerollLockingList[i] = true;
								}
								else
								{
									if (LootItemList[i].CurrencyType.ToString() == PriorityHeroTokenOrigin)
									{
										LootsRerollLockingList[i] = false;
									}
									else
									{
										var randomValue = rerollRandom.GetRandomInRange(0, 1);
										if (randomValue == 1)
										{
											lootMaxList.Add(LootItemList[i]);
											LootsRerollLockingList[i] = true;
										}
									}
								}
							}
						}
					}
					lockedList.AddRange(LootsRerollLockingList);
				}
				else
				{
					lootMaxList = new();
					if (callRarityTypeCurrent != CallRarityType.Auto)
					{
						if (callRarityTypeCurrent == CallRarityType.All)
						{
							lootMaxList = LootItemList;
						}

						if (MaxLoot != null && !lootMaxList.Contains(MaxLoot))
						{
							lootMaxList.Add(MaxLoot);
						}
					}
					else
					{
						if (CallType == PhoneCallDefinitionType.None)
						{
							var heroTokens = LootItemList.Where(x => x.Type == 1);

							if (heroTokens != null && heroTokens.Count() > 0)
							{
								rewardAmountTokenMax = heroTokens.Max(x => x.RewardAmount);
								lootMaxList = heroTokens.Where(x => x.RewardAmount == rewardAmountTokenMax).ToList();
							}
							else
							{
								int rewardAmountSurvivorMax = LootItemList.Max(x => x.RewardAmount);
								lootMaxList = LootItemList.Where(x => x.RewardAmount == rewardAmountSurvivorMax).ToList();
							}
						}
						else
						{
							rewardAmountTokenMax = LootItemList.Max(x => x.RewardAmount);
							if (rewardAmountTokenMax > HeroRarityMin)
							{
								lootMaxList = LootItemList.Where(x => x.RewardAmount <= rewardAmountTokenMax && x.RewardAmount > HeroRarityMin).ToList();
							}
						}
						if (lootMaxList.Count == 0) lootMaxList = null;
					}
					
					//
					if (lootMaxList != null && lootMaxList.Count > 0)
					{
						foreach (var loot in lootMaxList)
						{
							if (SetLootLockedForReroll(LootsList, loot.Index, true))
							{
								DebugTWD.Log("Call " + CallCount + ", Reroll " + RerollIndex + ", Block: " + loot.CurrencyType, DebugType.Call);
							}
						}
						if (LootsRerollLockingList != null) lockedList.AddRange(LootsRerollLockingList);
					}
					else
					{
						if (LootsRerollLockingList == null)
						{
							LootsRerollLockingList = new List<bool>();
							foreach (var loot in LootsList)
							{
								lockedList.Add(false);
								LootsRerollLockingList.Add(false);
							}
						}
						else
						{
							lockedList.AddRange(LootsRerollLockingList);
						}
					}
				}

				if (RerollCall(LootsList) == TWDModelResult.OK)
				{
					RerollIndex++;
				}

				List<string> rewardAmountList = LootItemList.Select(x => x.RewardAmount + "|" + x.CurrencyType).ToList();
				CurrentCallData.RewardAmountList.Add(rewardAmountList);
				if (lockedList.Count == 0) lockedList = new() { false, false, false };
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
					rewardAmountCurrent = HelpersUI.GetActualRewardValue(buttonBySlotNumberCurrent, lootEntry.RewardedAmount);
					rewardType = 1;
				}
				LootItemList.Add(new LootItem()
				{
					Index = j,
					Type = rewardType,
					RewardAmount = rewardAmountCurrent,
					CurrencyType = rewardType == 1 ? lootEntry.RewardedCurrency : SurvivorToken.GetClassAsCurrency(lootEntry.GeneratedSurvivor.SurvivorClass)
				});

				if (rewardAmountCurrent == HeroRarityMax)
				{
					MaxLoot = LootItemList.Last();
					DebugTWD.Log("Find MAX Loot: " + MaxLoot.CurrencyType + " " + MaxLoot.RewardAmount);
				}
			}

			var heroTokens = LootItemList.Where(x => x.Type == 1);

			if (callRarityTypeCurrent != CallRarityType.Auto)
			{
				lootMaxList = new();
				if (heroTokens != null && heroTokens.Count() > 0)
				{
					lootMaxList.Add(heroTokens.First());
				}
				else if (callRarityTypeCurrent == CallRarityType.All)
				{
					lootMaxList = LootItemList;
				}

				if (MaxLoot != null && !lootMaxList.Contains(MaxLoot))
				{
					lootMaxList.Insert(0, MaxLoot);
				}
				if (lootMaxList.Count == 0) lootMaxList = null;
			}
			else
			{
				if (heroTokens != null && heroTokens.Count() > 0)
				{
					int rewardAmountTokenMax = heroTokens.Max(x => x.RewardAmount);
					lootMaxList = heroTokens.Where(x => x.RewardAmount == rewardAmountTokenMax).ToList();
				}
				else
				{
					int rewardAmountSurvivorMax = LootItemList.Max(x => x.RewardAmount);
					lootMaxList = LootItemList.Where(x => x.RewardAmount == rewardAmountSurvivorMax).ToList();
				}
			}

			lootMaxList ??= LootItemList;
			LootItem lootMaxSingle = lootMaxList.First();

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

		if (lootEntry.RewardedAmount == buttonBySlotNumberCurrent.parsedHeroTokensDropNumberValues.Last())
		{
			RewardHero = lootEntry.RewardedCurrency.ToString();
			RewardAmount = lootEntry.RewardedAmount.ToString();

			DebugTWD.LogError("Reroll for: " + PriorityHeroTokenOrigin + ", Value: " + RewardHero + "|" + RewardAmount, DebugType.Call);
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
		if (lootIndex < 0 || lootIndex >= LootsList.Count)
		{
			//DebugTWD.LogError("Attempt to set loot claimed state with out of bounds phone call loot index.", DebugType.Call);
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
			//DebugTWD.LogError("Attempt to access loot claiming state with out of bounds phone call loot index.", DebugType.Call);
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
				//DebugTWD.LogError("LootsRerollLockingList and LootsList count mismatch!", DebugType.Call);
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
			//DebugTWD.LogError("LootsClaimedTypeList and LootsList count mismatch!", DebugType.Call);
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
			//DebugTWD.LogError("Attempt to access loot claiming state with out of bounds phone call loot index.", DebugType.Call);
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
		//int HeroRarityMax = buttonBySlotNumberCurrent.parsedHeroTokensDropNumberValues != null ? buttonBySlotNumberCurrent.parsedHeroTokensDropNumberValues.Last() : int.Parse(buttonBySlotNumberExpensive.HeroRarityAmounts.Last().Amount);

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
			SurvivorModel generatedSurvivor = Player.SurvivorContainer.CreateRandomSurvivor(0, lootEntry.RewardedStartingLevel, lootEntry.RewardedStartingLevel, lootEntry.RewardedRarityLevel, forceSurvivorClass, null, 1, 0, lootEntry.ExcludeSurvivorClasses, includeGachaOnly: true, lootEntry.Random, SurvivorClass.None, 0, allowLockedClasses);
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
					DebugTWD.LogError("Max Reward Call for PriorityToken: " + PriorityHeroTokenOrigin + ", Value: " + RewardHero + "|" + RewardAmount);
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

	private KeyValuePair<UIInput, RadioCallButton> GetCallButton(int index)
	{
		switch (index)
		{
			case 1: return new(CallButtonExpensiveInput, buttonBySlotNumberExpensive);
			case 2: return new(CallButtonCheapInput, buttonBySlotNumberСheap);
			case 3: return new(CallButtonSimpleInput, buttonBySlotNumberSimple);
			default: return new(null, null);
		}
	}

	public RadioCallButton SetStartCallNumber(int btIndex, int price)
	{
		var btKeyValue = GetCallButton(btIndex);
		string inputText = btKeyValue.Key.value;
		if (string.IsNullOrEmpty(inputText))
		{
			var bt = NewPhonePopup.Instance.GetButtonByPrice(price);
			btKeyValue.Key.value = bt != null ? bt.SlotNumber.ToString() : "";
			return bt;
		}
		return null;
	}

	public void SetStartCallNumbers(int maxNumber, int cheapNumber, int regularNumber)
	{
		buttonBySlotNumberExpensive = SetStartCallNumber(1, maxNumber);
		buttonBySlotNumberСheap = SetStartCallNumber(2, cheapNumber != 0 ? cheapNumber : 40);
		buttonBySlotNumberSimple = SetStartCallNumber(3, regularNumber != 0 ? regularNumber : 15);
	}

	private RadioCallButton GetCallButtonFromInput(UIInput input)
	{
		string error = "";
		if (int.TryParse(input.value, out int result))
		{
			try
			{
				return NewPhonePopup.Instance.GetButtonBySlotNumber(result);
			}
			catch
			{
				if (DataManager.Instance.language == DataManager.Language.Ru)
				{
					error = "Вызова с номером " + result + " не существует";
				}
				else
				{
					error = "Call number " + result + " does not exist";
				}
			}
		}
		else
		{
			if (DataManager.Instance.language == DataManager.Language.Ru)
			{
				error = "Некорректно введен номер. Исправьте " + input.value;
			}
			else
			{
				error = "The number was entered incorrectly. Correct input " + input.value;
			}
		}
		MyTools.OpenAlert(error);
		return null;
	}
}
