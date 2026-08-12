using UnityEngine;

public class ScrollableWidgetListPanel<T> : ScrollableListPanel<string> where T : ListWidgetBase
{
	public ListWidgetBase InstantiateItemToList(GameObject prefab)
	{
		if (prefab != null)
		{
			GameObject gameObject = Helpers.InstantiateToParent(prefab, cardsContainer);
			if (gameObject != null)
			{
				T component = gameObject.GetComponent<T>();
				if (component != null)
				{
					SetCard(component);
					cards.Add(component);
					component.AddedToList();
					return component;
				}
				Debug.LogWarning("ScrollableWidgetListPanel: No component of type " + typeof(T).ToString() + " in " + prefab.name);
			}
			else
			{
				Debug.LogWarning("ScrollableWidgetListPanel: Could not instantiate " + prefab);
			}
		}
		else
		{
			Debug.LogWarning("ScrollableWidgetListPanel: Given prefab is NULL");
		}
		return null;
	}

	public Vector2 CalculateContentSize()
	{
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < cards.Count; i++)
		{
			if (cards[i] != null)
			{
				if (base.Movement == UIScrollView.Movement.Vertical)
				{
					zero.y += cards[i].GetComponent<BoxCollider>().size.y;
				}
				else
				{
					zero.y = Mathf.Max(zero.y, cards[i].GetComponent<BoxCollider>().size.y);
				}
				if (base.Movement == UIScrollView.Movement.Horizontal)
				{
					zero.x += cards[i].GetComponent<BoxCollider>().size.x;
				}
				else
				{
					zero.x = Mathf.Max(zero.y, cards[i].GetComponent<BoxCollider>().size.x);
				}
			}
		}
		return zero;
	}

	public void Position()
	{
		PositionCards();
	}
}
