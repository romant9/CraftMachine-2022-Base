using System.Collections.Generic;
using System.Linq;
using TwdCustomMod;
using TWDModel;
using UnityEngine;

public class TooltipComponentSelector : TooltipBox
{
	public enum FilterType
	{
		All = 0,
		Only = 1,
		Exclude = 2
	}

	[SerializeField]
	private NUIScrollableList scrollableList;

	private int currenctIndex = -1;

	private List<CurrencyType> excludeCurrencies;

	public const string defaultComponentResourcePath = "Component_Icon";

	private List<CurrencyModel> componentList;

	public override void Update()
	{
	}

	public void UpdateWithParams(int index, List<CurrencyType> excludeCurrencies)
	{
		currenctIndex = index;
		this.excludeCurrencies = excludeCurrencies;
	}

	public override void Show()
	{
		if (!(GameManager.Instance != null) || GameManager.Instance.playerModel == null || !(scrollableList != null))
		{
			return;
		}

		if (IsLoadDataManager && scrollableList == null) return;
		base.Show();
		if (currenctIndex == -1)
		{
			return;
		}
		if (currenctIndex == 0)
		{
			GetAllComponentModelsList(GameManager.Instance.playerModel, ref componentList, FilterType.Only, CurrencyType.Badge0);
		}
		else
		{
			GetAllComponentModelsList(GameManager.Instance.playerModel, ref componentList, FilterType.Exclude, CurrencyType.Badge0);
		}
		List<CurrencyModel> list = new List<CurrencyModel>();
		int i;
		for (i = 0; i < componentList.Count; i++)
		{
			CurrencyModel currencyModel = new CurrencyModel(componentList[i].Type);
			currencyModel.SetCapacity(componentList[i].Max);
			currencyModel.SetValue(componentList[i].Value - excludeCurrencies.Count((CurrencyType x) => x == componentList[i].Type));
			list.Add(currencyModel);
		}
		scrollableList.UpdateWithList(list, "Component_Icon", null);
		if (currenctIndex == 0)
		{
			scrollableList.RepositionItemsHorizontal();
		}
		else
		{
			scrollableList.RepositionItemsFillDownwards();
		}
		scrollableList.ResetScrollPosition();
		for (int num = 0; num < scrollableList.currentItemsList.Count; num++)
		{
			if (scrollableList.currentItemsList[num] != null && scrollableList.currentItemsList[num] is ComponentInventoryButton)
			{
				scrollableList.currentItemsList[num].UpdateUI();
				(scrollableList.currentItemsList[num] as ComponentInventoryButton).SetClickCallback(OnClickBadges);
			}
		}
	}

	public override void Hide()
	{
		base.Hide();
		if (scrollableList != null)
		{
			scrollableList.Clear();
		}
		if (componentList != null)
		{
			componentList.Clear();
			componentList = null;
		}
		currenctIndex = -1;
	}

	private void OnClickBadges(UIButtonExtended button)
	{
		if (!(button != null))
		{
			return;
		}
		NUIListItem<CurrencyModel> component = button.GetComponent<NUIListItem<CurrencyModel>>();
		if (component != null)
		{
			if (component.GetData().Value > 0)
			{
				SingularityMonoBehaviour<AudioManager>.Instance.PlayEvent("camp/ui_add_item");
				UIEvent.Send("OnClickComponentSelected", new SelectComponentEvent
				{
					index = currenctIndex,
					model = component.GetData()
				});
			}
			Hide();
		}
	}

	public static void GetAllComponentModelsList(PlayerModel player, ref List<CurrencyModel> listRef, FilterType filterType = FilterType.All, CurrencyType currencyType = CurrencyType.None, int minAmount = 0)
	{
		if (listRef == null)
		{
			listRef = new List<CurrencyModel>();
		}
		else
		{
			listRef.Clear();
		}
		if (listRef.Count != 0)
		{
			return;
		}
		currencyType = ((currencyType != CurrencyType.None) ? ComponentHelper.GetComponentBaseCurrency(currencyType) : CurrencyType.None);
		List<CurrencyType> list = new List<CurrencyType>();
		for (int i = 0; i < ComponentHelper.ComponentCurrencies.Length; i++)
		{
			if (filterType == FilterType.Only && currencyType == ComponentHelper.ComponentCurrencies[i][0])
			{
				list.AddRange(ComponentHelper.ComponentCurrencies[i]);
			}
			else if (filterType == FilterType.Exclude && currencyType != ComponentHelper.ComponentCurrencies[i][0])
			{
				list.AddRange(ComponentHelper.ComponentCurrencies[i]);
			}
			else if (filterType == FilterType.All)
			{
				list.AddRange(ComponentHelper.ComponentCurrencies[i]);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			if (!OfflineManager.IsLoadDataManager)
			{
				if (minAmount <= 0 || player.GetCurrency(list[j]).Value >= minAmount)
				{
					listRef.Add(player.GetCurrency(list[j]));
				}
			}
			else
			{
				CurrencyModel currencyModel;
				if (CraftSettings.Instance != null && !CraftSettings.Instance.IsRealPlayerData)
				{
					CraftSettings craftSettings = CraftSettings.Instance;

					currencyModel = craftSettings.Currency.FirstOrDefault(x => x.Type == list[j]);
					if (currencyModel == null)
					{
						currencyModel = new CurrencyModel(list[j]);
						currencyModel.SetValue(craftSettings.CurrencyCountMax);
						craftSettings.Currency.Add(currencyModel);
					}
				}
				else
				{
					currencyModel = player.GetCurrency(list[j]);
				}

				if (minAmount <= 0 || currencyModel.Value >= minAmount)
				{
					listRef.Add(currencyModel);
				}
			}
		}
	}



	#region myparams
	private bool IsClickOver = false;
	private bool IsLoadDataManager => OfflineManager.IsLoadDataManager;
	#endregion
}
