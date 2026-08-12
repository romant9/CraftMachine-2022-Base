using TWDModel;
using UnityEngine;

public class EndScreenStatisticLine : MonoBehaviour
{
	[SerializeField]
	private UILabel statLabel;

	[SerializeField]
	private UILabel moreInfoLabel;

	[SerializeField]
	private CurrencyAmountPanel[] currencyAmountPanels;

	public UISprite MainCurrencyIcon => currencyAmountPanels[0].Icon;

	public Callback Callback { get; set; }

	public void Setup(string statText, int medalAmounts)
	{
		statLabel.text = statText;
		if (moreInfoLabel != null)
		{
			moreInfoLabel.text = "";
		}
		currencyAmountPanels[0].Set("Medal", medalAmounts);
		if (currencyAmountPanels.Length > 1)
		{
			currencyAmountPanels[1].Show(show: false);
		}
	}

	public void Setup(string statText, string moreInfoText, CurrencyType mainCurrency, int mainCurrencyAmount, int medalAmounts = int.MaxValue)
	{
		CombatModel combat = GameManager.Instance.playerModel.Combat;
		statLabel.text = statText;
		if (medalAmounts != int.MaxValue && moreInfoText != null)
		{
			Debug.LogError("You cannot put a 2nd currency if you have more info txt");
		}
		if (moreInfoLabel != null && moreInfoText != null)
		{
			moreInfoLabel.text = moreInfoText;
		}
		if (combat != null && combat.HasPvPRules && mainCurrencyAmount == 0)
		{
			currencyAmountPanels[0].Show(show: false);
		}
		else
		{
			currencyAmountPanels[0].Set(mainCurrency, mainCurrencyAmount);
		}
		if (currencyAmountPanels.Length > 1)
		{
			if (medalAmounts == int.MaxValue || medalAmounts == 0)
			{
				currencyAmountPanels[1].Show(show: false);
			}
			else
			{
				currencyAmountPanels[1].Set("Medal", medalAmounts);
			}
		}
		TweenManager.PlayTweenGroup(base.gameObject, 1, forward: true, OnAnimationFinished);
	}

	public void OnAnimationFinished()
	{
		CampHUD campHUD = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.CampCampMapHud, null, createIfNotExist: false) as CampHUD;
		if (campHUD != null)
		{
			for (int i = 0; i < currencyAmountPanels.Length; i++)
			{
				if (currencyAmountPanels[i].Amount > 0 && currencyAmountPanels[i].CurrencyType != CurrencyType.None)
				{
					campHUD.GetComponent<BuildingsHUD>().CreateCollectAnim(currencyAmountPanels[i].CurrencyType, currencyAmountPanels[i].Icon.gameObject, currencyAmountPanels[i].Amount);
				}
			}
		}
		if (Callback != null)
		{
			Callback();
		}
	}
}
