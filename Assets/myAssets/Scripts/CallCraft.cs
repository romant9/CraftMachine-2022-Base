using BaseModel;
using BestHTTP.Authentication;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;
using static Client.Tweener.Easing;

public partial class CallCraft : MonoBehaviour
{
	public static CallCraft Instance;

	public UILabel radioExtraAmountLabel;

	public NewPhonePopup _NewPhonePopup;
	public SelectSurvivorsPopup _SelectSurvivorsPopup;

	public List<CallGridItem> CallItemsList { get; set; }
	public List<CallGridItem> CallItemsScenarioList { get; set; }

	//
	public UIScrollView CallScrollView;
	public UITable CallTable;
	public CallGridItem InitCallGridItem; //префаб, скрытый объект

	public UIScrollView TokenScrollView;
	public UITable TokenTable;
	public TokenInfo TokenInfoPrefab;

	public UIScrollView CallScenarioScrollView;
	public UITable CallScenarioTable;
	public CallGridItem InitCallScenarioGridItem; //префаб, скрытый объект
	//

	public int CraftRangeCount = 1;

	public Dictionary<string, ModelRandom> InitDedicatedRandoms { get; set; } //Player random (из content)
	public Dictionary<string, ModelRandom> CallRandomLast { get; set; } //последняя, после крафта
	public Dictionary<string, ModelRandom> SavedDedicatedRandoms { get; set; } //для передачи текущей в CallGridItem до ее изменения
	public Dictionary<string, ModelRandom> MaxRewardDedicatedRandoms { get; set; }

	private PlayerModel Player => DataManager.Instance.Player;

	public bool IsCallFinish;
	public bool IsMultiCallProcess;

	public UIButtonSingleToggleSet CallToggleSet;
	public CallGridItem SelectedCall { get; set; } //выбранный через UIToggle
	public CallGridItem CurrentCall { get; set; } //текущий вызов на этапе создания, перед добавлением в лист
	public bool IsAccepted => CurrentCall != null && CurrentCall.IsAccepted;
	public int CurrentIndex { get; set; }
	public RadioCallButton CurrentCallButton { get; set; }

	public Transform CallTableHat;

	public bool IsMultiCallMode;
	public int multiCallCount = 10;
	public UIInput multiCallInput;

	public UILabel zoom;

	public CallInfo InitCallData { get; set; }
	public CallInfo CurrentCallData { get; set; }

	public List<TokenInfo> TokenItemsList { get; set; }

	public bool IsAddRadios { get; private set; }

	public string PriorityHeroToken { get; private set; } = "None";

	static int ComparisonTransform(Transform x, Transform y) => x.GetComponent<TokenInfo>().tokenAmount.CompareTo(y.GetComponent<TokenInfo>().tokenAmount);

	public Action AddRadiosAction;

	public UILabel CallPriceSummLabel;

	public UIButtonToggleSet CallRaritySet;
	public enum CallRarityType
	{
		Auto,
		Manual,
		All,
		None
	}
	public CallRarityType CallRarityTypeCurrent = CallRarityType.Manual;
	public UIButtonExtended ButtonToCall;

	#region CallBatch
	public bool IsGeneratedScenarioCards { get; set; }
	public bool IsCardsViewed { get; set; }
	public bool IsVisualized { get; set; }
	public UIButtonToggle ShowJackpotToggle;
	public List<CallData> CallDataList;
	#endregion

	#region RouletteLotteryPopup
	public bool IsAutoDraw { get; set; }
	public bool IsQuickDraw { get; set; } = true;
	public bool SeenPopup { get; set; }
	public string SavedRouletteDataJson { get; set; }
	#endregion

	public string LocalizeCallType(PhoneCallDefinitionType type, DataManager.Language lang)
	{
		switch (type)
		{
			case PhoneCallDefinitionType.BetterChanceOfHero:
				if (lang == DataManager.Language.Ru) return "Лучший шанс героя";
				else return "";
			case PhoneCallDefinitionType.GuaranteedHero:
				if (lang == DataManager.Language.Ru) return "";
				else return "";
			case PhoneCallDefinitionType.BetterChanceOfSurvivor:
				if (lang == DataManager.Language.Ru) return "";
				else return "";
			case PhoneCallDefinitionType.BetterChanceOfMultipleHeroes:
				if (lang == DataManager.Language.Ru) return "";
				else return "";
			default:
				if (lang == DataManager.Language.Ru) return "";
				else return "";
		}
	}

	public void SetHeroNames(UIInput input)
	{
		PriorityHeroToken = input.value;
	}

	public void SetPriorityHeroToken(string tokenName)
	{
		PriorityHeroToken = tokenName;
	}

	private void Awake()
	{
		Instance = this;
	}

	void Start()
	{
		IsAddRadios = true;
		CurrentIndex = 0;
		CallItemsList = new List<CallGridItem>();
		CallItemsScenarioList = new List<CallGridItem>();
		TokenItemsList = new List<TokenInfo>();

		CallRaritySet.OnChangeDelegate += OnChangeRarityType;
		CallRaritySet.SetInitialToggle(1);
	}

	private void OnChangeRarityType(UIButtonExtended toggle)
	{
		var index = CallRaritySet.GetSelectedIndex();
		CallRarityTypeCurrent = (CallRarityType)index;
		DebugTWD.Log("Set CallRarityType to " + CallRarityTypeCurrent);
	}

	void Update()
	{
	}

	public void SwitchGoldRadios(UIToggle tg)
	{
		IsAddRadios = !tg.value;
	}

	public void SetCurrencyAmount()
	{
		//if (OfflineManager.IsFreeAll) return;

		int radioExtraAmount;
		try
		{
			radioExtraAmount = Convert.ToInt32(radioExtraAmountLabel.text.ToString());
		}
		catch
		{
			radioExtraAmount = 100;
		}

		Player.SetCurrency(CurrencyType.Phone, radioExtraAmount);
		int radios = Player.GetCurrency(CurrencyType.Phone).Value;
		_NewPhonePopup.RadiophonesAmountLabel.text = radios.ToString();
		AddRadiosAction.Invoke();

		_NewPhonePopup.EnableAllCallButtonsByPrice();

		if (!IsAddRadios)
		{
			//Add gold
			Player.SetCurrency(CurrencyType.Diamonds, radioExtraAmount);
			int gold = Player.GetCurrency(CurrencyType.Diamonds).Value;
			_NewPhonePopup.GoldMeterAmountLabel.text = gold.ToString();
		}
	}

	public IEnumerator InitData()
	{
		yield return new WaitUntil(() => OfflineManager.Instance.IsPlayerLoaded);

		InitDedicatedRandoms = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);
		CallRandomLast = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);
	}

	public void Reset()
	{
		SelectedCall = null;
		if (CallItemsList.Count > 0)
		{
			OnClickDelete();
			CalculateHeroTokenQueue();
		}
	}

	public void GoBackToCallList()
	{
		_SelectSurvivorsPopup.Clear();
		_SelectSurvivorsPopup.FinishCall(true); //CardsList.Clear()
		Helpers.DestroyAllChildren(_SelectSurvivorsPopup.ButtonParentTarget.gameObject); //карточки героев (обычно 3)
		_SelectSurvivorsPopup.gameObject.SetActive(false);
		_NewPhonePopup._CallListPanel.SetActive(true);
		_NewPhonePopup.survivorToggle.gameObject.SetActive(true);
		_NewPhonePopup.EnableAllCallButtons(value: true); //все вызовы
	}

	public void GoBackToCallCards()
	{
		//_SelectSurvivorsPopup.Clear();
		//_SelectSurvivorsPopup.FinishCall(true);
		//Helpers.DestroyAllChildren(_SelectSurvivorsPopup.ButtonParentTarget.gameObject);

		if (_SelectSurvivorsPopup.ButtonParentTarget.childCount == 0)
		{
			SelectedCall.OnCallClick();
		}

		_NewPhonePopup.survivorToggle.gameObject.SetActive(false);
		_NewPhonePopup._CallListPanel.SetActive(false);
		_SelectSurvivorsPopup.gameObject.SetActive(true);

		//_NewPhonePopup.survivorToggle.gameObject.SetActive(true);
		//_NewPhonePopup.EnableAllCallButtons(value: true);
	}

	private void CalculateDedicatedRandom(bool IsMulti)
	{
		if (!IsMulti && SelectedCall != null)
		{
			DebugTWD.LogWarning("IsSelected");
			Player.LootManager.DedicatedRandoms = SetDedicatedRandom(SelectedCall.CallRandomCurrent);
		}
		else
		{
			DebugTWD.LogWarning("IsMulti or CallRandomLast");
			Player.LootManager.DedicatedRandoms = SetDedicatedRandom(CallRandomLast);
		}

		SavedDedicatedRandoms = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);
	}

	public IEnumerator OnClickMultiCallJackpot(List<CallGridItem> CallsJackpot)
	{
		IsScenarioTableApplying = true;

		RadioCallButton currentCallButton = CallsJackpot.First().CallButton;

		InitCallData ??= new CallInfo(currentCallButton);

		if (CallItemsList.Count > 0)
		{
			CurrentIndex = CallItemsList.Last().CallIndex + 1;
		}
		else
		{
			CurrentIndex = 0;
			CallRandomLast = SetDedicatedRandom(InitDedicatedRandoms);
		}

		foreach (var jGridItem in CallsJackpot)
		{
			IsCallFinish = false;

			_SelectSurvivorsPopup.Clear();
			Player.PhoneCall.LootsList.Clear();
			Helpers.DestroyAllChildren(_SelectSurvivorsPopup.ButtonParentTarget.gameObject);

			SavedDedicatedRandoms = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);

			List<List<bool>> LootsRerollLockingList = jGridItem.AcceptIndexes.Select(x => x.Select(y => y.value).ToList()).ToList();
			CurrentCall = Instantiate(InitCallGridItem, CallTable.transform);
			CurrentCall.gameObject.name += "_" + CurrentIndex;

			CurrentCall.Init(true);

			currentCallButton = jGridItem.CallButton;
			CurrentCallButton = currentCallButton;

			TWDModelResult resultCall = Player.PhoneCall.Call(currentCallButton.dropType, currentCallButton.SlotNumber);
			if (resultCall == TWDModelResult.NotEnoughCurrency)
			{
				MyTools.OpenAlert("Not Enough Phones. Need to check ON \"FREE ALL\" flag in Settings panel");
				IsCallFinish = true;
				IsScenarioTableApplying = false;
				yield break;
			}
			_NewPhonePopup.OnPhoneCallMade(resultCall);

			yield return _SelectSurvivorsPopup.CardsList != null && _SelectSurvivorsPopup.CardsList.Count > 0;

			int rewardAmountCurrent = 0;
			int rewardType = 0;
			List<LootItem> lootMaxList = null;
			List<LootItem> LootItemList = new List<LootItem>();

			var parsedHeroTokensDropNumberValues = currentCallButton.parsedHeroTokensDropNumberValues ?? currentCallButton.HeroRarityAmounts.Select(x => int.Parse(x.Amount));
			int HeroRarityMin = parsedHeroTokensDropNumberValues.First();
			int HeroRarityMax = parsedHeroTokensDropNumberValues.Last();

			var callType = GameManager.Instance.playerModel.PhoneCall.CallType;
			DebugTWD.Log("CallType " + callType + " , slot :" + currentCallButton.SlotNumber);
			int currentCallRerollIndex = -1;
			if (callType == PhoneCallDefinitionType.GuaranteedHero || (callType == PhoneCallDefinitionType.None && currentCallButton.SlotNumber > 2))
			{
				//_SelectSurvivorsPopup.RerollIndex = 0;
				//var rerolls = _SelectSurvivorsPopup.RerollIndex;
				while (Player.PhoneCall.NumRerolls > 0)
				{
					for (int j = 0; j < _SelectSurvivorsPopup.CardsList.Count; j++)
					{
						DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrencyType = Player.PhoneCall.LootsList[j].DropCurrencyType;

						if (dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor || dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.ClassToken)
						{
							rewardType = 0;
							rewardAmountCurrent = Player.PhoneCall.LootsList[j].GeneratedSurvivor.DemoteTokens;
						}
						else
						{
							rewardAmountCurrent = HelpersUI.GetActualRewardValue(currentCallButton, Player.PhoneCall.LootsList[j].RewardedAmount);
							rewardType = 1;
						}

						LootItemList.Add(new LootItem()
						{
							Index = j,
							Type = rewardType,
							RewardAmount = rewardAmountCurrent,
							CurrencyType = Player.PhoneCall.LootsList[j].RewardedCurrency
						});
					}

					if (Player.PhoneCall.NumRerolls > 0)
					{
						currentCallRerollIndex++;
						lootMaxList = new();
						for (int j = 0; j < LootsRerollLockingList[currentCallRerollIndex].Count; j++)
						{
							if (LootsRerollLockingList[currentCallRerollIndex][j])
							{
								lootMaxList.Add(LootItemList[j]);
							}
						}						
					}
					else
					{
						lootMaxList = LootItemList;
					}

					if (lootMaxList != null)
					{
						foreach (var loot in lootMaxList)
						{
							if (!Player.PhoneCall.IsLootLockedForReroll(loot.Index))
							{
								_SelectSurvivorsPopup.OnClickLockLoot(loot.Index);
							}
							yield return null;
						}
					}

					if (_SelectSurvivorsPopup.RerollIndex < CurrentCall.AcceptIndexesGroups.Count) 
					{
						CurrentCall.AcceptIndexesGroups[_SelectSurvivorsPopup.RerollIndex].gameObject.SetActive(true);
					}
					else
					{
						DebugTWD.LogError("RerollIndex > AcceptIndexesGroups.Count");
					}
					_SelectSurvivorsPopup.SelectLootEntry(null);

					TWDModelResult resultReroll = Player.PhoneCall.RerollCall();
					if (resultReroll == TWDModelResult.OK)
					{
						_SelectSurvivorsPopup.RerollsLeft = Player.PhoneCall.NumRerolls;
						_SelectSurvivorsPopup.UpdateManagePanel();
						_SelectSurvivorsPopup.ReOpenAfterReroll();
						_SelectSurvivorsPopup.RerollIndex++;
					}

					yield return null;
				}

				List<int> CardsListIndexes = _SelectSurvivorsPopup.CardsList.Select(x => x.GetLootEntryIndex()).ToList();
				foreach (var index in CardsListIndexes)
				{
					//_SelectSurvivorsPopup.OnClickAcceptSelectedLoot(index);

					CurrentCall.AcceptIndexesGroups.First().gameObject.SetActive(true);

					CurrentCall.tokenValues[index].transform.parent.gameObject.SetActive(true);

					LootEntry loot = _SelectSurvivorsPopup.CardsList[index].GetLootEntry();

					CurrentCall.LootEntryList.Add(loot);
					string spriteName;
					if (loot.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
					{
						spriteName = HelpersGfx.GetCurrencyIconName(loot.RewardedCurrency);
						rewardAmountCurrent = HelpersUI.GetActualRewardValue(currentCallButton, loot.RewardedAmount);
					}
					else
					{
						spriteName = _SelectSurvivorsPopup.CardsList[index].GetComponent<SurvivorCard>().surviorCardTokenAccept.classIconSprite.spriteName;
						rewardAmountCurrent = loot.GeneratedSurvivor.DemoteTokens;
					}
					HelpersUI.SetSprite(CurrentCall.tokenSprites[index], spriteName);
					HelpersUI.SetContentToLabel(CurrentCall.tokenValues[index], rewardAmountCurrent.ToString());
				}

				DebugTWD.Log("OnClickAccept rerolled call" + CurrentCall.LootEntryList.Count);
			}
			else
			{
				for (int j = 0; j < _SelectSurvivorsPopup.CardsList.Count; j++)
				{
					DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrencyType = Player.PhoneCall.LootsList[j].DropCurrencyType;

					if (dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor || dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.ClassToken)
					{
						rewardType = 0;
						rewardAmountCurrent = Player.PhoneCall.LootsList[j].GeneratedSurvivor.DemoteTokens;
					}
					else
					{
						rewardAmountCurrent = HelpersUI.GetActualRewardValue(currentCallButton, Player.PhoneCall.LootsList[j].RewardedAmount);
						rewardType = 1;
					}
					LootItemList.Add(new LootItem()
					{
						Index = j,
						Type = rewardType,
						RewardAmount = rewardAmountCurrent,
						CurrencyType = Player.PhoneCall.LootsList[j].RewardedCurrency
					});
				}

				currentCallRerollIndex++;
				lootMaxList = new();
				for (int j = 0; j < LootsRerollLockingList[currentCallRerollIndex].Count; j++)
				{
					if (LootsRerollLockingList[currentCallRerollIndex][j])
					{
						lootMaxList.Add(LootItemList[j]);
					}
				}

				lootMaxList ??= LootItemList;
				var lootMaxSingle = lootMaxList.First();
				int index = lootMaxSingle.Index;

				//_SelectSurvivorsPopup.OnClickAcceptSelectedLoot(index);

				CurrentCall.tokenValues[index].transform.parent.gameObject.SetActive(true);
				CurrentCall.AcceptIndexesGroups.First().gameObject.SetActive(true);
				CurrentCall.AcceptIndexes.First()[index].Set(true);

				var loot = _SelectSurvivorsPopup.CardsList[index].GetLootEntry();
				CurrentCall.LootEntryList.Add(loot);

				if (lootMaxSingle.Type == 0)
				{
					var spriteName = _SelectSurvivorsPopup.CardsList[index].GetComponent<SurvivorCard>().surviorCardTokenAccept.classIconSprite.spriteName;
					HelpersUI.SetSprite(CurrentCall.tokenSprites[index], spriteName);
				}
				else
				{
					HelpersUI.SetSprite(CurrentCall.tokenSprites[index], HelpersGfx.GetCurrencyIconName(loot.RewardedCurrency));
				}
				HelpersUI.SetContentToLabel(CurrentCall.tokenValues[index], lootMaxSingle.RewardAmount.ToString());

				DebugTWD.Log("OnClickAccept single call" + CurrentCall.LootEntryList.Count);
			}

			yield return null;

			CurrentCall.gameObject.SetActive(true);

			//CurrentCall.SetData(CurrentIndex, CurrentCallData, SavedDedicatedRandoms);

			CurrentCall.CallIndex = CurrentIndex;
			CurrentCall.CallPrice = currentCallButton.GetCallPrice();
			CurrentCall.CallType = currentCallButton.SlotNumber;
			CurrentCall.CallInfoStr = currentCallButton.GetInfo();
			CurrentCall.CallButton = currentCallButton;
			CurrentCall.CallRandomCurrent = SavedDedicatedRandoms;

			CurrentCall.UpdateUI();

			CallItemsList.Add(CurrentCall);

			//CalculateHeroTokenQueue();

			CallTable.Reposition();
			CallScrollView.ResetPositionInverted();

			var buttonToggle = CurrentCall.GetComponent<UIButtonToggle>();
			CallToggleSet.Buttons.Add(buttonToggle);
			buttonToggle.SetClickCallback(CallToggleSet.OnClick);

			CallRandomLast = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);

			CurrentIndex++;

			yield return null;

			IsCallFinish = true;
		}
		if (SelectedCall == null) CurrentCall.OnCallClick();
		else SelectedCall.OnCallClick();
		IsScenarioTableApplying = false;

		StartCoroutine(SetTokenListData());
	}

	public IEnumerator OnClickMultiCall(List<CallGridItem> CallsForChange = null)
	{
		DebugTWD.Log("Start Multi Calling");

		IsMultiCallProcess = true;

		bool IsOnlyRecalculate = CallsForChange != null;
		bool IsSelected = SelectedCall != null;

		if (!IsSelected && !IsOnlyRecalculate)
		{
			if (CallItemsList.Count > 0)
			{
				CurrentIndex = CallItemsList.Last().CallIndex + 1;
			}
			else
			{
				CurrentIndex = 0;
				CallRandomLast = SetDedicatedRandom(InitDedicatedRandoms);
			}
		}
		else
		{
			if (IsOnlyRecalculate)
			{
				CurrentIndex = SelectedCall.CallIndex + 1;
			}
			else
			{
				CurrentIndex = SelectedCall.CallIndex;
				CallRandomLast = SetDedicatedRandom(SelectedCall.CallRandomCurrent);
			}
		}

		int countNew;
		bool isOverAddMode;

		if (IsOnlyRecalculate)
		{
			countNew = CallsForChange.Count;
			isOverAddMode = true;
		}
		else
		{
			multiCallCount = SetMultiCallCount();

			int countMax = Mathf.RoundToInt(Player.GetCurrency(CurrencyType.Phone).Value / (InitCallData.Price > 0 ? InitCallData.Price : multiCallCount + 1));
			countNew = countMax < multiCallCount && !OfflineManager.IsFreeAll ? countMax : multiCallCount;
			int countOver = CallItemsList.Count > 0 ? CallItemsList.Count - CurrentIndex - countNew : 0;

			isOverAddMode = countOver > 0;
			if (isOverAddMode)
			{
				CallsForChange = CallItemsList.GetRange(CurrentIndex + countNew, countOver);
				countNew += countOver;
			}

			DebugTWD.Log("MultiCall Count : " + countNew);

			if (countMax < 1)
			{
				DebugTWD.LogWarning("Not enough money to call");
				yield break;
			}
		}

		for (int i = 0; i < countNew; i++)
		{
			_SelectSurvivorsPopup.Clear();
			Player.PhoneCall.LootsList.Clear();
			Helpers.DestroyAllChildren(_SelectSurvivorsPopup.ButtonParentTarget.gameObject);

			CalculateDedicatedRandom(IsMulti: true);

			IsCallFinish = false;
			bool IsModified = CallItemsList.Count > CurrentIndex;

			CurrentCallData = InitCallData.CopyOf();
			List<List<bool>> LootsRerollLockingList = new List<List<bool>>();
			if (IsModified)
			{
				CurrentCall = CallItemsList[CurrentIndex];
				LootsRerollLockingList = CurrentCall.AcceptIndexes.Select(x => x.Select(y => y.value).ToList()).ToList();
				
				if (isOverAddMode && CallsForChange.Contains(CurrentCall))
				{
					CurrentCallData = new CallInfo()
					{
						Price = CurrentCall.CallPrice,
						CurrentTypeIndex = CurrentCall.CallType,
						CallButton = _NewPhonePopup.GetButtonBySlotNumber(CurrentCall.CallType),
						CurrentCallInfo = CurrentCall.CallInfoStr
					};
				}
			}
			else
			{
				CurrentCall = Instantiate(InitCallGridItem, CallTable.transform);
				CurrentCall.gameObject.name += "_" + CurrentIndex;
			}

			CurrentCall.Init(true);

			CurrentCallButton = CurrentCallData.CallButton;

			TWDModelResult resultCall = Player.PhoneCall.Call(CurrentCallButton.dropType, CurrentCallButton.SlotNumber);
			_NewPhonePopup.OnPhoneCallMade(resultCall);

			yield return _SelectSurvivorsPopup.CardsList != null && _SelectSurvivorsPopup.CardsList.Count > 0;

			int rewardAmountCurrent = 0;
			int rewardType = 0;
			List<LootItem> lootMaxList = null;
			List<LootItem> LootItemList = new List<LootItem>();

			int HeroRarityMin = CurrentCallButton.parsedHeroTokensDropNumberValues != null ? CurrentCallButton.parsedHeroTokensDropNumberValues.First() : int.Parse(CurrentCallButton.HeroRarityAmounts.First().Amount);
			int HeroRarityMax = CurrentCallButton.parsedHeroTokensDropNumberValues != null ? CurrentCallButton.parsedHeroTokensDropNumberValues.Last() : int.Parse(CurrentCallButton.HeroRarityAmounts.Last().Amount);

			var callType = GameManager.Instance.playerModel.PhoneCall.CallType;
			DebugTWD.Log("CallType " + callType + " , slot :" + CurrentCallButton.SlotNumber);
			int currentCallRerollIndex = -1;
			LootItem MaxLoot = null;
			if (callType == PhoneCallDefinitionType.GuaranteedHero || (callType == PhoneCallDefinitionType.None && CurrentCallButton.SlotNumber > 2))
			{
				while (Player.PhoneCall.NumRerolls > 0)
				{
					for (int j = 0; j < _SelectSurvivorsPopup.CardsList.Count; j++)
					{
						DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrencyType = Player.PhoneCall.LootsList[j].DropCurrencyType;

						if (dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor || dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.ClassToken)
						{
							rewardType = 0;
							rewardAmountCurrent = Player.PhoneCall.LootsList[j].GeneratedSurvivor.DemoteTokens;
						}
						else
						{
							rewardAmountCurrent = HelpersUI.GetActualRewardValue(CurrentCallButton, Player.PhoneCall.LootsList[j].RewardedAmount);
							rewardType = 1;
						}

						LootItemList.Add(new LootItem()
						{
							Index = j,
							Type = rewardType,
							RewardAmount = rewardAmountCurrent,
							CurrencyType = Player.PhoneCall.LootsList[j].RewardedCurrency
						});

						if (rewardAmountCurrent == HeroRarityMax)
						{
							MaxLoot = LootItemList.Last();
							DebugTWD.Log("Find MAX Loot: " + MaxLoot.CurrencyType + " " + MaxLoot.RewardAmount);
						}
					}

					if (Player.PhoneCall.NumRerolls > 0)
					{
						if (CallRarityTypeCurrent != CallRarityType.Auto)
						{
							currentCallRerollIndex++;
							lootMaxList = new();
							if (CallRarityTypeCurrent == CallRarityType.Manual && LootsRerollLockingList.Count > 0 && currentCallRerollIndex < LootsRerollLockingList.Count)
							{
								for (int j = 0; j < LootsRerollLockingList[currentCallRerollIndex].Count; j++)
								{
									if (LootsRerollLockingList[currentCallRerollIndex][j])
									{
										lootMaxList.Add(LootItemList[j]);
									}
								}
							}
							else if (CallRarityTypeCurrent == CallRarityType.All)
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
							int rewardAmountTokenMax;
							if (callType == PhoneCallDefinitionType.None)
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
						}
						if (lootMaxList.Count == 0) lootMaxList = null;
					}
					else
					{
						lootMaxList = LootItemList;
					}

					if (lootMaxList != null)
					{
						foreach (var loot in lootMaxList)
						{
							if (!Player.PhoneCall.IsLootLockedForReroll(loot.Index))
							{
								_SelectSurvivorsPopup.OnClickLockLoot(loot.Index);
							}
							yield return null;
						}
					}

					CurrentCall.AcceptIndexesGroups[_SelectSurvivorsPopup.RerollIndex].gameObject.SetActive(true);
					_SelectSurvivorsPopup.SelectLootEntry(null);

					TWDModelResult resultReroll = Player.PhoneCall.RerollCall();
					if (resultReroll == TWDModelResult.OK)
					{
						_SelectSurvivorsPopup.RerollsLeft = Player.PhoneCall.NumRerolls;
						_SelectSurvivorsPopup.UpdateManagePanel();
						_SelectSurvivorsPopup.ReOpenAfterReroll();
						_SelectSurvivorsPopup.RerollIndex++;
					}

					yield return null;
				}

				List<int> CardsListIndexes = _SelectSurvivorsPopup.CardsList.Select(x => x.GetLootEntryIndex()).ToList();
				foreach (var index in CardsListIndexes)
				{
					//_SelectSurvivorsPopup.OnClickAcceptSelectedLoot(index);

					CurrentCall.AcceptIndexesGroups.First().gameObject.SetActive(true);

					CurrentCall.tokenValues[index].transform.parent.gameObject.SetActive(true);

					LootEntry loot = _SelectSurvivorsPopup.CardsList[index].GetLootEntry();

					CurrentCall.LootEntryList.Add(loot);

					string spriteName;
					if (loot.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
					{
						spriteName = HelpersGfx.GetCurrencyIconName(loot.RewardedCurrency);
						rewardAmountCurrent = HelpersUI.GetActualRewardValue(CurrentCallButton, loot.RewardedAmount);
					}
					else
					{
						spriteName = _SelectSurvivorsPopup.CardsList[index].GetComponent<SurvivorCard>().surviorCardTokenAccept.classIconSprite.spriteName;
						rewardAmountCurrent = loot.GeneratedSurvivor.DemoteTokens;
					}
					HelpersUI.SetSprite(CurrentCall.tokenSprites[index], spriteName);
					HelpersUI.SetContentToLabel(CurrentCall.tokenValues[index], rewardAmountCurrent.ToString());
				}

				DebugTWD.Log("OnClickAccept rerolled call" + CurrentCall.LootEntryList.Count);
			}
			else
			{
				for (int j = 0; j < _SelectSurvivorsPopup.CardsList.Count; j++)
				{
					DropCurrenciesProbabilitiesDefinition.DropCurrency dropCurrencyType = Player.PhoneCall.LootsList[j].DropCurrencyType;

					if (dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.Survivor || dropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.ClassToken)
					{
						rewardType = 0;
						rewardAmountCurrent = Player.PhoneCall.LootsList[j].GeneratedSurvivor.DemoteTokens;
					}
					else
					{
						rewardAmountCurrent = HelpersUI.GetActualRewardValue(CurrentCallButton, Player.PhoneCall.LootsList[j].RewardedAmount);
						rewardType = 1;
					}
					LootItemList.Add(new LootItem()
					{
						Index = j,
						Type = rewardType,
						RewardAmount = rewardAmountCurrent,
						CurrencyType = Player.PhoneCall.LootsList[j].RewardedCurrency
					});

					if (rewardAmountCurrent == HeroRarityMax)
					{
						MaxLoot = LootItemList.Last();
						DebugTWD.Log("Find MAX Loot: " + MaxLoot.CurrencyType + " " + MaxLoot.RewardAmount);
					}
				}

				var heroTokens = LootItemList.Where(x => x.Type == 1);

				if (CallRarityTypeCurrent != CallRarityType.Auto)
				{
					currentCallRerollIndex++;
					lootMaxList = new();
					if (CallRarityTypeCurrent == CallRarityType.Manual && LootsRerollLockingList.Count > 0 && currentCallRerollIndex < LootsRerollLockingList.Count)
					{
						for (int j = 0; j < LootsRerollLockingList[currentCallRerollIndex].Count; j++)
						{
							if (LootsRerollLockingList[currentCallRerollIndex][j])
							{
								lootMaxList.Add(LootItemList[j]);
							}
						}
					}
					else
					{
						if (heroTokens != null && heroTokens.Count() > 0)
						{
							lootMaxList.Add(heroTokens.First());
						}
						else if (CallRarityTypeCurrent == CallRarityType.All)
						{
							lootMaxList = LootItemList;
						}
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
				var lootMaxSingle = lootMaxList.First();
				int index = lootMaxSingle.Index;

				//_SelectSurvivorsPopup.OnClickAcceptSelectedLoot(index);

				CurrentCall.tokenValues[index].transform.parent.gameObject.SetActive(true);
				CurrentCall.AcceptIndexesGroups.First().gameObject.SetActive(true);
				CurrentCall.AcceptIndexes.First()[index].Set(true);

				var loot = _SelectSurvivorsPopup.CardsList[index].GetLootEntry();
				CurrentCall.LootEntryList.Add(loot);

				if (lootMaxSingle.Type == 0)
				{
					var spriteName = _SelectSurvivorsPopup.CardsList[index].GetComponent<SurvivorCard>().surviorCardTokenAccept.classIconSprite.spriteName;
					//var spriteName = HelpersGfx.GetCurrencyIconName(SurvivorModel.GetSurvivorTraitUpgradeCurrencyType(survivor));
					HelpersUI.SetSprite(CurrentCall.tokenSprites[index], spriteName);
				}
				else
				{
					HelpersUI.SetSprite(CurrentCall.tokenSprites[index], HelpersGfx.GetCurrencyIconName(loot.RewardedCurrency));
				}
				HelpersUI.SetContentToLabel(CurrentCall.tokenValues[index], lootMaxSingle.RewardAmount.ToString());

				DebugTWD.Log("OnClickAccept single call" + CurrentCall.LootEntryList.Count);
			}

			yield return null;

			CurrentCall.gameObject.SetActive(true);

			CurrentCall.SetData(CurrentIndex, CurrentCallData, SavedDedicatedRandoms);

			CurrentCall.UpdateUI();

			if (!IsModified)
			{
				CallItemsList.Add(CurrentCall);

				CallTable.Reposition();
				CallScrollView.ResetPositionInverted();

				var buttonToggle = CurrentCall.GetComponent<UIButtonToggle>();
				CallToggleSet.Buttons.Add(buttonToggle);
				buttonToggle.SetClickCallback(CallToggleSet.OnClick);
			}
			else
			{
				CallItemsList[CurrentIndex] = CurrentCall;
			}

			CallRandomLast = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);

			if (IsOnlyRecalculate || IsSelected)
			{
				Player.SetCurrency(CurrencyType.Phone, CurrentCall.CallPrice);

				int radios = Player.GetCurrency(CurrencyType.Phone).Value;
				_NewPhonePopup.RadiophonesAmountLabel.text = radios.ToString();
			}

			CurrentIndex++;

			yield return null;

			IsCallFinish = true;
		}

		DebugTWD.Log("MultiCall finished succesfully");
		_SelectSurvivorsPopup.ManagePanel.AcceptButton.isEnabled = false;

		if (IsSelected && CallToggleSet.Buttons.Count > 0)
		{
			var tg = CallToggleSet.Buttons.First(x => x.IsToggled);
			int index = CallToggleSet.Buttons.IndexOf(tg);
			SelectedCall = CallItemsList[index];
			CurrentIndex = SelectedCall.CallIndex;
			CurrentCallData = InitCallData.CopyOf();
			CurrentCallButton = CurrentCallData.CallButton;
		}

		yield return null;
		IsMultiCallProcess = false;

		if (SelectedCall == null) CurrentCall.OnCallClick();
		else SelectedCall.OnCallClick();

		StartCoroutine(SetTokenListData());
	}

	public void OnClickCall()
	{
		DebugTWD.Log("Start Single Calling");
		IsCallFinish = false;

		CalculateDedicatedRandom(IsMulti: false);
		StartCoroutine(CraftCall());
	}

	public IEnumerator CraftCall()
	{
		bool IsCallNew = SelectedCall == null;

		if (!IsCallNew)
		{
			CurrentIndex = SelectedCall.CallIndex;
			CurrentCall = SelectedCall;
			CurrentCall.IsAccepted = false;
		}
		else
		{
			CurrentCall = Instantiate(InitCallGridItem, CallTable.transform);
			CurrentCall.gameObject.name += "_" + CurrentIndex;
		}

		CurrentCall.Init(false);

		yield return new WaitUntil(() => CurrentCall != null && IsCallFinish);

		DebugTWD.Log("Call Finish, Update Call Index " + CurrentIndex);

		CurrentCallData = InitCallData.CopyOf();

		CurrentCall.gameObject.SetActive(true);

		CurrentCall.SetData(CurrentIndex, CurrentCallData, SavedDedicatedRandoms);

		CurrentCall.UpdateUI();

		CallRandomLast = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);

		if (IsCallNew)
		{
			CallItemsList.Add(CurrentCall);

			CallTable.Reposition();

			CallScrollView.ResetPositionInverted();

			var buttonToggle = CurrentCall.GetComponent<UIButtonToggle>();
			CallToggleSet.Buttons.Add(buttonToggle);
			buttonToggle.SetClickCallback(CallToggleSet.OnClick);
		}
		else
		{
			CallItemsList[CurrentIndex] = CurrentCall;

			List<CallGridItem> CallsForChange = CallItemsList.GetRange(CurrentIndex + 1, CallItemsList.Count - (CurrentIndex + 1));

			if (CallsForChange != null && CallsForChange.Count > 0)
			{
				SelectedCall = CurrentCall;

				StartCoroutine(OnClickMultiCall(CallsForChange));
				yield break;
			}
		}

		CurrentIndex++;

		if (!IsCallNew)
		{
			Player.SetCurrency(CurrencyType.Phone, CurrentCall.CallPrice);

			int radios = Player.GetCurrency(CurrencyType.Phone).Value;
			_NewPhonePopup.RadiophonesAmountLabel.text = radios.ToString();
		}

		yield return null;

		StartCoroutine(SetTokenListData());

		IsCallFinish = false;
	}

	public IEnumerator SetTokenListData()
	{
		DebugTWD.Log("Recalculate tokens count");
		if (TokenItemsList.Count > 0)
		{
			var count = TokenItemsList.Count;
			for (int j = 0; j < count; j++)
			{
				var item = TokenItemsList[j];

				if (item != null) Destroy(item.gameObject);
				yield return null;
			}
		}
		TokenItemsList.Clear();

		yield return null;

		if (CallItemsList.Count == 0)
		{
			CallPriceSummLabel.text = "0";
			yield break;
		}

		int price = 0;
		foreach (var item in CallItemsList)
		{
			price += item.CallPrice;
			int index = -1;
			foreach (var sprite in item.tokenSprites)
			{
				index++;
				TokenInfo tokenInfo = TokenItemsList.FirstOrDefault(x => x.sprite.spriteName == sprite.spriteName);
				string tokenValuesString = item.tokenValues[index].text;
				int tokenAmount = int.Parse(tokenValuesString);
				if (tokenAmount == 0) continue;

				if (tokenInfo == null)
				{
					tokenInfo = Instantiate(TokenInfoPrefab, TokenTable.transform);
					tokenInfo.gameObject.SetActive(true);
					tokenInfo.tokenAmount = tokenAmount;
					tokenInfo.gameObject.name = tokenAmount.ToString("0000") + "_" + tokenInfo.gameObject.name;
					tokenInfo.label.text = "(1)" + tokenValuesString;
					HelpersUI.SetSprite(tokenInfo.sprite, sprite.spriteName);
					tokenInfo.repeatCount = 1;

					TokenItemsList.Add(tokenInfo);
				}
				else
				{
					int tokenStartValue = tokenInfo.tokenAmount;

					int summ = tokenStartValue + tokenAmount;
					tokenInfo.repeatCount++;
					tokenInfo.tokenAmount = summ;
					string newName = tokenInfo.gameObject.name.Split('_')[1];
					tokenInfo.gameObject.name = summ.ToString("0000") + "_" + newName;
					tokenInfo.label.text = "(" + tokenInfo.repeatCount + ")" + summ.ToString();
				}
			}
		}

		CallPriceSummLabel.text = price.ToString();
		//static int comparisonToken(TokenInfo x, TokenInfo y) => x.tokenAmount.CompareTo(y.tokenAmount);
		//TokenItemsList.StableSort(comparisonToken);

		yield return null;

		TokenTable.onCustomSort = ComparisonTransform;
		TokenTable.Reposition();

		TokenScrollView.ResetPositionInverted();

		int maxValue = CallItemsList.Select(x => x.SummTokenValues).Max();
		CallGridItem callMaxOldItem = CallItemsList.FirstOrDefault(x => x.IsMaxSumm == true);
		CallGridItem callMaxItem = CallItemsList.FirstOrDefault(x => x.SummTokenValues == maxValue);
		if (callMaxOldItem != null)
		{
			callMaxOldItem.UpdateLabel(false);
		}
		if (callMaxItem != null)
		{
			callMaxItem.UpdateLabel(true);
		}
	}

	public void OnClickDeleteLast()
	{
		int index = CallItemsList.Count;
		if (!OfflineManager.IsFreeAll)
		{
			int price = InitCallData.Price;
			Player.SetCurrency(CurrencyType.Phone, price);
			int radios = Player.GetCurrency(CurrencyType.Phone).Value;
			_NewPhonePopup.RadiophonesAmountLabel.text = radios.ToString();
		}
		if (Player.PhoneCall.LootsClaimedTypeList != null || Player.PhoneCall.LootsRerollLockingList != null)
		{
			Player.PhoneCall.LootsRerollLockingList = null;
			Player.PhoneCall.LootsClaimedTypeList = null;

			if (index > 0)
			{
				CurrentIndex = index;
				Player.LootManager.DedicatedRandoms = SetDedicatedRandom(CallRandomLast);
			}
			else
			{
				Player.LootManager.DedicatedRandoms = SetDedicatedRandom(InitDedicatedRandoms);
				CurrentIndex = 0;
			}
			CallRandomLast = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);

			SelectedCall = null;

			OnClickDeleteUI();

			CalculateHeroTokenQueue();
		}
		GoBackToCallList();
	}

	public void OnClickDelete()
	{
		DebugTWD.Log("Delete Calls!");

		bool IsDeleteAll = SelectedCall == null;
		int index = 0;

		if (!IsDeleteAll)
		{
			index = CallItemsList.IndexOf(SelectedCall);
			if (index < 0) index = 0;
			if (index > 0)
			{
				CurrentIndex = index;
				Player.LootManager.DedicatedRandoms = SetDedicatedRandom(CallItemsList[index - 1].CallRandomCurrent);
				CallRandomLast = CallItemsList[index].CallRandomCurrent;
			}
			else
			{
				Player.LootManager.DedicatedRandoms = SetDedicatedRandom(InitDedicatedRandoms);
				CallRandomLast = Player.LootManager.DedicatedRandoms;
				CurrentIndex = 0;
			}
		}
		else
		{
			Player.LootManager.DedicatedRandoms = SetDedicatedRandom(InitDedicatedRandoms);
			CallRandomLast = Player.LootManager.DedicatedRandoms;
			CurrentIndex = 0;
		}

		if (index < 0) index = 0;
		List<CallGridItem> BadgesForDelete = CallItemsList.GetRange(index, CallItemsList.Count - index);

		for (int i = 0; i < BadgesForDelete.Count; i++)
		{
			var item = BadgesForDelete[i];
			if (!OfflineManager.IsFreeAll)
			{
				Player.SetCurrency(CurrencyType.Phone, item.CallPrice);
				foreach (var loot in item.LootEntryList)
				{
					int RewardedAmount = loot.RewardedAmount;
					if (loot.DropCurrencyType == DropCurrenciesProbabilitiesDefinition.DropCurrency.HeroToken)
					{
						RewardedAmount = HelpersUI.GetActualRewardValue(CurrentCallButton, loot.RewardedAmount);
					}

					Player.SetCurrency(loot.RewardedCurrency, -RewardedAmount);
				}
			}

			var buttonToggle = item.GetComponent<UIButtonToggle>();
			CallToggleSet.Buttons.Remove(buttonToggle);
			CallItemsList.Remove(item);
			Destroy(item.gameObject);
		}

		int radios = Player.GetCurrency(CurrencyType.Phone).Value;
		_NewPhonePopup.RadiophonesAmountLabel.text = radios.ToString();

		SelectedCall = null;

		CalculateHeroTokenQueue();

		StartCoroutine(SetTokenListData());

		OnClickDeleteUI();

		StartCoroutine(ResetPositionInverted());
	}

	public IEnumerator ResetPositionInverted()
	{
		yield return new WaitForEndOfFrame();
		CallTable.Reposition();
		CallScrollView.ResetPositionInverted();
	}

	public void OnClickDeleteUI()
	{
		string textTitle = "";
		string textButton;

		if (DataManager.Instance.language == DataManager.Language.Ru)
		{
			textButton = "Новый вызов";
		}
		else
		{
			textButton = "Next Call";
		}

		_NewPhonePopup.CallChangeLabel.text = textTitle;
		_SelectSurvivorsPopup.ManagePanel.DeleteButton.isEnabled = false;
		_NewPhonePopup.ButtonDelete.isEnabled = false;
		_SelectSurvivorsPopup.ManagePanel.NextButtonLabel.text = textButton;
	}

	//масштаб таблицы вызовов
	public void ScaleSetValue(UIScrollBar scrollBar)
	{
		float scale = scrollBar.value / 2 + .5f;
		zoom.text = (Math.Round(scale * 100)).ToString();
		ScaleGrid(scale);
	}

	public void SetSatetButtonToCall()
	{
		ButtonToCall.isEnabled = SelectedCall != null;
	}

	public void OnClickCallItem(CallGridItem item, bool IsSelected)
	{
		SelectedCall = IsSelected ? item : null;
		if (IsSelected) CurrentCallButton = SelectedCall.CallButton;
		_SelectSurvivorsPopup.IsSelectedMode = IsSelected;
		SetSatetButtonToCall();

		if (IsSelected) DebugTWD.Log("IsSelected: " + item.CallIndex);
		string textTitle;
		string textButtonRu = !IsSelected ? "Новый вызов" : "Изменить вызов";
		string textButtonEn = !IsSelected ? "Next Call" : "Change Call";
		if (DataManager.Instance.language == DataManager.Language.Ru)
		{
			textTitle = !IsSelected ? "" : "Вызов " + (item.CallIndex + 1) + "(Тип " + item.CallType + ") - Изменить!";
		}
		else
		{
			textTitle = !IsSelected ? "" : "Call " + (item.CallIndex + 1) + "(Type " + item.CallType + ") - Change!";
		}

		_NewPhonePopup.CallChangeLabel.text = textTitle;

		_SelectSurvivorsPopup.ManagePanel.OnClickCallItem(IsSelected, textButtonEn, textButtonRu);

		_NewPhonePopup.ButtonDelete.isEnabled = IsSelected;

		if (IsSelected)
		{
			_SelectSurvivorsPopup.ReOpenAfterReroll();
		}
	}

	public void ScaleGrid(float scale)
	{
		if (CallTable)
		{
			var tweenScale = CallTable.gameObject.GetComponent<TweenScale>();
			tweenScale.to = Vector3.one * scale;
			tweenScale.PlayForward();

			var tweenScaleHat = CallTableHat.gameObject.GetComponent<TweenScale>();
			tweenScaleHat.to = Vector3.one * scale;
			tweenScaleHat.PlayForward();

			var tweenScaleJack = CallScenarioTable.gameObject.GetComponent<TweenScale>();
			tweenScaleJack.to = Vector3.one * scale;
			tweenScaleJack.PlayForward();
		}
	}

	public void SwitchMultiCallTg(UIToggle toggle)
	{
		IsMultiCallMode = toggle.value;
		multiCallInput.transform.parent.gameObject.SetActive(IsMultiCallMode);
	}

	public void SwitchMultiCallBt(UIButtonToggle toggle)
	{
		IsMultiCallMode = toggle.IsToggled;
		multiCallInput.transform.parent.gameObject.SetActive(IsMultiCallMode);
	}

	//Открыть коллекцию токенов для выбора приоритетного героя
	public void OnClickInstanceToken(UIButtonExtended bt)
	{
		var toolTip = CraftSettings.Instance.ToolTipTokenLarge;
		toolTip.SetActive(true);
		TooltipManager.OpenForTokenSlot(NewPhonePopup.Instance.tokenButton.gameObject, toolTip);
	}

	public int SetMultiCallCount()
	{
		if (!string.IsNullOrEmpty(multiCallInput.label.text))
			return Math.Abs(int.Parse(multiCallInput.label.text));
		else return 10;
	}

	public static Dictionary<string, ModelRandom> SetDedicatedRandom(Dictionary<string, ModelRandom> dedicatedRandoms)
	{
		Dictionary<string, ModelRandom> newDedicatedRandoms = new();
		foreach (var item in dedicatedRandoms)
		{
			newDedicatedRandoms.Add(item.Key, new ModelRandom(item.Value));
		}
		return newDedicatedRandoms;
	}

	public delegate void OnVisualize();
	public OnVisualize OnVisualizeHandler;
	//UIToggle
	public void VisualizeJackPot(UIButtonToggle tg)
	{
		IsVisualized = tg.IsToggled;
		OnVisualizeHandler?.Invoke();
		VisualizeJackPotInternal(IsVisualized);
	}

	public void SetGenerated(bool isGenerated)
	{
		IsGeneratedScenarioCards = isGenerated;

		if (IsVisualized)
		{
			VisualizeJackPotInternal(true);
		}
	}

	public void SetViewed(bool isViewed)
	{
		IsCardsViewed = isViewed;
	}

	public void ConvertScenarioCallsToCalls()
	{
		if (CallItemsScenarioList == null || CallItemsScenarioList.Count == 0 || IsScenarioTableApplying) return;

		if (CallItemsList.Count > 0 && CallItemsList.Last().SummTokenValues == CallItemsScenarioList.Last().SummTokenValues) return;

		ShowJackpotToggle.SetToggled(false);
		VisualizeJackPot(ShowJackpotToggle);
		ShowJackpotToggle.GetComponent<UIButtonToggleHelper>().SetSprites(false);

		StartCoroutine(OnClickMultiCallJackpot(CallItemsScenarioList));
	}

	private bool IsScenarioTableApplying;
	public IEnumerator ConvertScenarioCallsToCallsCor()
	{
		IsScenarioTableApplying = true;

		var toggleSet = CallTable.GetComponent<UIButtonSingleToggleSet>();

		var index = CallItemsList.Count - 1;
		foreach (var collItem in CallItemsScenarioList)
		{
			index++;
			CallGridItem gridItem = Instantiate(InitCallGridItem, CallTable.transform);
			gridItem.gameObject.name += "_" + (index + 1);
			CallItemsList.Add(gridItem);

			gridItem.Init(false);
			for (int i = 0; i < collItem.AcceptIndexes.Count; i++)
			{
				if (!collItem.AcceptIndexesGroups[i].gameObject.activeSelf) continue;
				var lockingItem = collItem.AcceptIndexes[i];
				gridItem.AcceptIndexesGroups[i].gameObject.SetActive(true);
				for (int j = 0; j < lockingItem.Count; j++)
				{
					gridItem.AcceptIndexes[i][j].value = lockingItem[j].value;
				}
			}
			for (int k = 0; k < collItem.tokenValues.Count; k++)
			{
				if (collItem.tokenSprites[k].transform.parent.gameObject.activeSelf)
				{
					gridItem.tokenSprites[k].transform.parent.gameObject.SetActive(true);
				}
				HelpersUI.SetSprite(gridItem.tokenSprites[k], collItem.tokenSprites[k].spriteName);
				HelpersUI.SetContentToLabel(gridItem.tokenValues[k], collItem.tokenValues[k].text);
				if (collItem.tokenValues[k].color == Color.green)
				{
					gridItem.tokenValues[k].color = Color.green;
				}
			}

			gridItem.SummTokenValues = collItem.SummTokenValues;
			gridItem.IsAccepted = true;
			gridItem.CallIndex = index;
			gridItem.CallRandomCurrent = SetDedicatedRandom(collItem.CallRandomCurrent);
			gridItem.CallType = collItem.CallType;
			gridItem.CallPrice = collItem.CallPrice;
			gridItem.LootEntryList = collItem.LootEntryList;
			gridItem.CallButton = collItem.CallButton;

			gridItem.SummTokenValuesLabel.text = gridItem.SummTokenValues.ToString();
			gridItem.callIndexLabel.text = (index + 1).ToString();
			gridItem.CallTypeLabel.text = gridItem.CallType.ToString();
			gridItem.CallPriceLabel.text = gridItem.CallPrice.ToString();

			gridItem.gameObject.SetActive(true);

			SelectedCall = gridItem;

			//CalculateDedicatedRandom(false);
			CalculateHeroTokenQueue(gridItem.CallRandomCurrent["HeroToken"]);

			CallTable.Reposition();
			CallScrollView.ResetPositionInverted();

			var buttonToggle = gridItem.GetComponent<UIButtonToggle>();
			toggleSet.Buttons.Add(buttonToggle);
			buttonToggle.SetClickCallback(toggleSet.OnClick);
			yield return null;
		}
		IsScenarioTableApplying = false;

		Player.LootManager.DedicatedRandoms = SetDedicatedRandom(MaxRewardDedicatedRandoms);
		CalculateHeroTokenQueue();
		CallRandomLast = SetDedicatedRandom(Player.LootManager.DedicatedRandoms);
	}

	public IEnumerator ConvertScenarioCalls(List<CallData> callDataList)
	{
		CallItemsScenarioList = new List<CallGridItem>();
		var toggleSet = CallScenarioTable.GetComponent<UIButtonSingleToggleSet>();

		int callIndex = -1;
		foreach (var collItem in callDataList)
		{
			callIndex++;
			RadioCallButton callButton = NewPhonePopup.Instance.GetButtonBySlotNumber(collItem.SlotNumber);
			CallGridItem gridItem = Instantiate(InitCallGridItem, CallScenarioTable.transform);
			gridItem.gameObject.name += "_" + collItem.CallNumber;
			gridItem.Init(false);

			for (int i=0; i<collItem.LootsRerollLockingList.Count; i++)
			{
				var lockingItem = collItem.LootsRerollLockingList[i];
				gridItem.AcceptIndexesGroups[i].gameObject.SetActive(true);
				for (int j=0; j < lockingItem.Count; j++)
				{
					gridItem.AcceptIndexes[i][j].value = lockingItem[j];
				}
			}
			int index = -1;
			foreach (var item in collItem.RewardAmountList.Last())
			{
				index++;
				var locked = collItem.SlotNumber > 3 || collItem.LootsRerollLockingList.Any(x => x[index] == true);
				if (locked)
				{
					gridItem.tokenValues[index].transform.parent.gameObject.SetActive(true);
					var list = item.Split('|');

					var rewardedAmount = list[0];
					var rewardedCurrency = Enum.Parse<CurrencyType>(list[1]);
					HelpersUI.SetSprite(gridItem.tokenSprites[index], HelpersGfx.GetCurrencyIconName(rewardedCurrency));
					HelpersUI.SetContentToLabel(gridItem.tokenValues[index], rewardedAmount);
					int rewardedAmountDigit = int.Parse(rewardedAmount);

					gridItem.SummTokenValues += rewardedAmountDigit;
					if (rewardedAmountDigit == callButton.parsedHeroTokensDropNumberValues.Last())
					{
						gridItem.tokenValues[index].color = Color.green;
					}
				}			
			}
			//
			gridItem.CallRandomCurrent = collItem.DedicatedRandoms;
			gridItem.CallIndex = callIndex + 1;
			gridItem.CallType = collItem.SlotNumber;
			gridItem.CallPrice = collItem.CallPrice;
			gridItem.LootEntryList = collItem.LootEntryList;
			gridItem.CallButton = callButton;
			//

			gridItem.SummTokenValuesLabel.text = gridItem.SummTokenValues.ToString();
			gridItem.callIndexLabel.text = (callIndex + 1).ToString();
			gridItem.CallTypeLabel.text = gridItem.CallType.ToString();
			gridItem.CallPriceLabel.text = gridItem.CallPrice.ToString();

			gridItem.gameObject.SetActive(true);

			CallItemsScenarioList.Add(gridItem);

			CallScenarioTable.Reposition();
			CallScenarioScrollView.ResetPositionInverted();

			var buttonToggle = gridItem.GetComponent<UIButtonToggle>();
			toggleSet.Buttons.Add(buttonToggle);
			buttonToggle.SetClickCallback(toggleSet.OnClick);
			yield return null;
		}
		int maxValue = CallItemsScenarioList.Select(x => x.SummTokenValues).Max();
		CallGridItem callMaxItem = CallItemsScenarioList.FirstOrDefault(x => x.SummTokenValues == maxValue);
		if (callMaxItem != null)
		{
			callMaxItem.UpdateLabel(true);
		}
		CallPriceSummLabel.text = callDataList.Select(x=>x.CallPrice).Sum().ToString();
		IsCardsViewed = true;
	}

	public void VisualizeJackPotInternal(bool isVisualize)
	{
		if (isVisualize)
		{
			CallScrollView.gameObject.SetActive(false);
			CallScenarioScrollView.gameObject.SetActive(true);
			if (IsGeneratedScenarioCards && !IsCardsViewed)
			{
				CallScenarioTable.GetComponent<UIButtonSingleToggleSet>().Buttons = new();
				if (CallItemsScenarioList.Count > 0)
				{
					Helpers.DestroyAllChildren(CallScenarioTable.gameObject);
				}
				CallItemsScenarioList = new();
				StartCoroutine(ConvertScenarioCalls(CallDataList));
			}
		}
		else
		{
			CallScenarioScrollView.gameObject.SetActive(false);
			CallScrollView.gameObject.SetActive(true);
		}
	}

	public UILabel HeroTokenQueueLabel;
	public int CallSlotNumberForCalculate = 8;
	public int CycleCountMax = 200;
	public int HeroTokenQueueCount = 0;

	[ContextMenu("Calculate HeroToken Queue")]
	public void CalculateHeroTokenQueue(ModelRandom random = null)
	{
		HeroTokenQueueCount = GetHeroTokenQueueCount(random);

		string heroTokenQueueText = "";
		var heroTokenQueueCountText = HeroTokenQueueCount > 0 ? HeroTokenQueueCount.ToString() : "∞";
		if (DataManager.Instance.language == DataManager.Language.Ru)
		{
			heroTokenQueueText = "Jackpot after [FFDB00]" + heroTokenQueueCountText + "[-]";
		}
		else
		{
			heroTokenQueueText = "Джекпот через [FFDB00]" + heroTokenQueueCountText + "[-]";
		}
		HeroTokenQueueLabel.text = heroTokenQueueText;
	}

	public void ShowHeroTokenQueueTooltip()
	{
		var heroTokenQueueCountText = HeroTokenQueueCount > 0 ? HeroTokenQueueCount.ToString() : "∞";
		string heroTokenQueueTextEn = "Jackpot will be in [FFDB00]" + heroTokenQueueCountText + "[-] hero cards";
		string heroTokenQueueTextRu = "Джекпот будет через [FFDB00]" + heroTokenQueueCountText + "[-] карточек героев";

		var toolTip = HeroTokenQueueLabel.GetComponent<ShowTooltip>();
		toolTip.EnCustomText = heroTokenQueueTextEn;
		toolTip.RuCustomText = heroTokenQueueTextRu;
		toolTip.OnClickEventIcon();
	}

	public void ShowBlockTypeTooltip(UIButtonExtended bt)
	{
		string heroTokenQueueTextRu = "Расчет блокировок карточек вызовов для мультивызова или пересчета текущей очереди вызовов.\n" +
										"[FF4343]Авто[-] - наибольшая ценность,\n" +
										"[FF4343]Сохраненные[-] - подставить сохраненные блокировки для измененного вызова,\n" +
										"[FF4343]Вкл. Все[-] - блокировать и принять все карточки сразу (для вызовов с перекатом)\n" +
										"[FF4343]Выкл. Все[-] - не блокировать ничего (для вызовов с перекатом)";
		string heroTokenQueueTextEn = "Calculation of call card locks for multi-calls or recalculation of the current call queue.\n" +
										"[FF4343]Auto[-] – highest reward rarity,\n" +
										"[FF4343]Saved[-] – apply saved locks for the modified call,\n" +
										"[FF4343]All Enable[-] – lock and apply all cards at once (for reroll calls),\n" +
										"[FF4343]All Disable[-] – don't lock anything (for reroll calls).";

		var toolTip = bt.GetComponent<ShowTooltip>();
		toolTip.EnCustomText = heroTokenQueueTextEn;
		toolTip.RuCustomText = heroTokenQueueTextRu;
		toolTip.OnClickEventIcon();
	}

	public int GetHeroTokenQueueCount(ModelRandom random = null)
	{
		GameEconomyData gameEconomyData = Player.gameEconomyData;
		PhoneCallDefinition phoneCallDefinition = gameEconomyData.GetPhoneCallDefinition(Player.UtcTimeStamp, CallSlotNumberForCalculate);
		int buildingLevel = Player.Camp.GetBuildingLevel("RadioTent");
		var currency = phoneCallDefinition.GetParsedCurrencyTypeValues().First();

		var randomName = "HeroToken";
		var modelRandom = random ?? new ModelRandom(Player.LootManager.GetDedicatedRandom(randomName));
		int maxRewardValue = phoneCallDefinition.HeroTokensDropNumber.Split(';').Select(x => int.Parse(x)).Last();
		int rewardedAmount = 0;
		int cycleCount = 0;
		int rewardedRarityLevel = 0;

		while (cycleCount < CycleCountMax)
		{
			SurvivorToken heroTokenForGatcha = gameEconomyData.GetHeroTokenForGatcha(DropEventDefinition.DropEventType.RadioPhone, DropType.Gold, DropEventDefinition.DropEventTag.None, buildingLevel, currency, modelRandom, -1, phoneCallDefinition);
			if (heroTokenForGatcha != null && heroTokenForGatcha.Type != CurrencyType.None)
			{
				cycleCount++;
				rewardedAmount = heroTokenForGatcha.Amount;
				rewardedRarityLevel = heroTokenForGatcha.AmountRarityLevel;
				if (rewardedAmount == maxRewardValue)
				{
					break;
				}
			}
		}
		DebugTWD.Log("cycleCount is " + cycleCount + ", rewardedRarityLevel is " + rewardedRarityLevel);
		return cycleCount;
	}


	public class LootItem
	{
		public int Index { get; set; }
		public int Type { get; set; }
		public int RewardAmount { get; set; }
		public CurrencyType CurrencyType { get; set; }
	}

	public class CallInfo
	{
		public int CurrentTypeIndex { get; set; }
		public int Price { get; set; }
		public string CurrentCallInfo { get; set; }
		public RadioCallButton CallButton { get; set; }

		public CallInfo() { }

		public CallInfo(RadioCallButton currentCallButton)
		{
			CallButton = currentCallButton;
			Price = currentCallButton.GetCallPrice();
			CurrentTypeIndex = currentCallButton.SlotNumber;
			CurrentCallInfo = currentCallButton.GetInfo();
		}

		public CallInfo CopyOf()
		{
			return new CallInfo { CurrentTypeIndex = CurrentTypeIndex, Price = Price, CurrentCallInfo = CurrentCallInfo, CallButton = CallButton };
		}
	}
}
