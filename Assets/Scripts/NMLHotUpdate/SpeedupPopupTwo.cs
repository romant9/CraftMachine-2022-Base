using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class SpeedupPopupTwo : HUDElement
{
	[SerializeField]
	private UISprite[] speedIcons;

	[SerializeField]
	protected UILabel TimeLabel;

	[SerializeField]
	private UIGrid grid;

	[SerializeField]
	private UISprite TokenIcon;

	[SerializeField]
	private UISprite edBar;

	[SerializeField]
	private UISprite preBar;

	[SerializeField]
	private PayButton payButton;

	[SerializeField]
	private UIButton itemButton;

	[SerializeField]
	private UISprite ItemPayIcon;

	public ConsumeCurrencyCommand consumeCurrencyCommand;

	private List<int> amounts = new List<int>();

	private List<long> typeNums = new List<long>();

	private List<CurrencyModel> tokenCurrencys = new List<CurrencyModel>();

	private CurrencyType tokenType;

	private long wasteTimeMilliseconds;

	private long TotalMillisecondsTime;

	private long OldRemainMillisecondsTime;

	private int diamondAmount;

	private Cashier goldCashier;

	private Callback cancelCallback;

	private Callback tokenCallback;

	private Callback goldCallback;

	private bool StopUpdate;

	private MedicTentModel medicTentModelCached;

	private const int MaxTyoeTokenNum = 9;

	private int secondsLeftLastFranme;

	private MedicTentModel medicTentModel
	{
		get
		{
			if (medicTentModelCached == null && GameManager.Instance != null && GameManager.Instance.playerModel != null && GameManager.Instance.playerModel.Camp != null)
			{
				medicTentModelCached = GameManager.Instance.playerModel.Camp.GetBuilding("MedicTent") as MedicTentModel;
			}
			return medicTentModelCached;
		}
	}

	private void ResetData()
	{
		secondsLeftLastFranme = 0;
		amounts.Clear();
		tokenCurrencys.Clear();
		typeNums.Clear();
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		UpdateTokenBP();
		UpdateTimeInfo();
	}

	public override void Open()
	{
		base.Open();
		InitData();
		UpdateUI();
	}

	public override void Update()
	{
		base.Update();
		UpdateTimeInfo();
	}

	public override void Close()
	{
		secondsLeftLastFranme = 0;
		StopUpdate = true;
		base.Close();
	}

	public void SetContent(string title, string info, int amount, CurrencyType currencyType = CurrencyType.Diamonds)
	{
		diamondAmount = amount;
		goldCashier = Cashier.CreateOneItemCashier(GameManager.Instance.modelManager, PurchaseType.None, CurrencyType.Diamonds, amount);
		payButton.UpdateUI(goldCashier);
		tokenType = currencyType;
	}

	public void SetSpeedupCallbacks(Callback tokenCallback = null, Callback goldCallback = null, Callback cancelCallback = null)
	{
		this.tokenCallback = tokenCallback;
		this.goldCallback = goldCallback;
		this.cancelCallback = cancelCallback;
	}

	private void UpdateTimeInfo()
	{
		if (StopUpdate)
		{
			return;
		}
		SurvivorModel survivorModel = null;
		switch (tokenType)
		{
		case CurrencyType.EquipmentTokenBP:
		{
			EquipmentItemModel equipmentItemModel = GameManager.Instance.modelManager.GetModel<EquipmentItemModel>(consumeCurrencyCommand.ModelId);
			OldRemainMillisecondsTime = equipmentItemModel.TimedActionModel.MillisecondsTillCompletion;
			TotalMillisecondsTime = equipmentItemModel.TimedActionModel.OriginalActionTime;
			break;
		}
		case CurrencyType.BuildingTokenBP:
		{
			BuildingModel buildingModel = GameManager.Instance.modelManager.GetModel<BuildingModel>(consumeCurrencyCommand.ModelId);
			OldRemainMillisecondsTime = buildingModel.UpgradeTimer;
			TotalMillisecondsTime = buildingModel.OriginalUpgradeTimer;
			break;
		}
		case CurrencyType.TrainingTokenBP:
			survivorModel = GameManager.Instance.modelManager.GetModel<SurvivorModel>(consumeCurrencyCommand.ModelId);
			if (survivorModel != null)
			{
				OldRemainMillisecondsTime = survivorModel.TimedActionModel.MillisecondsTillCompletion;
			}
			else
			{
				OldRemainMillisecondsTime = 0L;
			}
			TotalMillisecondsTime = survivorModel.TimedActionModel.OriginalActionTime;
			break;
		case CurrencyType.HealingTokenBP:
		{
			survivorModel = GameManager.Instance.modelManager.GetModel<SurvivorModel>(consumeCurrencyCommand.ModelId);
			TimedQueueItemModel queueItemFromItem = medicTentModel.TimedQueueModel.GetQueueItemFromItem(survivorModel);
			OldRemainMillisecondsTime = queueItemFromItem.MillisecondsTillCompletion;
			TotalMillisecondsTime = queueItemFromItem.OriginalActionTime;
			break;
		}
		}
		int count = tokenCurrencys.Count;
		long num = 0L;
		for (int i = 0; i < count; i++)
		{
			num = ((i != count - 1) ? (num + amounts[i] * typeNums[i] * 60 * 1000) : (num + amounts[i] * TotalMillisecondsTime));
		}
		if (OldRemainMillisecondsTime > 0)
		{
			Helpers.ConvertToSecondsNoZero(OldRemainMillisecondsTime);
			if (OldRemainMillisecondsTime - num < 0)
			{
				wasteTimeMilliseconds = num - OldRemainMillisecondsTime;
			}
			else
			{
				wasteTimeMilliseconds = 0L;
			}
			long num2 = ((OldRemainMillisecondsTime - num > 0) ? (OldRemainMillisecondsTime - num) : 0);
			int num3 = Helpers.ConvertToSecondsNoZero(num2);
			if (secondsLeftLastFranme != num3 && num3 > -1)
			{
				secondsLeftLastFranme = num3;
				if (num3 <= 0)
				{
					TimeLabel.text = LocalizationManager.GetText("ListItem.Achievement.Completed");
				}
				else
				{
					TimeLabel.text = LocalizationManager.GetText("Popup.SpeedUp.Use.Bar.TimeLeft") + Helpers.FormatTime(num3 * 1000);
				}
			}
			edBar.fillAmount = (float)(TotalMillisecondsTime - OldRemainMillisecondsTime) / (float)TotalMillisecondsTime;
			preBar.fillAmount = (float)(TotalMillisecondsTime - num2) / (float)TotalMillisecondsTime;
		}
		else
		{
			Close();
		}
		itemButton.isEnabled = GetTotal() > 0;
	}

	public void InitData()
	{
		ResetData();
		if (tokenType == CurrencyType.EquipmentTokenBP)
		{
			TokenIcon.spriteName = "Ui_Icon_Workshop_Speedup";
			ItemPayIcon.spriteName = "Ui_Icon_SpeedUpToken_Workshop_Empty";
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken1min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken10min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken20min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken1h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken3h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken7h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentToken14h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.EquipmentTokenBP));
			EquipmentItemModel equipmentItemModel = GameManager.Instance.modelManager.GetModel<EquipmentItemModel>(consumeCurrencyCommand.ModelId);
			consumeCurrencyCommand.Cashier = equipmentItemModel.TimedActionModel.GetSpeedUpCashierWithTokens(CurrencyType.EquipmentTokenBP);
		}
		else if (tokenType == CurrencyType.BuildingTokenBP)
		{
			TokenIcon.spriteName = "Ui_Icon_Build_Speedup";
			ItemPayIcon.spriteName = "Ui_Icon_SpeedUpToken_Building_Empty";
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken1min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken5min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken10min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken30min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken1h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken6h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken12h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingToken24h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.BuildingTokenBP));
			BuildingModel buildingModel = GameManager.Instance.modelManager.GetModel<BuildingModel>(consumeCurrencyCommand.ModelId);
			consumeCurrencyCommand.Cashier = buildingModel.GetSpeedUpUpgradeCashierWithTokens();
		}
		else if (tokenType == CurrencyType.TrainingTokenBP)
		{
			TokenIcon.spriteName = "Ui_Icon_TrainingGround_Speedup";
			ItemPayIcon.spriteName = "Ui_Icon_SpeedUpToken_Training_Empty";
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken5min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken20min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken1h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken3h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken8h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingToken16h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.TrainingTokenBP));
			SurvivorModel survivorModel = GameManager.Instance.modelManager.GetModel<SurvivorModel>(consumeCurrencyCommand.ModelId);
			consumeCurrencyCommand.Cashier = survivorModel.TimedActionModel.GetSpeedUpCashierWithTokens(CurrencyType.TrainingTokenBP);
		}
		else if (tokenType == CurrencyType.HealingTokenBP)
		{
			TokenIcon.spriteName = "Ui_Icon_BuffBuildingHealth_Speedup";
			ItemPayIcon.spriteName = "Ui_Icon_SpeedUpToken_Healing_Empty";
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken1min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken5min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken10min));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken1h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken2h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingToken4h));
			tokenCurrencys.Add(GameManager.Instance.modelManager.Player.GetCurrency(CurrencyType.HealingTokenBP));
			SurvivorModel survivorModel2 = GameManager.Instance.modelManager.GetModel<SurvivorModel>(consumeCurrencyCommand.ModelId);
			consumeCurrencyCommand.Cashier = survivorModel2.TimedActionModel.GetSpeedUpCashierWithTokens(CurrencyType.HealingTokenBP);
		}
		for (int i = 0; i < 9; i++)
		{
			amounts.Add(0);
			typeNums.Add(0L);
		}
		SetTypeNums();
		UpdateTimeInfo();
		int count = tokenCurrencys.Count;
		for (int j = 0; j < count; j++)
		{
			int newIndex = j;
			UIButton component = speedIcons[j].transform.Find("reduce").GetComponent<UIButton>();
			component.onClick.Clear();
			component.onClick.Add(new EventDelegate(delegate
			{
				ReduceToken(newIndex);
			}));
			UIButton component2 = speedIcons[j].transform.Find("add").GetComponent<UIButton>();
			component2.onClick.Clear();
			component2.onClick.Add(new EventDelegate(delegate
			{
				AddToken(newIndex);
			}));
		}
		SetBestAmounts();
	}

	public void AddToken(int index)
	{
		if (amounts.Count > index)
		{
			amounts[index]++;
		}
		UpdateUI();
	}

	public void ReduceToken(int index)
	{
		if (amounts.Count > index)
		{
			amounts[index]--;
		}
		UpdateUI();
	}

	public Dictionary<CurrencyType, int> GetUseExtraTokensDic()
	{
		Dictionary<CurrencyType, int> dictionary = new Dictionary<CurrencyType, int>();
		int count = tokenCurrencys.Count;
		for (int i = 0; i < count; i++)
		{
			dictionary.Add(tokenCurrencys[i].Type, amounts[i]);
		}
		return dictionary;
	}

	public void OnClickGold()
	{
		consumeCurrencyCommand.Cashier.useTokensForPayment = false;
		cancelCallback = null;
		tokenCallback = null;
		Close();
		if (goldCallback != null && goldCashier.CanAfford())
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/use_diamonds");
			goldCallback();
		}
		else
		{
			ShopPopupHelper.OpenForMissingCurrencyWithTotalRequiredAmount(diamondAmount);
		}
	}

	public void OnClickItem()
	{
		int speedUpBeyond = GameManager.Instance.gameEconomyData.ConfigData.SpeedUpBeyond;
		if (wasteTimeMilliseconds >= TotalMillisecondsTime * speedUpBeyond / 100)
		{
			int num = Helpers.ConvertToSecondsNoZero(wasteTimeMilliseconds);
			ConfirmationPopup.ShowPopup(LocalizationManager.GetText("Popup.SpeedUp.Use.Caution.Title"), LocalizationManager.GetText("Popup.SpeedUp.Use.Caution.Description{Parameter}", Helpers.FormatTime(num * 1000)), LocalizationManager.GetText("Button.Ok"), delegate
			{
				OkPressed();
			}, LocalizationManager.GetText("Button.Cancel"), delegate
			{
			});
		}
		else
		{
			OkPressed();
		}
	}

	private int GetTotal()
	{
		int num = 0;
		for (int i = 0; i < amounts.Count; i++)
		{
			num += amounts[i];
		}
		return num;
	}

	public void OkPressed()
	{
		if (GetTotal() > 0)
		{
			SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/use_diamonds");
			consumeCurrencyCommand.Cashier.UseExtraTokens = GetUseExtraTokensDic();
			consumeCurrencyCommand.Cashier.useTokensForPayment = false;
			if (Helpers.ExecuteCommand(consumeCurrencyCommand) == TWDModelResult.OK)
			{
				cancelCallback = null;
				tokenCallback = null;
				Close();
			}
		}
	}

	private void UpdateTokenBP()
	{
		for (int i = 0; i < speedIcons.Length; i++)
		{
			Helpers.GameObjectSetActive(speedIcons[i], value: false);
			Helpers.GameObjectSetActive(speedIcons[i].transform.Find("high").gameObject, value: false);
		}
		int count = tokenCurrencys.Count;
		for (int j = 0; j < count; j++)
		{
			if (amounts[j] <= 0)
			{
				amounts[j] = 0;
			}
			if (amounts[j] >= tokenCurrencys[j].Value)
			{
				amounts[j] = tokenCurrencys[j].Value;
			}
			Helpers.GameObjectSetActive(speedIcons[j], value: true);
			if (amounts[j] > 0)
			{
				Helpers.GameObjectSetActive(speedIcons[j].transform.Find("high").gameObject, value: true);
			}
			speedIcons[j].transform.Find("AmountLabel").GetComponent<UILabel>().text = amounts[j] + "/" + tokenCurrencys[j].Value;
			speedIcons[j].spriteName = HelpersGfx.GetCurrencyIconName(tokenCurrencys[j].Type);
			SpeedupTokenTimeDefinition speedupTokenTimeDefinitionByCurrency = GameManager.Instance.playerModel.gameEconomyData.GetSpeedupTokenTimeDefinitionByCurrency(tokenCurrencys[j].Type.ToString());
			speedIcons[j].transform.Find("timeTitle").GetComponent<UILabel>().text = speedupTokenTimeDefinitionByCurrency.Title;
			speedIcons[j].transform.Find("time").GetComponent<UILabel>().text = HelpersLocalization.GetSpeedCurrencyShortName(tokenCurrencys[j].Type);
			if (tokenCurrencys[j].Value <= 0)
			{
				speedIcons[j].transform.Find("reduce").GetComponent<UIButton>().normalSprite = "Ui_Icon_Reduce_Grey";
				speedIcons[j].transform.Find("add").GetComponent<UIButton>().normalSprite = "Ui_Icon_Add_Grey";
			}
			else if (amounts[j] > 0)
			{
				if (amounts[j] == tokenCurrencys[j].Value)
				{
					speedIcons[j].transform.Find("add").GetComponent<UIButton>().normalSprite = "Ui_Icon_Add_Grey";
				}
				else
				{
					speedIcons[j].transform.Find("add").GetComponent<UIButton>().normalSprite = "Ui_Icon_Add";
				}
				speedIcons[j].transform.Find("reduce").GetComponent<UIButton>().normalSprite = "Ui_Icon_Reduce";
				Helpers.GameObjectSetActive(speedIcons[j].transform.Find("high").gameObject, value: true);
			}
			else
			{
				speedIcons[j].transform.Find("reduce").GetComponent<UIButton>().normalSprite = "Ui_Icon_Reduce_Grey";
				speedIcons[j].transform.Find("add").GetComponent<UIButton>().normalSprite = "Ui_Icon_Add";
			}
		}
	}

	private long GetTotalMinTimesNotMax()
	{
		long num = 0L;
		int count = tokenCurrencys.Count;
		for (int i = 0; i < count - 1; i++)
		{
			num += tokenCurrencys[i].Value * typeNums[i];
		}
		return num;
	}

	private void SetTypeNums()
	{
		switch (tokenType)
		{
		case CurrencyType.EquipmentTokenBP:
			typeNums[0] = 1L;
			typeNums[1] = 10L;
			typeNums[2] = 20L;
			typeNums[3] = 60L;
			typeNums[4] = 180L;
			typeNums[5] = 420L;
			typeNums[6] = 840L;
			break;
		case CurrencyType.BuildingTokenBP:
			typeNums[0] = 1L;
			typeNums[1] = 5L;
			typeNums[2] = 10L;
			typeNums[3] = 30L;
			typeNums[4] = 60L;
			typeNums[5] = 360L;
			typeNums[6] = 720L;
			typeNums[7] = 1440L;
			break;
		case CurrencyType.TrainingTokenBP:
			typeNums[0] = 5L;
			typeNums[1] = 20L;
			typeNums[2] = 60L;
			typeNums[3] = 180L;
			typeNums[4] = 480L;
			typeNums[5] = 960L;
			break;
		case CurrencyType.HealingTokenBP:
			typeNums[0] = 1L;
			typeNums[1] = 5L;
			typeNums[2] = 10L;
			typeNums[3] = 60L;
			typeNums[4] = 120L;
			typeNums[5] = 240L;
			break;
		case CurrencyType.SuperBuildingTokenBP:
		case CurrencyType.SuperTrainingTokenBP:
		case CurrencyType.SuperEquipmentTokenBP:
			break;
		}
	}

	public void SetBestAmounts()
	{
		long num = GetTotalMinTimesNotMax() * 60 * 1000;
		if (OldRemainMillisecondsTime > num)
		{
			int count = tokenCurrencys.Count;
			if (tokenCurrencys[count - 1].Value > 0)
			{
				amounts[count - 1] = 1;
			}
			else
			{
				for (int i = 0; i < count - 1; i++)
				{
					amounts[i] = tokenCurrencys[i].Value;
				}
				amounts[count - 1] = 0;
			}
		}
		if (OldRemainMillisecondsTime > num)
		{
			return;
		}
		int count2 = tokenCurrencys.Count;
		long num2 = OldRemainMillisecondsTime;
		for (int num3 = count2 - 2; num3 >= 0; num3--)
		{
			if (typeNums[num3] != 0L)
			{
				amounts[num3] = (int)(num2 / (typeNums[num3] * 60 * 1000));
				amounts[num3] = ((amounts[num3] < tokenCurrencys[num3].Value) ? amounts[num3] : tokenCurrencys[num3].Value);
				num2 -= amounts[num3] * typeNums[num3] * 60 * 1000;
			}
		}
	}
}
