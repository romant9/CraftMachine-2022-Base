using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TWDModel;
using UnityEngine;

public class RouletteLotteryPopup : MonoBehaviour
{
	[SerializeField]
	private UIButton DrawOnceButton;

	[SerializeField]
	private UIButton DrawMultiButton;

	[SerializeField]
	private GameObject CompletedGO;

	[SerializeField]
	private LargeWheelSyncController largeWheelSyncController;

	[SerializeField]
	private SmallWheelSyncController smallWheelSyncController;

	[SerializeField]
	private List<RouletteRewardCard> RightWheelRewardGameObjectList;

	[SerializeField]
	private List<RouletteRewardCard> LeftWheelRewardGameObjectList;

	[SerializeField]
	private RouletteRewardCard RouletteReward;

	[SerializeField]
	private GameObject RouletteMask;

	[SerializeField]
	private UILabel CompleteTitleLabel;

	[SerializeField]
	private UILabel TitleLabel;

	[SerializeField]
	private UILabel TimeLabel;

	[SerializeField]
	private UILabel DiscountLabel;

	[SerializeField]
	private UISprite DrawOnceConsumeIcon;

	[SerializeField]
	private UISprite DrawMultiConsumeIcon;

	[SerializeField]
	private UILabel DrawOnceConsumeLabel;

	[SerializeField]
	private UILabel DrawMultiConsumeLabel;

	[SerializeField]
	private ParticleSystem RotateEffect;

	[SerializeField]
	private UILabel MultiButtonLabel1;

	[SerializeField]
	private UILabel MultiButtonLabel2;

	[SerializeField]
	private UILabel MultiButtonLabel3;

	[SerializeField]
	private UISprite MultiButtonIcon;

	[SerializeField]
	private UISprite DiscountIcon;

	[SerializeField]
	private GameObject MultiButtonEffect;

	[SerializeField]
	private Color GrayButtonColor;

	[SerializeField]
	private Color GrayDiscountColor;

	[SerializeField]
	private Color GrayConsumeColor;

	private RouletteActivityDataModel _rouletteActivityData;

	private List<RouletteDefinition> _leftWheelRewardList;

	private List<RouletteDefinition> _rightWheelRewardList;

	private RouletteConfig _rouletteConfig;

	private int _curRewardIndex;

	private int _curHighLightIndex = -1;

	private long _gameModeTimeLeft;

	private RouletteDrawCommand _rouletteDrawCommand;

	private RouletteMultiDrawCommand _rouletteMultiDrawCommand;

	private int _turningType2RewardIndex = -1;

	private bool _isLargeWheelTurning;

	private GameObject _highLightEffect;

	private string _bigRollSound = "camp/roulette_bigroll";

	private string _smallRollSound = "camp/roulette_smallroll";

	private bool IsValid
	{
		get
		{
			if (GameManager.Instance.playerModel.RouletteManager == null)
			{
				return false;
			}
			return true;
		}
	}

	private void OnEnable()
	{
		UIEvent.OnUIEvent += OnUIEvent;
		if (OfflineManager.IsLoadDataManager)
		{
			PlayerRandomValues.Instance.On_Call_Reset += OnClickReset;
			PlayerRandomValues.Instance.On_Call_Change += OnCounterChange;
		}
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		if (OfflineManager.IsLoadDataManager)
		{
			PlayerRandomValues.Instance.On_Call_Reset -= OnClickReset;
			PlayerRandomValues.Instance.On_Call_Change -= OnCounterChange;
		}
	}

	public void Open(RouletteActivityDataModel data)
	{
		if (OfflineManager.IsLoadDataManager)
		{
			AutoDrawToggle.value = IsAutoDraw;
			QuickDrawToggle.value = IsQuickDraw;

			if (!seenPopup)
			{
				seenPopup = true;
				BackupRouletteDefinitionData(data);
			}
		}
		_rouletteActivityData = data;
		List<RouletteDefinition> definitions = _rouletteActivityData.GetDefinitions();
		_rightWheelRewardList = definitions.Where((RouletteDefinition definition) => definition != null && definition.RouletteType == 1).ToList();
		_leftWheelRewardList = definitions.Where((RouletteDefinition definition) => definition != null && definition.RouletteType == 2).ToList();
		largeWheelSyncController.InitializeWheel(_rightWheelRewardList);
		smallWheelSyncController.InitializeWheel();
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		_rouletteConfig = _rouletteActivityData.GetConfig();
		_gameModeTimeLeft = _rouletteConfig.EndTimeMilliseconds - GameManager.Instance.playerModel.UtcTimeStamp;
		HelpersUI.SetContentToLabel(TitleLabel, LocalizationManager.GetText(_rouletteConfig.NameDesc));
		HelpersUI.SetContentToLabel(DiscountLabel, "-" + _rouletteConfig.Discount + "%");
		List<CurrencyType> singleCostCurrencyTypes = _rouletteConfig.GetSingleCostCurrencyTypes();
		List<CurrencyType> multiCostCurrencyTypes = _rouletteConfig.GetMultiCostCurrencyTypes();
		HelpersUI.SetContentToLabel(DrawOnceConsumeLabel, _rouletteConfig.GetSingleCostAmountByCurrencyType(singleCostCurrencyTypes[0]).ToString());
		HelpersUI.SetContentToLabel(DrawMultiConsumeLabel, _rouletteConfig.GetMultiCostAmountByCurrencyType(multiCostCurrencyTypes[0]).ToString());
		DrawOnceConsumeIcon.spriteName = HelpersGfx.GetCurrencyIconName(singleCostCurrencyTypes[0]);
		DrawMultiConsumeIcon.spriteName = HelpersGfx.GetCurrencyIconName(multiCostCurrencyTypes[0]);
		UpdateUI();
		if (_rouletteActivityData.DrawnType2SlotIndices.Contains(1))
		{
			if (!OfflineManager.IsLoadDataManager) Helpers.GameObjectSetActive(RouletteReward.gameObject, value: false);
		}
		else
		{
			Helpers.GameObjectSetActive(RouletteReward.gameObject, value: true);
			RouletteReward.Bind(_leftWheelRewardList[0].RewardsObj.GetRewardAt(0), isSpecial: false, isPremium: true);
		}
		if (OfflineManager.IsLoadDataManager)
		{
			Helpers.GameObjectSetActive(RouletteFreeReward.gameObject, value: true);
			var largeIndex = largeWheelSyncController.GetCurrentPrizeIndex();
			RouletteFreeReward.Bind(_rightWheelRewardList[largeIndex].RewardsObj.GetRewardAt(0), isSpecial: false, isPremium: false);
		}
		_turningType2RewardIndex = -1;
		_curHighLightIndex = -1;
		_isLargeWheelTurning = false;
	}

	public void Close()
	{
		if (IsValid)
		{
			Helpers.GameObjectSetActive(base.gameObject, value: false);
		}
	}

	public void Update()
	{
		int currentPrizeIndex = smallWheelSyncController.GetCurrentPrizeIndex();

		if (_curRewardIndex != currentPrizeIndex)
		{
			_curRewardIndex = currentPrizeIndex;
			if (_curRewardIndex == _turningType2RewardIndex - 1)
			{
				Helpers.GameObjectSetActive(RouletteReward.gameObject, value: true);
				RouletteReward.Bind(_leftWheelRewardList[currentPrizeIndex].RewardsObj.GetRewardAt(0), isSpecial: false, isPremium: true);
			}
			else if (_rouletteActivityData.DrawnType2SlotIndices.Contains(_curRewardIndex + 1))
			{
				if (!OfflineManager.IsLoadDataManager) Helpers.GameObjectSetActive(RouletteReward.gameObject, value: false);
			}
			else
			{
				Helpers.GameObjectSetActive(RouletteReward.gameObject, value: true);
				RouletteReward.Bind(_leftWheelRewardList[currentPrizeIndex].RewardsObj.GetRewardAt(0), isSpecial: false, isPremium: true);
			}
		}
		if (_isLargeWheelTurning)
		{
			int currentPrizeIndex2 = largeWheelSyncController.GetCurrentPrizeIndex();
			if (_curHighLightIndex != currentPrizeIndex2)
			{
				_curHighLightIndex = currentPrizeIndex2;

				if (OfflineManager.IsLoadDataManager)
				{
					Helpers.GameObjectSetActive(RouletteFreeReward.gameObject, value: true);
					var rewardObj = _rightWheelRewardList[currentPrizeIndex2].RewardsObj;
					if (rewardObj != null) RouletteFreeReward.Bind(rewardObj.GetRewardAt(0), isSpecial: false, isPremium: false);
				}
				for (int i = 0; i < RightWheelRewardGameObjectList.Count; i++)
				{
					if (currentPrizeIndex2 == i)
					{
						Helpers.GameObjectSetActive(_highLightEffect, value: false);
						if (currentPrizeIndex2 == 6)
						{
							_highLightEffect = RightWheelRewardGameObjectList[currentPrizeIndex2].getAllHighLightGo;
						}
						else
						{
							_highLightEffect = RightWheelRewardGameObjectList[currentPrizeIndex2].highLightGo;
						}
						SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(_bigRollSound);
						Helpers.GameObjectSetActive(_highLightEffect, value: true);
					}
				}
			}
		}
		if (_isSmallWheelTurning)
		{
			int currentPrizeIndex2 = smallWheelSyncController.GetCurrentPrizeIndex();
			if (_curHighLightPremiumIndex != currentPrizeIndex2)
			{
				_curHighLightPremiumIndex = currentPrizeIndex2;
				Helpers.GameObjectSetActive(RouletteReward.gameObject, value: true);
				var rewardObj = _leftWheelRewardList[currentPrizeIndex2].RewardsObj;
				if (rewardObj != null) RouletteReward.Bind(rewardObj.GetRewardAt(0), isSpecial: false, isPremium: true);
			}
		}
		if (_gameModeTimeLeft >= 0)
		{
			_gameModeTimeLeft -= (long)(Time.deltaTime * 1000f);
			if (_gameModeTimeLeft <= 0)
			{
				_gameModeTimeLeft = 0L;
			}
		}
		if (TimeLabel != null)
		{
			string text = LocalizationManager.GetText("UI_Roulette_Countdown", FormatTimeLeft(_gameModeTimeLeft));
			HelpersUI.SetContentToLabel(TimeLabel, text);
		}
	}

	public void UpdateUI()
	{
		if (!IsValid)
		{
			return;
		}
		if (_rouletteActivityData.DrawnType1SlotIndices.Count > 0)
		{
			DrawMultiButton.isEnabled = false;
			MultiButtonLabel1.color = GrayButtonColor;
			MultiButtonLabel2.color = GrayButtonColor;
			MultiButtonLabel3.color = GrayConsumeColor;
			MultiButtonIcon.color = GrayButtonColor;
			DiscountIcon.color = GrayDiscountColor;
			DiscountLabel.color = GrayButtonColor;
			if (!OfflineManager.IsLoadDataManager) Helpers.GameObjectSetActive(MultiButtonEffect, value: false);
		}
		else
		{
			DrawMultiButton.isEnabled = true;
			MultiButtonLabel1.color = Color.white;
			MultiButtonLabel2.color = Color.white;
			MultiButtonLabel3.color = new Color(0.917f, 0.8667f, 0.796f, 1f);
			MultiButtonIcon.color = Color.white;
			DiscountIcon.color = Color.white;
			DiscountLabel.color = Color.white;
			if (!OfflineManager.IsLoadDataManager) Helpers.GameObjectSetActive(MultiButtonEffect, value: true);
		}
		if (_rouletteActivityData.IsActivityCompleted())
		{
			Helpers.GameObjectSetActive(CompletedGO, value: true);
			HelpersUI.SetContentToLabel(CompleteTitleLabel, LocalizationManager.GetText(_rouletteConfig.NameDesc));
		}
		else
		{
			Helpers.GameObjectSetActive(CompletedGO, value: false);
		}
		for (int i = 0; i < RightWheelRewardGameObjectList.Count; i++)
		{
			if (_rightWheelRewardList == null)
			{
				return;
			}
			if (_rightWheelRewardList[i].Rewards.Equals("GetALL"))
			{
				RightWheelRewardGameObjectList[i].Bind(null);
			}
			else if (_rouletteActivityData.DrawnType1SlotIndices.Contains(i + 1))
			{
				RightWheelRewardGameObjectList[i].Bind(_rightWheelRewardList[i].RewardsObj.GetRewardAt(0), isSpecial: false, isPremium: false, isDel: true);
			}
			else
			{
				RightWheelRewardGameObjectList[i].Bind(_rightWheelRewardList[i].RewardsObj.GetRewardAt(0));
			}
		}
		for (int j = 0; j < LeftWheelRewardGameObjectList.Count; j++)
		{
			if (_rouletteActivityData.DrawnType2SlotIndices.Contains(j + 1))
			{
				LeftWheelRewardGameObjectList[j].Bind(_leftWheelRewardList[j].RewardsObj.GetRewardAt(0), isSpecial: false, isPremium: true, isDel: true);
			}
			else
			{
				LeftWheelRewardGameObjectList[j].Bind(_leftWheelRewardList[j].RewardsObj.GetRewardAt(0), isSpecial: false, isPremium: true);
			}
		}
	}

	public void OnDrawOnceButtonClicked()
	{
		if (OfflineManager.IsLoadDataManager) largeWheelSyncController.ResetWheel(forced: true);

		List<CurrencyType> singleCostCurrencyTypes = _rouletteConfig.GetSingleCostCurrencyTypes();
		if (singleCostCurrencyTypes == null)
		{
			return;
		}
		if ((float)_gameModeTimeLeft <= 0f)
		{
			HUDNotification.Info(LocalizationManager.GetText("UI_Roulette_End_Tips2"));
			return;
		}
		foreach (CurrencyType item in singleCostCurrencyTypes)
		{
			int singleCostAmountByCurrencyType = _rouletteConfig.GetSingleCostAmountByCurrencyType(item);
			int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(item);
			if (singleCostAmountByCurrencyType > currencyAmount)
			{
				switch (item)
				{
					case CurrencyType.Diamonds:
						MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Diamonds, singleCostAmountByCurrencyType);
						break;
					case CurrencyType.Fairmoney:
						MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Fairmoney, singleCostAmountByCurrencyType);
						break;
					case CurrencyType.GoldRadio:
						MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.GoldRadio, singleCostAmountByCurrencyType);
						break;
				}
				return;
			}
		}

		RouletteResult _rouletteDrawResult = null;
		Rewards _rewards = null;
		if (OfflineManager.IsFakeExecuteCommands)
		{
			var player = GameManager.Instance.playerModel;
			_rouletteDrawResult = _rouletteActivityData?.ExecuteRoulette();
			_rouletteActivityData.LastDrawTime = player.UtcTimeStamp;
			List<RouletteDefinition> allRewardsList = _rouletteDrawResult.GetAllRewardsList();
			if (allRewardsList == null || allRewardsList.Count == 0)
			{
				allRewardsList = new List<RouletteDefinition>();
			}
			List<string> list = new List<string>();
			foreach (RouletteDefinition item in allRewardsList)
			{
				if (item != null && !string.IsNullOrEmpty(item.Rewards) && item.Rewards != "GetALL")
				{
					list.Add(item.Rewards);
				}
			}
			int grantedRewardCount = 0;
			if (list.Count > 0)
			{
				Rewards rewards = new Rewards(string.Join(";", list), player.manager);
				List<object> list2 = rewards.Give(player.manager);
				if (list2 != null && list2.Count > 0)
				{
					grantedRewardCount = list2.Count;
					Debug.Log($"GrantRouletteRewards: Successfully granted {list2.Count} rewards for config {_rouletteActivityData.ConfigId}");
					_rewards = rewards;
				}
			}
			RouletteConfig rouletteConfig = _rouletteActivityData?.GetConfig();
			Dictionary<CurrencyType, int> dictionary = rouletteConfig?.GetSingleCostInfo();
			string cost_used = "";
			string cost_currency_types = "";
			string draw_count = "";
			if (dictionary != null && dictionary.Count > 0)
			{
				cost_used = string.Join(", ", dictionary.Select((KeyValuePair<CurrencyType, int> kvp) => $"{kvp.Key}:{kvp.Value}"));
			}
			if (rouletteConfig != null && singleCostCurrencyTypes != null)
			{
				cost_currency_types = string.Join(",", singleCostCurrencyTypes);
			}
			draw_count = (allRewardsList?.Count ?? 0).ToString();
			DebugTWD.Log($"draw_count: {draw_count}, cost_used: {cost_used}, cost_currency_types: {cost_currency_types}", DebugType.Call);
		}
		else
		{
			_rouletteDrawCommand = new RouletteDrawCommand(_rouletteActivityData.ConfigId);
			if (Helpers.ExecuteCommand(_rouletteDrawCommand) != TWDModelResult.OK)
			{
				return;
			}
			_rouletteDrawResult = _rouletteDrawCommand.RouletteResult;
			_rewards = _rouletteDrawCommand.Rewards;
		}

		Helpers.GameObjectSetActive(RouletteMask, value: true);
		_isLargeWheelTurning = true;
		if (!OfflineManager.IsNoEffects) largeWheelSyncController.Effect = RightWheelRewardGameObjectList[_rouletteDrawResult.DrawnSlotIndex - 1].effect;
		largeWheelSyncController.StartSpin(_rouletteDrawResult.DrawnSlotIndex - 1, delegate
		{
			_isLargeWheelTurning = false;
			_curHighLightIndex = -1;
			if (_rouletteDrawResult.DrawnSlotIndex == 7)
			{
				_turningType2RewardIndex = _rouletteDrawResult.DrawnType2SlotIndex;
				ShowGetAllEffect(_rouletteDrawResult.DrawnType2SlotIndex - 1, _rewards.RewardsList);
			}
			else
			{
				if (!IsQuickDraw)
				{
					IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
					if (iAPConfirmPopupNew != null)
					{
						iAPConfirmPopupNew.OpenForRewards(_rewards.RewardsList);
						iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
					}
				}
				else
				{
					Helpers.GameObjectSetActive(RouletteFreeReward.gameObject, value: true);
					int currentPrizeIndex2 = largeWheelSyncController.GetCurrentPrizeIndex();
					var rewardObj = _rightWheelRewardList[currentPrizeIndex2].RewardsObj;
					if (rewardObj != null) RouletteFreeReward.Bind(rewardObj.GetRewardAt(0), isSpecial: false, isPremium: false);
				}

				largeWheelSyncController.ResetWheel();

				Helpers.GameObjectSetActive(RouletteMask, value: false);
				Helpers.GameObjectSetActive(_highLightEffect, value: false);
				UpdateUI();
			}
		}, isQuick : IsQuickDraw);
	}

	public void OnDrawMultiButtonClicked()
	{
		smallWheelSyncController.ResetWheel(forced: true);

		List<CurrencyType> multiCostCurrencyTypes = _rouletteConfig.GetMultiCostCurrencyTypes();
		if (multiCostCurrencyTypes == null)
		{
			return;
		}
		if ((float)_gameModeTimeLeft <= 0f)
		{
			HUDNotification.Info(LocalizationManager.GetText("UI_Roulette_End_Tips2"));
			return;
		}
		foreach (CurrencyType item in multiCostCurrencyTypes)
		{
			int multiCostAmountByCurrencyType = _rouletteConfig.GetMultiCostAmountByCurrencyType(item);
			int currencyAmount = GameManager.Instance.playerModel.GetCurrencyAmount(item);
			if (multiCostAmountByCurrencyType > currencyAmount && !OfflineManager.IsFreeAll && !IsQuickDraw)
			{
				switch (item)
				{
				case CurrencyType.Diamonds:
					MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Diamonds, multiCostAmountByCurrencyType);
					break;
				case CurrencyType.Fairmoney:
				{
					MiniShopPopup.OpenWithTotalRequiredCurrencyAmount(CurrencyType.Fairmoney, multiCostAmountByCurrencyType);
					break;
				}
				}
				return;
			}
		}
		GameManager.Instance.CheckConnectionReachability(showPopup: true, "RouletteMultiDrawCommand");
		RouletteResult _rouletteResult = null;
		Rewards _rewards = null;
		if (OfflineManager.IsFakeExecuteCommands)
		{
			var player = GameManager.Instance.playerModel;
			_rouletteResult = _rouletteActivityData?.ExecuteMultiDraw();

			List<RouletteDefinition> allRewardsList = _rouletteResult?.GetAllRewardsList();
			if (allRewardsList == null || allRewardsList.Count == 0)
			{
				allRewardsList = new List<RouletteDefinition>();
			}
			List<string> list = new List<string>();
			foreach (RouletteDefinition item in allRewardsList)
			{
				if (item != null && !string.IsNullOrEmpty(item.Rewards) && item.Rewards != "GetALL")
				{
					list.Add(item.Rewards);
				}
			}
			int grantedRewardCount = 0;
			if (list.Count > 0)
			{
				Rewards rewards = new Rewards(string.Join(";", list), player.manager);
				List<object> list2 = rewards.Give(player.manager);
				if (list2 != null && list2.Count > 0)
				{
					grantedRewardCount = list2.Count;
					DebugTWD.Log($"GrantRouletteRewards: Successfully granted {list2.Count} rewards for config {_rouletteActivityData.ConfigId}", DebugType.Call);
					_rewards = rewards;
				}
			}
		}
		else
		{
			RouletteMultiDrawCommand command = new RouletteMultiDrawCommand(_rouletteActivityData.ConfigId);
			if (Helpers.ExecuteCommand(command) != TWDModelResult.OK) return;

			_rouletteResult = command.RouletteResult;
			_rewards = command.Rewards;
		}
		Helpers.GameObjectSetActive(RouletteMask, value: true);
		_turningType2RewardIndex = _rouletteResult.DrawnType2SlotIndex;
		_isLargeWheelTurning = true;
		if (!OfflineManager.IsNoEffects) largeWheelSyncController.Effect = RightWheelRewardGameObjectList[6].effect;
		largeWheelSyncController.StartSpin(6, delegate
		{
			_isLargeWheelTurning = false;
			_curHighLightIndex = -1;
			ShowGetAllEffect(_rouletteResult.DrawnType2SlotIndex - 1, _rewards.RewardsList);
		}, isQuick: IsQuickDraw);
	}

	private string FormatTimeLeft(long timeLeft)
	{
		if (timeLeft <= 0)
		{
			return "0";
		}
		return Helpers.FormatTimeNoZero(timeLeft);
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "RouletteDrawCommandResponseReceived")
		{
			if (_rouletteDrawCommand == null)
			{
				return;
			}
			largeWheelSyncController.StartSpin(_rouletteDrawCommand.RouletteResult.DrawnSlotIndex - 1, delegate
			{
				if (_rouletteDrawCommand.RouletteResult.DrawnSlotIndex == 7)
				{
					_isSmallWheelTurning = true;
					smallWheelSyncController.SpinToPrize(_rouletteDrawCommand.RouletteResult.DrawnType2SlotIndex - 1, delegate
					{
						_isSmallWheelTurning = false;
						IAPConfirmPopupNew iAPConfirmPopupNew2 = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
						if (iAPConfirmPopupNew2 != null)
						{
							iAPConfirmPopupNew2.OpenForRewards(_rouletteDrawCommand.Rewards.RewardsList);
							iAPConfirmPopupNew2.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
						}
						largeWheelSyncController.ResetWheel();
						smallWheelSyncController.ResetWheel();
						Helpers.GameObjectSetActive(RouletteMask, value: false);
						UpdateUI();
						_rouletteDrawCommand = null;
					}, IsQuickDraw);
				}
				else
				{
					IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
					if (iAPConfirmPopupNew != null)
					{
						iAPConfirmPopupNew.OpenForRewards(_rouletteDrawCommand.Rewards.RewardsList);
						iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
					}
					largeWheelSyncController.ResetWheel();
					Helpers.GameObjectSetActive(RouletteMask, value: false);
					UpdateUI();
					_rouletteDrawCommand = null;
				}
			});
		}
		else
		{
			if (!(type == "RouletteMultiDrawCommandResponseReceived") || _rouletteMultiDrawCommand == null)
			{
				return;
			}
			_isSmallWheelTurning = true;
			largeWheelSyncController.StartSpin(6, delegate
			{
				_isSmallWheelTurning = false;
				smallWheelSyncController.SpinToPrize(_rouletteMultiDrawCommand.RouletteResult.DrawnType2SlotIndex - 1, delegate
				{
					IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
					if (iAPConfirmPopupNew != null)
					{
						iAPConfirmPopupNew.OpenForRewards(_rouletteMultiDrawCommand.Rewards.RewardsList);
						iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
					}
					largeWheelSyncController.ResetWheel();
					smallWheelSyncController.ResetWheel();
					Helpers.GameObjectSetActive(RouletteMask, value: false);
					UpdateUI();
					_rouletteMultiDrawCommand = null;
				}, IsQuickDraw);
			});
		}
	}

	private void ShowGetAllEffect(int wheelIndex, List<IReward> rewardList)
	{
		StartCoroutine(LargeWheelGetAllEffect(wheelIndex, rewardList));
	}

	private void TurnSmallWheel(int wheelIndex, List<IReward> rewardList)
	{
		if (OfflineManager.IsLoadDataManager)
		{
			_isSmallWheelTurning = true;
			smallWheelSyncController.ResetWheel(forced: true);
		}

		ParticleSystem.MainModule main = RotateEffect.main;
		main.simulationSpeed = 1.5f;
		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent(_smallRollSound);
		smallWheelSyncController.SpinToPrize(wheelIndex, delegate
		{
			_isSmallWheelTurning = false;
			if (!IsQuickDraw)
			{
				IAPConfirmPopupNew iAPConfirmPopupNew = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.IAPConfirmPopupNew) as IAPConfirmPopupNew;
				if (iAPConfirmPopupNew != null)
				{
					iAPConfirmPopupNew.OpenForRewards(rewardList);
					iAPConfirmPopupNew.SetContent(LocalizationManager.GetText("Popup.IAPConfirm.Title.GenericReward"), null);
				}
			}
			else
			{
				int currentPrizeIndex2 = smallWheelSyncController.GetCurrentPrizeIndex();
				var rewardObj = _leftWheelRewardList[currentPrizeIndex2].RewardsObj;
				if (rewardObj != null) RouletteReward.Bind(rewardObj.GetRewardAt(0), isSpecial: false, isPremium: true);
			}

			largeWheelSyncController.ResetWheel();
			smallWheelSyncController.ResetWheel();

			Helpers.GameObjectSetActive(RouletteMask, value: false);
			Helpers.GameObjectSetActive(_highLightEffect, value: false);
			_turningType2RewardIndex = -1;
			if (!OfflineManager.IsLoadDataManager) Helpers.GameObjectSetActive(RouletteReward.gameObject, value: false);
			main.simulationSpeed = 0.5f;
			UpdateUI();
		}, isQuick: IsQuickDraw);
	}

	private IEnumerator LargeWheelGetAllEffect(int wheelIndex, List<IReward> rewardList)
	{
		RightWheelRewardGameObjectList[7].ShowEffect();
		RightWheelRewardGameObjectList[7].Bind(_rightWheelRewardList[7].RewardsObj.GetRewardAt(0), isSpecial: false, isPremium: false, isDel: true);
		yield return new WaitForSeconds(0.1f);
		for (int i = 0; i < RightWheelRewardGameObjectList.Count - 2; i++)
		{
			RightWheelRewardGameObjectList[i].ShowEffect();
			RightWheelRewardGameObjectList[i].Bind(_rightWheelRewardList[i].RewardsObj.GetRewardAt(0), isSpecial: false, isPremium: false, isDel: true);
			yield return new WaitForSeconds(0.1f);
		}
		yield return new WaitForSeconds(0.3f);
		TurnSmallWheel(wheelIndex, rewardList);
	}


	#region myparams
	[SerializeField]
	private RouletteRewardCard RouletteFreeReward;
	[SerializeField]
	private UIToggle AutoDrawToggle;
	[SerializeField]
	private UIToggle QuickDrawToggle;
	private bool _isSmallWheelTurning;
	private int _curHighLightPremiumIndex;

	private bool IsQuickDraw => CallCraft.Instance.IsQuickDraw;
	private bool IsAutoDraw => CallCraft.Instance.IsAutoDraw;

	private bool seenPopup { get { return CallCraft.Instance.SeenPopup; } set { CallCraft.Instance.SeenPopup = value; } }

	#endregion

	#region mycode
	public void SetAutoDraw(UIToggle tg)
	{
		CallCraft.Instance.IsAutoDraw = tg.value;
	}

	public void SetQuickDraw(UIToggle tg)
	{
		CallCraft.Instance.IsQuickDraw = tg.value;
	}

	public void Reset()
	{
		PlayerRandomValues.Instance.ResetAll(true);
	}

	public void CloseComplete()
	{
		Helpers.GameObjectSetActive(CompletedGO, value: false);
		PlayerRandomValues.Instance.ResetAll(true);
	}

	private void OnCounterChange(int value)
	{
		Debug.Log("RouletteLotteryPopup OnCounterChange");
	}

	private void OnClickReset(bool IsZeroCounter)
	{
		Debug.Log("RouletteLotteryPopup OnClickReset");
		RestoreRouletteDefinitionData();
		largeWheelSyncController.ResetWheel(true);
		smallWheelSyncController.ResetWheel(true);

		Open(_rouletteActivityData);
	}

	private void BackupRouletteDefinitionData(RouletteActivityDataModel data)
	{
		CallCraft.Instance.SavedRouletteDataJson = OfflineManager.JsonSerializer.SerializeObject(data);
	}

	private void RestoreRouletteDefinitionData()
	{
		_rouletteActivityData = OfflineManager.JsonSerializer.DeserializeObject<RouletteActivityDataModel>(CallCraft.Instance.SavedRouletteDataJson);
		GameManager.Instance.playerModel.ActivityIntegrationManager.InterfaceImplementers[0] = _rouletteActivityData;
	}
	#endregion
}
