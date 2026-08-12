using System.Collections;
using TWDModel;
using UnityEngine;

public class BadgeReceivePopup : HUDElement
{
	public enum State
	{
		None = 0,
		WaitForClick = 1,
		ShowResult = 2,
		Complete = 3
	}

	[Header("Tween Group Setting")]
	public int TweenGroupWaitForClick = -1;

	public int TweenGroupShowResult = -1;

	[Header("How long until last state is allowed")]
	[SerializeField]
	private float waitUntilAllowComplete = 0.2f;

	[Header("Bundle Card Related")]
	[SerializeField]
	private UISprite cardBackRaritySprite;

	[SerializeField]
	private GameObject cardParentTarget;

	[SerializeField]
	private GameObject displayEffectNormal;

	[SerializeField]
	private GameObject displayEffectLegendary;

	[Header("Debug Preview Only")]
	public string currentStatePreview = "";

	private SurvivorBadgesIcon badgesCard;

	private bool coroutineBusy;

	private State currentState;

	public static void OpenForBadge(BadgeModel badge)
	{
		BadgeReceivePopup badgeReceivePopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeReceivePopup) as BadgeReceivePopup;
		if (badgeReceivePopup != null)
		{
			badgeReceivePopup.OpenForModel(badge);
		}
	}

	public override void Open()
	{
		base.Open();
		if (cardParentTarget != null && badgesCard == null)
		{
			GameObject prefab = UnityUtils.LoadFromAssetBundle("BadgeCard", "uilistitems") as GameObject;
			badgesCard = Helpers.InstantiateWithComponent<SurvivorBadgesIcon>(prefab, cardParentTarget);
		}
		ChangeState(State.WaitForClick);
	}

	public override void OnClickClose()
	{
		if (currentState == State.Complete)
		{
			base.OnClickClose();
		}
	}

	public override void UpdateUI()
	{
		base.UpdateUI();
		if (badgesCard != null && model != null && model is BadgeModel)
		{
			BadgeModel badgeModel = model as BadgeModel;
			BadgeInfo badgeInfo = new BadgeInfo(badgeModel);
			HelpersUI.SetSprite(cardBackRaritySprite, HelpersGfx.GetEquipmentRaritySprite(badgeInfo.Model.Rarity));
			badgesCard.SetData(badgeInfo);
			badgesCard.UpdateUI();
			if (badgeModel != null)
			{
				if (badgeModel.Rarity >= 4)
				{
					Helpers.GameObjectSetActive(displayEffectNormal, value: false);
					Helpers.GameObjectSetActive(displayEffectLegendary, value: true);
				}
				else
				{
					Helpers.GameObjectSetActive(displayEffectNormal, value: true);
					Helpers.GameObjectSetActive(displayEffectLegendary, value: false);
				}
			}
		}
		if (currentState == State.WaitForClick && TweenGroupWaitForClick != -1)
		{
			TweenManager.PlayTweenGroup(base.gameObject, TweenGroupWaitForClick);
		}
		else if (currentState == State.ShowResult && TweenGroupShowResult != -1)
		{
			TweenManager.PlayTweenGroup(base.gameObject, TweenGroupShowResult);
			if (!coroutineBusy)
			{
				StartCoroutine(ChangeStateAfter(waitUntilAllowComplete, State.Complete));
			}
		}
	}

	public override void Update()
	{
		base.Update();
		if (currentState == State.WaitForClick && (Input.GetMouseButtonDown(0) || Input.GetKeyUp(KeyCode.Escape)))
		{
			ChangeState(State.ShowResult);
		}
	}

	public override void OnBackButtonClicked()
	{
		if (currentState == State.WaitForClick)
		{
			ChangeState(State.ShowResult);
		}
		else
		{
			base.OnBackButtonClicked();
		}
	}

	public override void Close()
	{
		base.Close();
		Clear();
	}

	private void Clear()
	{
		currentState = State.None;
		badgesCard.Clear();
		badgesCard = null;
		coroutineBusy = false;
	}

	private IEnumerator ChangeStateAfter(float seconds, State newState)
	{
		coroutineBusy = true;
		yield return new WaitForSeconds(seconds);
		ChangeState(newState);
		coroutineBusy = false;
	}

	protected void ChangeState(State newState)
	{
		if (currentState != newState)
		{
			switch (newState)
			{
			case State.WaitForClick:
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/iap_reward");
				break;
			case State.ShowResult:
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("global/reward_flip");
				break;
			}
			if (newState > currentState)
			{
				currentState = newState;
				UpdateUI();
			}
			else
			{
				Debug.LogWarning("Cannot NOT go backwards! Current: " + currentState.ToString() + ", New: " + newState);
			}
		}
	}

	public void OnClickBadgeDetailsButton()
	{
		HUDElement hUDElement = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.BadgeDetailsPopup);
		if (!(hUDElement == null))
		{
			hUDElement.OpenForModel(GameManager.Instance.playerModel.LastCraftedBadge);
		}
	}
}
