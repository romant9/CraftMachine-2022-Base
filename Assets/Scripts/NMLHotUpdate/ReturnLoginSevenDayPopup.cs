using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class ReturnLoginSevenDayPopup : MonoBehaviour
{
	[SerializeField]
	private ReturnRewardCard cardTemplate;

	[SerializeField]
	private ReturnRewardCard seventhDayCard;

	private const int DynamicRewardDays = 6;

	private readonly List<ReturnRewardCard> _dynamicRewardCards = new List<ReturnRewardCard>(6);

	private bool _cardsCreated;

	public bool IsCompleted => GetReturnLoginModel()?.IsCompleted ?? false;

	private void OnEnable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
		UIEvent.OnUIEvent += OnUIEvent;
	}

	private void OnDisable()
	{
		UIEvent.OnUIEvent -= OnUIEvent;
	}

	private static ReturnLoginModel GetReturnLoginModel()
	{
		return GameManager.Instance?.playerModel?.ReturnActivityManager?.ReturnLogin;
	}

	private void Awake()
	{
		CreateDynamicCards();
	}

	private void OnUIEvent(string type, object parameter)
	{
		if (type == "ReturnLoginSevenDayClaimEvent")
		{
			Refresh();
		}
	}

	public void Open()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: true);
		Refresh();
	}

	public void Close()
	{
		Helpers.GameObjectSetActive(base.gameObject, value: false);
	}

	private void Refresh()
	{
		CreateDynamicCards();
		if (_dynamicRewardCards.Count == 0)
		{
			return;
		}
		ReturnLoginModel returnLoginModel = GetReturnLoginModel();
		if (returnLoginModel?.RewardDays == null || returnLoginModel.RewardDays.Count == 0)
		{
			for (int i = 0; i < _dynamicRewardCards.Count; i++)
			{
				if (_dynamicRewardCards[i] != null)
				{
					Helpers.GameObjectSetActive(_dynamicRewardCards[i].gameObject, value: false);
				}
			}
			Helpers.GameObjectSetActive((seventhDayCard != null) ? seventhDayCard.gameObject : null, value: false);
			return;
		}
		for (int j = 0; j < _dynamicRewardCards.Count; j++)
		{
			ReturnRewardCard returnRewardCard = _dynamicRewardCards[j];
			if (!(returnRewardCard == null))
			{
				if (j < returnLoginModel.RewardDays.Count)
				{
					Helpers.GameObjectSetActive(returnRewardCard.gameObject, value: true);
					returnRewardCard.Bind(returnLoginModel.RewardDays[j]);
				}
				else
				{
					Helpers.GameObjectSetActive(returnRewardCard.gameObject, value: false);
				}
			}
		}
		if (seventhDayCard != null)
		{
			if (returnLoginModel.RewardDays.Count > 6)
			{
				Helpers.GameObjectSetActive(seventhDayCard.gameObject, value: true);
				seventhDayCard.Bind(returnLoginModel.RewardDays[6]);
			}
			else
			{
				Helpers.GameObjectSetActive(seventhDayCard.gameObject, value: false);
			}
		}
	}

	private void CreateDynamicCards()
	{
		if (_cardsCreated)
		{
			return;
		}
		_cardsCreated = true;
		if (cardTemplate == null)
		{
			return;
		}
		Transform parent = cardTemplate.transform.parent;
		if (parent == null)
		{
			return;
		}
		for (int i = 0; i < 6; i++)
		{
			GameObject obj = Helpers.InstantiateToParent(cardTemplate.gameObject, parent.gameObject);
			obj.transform.localRotation = cardTemplate.transform.localRotation;
			obj.transform.localScale = cardTemplate.transform.localScale;
			ReturnRewardCard component = obj.GetComponent<ReturnRewardCard>();
			if (component != null)
			{
				_dynamicRewardCards.Add(component);
			}
		}
		Helpers.GameObjectSetActive(cardTemplate.gameObject, value: false);
		UITable component2 = parent.GetComponent<UITable>();
		if (component2 != null)
		{
			component2.Reposition();
		}
	}
}
