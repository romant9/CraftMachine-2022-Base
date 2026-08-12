using TWDModel;
using UnityEngine;

public class CombatEndFlowStatsWidget : CombatEndWidget
{
	[SerializeField]
	private UILabel InfoLabel;

	[SerializeField]
	private UILabel FirstAmountLabel;

	[SerializeField]
	private UISprite CurrencyIconSprite;

	[SerializeField]
	private UILabel SecondAmountLabel;

	[SerializeField]
	private UILabel ThirdAmountLabel;

	[Tooltip("Container for the double xp effect")]
	[SerializeField]
	private GameObject doubleXpContainer;

	[Tooltip("Container for the Best Score")]
	[SerializeField]
	private GameObject bestScoreContainer;

	[SerializeField]
	private SpeedUpTitle speedUpTitle;

	private bool doubleXpActive;

	private int doubleXpAmount;

	public override void Awake()
	{
		base.Awake();
		DebugClassString = "CombatEndFlowStatsWidget";
		if (InfoLabel != null)
		{
			InfoLabel.gameObject.SetActive(value: false);
		}
		if (FirstAmountLabel != null)
		{
			FirstAmountLabel.gameObject.SetActive(value: false);
		}
		if (CurrencyIconSprite != null)
		{
			CurrencyIconSprite.gameObject.SetActive(value: false);
		}
		if (SecondAmountLabel != null)
		{
			SecondAmountLabel.gameObject.SetActive(value: false);
		}
		if (doubleXpContainer != null)
		{
			doubleXpContainer.gameObject.SetActive(value: false);
		}
		if (bestScoreContainer != null)
		{
			bestScoreContainer.SetActive(value: false);
		}
		if (ThirdAmountLabel != null)
		{
			ThirdAmountLabel.gameObject.SetActive(value: false);
		}
	}

	public override void Activate()
	{
		base.Activate();
		if (!(doubleXpContainer != null))
		{
			return;
		}
		Helpers.GameObjectSetActive(doubleXpContainer, doubleXpActive);
		if (doubleXpActive && doubleXpContainer.GetComponent<AnimateNumberFromTo>() != null)
		{
			AnimateNumberFromTo component = doubleXpContainer.GetComponent<AnimateNumberFromTo>();
			if (component != null)
			{
				component.Animate(doubleXpAmount / 2, doubleXpAmount);
			}
		}
	}

	public void SetInfo(string content)
	{
		HelpersUI.SetContentToLabel(InfoLabel, content);
	}

	public void SetFirstAmount(string amount)
	{
		HelpersUI.SetContentToLabel(FirstAmountLabel, amount);
	}

	public void SetSecondAmount(string amount)
	{
		HelpersUI.SetContentToLabel(SecondAmountLabel, amount);
	}

	public void SetThirdAmount(string amount)
	{
		HelpersUI.SetContentToLabel(ThirdAmountLabel, amount);
	}

	public void SetCurrencyData(CurrencyType currencyType, string amount)
	{
		HelpersUI.SetContentToLabel(SecondAmountLabel, amount);
		HelpersUI.SetSprite(CurrencyIconSprite, HelpersGfx.GetCurrencyIconName(currencyType));
		if (speedUpTitle != null)
		{
			speedUpTitle.UpdateUI(currencyType);
		}
		doubleXpAmount = 0;
		doubleXpActive = currencyType == CurrencyType.SurvivalPoints && GameManager.Instance.playerModel.IsTimedBonusActive(TimedBonusType.DoubleXp) && int.TryParse(amount, out doubleXpAmount);
		if (doubleXpActive)
		{
			HelpersUI.SetContentToLabel(SecondAmountLabel, (doubleXpAmount / 2).ToString() ?? "");
		}
	}

	public void SetCurrencyIcon(string spriteName)
	{
		HelpersUI.SetSprite(CurrencyIconSprite, spriteName);
	}

	public void CreateCurrencyAnimation(CurrencyType currencyType, int amount)
	{
		if (SingularityMonoBehaviour<HUDManager>.Instance != null && CurrencyIconSprite != null)
		{
			CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
			if (campHUD != null && campHUD.GetComponent<BuildingsHUD>() != null)
			{
				campHUD.GetComponent<BuildingsHUD>().CreateCollectAnim(currencyType, CurrencyIconSprite.gameObject, amount);
			}
		}
	}

	public void SetBestScoreContainer(bool personalBest)
	{
		Helpers.GameObjectSetActive(bestScoreContainer, personalBest);
	}
}
