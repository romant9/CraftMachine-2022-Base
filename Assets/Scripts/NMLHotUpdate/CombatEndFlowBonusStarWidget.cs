using TWDModel;
using UnityEngine;

public class CombatEndFlowBonusStarWidget : CombatEndWidget
{
	[SerializeField]
	private UILabel TextLabel;

	[SerializeField]
	private UISprite CurrencyIconSprite;

	[SerializeField]
	private UILabel CurrencyAmountLabel;

	[SerializeField]
	private GameObject starObj;

	[SerializeField]
	private GameObject apocalypticStarObj;

	public override void Awake()
	{
		base.Awake();
		DebugClassString = "CombatEndFlowBonusStarWidget";
		if (TextLabel != null)
		{
			TextLabel.gameObject.SetActive(value: false);
		}
		if (CurrencyIconSprite != null)
		{
			CurrencyIconSprite.gameObject.SetActive(value: false);
		}
		if (CurrencyAmountLabel != null)
		{
			CurrencyAmountLabel.gameObject.SetActive(value: false);
		}
	}

	public void SetInfo(string content)
	{
		HelpersUI.SetContentToLabel(TextLabel, content);
	}

	public void SetCurrencyData(string amount)
	{
		if (HelpersUI.SetContentToLabel(CurrencyAmountLabel, amount))
		{
			Helpers.GameObjectSetActive(CurrencyIconSprite.gameObject, value: true);
		}
	}

	public void SetStar(MapMissionModel mapMission)
	{
		Helpers.GameObjectSetActive(starObj, value: true);
		Helpers.GameObjectSetActive(apocalypticStarObj, value: false);
		if (mapMission != null && !mapMission.IsInWeeklyChallenge && mapMission.IsInApocalyptiWeeklyChallenge)
		{
			Helpers.GameObjectSetActive(starObj, value: false);
			Helpers.GameObjectSetActive(apocalypticStarObj, value: true);
		}
	}
}
