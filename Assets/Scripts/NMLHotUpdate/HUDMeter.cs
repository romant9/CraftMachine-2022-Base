using System;
using System.Collections;
using Newtonsoft.Json;
using TWDModel;
using UnityEngine;

public class HUDMeter : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Length of the animation in seconds")]
	private int animationLength;

	[SerializeField]
	private UILabel label;

	[SerializeField]
	private UIProgressBar meter;

	[SerializeField]
	private UILabel rechargeTimeLabel;

	private int lastRechargeTimeSeconds = -1;

	[SerializeField]
	private GameObject meterFullEffect;

	[SerializeField]
	private GameObject unlimitedEffect;

	[SerializeField]
	private GameObject timeBonusContainer;

	[SerializeField]
	private bool showMaxValue;

	private CurrencyModel currencyModel;

	private bool useValueFormating;

	private double value;

	private long targetValue;

	private long maxValue;

	private float updateSpeed;

	private float elapsed;

	public bool hasBeenInitialised { get; set; }

	private CampHUD campHUD;

	public CurrencyType CurrencyType { get; private set; }

	[JsonIgnore]
	public TimedBonusModel TimedBonusModel { get; set; }

	public Callback OnProgressBarAnimationStart { get; set; }

	public Callback OnProgressBarAnimationDone { get; set; }

	public long MaxValue => maxValue;

	public long Value => targetValue;

	public bool IsTimedBonusActive
	{
		get
		{
			if (TimedBonusModel != null)
			{
				return TimedBonusModel.IsActive;
			}
			return false;
		}
	}

	public virtual void SetCurrencyType(CurrencyType currencyType, bool formatValue = false)
	{
		CurrencyType = currencyType;
		currencyModel = GameManager.Instance.playerModel.GetCurrency(currencyType);
		useValueFormating = formatValue;

		if (OfflineManager.IsLoadDataManager && CurrencySprite != null)
		{
			CurrencySprite.spriteName = HelpersGfx.GetCurrencyIconName(CurrencyType);
		}
	}

	public virtual void Update()
	{
		if (IsNoUpdate) return;

		if (timeBonusContainer != null)
		{
			if (timeBonusContainer.activeInHierarchy && !IsTimedBonusActive)
			{
				timeBonusContainer.SetActive(value: false);
			}
			else if (!timeBonusContainer.activeInHierarchy && IsTimedBonusActive)
			{
				timeBonusContainer.SetActive(value: true);
			}
		}
		if (rechargeTimeLabel != null && currencyModel != null)
		{
			if (IsTimedBonusActive)
			{
				rechargeTimeLabel.gameObject.SetActive(value: true);
				SetRechargeTimer(TimedBonusModel.MillisecondsTillCompletion);
			}
			else
			{
				rechargeTimeLabel.gameObject.SetActive(value != (double)maxValue && targetValue != maxValue);
				if (CurrencyType == CurrencyType.EndlessPassToken || CurrencyType == CurrencyType.EndlessPassExpertToken)
				{
					long rechargeTimer = GameManager.Instance.playerModel.EndlessModeManager.NextEndlessPassClaimTimeStamp - GameManager.Instance.playerModel.UtcTimeStamp;
					SetRechargeTimer(rechargeTimer);
				}
				else
				{
					SetRechargeTimer(currencyModel.MillisecondsToNextRecharge);
				}
			}
			if (CurrencyType == CurrencyType.ReplayToken && string.IsNullOrEmpty(rechargeTimeLabel.text))
			{
				rechargeTimeLabel.gameObject.SetActive(value: false);
			}
		}
		if (!OfflineManager.IsLoadDataManager) UpdateTimeContainer();
	}

	private void SetRechargeTimer(long milliseconds)
	{
		int num = Helpers.ConvertToSecondsNoZero(milliseconds);
		if (num != lastRechargeTimeSeconds)
		{
			lastRechargeTimeSeconds = num;
			rechargeTimeLabel.text = Helpers.FormatTime((long)num * 1000L);
			SetMeterLabel();
		}
	}

	public void SetMaxValue(int max)
	{
		if (maxValue != max)
		{
			maxValue = max;
			UpdateGUI();
		}
		else if (showMaxValue)
		{
			UpdateGUI();
		}
	}

	public void SetValue(long value)
	{
		long num = (long)Math.Floor(this.value);
		bool flag = num != targetValue;
		if (!hasBeenInitialised)
		{
			hasBeenInitialised = true;
			this.value = value;
			targetValue = value;
			UpdateGUI();
		}
		else if (num != value && (!flag || targetValue != value))
		{
			targetValue = value;
			if (num == int.MaxValue || !base.gameObject.activeInHierarchy)
			{
				this.value = value;
				UpdateGUI();
			}
			else
			{
				StartUpdate();
			}
		}
	}

	private void StartUpdate()
	{
		if (OnProgressBarAnimationStart != null)
		{
			OnProgressBarAnimationStart();
		}
		updateSpeed = Mathf.Abs(targetValue - (long)value) / (float)animationLength;
		elapsed = 0f;
		StopCoroutine(AnimateGUI());
		StartCoroutine(AnimateGUI());
	}

	private IEnumerator AnimateGUI()
	{
		while (elapsed < (float)animationLength)
		{
			elapsed += Time.deltaTime;
			float num = updateSpeed * Time.deltaTime;
			if (value > (double)targetValue)
			{
				value -= num;
				if (value < (double)targetValue)
				{
					value = targetValue;
					break;
				}
			}
			else
			{
				value += num;
				if (value > (double)targetValue)
				{
					value = targetValue;
					break;
				}
			}
			UpdateGUI();
			yield return null;
		}
		UpdateGUI();
		if (OnProgressBarAnimationDone != null)
		{
			OnProgressBarAnimationDone();
		}
	}

	private void SetMeterLabel()
	{
		bool flag = IsTimedBonusActive && currencyModel.Type == CurrencyType.ReplayToken;
		if (flag)
		{
			label.text = LocalizationManager.GetText("Generic.Hud.Unlimited");
		}
		else if (showMaxValue)
		{
			UILabel uILabel = label;
			string text = ((long)value).ToString();
			long num = maxValue;
			uILabel.text = text + "/" + num;
		}
		else
		{
			useValueFormating |= GameManager.Instance.gameEconomyData.ConfigData.CurrencyScientificNotation;
			if (useValueFormating)
			{
				label.text = Helpers.FormatNumber((long)value);
			}
			else
			{
				label.text = ((long)value).ToString();
			}
		}
		if (meterFullEffect != null)
		{
			bool active = (long)value >= maxValue && !flag;
			meterFullEffect.SetActive(active);
		}
		if (unlimitedEffect != null)
		{
			unlimitedEffect.SetActive(flag);
		}
	}

	private void UpdateGUI()
	{
		SetMeterLabel();
		if (meter != null)
		{
			float num = 0f;
			num = ((maxValue != 0L) ? Mathf.Max(0f, Mathf.Min(1f, (float)(value / (double)maxValue))) : 1f);
			meter.value = num;
		}
	}

	private void OnClick()
	{
		DebugTWD.Log("OnClick HUDMeter: " + this.CurrencyType, DebugType.OnClick);

		if (OfflineManager.IsLoadDataManager)
		{
			if (IsRandomMeter)
			{
				GameObject parent = SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.ActivityPopup) ? HUDManager.Instance.UIContainerTopCameras : null;
				OfflineManager.Instance.ShowRandomValuesPopup(parent);
			}
			return;
		}


		SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/ui_click");
		if (CurrencyType == CurrencyType.Diamonds)
		{
			return;
		}
		string text = "";
		string[] array = new string[5];
		string[] array2 = new string[5];
		if (CurrencyType == CurrencyType.ReplayToken || CurrencyType == CurrencyType.EndlessPassToken || CurrencyType == CurrencyType.EndlessPassExpertToken)
		{
			text = HelpersLocalization.GetCurrencyDescription(CurrencyType) + "\n";
			if (IsTimedBonusActive)
			{
				text = text + LocalizationManager.GetText("Tooltip.Currency.ReplayToken.ProductionUnlimited{Time}", Helpers.FormatTime(TimedBonusModel.MillisecondsTillCompletion)) + "\n";
			}
			else if (CurrencyType != CurrencyType.EndlessPassToken && CurrencyType != CurrencyType.EndlessPassExpertToken)
			{
				int replayTokensRechargeSpeed = GameManager.Instance.playerModel.ActivityManager.GetReplayTokensRechargeSpeed(GameManager.Instance.gameEconomyData.ConfigData);
				text = text + LocalizationManager.GetText("Tooltip.Currency.ReplayToken.Production{Time}", Helpers.FormatTime(replayTokensRechargeSpeed * 1000)) + "\n";
			}
			array[0] = HelpersLocalization.GetCurrencyName(CurrencyType);
			array2[0] = GameManager.Instance.playerModel.GetCurrency(CurrencyType).TotalValue.ToString() ?? "";
			array[1] = LocalizationManager.GetText("Tooltip.Currency.StorageCapacity");
			if (CurrencyType == CurrencyType.EndlessPassToken)
			{
				if (EndlessModeHelpers.EndlessManagerModel().GetMaxPasses() - EndlessModeHelpers.EndlessModeConfig.MaxPasses > 0)
				{
					array2[1] = EndlessModeHelpers.EndlessModeConfig.MaxPasses + "+" + (EndlessModeHelpers.EndlessManagerModel().GetMaxPasses() - EndlessModeHelpers.EndlessModeConfig.MaxPasses);
				}
				else
				{
					array2[1] = EndlessModeHelpers.EndlessModeConfig.MaxPasses.ToString() ?? "";
				}
				array[2] = LocalizationManager.GetText("Tooltip.Currency.RefreshDays");
				array2[2] = string.Join(", ", GameManager.Instance.gameEconomyData.EndlessModeConfig.GetValidRefreshDays().ToArray());
			}
			else if (CurrencyType == CurrencyType.EndlessPassExpertToken)
			{
				if (EndlessModeHelpers.EndlessManagerModel().GetMaxExpertPasses() - EndlessModeHelpers.EndlessModeConfig.MaxEndlessPassExpertToken > 0)
				{
					array2[1] = EndlessModeHelpers.EndlessModeConfig.MaxEndlessPassExpertToken + "+" + (EndlessModeHelpers.EndlessManagerModel().GetMaxExpertPasses() - EndlessModeHelpers.EndlessModeConfig.MaxEndlessPassExpertToken);
				}
				else
				{
					array2[1] = EndlessModeHelpers.EndlessModeConfig.MaxEndlessPassExpertToken.ToString() ?? "";
				}
				array[2] = LocalizationManager.GetText("Tooltip.Currency.RefreshDays");
				array2[2] = string.Join(", ", GameManager.Instance.gameEconomyData.EndlessModeConfig.GetValidRefreshDays().ToArray());
			}
			else
			{
				array2[1] = GameManager.Instance.playerModel.GetCapacity(CurrencyType).ToString() ?? "";
			}
		}
		else if (CurrencyType == CurrencyType.ApocalypticEquipToken)
		{
			text = HelpersLocalization.GetCurrencyDescription(CurrencyType) + "\n";
			array[0] = HelpersLocalization.GetCurrencyName(CurrencyType);
			array2[0] = GameManager.Instance.playerModel.GetCurrency(CurrencyType).TotalValue.ToString() ?? "";
		}
		else if (CurrencyType == CurrencyType.GvGMissionKey || CurrencyType == CurrencyType.GuildBattleRP || CurrencyType == CurrencyType.EquipmentUpgradeToken || CurrencyType == CurrencyType.TraitRerollToken || CurrencyType == CurrencyType.GvGGas || CurrencyType == CurrencyType.BlackMarketToken || CurrencyType == CurrencyType.Fairmoney || CurrencyType == CurrencyType.HillTopCoin || CurrencyType == CurrencyType.BulePrintToken)
		{
			text = HelpersLocalization.GetCurrencyDescription(CurrencyType);
			text = HelpersLocalization.ReplaceTripleSpaceWithNewline(text);
		}
		else if (CurrencyType == CurrencyType.EXToken)
		{
			text = HelpersLocalization.GetCurrencyDescription(CurrencyType) + "\n";
			array[0] = HelpersLocalization.GetCurrencyName(CurrencyType);
			array2[0] = GameManager.Instance.playerModel.GetCurrency(CurrencyType).TotalValue.ToString() ?? "";
		}
		else if (CurrencyType == CurrencyType.SPTraitsRemoldToken)
		{
			text = HelpersLocalization.GetCurrencyDescription(CurrencyType) + "\n";
			array[0] = HelpersLocalization.GetCurrencyName(CurrencyType);
			array2[0] = GameManager.Instance.playerModel.GetCurrency(CurrencyType).Value.ToString() ?? "";
		}
		else
		{
			text = HelpersLocalization.GetCurrencyDescription(CurrencyType) + "\n";
			array[0] = HelpersLocalization.GetCurrencyName(CurrencyType);
			array2[0] = GameManager.Instance.playerModel.GetCurrency(CurrencyType).TotalValue.ToString() ?? "";
			array[1] = LocalizationManager.GetText("Tooltip.Currency.ProductionPerHour");
			array2[1] = GameManager.Instance.playerModel.GetProductionPerHour(CurrencyType).ToString() ?? "";
			if (GameManager.Instance.playerModel.GetCurrency(CurrencyType).Max < PlayerModel.UnlimitedCapacityAmount)
			{
				array[2] = LocalizationManager.GetText("Tooltip.Currency.StorageCapacity");
				array2[2] = GameManager.Instance.playerModel.GetCapacity(CurrencyType).ToString() ?? "";
			}
		}
		TooltipManager.OpenTextBoxHud(base.gameObject, text, array, array2);
	}

	private void OnDisable()
	{
		if (IsNoUpdate || GameManager.Instance.playerModel == null) return;

		if ((double)targetValue != value && elapsed < (float)animationLength)
		{
			StopCoroutine(AnimateGUI());
			value = targetValue;
			UpdateGUI();
			OnProgressBarAnimationDone?.Invoke();
		}
		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged -= OnLocalizationLanguageChanged;
	}

	private void OnEnable()
	{
		if (IsNoUpdate || GameManager.Instance.playerModel == null) return;

		SingularityMonoBehaviour<LocalizationManager>.Instance.OnLocalizationLanguageChanged += OnLocalizationLanguageChanged;
		try
		{
			UpdateGUI();
		}
		catch (Exception arg)
		{
			Debug.LogError($"HUDMeter Error:{arg}");
		}
	}

	private void UpdateTimeContainer()
	{
		if (IsNoUpdate) return;

		campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud) as CampHUD;
		if (CurrencyType == CurrencyType.ReplayToken || CurrencyType == CurrencyType.SurvivalPoints)
		{
			HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ActivityPopup);
			if (campHUD == null || !campHUD.GetCampHudContainerShowState())
			{
				Helpers.GameObjectSetActive(timeBonusContainer, value: false);
				Helpers.GameObjectSetActive(rechargeTimeLabel, value: false);
			}
			if (hUDElement != null && hUDElement.IsOpen)
			{
				Helpers.GameObjectSetActive(timeBonusContainer, value: false);
				Helpers.GameObjectSetActive(rechargeTimeLabel, value: false);
			}
		}
	}

	private void OnLocalizationLanguageChanged(string newLanguage)
	{
		UpdateGUI();
	}



	#region myparams
	[SerializeField]
	private bool IsNoUpdate;

	[SerializeField]
	private bool IsRandomMeter;

	[SerializeField]
	private UISprite CurrencySprite;
	#endregion

	#region mycode
	public void SetValueImmediate(int value)
	{
		this.value = value;
		targetValue = value;
		label.text = ((int)value).ToString();
	}
	#endregion
}
