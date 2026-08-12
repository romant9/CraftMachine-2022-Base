using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINewShopList : MonoBehaviour
{
	[SerializeField]
	private GameObject EntryPrefab;

	[SerializeField]
	private NUIScrollableList scrollableList;

	[SerializeField]
	private UIScrollBar ScrollBar;

	private float ScrollBarValue;

	private List<UINewShopLineData> ItemDatas = new List<UINewShopLineData>();

	public void UpdateListWithData(List<UINewShopLineData> dataList, bool resetScrollPosition)
	{
		if (scrollableList == null)
		{
			scrollableList.Clear();
			return;
		}
		if (dataList == null || dataList.Count <= 0)
		{
			scrollableList.Clear();
			return;
		}
		ScrollBarValue = ScrollBar.value;
		scrollableList.SaveCurrentScrollPosition();
		scrollableList.Clear();
		foreach (UINewShopLineData data in dataList)
		{
			if (data != null && (Helpers.IsPCPlatform() || !data.IsBanner() || CheckBanana()))
			{
				NUIListItem<UINewShopLineData> nUIListItem = scrollableList.InstantiateAdd(EntryPrefab) as NUIListItem<UINewShopLineData>;
				Helpers.GameObjectSetActive(nUIListItem, value: true);
				if (nUIListItem != null)
				{
					nUIListItem.SetData(data);
				}
			}
		}
		ItemDatas = dataList;
		if (resetScrollPosition)
		{
			scrollableList.SortAndReset();
			return;
		}
		scrollableList.SortAndRepositionItems();
		scrollableList.ResetScrollPosition();
		StartCoroutine(SetUpScrollBar());
	}

	private IEnumerator SetUpScrollBar()
	{
		if (ItemDatas.Count <= 2)
		{
			ScrollBarValue = 0f;
			Helpers.GameObjectSetActive(ScrollBar, value: false);
		}
		else
		{
			ScrollBar.value = ScrollBarValue;
			Helpers.GameObjectSetActive(ScrollBar, value: true);
		}
		yield return null;
	}

	private bool CheckBanana()
	{
		bool ingameBanana = Helpers.GetIngameBanana();
		double totalUSDSpent = GameManager.Instance.playerModel.TotalUSDSpent;
		if (ingameBanana && totalUSDSpent > 0.0)
		{
			return true;
		}
		return false;
	}
}
