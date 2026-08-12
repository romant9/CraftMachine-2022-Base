using System.Collections.Generic;
using TWDModel;
using UnityEngine;

public class CarouselPanel : MonoBehaviour
{
	[SerializeField]
	private InteractiveCarousel carousel;

	[SerializeField]
	private GameObject showGo;

	[SerializeField]
	private GameObject showContainer;

	private List<Transform> _itemList = new List<Transform>();

	public void OnEnable()
	{
		ClearItemList();
		List<IActivityManagerIntegrationInterface> list = GameManager.Instance.playerModel?.ActivityIntegrationManager?.GetIntegrationActivityList();
		if (list != null && list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				BroadcastDefinition broadcastDefinition = ((!(list[i] is RouletteActivityDataModel rouletteActivityDataModel)) ? GameManager.Instance.gameEconomyData?.GetBroadcastDefinitionById(list[i].GetIntegrationEventId()) : GameManager.Instance.gameEconomyData?.GetBroadcastDefinitionById(rouletteActivityDataModel.GetIntegrationEventId(), rouletteActivityDataModel.ConfigId));
				if (broadcastDefinition != null)
				{
					AddShowItem(broadcastDefinition, list[i]);
				}
			}
			if (_itemList.Count <= 0)
			{
				base.gameObject.SetActive(value: false);
			}
			else
			{
				carousel.Initialize(_itemList);
			}
		}
		else
		{
			base.gameObject.SetActive(value: false);
		}
	}

	private void AddShowItem(BroadcastDefinition broadcastDefinition, IActivityManagerIntegrationInterface activityData)
	{
		if (broadcastDefinition.HaveBanner)
		{
			GameObject gameObject = showContainer.AddChild(showGo);
			ActivityBanner component = gameObject.GetComponent<ActivityBanner>();
			_itemList.Add(gameObject.transform);
			component.Init(broadcastDefinition.BannerImage, broadcastDefinition.EndTimeMilliseconds, broadcastDefinition.BannerDesc, activityData);
		}
	}

	private void ClearItemList()
	{
		for (int i = 0; i < _itemList.Count; i++)
		{
			NGUITools.Destroy(_itemList[i].gameObject);
		}
		_itemList.Clear();
	}

	public void OnClick()
	{
		if (SingularityMonoBehaviour<HUDManager>.Instance.IsOpen(UIType.ActivityPopup))
		{
			return;
		}
		ActivityPopup activityPopup = SingularityMonoBehaviour<HUDManager>.Instance.Get(UIType.ActivityPopup) as ActivityPopup;
		if (activityPopup != null)
		{
			activityPopup.Open();
			ActivityBanner component = _itemList[carousel.GetCurrentPage()].GetComponent<ActivityBanner>();
			UIEvent.Send("ActivityClickEvent", component.integrationInterface);
			List<IActivityManagerIntegrationInterface> list = GameManager.Instance.playerModel?.ActivityIntegrationManager?.GetIntegrationActivityList();
			if (list != null)
			{
				int index = list.IndexOf(component.integrationInterface);
				activityPopup.ScrollToIndex(index);
			}
			carousel.isSwitching = false;
		}
	}
}
