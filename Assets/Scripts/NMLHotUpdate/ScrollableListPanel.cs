using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollableListPanel<T> : MonoBehaviour where T : class
{
	[SerializeField]
	private UIScrollView.Movement movement;

	[Tooltip("Prefab of a card.")]
	[SerializeField]
	protected GameObject cardPrefab;

	[SerializeField]
	[Tooltip("Space between the cards.")]
	private float cardSpacing;

	[Tooltip("The GameObject that will contain the cards.")]
	[SerializeField]
	protected GameObject cardsContainer;

	[SerializeField]
	[Tooltip("Sort cards in order")]
	private bool sortCards = true;

	[SerializeField]
	private UIGrid gridOverride;

	[SerializeField]
	private UITable tableOverride;

	protected UIScrollView scrollView;

	protected List<UIListCard<T>> cards = new List<UIListCard<T>>();

	private bool allowMultibleResetCalls;

	protected virtual bool LastEntryAtTop => true;

	public UIScrollView.Movement Movement => movement;

	protected virtual void Awake()
	{
		InitScrollView();
		if (!GameManager.Instance.gameEconomyData.GetFeature("MultibleResetCalls").Enabled)
		{
			allowMultibleResetCalls = false;
		}
		else
		{
			allowMultibleResetCalls = true;
		}
	}

	private void InitScrollView()
	{
		scrollView = cardsContainer.GetComponent<UIScrollView>();
		if (scrollView == null)
		{
			scrollView = cardsContainer.transform.parent.GetComponent<UIScrollView>();
		}
	}

	public void SetCards(IEnumerable<T> items, bool resetScrollView = true)
	{
		ClearCards();
		AddCards(items);
		PositionCards(resetScrollView);
	}

	protected void AddCards(IEnumerable<T> items)
	{
		if (items == null)
		{
			return;
		}
		foreach (T item in items)
		{
			AddCard(item);
		}
	}

	protected void RemoveCard(T item)
	{
		foreach (UIListCard<T> listCard in cards)
		{
			if (!(listCard != null) || !(listCard.gameObject != null) || listCard.Item != item)
			{
				continue;
			}
			cards.Remove(listCard);
			TweenManager.PlayTweenGroup(listCard.gameObject, 4, forward: true, delegate
			{
				CacheableObject component = listCard.GetComponent<CacheableObject>();
				if (component != null)
				{
					component.Destroy();
				}
				else
				{
					NGUITools.Destroy(listCard.gameObject);
				}
				PositionCards();
			});
			break;
		}
	}

	protected void AddCard(T item, bool setupInitialPosition = false)
	{
		GameObject gameObject = CreateCard(item);
		UIListCard<T> component = gameObject.GetComponent<UIListCard<T>>();
		component.Item = item;
		SetCard(component);
		component.UpdateUI();
		cards.Add(component);
		if (!setupInitialPosition)
		{
			return;
		}
		Vector3 localPosition = Vector3.zero;
		int num = cards.Count - 2;
		if (num >= 0)
		{
			UIListCard<T> uIListCard = cards[num];
			localPosition = uIListCard.transform.localPosition;
			if (movement == UIScrollView.Movement.Horizontal)
			{
				float num2 = cardSpacing + uIListCard.GetComponent<BoxCollider>().size.x / 2f + component.GetComponent<BoxCollider>().size.x / 2f;
				localPosition.x += (LastEntryAtTop ? num2 : (0f - num2));
			}
			else
			{
				float num3 = 0f;
				num3 = ((!(component.ColliderForBuildingTheList != null)) ? (cardSpacing + uIListCard.GetComponent<BoxCollider>().size.y / 2f + component.GetComponent<BoxCollider>().size.y / 2f) : (cardSpacing + uIListCard.ColliderForBuildingTheList.size.y / 2f + component.ColliderForBuildingTheList.size.y / 2f));
				localPosition.y += (LastEntryAtTop ? num3 : (0f - num3));
			}
		}
		gameObject.transform.localPosition = localPosition;
		TweenManager.PlayTweenGroup(gameObject, 3);
	}

	protected virtual void SetCard(UIListCard<T> card)
	{
	}

	public virtual void ClearCards()
	{
		if (cards == null)
		{
			return;
		}
		bool disableDragIfFits = false;
		if (scrollView != null)
		{
			disableDragIfFits = scrollView.disableDragIfFits;
			scrollView.disableDragIfFits = false;
		}
		for (int i = 0; i < cards.Count; i++)
		{
			if (cards[i] != null && (object)cards[i] != null)
			{
				Helpers.DestroyOrCache(cards[i].gameObject);
			}
		}
		cards.Clear();
		if (scrollView != null)
		{
			scrollView.disableDragIfFits = disableDragIfFits;
		}
	}

	public void PositionCards(bool resetScrollView = true)
	{
		if (cards.Count == 0)
		{
			return;
		}
		if (sortCards)
		{
			Sort();
		}
		if ((bool)gridOverride)
		{
			for (int i = 0; i < cards.Count; i++)
			{
				cards[i].transform.SetSiblingIndex(i);
			}
			gridOverride.Reposition();
		}
		else if ((bool)tableOverride)
		{
			for (int j = 0; j < cards.Count; j++)
			{
				cards[j].transform.SetSiblingIndex(j);
			}
			tableOverride.Reposition();
		}
		else
		{
			Vector3 zero = Vector3.zero;
			for (int k = 0; k < cards.Count; k++)
			{
				if (k != 0)
				{
					if (movement == UIScrollView.Movement.Horizontal)
					{
						float num = cardSpacing + cards[k - 1].GetComponent<BoxCollider>().size.x / 2f + cards[k].GetComponent<BoxCollider>().size.x / 2f;
						zero.x += (LastEntryAtTop ? num : (0f - num));
					}
					else
					{
						float num2 = 0f;
						num2 = ((!(cards[k].ColliderForBuildingTheList != null)) ? (cardSpacing + cards[k - 1].GetComponent<BoxCollider>().size.y / 2f + cards[k].GetComponent<BoxCollider>().size.y / 2f) : (cardSpacing + cards[k - 1].ColliderForBuildingTheList.size.y / 2f + cards[k].ColliderForBuildingTheList.size.y / 2f));
						zero.y += (LastEntryAtTop ? num2 : (0f - num2));
					}
				}
				cards[k].transform.localPosition = zero;
			}
		}
		if (resetScrollView)
		{
			if (scrollView == null)
			{
				InitScrollView();
			}
			scrollView.ResetPosition();
			if (allowMultibleResetCalls && base.gameObject.activeInHierarchy)
			{
				StartCoroutine(DelayedResetPosition());
			}
		}
	}

	private IEnumerator DelayedResetPosition()
	{
		yield return null;
		scrollView.ResetPosition();
		yield return null;
		scrollView.ResetPosition();
	}

	protected virtual void Sort()
	{
		cards.StableSort(delegate(UIListCard<T> a, UIListCard<T> b)
		{
			int sortValue = a.GetSortValue();
			int sortValue2 = b.GetSortValue();
			if (sortValue == sortValue2)
			{
				return 0;
			}
			return (sortValue <= sortValue2) ? 1 : (-1);
		});
	}

	protected virtual GameObject CreateCard(T item)
	{
		return Helpers.InstantiateToParentAndLayer(cardPrefab, cardsContainer);
	}

	public void SelectCard(int index)
	{
		if (cards != null && index < cards.Count)
		{
			UIToggle component = cards[index].GetComponent<UIToggle>();
			if (component != null)
			{
				component.value = true;
			}
		}
	}

	public UIListCard<T> getCardAt(int index)
	{
		if (cards == null || index >= cards.Count)
		{
			return null;
		}
		return cards[index];
	}

	public UIListCard<T> GetCard(T model)
	{
		if (cards == null)
		{
			return null;
		}
		for (int i = 0; i < cards.Count; i++)
		{
			if (cards[i].Item == model)
			{
				return cards[i];
			}
		}
		return null;
	}

	public List<UIListCard<T>> GetCards()
	{
		return cards;
	}



	#region myparams
	private GameObject cardPrefabOrigin;
	#endregion

	#region mycode
	public void ChangeCardType(GameObject go)
	{
		if (go == null)
		{
			if (cardPrefabOrigin != null)
			{
				cardPrefab = cardPrefabOrigin;
				cardPrefabOrigin = null;
			}
		}
		else
		{
			cardPrefabOrigin = cardPrefab;
			cardPrefab = go;
		}
	}

	public void SortOnOff(bool isOn)
	{
		sortCards = isOn;
	}
	#endregion
}
